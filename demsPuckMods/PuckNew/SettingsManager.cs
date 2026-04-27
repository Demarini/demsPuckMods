using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000129 RID: 297
public static class SettingsManager
{
	// Token: 0x0600084D RID: 2125 RVA: 0x00027D6C File Offset: 0x00025F6C
	public static void Initialize()
	{
		SettingsManager.Debug = SaveManager.GetBool("debug", false);
		SettingsManager.CameraAngle = SaveManager.GetFloat("cameraAngle", 30f);
		SettingsManager.Handedness = SaveManager.GetEnum<PlayerHandedness>("handedness", PlayerHandedness.Right);
		SettingsManager.ShowPuckSilhouette = SaveManager.GetBool("showPuckSilhouette", true);
		SettingsManager.ShowPuckOutline = SaveManager.GetBool("showPuckOutline", false);
		SettingsManager.ShowPuckElevation = SaveManager.GetBool("showPuckElevation", true);
		SettingsManager.ShowPlayerUsernames = SaveManager.GetBool("showPlayerUsernames", false);
		SettingsManager.PlayerUsernamesFadeThreshold = SaveManager.GetFloat("playerUsernamesFadeThreshold", 1f);
		SettingsManager.UseNetworkSmoothing = SaveManager.GetBool("useNetworkSmoothing", false);
		SettingsManager.NetworkSmoothingStrength = SaveManager.GetInt("networkSmoothingStrength", 1);
		SettingsManager.MaxMatchmakingPing = SaveManager.GetInt("maxMatchmakingPing", 50);
		SettingsManager.FilterChatProfanity = SaveManager.GetBool("filterChatProfanity", true);
		SettingsManager.Units = SaveManager.GetEnum<Units>("units", Units.Metric);
		SettingsManager.ShowGameUserInterface = SaveManager.GetBool("showGameUserInterface", true);
		SettingsManager.UserInterfaceScale = SaveManager.GetFloat("userInterfaceScale", 1f);
		SettingsManager.ChatOpacity = SaveManager.GetFloat("chatOpacity", 1f);
		SettingsManager.ChatScale = SaveManager.GetFloat("chatScale", 1f);
		SettingsManager.MinimapOpacity = SaveManager.GetFloat("minimapOpacity", 1f);
		SettingsManager.MinimapBackgroundOpacity = SaveManager.GetFloat("minimapBackgroundOpacity", 1f);
		SettingsManager.MinimapHorizontalPosition = SaveManager.GetFloat("minimapHorizontalPosition", 100f);
		SettingsManager.MinimapVerticalPosition = SaveManager.GetFloat("minimapVerticalPosition", 0f);
		SettingsManager.MinimapScale = SaveManager.GetFloat("minimapScale", 1f);
		SettingsManager.GlobalStickSensitivity = SaveManager.GetFloat("globalStickSensitivity", 0.2f);
		SettingsManager.HorizontalStickSensitivity = SaveManager.GetFloat("horizontalStickSensitivity", 1f);
		SettingsManager.VerticalStickSensitivity = SaveManager.GetFloat("verticalStickSensitivity", 1f);
		SettingsManager.LookSensitivity = SaveManager.GetFloat("lookSensitivity", 0.2f);
		SettingsManager.GlobalVolume = SaveManager.GetFloat("globalVolume", 0.5f);
		SettingsManager.AmbientVolume = SaveManager.GetFloat("ambientVolume", 1f);
		SettingsManager.GameVolume = SaveManager.GetFloat("gameVolume", 1f);
		SettingsManager.VoiceVolume = SaveManager.GetFloat("voiceVolume", 1f);
		SettingsManager.UIVolume = SaveManager.GetFloat("uiVolume", 0.5f);
		SettingsManager.FullScreenMode = SaveManager.GetEnum<FullScreenMode>("fullScreenMode", FullScreenMode.FullScreenWindow);
		SettingsManager.DisplayIndex = SaveManager.GetInt("displayIndex", 0);
		SettingsManager.ResolutionIndex = SaveManager.GetInt("resolutionIndex", -1);
		SettingsManager.VSync = SaveManager.GetBool("vSync", false);
		SettingsManager.FpsLimit = SaveManager.GetInt("fpsLimit", 240);
		SettingsManager.Fov = SaveManager.GetFloat("fov", 90f);
		SettingsManager.Quality = SaveManager.GetEnum<ApplicationQuality>("quality", ApplicationQuality.High);
		SettingsManager.MotionBlur = SaveManager.GetBool("motionBlur", true);
		SettingsManager.Team = SaveManager.GetEnum<PlayerTeam>("team", PlayerTeam.Blue);
		SettingsManager.Role = SaveManager.GetEnum<PlayerRole>("role", PlayerRole.Attacker);
		SettingsManager.ApplyForBothTeams = SaveManager.GetBool("applyForBothTeams", false);
		SettingsManager.FlagID = SaveManager.GetInt("flagID", -1);
		SettingsManager.HeadgearIDBlueAttacker = SaveManager.GetInt("headgearIDBlueAttacker", 513);
		SettingsManager.HeadgearIDRedAttacker = SaveManager.GetInt("headgearIDRedAttacker", 513);
		SettingsManager.HeadgearIDBlueGoalie = SaveManager.GetInt("headgearIDBlueGoalie", 527);
		SettingsManager.HeadgearIDRedGoalie = SaveManager.GetInt("headgearIDRedGoalie", 527);
		SettingsManager.MustacheID = SaveManager.GetInt("mustacheID", -1);
		SettingsManager.BeardID = SaveManager.GetInt("beardID", -1);
		SettingsManager.JerseyIDBlueAttacker = SaveManager.GetInt("jerseyIDBlueAttacker", 2048);
		SettingsManager.JerseyIDRedAttacker = SaveManager.GetInt("jerseyIDRedAttacker", 2048);
		SettingsManager.JerseyIDBlueGoalie = SaveManager.GetInt("jerseyIDBlueGoalie", 2048);
		SettingsManager.JerseyIDRedGoalie = SaveManager.GetInt("jerseyIDRedGoalie", 2048);
		SettingsManager.StickSkinIDBlueAttacker = SaveManager.GetInt("stickSkinIDBlueAttacker", 2621);
		SettingsManager.StickSkinIDRedAttacker = SaveManager.GetInt("stickSkinIDRedAttacker", 2621);
		SettingsManager.StickSkinIDBlueGoalie = SaveManager.GetInt("stickSkinIDBlueGoalie", 2621);
		SettingsManager.StickSkinIDRedGoalie = SaveManager.GetInt("stickSkinIDRedGoalie", 2621);
		SettingsManager.StickShaftTapeIDBlueAttacker = SaveManager.GetInt("stickShaftTapeIDBlueAttacker", -1);
		SettingsManager.StickShaftTapeIDRedAttacker = SaveManager.GetInt("stickShaftTapeIDRedAttacker", -1);
		SettingsManager.StickShaftTapeIDBlueGoalie = SaveManager.GetInt("stickShaftTapeIDBlueGoalie", -1);
		SettingsManager.StickShaftTapeIDRedGoalie = SaveManager.GetInt("stickShaftTapeIDRedGoalie", -1);
		SettingsManager.StickBladeTapeIDBlueAttacker = SaveManager.GetInt("stickBladeTapeIDBlueAttacker", -1);
		SettingsManager.StickBladeTapeIDRedAttacker = SaveManager.GetInt("stickBladeTapeIDRedAttacker", -1);
		SettingsManager.StickBladeTapeIDBlueGoalie = SaveManager.GetInt("stickBladeTapeIDBlueGoalie", -1);
		SettingsManager.StickBladeTapeIDRedGoalie = SaveManager.GetInt("stickBladeTapeIDRedGoalie", -1);
		SettingsManagerController.Initialize();
	}

