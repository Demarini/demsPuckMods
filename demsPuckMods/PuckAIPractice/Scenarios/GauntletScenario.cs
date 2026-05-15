using System.Collections.Generic;
using System.Linq;
using PuckAIPractice.Chaser;
using PuckAIPractice.Defender;
using PuckAIPractice.GameModes;
using PuckAIPractice.Scenarios.Environment;
using PuckAIPractice.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace PuckAIPractice.Scenarios
{
    // Gauntlet scenario:
    //   /scenario gauntlet            -> empty arena, no bots, no pressure wall
    //                                    (open practice)
    //   /scenario gauntlet <rate>     -> chaser spawns every <rate> meters of
    //                                    forward progress, plus a pressure
    //                                    wall closing in from behind. Each
    //                                    bot pass snaps the wall a few meters
    //                                    behind the player; touching the wall
    //                                    resets the run.
    //
    // Bots are recycled to a far-off-arena standby position rather than
    // despawned, so we keep a small pool and avoid spawn churn.
    public class GauntletScenario : IScenario
    {
        public string Name => _isChallenge ? "gauntletChallenge" : "gauntlet";

        private readonly bool _isChallenge;

        public GauntletScenario(bool isChallenge)
        {
            _isChallenge = isChallenge;
        }

        // --- Tunables ---
        private const float PuckSpawnAheadOfPlayer = 1.5f;
        // How far ahead of the player a fresh bot appears in Z. Enough runway
        // for the bot to build chase speed before contact.
        private const float BotSpawnAheadDistance = 50f;
        // Once a bot's Z is this far behind the player, recycle to standby.
        private const float BotPassedThreshold = 5f;
        // Horizontal spawn cannot land within this much of either wall.
        private const float SpawnXWallMargin = 2f;
        // Side-step offset relative to player when biasing the open side.
        private const float MinSideOffset = 2f;
        private const float MaxSideOffset = 6f;
        // Don't spawn within this many meters of the previous spawn X.
        private const float MinSpawnXSeparation = 3f;
        // Standby position for recycled bots: well past the front wall and
        // high up so they sit out of sight and can't reach the player.
        private const float StandbyZ = RinkVoid.ForwardLength + 200f;
        private const float StandbyY = 500f;

        // Pressure wall.
        private const float WallStartBehindPlayer = 20f;
        // Constant forward speed. Skating tops out near 8 m/s so 5 m/s gives
        // the player room to escape on clean lines but punishes any stall.
        private const float WallApproachSpeed = 6f;
        // After each bot pass, wall snaps to this distance behind the player.
        private const float WallTeleportBehindOnPass = 7f;
        // Margin used for the "wall touched player" loss check.
        private const float WallLossMargin = 0.5f;
        // Wall is taller than the containment so a deflected puck can't fly
        // over it either, and 2m thick so it reads as a solid threat.
        private const float WallHeight = 200f;
        private const float WallThickness = 2f;
        // Seconds the "CAUGHT!" banner stays up after a loss.
        private const float LossBannerSeconds = 2f;

        // --- HUD-readable state (server-set, read by GauntletHud.OnGUI) ---
        public static bool ShowHud;
        public static int PassedBots;
        public static float WallDistance;
        public static float LossBannerSecondsRemaining;
        // Captured at OnLoss so the CAUGHT banner can show what the run ended on.
        public static int LastFinalScore;

        private class BotEntry
        {
            public Player Player;
            public bool Active;
        }

        private RinkVoid _void;
        private float _spawnRate;
        private ulong _callerClientId;
        private float _iceY;
        private float _nextSpawnZ;
        private float _lastSpawnX;
        private bool _hasLastSpawnX;
        private int _spawnCounter;
        private readonly List<BotEntry> _bots = new List<BotEntry>();

        private GameObject _pressureWall;
        private float _wallZ;
        private bool _wallEnabled;

        public void Start(ulong callerClientId, string[] args)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                Debug.LogWarning("[Scenario:gauntlet] Not server, ignoring");
                return;
            }

            var caller = PlayerManager.Instance?.GetPlayerByClientId(callerClientId);
            if (caller == null || caller.PlayerBody == null)
            {
                Debug.LogWarning($"[Scenario:gauntlet] Caller clientId={callerClientId} not spawned");
                return;
            }

            _callerClientId = callerClientId;
            _spawnRate = ParseRate(args);
            _iceY = FindIceY(caller.Team);

            BotSpawning.SuppressAutoGoalieSpawn = true;
            Goalies.EndGoalieSession(GoalieSession.Both);
            ChaserSpawner.DespawnAll();
            DefenderSpawner.DespawnAll();
            PuckManager.Instance?.Server_DespawnPucks(false);

            _bots.Clear();
            _hasLastSpawnX = false;
            _spawnCounter = 0;
            PassedBots = 0;
            LossBannerSecondsRemaining = 0f;

            _void = new RinkVoid();
            _void.Enter(_iceY);

            TeleportPlayerToStart(caller);
            SpawnFreshPuck();

            // Wall only exists in challenge mode and only when there are bots
            // to pass (rate 0 has nothing to drive the wall mechanic).
            _wallEnabled = _isChallenge && _spawnRate > 0f;
            if (_wallEnabled)
            {
                CreatePressureWall();
                ResetWallToStart();
            }

            ShowHud = _wallEnabled;
            _nextSpawnZ = (_spawnRate > 0f) ? _spawnRate : float.PositiveInfinity;

            Debug.Log($"[Scenario:gauntlet] caller={callerClientId} iceY={_iceY:F3} spawnRate={_spawnRate}m " +
                      (_spawnRate > 0f ? $"firstSpawnAtZ={_nextSpawnZ:F1}" : "(no bots)"));
        }

        public void Tick(float dt)
        {
            if (LossBannerSecondsRemaining > 0f)
            {
                LossBannerSecondsRemaining -= dt;
                if (LossBannerSecondsRemaining < 0f) LossBannerSecondsRemaining = 0f;
            }

            if (_spawnRate <= 0f) return;

            var caller = PlayerManager.Instance?.GetPlayerByClientId(_callerClientId);
            if (caller == null || caller.PlayerBody == null) return;

            Vector3 playerPos = caller.PlayerBody.transform.position;

            RecyclePassedBots(playerPos.z);

            if (_wallEnabled)
            {
                _wallZ += WallApproachSpeed * dt;
                ApplyWallTransform();
                // Wall's leading edge is half its thickness ahead of center.
                // The wall physically pushes the player forward on contact, so
                // playerZ stays slightly ahead of the front face — comparing
                // against center never triggers because the gap stays at
                // ~WallThickness/2. Use the front face for the loss check.
                float wallFrontZ = _wallZ + WallThickness * 0.5f;
                WallDistance = playerPos.z - wallFrontZ;

                // Any bot behind the wall gets swept to standby; without this
                // they get pushed forward by the wall and end up sitting on
                // the player's back.
                RecycleBotsBehindWall(wallFrontZ);

                if (wallFrontZ >= playerPos.z - WallLossMargin || PuckHitWall(wallFrontZ))
                {
                    OnLoss(caller);
                    return;
                }
            }

            if (playerPos.z >= _nextSpawnZ)
            {
                SpawnNextBot(playerPos, caller.Team);
                _nextSpawnZ = playerPos.z + _spawnRate;
            }
        }

        public void Stop()
        {
            ChaserSpawner.DespawnAll();
            _bots.Clear();

            if (_pressureWall != null)
            {
                UnityEngine.Object.Destroy(_pressureWall);
                _pressureWall = null;
            }
            _wallEnabled = false;

            _void?.Exit();
            _void = null;

            BotSpawning.SuppressAutoGoalieSpawn = false;
            ShowHud = false;
            LossBannerSecondsRemaining = 0f;
        }

        private void RecyclePassedBots(float playerZ)
        {
            bool anyPassed = false;
            for (int i = _bots.Count - 1; i >= 0; i--)
            {
                var entry = _bots[i];
                if (!entry.Active) continue;
                if (entry.Player == null || entry.Player.PlayerBody == null)
                {
                    _bots.RemoveAt(i);
                    continue;
                }

                float botZ = entry.Player.PlayerBody.transform.position.z;
                if (botZ + BotPassedThreshold >= playerZ) continue;

                SendBotToStandby(entry, i);
                anyPassed = true;
                PassedBots++;
                Debug.Log($"[Scenario:gauntlet] Pass #{PassedBots} (botZ={botZ:F1}, playerZ={playerZ:F1})");
            }

            if (anyPassed && _wallEnabled)
            {
                _wallZ = playerZ - WallTeleportBehindOnPass;
                ApplyWallTransform();
            }
        }

        private void RecycleBotsBehindWall(float wallFrontZ)
        {
            for (int i = _bots.Count - 1; i >= 0; i--)
            {
                var entry = _bots[i];
                if (!entry.Active) continue;
                if (entry.Player == null || entry.Player.PlayerBody == null)
                {
                    _bots.RemoveAt(i);
                    continue;
                }
                if (entry.Player.PlayerBody.transform.position.z >= wallFrontZ) continue;

                SendBotToStandby(entry, i);
                Debug.Log($"[Scenario:gauntlet] Bot swept by wall (wallFrontZ={wallFrontZ:F1})");
            }
        }

        private bool PuckHitWall(float wallFrontZ)
        {
            var pucks = PuckManager.Instance?.GetPucks();
            if (pucks == null) return false;
            foreach (var p in pucks)
            {
                if (p == null) continue;
                if (wallFrontZ >= p.transform.position.z - WallLossMargin) return true;
            }
            return false;
        }

        private void SendBotToStandby(BotEntry entry, int index)
        {
            float standbyX = (index % 2 == 0 ? -1f : 1f) * (3f + (index % 5));
            entry.Player.PlayerBody.Server_Teleport(
                new Vector3(standbyX, StandbyY, StandbyZ),
                Quaternion.identity);
            entry.Active = false;
        }

        private void SpawnNextBot(Vector3 playerPos, PlayerTeam callerTeam)
        {
            float spawnX = PickSpawnX(playerPos.x);
            float spawnZ = playerPos.z + BotSpawnAheadDistance;
            Vector3 spawnPos = new Vector3(spawnX, _iceY, spawnZ);
            Quaternion spawnRot = Quaternion.LookRotation(Vector3.back, Vector3.up);
            PlayerTeam botTeam = (callerTeam == PlayerTeam.Red) ? PlayerTeam.Blue : PlayerTeam.Red;

            var standby = _bots.FirstOrDefault(b => !b.Active && b.Player != null && b.Player.PlayerBody != null);
            if (standby != null)
            {
                standby.Player.PlayerBody.Server_Teleport(spawnPos, spawnRot);
                // Clear any HasFallen the bot might be carrying from its
                // previous active cycle (e.g. knocked over during contact).
                standby.Player.PlayerBody.OnStandUp();
                standby.Active = true;
                Debug.Log($"[Scenario:gauntlet] Reactivated standby bot at x={spawnX:F1} z={spawnZ:F1}");
            }
            else
            {
                var bot = ChaserSpawner.SpawnAtWorld(botTeam, spawnPos, spawnRot, _callerClientId, $"Gauntlet{++_spawnCounter}");
                if (bot != null)
                {
                    _bots.Add(new BotEntry { Player = bot, Active = true });
                    Debug.Log($"[Scenario:gauntlet] Spawned new bot at x={spawnX:F1} z={spawnZ:F1} (pool={_bots.Count})");
                }
            }

            _lastSpawnX = spawnX;
            _hasLastSpawnX = true;
        }

        private float PickSpawnX(float playerX)
        {
            float minX = -(RinkVoid.ArenaWidth * 0.5f - SpawnXWallMargin);
            float maxX = (RinkVoid.ArenaWidth * 0.5f - SpawnXWallMargin);

            const float CenterDeadband = 0.5f;
            float moreRoomSide = (playerX < -CenterDeadband) ? 1f
                               : (playerX > CenterDeadband) ? -1f
                               : 0f;

            float x;
            if (moreRoomSide == 0f)
            {
                x = Random.Range(minX, maxX);
            }
            else
            {
                float offset = Random.Range(MinSideOffset, MaxSideOffset);
                x = playerX + moreRoomSide * offset;
            }

            if (_hasLastSpawnX && Mathf.Abs(x - _lastSpawnX) < MinSpawnXSeparation)
            {
                float sign = (x >= _lastSpawnX) ? 1f : -1f;
                x = _lastSpawnX + MinSpawnXSeparation * sign;
            }

            return Mathf.Clamp(x, minX, maxX);
        }

        private void OnLoss(Player caller)
        {
            Debug.Log($"[Scenario:gauntlet] CAUGHT after {PassedBots} pass(es). Resetting.");
            LastFinalScore = PassedBots;
            LossBannerSecondsRemaining = LossBannerSeconds;
            PassedBots = 0;
            _spawnCounter = 0;
            _hasLastSpawnX = false;
            _nextSpawnZ = _spawnRate;

            // Recycle every active bot to standby so they aren't sitting on
            // top of the player at reset.
            for (int i = _bots.Count - 1; i >= 0; i--)
            {
                var entry = _bots[i];
                if (entry.Player == null || entry.Player.PlayerBody == null)
                {
                    _bots.RemoveAt(i);
                    continue;
                }
                if (!entry.Active) continue;
                SendBotToStandby(entry, i);
            }

            TeleportPlayerToStart(caller);
            SpawnFreshPuck();
            ResetWallToStart();
        }

        private void TeleportPlayerToStart(Player caller)
        {
            if (caller?.PlayerBody == null) return;
            Vector3 startPos = new Vector3(0f, _iceY, 0f);
            caller.PlayerBody.Server_Teleport(startPos, Quaternion.LookRotation(Vector3.forward, Vector3.up));
        }

        private void SpawnFreshPuck()
        {
            PuckManager.Instance?.Server_DespawnPucks(false);
            Vector3 puckPos = new Vector3(0f, _iceY, PuckSpawnAheadOfPlayer);
            PuckManager.Instance?.Server_SpawnPuck(puckPos, Quaternion.identity, false);
        }

        private void CreatePressureWall()
        {
            if (_pressureWall != null) return;
            _pressureWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _pressureWall.name = "PuckAIPractice_GauntletPressureWall";
            _pressureWall.transform.localScale = new Vector3(RinkVoid.ArenaWidth + 2f, WallHeight, WallThickness);

            int iceLayer = LayerMask.NameToLayer("Ice");
            if (iceLayer >= 0) _pressureWall.layer = iceLayer;

            var renderer = _pressureWall.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit")
                    ?? Shader.Find("Universal Render Pipeline/Simple Lit");
                if (shader != null)
                {
                    var mat = new Material(shader);
                    var red = new Color(0.9f, 0.15f, 0.15f);
                    if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", red);
                    if (mat.HasProperty("_Color")) mat.SetColor("_Color", red);
                    renderer.material = mat;
                }
            }
        }

        private void ResetWallToStart()
        {
            _wallZ = 0f - WallStartBehindPlayer;
            ApplyWallTransform();
            WallDistance = WallStartBehindPlayer;
        }

        private void ApplyWallTransform()
        {
            if (_pressureWall == null) return;
            _pressureWall.transform.position = new Vector3(0f, _iceY + WallHeight * 0.5f, _wallZ);
        }

        private static float ParseRate(string[] args)
        {
            if (args == null || args.Length == 0) return 0f;
            if (!float.TryParse(args[0], out float rate))
            {
                Debug.LogWarning($"[Scenario:gauntlet] Could not parse rate '{args[0]}'; defaulting to 0 (no bots)");
                return 0f;
            }
            if (rate < 0f)
            {
                Debug.LogWarning($"[Scenario:gauntlet] Negative rate {rate}; clamping to 0");
                return 0f;
            }
            return rate;
        }

        private static float FindIceY(PlayerTeam team)
        {
            var positions = Object.FindObjectsOfType<PlayerPosition>();
            var preferred = positions.FirstOrDefault(p => p.Team == team && p.Role == PlayerRole.Goalie);
            if (preferred != null) return preferred.transform.position.y;
            var any = positions.FirstOrDefault();
            return any != null ? any.transform.position.y : 0f;
        }
    }
}
