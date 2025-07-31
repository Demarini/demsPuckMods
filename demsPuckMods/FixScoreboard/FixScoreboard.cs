using ArtificialInputDelay;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace FixScoreboard
{  
    public class FixScoreboard : IPuckMod
    {
        private static readonly Harmony harmony = new Harmony("GAFURIX.FixScoreboard");

        public bool OnEnable()
        {
            Debug.Log("[FixScoreboard] Mod enabled");
            //ModConfig.Initialize();
            //ConfigData.Load();

            try
            {
                harmony.PatchAll();
                Debug.Log("[FixScoreboard] Harmony patches applied");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[FixScoreboard] Harmony patch failed: {e.Message}");
                return false;
            }

            return true;
        }

        public bool OnDisable()
        {
            try
            {
                harmony.UnpatchSelf();
                Debug.Log("[FixScoreboard] Mod disabled");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[FixScoreboard] Harmony unpatch failed: {e.Message}");
                return false;
            }

            return true;
        }
    }
    [HarmonyPatch(typeof(UIScoreboard), nameof(UIScoreboard.UpdatePlayer))]
    public static class UpdatePlayer_JoinOrderPreservingPatch
    {
        public static bool Prefix(UIScoreboard __instance, Player player)
        {
            if (Application.isBatchMode) return false;

            var playerMap = Traverse.Create(__instance)
                                    .Field("playerVisualElementMap")
                                    .GetValue<Dictionary<Player, VisualElement>>();

            if (!playerMap.TryGetValue(player, out var visualElement))
                return false;

            // Update UI text and colors
            VisualElement patreonVE = visualElement.Query<VisualElement>("PatreonVisualElement");
            Label label = visualElement.Query<Label>("UsernameLabel");
            Label label2 = visualElement.Query<Label>("GoalsLabel");
            Label label3 = visualElement.Query<Label>("AssistsLabel");
            Label label4 = visualElement.Query<Label>("PointsLabel");
            Label label5 = visualElement.Query<Label>("PingLabel");
            Label label6 = visualElement.Query<Label>("PositionLabel");

            string adminTag = "";
            int adminLevel = player.AdminLevel.Value;
            if (adminLevel == 1)
                adminTag = "<b><color=#206694>*</color></b>";
            else if (adminLevel == 2)
                adminTag = "<b><color=#992d22>*</color></b>";
            else if (adminLevel > 2)
                adminTag = "<b><color=#71368a>*</color></b>";

            patreonVE.style.display = (player.PatreonLevel.Value > 0) ? DisplayStyle.Flex : DisplayStyle.None;
            label.style.color = (player.PatreonLevel.Value > 0) ? GetPatreonColor(__instance) : Color.white;
            label6.text = player.PlayerPosition != null ? player.PlayerPosition.Name.ToString() : "N/A";
            label.text = $"{adminTag}<noparse>#{player.Number.Value} {player.Username.Value}</noparse>";
            label2.text = player.Goals.Value.ToString();
            label3.text = player.Assists.Value.ToString();
            label4.text = (player.Goals.Value + player.Assists.Value).ToString();
            label5.text = player.Ping.Value.ToString();

            // Team logic
            var currentTeam = player.Team.Value;
            var currentParent = visualElement.parent;

            var teamBlue = Traverse.Create(__instance).Field("teamBlueContainer").GetValue<VisualElement>();
            var teamRed = Traverse.Create(__instance).Field("teamRedContainer").GetValue<VisualElement>();
            var teamSpectator = Traverse.Create(__instance).Field("teamSpectatorContainer").GetValue<VisualElement>();

            VisualElement targetContainer = null;
            switch (currentTeam)
            {
                case PlayerTeam.Blue: targetContainer = teamBlue; break;
                case PlayerTeam.Red: targetContainer = teamRed; break;
                case PlayerTeam.Spectator: targetContainer = teamSpectator; break;
            }

            if (targetContainer != null && currentParent != targetContainer)
            {
                visualElement.RemoveFromHierarchy();

                if (currentTeam == PlayerTeam.Spectator)
                {
                    label2.text = "";
                    label3.text = "";
                    label4.text = "";
                }

                int joinIndex = ScoreboardOrderTracker.GetIndex(player);
                if (joinIndex >= 0 && joinIndex <= targetContainer.childCount)
                    targetContainer.Insert(joinIndex, visualElement);
                else
                    targetContainer.Add(visualElement);
            }

            return false; // Skip original method entirely
        }

        private static Color GetPatreonColor(UIScoreboard scoreboard)
        {
            return Traverse.Create(scoreboard).Field("patreonColor").GetValue<Color>();
        }
    }
    public static class ScoreboardOrderTracker
    {
        private static readonly List<Player> PlayerJoinOrder = new List<Player>();
        private static readonly Dictionary<Player, PlayerTeam> LastKnownTeam = new Dictionary<Player, PlayerTeam>();

        public static void AddPlayer(Player player)
        {
            if (!PlayerJoinOrder.Contains(player))
                PlayerJoinOrder.Add(player);

            LastKnownTeam[player] = player.Team.Value;
        }

        public static void RemovePlayer(Player player)
        {
            PlayerJoinOrder.Remove(player);
            LastKnownTeam.Remove(player);
        }

        public static int GetIndex(Player player)
        {
            if (!LastKnownTeam.TryGetValue(player, out var team))
                return -1;

            // Filter for same-team players, then get index in that list
            var sameTeam = PlayerJoinOrder.Where(p => LastKnownTeam.TryGetValue(p, out var t) && t == team).ToList();
            return sameTeam.IndexOf(player);
        }

        public static void UpdateTeam(Player player)
        {
            LastKnownTeam[player] = player.Team.Value;
        }

        // Optional: for debugging
        public static IEnumerable<Player> GetOrderedTeam(PlayerTeam team)
        {
            return PlayerJoinOrder.Where(p => LastKnownTeam.TryGetValue(p, out var t) && t == team);
        }
    }
}
