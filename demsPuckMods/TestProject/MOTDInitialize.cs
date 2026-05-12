using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOTD.Behaviors;
using UnityEngine.UIElements;
using MOTD.Config;
using MOTD.Singletons;
using System.IO;
using Newtonsoft.Json;
using TestProject.Singletons;
using UnityEngine;
using UnityEngine.Rendering;
namespace MOTD
{
    public class MOTDInitialize : IPuckPlugin
    {
        static readonly Harmony harmony = new Harmony("GAFURIX.MOTD");
        public bool OnDisable()
        {
            harmony.UnpatchSelf();
            SimpleModal.Uninstall();
            return true;
        }

        public bool OnEnable()
        {
            harmony.PatchAll();
            MOTD.Config.MOTDConfig.Initialize();
            ConfigData.Load();
            if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null)
                SimpleModal.Install();
            return true;
        }
    }
}