using HarmonyLib;
using PuckAIPractice.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ServerBrowserInGame
{
    public class InitializeServerBrowserInGame : IPuckMod
    {
        static readonly Harmony harmony = new Harmony("GAFURIX.ServerBrowserInGame");

        public bool OnEnable()
        {
            Debug.Log("[ServerBrowserInGame] Enabled");
            try
            {
                harmony.PatchAll();
                PauseMenuServerBrowserInjector.Install();
                //HarmonyLogger.PatchSpecificMethods(harmony, typeof(Player), new List<string>() { "OnNetworkSpawn", "OnNetworkPostSpawn", "Client_SetPlayerStateRpc", "Server_RespawnCharacter", "Server_DespawnCharacter" });
                //HarmonyLogger.PatchAllMethods(harmony, typeof(UIManager));
            }
            catch (Exception e)
            {
                Debug.LogError($"[ServerBrowserInGame] Harmony patch failed: {e}");
                return false;
            }

            return true;
        }

        public bool OnDisable()
        {
            try
            {
                harmony.UnpatchSelf();
                PauseMenuServerBrowserInjector.Uninstall();
            }
            catch (Exception e)
            {
                Debug.LogError($"[PuckAIPractice] Harmony unpatch failed: {e}");
                return false;
            }

            return true;
        }
    }
}
