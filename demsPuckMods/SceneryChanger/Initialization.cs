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
            RinkOnlyPruner.Uninstall();
            //DisableAmbientCrowd.Uninstall();
            return true;
        }

        public bool OnEnable()
        {
            harmony.PatchAll();
            ModConfig.Initialize();
            ConfigData.Load();
            RinkOnlyPruner.Install();
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
