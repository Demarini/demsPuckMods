using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001DD RID: 477
public static class Constants
{
	// Token: 0x04000827 RID: 2087
	public const uint APP_ID = 2994020U;

	// Token: 0x04000828 RID: 2088
	public const float STEAM_INITIALIZATION_RETRY_DELAY = 5f;

	// Token: 0x04000829 RID: 2089
	public static readonly Dictionary<EdgegapDependency, float> EDGEGAP_DEPENDENCY_TIMEOUTS = new Dictionary<EdgegapDependency, float>
	{
		{
			EdgegapDependency.IsAuthenticated,
			60f
		},
		{
			EdgegapDependency.IsOccupied,
			60f
		}
	};

	// Token: 0x0400082A RID: 2090
	public const int WEB_SOCKET_CONNECTION_TIMEOUT = 5000;

	// Token: 0x0400082B RID: 2091
	public const string TEAM_BLUE_COLOR = "#3b82f6";

	// Token: 0x0400082C RID: 2092
	public const string TEAM_RED_COLOR = "#d13333";

	// Token: 0x0400082D RID: 2093
	public const string TEAM_SPECTATOR_COLOR = "#404040";

	// Token: 0x0400082E RID: 2094
	public const string PATREON_COLOR = "#f1c40f";

	// Token: 0x0400082F RID: 2095
	public const string MODERATOR_COLOR = "#206694";

	// Token: 0x04000830 RID: 2096
	public const string ADMIN_COLOR = "#992d22";

	// Token: 0x04000831 RID: 2097
	public const string DEVELOPER_COLOR = "#71368a";

	// Token: 0x04000832 RID: 2098
	public const bool DEFAULT_SETTINGS_DEBUG = false;

	// Token: 0x04000833 RID: 2099
	public const float DEFAULT_SETTINGS_CAMERA_ANGLE = 30f;

	// Token: 0x04000834 RID: 2100
	public const PlayerHandedness DEFAULT_SETTINGS_HANDEDNESS = PlayerHandedness.Right;

	// Token: 0x04000835 RID: 2101
	public const bool DEFAULT_SETTINGS_SHOW_PUCK_SILHOUETTE = true;

	// Token: 0x04000836 RID: 2102
	public const bool DEFAULT_SETTINGS_SHOW_PUCK_OUTLINE = false;

	// Token: 0x04000837 RID: 2103
	public const bool DEFAULT_SETTINGS_SHOW_PUCK_ELEVATION = true;

	// Token: 0x04000838 RID: 2104
	public const bool DEFAULT_SETTINGS_SHOW_PLAYER_USERNAMES = false;

	// Token: 0x04000839 RID: 2105
	public const float DEFAULT_SETTINGS_PLAYER_USERNAMES_FADE_THRESHOLD = 1f;

	// Token: 0x0400083A RID: 2106
	public const bool DEFAULT_SETTINGS_USE_NETWORK_SMOOTHING = false;

	// Token: 0x0400083B RID: 2107
	public const int DEFAULT_SETTINGS_NETWORK_SMOOTHING_STRENGTH = 1;

	// Token: 0x0400083C RID: 2108
	public const int DEFAULT_SETTINGS_MAX_MATCHMAKING_RTT = 50;

	// Token: 0x0400083D RID: 2109
	public const bool DEFAULT_SETTINGS_FILTER_CHAT_PROFANITY = true;

	// Token: 0x0400083E RID: 2110
	public const Units DEFAULT_SETTINGS_UNITS = Units.Metric;

	// Token: 0x0400083F RID: 2111
	public const bool DEFAULT_SETTINGS_SHOW_GAME_USER_INTERFACE = true;

	// Token: 0x04000840 RID: 2112
	public const float DEFAULT_SETTINGS_USER_INTERFACE_SCALE = 1f;

	// Token: 0x04000841 RID: 2113
	public const float DEFAULT_SETTINGS_CHAT_OPACITY = 1f;

	// Token: 0x04000842 RID: 2114
	public const float DEFAULT_SETTINGS_CHAT_SCALE = 1f;

	// Token: 0x04000843 RID: 2115
	public const float DEFAULT_SETTINGS_MINIMAP_OPACITY = 1f;

	// Token: 0x04000844 RID: 2116
	public const float DEFAULT_SETTINGS_MINIMAP_BACKGROUND_OPACITY = 1f;

