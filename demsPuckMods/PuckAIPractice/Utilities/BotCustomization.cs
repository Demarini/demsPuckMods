using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace PuckAIPractice.Utilities
{
    // Bots created by this mod don't go through ConnectionManager, so their
    // PlayerCustomizationState never gets populated from SettingsManager the
    // way a real client's would. `new PlayerCustomizationState()` is an all-zero
    // struct; PlayerBody.ApplyCustomizations() reads JerseyID=0 from it and the
    // mesh renders blank. Build the struct from SettingsManager (same shape the
    // game uses for connecting clients in ServerManager.cs / ConnectionManager.cs)
    // so the bots inherit the host's jersey/stick/headgear choices and render
    // with the proper team colors.
    public static class BotCustomization
    {
        private static readonly System.Random Rng = new System.Random();

        public static PlayerCustomizationState BuildFromSettings()
        {
            return new PlayerCustomizationState
            {
                FlagID = SettingsManager.FlagID,
                HeadgearIDBlueAttacker = SettingsManager.HeadgearIDBlueAttacker,
                HeadgearIDRedAttacker = SettingsManager.HeadgearIDRedAttacker,
                HeadgearIDBlueGoalie = SettingsManager.HeadgearIDBlueGoalie,
                HeadgearIDRedGoalie = SettingsManager.HeadgearIDRedGoalie,
                MustacheID = SettingsManager.MustacheID,
                BeardID = SettingsManager.BeardID,
                JerseyIDBlueAttacker = SettingsManager.JerseyIDBlueAttacker,
                JerseyIDRedAttacker = SettingsManager.JerseyIDRedAttacker,
                JerseyIDBlueGoalie = SettingsManager.JerseyIDBlueGoalie,
                JerseyIDRedGoalie = SettingsManager.JerseyIDRedGoalie,
                StickSkinIDBlueAttacker = SettingsManager.StickSkinIDBlueAttacker,
                StickSkinIDRedAttacker = SettingsManager.StickSkinIDRedAttacker,
                StickSkinIDBlueGoalie = SettingsManager.StickSkinIDBlueGoalie,
                StickSkinIDRedGoalie = SettingsManager.StickSkinIDRedGoalie,
                StickShaftTapeIDBlueAttacker = SettingsManager.StickShaftTapeIDBlueAttacker,
                StickShaftTapeIDRedAttacker = SettingsManager.StickShaftTapeIDRedAttacker,
                StickShaftTapeIDBlueGoalie = SettingsManager.StickShaftTapeIDBlueGoalie,
                StickShaftTapeIDRedGoalie = SettingsManager.StickShaftTapeIDRedGoalie,
                StickBladeTapeIDBlueAttacker = SettingsManager.StickBladeTapeIDBlueAttacker,
                StickBladeTapeIDRedAttacker = SettingsManager.StickBladeTapeIDRedAttacker,
                StickBladeTapeIDBlueGoalie = SettingsManager.StickBladeTapeIDBlueGoalie,
                StickBladeTapeIDRedGoalie = SettingsManager.StickBladeTapeIDRedGoalie,
            };
        }

        // Reads the available option lists off the bot's already-spawned mesh
        // and rolls a random ID per category, starting from SettingsManager
        // defaults so the irrelevant team/role slots stay sensible (only the
        // bot's actual team+role slot is read by the game's render path).
        //
        // Returns BuildFromSettings() unchanged if the mesh/stick isn't spawned
        // yet (caller should re-invoke once the body is up).
        public static PlayerCustomizationState BuildRandom(Player player)
        {
            var state = BuildFromSettings();
            if (player == null) return state;

            var body = player.PlayerBody;
            var mesh = body != null ? body.PlayerMesh : null;
            if (mesh == null) return state;

            var head = mesh.PlayerHead;
            if (head != null)
            {
                state.FlagID = PickRandomId(head, "flags", flag => true) ?? state.FlagID;

                // Headgear includes visors; filter by the bot's role.
                var role = player.Role;
                var headgearId = PickRandomId(head, "headgear", h =>
                {
                    var traverse = Traverse.Create(h);
                    var roleVal = traverse.Field("Role").GetValue();
                    // Headgear.IsForRole(role) is the canonical filter — use it.
                    var isForRole = AccessTools.Method(h.GetType(), "IsForRole");
                    if (isForRole == null) return true;
                    return (bool)isForRole.Invoke(h, new object[] { role });
                });
                if (headgearId.HasValue)
                {
                    SetHeadgearForTeamRole(ref state, player.Team, role, headgearId.Value);
                }

                state.MustacheID = PickRandomId(head, "mustaches", m => true) ?? state.MustacheID;
                state.BeardID = PickRandomId(head, "beards", b => true) ?? state.BeardID;
            }

            var torso = mesh.PlayerTorso;
            if (torso != null)
            {
                var team = player.Team;
                var jerseyId = PickRandomId(torso, "jerseys", j =>
                {
                    var isForTeam = AccessTools.Method(j.GetType(), "IsForTeam");
                    if (isForTeam == null) return true;
                    return (bool)isForTeam.Invoke(j, new object[] { team });
                });
                if (jerseyId.HasValue)
                {
                    SetJerseyForTeamRole(ref state, team, player.Role, jerseyId.Value);
                }
            }

            var stick = body != null ? body.Stick : null;
            var stickMesh = stick != null ? stick.StickMesh : null;
            if (stickMesh != null)
            {
                var team = player.Team;
                var skinId = PickRandomId(stickMesh, "skins", s =>
                {
                    var isForTeam = AccessTools.Method(s.GetType(), "IsForTeam");
                    if (isForTeam == null) return true;
                    return (bool)isForTeam.Invoke(s, new object[] { team });
                });
                if (skinId.HasValue)
                {
                    SetStickSkinForTeamRole(ref state, team, player.Role, skinId.Value);
                }

                var shaftTapeId = PickRandomId(stickMesh, "shaftTapes", t => true);
                if (shaftTapeId.HasValue)
                {
                    SetShaftTapeForTeamRole(ref state, team, player.Role, shaftTapeId.Value);
                }
                var bladeTapeId = PickRandomId(stickMesh, "bladeTapes", t => true);
                if (bladeTapeId.HasValue)
                {
                    SetBladeTapeForTeamRole(ref state, team, player.Role, bladeTapeId.Value);
                }
            }

            return state;
        }

        // Generic "read a private List<T> field via Traverse, filter, pick a
        // random ID via reflection on the T.ID public int field." Returns null
        // if the field is missing, empty, or fully filtered out.
        private static int? PickRandomId(object owner, string fieldName, Func<object, bool> predicate)
        {
            try
            {
                var list = Traverse.Create(owner).Field(fieldName).GetValue() as System.Collections.IList;
                if (list == null || list.Count == 0) return null;

                var candidates = new List<int>();
                foreach (var item in list)
                {
                    if (item == null) continue;
                    if (!predicate(item)) continue;
                    var idField = item.GetType().GetField("ID");
                    if (idField == null) continue;
                    var idVal = idField.GetValue(item);
                    if (idVal is int idInt) candidates.Add(idInt);
                }
                if (candidates.Count == 0) return null;
                return candidates[Rng.Next(candidates.Count)];
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[BotCustomization] PickRandomId({fieldName}) failed: {ex.Message}");
                return null;
            }
        }

        private static void SetHeadgearForTeamRole(ref PlayerCustomizationState s, PlayerTeam team, PlayerRole role, int id)
        {
            if (team == PlayerTeam.Blue && role == PlayerRole.Attacker) s.HeadgearIDBlueAttacker = id;
            else if (team == PlayerTeam.Red && role == PlayerRole.Attacker) s.HeadgearIDRedAttacker = id;
            else if (team == PlayerTeam.Blue && role == PlayerRole.Goalie) s.HeadgearIDBlueGoalie = id;
            else if (team == PlayerTeam.Red && role == PlayerRole.Goalie) s.HeadgearIDRedGoalie = id;
        }

        private static void SetJerseyForTeamRole(ref PlayerCustomizationState s, PlayerTeam team, PlayerRole role, int id)
        {
            if (team == PlayerTeam.Blue && role == PlayerRole.Attacker) s.JerseyIDBlueAttacker = id;
            else if (team == PlayerTeam.Red && role == PlayerRole.Attacker) s.JerseyIDRedAttacker = id;
            else if (team == PlayerTeam.Blue && role == PlayerRole.Goalie) s.JerseyIDBlueGoalie = id;
            else if (team == PlayerTeam.Red && role == PlayerRole.Goalie) s.JerseyIDRedGoalie = id;
        }

        private static void SetStickSkinForTeamRole(ref PlayerCustomizationState s, PlayerTeam team, PlayerRole role, int id)
        {
            if (team == PlayerTeam.Blue && role == PlayerRole.Attacker) s.StickSkinIDBlueAttacker = id;
            else if (team == PlayerTeam.Red && role == PlayerRole.Attacker) s.StickSkinIDRedAttacker = id;
            else if (team == PlayerTeam.Blue && role == PlayerRole.Goalie) s.StickSkinIDBlueGoalie = id;
            else if (team == PlayerTeam.Red && role == PlayerRole.Goalie) s.StickSkinIDRedGoalie = id;
        }

        private static void SetShaftTapeForTeamRole(ref PlayerCustomizationState s, PlayerTeam team, PlayerRole role, int id)
        {
            if (team == PlayerTeam.Blue && role == PlayerRole.Attacker) s.StickShaftTapeIDBlueAttacker = id;
            else if (team == PlayerTeam.Red && role == PlayerRole.Attacker) s.StickShaftTapeIDRedAttacker = id;
            else if (team == PlayerTeam.Blue && role == PlayerRole.Goalie) s.StickShaftTapeIDBlueGoalie = id;
            else if (team == PlayerTeam.Red && role == PlayerRole.Goalie) s.StickShaftTapeIDRedGoalie = id;
        }

        private static void SetBladeTapeForTeamRole(ref PlayerCustomizationState s, PlayerTeam team, PlayerRole role, int id)
        {
            if (team == PlayerTeam.Blue && role == PlayerRole.Attacker) s.StickBladeTapeIDBlueAttacker = id;
            else if (team == PlayerTeam.Red && role == PlayerRole.Attacker) s.StickBladeTapeIDRedAttacker = id;
            else if (team == PlayerTeam.Blue && role == PlayerRole.Goalie) s.StickBladeTapeIDBlueGoalie = id;
            else if (team == PlayerTeam.Red && role == PlayerRole.Goalie) s.StickBladeTapeIDRedGoalie = id;
        }
    }
}
