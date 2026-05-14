using HarmonyLib;
using PuckAIPractice.AI;
using PuckAIPractice.GameModes;
using PuckAIPractice.Patches;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Mathematics.Geometry;
using Unity.Netcode;
using UnityEngine;

namespace PuckAIPractice.Utilities
{
    public static class BotSpawning
    {
        static Vector3 redGoal = new Vector3(0.0f, 0f, -40.23f);
        static Vector3 blueGoal = new Vector3(0.0f, 0f, 40.23f);
        static Player redGoalie = null;
        static Player blueGoalie = null;
        static bool blueGoalieSpawned;
        static bool redGoalieSpawned;
        static int botIndex = 0;
        public static void SpawnChaser(PlayerTeam team, PlayerRole role)
        {
            if (!PracticeModeDetector.IsPracticeMode && !NetworkManager.Singleton.IsServer)
            {
                return;
            }
            var playerManager = PlayerManager.Instance;
            if (playerManager == null)
            {
                //Debug.LogError("[FAKE_SPAWN] PlayerManager.Instance was null!");
                return;
            }

            // Reject duplicate spawn: if a bot for this team already exists in the
            // registry, a 2nd SpawnWithOwnership call would create an orphan
            // NetworkObject sharing the same fake clientId — the registry's
            // OwnerClientId-uniqueness check then silently drops the new entry,
            // and the orphan sticks around with its own GoalieAI until the next
            // phase cleanup ("stacked goalies in the net" bug).
            FakePlayerRegistry.CleanupDestroyed();
            if (FakePlayerRegistry.HasBotForTeam(team))
            {
                Debug.Log($"[BotSpawning] {team} bot already exists; skipping duplicate SpawnChaser");
                return;
            }

            var prefab = Traverse.Create(playerManager).Field("playerPrefab").GetValue<Player>();
            if (prefab == null)
            {
                //Debug.LogError("[FAKE_SPAWN] Could not access playerPrefab from PlayerManager!");
                return;
            }
            //Debug.Log("Got player instance?");
            var playerObj = UnityEngine.Object.Instantiate(prefab);
            var netObj = playerObj.GetComponent<NetworkObject>();

            ulong fakeClientId = 7777777UL + (ulong)(team == PlayerTeam.Red ? 1 : 0);
            botIndex++;
            FakePlayerRegistry.ReserveFakeClientId(fakeClientId);
            netObj.SpawnWithOwnership(fakeClientId, true);

            var player = playerObj.GetComponent<Player>();
            player.Username.Value = $"demBot_Chaser";
            player.Server_SetGameState(team: team, role: role);
            player.Number.Value = 7;
            player.CustomizationState.Value = new PlayerCustomizationState();
            FakePlayerRegistry.Register(player, team);
            var position = GetNextUnclaimedPosition(player.Team, player.Role);
            if (position != null)
                position.Server_Claim(player);
            var body = player.PlayerBody;
            if (body != null)
            {
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
            //Goalies.GoaliesAreRunning = true;
            //NetworkBehaviourSingleton<UIScoreboard>.Instance.RemovePlayer(player);
            //NetworkBehaviourSingleton<PlayerManager>.Instance.RemovePlayer(player);
            //Debug.Log($"Player Count: {PlayerManager.Instance.GetPlayers(false).Count}");
            //UIScoreboard.Instance.UpdateServer(NetworkBehaviourSingleton<ServerManager>.Instance.Server, PlayerManager.Instance.GetPlayers(false).Count);
            //DetectPositions.UpdateLabel(player);
            //Debug.Log($"[FAKE_SPAWN] Spawned {player.Username.Value} as {player.Role.Value} on {player.Team.Value}");
        }
        public static void SpawnFakePlayer(int index, PlayerRole role, PlayerTeam team)
        {
            if (!PracticeModeDetector.IsPracticeMode && !NetworkManager.Singleton.IsServer)
            {
                return;
            }
            var playerManager = PlayerManager.Instance;
            if (playerManager == null)
            {
                return;
            }

            FakePlayerRegistry.CleanupDestroyed();
            if (FakePlayerRegistry.HasBotForTeam(team))
            {
                Debug.Log($"[BotSpawning] {team} bot already exists; skipping duplicate SpawnFakePlayer");
                return;
            }

            var prefab = Traverse.Create(playerManager).Field("playerPrefab").GetValue<Player>();
            if (prefab == null)
            {
                return;
            }
            var playerObj = UnityEngine.Object.Instantiate(prefab);
            var netObj = playerObj.GetComponent<NetworkObject>();

            ulong fakeClientId = 7777777UL + (ulong)(team == PlayerTeam.Red ? 1 : 0);
            botIndex++;
            FakePlayerRegistry.ReserveFakeClientId(fakeClientId);
            netObj.SpawnWithOwnership(fakeClientId, true);

            var player = playerObj.GetComponent<Player>();
            player.Username.Value = $"demBot{team.ToString()}_{(team == PlayerTeam.Red ? GoalieSettings.InstanceRed.Difficulty.ToString() : GoalieSettings.InstanceBlue.Difficulty.ToString())}";
            player.Server_SetGameState(team: team, role: role);
            player.Number.Value = 7;
            player.CustomizationState.Value = new PlayerCustomizationState();
            var position = GetNextUnclaimedPosition(player.Team, player.Role);
            //player.PlayerPosition = new PlayerPosition();
            //player.PlayerPosition.Name = "G";
        //    MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerPositionChanged", new Dictionary<string, object>
        //{
        //    {
        //        "player",
        //        player
        //    },
        //    {
        //        "oldPlayerPosition",
        //        position
        //    },
        //    {
        //        "newPlayerPosition",
        //        position
        //    }
        //});
            FakePlayerRegistry.Register(player, team);
            Goalies.GoaliesAreRunning = true;

            if (NetworkManager.Singleton.IsServer)
            {
                if (playerObj.IsCharacterSpawned)
                    playerObj.Server_DespawnCharacter();
                playerObj.Server_SpawnCharacter(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), role);
            }

            var body = player.PlayerBody;
            if (body != null)
            {
                Vector3 neutralForward = (player.Team == PlayerTeam.Red) ? Vector3.forward : Vector3.back;
                Vector3 adjustedPosition = (team == PlayerTeam.Red ? redGoal : blueGoal) + neutralForward * (team == PlayerTeam.Red ? GoalieSettings.InstanceRed.DistanceFromNet : GoalieSettings.InstanceBlue.DistanceFromNet);
                adjustedPosition.y = player.transform.position.y;
                player.transform.position = adjustedPosition;
                body.Rigidbody.position = adjustedPosition;
                body.Rigidbody.isKinematic = true;

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

            var ai = player.NetworkObject.gameObject.AddComponent<GoalieAI>();
            ai.controlledPlayer = player;
            ai.team = player.Team;
            ai.puckTransform = PuckManager.Instance
                .GetPlayerPuck(NetworkManager.Singleton.LocalClientId)
                ?.transform;

            PlayerManager.Instance.RemovePlayer(player);
            try
            {
                UIManager.Instance.Scoreboard.StyleServer(ServerManager.Instance.Server.Value, PlayerManager.Instance.GetPlayers(false).Count);
            }
            catch { }
            //DetectPositions.UpdateLabel(player);
            //Debug.Log($"[FAKE_SPAWN] Spawned {player.Username.Value} as {player.Role.Value} on {player.Team.Value}");
        }
        private static PlayerPosition GetNextUnclaimedPosition(PlayerTeam team, PlayerRole? role = null)
        {
            var positions = UnityEngine.Object.FindObjectsOfType<PlayerPosition>();
            if (positions == null || positions.Length == 0)
                return null;

            foreach (var pos in positions)
            {
                if (pos.Team == team && !pos.IsClaimed && (!role.HasValue || pos.Role == role.Value))
                    return pos;
            }

            //Debug.LogWarning($"[FAKE_SPAWN] No available position found for team {team} and role {role}");
            return null;
        }
        public static void DespawnBots(GoalieSession type)
        {
            //Debug.Log("Started Despawn Bots");
            HashSet<Player> players = FakePlayerRegistry.All.ToHashSet<Player>();
            if (players == null)
            {
                //Debug.Log("No Bots in Registry");
            }
            foreach (Player p in players)
            {
                var botTeam = FakePlayerRegistry.GetTeam(p);
                if (type == GoalieSession.Blue && botTeam == PlayerTeam.Blue)
                {
                    Despawn(p);
                }
                else if (type == GoalieSession.Red && botTeam == PlayerTeam.Red)
                {
                    Despawn(p);
                }
                else if (type == GoalieSession.Both)
                {
                    Despawn(p);
                }

            }
            //Debug.Log("Finished Despawning Bots");
        }
        public static void Despawn(Player p)
        {
            FakePlayerRegistry.Unregister(p);
            try
            {
                p.Server_DespawnCharacter();
                p.Server_SetGameState(team: PlayerTeam.None);
                PlayerManager.Instance.RemovePlayer(p);
                p.NetworkObject.Despawn(true);
            }
            catch (System.Exception ex)
            {
                Debug.Log($"[BotSpawning] Despawn error: {ex.Message}");
            }
        }
        public static void DetectOpenGoalAndSpawnBot()
        {
            FakePlayerRegistry.CleanupDestroyed();

            List<Player> players = PlayerManager.Instance.GetPlayers(false);
            bool hasBlueGoalie = false;
            bool hasRedGoalie = false;
            bool hasRedBot = false;
            bool hasBlueBot = false;
            Player blueBot = null;
            Player redBot = null;
            List<Player> bots = FakePlayerRegistry.All.ToList();
            foreach (Player b in bots)
            {
                if (b == null) continue;
                var botTeam = FakePlayerRegistry.GetTeam(b);
                if (botTeam == PlayerTeam.Blue)
                {
                    hasBlueBot = true;
                    blueBot = b;
                }
                else if (botTeam == PlayerTeam.Red)
                {
                    hasRedBot = true;
                    redBot = b;
                }
            }
            foreach (Player p in players)
            {
                if (p.Role == PlayerRole.Goalie && p.IsSpawned && p.PlayerBody != null)
                {
                    if (p.Team == PlayerTeam.Red)
                    {
                        hasRedGoalie = true;
                    }
                    else
                    {
                        hasBlueGoalie = true;
                    }
                }
            }
            if (hasBlueGoalie)
            {
                if (hasBlueBot)
                {
                    BotSpawning.Despawn(blueBot);
                    blueGoalieSpawned = false;
                }
            }
            else
            {
                if (!hasBlueBot)
                {
                    BotSpawning.SpawnFakePlayer(0, PlayerRole.Goalie, PlayerTeam.Blue);
                    blueGoalieSpawned = true;
                }
            }
            if (hasRedGoalie)
            {
                if (hasRedBot)
                {
                    BotSpawning.Despawn(redBot);
                    redGoalieSpawned = false;
                }
            }
            else
            {
                if (!hasRedBot)
                {
                    BotSpawning.SpawnFakePlayer(1, PlayerRole.Goalie, PlayerTeam.Red);
                    redGoalieSpawned = true;
                }
            }
        }
    }
}