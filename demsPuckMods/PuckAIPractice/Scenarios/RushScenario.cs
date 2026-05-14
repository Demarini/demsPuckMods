using System.Linq;
using PuckAIPractice.Chaser;
using PuckAIPractice.Defender;
using Unity.Netcode;
using UnityEngine;

namespace PuckAIPractice.Scenarios
{
    public class RushScenario : IScenario
    {
        public string Name => "rush";

        // ~2.5m ahead of the goalie so the player has a sliver of runway before
        // they have to choose a lane.
        private const float PlayerSpawnAheadOfGoalie = 2.5f;
        // Puck sits just in front of the player's stick reach.
        private const float PuckSpawnAheadOfPlayer = 1.5f;
        private static readonly string[] DefenderPositions = { "LD", "RD", "C", "LW", "RW" };

        public void Start(ulong callerClientId)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                Debug.LogWarning("[Scenario:rush] Not server, ignoring");
                return;
            }

            var caller = PlayerManager.Instance.GetPlayerByClientId(callerClientId);
            if (caller == null || caller.PlayerBody == null)
            {
                Debug.LogWarning($"[Scenario:rush] Caller clientId={callerClientId} not spawned");
                return;
            }
            var callerTeam = caller.Team;
            if (callerTeam != PlayerTeam.Red && callerTeam != PlayerTeam.Blue)
            {
                Debug.LogWarning($"[Scenario:rush] Caller team must be Red or Blue (got {callerTeam})");
                return;
            }
            var botTeam = (callerTeam == PlayerTeam.Red) ? PlayerTeam.Blue : PlayerTeam.Red;

            var goalie = FindGoaliePosition(callerTeam);
            if (goalie == null)
            {
                Debug.LogWarning($"[Scenario:rush] No goalie PlayerPosition on team {callerTeam}");
                return;
            }

            // Scenario owns the bot population — clear any prior bots first.
            ChaserSpawner.DespawnAll();
            DefenderSpawner.DespawnAll();
            PuckManager.Instance?.Server_DespawnPucks(false);

            // Attack direction for caller's team = away from own goal along the rink length axis.
            // Each team is on its own side, so sign(goalieWorld.z) points to "own end".
            Vector3 goalieWorld = goalie.transform.position;
            Vector3 attackDir = (Mathf.Abs(goalieWorld.z) > 0.1f)
                ? new Vector3(0f, 0f, -Mathf.Sign(goalieWorld.z))
                : Vector3.forward;

            Vector3 playerPos = goalieWorld + attackDir * PlayerSpawnAheadOfGoalie;
            Quaternion playerRot = Quaternion.LookRotation(attackDir, Vector3.up);
            caller.PlayerBody.Server_Teleport(playerPos, playerRot);

            Vector3 puckPos = playerPos + attackDir * PuckSpawnAheadOfPlayer;
            // Match goalie spawn Y; puck has its own thickness and gravity will settle it.
            puckPos.y = goalieWorld.y;
            PuckManager.Instance?.Server_SpawnPuck(puckPos, Quaternion.identity, false);

            int spawned = 0;
            foreach (var pos in DefenderPositions)
            {
                var bot = DefenderSpawner.Spawn(botTeam, pos, callerClientId);
                if (bot == null) continue;
                spawned++;
                var ai = bot.GetComponent<DefenderAI>();
                if (ai != null && bot.PlayerBody != null)
                {
                    var threat = ai.ThreatDirection.sqrMagnitude > 0.001f
                        ? ai.ThreatDirection
                        : -attackDir;
                    bot.PlayerBody.Server_Teleport(ai.StaticHome, Quaternion.LookRotation(threat, Vector3.up));
                }
            }

            Debug.Log($"[Scenario:rush] caller={callerClientId} team={callerTeam} botTeam={botTeam} " +
                      $"playerPos={playerPos} puckPos={puckPos} defenders={spawned}/{DefenderPositions.Length}");
        }

        public void Stop()
        {
            DefenderSpawner.DespawnAll();
            ChaserSpawner.DespawnAll();
        }

        private static PlayerPosition FindGoaliePosition(PlayerTeam team)
        {
            return Object.FindObjectsOfType<PlayerPosition>()
                .FirstOrDefault(p => p.Team == team && p.Role == PlayerRole.Goalie);
        }
    }
}
