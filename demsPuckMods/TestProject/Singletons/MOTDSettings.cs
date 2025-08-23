using MOTD.Behaviors;
using MOTD.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TestProject.Singletons
{
    public sealed class MOTDSettings
    {
        [JsonProperty("modalDoc")]
        public ModalDoc ModalDoc { get; set; }

        [JsonProperty("theme")]
        public ThemeDto Theme { get; set; }
    }

    public sealed class ThemeDto
    {
        public string overlay;
        public string panel;
        public string text;
        public string mutedText;
        public string button;
        public string buttonHover;
        public string buttonActive;
        public string toggleBox;
        public string toggleCheck;
        public string scrollbarTrack;
        public string scrollbarThumb;
    }

    public static class ThemeMapper
    {
        // Parse HTML hex (#RRGGBB or #RRGGBBAA). If missing '#', we add it.
        public static Color Hex(string input, Color fallback)
        {
            if (string.IsNullOrWhiteSpace(input)) return fallback;

            var s = input.Trim();
            if (!s.StartsWith("#")) s = "#" + s;

            // Accept both #RRGGBB and #RRGGBBAA
            if (ColorUtility.TryParseHtmlString(s, out var c))
                return c;

            return fallback;
        }

        public static SimpleModal.Theme ToTheme(ThemeDto dto, SimpleModal.Theme fallback)
        {
            if (dto == null) return fallback;

            return new SimpleModal.Theme
            {
                Overlay = Hex(dto.overlay, fallback.Overlay),
                Panel = Hex(dto.panel, fallback.Panel),
                Text = Hex(dto.text, fallback.Text),
                MutedText = Hex(dto.mutedText, fallback.MutedText),
                Button = Hex(dto.button, fallback.Button),
                ButtonHover = Hex(dto.buttonHover, fallback.ButtonHover),
                ButtonActive = Hex(dto.buttonActive, fallback.ButtonActive),
                ToggleBox = Hex(dto.toggleBox, fallback.ToggleBox),
                ToggleCheck = Hex(dto.toggleCheck, fallback.ToggleCheck),
                ScrollbarTrack = Hex(dto.scrollbarTrack, fallback.ScrollbarTrack),
                ScrollbarThumb = Hex(dto.scrollbarThumb, fallback.ScrollbarThumb),
            };
        }
    }
}
