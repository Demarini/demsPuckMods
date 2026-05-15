using HarmonyLib;
using PuckAIPractice.Singletons;
using PuckAIPractice.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace PuckAIPractice.Defender
{
    public static class DefenderSpawner
    {
        public static Player Spawn(PlayerTeam team, string positionName, ulong targetClientId)
        {
            if (!NetworkManager.Singleton.IsServer) return null;

            var playerManager = PlayerManager.Instance;
            if (playerManager == null)
            {
                Debug.LogWarning("[Defender] PlayerManager not ready");
                return null;
            }

            var position = FindPosition(team, positionName);
            if (position == null)
            {
                Debug.LogWarning($"[Defender] No PlayerPosition named '{positionName}' on team {team}");
                return null;
            }
            if (position.IsClaimed)
            {
                Debug.LogWarning($"[Defender] {team} {positionName} already claimed by {position.ClaimedByPlayer?.Username.Value}");
                return null;
            }

            var prefab = Traverse.Create(playerManager).Field("playerPrefab").GetValue<Player>();
            if (prefab == null)
            {
                Debug.LogError("[Defender] playerPrefab missing");
                return null;
            }

            var playerObj = UnityEngine.Object.Instantiate(prefab);
            var netObj = playerObj.GetComponent<NetworkObject>();
            var clientId = DefenderRegistry.AllocateClientId();

            netObj.SpawnWithOwnership(clientId, true);

            var player = playerObj.GetComponent<Player>();
            player.Username.Value = $"demDef_{positionName}";
            player.Number.Value = 99;
            player.CustomizationState.Value = BotCustomization.BuildFromSettings();
            player.Server_SetGameState(team: team, role: PlayerRole.Attacker);

            position.Server_Claim(player);

            player.Server_SpawnCharacter(
                position.transform.position,
                position.transform.rotation,
                PlayerRole.Attacker);

            if (ConfigData.Instance.RandomizeBotAppearance)
            {
                player.CustomizationState.Value = BotCustomization.BuildRandom(player);
            }

            var ai = player.gameObject.AddComponent<DefenderAI>();
            ai.ControlledPlayer = player;
            ai.TargetClientId = targetClientId;

            // Per-position defaults for home offset and zone radius. Defensemen sit
            // deeper (closer to own net, near the slot); forwards sit at the blue line;
            // C is centered. Wings tuck the same as defensemen so adjacent positions'
            // zones overlap with a clean margin.
            //
            // ZoneRadius is also per-position: LD/RD get a wider zone (20m) so they
            // engage simultaneously with the forward line when an attacker enters the
            // C's zone. Without the wider D-zone, the corner defenders only react once
            // the attacker has already beaten the forwards — leaving the player
            // facing them one at a time. Forwards keep the default 16m.
            string posUpper = positionName.ToUpperInvariant();
            switch (posUpper)
            {
                case "LD":
                case "RD":
                    ai.HomePullBackDistance = 18f;
                    ai.HomeTuckInDistance = 4f;
                    ai.ZoneRadius = 25f;
                    break;
                case "LW":
                case "RW":
                    ai.HomePullBackDistance = 15f;
                    ai.HomeTuckInDistance = 4f;
                    ai.ZoneRadius = 16f;
                    break;
                case "C":
                    ai.HomePullBackDistance = 15f;
                    ai.HomeTuckInDistance = 0f;
                    ai.ZoneRadius = 17f;
                    break;
            }

            // Use rink-axis-aligned directions, NOT position.transform.forward.
            // Puck's PlayerPosition transforms face the faceoff dot at an angle (e.g.
            // LD's forward is (0.57, 0, 0.82), not pure +Z), so pulling back along
            // -spawnForward drags the bot diagonally outward toward the boards.
            //
            // Assumptions about Puck's rink (consistent with observed spawn coords):
            //   - Lateral axis = world X (rink width). Centerline at X=0.
            //   - Length axis = world Z (rink length).
            //   - Each team spawns on its own side, so sign(spawnPos.z) tells us
            //     which Z direction is "toward own goal" (= backward).
            Vector3 spawnPos = position.transform.position;

            Vector3 backwardDir;
            if (Mathf.Abs(spawnPos.z) > 0.1f)
            {
                backwardDir = new Vector3(0f, 0f, Mathf.Sign(spawnPos.z));
            }
            else
            {
                // C spawns ~Z=0; fall back to a teammate's position to determine direction.
                var reference = FindPosition(team, "LD") ?? FindPosition(team, "RD")
                              ?? FindPosition(team, "LW") ?? FindPosition(team, "RW");
                if (reference != null && Mathf.Abs(reference.transform.position.z) > 0.1f)
                {
                    backwardDir = new Vector3(0f, 0f, Mathf.Sign(reference.transform.position.z));
                }
                else
                {
                    backwardDir = Vector3.back;
                }
            }
            Vector3 attackDir = -backwardDir;

            // Inward direction = toward X=0 lateral centerline.
            Vector3 inwardDir = Vector3.zero;
            if (Mathf.Abs(spawnPos.x) > 0.1f)
            {
                inwardDir = new Vector3(-Mathf.Sign(spawnPos.x), 0f, 0f);
            }

            // Angled patrol axis: mostly inward, with a 25° tilt toward the attack
            // direction so the patrol stroke funnels attackers toward the center.
            const float patrolAxisAngleDeg = 25f;
            float angleRad = patrolAxisAngleDeg * Mathf.Deg2Rad;
            Vector3 angledAxis = (inwardDir.sqrMagnitude > 0.0001f)
                ? (inwardDir * Mathf.Cos(angleRad) + attackDir * Mathf.Sin(angleRad)).normalized
                : Vector3.right;  // C: no inward direction, use pure lateral X

            ai.StaticHome = spawnPos
                + backwardDir * ai.HomePullBackDistance
                + inwardDir * ai.HomeTuckInDistance;
            ai.PatrolAxis = angledAxis;
            ai.ThreatDirection = attackDir;

            DefenderRegistry.Register(new DefenderRegistry.Entry
            {
                Player = player,
                Team = team,
                PositionName = positionName,
                ClientId = clientId,
            });

            Debug.Log($"[Defender] Spawned at {team} {positionName} (clientId={clientId}, spawnPos={spawnPos}, backward={backwardDir}, inward={inwardDir}, axis={ai.PatrolAxis}, staticHome={ai.StaticHome})");
            return player;
        }

        public static int DespawnAll()
        {
            DefenderRegistry.CleanupDestroyed();
            var entries = DefenderRegistry.All.ToList();
            int count = 0;
            foreach (var e in entries)
            {
                if (DespawnEntry(e)) count++;
            }
            Debug.Log($"[Defender] Cleared {count} defender bot(s)");
            return count;
        }

        public static bool DespawnAt(string positionName)
        {
            DefenderRegistry.CleanupDestroyed();
            var entry = DefenderRegistry.All
                .FirstOrDefault(e => string.Equals(e.PositionName, positionName, StringComparison.OrdinalIgnoreCase));
            if (entry == null)
            {
                Debug.Log($"[Defender] No defender bot at position {positionName}");
                return false;
            }
            return DespawnEntry(entry);
        }

        private static bool DespawnEntry(DefenderRegistry.Entry e)
        {
            if (!NetworkManager.Singleton.IsServer) return false;
            try
            {
                if (e.Player != null)
                {
                    var pos = FindPosition(e.Team, e.PositionName);
                    if (pos != null && pos.ClaimedByPlayer == e.Player)
                    {
                        pos.Server_Unclaim();
                    }
                    e.Player.Server_DespawnCharacter();
                    e.Player.Server_SetGameState(team: PlayerTeam.None);
                    PlayerManager.Instance.RemovePlayer(e.Player);
                    e.Player.NetworkObject.Despawn(true);
                }
                DefenderRegistry.Unregister(e.ClientId);
                Debug.Log($"[Defender] Despawned at {e.Team} {e.PositionName}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Defender] Despawn error: {ex.Message}");
                DefenderRegistry.Unregister(e.ClientId);
                return false;
            }
        }

        private static PlayerPosition FindPosition(PlayerTeam team, string name)
        {
            return UnityEngine.Object.FindObjectsOfType<PlayerPosition>()
                .FirstOrDefault(p =>
                    p.Team == team &&
                    string.Equals(p.Name, name, System.StringComparison.OrdinalIgnoreCase));
        }

    }
}