	// Token: 0x04000845 RID: 2117
	public const float DEFAULT_SETTINGS_MINIMAP_HORIZONTAL_POSITION = 100f;

	// Token: 0x04000846 RID: 2118
	public const float DEFAULT_SETTINGS_MINIMAP_VERTICAL_POSITION = 0f;

	// Token: 0x04000847 RID: 2119
	public const float DEFAULT_SETTINGS_MINIMAP_SCALE = 1f;

	// Token: 0x04000848 RID: 2120
	public const float DEFAULT_SETTINGS_GLOBAL_STICK_SENSITIVITY = 0.2f;

	// Token: 0x04000849 RID: 2121
	public const float DEFAULT_SETTINGS_HORIZONTAL_STICK_SENSITIVITY = 1f;

	// Token: 0x0400084A RID: 2122
	public const float DEFAULT_SETTINGS_VERTICAL_STICK_SENSITIVITY = 1f;

	// Token: 0x0400084B RID: 2123
	public const float DEFAULT_SETTINGS_LOOK_SENSITIVITY = 0.2f;

	// Token: 0x0400084C RID: 2124
	public const float DEFAULT_SETTINGS_GLOBAL_VOLUME = 0.5f;

	// Token: 0x0400084D RID: 2125
	public const float DEFAULT_SETTINGS_AMBIENT_VOLUME = 1f;

	// Token: 0x0400084E RID: 2126
	public const float DEFAULT_SETTINGS_GAME_VOLUME = 1f;

	// Token: 0x0400084F RID: 2127
	public const float DEFAULT_SETTINGS_VOICE_VOLUME = 1f;

	// Token: 0x04000850 RID: 2128
	public const float DEFAULT_SETTINGS_UI_VOLUME = 0.5f;

	// Token: 0x04000851 RID: 2129
	public const FullScreenMode DEFAULT_SETTINGS_FULL_SCREEN_MODE = FullScreenMode.FullScreenWindow;

	// Token: 0x04000852 RID: 2130
	public const int DEFAULT_SETTINGS_DISPLAY_INDEX = 0;

	// Token: 0x04000853 RID: 2131
	public const int DEFAULT_SETTINGS_RESOLUTION_INDEX = -1;

	// Token: 0x04000854 RID: 2132
	public const bool DEFAULT_SETTINGS_VSYNC = false;

	// Token: 0x04000855 RID: 2133
	public const int DEFAULT_SETTINGS_FPS_LIMIT = 240;

	// Token: 0x04000856 RID: 2134
	public const float DEFAULT_SETTINGS_FOV = 90f;

	// Token: 0x04000857 RID: 2135
	public const ApplicationQuality DEFAULT_SETTINGS_QUALITY = ApplicationQuality.High;

	// Token: 0x04000858 RID: 2136
	public const bool DEFAULT_SETTINGS_MOTION_BLUR = true;

	// Token: 0x04000859 RID: 2137
	public const PlayerTeam DEFAULT_TEAM = PlayerTeam.Blue;

	// Token: 0x0400085A RID: 2138
	public const PlayerRole DEFAULT_ROLE = PlayerRole.Attacker;

	// Token: 0x0400085B RID: 2139
	public const bool DEFAULT_APPLY_FOR_BOTH_TEAMS = false;

	// Token: 0x0400085C RID: 2140
	public const int DEFAULT_FLAG_ID = -1;

	// Token: 0x0400085D RID: 2141
	public const int DEFAULT_HEADGEAR_ID_BLUE_ATTACKER = 513;

	// Token: 0x0400085E RID: 2142
	public const int DEFAULT_HEADGEAR_ID_RED_ATTACKER = 513;

	// Token: 0x0400085F RID: 2143
	public const int DEFAULT_HEADGEAR_ID_BLUE_GOALIE = 527;

	// Token: 0x04000860 RID: 2144
	public const int DEFAULT_HEADGEAR_ID_RED_GOALIE = 527;

	// Token: 0x04000861 RID: 2145
	public const int DEFAULT_MUSTACHE_ID = -1;

	// Token: 0x04000862 RID: 2146
	public const int DEFAULT_BEARD_ID = -1;

	// Token: 0x04000863 RID: 2147
	public const int DEFAULT_JERSEY_ID_BLUE_ATTACKER = 2048;

	// Token: 0x04000864 RID: 2148
	public const int DEFAULT_JERSEY_ID_RED_ATTACKER = 2048;

