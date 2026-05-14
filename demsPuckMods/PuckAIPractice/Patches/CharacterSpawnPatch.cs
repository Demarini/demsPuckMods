using HarmonyLib;
using PuckAIPractice.Utilities;

namespace PuckAIPractice.Patches
{
    // StandardGameMode runs Server_SpawnCharacter on every Player at the start of
    // each Play phase (post-goal, period start, game start). For fake-bot Players
    // that survived the previous Play phase (i.e. anything other than the Replay
    // path, which despawns them via DetectPositions), this attaches a second
    // character NetworkObject to the same Player without despawning the first —
    // accumulating "stacked goalies" across periods and games.
    //
    // Skip the game's spawn when the fake bot already has a character; SpawnFakePlayer
    // itself still works because it runs before any character exists.
    [HarmonyPatch(typeof(Player), nameof(Player.Server_SpawnCharacter))]
    public static class BlockDuplicateCharacterSpawnForFakeBot
    {
        static bool Prefix(Player __instance)
        {
            if (__instance == null) return true;
            if (!FakePlayerRegistry.IsFakeClientId(__instance.OwnerClientId)) return true;
            if (__instance.IsCharacterSpawned) return false;
            return true;
        }
    }
}
