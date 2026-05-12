using HarmonyLib;
using System;
using UnityEngine;

namespace AutoJoin
{
    public class AutoJoinInitialize : IPuckPlugin
    {
        static readonly Harmony harmony = new Harmony("dems.AutoJoin");

        public bool OnEnable()
        {
            Debug.Log("[AutoJoin] Enabled");
            try
            {
                harmony.PatchAll();
                AutoJoinManager.Instance.Initialize();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[AutoJoin] Initialization failed: {e}");
                return false;
            }
        }

        public bool OnDisable()
        {
            try
            {
                AutoJoinManager.Instance.Shutdown();
                harmony.UnpatchSelf();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[AutoJoin] Cleanup failed: {e}");
                return false;
            }
        }
    }
}
