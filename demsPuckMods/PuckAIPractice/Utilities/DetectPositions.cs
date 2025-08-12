using PuckAIPractice.AI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using PuckAIPractice.Singletons;
using HarmonyLib;
using Unity.Mathematics.Geometry;

namespace PuckAIPractice.Utilities
{
    public class DetectPositions : MonoBehaviour
    {
        int frameCounter = 0;

        void Update()
        {
            frameCounter++;

            if (frameCounter >= 10) // every 10 frames
            {
                frameCounter = 0; // reset
                DetectOpenGoalAndSpawnBot();
            }
        }

        void RunMyLogic()
        {
            // put your logic here
        }
        public void DetectOpenGoalAndSpawnBot()
        {
            //Debug.Log($"[Harmony Prefix] Respawning character for {__instance.Username.Value} at position {position} with role {role}");
            List<Player> players = PlayerManager.Instance.GetPlayers(false);
            bool hasBlueGoalie = false;
            bool hasRedGoalie = false;
            bool hasRedBot = false;
            bool hasBlueBot = false;
            Player blueBot = null;
            Player redBot = null;
            List<Player> bots = FakePlayerRegistry.All.ToList();
            List<Player> existingBots = FakePlayerRegistry.AllExisting.ToList();         
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
                if (existingBots.Contains(p)) continue;
                if (p.Role.Value == PlayerRole.Goalie)
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
                }
            }
            else
            {
                if (!hasBlueBot)
                {
                    //Debug.Log("Spawning Blue Bot");
                    GoalieSettings.InstanceBlue.ApplyDifficulty(ConfigData.Instance.BlueGoalieDefaultDifficulty);
                    BotSpawning.SpawnFakePlayer(0, PlayerRole.Goalie, PlayerTeam.Blue);
                }
            }
            if (hasRedGoalie)
            {
                if (hasRedBot)
                {
                    BotSpawning.Despawn(redBot);
                }
            }
            else
            {
                if (!hasRedBot)
                {
                    GoalieSettings.InstanceRed.ApplyDifficulty(ConfigData.Instance.RedGoalieDefaultDifficulty);
                    BotSpawning.SpawnFakePlayer(1, PlayerRole.Goalie, PlayerTeam.Red);
                }
            }
            // Optionally, you can prevent the original method from running by returning false
            // return false;

            // Return true to allow the original method to execute after the prefix
        }
        public static DetectPositions Create()
        {
            var go = new GameObject("DetectPositions");
            DontDestroyOnLoad(go); // optional, keeps it across scenes
            return go.AddComponent<DetectPositions>();
        }
        public static void UpdateLabel(Player player)
        {
            var __instance = UIScoreboard.Instance;
            if (__instance == null || player == null)
                return;

            // Grab the visual element associated with this player
            VisualElement visualElement;
            if (!Traverse.Create(__instance)
                         .Field("playerVisualElementMap")
            .GetValue<Dictionary<Player, VisualElement>>()
                         .TryGetValue(player, out visualElement))
                return;

            // Rebuild label text with our own admin prefix logic
            Label label6 = visualElement.Query<Label>("PositionLabel");
            Label usernameLabel = visualElement.Query<Label>("Username");
            if (FakePlayerRegistry.All.Contains(player))
            {
                label6.text = "G";
                usernameLabel.text = string.Format("{0}<noparse>#{1} {2}</noparse>", "<b><color=#992d22>BOT</color></b>", player.Number.Value, player.Username.Value);
            }
        }
    }
}
