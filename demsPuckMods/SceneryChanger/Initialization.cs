using HarmonyLib;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace SceneryChanger
{
    public class Initialization : IPuckPlugin
    {
        static readonly Harmony harmony = new Harmony("GAFURIX.SceneryChanger");

        // === TLE DIAGNOSTIC ===
        // Phase 1: Set NOOP=true. If TLE still floods, the assembly itself is the problem.
        //          If TLE stops, set NOOP=false and move to Phase 2.
        // Phase 2: Uncomment lines in DoEnable() one group at a time to find the source.
        static readonly bool NOOP = false;

        public bool OnEnable()
        {
            if (NOOP)
            {
                Debug.Log("[SceneryChanger] NOOP mode — mod loaded but doing nothing");
                return true;
            }
            return DoEnable();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        bool DoEnable()
        {
            Debug.Log("Entering On Enable");

            // --- GROUP 1: Patches ---
            harmony.PatchAll();

            // --- GROUP 2: Config ---
            SceneryLoader.Config.ModConfig.Initialize();
            SceneryLoader.Singletons.ConfigData.Load();

            // --- GROUP 3: Behaviors ---
            SceneryChanger.Behaviors.CoroutineRunner.Install();
            SceneryLoader.Behaviors.RinkOnlyPruner.Install();
            SceneryChanger.Behaviors.DetectGameState.Install();
            SceneryChanger.Behaviors.UpdateAudioSources.Install();

            // --- GROUP 4: Unity settings ---
            Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
            QualitySettings.asyncUploadBufferSize = 64;
            QualitySettings.asyncUploadTimeSlice = 4;
            QualitySettings.asyncUploadPersistentBuffer = true;

            return true;
        }

        public bool OnDisable()
        {
            if (NOOP) return true;
            harmony.UnpatchSelf();
            try
            {
                SceneryChanger.Behaviors.CoroutineRunner.Uninstall();
                SceneryLoader.Behaviors.RinkOnlyPruner.Uninstall();
                SceneryChanger.Behaviors.DetectGameState.Uninstall();
                SceneryChanger.Behaviors.UpdateAudioSources.Uninstall();
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace.ToString());
            }
            return true;
        }
    }
}
