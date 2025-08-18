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
        public static void SpawnFakePlayer(int index, PlayerRole role, PlayerTeam team)
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
            netObj.SpawnWithOwnership(fakeClientId, true);

            var player = playerObj.GetComponent<Player>();
            player.Username.Value = $"demBot{team.ToString()}_{(team == PlayerTeam.Red ? GoalieSettings.InstanceRed.Difficulty.ToString() : GoalieSettings.InstanceBlue.Difficulty.ToString())}";
            player.Team.Value = team;
            player.Number.Value = 7;
            player.Role.Value = role;
            string randomJersey = RandomSkins.GetRandomJersey();
            string randomStick = RandomSkins.GetRandomStickSkin(role);
            string randomShaftTape = RandomSkins.GetRandomShaftTape(role);
            string randomBladeTape = RandomSkins.GetRandomBladeTape(role);
            string randomMustache = RandomSkins.GetRandomMustache();
            string randomBeard = RandomSkins.GetRandomBeard();
            string flag = RandomSkins.GetRandomCountry();
            string visor = RandomSkins.GetRandomVisor();
            player.JerseyGoalieRedSkin.Value = new FixedString32Bytes(randomJersey);
            player.JerseyGoalieBlueSkin.Value = new FixedString32Bytes(randomJersey);
            player.StickGoalieRedSkin.Value = new FixedString32Bytes(randomStick);
            player.StickGoalieBlueSkin.Value = new FixedString32Bytes(randomStick);
            player.StickBladeGoalieBlueTapeSkin.Value = new FixedString32Bytes(randomBladeTape);
            player.StickBladeGoalieRedTapeSkin.Value = new FixedString32Bytes(randomBladeTape);
            player.StickShaftGoalieBlueTapeSkin.Value = new FixedString32Bytes(randomShaftTape);
            player.StickShaftGoalieRedTapeSkin.Value = new FixedString32Bytes(randomShaftTape);
            player.Mustache.Value = new FixedString32Bytes(randomMustache);
            player.Beard.Value = new FixedString32Bytes(randomBeard);
            player.Country.Value = new FixedString32Bytes(flag);
            player.VisorAttackerBlueSkin.Value = new FixedString32Bytes(visor);
            player.VisorAttackerRedSkin.Value = new FixedString32Bytes(visor);
            player.VisorGoalieBlueSkin.Value = new FixedString32Bytes(visor);
            player.VisorGoalieRedSkin.Value = new FixedString32Bytes(visor);
            var position = GetNextUnclaimedPosition(player.Team.Value, player.Role.Value);
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
            if (NetworkManager.Singleton.IsServer)
            {
                if (playerObj.IsCharacterPartiallySpawned)
                {
                    playerObj.Server_DespawnCharacter();
                    playerObj.Server_SpawnCharacter(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), role);
                    return;
                }
                else
                {
                    playerObj.Server_SpawnCharacter(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), role);
                }
            }
            //playerObj.Server_RespawnCharacter(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), PlayerRole.Goalie);
            //position.Server_Claim(player);
            // Nudge goalie back toward the goal slightly to reduce open angles
            Vector3 neutralForward = (player.Team.Value == PlayerTeam.Red) ? Vector3.forward : Vector3.back;
            Vector3 adjustedPosition = (team == PlayerTeam.Red ? redGoal : blueGoal) + neutralForward * (team == PlayerTeam.Red ? GoalieSettings.InstanceRed.DistanceFromNet : GoalieSettings.InstanceBlue.DistanceFromNet);
            adjustedPosition.y = player.transform.position.y; // maintain height
            player.transform.position = adjustedPosition;
            player.PlayerBody.Rigidbody.position = adjustedPosition; // sync rigidbody position too
            var body = player.PlayerBody;
            var mesh = body.PlayerMesh;
            var stickMesh = body.Stick.StickMesh;
            //Debug.Log("Player Jersey - " + player.GetPlayerJerseySkin().Value.ToString());
            mesh.SetJersey(player.Team.Value, randomJersey);
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
            Goalies.GoaliesAreRunning = true;
            //NetworkBehaviourSingleton<UIScoreboard>.Instance.RemovePlayer(player);
            NetworkBehaviourSingleton<PlayerManager>.Instance.RemovePlayer(player);
            //Debug.Log($"Player Count: {PlayerManager.Instance.GetPlayers(false).Count}");
            UIScoreboard.Instance.UpdateServer(NetworkBehaviourSingleton<ServerManager>.Instance.Server, PlayerManager.Instance.GetPlayers(false).Count);
            //DetectPositions.UpdateLabel(player);
            //Debug.Log($"[FAKE_SPAWN] Spawned {player.Username.Value} as {player.Role.Value} on {player.Team.Value}");
        }
        private static PlayerPosition GetNextUnclaimedPosition(PlayerTeam team, PlayerRole? role = null)
        {
            var positionManager = PlayerPositionManager.Instance;
            if (positionManager == null)
            {
                //Debug.LogError("[FAKE_SPAWN] PlayerPositionManager.Instance was null!");
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
                    //Debug.LogError($"[FAKE_SPAWN] Invalid team: {team}");
                    return null;
            }

            foreach (var pos in positions)
            {
                if (!pos.IsClaimed && (!role.HasValue || pos.Role == role.Value))
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
                if (type == GoalieSession.Blue && p.Team.Value == PlayerTeam.Blue)
                {
                    Despawn(p);
                }
                else if (type == GoalieSession.Red && p.Team.Value == PlayerTeam.Red)
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
            p.Server_DespawnCharacter();
            p.NetworkObject.Despawn(true);
            FakePlayerRegistry.Unregister(p);
            p.Team.Value = PlayerTeam.None;
            NetworkBehaviourSingleton<PlayerManager>.Instance.RemovePlayer(p);
            //Debug.Log("Removed Bot From Game");
            //EventManager.TriggerEvent("Event_Server_OnPlayerDespawned", new Dictionary<string, object> { { "player", p } });
        }
        public static void DetectOpenGoalAndSpawnBot()
        {

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
                if (b.Team.Value == PlayerTeam.Blue)
                {
                    //Debug.Log("Has Blue Bot: " + b.Username.Value);
                    hasBlueBot = true;
                    blueBot = b;
                }
                else
                {
                    //Debug.Log("Has Red Bot: " + b.Username.Value);
                    hasRedBot = true;
                    redBot = b;
                }
            }
            foreach (Player p in players)
            {
                //Debug.Log($"Player Count: {players.Count()}");
                if (p.Role.Value == PlayerRole.Goalie && p.IsSpawned && p.PlayerBody != null)
                {
                    if (p.Team.Value == PlayerTeam.Red)
                    {
                        //Debug.Log("Has Red Goalie: " + p.Username.Value);
                        hasRedGoalie = true;
                    }
                    else
                    {
                        //Debug.Log("Has Blue Goalie: " + p.Username.Value);
                        hasBlueGoalie = true;
                    }
                }
            }
            if (hasBlueGoalie)
            {
                if (hasBlueBot)
                {
                    //Debug.Log("Despawning Blue Bot");
                    BotSpawning.Despawn(blueBot);
                    blueGoalieSpawned = false;
                }
            }
            else
            {
                if (!hasBlueBot)
                {
                    //Debug.Log("Spawning Blue Bot");
                    BotSpawning.SpawnFakePlayer(0, PlayerRole.Goalie, PlayerTeam.Blue);
                    blueGoalieSpawned = true;
                }
            }
            if (hasRedGoalie)
            {
                if (hasRedBot)
                {
                    //Debug.Log("Spawning Red Bot");
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
            // Optionally, you can prevent the original method from running by returning false
            // return false;

            // Return true to allow the original method to execute after the prefix
        }
    }
}