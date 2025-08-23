using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProject.Behaviors;
using UnityEngine.UIElements;

namespace TestProject
{
    public class TestProjectInitialize : IPuckMod
    {
        static readonly Harmony harmony = new Harmony("GAFURIX.TestProject");
        public bool OnDisable()
        {
            throw new NotImplementedException();
        }

        public bool OnEnable()
        {
            harmony.PatchAll();
            SimpleModal.Install();
            return true;
        }
    }
}