using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModifyMinimapIcons
{
    public static class OpenLineManager
    {
        private static Dictionary<PlayerBodyV2, LineRenderer> activeLines = new Dictionary<PlayerBodyV2, LineRenderer>();
        private static Material lineMaterial;

        public static void Init()
        {
            if (!lineMaterial)
            {
                lineMaterial = new Material(Shader.Find("Sprites/Default"));
            }
        }

        private static LineRenderer CreateLine()
        {
            GameObject lineObj = new GameObject("OpenPassLine");
            var lr = lineObj.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.startColor = Color.yellow;
            lr.endColor = Color.yellow;
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.positionCount = 2;
            return lr;
        }

        public static void UpdateLine(PlayerBodyV2 localPlayer, PlayerBodyV2 teammate)
        {
            if (!activeLines.TryGetValue(teammate, out var lr))
            {
                lr = CreateLine();
                activeLines[teammate] = lr;
            }

            lr.enabled = true;
            lr.SetPosition(0, localPlayer.transform.position + Vector3.up * 0.5f); // Slightly above ground
            lr.SetPosition(1, teammate.transform.position + Vector3.up * 0.5f);
        }

        public static void HideLine(PlayerBodyV2 teammate)
        {
            if (activeLines.TryGetValue(teammate, out var lr))
            {
                lr.enabled = false;
            }
        }

        public static void Clear()
        {
            foreach (var kvp in activeLines)
            {
                if (kvp.Value) UnityEngine.Object.Destroy(kvp.Value.gameObject);
            }
            activeLines.Clear();
        }
    }
}
