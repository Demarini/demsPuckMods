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
using PuckAIPractice.GameModes;
using Unity.Netcode;
using PuckAIPractice.Patches;

namespace PuckAIPractice.Utilities
{
    public class DetectPositions : MonoBehaviour
    {
        int frameCounter = 0;
        GamePhase currentPhase = GamePhase.Warmup;
        GamePhase lastPhase = GamePhase.Warmup;
        void Update()
        {
            frameCounter++;

            if (frameCounter >= 10 || true) // every 10 frames
            {
                if ((!PracticeModeDetector.IsPracticeMode && !NetworkManager.Singleton.IsServer) || GameManager.Instance.Phase == GamePhase.Replay)
                {
                    if(GameManager.Instance.Phase == GamePhase.Replay)
                    {
                        BotSpawning.DespawnBots(GoalieSession.Both);
                    }
                    return;
                }
                //if (currentPhase != lastPhase)
                //{
                //    if (currentPhase == GamePhase.FaceOff)
                //    {
                //        foreach (Player player in FakePlayerRegistry.All)
                //        {
                //            ReplayManager.Instance.ReplayRecorder.Server_AddPlayerSpawnedEvent(player);
                //            if (player.PlayerBody)
                //            {
                //                Debug.Log("Goalie Skins");
                //                Debug.Log(player.PlayerBody.Player.JerseyGoalieRedSkin.Value.ToString());
                //                Debug.Log(player.PlayerBody.Player.JerseyGoalieBlueSkin.Value.ToString());
                //                ReplayManager.Instance.ReplayRecorder.Server_AddPlayerBodySpawnedEvent(player.PlayerBody);
                //            }
                //            if (player.Stick)
                //            {
                //                ReplayManager.Instance.ReplayRecorder.Server_AddStickSpawnedEvent(player.Stick);
                //            }
                //        }
                //    }
                //}
                frameCounter = 0; // reset
                lastPhase = currentPhase;
                BotSpawning.DetectOpenGoalAndSpawnBot();
            }
        }

        void RunMyLogic()
        {
            // put your logic here
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