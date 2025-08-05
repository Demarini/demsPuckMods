using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020000A4 RID: 164
public class SettingsManager : MonoBehaviourSingleton<SettingsManager>
{
	// Token: 0x06000463 RID: 1123 RVA: 0x0001D9DC File Offset: 0x0001BBDC
	public void LoadSettings()
	{
		if (Application.isBatchMode)
		{
			return;
		}
		this.Debug = PlayerPrefs.GetInt("debug", 0);
		this.CameraAngle = PlayerPrefs.GetFloat("cameraAngle", 30f);
		this.Handedness = PlayerPrefs.GetString("handedness", "RIGHT");
		this.ShowPuckSilhouette = PlayerPrefs.GetInt("showPuckSilhouette", 1);
		this.ShowPuckOutline = PlayerPrefs.GetInt("showPuckOutline", 0);
		this.ShowPuckElevation = PlayerPrefs.GetInt("showPuckElevation", 1);
		this.ShowPlayerUsernames = PlayerPrefs.GetInt("showPlayerUsernames", 0);
		this.PlayerUsernamesFadeThreshold = PlayerPrefs.GetFloat("playerUsernamesFadeThreshold", 1f);
		this.UseNetworkSmoothing = PlayerPrefs.GetInt("useNetworkSmoothing", 0);
		this.NetworkSmoothingStrength = PlayerPrefs.GetFloat("networkSmoothingStrength", 1f);
		this.FilterChatProfanity = PlayerPrefs.GetInt("filterChatProfanity", 1);
		this.Units = PlayerPrefs.GetString("units", "METRIC");
		this.ShowGameUserInterface = PlayerPrefs.GetInt("showGameUserInterface", 1);
		this.UserInterfaceScale = PlayerPrefs.GetFloat("userInterfaceScale", 1f);
		this.ChatOpacity = PlayerPrefs.GetFloat("chatOpacity", 1f);
		this.ChatScale = PlayerPrefs.GetFloat("chatScale", 1f);
		this.MinimapOpacity = PlayerPrefs.GetFloat("minimapOpacity", 1f);
		this.MinimapBackgroundOpacity = PlayerPrefs.GetFloat("minimapBackgroundOpacity", 0.25f);
		this.MinimapHorizontalPosition = PlayerPrefs.GetFloat("minimapHorizontalPosition", 100f);
		this.MinimapVerticalPosition = PlayerPrefs.GetFloat("minimapVerticalPosition", 100f);
		this.MinimapScale = PlayerPrefs.GetFloat("minimapScale", 1f);
		this.GlobalStickSensitivity = PlayerPrefs.GetFloat("globalStickSensitivity", 0.2f);
		this.HorizontalStickSensitivity = PlayerPrefs.GetFloat("horizontalStickSensitivity", 1f);
		this.VerticalStickSensitivity = PlayerPrefs.GetFloat("verticalStickSensitivity", 1f);
		this.LookSensitivity = PlayerPrefs.GetFloat("lookSensitivity", 0.2f);
		this.GlobalVolume = PlayerPrefs.GetFloat("globalVolume", 0.5f);
		this.AmbientVolume = PlayerPrefs.GetFloat("ambientVolume", 1f);
		this.GameVolume = PlayerPrefs.GetFloat("gameVolume", 1f);
		this.VoiceVolume = PlayerPrefs.GetFloat("voiceVolume", 1f);
		this.UIVolume = PlayerPrefs.GetFloat("uiVolume", 0.5f);
		this.WindowMode = PlayerPrefs.GetString("windowMode", "BORDERLESS");
		this.DisplayIndex = PlayerPrefs.GetInt("displayIndex", -1);
		this.ResolutionIndex = PlayerPrefs.GetInt("resolutionIndex", -1);
		this.VSync = PlayerPrefs.GetInt("vSync", 0);
		this.FpsLimit = PlayerPrefs.GetInt("fpsLimit", 240);
		this.Fov = PlayerPrefs.GetFloat("fov", 90f);
		this.Quality = PlayerPrefs.GetString("quality", "HIGH");
		this.MotionBlur = PlayerPrefs.GetInt("motionBlur", 1);
		this.Country = PlayerPrefs.GetString("country", "none");
		this.VisorAttackerBlueSkin = PlayerPrefs.GetString("visorAttackerBlueSkin", "visor_default_attacker");
		this.VisorAttackerRedSkin = PlayerPrefs.GetString("visorAttackerRedSkin", "visor_default_attacker");
		this.VisorGoalieBlueSkin = PlayerPrefs.GetString("visorGoalieBlueSkin", "visor_default_attacker");
		this.VisorGoalieRedSkin = PlayerPrefs.GetString("visorGoalieRedSkin", "visor_default_attacker");
		this.Mustache = PlayerPrefs.GetString("mustache", "none");
		this.Beard = PlayerPrefs.GetString("beard", "none");
		this.JerseyAttackerBlueSkin = PlayerPrefs.GetString("jerseyAttackerBlueSkin", "default");
		this.JerseyAttackerRedSkin = PlayerPrefs.GetString("jerseyAttackerRedSkin", "default");
		this.JerseyGoalieBlueSkin = PlayerPrefs.GetString("jerseyGoalieBlueSkin", "default");
		this.JerseyGoalieRedSkin = PlayerPrefs.GetString("jerseyGoalieRedSkin", "default");
		this.StickAttackerBlueSkin = PlayerPrefs.GetString("stickAttackerBlueSkin", "classic_attacker");
		this.StickAttackerRedSkin = PlayerPrefs.GetString("stickAttackerRedSkin", "classic_attacker");
		this.StickGoalieBlueSkin = PlayerPrefs.GetString("stickGoalieBlueSkin", "classic_goalie");
		this.StickGoalieRedSkin = PlayerPrefs.GetString("stickGoalieRedSkin", "classic_goalie");
		this.StickShaftAttackerBlueTapeSkin = PlayerPrefs.GetString("stickShaftAttackerBlueTapeSkin", "gray_cloth");
		this.StickShaftAttackerRedTapeSkin = PlayerPrefs.GetString("stickShaftAttackerRedTapeSkin", "gray_cloth");
		this.StickShaftGoalieBlueTapeSkin = PlayerPrefs.GetString("stickShaftGoalieBlueTapeSkin", "gray_cloth");
		this.StickShaftGoalieRedTapeSkin = PlayerPrefs.GetString("stickShaftGoalieRedTapeSkin", "gray_cloth");
		this.StickBladeAttackerBlueTapeSkin = PlayerPrefs.GetString("stickBladeAttackerBlueTapeSkin", "gray_cloth");
		this.StickBladeAttackerRedTapeSkin = PlayerPrefs.GetString("stickBladeAttackerRedTapeSkin", "gray_cloth");
		this.StickBladeGoalieBlueTapeSkin = PlayerPrefs.GetString("stickBladeGoalieBlueTapeSkin", "gray_cloth");
		this.StickBladeGoalieRedTapeSkin = PlayerPrefs.GetString("stickBladeGoalieRedTapeSkin", "gray_cloth");
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x0001DEC4 File Offset: 0x0001C0C4
	public void ApplySettings()
	{
		if (Application.isBatchMode)
		{
			return;
		}
		this.UpdateDebug(this.Debug > 0);
		this.UpdateCameraAngle(this.CameraAngle);
		this.UpdateHandedness(this.Handedness);
		this.UpdateShowPuckSilhouette(this.ShowPuckSilhouette > 0);
		this.UpdateShowPuckOutline(this.ShowPuckOutline > 0);
		this.UpdateShowPuckElevation(this.ShowPuckElevation > 0);
		this.UpdateShowPlayerUsernames(this.ShowPlayerUsernames > 0);
		this.UpdatePlayerUsernamesFadeThreshold(this.PlayerUsernamesFadeThreshold);
		this.UpdateUseNetworkSmoothing(this.UseNetworkSmoothing > 0);
		this.UpdateNetworkSmoothingStrength(this.NetworkSmoothingStrength);
		this.UpdateFilterChatProfanity(this.FilterChatProfanity > 0);
		this.UpdateUnits(this.Units);
		this.UpdateShowGameUserInterface(this.ShowGameUserInterface > 0);
		this.UpdateUserInterfaceScale(this.UserInterfaceScale);
		this.UpdateChatOpacity(this.ChatOpacity);
		this.UpdateChatScale(this.ChatScale);
		this.UpdateMinimapOpacity(this.MinimapOpacity);
		this.UpdateMinimapBackgroundOpacity(this.MinimapBackgroundOpacity);
		this.UpdateMinimapHorizontalPosition(this.MinimapHorizontalPosition);
		this.UpdateMinimapVerticalPosition(this.MinimapVerticalPosition);
		this.UpdateMinimapScale(this.MinimapScale);
		this.UpdateGlobalStickSensitivity(this.GlobalStickSensitivity);
		this.UpdateHorizontalStickSensitivity(this.HorizontalStickSensitivity);
		this.UpdateVerticalStickSensitivity(this.VerticalStickSensitivity);
		this.UpdateLookSensitivity(this.LookSensitivity);
		this.UpdateGlobalVolume(this.GlobalVolume);
		this.UpdateAmbientVolume(this.AmbientVolume);
		this.UpdateGameVolume(this.GameVolume);
		this.UpdateVoiceVolume(this.VoiceVolume);
		this.UpdateUIVolume(this.UIVolume);
		this.UpdateWindowMode(this.WindowMode);
		this.UpdateDisplayIndex(this.DisplayIndex, true);
		this.UpdateVSync(this.VSync > 0);
		this.UpdateFpsLimit(this.FpsLimit);
		this.UpdateFov(this.Fov);
		this.UpdateQuality(this.Quality);
		this.UpdateMotionBlur(this.MotionBlur > 0);
		this.UpdateCountry(this.Country);
		this.UpdateVisorSkin(PlayerTeam.Blue, PlayerRole.Attacker, this.VisorAttackerBlueSkin);
		this.UpdateMustache(this.Mustache);
		this.UpdateBeard(this.Beard);
		this.UpdateJerseySkin(PlayerTeam.Blue, PlayerRole.Attacker, this.JerseyAttackerBlueSkin);
		this.UpdateStickSkin(PlayerTeam.Blue, PlayerRole.Attacker, this.StickAttackerBlueSkin);
		this.UpdateStickShaftSkin(PlayerTeam.Blue, PlayerRole.Attacker, this.StickShaftAttackerBlueTapeSkin);
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x00009B12 File Offset: 0x00007D12
	public void SaveSettings()
	{
		if (Application.isBatchMode)
		{
			return;
		}
		PlayerPrefs.Save();
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x0001E138 File Offset: 0x0001C338
	public string GetVisorSkin(PlayerTeam team, PlayerRole role)
	{
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team == PlayerTeam.Blue)
				{
					return this.VisorGoalieBlueSkin;
				}
				if (team == PlayerTeam.Red)
				{
					return this.VisorGoalieRedSkin;
				}
			}
		}
		else
		{
			if (team == PlayerTeam.Blue)
			{
				return this.VisorAttackerBlueSkin;
			}
			if (team == PlayerTeam.Red)
			{
				return this.VisorAttackerRedSkin;
			}
		}
		return "default";
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x0001E184 File Offset: 0x0001C384
	public string GetJerseySkin(PlayerTeam team, PlayerRole role)
	{
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team == PlayerTeam.Blue)
				{
					return this.JerseyGoalieBlueSkin;
				}
				if (team == PlayerTeam.Red)
				{
					return this.JerseyGoalieRedSkin;
				}
			}
		}
		else
		{
			if (team == PlayerTeam.Blue)
			{
				return this.JerseyAttackerBlueSkin;
			}
			if (team == PlayerTeam.Red)
			{
				return this.JerseyAttackerRedSkin;
			}
		}
		return "default";
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x0001E1D0 File Offset: 0x0001C3D0
	public string GetStickSkin(PlayerTeam team, PlayerRole role)
	{
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team == PlayerTeam.Blue)
				{
					return this.StickGoalieBlueSkin;
				}
				if (team == PlayerTeam.Red)
				{
					return this.StickGoalieRedSkin;
				}
			}
		}
		else
		{
			if (team == PlayerTeam.Blue)
			{
				return this.StickAttackerBlueSkin;
			}
			if (team == PlayerTeam.Red)
			{
				return this.StickAttackerRedSkin;
			}
		}
		return "classic";
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x0001E21C File Offset: 0x0001C41C
	public string GetStickShaftSkin(PlayerTeam team, PlayerRole role)
	{
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team == PlayerTeam.Blue)
				{
					return this.StickShaftGoalieBlueTapeSkin;
				}
				if (team == PlayerTeam.Red)
				{
					return this.StickShaftGoalieRedTapeSkin;
				}
			}
		}
		else
		{
			if (team == PlayerTeam.Blue)
			{
				return this.StickShaftAttackerBlueTapeSkin;
			}
			if (team == PlayerTeam.Red)
			{
				return this.StickShaftAttackerRedTapeSkin;
			}
		}
		return "gray";
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x0001E268 File Offset: 0x0001C468
	public string GetStickBladeSkin(PlayerTeam team, PlayerRole role)
	{
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team == PlayerTeam.Blue)
				{
					return this.StickBladeGoalieBlueTapeSkin;
				}
				if (team == PlayerTeam.Red)
				{
					return this.StickBladeGoalieRedTapeSkin;
				}
			}
		}
		else
		{
			if (team == PlayerTeam.Blue)
			{
				return this.StickBladeAttackerBlueTapeSkin;
			}
			if (team == PlayerTeam.Red)
			{
				return this.StickBladeAttackerRedTapeSkin;
			}
		}
		return "gray";
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x0001E2B4 File Offset: 0x0001C4B4
	public void UpdateDebug(bool value)
	{
		this.Debug = (value ? 1 : 0);
		PlayerPrefs.SetInt("debug", this.Debug);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnDebugChanged", new Dictionary<string, object>
		{
			{
				"value",
				this.Debug
			}
		});
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x0001E308 File Offset: 0x0001C508
	public void UpdateCameraAngle(float value)
	{
		this.CameraAngle = value;
		PlayerPrefs.SetFloat("cameraAngle", this.CameraAngle);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnCameraAngleChanged", new Dictionary<string, object>
		{
			{
				"value",
				this.CameraAngle
			}
		});
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x00009B21 File Offset: 0x00007D21
	public void UpdateHandedness(string value)
	{
		this.Handedness = value;
		PlayerPrefs.SetString("handedness", this.Handedness);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnHandednessChanged", new Dictionary<string, object>
		{
			{
				"value",
				this.Handedness
			}
		});
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x0001E358 File Offset: 0x0001C558
	public void UpdateShowPuckSilhouette(bool value)
	{
		this.ShowPuckSilhouette = (value ? 1 : 0);
		PlayerPrefs.SetInt("showPuckSilhouette", this.ShowPuckSilhouette);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnShowPuckSilhouetteChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x0001E3A8 File Offset: 0x0001C5A8
	public void UpdateShowPuckOutline(bool value)
	{
		this.ShowPuckOutline = (value ? 1 : 0);
		PlayerPrefs.SetInt("showPuckOutline", this.ShowPuckOutline);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnShowPuckOutlineChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x0001E3F8 File Offset: 0x0001C5F8
	public void UpdateShowPuckElevation(bool value)
	{
		this.ShowPuckElevation = (value ? 1 : 0);
		PlayerPrefs.SetInt("showPuckElevation", this.ShowPuckElevation);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnShowPuckElevationChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x0001E448 File Offset: 0x0001C648
	public void UpdateShowPlayerUsernames(bool value)
	{
		this.ShowPlayerUsernames = (value ? 1 : 0);
		PlayerPrefs.SetInt("showPlayerUsernames", this.ShowPlayerUsernames);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnShowPlayerUsernamesChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x00009B5F File Offset: 0x00007D5F
	public void UpdatePlayerUsernamesFadeThreshold(float value)
	{
		this.PlayerUsernamesFadeThreshold = value;
		PlayerPrefs.SetFloat("playerUsernamesFadeThreshold", this.PlayerUsernamesFadeThreshold);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerUsernamesFadeThresholdChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x0001E498 File Offset: 0x0001C698
	public void UpdateUseNetworkSmoothing(bool value)
	{
		this.UseNetworkSmoothing = (value ? 1 : 0);
		PlayerPrefs.SetInt("useNetworkSmoothing", this.UseNetworkSmoothing);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnUseNetworkSmoothingChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x00009B9D File Offset: 0x00007D9D
	public void UpdateNetworkSmoothingStrength(float value)
	{
		this.NetworkSmoothingStrength = value;
		PlayerPrefs.SetFloat("networkSmoothingStrength", this.NetworkSmoothingStrength);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnNetworkSmoothingStrengthChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x0001E4E8 File Offset: 0x0001C6E8
	public void UpdateFilterChatProfanity(bool value)
	{
		this.FilterChatProfanity = (value ? 1 : 0);
		PlayerPrefs.SetInt("filterChatProfanity", this.FilterChatProfanity);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnFilterChatProfanityChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x00009BDB File Offset: 0x00007DDB
	public void UpdateUnits(string value)
	{
		this.Units = value;
		PlayerPrefs.SetString("units", this.Units);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnUnitsChanged", new Dictionary<string, object>
		{
			{
				"value",
				this.Units
			}
		});
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x0001E538 File Offset: 0x0001C738
	public void UpdateShowGameUserInterface(bool value)
	{
		this.ShowGameUserInterface = (value ? 1 : 0);
		PlayerPrefs.SetInt("showGameUserInterface", this.ShowGameUserInterface);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnShowGameUserInterfaceChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x00009C19 File Offset: 0x00007E19
	public void UpdateUserInterfaceScale(float value)
	{
		this.UserInterfaceScale = value;
		PlayerPrefs.SetFloat("userInterfaceScale", this.UserInterfaceScale);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnUserInterfaceScaleChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x00009C57 File Offset: 0x00007E57
	public void UpdateChatOpacity(float value)
	{
		this.ChatOpacity = value;
		PlayerPrefs.SetFloat("chatOpacity", this.ChatOpacity);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnChatOpacityChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x00009C95 File Offset: 0x00007E95
	public void UpdateChatScale(float value)
	{
		this.ChatScale = value;
		PlayerPrefs.SetFloat("chatScale", this.ChatScale);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnChatScaleChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x00009CD3 File Offset: 0x00007ED3
	public void UpdateMinimapOpacity(float value)
	{
		this.MinimapOpacity = value;
		PlayerPrefs.SetFloat("minimapOpacity", this.MinimapOpacity);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMinimapOpacityChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x00009D11 File Offset: 0x00007F11
	public void UpdateMinimapBackgroundOpacity(float value)
	{
		this.MinimapBackgroundOpacity = value;
		PlayerPrefs.SetFloat("minimapBackgroundOpacity", this.MinimapBackgroundOpacity);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMinimapBackgroundOpacityChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x00009D4F File Offset: 0x00007F4F
	public void UpdateMinimapHorizontalPosition(float value)
	{
		this.MinimapHorizontalPosition = value;
		PlayerPrefs.SetFloat("minimapHorizontalPosition", this.MinimapHorizontalPosition);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMinimapHorizontalPositionChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x00009D8D File Offset: 0x00007F8D
	public void UpdateMinimapVerticalPosition(float value)
	{
		this.MinimapVerticalPosition = value;
		PlayerPrefs.SetFloat("minimapVerticalPosition", this.MinimapVerticalPosition);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMinimapVerticalPositionChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x00009DCB File Offset: 0x00007FCB
	public void UpdateMinimapScale(float value)
	{
		this.MinimapScale = value;
		PlayerPrefs.SetFloat("minimapScale", this.MinimapScale);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMinimapScaleChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x00009E09 File Offset: 0x00008009
	public void UpdateGlobalStickSensitivity(float value)
	{
		this.GlobalStickSensitivity = value;
		PlayerPrefs.SetFloat("globalStickSensitivity", this.GlobalStickSensitivity);
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x00009E22 File Offset: 0x00008022
	public void UpdateHorizontalStickSensitivity(float value)
	{
		this.HorizontalStickSensitivity = value;
		PlayerPrefs.SetFloat("horizontalStickSensitivity", this.HorizontalStickSensitivity);
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x00009E3B File Offset: 0x0000803B
	public void UpdateVerticalStickSensitivity(float value)
	{
		this.VerticalStickSensitivity = value;
		PlayerPrefs.SetFloat("verticalStickSensitivity", this.VerticalStickSensitivity);
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x00009E54 File Offset: 0x00008054
	public void UpdateLookSensitivity(float value)
	{
		this.LookSensitivity = value;
		PlayerPrefs.SetFloat("lookSensitivity", this.LookSensitivity);
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x0001E588 File Offset: 0x0001C788
	public void UpdateGlobalVolume(float value)
	{
		this.GlobalVolume = value;
		PlayerPrefs.SetFloat("globalVolume", this.GlobalVolume);
		this.audioMixer.SetFloat("globalVolume", Mathf.Log(this.GlobalVolume + 0.001f) * 20f);
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x0001E5D4 File Offset: 0x0001C7D4
	public void UpdateAmbientVolume(float value)
	{
		this.AmbientVolume = value;
		PlayerPrefs.SetFloat("ambientVolume", this.AmbientVolume);
		this.audioMixer.SetFloat("ambientVolume", Mathf.Log(this.AmbientVolume + 0.001f) * 20f);
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x0001E620 File Offset: 0x0001C820
	public void UpdateGameVolume(float value)
	{
		this.GameVolume = value;
		PlayerPrefs.SetFloat("gameVolume", this.GameVolume);
		this.audioMixer.SetFloat("gameVolume", Mathf.Log(this.GameVolume + 0.001f) * 20f);
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x0001E66C File Offset: 0x0001C86C
	public void UpdateVoiceVolume(float value)
	{
		this.VoiceVolume = value;
		PlayerPrefs.SetFloat("voiceVolume", this.VoiceVolume);
		this.audioMixer.SetFloat("voiceVolume", Mathf.Log(this.VoiceVolume + 0.001f) * 20f);
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x0001E6B8 File Offset: 0x0001C8B8
	public void UpdateUIVolume(float value)
	{
		this.UIVolume = value;
		PlayerPrefs.SetFloat("uiVolume", this.UIVolume);
		this.audioMixer.SetFloat("uiVolume", Mathf.Log(this.UIVolume + 0.001f) * 20f);
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x0001E704 File Offset: 0x0001C904
	public void UpdateWindowMode(string value)
	{
		this.WindowMode = value;
		PlayerPrefs.SetString("windowMode", this.WindowMode);
		string windowMode = this.WindowMode;
		if (windowMode == "FULLSCREEN")
		{
			Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
			return;
		}
		if (windowMode == "BORDERLESS")
		{
			Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
			return;
		}
		if (!(windowMode == "WINDOWED"))
		{
			return;
		}
		Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x0001E76C File Offset: 0x0001C96C
	public void UpdateDisplayIndex(int value, bool isInitialLoad = false)
	{
		SettingsManager.<UpdateDisplayIndex>d__101 <UpdateDisplayIndex>d__;
		<UpdateDisplayIndex>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<UpdateDisplayIndex>d__.<>4__this = this;
		<UpdateDisplayIndex>d__.value = value;
		<UpdateDisplayIndex>d__.isInitialLoad = isInitialLoad;
		<UpdateDisplayIndex>d__.<>1__state = -1;
		<UpdateDisplayIndex>d__.<>t__builder.Start<SettingsManager.<UpdateDisplayIndex>d__101>(ref <UpdateDisplayIndex>d__);
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x0001E7B4 File Offset: 0x0001C9B4
	public void UpdateResolutionIndex(int value)
	{
		this.ResolutionIndex = value;
		PlayerPrefs.SetInt("resolutionIndex", this.ResolutionIndex);
		Screen.SetResolution(Screen.resolutions[this.ResolutionIndex].width, Screen.resolutions[this.ResolutionIndex].height, Screen.fullScreenMode, Screen.resolutions[this.ResolutionIndex].refreshRateRatio);
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x00009E6D File Offset: 0x0000806D
	public void UpdateVSync(bool value)
	{
		this.VSync = (value ? 1 : 0);
		PlayerPrefs.SetInt("vSync", this.VSync);
		if (value)
		{
			QualitySettings.vSyncCount = 1;
			return;
		}
		QualitySettings.vSyncCount = 0;
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x00009E9C File Offset: 0x0000809C
	public void UpdateFpsLimit(int value)
	{
		this.FpsLimit = value;
		PlayerPrefs.SetInt("fpsLimit", this.FpsLimit);
		Application.targetFrameRate = this.FpsLimit;
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x0001E824 File Offset: 0x0001CA24
	public void UpdateFov(float value)
	{
		this.Fov = value;
		PlayerPrefs.SetFloat("fov", this.Fov);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnFovChanged", new Dictionary<string, object>
		{
			{
				"value",
				this.Fov
			}
		});
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x0001E874 File Offset: 0x0001CA74
	public void UpdateQuality(string value)
	{
		this.Quality = value;
		PlayerPrefs.SetString("quality", this.Quality);
		string quality = this.Quality;
		if (!(quality == "LOW"))
		{
			if (!(quality == "MEDIUM"))
			{
				if (!(quality == "HIGH"))
				{
					if (quality == "ULTRA")
					{
						QualitySettings.SetQualityLevel(3, true);
					}
				}
				else
				{
					QualitySettings.SetQualityLevel(2, true);
				}
			}
			else
			{
				QualitySettings.SetQualityLevel(1, true);
			}
		}
		else
		{
			QualitySettings.SetQualityLevel(0, true);
		}
		this.UpdateVSync(this.VSync > 0);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnQualityChanged", new Dictionary<string, object>
		{
			{
				"value",
				this.Quality
			}
		});
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x0001E930 File Offset: 0x0001CB30
	public void UpdateMotionBlur(bool value)
	{
		this.MotionBlur = (value ? 1 : 0);
		PlayerPrefs.SetInt("motionBlur", this.MotionBlur);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMotionBlurChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x00009EC0 File Offset: 0x000080C0
	public void UpdateCountry(string value)
	{
		this.Country = value;
		PlayerPrefs.SetString("country", this.Country);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnCountryChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x0001E980 File Offset: 0x0001CB80
	public void UpdateVisorSkin(PlayerTeam team, PlayerRole role, string value)
	{
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team != PlayerTeam.Blue)
				{
					if (team == PlayerTeam.Red)
					{
						this.VisorGoalieRedSkin = value;
						PlayerPrefs.SetString("visorGoalieRedSkin", this.VisorGoalieRedSkin);
					}
				}
				else
				{
					this.VisorGoalieBlueSkin = value;
					PlayerPrefs.SetString("visorGoalieBlueSkin", this.VisorGoalieBlueSkin);
				}
			}
		}
		else if (team != PlayerTeam.Blue)
		{
			if (team == PlayerTeam.Red)
			{
				this.VisorAttackerRedSkin = value;
				PlayerPrefs.SetString("visorAttackerRedSkin", this.VisorAttackerRedSkin);
			}
		}
		else
		{
			this.VisorAttackerBlueSkin = value;
			PlayerPrefs.SetString("visorAttackerBlueSkin", this.VisorAttackerBlueSkin);
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnVisorSkinChanged", new Dictionary<string, object>
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

	// Token: 0x06000493 RID: 1171 RVA: 0x00009EF9 File Offset: 0x000080F9
	public void UpdateMustache(string value)
	{
		this.Mustache = value;
		PlayerPrefs.SetString("mustache", this.Mustache);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMustacheChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x00009F32 File Offset: 0x00008132
	public void UpdateBeard(string value)
	{
		this.Beard = value;
		PlayerPrefs.SetString("beard", this.Beard);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnBeardChanged", new Dictionary<string, object>
		{
			{
				"value",
				value
			}
		});
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x0001EA50 File Offset: 0x0001CC50
	public void UpdateJerseySkin(PlayerTeam team, PlayerRole role, string value)
	{
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team != PlayerTeam.Blue)
				{
					if (team == PlayerTeam.Red)
					{
						this.JerseyGoalieRedSkin = value;
						PlayerPrefs.SetString("jerseyGoalieRedSkin", this.JerseyGoalieRedSkin);
					}
				}
				else
				{
					this.JerseyGoalieBlueSkin = value;
					PlayerPrefs.SetString("jerseyGoalieBlueSkin", this.JerseyGoalieBlueSkin);
				}
			}
		}
		else if (team != PlayerTeam.Blue)
		{
			if (team == PlayerTeam.Red)
			{
				this.JerseyAttackerRedSkin = value;
				PlayerPrefs.SetString("jerseyAttackerRedSkin", this.JerseyAttackerRedSkin);
			}
		}
		else
		{
			this.JerseyAttackerBlueSkin = value;
			PlayerPrefs.SetString("jerseyAttackerBlueSkin", this.JerseyAttackerBlueSkin);
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnJerseySkinChanged", new Dictionary<string, object>
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

	// Token: 0x06000496 RID: 1174 RVA: 0x0001EB20 File Offset: 0x0001CD20
	public void UpdateStickSkin(PlayerTeam team, PlayerRole role, string value)
	{
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team != PlayerTeam.Blue)
				{
					if (team == PlayerTeam.Red)
					{
						this.StickGoalieRedSkin = value;
						PlayerPrefs.SetString("stickGoalieRedSkin", this.StickGoalieRedSkin);
					}
				}
				else
				{
					this.StickGoalieBlueSkin = value;
					PlayerPrefs.SetString("stickGoalieBlueSkin", this.StickGoalieBlueSkin);
				}
			}
		}
		else if (team != PlayerTeam.Blue)
		{
			if (team == PlayerTeam.Red)
			{
				this.StickAttackerRedSkin = value;
				PlayerPrefs.SetString("stickAttackerRedSkin", this.StickAttackerRedSkin);
			}
		}
		else
		{
			this.StickAttackerBlueSkin = value;
			PlayerPrefs.SetString("stickAttackerBlueSkin", this.StickAttackerBlueSkin);
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnStickSkinChanged", new Dictionary<string, object>
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

	// Token: 0x06000497 RID: 1175 RVA: 0x0001EBF0 File Offset: 0x0001CDF0
	public void UpdateStickShaftSkin(PlayerTeam team, PlayerRole role, string value)
	{
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team != PlayerTeam.Blue)
				{
					if (team == PlayerTeam.Red)
					{
						this.StickShaftGoalieRedTapeSkin = value;
						PlayerPrefs.SetString("stickShaftGoalieRedTapeSkin", this.StickShaftGoalieRedTapeSkin);
					}
				}
				else
				{
					this.StickShaftGoalieBlueTapeSkin = value;
					PlayerPrefs.SetString("stickShaftGoalieBlueTapeSkin", this.StickShaftGoalieBlueTapeSkin);
				}
			}
		}
		else if (team != PlayerTeam.Blue)
		{
			if (team == PlayerTeam.Red)
			{
				this.StickShaftAttackerRedTapeSkin = value;
				PlayerPrefs.SetString("stickShaftAttackerRedTapeSkin", this.StickShaftAttackerRedTapeSkin);
			}
		}
		else
		{
			this.StickShaftAttackerBlueTapeSkin = value;
			PlayerPrefs.SetString("stickShaftAttackerBlueTapeSkin", this.StickShaftAttackerBlueTapeSkin);
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnStickShaftTapeSkinChanged", new Dictionary<string, object>
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

	// Token: 0x06000498 RID: 1176 RVA: 0x0001ECC0 File Offset: 0x0001CEC0
	public void UpdateStickBladeSkin(PlayerTeam team, PlayerRole role, string value)
	{
		if (role != PlayerRole.Attacker)
		{
			if (role == PlayerRole.Goalie)
			{
				if (team != PlayerTeam.Blue)
				{
					if (team == PlayerTeam.Red)
					{
						this.StickBladeGoalieRedTapeSkin = value;
						PlayerPrefs.SetString("stickBladeGoalieRedTapeSkin", this.StickBladeGoalieRedTapeSkin);
					}
				}
				else
				{
					this.StickBladeGoalieBlueTapeSkin = value;
					PlayerPrefs.SetString("stickBladeGoalieBlueTapeSkin", this.StickBladeGoalieBlueTapeSkin);
				}
			}
		}
		else if (team != PlayerTeam.Blue)
		{
			if (team == PlayerTeam.Red)
			{
				this.StickBladeAttackerRedTapeSkin = value;
				PlayerPrefs.SetString("stickBladeAttackerRedTapeSkin", this.StickBladeAttackerRedTapeSkin);
			}
		}
		else
		{
			this.StickBladeAttackerBlueTapeSkin = value;
			PlayerPrefs.SetString("stickBladeAttackerBlueTapeSkin", this.StickBladeAttackerBlueTapeSkin);
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnStickBladeTapeSkinChanged", new Dictionary<string, object>
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

	// Token: 0x0400027B RID: 635
	[Header("References")]
	public AudioMixer audioMixer;

	// Token: 0x0400027C RID: 636
	public int Debug;

	// Token: 0x0400027D RID: 637
	public float CameraAngle;

	// Token: 0x0400027E RID: 638
	public string Handedness;

	// Token: 0x0400027F RID: 639
	public int ShowPuckSilhouette;

	// Token: 0x04000280 RID: 640
	public int ShowPuckOutline;

	// Token: 0x04000281 RID: 641
	public int ShowPuckElevation;

	// Token: 0x04000282 RID: 642
	public int ShowPlayerUsernames;

	// Token: 0x04000283 RID: 643
	public float PlayerUsernamesFadeThreshold;

	// Token: 0x04000284 RID: 644
	public int UseNetworkSmoothing;

	// Token: 0x04000285 RID: 645
	public float NetworkSmoothingStrength;

	// Token: 0x04000286 RID: 646
	public int FilterChatProfanity;

	// Token: 0x04000287 RID: 647
	public string Units;

	// Token: 0x04000288 RID: 648
	public int ShowGameUserInterface;

	// Token: 0x04000289 RID: 649
	public float UserInterfaceScale;

	// Token: 0x0400028A RID: 650
	public float ChatOpacity;

	// Token: 0x0400028B RID: 651
	public float ChatScale;

	// Token: 0x0400028C RID: 652
	public float MinimapOpacity;

	// Token: 0x0400028D RID: 653
	public float MinimapBackgroundOpacity;

	// Token: 0x0400028E RID: 654
	public float MinimapHorizontalPosition;

	// Token: 0x0400028F RID: 655
	public float MinimapVerticalPosition;

	// Token: 0x04000290 RID: 656
	public float MinimapScale;

	// Token: 0x04000291 RID: 657
	public float GlobalStickSensitivity;

	// Token: 0x04000292 RID: 658
	public float HorizontalStickSensitivity;

	// Token: 0x04000293 RID: 659
	public float VerticalStickSensitivity;

	// Token: 0x04000294 RID: 660
	public float LookSensitivity;

	// Token: 0x04000295 RID: 661
	public float GlobalVolume;

	// Token: 0x04000296 RID: 662
	public float AmbientVolume;

	// Token: 0x04000297 RID: 663
	public float GameVolume;

	// Token: 0x04000298 RID: 664
	public float VoiceVolume;

	// Token: 0x04000299 RID: 665
	public float UIVolume;

	// Token: 0x0400029A RID: 666
	public string WindowMode;

	// Token: 0x0400029B RID: 667
	public int DisplayIndex;

	// Token: 0x0400029C RID: 668
	public int ResolutionIndex;

	// Token: 0x0400029D RID: 669
	public int VSync;

	// Token: 0x0400029E RID: 670
	public int FpsLimit;

	// Token: 0x0400029F RID: 671
	public float Fov;

	// Token: 0x040002A0 RID: 672
	public string Quality;

	// Token: 0x040002A1 RID: 673
	public int MotionBlur;

	// Token: 0x040002A2 RID: 674
	public string Country;

	// Token: 0x040002A3 RID: 675
	public string VisorAttackerBlueSkin;

	// Token: 0x040002A4 RID: 676
	public string VisorAttackerRedSkin;

	// Token: 0x040002A5 RID: 677
	public string VisorGoalieBlueSkin;

	// Token: 0x040002A6 RID: 678
	public string VisorGoalieRedSkin;

	// Token: 0x040002A7 RID: 679
	public string Mustache;

	// Token: 0x040002A8 RID: 680
	public string Beard;

	// Token: 0x040002A9 RID: 681
	public string JerseyAttackerBlueSkin;

	// Token: 0x040002AA RID: 682
	public string JerseyAttackerRedSkin;

	// Token: 0x040002AB RID: 683
	public string JerseyGoalieBlueSkin;

	// Token: 0x040002AC RID: 684
	public string JerseyGoalieRedSkin;

	// Token: 0x040002AD RID: 685
	public string StickAttackerBlueSkin;

	// Token: 0x040002AE RID: 686
	public string StickAttackerRedSkin;

	// Token: 0x040002AF RID: 687
	public string StickGoalieBlueSkin;

	// Token: 0x040002B0 RID: 688
	public string StickGoalieRedSkin;

	// Token: 0x040002B1 RID: 689
	public string StickShaftAttackerBlueTapeSkin;

	// Token: 0x040002B2 RID: 690
	public string StickShaftAttackerRedTapeSkin;

	// Token: 0x040002B3 RID: 691
	public string StickShaftGoalieBlueTapeSkin;

	// Token: 0x040002B4 RID: 692
	public string StickShaftGoalieRedTapeSkin;

	// Token: 0x040002B5 RID: 693
	public string StickBladeAttackerBlueTapeSkin;

	// Token: 0x040002B6 RID: 694
	public string StickBladeAttackerRedTapeSkin;

	// Token: 0x040002B7 RID: 695
	public string StickBladeGoalieBlueTapeSkin;

	// Token: 0x040002B8 RID: 696
	public string StickBladeGoalieRedTapeSkin;
}
