using HarmonyLib;
using PuckAIPractice.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace PuckAIPractice.Utilities
{
    public static class BotSpawning
    {
        public static void SpawnFakePlayer(int index, PlayerRole role, PlayerTeam team)
        {
            var playerManager = PlayerManager.Instance;
            if (playerManager == null)
            {
                Debug.LogError("[FAKE_SPAWN] PlayerManager.Instance was null!");
                return;
            }

            var prefab = Traverse.Create(playerManager).Field("playerPrefab").GetValue<Player>();
            if (prefab == null)
            {
                Debug.LogError("[FAKE_SPAWN] Could not access playerPrefab from PlayerManager!");
                return;
            }

            var playerObj = UnityEngine.Object.Instantiate(prefab);
            var netObj = playerObj.GetComponent<NetworkObject>();

            ulong fakeClientId = 9990UL + (ulong)index;
            netObj.SpawnWithOwnership(fakeClientId, true);

            var player = playerObj.GetComponent<Player>();
            player.Username.Value = $"Bot_{index + 1}";
            player.Team.Value = team;
            player.Number.Value = index + 1;
            player.Role.Value = role;
            player.PatreonLevel.Value = 69;
            var position = GetNextUnclaimedPosition(player.Team.Value, player.Role.Value);
            position.Server_Claim(player);
            // Nudge goalie back toward the goal slightly to reduce open angles
            Vector3 neutralForward = (player.Team.Value == PlayerTeam.Red) ? Vector3.forward : Vector3.back;
            Vector3 adjustedPosition = player.transform.position - neutralForward * GoalieSettings.Instance.DistanceFromNet;

            adjustedPosition.y = player.transform.position.y; // maintain height
            player.transform.position = adjustedPosition;
            player.PlayerBody.Rigidbody.position = adjustedPosition; // sync rigidbody position too
            var body = player.PlayerBody;
            var mesh = body.PlayerMesh;
            var stickMesh = body.Stick.StickMesh;
            Debug.Log("Player Jersey - " + player.GetPlayerJerseySkin().ToString());
            mesh.SetJersey(player.Team.Value, RandomSkins.GetRandomJersey());
            mesh.SetNumber(player.Number.Value.ToString());
            mesh.SetUsername(player.Username.Value.ToString());
            mesh.SetRole(player.Role.Value);
            mesh.PlayerHead.SetMustache(RandomSkins.GetRandomMustache());
            mesh.PlayerHead.SetHelmetFlag(RandomSkins.GetRandomCountry());
            mesh.PlayerHead.SetHelmetVisor(RandomSkins.GetRandomVisor());
            mesh.PlayerHead.SetBeard(RandomSkins.GetRandomBeard());
            stickMesh.SetBladeTape(RandomSkins.GetRandomBladeTape(player.Role.Value));
            stickMesh.SetSkin(player.Team.Value, RandomSkins.GetRandomStickSkin(player.Role.Value));
            stickMesh.SetShaftTape(RandomSkins.GetRandomShaftTape(player.Role.Value));
            var ai = player.NetworkObject.gameObject.AddComponent<GoalieAI>();
            ai.controlledPlayer = player;
            ai.team = player.Team.Value;
            ai.puckTransform = NetworkBehaviourSingleton<PuckManager>
                .Instance.GetPlayerPuck(NetworkBehaviourSingleton<PuckManager>.Instance.OwnerClientId)
                ?.transform;
            //ai.puckTransform = FindObjectOfType<Puck>()?.transform;
            player.PlayerBody.Rigidbody.isKinematic = true;
            //player.PlayerInput.Client_SlideInputRpc(true);
            FakePlayerRegistry.Register(player);         
            Debug.Log($"[FAKE_SPAWN] Spawned {player.Username.Value} as {player.Role.Value} on {player.Team.Value}");
        }
        private static PlayerPosition GetNextUnclaimedPosition(PlayerTeam team, PlayerRole? role = null)
        {
            var positionManager = PlayerPositionManager.Instance;
            if (positionManager == null)
            {
                Debug.LogError("[FAKE_SPAWN] PlayerPositionManager.Instance was null!");
                return null;
            }

            List<PlayerPosition> positions = null;

            switch (team)
            {
                case PlayerTeam.Blue:
                    positions = positionManager.BluePositions;
                    break;
                case PlayerTeam.Red:
                    positions = positionManager.RedPositions;
                    break;
                default:
                    Debug.LogError($"[FAKE_SPAWN] Invalid team: {team}");
                    return null;
            }

            foreach (var pos in positions)
            {
                if (!pos.IsClaimed && (!role.HasValue || pos.Role == role.Value))
                    return pos;
            }

            Debug.LogWarning($"[FAKE_SPAWN] No available position found for team {team} and role {role}");
            return null;
        }
        public static void DespawnBots()
        {
            HashSet<Player> players = FakePlayerRegistry.All.ToHashSet<Player>();
            foreach (Player p in players)
            {
                p.PlayerPosition.Server_Unclaim();
                p.Server_DespawnCharacter();
                p.Server_DespawnSpectatorCamera();
                p.Team.Value = PlayerTeam.None;
                NetworkBehaviourSingleton<PlayerManager>.Instance.RemovePlayer(p);
                p.NetworkObject.Despawn(true);
                UIScoreboard.Instance.RemovePlayer(p);
                //EventManager.TriggerEvent("Event_Server_OnPlayerDespawned", new Dictionary<string, object> { { "player", p } });
                FakePlayerRegistry.Unregister(p);
            }
        }
    }
}
