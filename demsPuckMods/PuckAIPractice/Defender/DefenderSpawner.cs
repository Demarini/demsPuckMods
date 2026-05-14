using HarmonyLib;
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
            player.Username.Value = $"demBot_{positionName}";
            player.Number.Value = 99;
            player.CustomizationState.Value = new PlayerCustomizationState();
            player.Server_SetGameState(team: team, role: PlayerRole.Attacker);

            position.Server_Claim(player);

            player.Server_SpawnCharacter(
                position.transform.position,
                position.transform.rotation,
                PlayerRole.Attacker);

            ApplyDefaultAppearance(player);

            var ai = player.gameObject.AddComponent<DefenderAI>();
            ai.ControlledPlayer = player;
            ai.TargetClientId = targetClientId;

            DefenderRegistry.Register(new DefenderRegistry.Entry
            {
                Player = player,
                Team = team,
                PositionName = positionName,
                ClientId = clientId,
            });

            Debug.Log($"[Defender] Spawned at {team} {positionName} (clientId={clientId})");
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

        private static void ApplyDefaultAppearance(Player player)
        {
            var body = player.PlayerBody;
            if (body == null) return;

            var mesh = body.PlayerMesh;
            if (mesh != null)
            {
                mesh.SetJerseyID(0, player.Team);
                mesh.SetNumber(player.Number.Value.ToString());
                mesh.SetUsername(player.Username.Value.ToString());
                if (mesh.PlayerHead != null)
                {
                    mesh.PlayerHead.SetMustacheID(0);
                    mesh.PlayerHead.SetFlagID(0);
                    mesh.PlayerHead.SetHeadgearID(0, player.Role);
                    mesh.PlayerHead.SetBeardID(0);
                }
            }

            if (body.Stick != null && body.Stick.StickMesh != null)
            {
                var stickMesh = body.Stick.StickMesh;
                stickMesh.SetBladeTapeID(0);
                stickMesh.SetSkinID(0, player.Team);
                stickMesh.SetShaftTapeID(0);
            }
        }
    }
}
