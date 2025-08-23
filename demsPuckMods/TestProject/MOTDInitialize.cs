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
namespace MOTD
{
    public class MOTDInitialize : IPuckMod
    {
        static readonly Harmony harmony = new Harmony("GAFURIX.MOTD");
        public bool OnDisable()
        {
            throw new NotImplementedException();
        }

        public bool OnEnable()
        {
            harmony.PatchAll();
            ModConfig.Initialize();
            ConfigData.Load();
            SimpleModal.Install();
            return true;
        }
    }
}