	// Token: 0x0600084E RID: 2126 RVA: 0x00028213 File Offset: 0x00026413
	public static void Dispose()
	{
		SettingsManagerController.Dispose();
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x0002821C File Offset: 0x0002641C
	public static void ResetToDefault()
	{
		SettingsManager.UpdateDebug(false);
		SettingsManager.UpdateCameraAngle(30f);
		SettingsManager.UpdateHandedness(PlayerHandedness.Right);
		SettingsManager.UpdateShowPuckSilhouette(true);
		SettingsManager.UpdateShowPuckOutline(false);
		SettingsManager.UpdateShowPuckElevation(true);
		SettingsManager.UpdateShowPlayerUsernames(false);
		SettingsManager.UpdatePlayerUsernamesFadeThreshold(1f);
		SettingsManager.UpdateUseNetworkSmoothing(false);
		SettingsManager.UpdateNetworkSmoothingStrength(1);
		SettingsManager.UpdateMaxMatchmakingPing(50);
		SettingsManager.UpdateFilterChatProfanity(true);
		SettingsManager.UpdateUnits(Units.Metric);
		SettingsManager.UpdateShowGameUserInterface(true);
		SettingsManager.UpdateUserInterfaceScale(1f);
		SettingsManager.UpdateChatOpacity(1f);
		SettingsManager.UpdateChatScale(1f);
		SettingsManager.UpdateMinimapOpacity(1f);
		SettingsManager.UpdateMinimapBackgroundOpacity(1f);
		SettingsManager.UpdateMinimapHorizontalPosition(100f);
		SettingsManager.UpdateMinimapVerticalPosition(0f);
		SettingsManager.UpdateMinimapScale(1f);
		SettingsManager.UpdateGlobalStickSensitivity(0.2f);
		SettingsManager.UpdateHorizontalStickSensitivity(1f);
		SettingsManager.UpdateVerticalStickSensitivity(1f);
		SettingsManager.UpdateLookSensitivity(0.2f);
		SettingsManager.UpdateGlobalVolume(0.5f);
		SettingsManager.UpdateAmbientVolume(1f);
		SettingsManager.UpdateGameVolume(1f);
		SettingsManager.UpdateVoiceVolume(1f);
		SettingsManager.UpdateUIVolume(0.5f);
		SettingsManager.UpdateFullScreenMode(FullScreenMode.FullScreenWindow);
		SettingsManager.UpdateDisplayIndex(0);
		SettingsManager.UpdateResolutionIndex(-1);
		SettingsManager.UpdateVSync(false);
		SettingsManager.UpdateFpsLimit(240);
		SettingsManager.UpdateFov(90f);
		SettingsManager.UpdateQuality(ApplicationQuality.High);
		SettingsManager.UpdateMotionBlur(true);
		SettingsManager.UpdateTeam(PlayerTeam.Blue);
		SettingsManager.UpdateRole(PlayerRole.Attacker);
		SettingsManager.UpdateApplyForBothTeams(false);
		SettingsManager.UpdateFlagID(-1);
		SettingsManager.UpdateHeadgearID(PlayerTeam.Blue, PlayerRole.Attacker, 513);
		SettingsManager.UpdateHeadgearID(PlayerTeam.Red, PlayerRole.Attacker, 513);
		SettingsManager.UpdateHeadgearID(PlayerTeam.Blue, PlayerRole.Goalie, 527);
		SettingsManager.UpdateHeadgearID(PlayerTeam.Red, PlayerRole.Goalie, 527);
		SettingsManager.UpdateMustacheID(-1);
		SettingsManager.UpdateBeardID(-1);
		SettingsManager.UpdateJerseyID(PlayerTeam.Blue, PlayerRole.Attacker, 2048);
		SettingsManager.UpdateJerseyID(PlayerTeam.Red, PlayerRole.Attacker, 2048);
		SettingsManager.UpdateJerseyID(PlayerTeam.Blue, PlayerRole.Goalie, 2048);
		SettingsManager.UpdateJerseyID(PlayerTeam.Red, PlayerRole.Goalie, 2048);
		SettingsManager.UpdateStickSkinID(PlayerTeam.Blue, PlayerRole.Attacker, 2621);
		SettingsManager.UpdateStickSkinID(PlayerTeam.Red, PlayerRole.Attacker, 2621);
		SettingsManager.UpdateStickSkinID(PlayerTeam.Blue, PlayerRole.Goalie, 2621);
		SettingsManager.UpdateStickSkinID(PlayerTeam.Red, PlayerRole.Goalie, 2621);
		SettingsManager.UpdateStickShaftTapeID(PlayerTeam.Blue, PlayerRole.Attacker, -1);
		SettingsManager.UpdateStickShaftTapeID(PlayerTeam.Red, PlayerRole.Attacker, -1);
		SettingsManager.UpdateStickShaftTapeID(PlayerTeam.Blue, PlayerRole.Goalie, -1);
		SettingsManager.UpdateStickShaftTapeID(PlayerTeam.Red, PlayerRole.Goalie, -1);
		SettingsManager.UpdateStickBladeTapeID(PlayerTeam.Blue, PlayerRole.Attacker, -1);
		SettingsManager.UpdateStickBladeTapeID(PlayerTeam.Red, PlayerRole.Attacker, -1);
		SettingsManager.UpdateStickBladeTapeID(PlayerTeam.Blue, PlayerRole.Goalie, -1);
		SettingsManager.UpdateStickBladeTapeID(PlayerTeam.Red, PlayerRole.Goalie, -1);
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x0002845C File Offset: 0x0002665C
	public static int GetHeadgearID(PlayerTeam team, PlayerRole role)
	{
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team == PlayerTeam.Blue)
				{
					return SettingsManager.HeadgearIDBlueGoalie;
				}
				if (team == PlayerTeam.Red)
				{
					return SettingsManager.HeadgearIDRedGoalie;
				}
			}
		}
		else
		{
			if (team == PlayerTeam.Blue)
			{
				return SettingsManager.HeadgearIDBlueAttacker;
			}
			if (team == PlayerTeam.Red)
			{
				return SettingsManager.HeadgearIDRedAttacker;
			}
		}
		return -1;
	}

	// Token: 0x06000851 RID: 2129 RVA: 0x00028495 File Offset: 0x00026695
	public static int GetJerseyID(PlayerTeam team, PlayerRole role)
	{
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team == PlayerTeam.Blue)
				{
					return SettingsManager.JerseyIDBlueGoalie;
				}
				if (team == PlayerTeam.Red)
				{
					return SettingsManager.JerseyIDRedGoalie;
				}
			}
		}
		else
		{
			if (team == PlayerTeam.Blue)
			{
				return SettingsManager.JerseyIDBlueAttacker;
			}
			if (team == PlayerTeam.Red)
			{
				return SettingsManager.JerseyIDRedAttacker;
			}
		}
		return 2048;
	}

	// Token: 0x06000852 RID: 2130 RVA: 0x000284D2 File Offset: 0x000266D2
	public static int GetStickSkinID(PlayerTeam team, PlayerRole role)
	{
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team == PlayerTeam.Blue)
				{
					return SettingsManager.StickSkinIDBlueGoalie;
				}
				if (team == PlayerTeam.Red)
				{
					return SettingsManager.StickSkinIDRedGoalie;
				}
			}
		}
		else
		{
			if (team == PlayerTeam.Blue)
			{
				return SettingsManager.StickSkinIDBlueAttacker;
			}
			if (team == PlayerTeam.Red)
			{
				return SettingsManager.StickSkinIDRedAttacker;
			}
		}
		return 2621;
	}

	// Token: 0x06000853 RID: 2131 RVA: 0x0002850F File Offset: 0x0002670F
	public static int GetStickShaftTapeID(PlayerTeam team, PlayerRole role)
	{
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team == PlayerTeam.Blue)
				{
					return SettingsManager.StickShaftTapeIDBlueGoalie;
				}
				if (team == PlayerTeam.Red)
				{
					return SettingsManager.StickShaftTapeIDRedGoalie;
				}
			}
		}
		else
		{
			if (team == PlayerTeam.Blue)
			{
				return SettingsManager.StickShaftTapeIDBlueAttacker;
			}
			if (team == PlayerTeam.Red)
			{
				return SettingsManager.StickShaftTapeIDRedAttacker;
			}
		}
		return -1;
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x00028548 File Offset: 0x00026748
	public static int GetStickBladeTapeID(PlayerTeam team, PlayerRole role)
	{
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team == PlayerTeam.Blue)
				{
					return SettingsManager.StickBladeTapeIDBlueGoalie;
				}
				if (team == PlayerTeam.Red)
				{
					return SettingsManager.StickBladeTapeIDRedGoalie;
				}
			}
		}
		else
		{
			if (team == PlayerTeam.Blue)
			{
				return SettingsManager.StickBladeTapeIDBlueAttacker;
			}
			if (team == PlayerTeam.Red)
			{
				return SettingsManager.StickBladeTapeIDRedAttacker;
			}
		}
		return -1;
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x00028584 File Offset: 0x00026784
	public static void UpdateDebug(bool value)
	{
		if (SettingsManager.Debug == value)
		{
			return;
		}
		SettingsManager.Debug = value;
		SaveManager.SetBool("debug", SettingsManager.Debug);
		EventManager.TriggerEvent("Event_OnDebugChanged", new Dictionary<string, object>
		{
			{
				"value",
				SettingsManager.Debug
			}
		});
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x000285D4 File Offset: 0x000267D4
	public static void UpdateCameraAngle(float value)
	{
		if (SettingsManager.CameraAngle == value)
		{
			return;
		}
		SettingsManager.CameraAngle = value;
		SaveManager.SetFloat("cameraAngle", SettingsManager.CameraAngle);
		EventManager.TriggerEvent("Event_OnCameraAngleChanged", new Dictionary<string, object>
		{
			{
				"value",
				SettingsManager.CameraAngle
			}
		});
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x00028624 File Offset: 0x00026824
	public static void UpdateHandedness(PlayerHandedness value)
	{
		if (SettingsManager.Handedness == value)
		{
			return;
		}
		SettingsManager.Handedness = value;
		SaveManager.SetEnum<PlayerHandedness>("handedness", SettingsManager.Handedness);
		EventManager.TriggerEvent("Event_OnHandednessChanged", new Dictionary<string, object>
		{
			{
				"value",
				SettingsManager.Handedness
			}
		});
	}

	// Token: 0x06000858 RID: 2136 RVA: 0x00028673 File Offset: 0x00026873
	public static void UpdateShowPuckSilhouette(bool value)
	{
		if (SettingsManager.ShowPuckSilhouette == value)
		{
			return;
		}
		SettingsManager.ShowPuckSilhouette = value;
		SaveManager.SetBool("showPuckSilhouette", SettingsManager.ShowPuckSilhouette);
		EventManager.TriggerEvent("Event_OnShowPuckSilhouetteChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000859 RID: 2137 RVA: 0x000286B3 File Offset: 0x000268B3
	public static void UpdateShowPuckOutline(bool value)
	{
		if (SettingsManager.ShowPuckOutline == value)
		{
			return;
		}
		SettingsManager.ShowPuckOutline = value;
		SaveManager.SetBool("showPuckOutline", SettingsManager.ShowPuckOutline);
		EventManager.TriggerEvent("Event_OnShowPuckOutlineChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600085A RID: 2138 RVA: 0x000286F3 File Offset: 0x000268F3
	public static void UpdateShowPuckElevation(bool value)
	{
		if (SettingsManager.ShowPuckElevation == value)
		{
			return;
		}
		SettingsManager.ShowPuckElevation = value;
		SaveManager.SetBool("showPuckElevation", SettingsManager.ShowPuckElevation);
		EventManager.TriggerEvent("Event_OnShowPuckElevationChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600085B RID: 2139 RVA: 0x00028733 File Offset: 0x00026933
	public static void UpdateShowPlayerUsernames(bool value)
	{
		if (SettingsManager.ShowPlayerUsernames == value)
		{
			return;
		}
		SettingsManager.ShowPlayerUsernames = value;
		SaveManager.SetBool("showPlayerUsernames", SettingsManager.ShowPlayerUsernames);
		EventManager.TriggerEvent("Event_OnShowPlayerUsernamesChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600085C RID: 2140 RVA: 0x00028773 File Offset: 0x00026973
	public static void UpdatePlayerUsernamesFadeThreshold(float value)
	{
		if (SettingsManager.PlayerUsernamesFadeThreshold == value)
		{
			return;
		}
		SettingsManager.PlayerUsernamesFadeThreshold = value;
		SaveManager.SetFloat("playerUsernamesFadeThreshold", SettingsManager.PlayerUsernamesFadeThreshold);
		EventManager.TriggerEvent("Event_OnPlayerUsernamesFadeThresholdChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x000287B3 File Offset: 0x000269B3
	public static void UpdateUseNetworkSmoothing(bool value)
	{
		if (SettingsManager.UseNetworkSmoothing == value)
		{
			return;
		}
		SettingsManager.UseNetworkSmoothing = value;
		SaveManager.SetBool("useNetworkSmoothing", SettingsManager.UseNetworkSmoothing);
		EventManager.TriggerEvent("Event_OnUseNetworkSmoothingChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600085E RID: 2142 RVA: 0x000287F3 File Offset: 0x000269F3
	public static void UpdateNetworkSmoothingStrength(int value)
	{
		if (SettingsManager.NetworkSmoothingStrength == value)
		{
			return;
		}
		SettingsManager.NetworkSmoothingStrength = value;
		SaveManager.SetInt("networkSmoothingStrength", SettingsManager.NetworkSmoothingStrength);
		EventManager.TriggerEvent("Event_OnNetworkSmoothingStrengthChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600085F RID: 2143 RVA: 0x00028833 File Offset: 0x00026A33
	public static void UpdateMaxMatchmakingPing(int value)
	{
		if (SettingsManager.MaxMatchmakingPing == value)
		{
			return;
		}
		SettingsManager.MaxMatchmakingPing = value;
		SaveManager.SetInt("maxMatchmakingPing", SettingsManager.MaxMatchmakingPing);
		EventManager.TriggerEvent("Event_OnMaxMatchmakingPingChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x00028873 File Offset: 0x00026A73
	public static void UpdateFilterChatProfanity(bool value)
	{
		if (SettingsManager.FilterChatProfanity == value)
		{
			return;
		}
		SettingsManager.FilterChatProfanity = value;
		SaveManager.SetBool("filterChatProfanity", SettingsManager.FilterChatProfanity);
		EventManager.TriggerEvent("Event_OnFilterChatProfanityChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x000288B4 File Offset: 0x00026AB4
	public static void UpdateUnits(Units value)
	{
		if (SettingsManager.Units == value)
		{
			return;
		}
		SettingsManager.Units = value;
		SaveManager.SetEnum<Units>("units", SettingsManager.Units);
		EventManager.TriggerEvent("Event_OnUnitsChanged", new Dictionary<string, object>
		{
			{
				"value",
				SettingsManager.Units
			}
		});
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x00028903 File Offset: 0x00026B03
	public static void UpdateShowGameUserInterface(bool value)
	{
		if (SettingsManager.ShowGameUserInterface == value)
		{
			return;
		}
		SettingsManager.ShowGameUserInterface = value;
		SaveManager.SetBool("showGameUserInterface", SettingsManager.ShowGameUserInterface);
		EventManager.TriggerEvent("Event_OnShowGameUserInterfaceChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000863 RID: 2147 RVA: 0x00028943 File Offset: 0x00026B43
	public static void UpdateUserInterfaceScale(float value)
	{
		if (SettingsManager.UserInterfaceScale == value)
		{
			return;
		}
		SettingsManager.UserInterfaceScale = value;
		SaveManager.SetFloat("userInterfaceScale", SettingsManager.UserInterfaceScale);
		EventManager.TriggerEvent("Event_OnUserInterfaceScaleChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x00028983 File Offset: 0x00026B83
	public static void UpdateChatOpacity(float value)
	{
		if (SettingsManager.ChatOpacity == value)
		{
			return;
		}
		SettingsManager.ChatOpacity = value;
		SaveManager.SetFloat("chatOpacity", SettingsManager.ChatOpacity);
		EventManager.TriggerEvent("Event_OnChatOpacityChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x000289C3 File Offset: 0x00026BC3
	public static void UpdateChatScale(float value)
	{
		if (SettingsManager.ChatScale == value)
		{
			return;
		}
		SettingsManager.ChatScale = value;
		SaveManager.SetFloat("chatScale", SettingsManager.ChatScale);
		EventManager.TriggerEvent("Event_OnChatScaleChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000866 RID: 2150 RVA: 0x00028A03 File Offset: 0x00026C03
	public static void UpdateMinimapOpacity(float value)
	{
		if (SettingsManager.MinimapOpacity == value)
		{
			return;
		}
		SettingsManager.MinimapOpacity = value;
		SaveManager.SetFloat("minimapOpacity", SettingsManager.MinimapOpacity);
		EventManager.TriggerEvent("Event_OnMinimapOpacityChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x00028A43 File Offset: 0x00026C43
	public static void UpdateMinimapBackgroundOpacity(float value)
	{
		if (SettingsManager.MinimapBackgroundOpacity == value)
		{
			return;
		}
		SettingsManager.MinimapBackgroundOpacity = value;
		SaveManager.SetFloat("minimapBackgroundOpacity", SettingsManager.MinimapBackgroundOpacity);
		EventManager.TriggerEvent("Event_OnMinimapBackgroundOpacityChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x00028A83 File Offset: 0x00026C83
	public static void UpdateMinimapHorizontalPosition(float value)
	{
		if (SettingsManager.MinimapHorizontalPosition == value)
		{
			return;
		}
		SettingsManager.MinimapHorizontalPosition = value;
		SaveManager.SetFloat("minimapHorizontalPosition", SettingsManager.MinimapHorizontalPosition);
		EventManager.TriggerEvent("Event_OnMinimapHorizontalPositionChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x00028AC3 File Offset: 0x00026CC3
	public static void UpdateMinimapVerticalPosition(float value)
	{
		if (SettingsManager.MinimapVerticalPosition == value)
		{
			return;
		}
		SettingsManager.MinimapVerticalPosition = value;
		SaveManager.SetFloat("minimapVerticalPosition", SettingsManager.MinimapVerticalPosition);
		EventManager.TriggerEvent("Event_OnMinimapVerticalPositionChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600086A RID: 2154 RVA: 0x00028B03 File Offset: 0x00026D03
	public static void UpdateMinimapScale(float value)
	{
		if (SettingsManager.MinimapScale == value)
		{
			return;
		}
		SettingsManager.MinimapScale = value;
		SaveManager.SetFloat("minimapScale", SettingsManager.MinimapScale);
		EventManager.TriggerEvent("Event_OnMinimapScaleChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x00028B43 File Offset: 0x00026D43
	public static void UpdateGlobalStickSensitivity(float value)
	{
		if (SettingsManager.GlobalStickSensitivity == value)
		{
			return;
		}
		SettingsManager.GlobalStickSensitivity = value;
		SaveManager.SetFloat("globalStickSensitivity", SettingsManager.GlobalStickSensitivity);
		EventManager.TriggerEvent("Event_OnGlobalStickSensitivityChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x00028B83 File Offset: 0x00026D83
	public static void UpdateHorizontalStickSensitivity(float value)
	{
		if (SettingsManager.HorizontalStickSensitivity == value)
		{
			return;
		}
		SettingsManager.HorizontalStickSensitivity = value;
		SaveManager.SetFloat("horizontalStickSensitivity", SettingsManager.HorizontalStickSensitivity);
		EventManager.TriggerEvent("Event_OnHorizontalStickSensitivityChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x00028BC3 File Offset: 0x00026DC3
	public static void UpdateVerticalStickSensitivity(float value)
	{
		if (SettingsManager.VerticalStickSensitivity == value)
		{
			return;
		}
		SettingsManager.VerticalStickSensitivity = value;
		SaveManager.SetFloat("verticalStickSensitivity", SettingsManager.VerticalStickSensitivity);
		EventManager.TriggerEvent("Event_OnVerticalStickSensitivityChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x00028C03 File Offset: 0x00026E03
	public static void UpdateLookSensitivity(float value)
	{
		if (SettingsManager.LookSensitivity == value)
		{
			return;
		}
		SettingsManager.LookSensitivity = value;
		SaveManager.SetFloat("lookSensitivity", SettingsManager.LookSensitivity);
		EventManager.TriggerEvent("Event_OnLookSensitivityChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x00028C43 File Offset: 0x00026E43
	public static void UpdateGlobalVolume(float value)
	{
		if (SettingsManager.GlobalVolume == value)
		{
			return;
		}
		SettingsManager.GlobalVolume = value;
		SaveManager.SetFloat("globalVolume", SettingsManager.GlobalVolume);
		EventManager.TriggerEvent("Event_OnGlobalVolumeChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000870 RID: 2160 RVA: 0x00028C83 File Offset: 0x00026E83
	public static void UpdateAmbientVolume(float value)
	{
		if (SettingsManager.AmbientVolume == value)
		{
			return;
		}
		SettingsManager.AmbientVolume = value;
		SaveManager.SetFloat("ambientVolume", SettingsManager.AmbientVolume);
		EventManager.TriggerEvent("Event_OnAmbientVolumeChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x00028CC3 File Offset: 0x00026EC3
	public static void UpdateGameVolume(float value)
	{
		if (SettingsManager.GameVolume == value)
		{
			return;
		}
		SettingsManager.GameVolume = value;
		SaveManager.SetFloat("gameVolume", SettingsManager.GameVolume);
		EventManager.TriggerEvent("Event_OnGameVolumeChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x00028D03 File Offset: 0x00026F03
	public static void UpdateVoiceVolume(float value)
	{
		if (SettingsManager.VoiceVolume == value)
		{
			return;
		}
		SettingsManager.VoiceVolume = value;
		SaveManager.SetFloat("voiceVolume", SettingsManager.VoiceVolume);
		EventManager.TriggerEvent("Event_OnVoiceVolumeChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000873 RID: 2163 RVA: 0x00028D43 File Offset: 0x00026F43
	public static void UpdateUIVolume(float value)
	{
		if (SettingsManager.UIVolume == value)
		{
			return;
		}
		SettingsManager.UIVolume = value;
		SaveManager.SetFloat("uiVolume", SettingsManager.UIVolume);
		EventManager.TriggerEvent("Event_OnUIVolumeChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x00028D84 File Offset: 0x00026F84
	public static void UpdateFullScreenMode(FullScreenMode value)
	{
		if (SettingsManager.FullScreenMode == value)
		{
			return;
		}
		SettingsManager.FullScreenMode = value;
		SaveManager.SetEnum<FullScreenMode>("fullScreenMode", SettingsManager.FullScreenMode);
		EventManager.TriggerEvent("Event_OnFullScreenModeChanged", new Dictionary<string, object>
		{
			{
				"value",
				SettingsManager.FullScreenMode
			}
		});
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x00028DD4 File Offset: 0x00026FD4
	public static void UpdateDisplayIndex(int value)
	{
		if (SettingsManager.DisplayIndex == value)
		{
			return;
		}
		SettingsManager.DisplayIndex = value;
		SaveManager.SetInt("displayIndex", SettingsManager.DisplayIndex);
		EventManager.TriggerEvent("Event_OnDisplayIndexChanged", new Dictionary<string, object>
		{
			{
				"value",
				SettingsManager.DisplayIndex
			}
		});
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x00028E24 File Offset: 0x00027024
	public static void UpdateResolutionIndex(int value)
	{
		if (SettingsManager.ResolutionIndex == value)
		{
			return;
		}
		SettingsManager.ResolutionIndex = value;
		SaveManager.SetInt("resolutionIndex", SettingsManager.ResolutionIndex);
		EventManager.TriggerEvent("Event_OnResolutionIndexChanged", new Dictionary<string, object>
		{
			{
				"value",
				SettingsManager.ResolutionIndex
			}
		});
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x00028E74 File Offset: 0x00027074
	public static void UpdateVSync(bool value)
	{
		if (SettingsManager.VSync == value)
		{
			return;
		}
		SettingsManager.VSync = value;
		SaveManager.SetBool("vSync", SettingsManager.VSync);
		EventManager.TriggerEvent("Event_OnVSyncChanged", new Dictionary<string, object>
		{
			{
				"value",
				SettingsManager.VSync
			}
		});
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x00028EC4 File Offset: 0x000270C4
	public static void UpdateFpsLimit(int value)
	{
		if (SettingsManager.FpsLimit == value)
		{
			return;
		}
		SettingsManager.FpsLimit = value;
		SaveManager.SetInt("fpsLimit", SettingsManager.FpsLimit);
		EventManager.TriggerEvent("Event_OnFpsLimitChanged", new Dictionary<string, object>
		{
			{
				"value",
				SettingsManager.FpsLimit
			}
		});
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x00028F14 File Offset: 0x00027114
	public static void UpdateFov(float value)
	{
		if (SettingsManager.Fov == value)
		{
			return;
		}
		SettingsManager.Fov = value;
		SaveManager.SetFloat("fov", SettingsManager.Fov);
		EventManager.TriggerEvent("Event_OnFovChanged", new Dictionary<string, object>
		{
			{
				"value",
				SettingsManager.Fov
			}
		});
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x00028F64 File Offset: 0x00027164
	public static void UpdateQuality(ApplicationQuality value)
	{
		if (SettingsManager.Quality == value)
		{
			return;
		}
		SettingsManager.Quality = value;
		SaveManager.SetEnum<ApplicationQuality>("quality", SettingsManager.Quality);
		EventManager.TriggerEvent("Event_OnQualityChanged", new Dictionary<string, object>
		{
			{
				"value",
				SettingsManager.Quality
			}
		});
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x00028FB3 File Offset: 0x000271B3
	public static void UpdateMotionBlur(bool value)
	{
		if (SettingsManager.MotionBlur == value)
		{
			return;
		}
		SettingsManager.MotionBlur = value;
		SaveManager.SetBool("motionBlur", SettingsManager.MotionBlur);
		EventManager.TriggerEvent("Event_OnMotionBlurChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x00028FF3 File Offset: 0x000271F3
	public static void UpdateTeam(PlayerTeam team)
	{
		if (SettingsManager.Team == team)
		{
			return;
		}
		SettingsManager.Team = team;
		SaveManager.SetEnum<PlayerTeam>("team", SettingsManager.Team);
		EventManager.TriggerEvent("Event_OnTeamChanged", new Dictionary<string, object>
		{
			{
				"value",
				team
			}
		});
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x00029033 File Offset: 0x00027233
	public static void UpdateRole(PlayerRole role)
	{
		if (SettingsManager.Role == role)
		{
			return;
		}
		SettingsManager.Role = role;
		SaveManager.SetEnum<PlayerRole>("role", SettingsManager.Role);
		EventManager.TriggerEvent("Event_OnRoleChanged", new Dictionary<string, object>
		{
			{
				"value",
				role
			}
		});
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x00029073 File Offset: 0x00027273
	public static void UpdateApplyForBothTeams(bool value)
	{
		if (SettingsManager.ApplyForBothTeams == value)
		{
			return;
		}
		SettingsManager.ApplyForBothTeams = value;
		SaveManager.SetBool("applyForBothTeams", SettingsManager.ApplyForBothTeams);
		EventManager.TriggerEvent("Event_OnApplyForBothTeamsChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x000290B3 File Offset: 0x000272B3
	public static void UpdateFlagID(int value)
	{
		if (SettingsManager.FlagID == value)
		{
			return;
		}
		SettingsManager.FlagID = value;
		SaveManager.SetInt("flagID", SettingsManager.FlagID);
		EventManager.TriggerEvent("Event_OnFlagIDChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000880 RID: 2176 RVA: 0x000290F4 File Offset: 0x000272F4
	public static void UpdateHeadgearID(PlayerTeam team, PlayerRole role, int value)
	{
		if (SettingsManager.GetHeadgearID(team, role) == value)
		{
			return;
		}
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team != PlayerTeam.Blue)
				{
					if (team == PlayerTeam.Red)
					{
						SettingsManager.HeadgearIDRedGoalie = value;
						SaveManager.SetInt("headgearIDRedGoalie", SettingsManager.HeadgearIDRedGoalie);
					}
				}
				else
				{
					SettingsManager.HeadgearIDBlueGoalie = value;
					SaveManager.SetInt("headgearIDBlueGoalie", SettingsManager.HeadgearIDBlueGoalie);
				}
			}
		}
		else if (team != PlayerTeam.Blue)
		{
			if (team == PlayerTeam.Red)
			{
				SettingsManager.HeadgearIDRedAttacker = value;
				SaveManager.SetInt("headgearIDRedAttacker", SettingsManager.HeadgearIDRedAttacker);
			}
		}
		else
		{
			SettingsManager.HeadgearIDBlueAttacker = value;
			SaveManager.SetInt("headgearIDBlueAttacker", SettingsManager.HeadgearIDBlueAttacker);
		}
		EventManager.TriggerEvent("Event_OnHeadgearIDChanged", new Dictionary<string, object>
		{
			{
				"team",
				team
			},
			{
				"role",
				role
			},
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x000291C6 File Offset: 0x000273C6
	public static void UpdateMustacheID(int value)
	{
		if (SettingsManager.MustacheID == value)
		{
			return;
		}
		SettingsManager.MustacheID = value;
		SaveManager.SetInt("mustacheID", SettingsManager.MustacheID);
		EventManager.TriggerEvent("Event_OnMustacheIDChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x00029206 File Offset: 0x00027406
	public static void UpdateBeardID(int value)
	{
		if (SettingsManager.BeardID == value)
		{
			return;
		}
		SettingsManager.BeardID = value;
		SaveManager.SetInt("beardID", SettingsManager.BeardID);
		EventManager.TriggerEvent("Event_OnBeardIDChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x00029248 File Offset: 0x00027448
	public static void UpdateJerseyID(PlayerTeam team, PlayerRole role, int value)
	{
		if (SettingsManager.GetJerseyID(team, role) == value)
		{
			return;
		}
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team != PlayerTeam.Blue)
				{
					if (team == PlayerTeam.Red)
					{
						SettingsManager.JerseyIDRedGoalie = value;
						SaveManager.SetInt("jerseyIDRedGoalie", SettingsManager.JerseyIDRedGoalie);
					}
				}
				else
				{
					SettingsManager.JerseyIDBlueGoalie = value;
					SaveManager.SetInt("jerseyIDBlueGoalie", SettingsManager.JerseyIDBlueGoalie);
				}
			}
		}
		else if (team != PlayerTeam.Blue)
		{
			if (team == PlayerTeam.Red)
			{
				SettingsManager.JerseyIDRedAttacker = value;
				SaveManager.SetInt("jerseyIDRedAttacker", SettingsManager.JerseyIDRedAttacker);
			}
		}
		else
		{
			SettingsManager.JerseyIDBlueAttacker = value;
			SaveManager.SetInt("jerseyIDBlueAttacker", SettingsManager.JerseyIDBlueAttacker);
		}
		EventManager.TriggerEvent("Event_OnJerseyIDChanged", new Dictionary<string, object>
		{
			{
				"team",
				team
			},
			{
				"role",
				role
			},
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x0002931C File Offset: 0x0002751C
	public static void UpdateStickSkinID(PlayerTeam team, PlayerRole role, int value)
	{
		if (SettingsManager.GetStickSkinID(team, role) == value)
		{
			return;
		}
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team != PlayerTeam.Blue)
				{
					if (team == PlayerTeam.Red)
					{
						SettingsManager.StickSkinIDRedGoalie = value;
						SaveManager.SetInt("stickSkinIDRedGoalie", SettingsManager.StickSkinIDRedGoalie);
					}
				}
				else
				{
					SettingsManager.StickSkinIDBlueGoalie = value;
					SaveManager.SetInt("stickSkinIDBlueGoalie", SettingsManager.StickSkinIDBlueGoalie);
				}
			}
		}
		else if (team != PlayerTeam.Blue)
		{
			if (team == PlayerTeam.Red)
			{
				SettingsManager.StickSkinIDRedAttacker = value;
				SaveManager.SetInt("stickSkinIDRedAttacker", SettingsManager.StickSkinIDRedAttacker);
			}
		}
		else
		{
			SettingsManager.StickSkinIDBlueAttacker = value;
			SaveManager.SetInt("stickSkinIDBlueAttacker", SettingsManager.StickSkinIDBlueAttacker);
		}
		EventManager.TriggerEvent("Event_OnStickSkinIDChanged", new Dictionary<string, object>
		{
			{
				"team",
				team
			},
			{
				"role",
				role
			},
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x000293F0 File Offset: 0x000275F0
	public static void UpdateStickShaftTapeID(PlayerTeam team, PlayerRole role, int value)
	{
		if (SettingsManager.GetStickShaftTapeID(team, role) == value)
		{
			return;
		}
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team != PlayerTeam.Blue)
				{
					if (team == PlayerTeam.Red)
					{
						SettingsManager.StickShaftTapeIDRedGoalie = value;
						SaveManager.SetInt("stickShaftTapeIDRedGoalie", SettingsManager.StickShaftTapeIDRedGoalie);
					}
				}
				else
				{
					SettingsManager.StickShaftTapeIDBlueGoalie = value;
					SaveManager.SetInt("stickShaftTapeIDBlueGoalie", SettingsManager.StickShaftTapeIDBlueGoalie);
				}
			}
		}
		else if (team != PlayerTeam.Blue)
		{
			if (team == PlayerTeam.Red)
			{
				SettingsManager.StickShaftTapeIDRedAttacker = value;
				SaveManager.SetInt("stickShaftTapeIDRedAttacker", SettingsManager.StickShaftTapeIDRedAttacker);
			}
		}
		else
		{
			SettingsManager.StickShaftTapeIDBlueAttacker = value;
			SaveManager.SetInt("stickShaftTapeIDBlueAttacker", SettingsManager.StickShaftTapeIDBlueAttacker);
		}
		EventManager.TriggerEvent("Event_OnStickShaftTapeIDChanged", new Dictionary<string, object>
		{
			{
				"team",
				team
			},
			{
				"role",
				role
			},
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x000294C4 File Offset: 0x000276C4
	public static void UpdateStickBladeTapeID(PlayerTeam team, PlayerRole role, int value)
	{
		if (SettingsManager.GetStickBladeTapeID(team, role) == value)
		{
			return;
		}
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team != PlayerTeam.Blue)
				{
					if (team == PlayerTeam.Red)
					{
						SettingsManager.StickBladeTapeIDRedGoalie = value;
						SaveManager.SetInt("stickBladeTapeIDRedGoalie", SettingsManager.StickBladeTapeIDRedGoalie);
					}
				}
				else
				{
					SettingsManager.StickBladeTapeIDBlueGoalie = value;
					SaveManager.SetInt("stickBladeTapeIDBlueGoalie", SettingsManager.StickBladeTapeIDBlueGoalie);
				}
			}
		}
		else if (team != PlayerTeam.Blue)
		{
			if (team == PlayerTeam.Red)
			{
				SettingsManager.StickBladeTapeIDRedAttacker = value;
				SaveManager.SetInt("stickBladeTapeIDRedAttacker", SettingsManager.StickBladeTapeIDRedAttacker);
			}
		}
		else
		{
			SettingsManager.StickBladeTapeIDBlueAttacker = value;
			SaveManager.SetInt("stickBladeTapeIDBlueAttacker", SettingsManager.StickBladeTapeIDBlueAttacker);
		}
		EventManager.TriggerEvent("Event_OnStickBladeTapeIDChanged", new Dictionary<string, object>
		{
			{
				"team",
				team
			},
			{
				"role",
				role
			},
			{
				"value",
				value
			}
		});
	}

	// Token: 0x040004DE RID: 1246
	public static bool Debug;

	// Token: 0x040004DF RID: 1247
	public static float CameraAngle;

	// Token: 0x040004E0 RID: 1248
	public static PlayerHandedness Handedness;

	// Token: 0x040004E1 RID: 1249
	public static bool ShowPuckSilhouette;

	// Token: 0x040004E2 RID: 1250
	public static bool ShowPuckOutline;

	// Token: 0x040004E3 RID: 1251
	public static bool ShowPuckElevation;

	// Token: 0x040004E4 RID: 1252
	public static bool ShowPlayerUsernames;

	// Token: 0x040004E5 RID: 1253
	public static float PlayerUsernamesFadeThreshold;

	// Token: 0x040004E6 RID: 1254
	public static bool UseNetworkSmoothing;

	// Token: 0x040004E7 RID: 1255
	public static int NetworkSmoothingStrength;

	// Token: 0x040004E8 RID: 1256
	public static int MaxMatchmakingPing;

	// Token: 0x040004E9 RID: 1257
	public static bool FilterChatProfanity;

	// Token: 0x040004EA RID: 1258
	public static Units Units;

	// Token: 0x040004EB RID: 1259
	public static bool ShowGameUserInterface;

	// Token: 0x040004EC RID: 1260
	public static float UserInterfaceScale;

	// Token: 0x040004ED RID: 1261
	public static float ChatOpacity;

	// Token: 0x040004EE RID: 1262
	public static float ChatScale;

	// Token: 0x040004EF RID: 1263
	public static float MinimapOpacity;

	// Token: 0x040004F0 RID: 1264
	public static float MinimapBackgroundOpacity;

	// Token: 0x040004F1 RID: 1265
	public static float MinimapHorizontalPosition;

	// Token: 0x040004F2 RID: 1266
	public static float MinimapVerticalPosition;

	// Token: 0x040004F3 RID: 1267
	public static float MinimapScale;

	// Token: 0x040004F4 RID: 1268
	public static float GlobalStickSensitivity;

	// Token: 0x040004F5 RID: 1269
	public static float HorizontalStickSensitivity;

	// Token: 0x040004F6 RID: 1270
	public static float VerticalStickSensitivity;

	// Token: 0x040004F7 RID: 1271
	public static float LookSensitivity;

	// Token: 0x040004F8 RID: 1272
	public static float GlobalVolume;

	// Token: 0x040004F9 RID: 1273
	public static float AmbientVolume;

	// Token: 0x040004FA RID: 1274
	public static float GameVolume;

	// Token: 0x040004FB RID: 1275
	public static float VoiceVolume;

	// Token: 0x040004FC RID: 1276
	public static float UIVolume;

	// Token: 0x040004FD RID: 1277
	public static FullScreenMode FullScreenMode;

	// Token: 0x040004FE RID: 1278
	public static int DisplayIndex;

	// Token: 0x040004FF RID: 1279
	public static int ResolutionIndex;

	// Token: 0x04000500 RID: 1280
	public static bool VSync;

	// Token: 0x04000501 RID: 1281
	public static int FpsLimit;

	// Token: 0x04000502 RID: 1282
	public static float Fov;

	// Token: 0x04000503 RID: 1283
	public static ApplicationQuality Quality;

	// Token: 0x04000504 RID: 1284
	public static bool MotionBlur;

	// Token: 0x04000505 RID: 1285
	public static PlayerTeam Team;

	// Token: 0x04000506 RID: 1286
	public static PlayerRole Role;

	// Token: 0x04000507 RID: 1287
	public static bool ApplyForBothTeams;

	// Token: 0x04000508 RID: 1288
	public static int FlagID;

	// Token: 0x04000509 RID: 1289
	public static int HeadgearIDBlueAttacker;

	// Token: 0x0400050A RID: 1290
	public static int HeadgearIDRedAttacker;

	// Token: 0x0400050B RID: 1291
	public static int HeadgearIDBlueGoalie;

	// Token: 0x0400050C RID: 1292
	public static int HeadgearIDRedGoalie;

	// Token: 0x0400050D RID: 1293
	public static int MustacheID;

	// Token: 0x0400050E RID: 1294
	public static int BeardID;

	// Token: 0x0400050F RID: 1295
	public static int JerseyIDBlueAttacker;

	// Token: 0x04000510 RID: 1296
	public static int JerseyIDRedAttacker;

	// Token: 0x04000511 RID: 1297
	public static int JerseyIDBlueGoalie;

	// Token: 0x04000512 RID: 1298
	public static int JerseyIDRedGoalie;

	// Token: 0x04000513 RID: 1299
	public static int StickSkinIDBlueAttacker;

	// Token: 0x04000514 RID: 1300
	public static int StickSkinIDRedAttacker;

	// Token: 0x04000515 RID: 1301
	public static int StickSkinIDBlueGoalie;

	// Token: 0x04000516 RID: 1302
	public static int StickSkinIDRedGoalie;

	// Token: 0x04000517 RID: 1303
	public static int StickShaftTapeIDBlueAttacker;

	// Token: 0x04000518 RID: 1304
	public static int StickShaftTapeIDRedAttacker;

	// Token: 0x04000519 RID: 1305
	public static int StickShaftTapeIDBlueGoalie;

	// Token: 0x0400051A RID: 1306
	public static int StickShaftTapeIDRedGoalie;

	// Token: 0x0400051B RID: 1307
	public static int StickBladeTapeIDBlueAttacker;

	// Token: 0x0400051C RID: 1308
	public static int StickBladeTapeIDRedAttacker;

	// Token: 0x0400051D RID: 1309
	public static int StickBladeTapeIDBlueGoalie;

	// Token: 0x0400051E RID: 1310
	public static int StickBladeTapeIDRedGoalie;
}
