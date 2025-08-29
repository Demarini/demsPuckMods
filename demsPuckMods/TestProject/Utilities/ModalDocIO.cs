using MOTD.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProject.Singletons;
using UnityEngine;

namespace TestProject.Utilities
{
    public static class ModalDocIO
    {
        public static string DefaultPath =>
            Path.Combine(Application.persistentDataPath, "motd.json");

        static string NormalizeRich(string s) =>
            string.IsNullOrEmpty(s) ? s
            : s.Replace("<br/>", "\n").Replace("<br>", "\n")
               .Replace("<BR/>", "\n").Replace("<BR>", "\n");

        public static bool TryLoad(string message, out MOTDSettings doc, out string error)
        {
            doc = null; error = null;
            try
            {
                var json = message.Replace("!MOTD ", "");
                //Debug.Log(json);
                var settings = JsonConvert.DeserializeObject<MOTDSettings>(json);
                var parsed = settings.ModalDoc;
                if(parsed == null)
                {
                    //Debug.Log("Modal Doc is null.");
                }
                if(settings.Theme == null)
                {
                    //Debug.Log("Theme Is Null");
                }
                parsed.richText = NormalizeRich(parsed.richText);
                //Debug.Log(parsed.richText);
                parsed.title = string.IsNullOrWhiteSpace(parsed.title) ? "" : parsed.title;
                //Debug.Log(parsed.title);

                doc = settings;
                return true;
            }
            catch (Exception ex)
            {
                error = $"Failed to load MOTD: {ex}";
                return false;
            }
        }
        public static string MdToUnity(string md)
        {
            if (string.IsNullOrEmpty(md)) return md;

            // headings -> bigger size (tweak values to taste)
            md = System.Text.RegularExpressions.Regex.Replace(md, @"(?m)^###\s+(.*)$", "<size=18><b>$1</b></size>");
            md = System.Text.RegularExpressions.Regex.Replace(md, @"(?m)^##\s+(.*)$", "<size=22><b>$1</b></size>");
            md = System.Text.RegularExpressions.Regex.Replace(md, @"(?m)^#\s+(.*)$", "<size=26><b>$1</b></size>");

            // bold / italic
            md = System.Text.RegularExpressions.Regex.Replace(md, @"\*\*(.+?)\*\*", "<b>$1</b>");
            md = System.Text.RegularExpressions.Regex.Replace(md, @"\*(.+?)\*", "<i>$1</i>");

            // unordered lists -> bullets
            md = System.Text.RegularExpressions.Regex.Replace(md, @"(?m)^\s*[-*]\s+(.*)$", "• $1");

            // code blocks/inline are usually not needed; you can wrap with <color> if desired
            // md = Regex.Replace(md, @"`(.+?)`", "<color=#CCCCCC>$1</color>");

            // paragraphs: blank line -> extra newline
            md = md.Replace("\r\n", "\n");
            md = System.Text.RegularExpressions.Regex.Replace(md, @"\n{2,}", "\n\n");

            // final: Unity likes literal '\n' newlines
            return md;
        }
    }
}
