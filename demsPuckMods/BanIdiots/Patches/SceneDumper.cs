using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace BanIdiots.Patches
{
    public static class SceneDumper
    {
        public static void DumpActiveSceneHierarchy(Scene scene)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"[SceneDump] --- Scene: {scene.name} (buildIndex={scene.buildIndex}) ---");

            foreach (var root in scene.GetRootGameObjects())
            {
                DumpObjectRecursive(root.transform, sb, 0);
            }

            Debug.Log(sb.ToString());
        }

        private static void DumpObjectRecursive(Transform t, StringBuilder sb, int depth)
        {
            string indent = new string(' ', depth * 2);
            sb.AppendLine($"{indent}- {t.name} [{t.gameObject.layer}]");

            // Optional: dump components
            foreach (var comp in t.GetComponents<Component>())
            {
                if (comp == null) continue;
                sb.AppendLine($"{indent}  • {comp.GetType().FullName}");
            }

            // Recurse children
            foreach (Transform child in t)
            {
                DumpObjectRecursive(child, sb, depth + 1);
            }
        }
    }
}
