﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuckAIPractice
{
    public static class RandomSkins
    {
        private static System.Random rng = new System.Random();

        public static T RandomFrom<T>(this T[] array) => array[rng.Next(array.Length)];

        public static string GetRandomJersey() => JerseySkins.RandomFrom();
        public static string GetRandomMustache() => MustacheStyles.RandomFrom();
        public static string GetRandomBeard() => BeardStyles.RandomFrom();
        public static string GetRandomStickSkin(PlayerRole role) => role == PlayerRole.Attacker ? StickAttackerSkins.RandomFrom() : StickGoalieSkins.RandomFrom();
        public static string GetRandomBladeTape(PlayerRole role) => role == PlayerRole.Attacker ? StickBladeTapeAttackerSkins.RandomFrom() : StickBladeTapeGoalieSkins.RandomFrom();
        public static string GetRandomShaftTape(PlayerRole role) => role == PlayerRole.Attacker ? StickShaftTapeAttackerSkins.RandomFrom() : StickShaftTapeGoalieSkins.RandomFrom();
        public static string GetRandomVisor() => VisorSkins_Attacker.RandomFrom();
        public static string GetRandomCountry() => CountryCodes.RandomFrom();
        // Your actual string arrays here...
        public static string[] VisorSkins_Attacker = new string[]
{
    "none",
    "visor_default_attacker",
    "visor_tinted_25_attacker",
    "visor_tinted_50_attacker",
    "visor_tinted_75_attacker"
};
        static string[] CountryCodes = new string[]
{
    "none", "AD", "AE", "AF", "AG", "AI", "AL", "AM", "AO", "AQ", "AR", "AS", "AT", "AU", "AW", "AX", "AZ",
    "BA", "BB", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BL", "BM", "BN", "BO", "BQ", "BR", "BS", "BT", "BV",
    "BW", "BY", "BZ", "CA", "CC", "CD", "CF", "CG", "CH", "CI", "CK", "CL", "CM", "CN", "CO", "CR", "CU", "CV",
    "CW", "CX", "CY", "CZ", "DE", "DJ", "DK", "DM", "DO", "DZ", "EC", "EE", "EG", "EH", "ER", "ES", "ET", "EU",
    "FI", "FJ", "FK", "FM", "FO", "FR", "GA", "GB-ENG", "GB-NIR", "GB-SCT", "GB-WLS", "GB", "GD", "GE", "GF",
    "GG", "GH", "GI", "GL", "GM", "GN", "GP", "GQ", "GR", "GS", "GT", "GU", "GW", "GY", "HK", "HM", "HN", "HR",
    "HT", "HU", "ID", "IE", "IL", "IM", "IN", "IO", "IQ", "IR", "IS", "IT", "JE", "JM", "JO", "JP", "KE", "KG",
    "KH", "KI", "KM", "KN", "KP", "KR", "KW", "KY", "KZ", "LA", "LB", "LC", "LI", "LK", "LR", "LS", "LT", "LU",
    "LV", "LY", "MA", "MC", "MD", "ME", "MF", "MG", "MH", "MK", "ML", "MM", "MN", "MO", "MP", "MQ", "MR", "MS",
    "MT", "MU", "MV", "MW", "MX", "MY", "MZ", "NA", "NC", "NE", "NF", "NG", "NI", "NL", "NO", "NP", "NR", "NU",
    "NZ", "OM", "PA", "PE", "PF", "PG", "PH", "PK", "PL", "PM", "PN", "PR", "PS", "PT", "PW", "PY", "QA", "RE",
    "RO", "RS", "RU", "RW", "SA", "SB", "SC", "SD", "SE", "SG", "SH", "SI", "SJ", "SK", "SL", "SM", "SN", "SO",
    "SR", "SS", "ST", "SV", "SX", "SY", "SZ", "TC", "TD", "TF", "TG", "TH", "TJ", "TK", "TL", "TM", "TN", "TO",
    "TR", "TT", "TV", "TW", "TZ", "UA", "UG", "UM", "US", "UY", "UZ", "VA", "VC", "VE", "VG", "VI", "VN", "VU",
    "WF", "WS", "XK", "YE", "YT", "ZA", "ZM", "ZW"
};

        static string[] MustacheStyles = new string[]
{
    "none", "chevron", "lampshade", "pencil", "sheriff", "walrus", "hqm", "toothbrush"
};
        static string[] BeardStyles = new string[]
{
    "full", "chin_curtain", "goatee", "mutton_chops", "spade"
};
        static string[] JerseySkins = new string[]
{
    "default",
    "black_color_black_stripe",
    "black_stripe",
    "black_white_black_stripe",
    "white_black_white_stripe",
    "white_color_white_stripe",
    "white_stripe"
};
        public static readonly string[] StickAttackerSkins = new string[]
{
    "classic_attacker",
    "beta_attacker",
    "camouflage_attacker",
    "apex_green_attacker",
    "apex_purple_attacker",
    "apex_red_attacker",
    "carbon_blue_attacker",
    "carbon_green_attacker",
    "carbon_red_attacker",
    "carbon_white_attacker",
    "reinforcer_blue_attacker",
    "reinforcer_pink_attacker",
    "reinforcer_yellow_attacker"
};
        public static readonly string[] StickShaftTapeAttackerSkins = new string[]
{
    "none",
    "blue_cloth",
    "gray_cloth",
    "red_cloth",
    "black_cloth",
    "dark_gray_cloth",
    "green_cloth",
    "light_blue_cloth",
    "lime_cloth",
    "magenta_cloth",
    "navy_cloth",
    "orange_cloth",
    "pink_cloth",
    "purple_cloth",
    "teal_cloth",
    "white_cloth",
    "yellow_cloth"
};
        public static readonly string[] StickBladeTapeAttackerSkins = new string[]
{
    "none",
    "blue_cloth",
    "gray_cloth",
    "red_cloth",
    "black_cloth",
    "dark_gray_cloth",
    "green_cloth",
    "light_blue_cloth",
    "lime_cloth",
    "magenta_cloth",
    "navy_cloth",
    "orange_cloth",
    "pink_cloth",
    "purple_cloth",
    "teal_cloth",
    "white_cloth",
    "yellow_cloth"
};
        public static readonly string[] StickGoalieSkins = new string[]
{
    "classic_goalie",
    "beta_goalie",
    "havoc_black_goalie",
    "havoc_silver_goalie",
    "vapor_magenta_goalie",
    "vapor_silver_goalie",
    "vapor_teal_goalie",
    "digital_goalie"
};
        public static readonly string[] StickShaftTapeGoalieSkins = new string[]
{
    "none",
    "black_cloth",
    "blue_cloth",
    "dark_gray_cloth",
    "gray_cloth",
    "green_cloth",
    "light_blue_cloth",
    "lime_cloth",
    "magenta_cloth",
    "navy_cloth",
    "orange_cloth",
    "pink_cloth",
    "purple_cloth",
    "red_cloth",
    "teal_cloth",
    "white_cloth",
    "yellow_cloth"
};
        public static readonly string[] StickBladeTapeGoalieSkins = new string[]
{
    "none",
    "black_cloth",
    "blue_cloth",
    "dark_gray_cloth",
    "gray_cloth",
    "green_cloth",
    "light_blue_cloth",
    "lime_cloth",
    "magenta_cloth",
    "navy_cloth",
    "orange_cloth",
    "pink_cloth",
    "purple_cloth",
    "red_cloth",
    "teal_cloth",
    "white_cloth",
    "yellow_cloth"
};
    }
}
