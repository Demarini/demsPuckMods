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

            // Anywhere later:
            //SimpleModal.Show(
            //    "Server Browser — Notes",
            //    "<b>Welcome!</b><br/>You can <i>format</i> this text with basic rich text.<br/><br/>• Bullet 1<br/>• Bullet 2"
            //);

            // Add arbitrary UI in the body:
            SimpleModal.Show("Changelog", null, body =>
            {
                var p = new Label { enableRichText = true, text = "<b>v1.2.3</b> – Fixed stuff" };
                body.Add(p);
                var ok = new Button(() => SimpleModal.Hide()) { text = "GOT IT" };
                body.Add(ok);
            });
            return true;
        }
    }
}
