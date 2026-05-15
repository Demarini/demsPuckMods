using HarmonyLib;
using PuckAIPractice.Singletons;
using PuckAIPractice.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace PuckAIPractice.Chaser
{
    public static class ChaserSpawner
    {
        public static Player Spawn(PlayerTeam team, string positionName, ulong targetClientId)
        {
            if (!NetworkManager.Singleton.IsServer) return null;

            var playerManager = PlayerManager.Instance;
            if (playerManager == null)
            {
                Debug.LogWarning("[Chaser] PlayerManager not ready");
                return null;
            }

            var position = FindPosition(team, positionName);
            if (position == null)
            {
                Debug.LogWarning($"[Chaser] No PlayerPosition named '{positionName}' on team {team}");
                return null;
            }
            if (position.IsClaimed)
            {
                Debug.LogWarning($"[Chaser] {team} {positionName} already claimed by {position.ClaimedByPlayer?.Username.Value}");
                return null;
            }

            var prefab = Traverse.Create(playerManager).Field("playerPrefab").GetValue<Player>();
            if (prefab == null)
            {
                Debug.LogError("[Chaser] playerPrefab missing");
                return null;
            }

            var playerObj = UnityEngine.Object.Instantiate(prefab);
            var netObj = playerObj.GetComponent<NetworkObject>();
            var clientId = ChaserRegistry.AllocateClientId();

            netObj.SpawnWithOwnership(clientId, true);

            var player = playerObj.GetComponent<Player>();
            player.Username.Value = $"demBot_{positionName}";
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

            var ai = player.gameObject.AddComponent<ChaserAI>();
            ai.ControlledPlayer = player;
            ai.TargetClientId = targetClientId;

            ChaserRegistry.Register(new ChaserRegistry.Entry
            {
                Player = player,
                Team = team,
                PositionName = positionName,
                ClientId = clientId,
            });

            Debug.Log($"[Chaser] Spawned at {team} {positionName} (clientId={clientId})");
            return player;
        }

        // Spawn a chaser at an arbitrary world position. Bypasses PlayerPosition
        // claiming entirely — used by the gauntlet scenario where bots come in
        // along the player's forward axis at runtime rather than from a fixed
        // faceoff slot. `label` becomes the registry's synthetic PositionName.
        public static Player SpawnAtWorld(PlayerTeam team, Vector3 worldPos, Quaternion rotation, ulong targetClientId, string label)
        {
            if (!NetworkManager.Singleton.IsServer) return null;

            var playerManager = PlayerManager.Instance;
            if (playerManager == null)
            {
                Debug.LogWarning("[Chaser] PlayerManager not ready");
                return null;
            }

            var prefab = Traverse.Create(playerManager).Field("playerPrefab").GetValue<Player>();
            if (prefab == null)
            {
                Debug.LogError("[Chaser] playerPrefab missing");
                return null;
            }

            var playerObj = UnityEngine.Object.Instantiate(prefab);
            var netObj = playerObj.GetComponent<NetworkObject>();
            var clientId = ChaserRegistry.AllocateClientId();

            netObj.SpawnWithOwnership(clientId, true);

            var player = playerObj.GetComponent<Player>();
            player.Username.Value = $"demBot_{label}";
            player.Number.Value = 99;
            player.CustomizationState.Value = BotCustomization.BuildFromSettings();
            player.Server_SetGameState(team: team, role: PlayerRole.Attacker);

            player.Server_SpawnCharacter(worldPos, rotation, PlayerRole.Attacker);

            // Ensure the body isn't carrying a stale HasFallen state — fresh
            // spawns should always be upright.
            if (player.PlayerBody != null) player.PlayerBody.OnStandUp();

            if (ConfigData.Instance.RandomizeBotAppearance)
            {
                player.CustomizationState.Value = BotCustomization.BuildRandom(player);
            }

            var ai = player.gameObject.AddComponent<ChaserAI>();
            ai.ControlledPlayer = player;
            ai.TargetClientId = targetClientId;

            // Permanently immune to toppling. Used for gauntlet bots where
            // contact-induced ragdoll-style flops aren't wanted.
            var topple = player.gameObject.AddComponent<BotTopplePreventer>();
            topple.Bind(player.PlayerBody);

            ChaserRegistry.Register(new ChaserRegistry.Entry
            {
                Player = player,
                Team = team,
                PositionName = label,
                ClientId = clientId,
            });

            Debug.Log($"[Chaser] Spawned at world {worldPos} (clientId={clientId}, label={label})");
            return player;
        }

        public static int DespawnAll()
        {
            ChaserRegistry.CleanupDestroyed();
            var entries = ChaserRegistry.All.ToList();
            int count = 0;
            foreach (var e in entries)
            {
                if (DespawnEntry(e)) count++;
            }
            Debug.Log($"[Chaser] Cleared {count} chaser bot(s)");
            return count;
        }

        public static bool DespawnAt(string positionName)
        {
            ChaserRegistry.CleanupDestroyed();
            var entry = ChaserRegistry.All
                .FirstOrDefault(e => string.Equals(e.PositionName, positionName, StringComparison.OrdinalIgnoreCase));
            if (entry == null)
            {
                Debug.Log($"[Chaser] No chaser bot at position {positionName}");
                return false;
            }
            return DespawnEntry(entry);
        }

        private static bool DespawnEntry(ChaserRegistry.Entry e)
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
                ChaserRegistry.Unregister(e.ClientId);
                Debug.Log($"[Chaser] Despawned at {e.Team} {e.PositionName}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Chaser] Despawn error: {ex.Message}");
                ChaserRegistry.Unregister(e.ClientId);
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
