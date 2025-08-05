using HarmonyLib;
using PuckAIPractice.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuckAIPractice.Patches
{
    [HarmonyPatch(typeof(PlayerManager), "GetPlayers")]
    public static class ExcludeFakePlayersFromGetPlayers
    {
        [HarmonyPostfix]
        public static void Postfix(ref List<Player> __result, bool includeReplay)
        {
            __result = __result
                .Where(p => !FakePlayerRegistry.IsFake(p))
                .ToList();
        }
    }
}
