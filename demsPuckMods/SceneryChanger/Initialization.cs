using HarmonyLib;
using SceneryLoader.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SceneryLoader.Singletons;
using SceneryLoader.Config;
using UnityEngine;
using SceneryChanger.Behaviors;
namespace SceneryChanger
{
    public class Initialization : IPuckMod
    {
        static readonly Harmony harmony = new Harmony("GAFURIX.SceneryChanger");
        public bool OnDisable()
        {
            harmony.UnpatchSelf();
            try
            {
                CoroutineRunner.Uninstall();
                RinkOnlyPruner.Uninstall();
                DetectGameState.Uninstall();
                UpdateAudioSources.Uninstall();
            }
            catch(Exception e)
            {
                Debug.Log(e.StackTrace.ToString());
            }
            //DisableAmbientCrowd.Uninstall();
            return true;
        }

        public bool OnEnable()
        {
            Debug.Log("Entering On Enable");
            harmony.PatchAll();
            Debug.Log("Harmony Patched");
            ModConfig.Initialize();
            Debug.Log("Config Init");
            ConfigData.Load();
            Debug.Log("Config Load");
            CoroutineRunner.Install();
            Debug.Log("Coroutine Runner Load");
            try
            {
                RinkOnlyPruner.Install();
                Debug.Log("Rink Prune Installed");
            }
            catch(Exception ex)
            {
                Debug.Log("WTF Happened? " + ex.Message.ToString());
            }
            
            DetectGameState.Install();
            UpdateAudioSources.Install();
            //DisableAmbientCrowd.Install();
            Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
            // Larger buffer and more time slice = smoother uploads, fewer stalls
            QualitySettings.asyncUploadBufferSize = 64;   // MB
            QualitySettings.asyncUploadTimeSlice = 4;   // ms per frame
            QualitySettings.asyncUploadPersistentBuffer = true;
            return true;
        }
    }
}