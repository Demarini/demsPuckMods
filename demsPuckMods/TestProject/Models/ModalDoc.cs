using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOTD.Models
{
    [Serializable]
    public sealed class ModalDoc
    {
        public string title;
        public string richText;
        public string bannerImageUrl;
        public string panelImageUrl;
        public float panelWidthPercent = 60f;
        public float panelHeightPercent = 80f;

        public string discordUrl;
        public string discordImageUrl;
        public float discordSize = 44f;

        public string actionUrl;
        public string actionImageUrl;
        public float actionSize = 52f;

        public int closeDelaySeconds = 0;              // 0 = no delay
        public bool blockEscDuringDelay = true;        // prevent ESC until timer ends
        public bool blockClickOutsideDuringDelay = true; // prevent click-to-close outside panel

        public List<ButtonSpec> buttons { get; set; }
    }
    public sealed class ButtonSpec
    {
        public string name;
        public string url;
        public string iconUrl;
        public float height = 60f;     // px
        public float aspect = 2.5f;    // width = height * aspect
        public float corner = 8f;      // px border radius
        public float marginRight = 12f;
        public float pad = 0f;
        public float choke = 0f;
        public string backgroundColor;
        public string backgroundHover;
        public string backgroundActive;
    }
}