	// Token: 0x04000865 RID: 2149
	public const int DEFAULT_JERSEY_ID_BLUE_GOALIE = 2048;

	// Token: 0x04000866 RID: 2150
	public const int DEFAULT_JERSEY_ID_RED_GOALIE = 2048;

	// Token: 0x04000867 RID: 2151
	public const int DEFAULT_STICK_SKIN_ID_BLUE_ATTACKER = 2621;

	// Token: 0x04000868 RID: 2152
	public const int DEFAULT_STICK_SKIN_ID_RED_ATTACKER = 2621;

	// Token: 0x04000869 RID: 2153
	public const int DEFAULT_STICK_SKIN_ID_BLUE_GOALIE = 2621;

	// Token: 0x0400086A RID: 2154
	public const int DEFAULT_STICK_SKIN_ID_RED_GOALIE = 2621;

	// Token: 0x0400086B RID: 2155
	public const int DEFAULT_STICK_SHAFT_TAPE_ID_BLUE_ATTACKER = -1;

	// Token: 0x0400086C RID: 2156
	public const int DEFAULT_STICK_SHAFT_TAPE_ID_RED_ATTACKER = -1;

	// Token: 0x0400086D RID: 2157
	public const int DEFAULT_STICK_SHAFT_TAPE_ID_BLUE_GOALIE = -1;

	// Token: 0x0400086E RID: 2158
	public const int DEFAULT_STICK_SHAFT_TAPE_ID_RED_GOALIE = -1;

	// Token: 0x0400086F RID: 2159
	public const int DEFAULT_STICK_BLADE_TAPE_ID_BLUE_ATTACKER = -1;

	// Token: 0x04000870 RID: 2160
	public const int DEFAULT_STICK_BLADE_TAPE_ID_RED_ATTACKER = -1;

	// Token: 0x04000871 RID: 2161
	public const int DEFAULT_STICK_BLADE_TAPE_ID_BLUE_GOALIE = -1;

	// Token: 0x04000872 RID: 2162
	public const int DEFAULT_STICK_BLADE_TAPE_ID_RED_GOALIE = -1;

	// Token: 0x04000873 RID: 2163
	public const ushort DEFAULT_SERVER_PORT = 30609;

	// Token: 0x04000874 RID: 2164
	public const string DEFAULT_SERVER_NAME = "MY PUCK SERVER";

	// Token: 0x04000875 RID: 2165
	public const ushort DEFAULT_SERVER_MAX_PLAYERS = 12;

	// Token: 0x04000876 RID: 2166
	public const string DEFAULT_SERVER_PASSWORD = null;

	// Token: 0x04000877 RID: 2167
	public const ushort DEFAULT_SERVER_TICK_RATE = 200;

	// Token: 0x04000878 RID: 2168
	public const bool DEFAULT_SERVER_IS_PUBLIC = true;

	// Token: 0x04000879 RID: 2169
	public const bool DEFAULT_SERVER_USE_VOIP = false;

	// Token: 0x0400087A RID: 2170
	public const bool DEFAULT_SERVER_USE_WHITELIST = false;

	// Token: 0x0400087B RID: 2171
	public static readonly ModConfig[] DEFAULT_SERVER_MODS = new ModConfig[0];

	// Token: 0x0400087C RID: 2172
	public const string DEFAULT_SERVER_GAME_MODE = "public";

	// Token: 0x0400087D RID: 2173
	public const string DEFAULT_SERVER_LEVEL = "default";

	// Token: 0x0400087E RID: 2174
	public const float KICK_TIMEOUT = 60f;

	// Token: 0x0400087F RID: 2175
	public static readonly string[] CHAT_BLACKLIST = new string[]
	{
		"卐",
		"卍",
		"☭",
		"⛧"
	};

	// Token: 0x04000880 RID: 2176
	public static readonly string[] CHAT_WHITELIST = new string[]
	{
		"❤️",
		"\ud83d\ude2d",
		"\ud83d\udd25",
		"\ud83d\udcaf"
	};

	// Token: 0x04000881 RID: 2177
	public const float INPUT_DEADZONE = 0.05f;

	// Token: 0x04000882 RID: 2178
	public const float SPRINT_STAMINA_THRESHOLD = 0.25f;

	// Token: 0x04000883 RID: 2179
	public const float MATCH_JOIN_TIMEOUT = 60f;

	// Token: 0x04000884 RID: 2180
	public const float MATCH_ABANDONMENT_TIMEOUT = 120f;
}
