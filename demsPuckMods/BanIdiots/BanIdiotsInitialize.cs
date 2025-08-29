using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BanIdiots.Config;
namespace BanIdiots
{
    public class BanIdiots : IPuckMod
    {
        static readonly Harmony harmony = new Harmony("GAFURIX.BanIdiots");
        public bool OnDisable()
        {
            harmony.UnpatchSelf();
            return true;
        }

        public bool OnEnable()
        {
            harmony.PatchAll();
            ModConfig.Initialize();
            ConfigData.Load();
            return true;
        }
    }
}
