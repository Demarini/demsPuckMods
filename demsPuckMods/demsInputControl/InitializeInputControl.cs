using demsInputControl.Config;
using demsInputControl.Singletons;
using demsInputControl.UnityPlugins;
using HarmonyLib;
using HarmonyLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace demsInputControl
{
    public class InitializeInputControl : IPuckMod
    {
        private static readonly Harmony harmony = new Harmony("GAFURIX.InputControl");

        public bool OnEnable()
        {
            Debug.Log("[InputControl] Mod enabled");
            ModConfig.Initialize();
            ConfigData.Load();
            PluginBehaviour.Initialize();
            //HarmonyFileLog.Enabled = true;
            try
            {
                //foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                //{
                //    foreach (var type in asm.GetTypes().Where(t => t.Name.StartsWith("NetworkedInput")))
                //    {
                //        Debug.Log($"[InputControl] Found input type: {type.FullName}");
                //        foreach (var f in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                //        {
                //            Debug.Log($"[InputControl]   Field: {f.Name} ({f.FieldType})");
                //        }
                //    }
                //}

                harmony.PatchAll();
                Debug.Log("[InputControl] Harmony patches applied");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[InputControl] Harmony patch failed: {e.Message}");
                return false;
            }

            return true;
        }

        public bool OnDisable()
        {
            try
            {
                harmony.UnpatchSelf();
                Debug.Log("[InputControl] Mod disabled");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[InputControl] Harmony unpatch failed: {e.Message}");
                return false;
            }

            return true;
        }
    }
}
