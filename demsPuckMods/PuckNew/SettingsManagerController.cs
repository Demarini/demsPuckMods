using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x0200012A RID: 298
public static class SettingsManagerController
{
	// Token: 0x06000887 RID: 2183 RVA: 0x00029598 File Offset: 0x00027798
	public static void Initialize()
	{
		EventManager.AddEventListener("Event_OnDisplayIndexChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnDisplayIndexChanged));
		EventManager.AddEventListener("Event_OnIsDisplayChangeInProgressChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnIsDisplayChangeInProgressChanged));
		EventManager.AddEventListener("Event_OnBaseCameraEnabled", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnBaseCameraEnabled));
		EventManager.AddEventListener("Event_OnPopupClickOk", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnPopupClickOk));
		SettingsManagerController.AddSettingsEventListeners();
		SettingsManagerController.AddAppearanceEventListeners();
		InputManager.Debug1Action.performed += SettingsManagerController.OnDebug1ActionPerformed;
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x00029620 File Offset: 0x00027820
	public static void Dispose()
	{
		EventManager.RemoveEventListener("Event_OnDisplayIndexChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnDisplayIndexChanged));
		EventManager.RemoveEventListener("Event_OnIsDisplayChangeInProgressChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnIsDisplayChangeInProgressChanged));
		EventManager.RemoveEventListener("Event_OnBaseCameraEnabled", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnBaseCameraEnabled));
		EventManager.RemoveEventListener("Event_OnPopupClickOk", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnPopupClickOk));
		SettingsManagerController.RemoveSettingsEventListeners();
		SettingsManagerController.RemoveAppearanceEventListeners();
		InputManager.Debug1Action.performed -= SettingsManagerController.OnDebug1ActionPerformed;
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x000296A8 File Offset: 0x000278A8
	private static void AddSettingsEventListeners()
	{
		EventManager.AddEventListener("Event_OnSettingsCameraAngleChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsCameraAngleChanged));
		EventManager.AddEventListener("Event_OnSettingsHandednessChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsHandednessChanged));
		EventManager.AddEventListener("Event_OnSettingsShowPuckSilhouetteChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsShowPuckSilhouetteChanged));
		EventManager.AddEventListener("Event_OnSettingsShowPuckOutlineChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsShowPuckOutlineChanged));
		EventManager.AddEventListener("Event_OnSettingsShowPuckElevationChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsShowPuckElevationChanged));
		EventManager.AddEventListener("Event_OnSettingsShowPlayerUsernamesChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsShowPlayerUsernamesChanged));
		EventManager.AddEventListener("Event_OnSettingsPlayerUsernamesFadeThresholdChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsPlayerUsernamesFadeThresholdChanged));
		EventManager.AddEventListener("Event_OnSettingsUseNetworkSmoothingChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsUseNetworkSmoothingChanged));
		EventManager.AddEventListener("Event_OnSettingsNetworkSmoothingStrengthChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsNetworkSmoothingStrengthChanged));
		EventManager.AddEventListener("Event_OnSettingsMaxMatchmakingPingChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsMaxMatchmakingPingChanged));
		EventManager.AddEventListener("Event_OnSettingsFilterChatProfanityChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsFilterChatProfanityChanged));
		EventManager.AddEventListener("Event_OnSettingsUnitsChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsUnitsChanged));
		EventManager.AddEventListener("Event_OnSettingsShowGameUserInterfaceChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsShowGameUserInterfaceChanged));
		EventManager.AddEventListener("Event_OnSettingsUserInterfaceScaleChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsUserInterfaceScaleChanged));
		EventManager.AddEventListener("Event_OnSettingsChatOpacityChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsChatOpacityChanged));
		EventManager.AddEventListener("Event_OnSettingsChatScaleChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsChatScaleChanged));
		EventManager.AddEventListener("Event_OnSettingsMinimapOpacityChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsMinimapOpacityChanged));
		EventManager.AddEventListener("Event_OnSettingsMinimapBackgroundOpacityChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsMinimapBackgroundOpacityChanged));
		EventManager.AddEventListener("Event_OnSettingsMinimapHorizontalPositionChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsMinimapHorizontalPositionChanged));
		EventManager.AddEventListener("Event_OnSettingsMinimapVerticalPositionChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsMinimapVerticalPositionChanged));
		EventManager.AddEventListener("Event_OnSettingsMinimapScaleChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsMinimapScaleChanged));
		EventManager.AddEventListener("Event_OnSettingsGlobalStickSensitivityChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsGlobalStickSensitivityChanged));
		EventManager.AddEventListener("Event_OnSettingsHorizontalStickSensitivityChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsHorizontalStickSensitivityChanged));
		EventManager.AddEventListener("Event_OnSettingsVerticalStickSensitivityChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsVerticalStickSensitivityChanged));
		EventManager.AddEventListener("Event_OnSettingsLookSensitivityChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsLookSensitivityChanged));
		EventManager.AddEventListener("Event_OnSettingsGlobalVolumeChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsGlobalVolumeChanged));
		EventManager.AddEventListener("Event_OnSettingsAmbientVolumeChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsAmbientVolumeChanged));
		EventManager.AddEventListener("Event_OnSettingsGameVolumeChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsGameVolumeChanged));
		EventManager.AddEventListener("Event_OnSettingsVoiceVolumeChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsVoiceVolumeChanged));
		EventManager.AddEventListener("Event_OnSettingsUIVolumeChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsUIVolumeChanged));
		EventManager.AddEventListener("Event_OnSettingsFullScreenModeChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsFullScreenModeChanged));
		EventManager.AddEventListener("Event_OnSettingsDisplayChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsDisplayChanged));
		EventManager.AddEventListener("Event_OnSettingsResolutionChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsResolutionChanged));
		EventManager.AddEventListener("Event_OnSettingsVSyncChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsVSyncChanged));
		EventManager.AddEventListener("Event_OnSettingsFpsLimitChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsFpsLimitChanged));
		EventManager.AddEventListener("Event_OnSettingsFovChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsFovChanged));
		EventManager.AddEventListener("Event_OnSettingsQualityChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsQualityChanged));
		EventManager.AddEventListener("Event_OnSettingsMotionBlurChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsMotionBlurChanged));
	}

	// Token: 0x0600088A RID: 2186 RVA: 0x000299FC File Offset: 0x00027BFC
	private static void AddAppearanceEventListeners()
	{
		EventManager.AddEventListener("Event_OnAppearanceTeamChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnAppearanceTeamChanged));
		EventManager.AddEventListener("Event_OnAppearanceRoleChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnAppearanceRoleChanged));
		EventManager.AddEventListener("Event_OnAppearanceApplyForBothTeamsChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnAppearanceApplyForBothTeamsChanged));
		EventManager.AddEventListener("Event_OnAppearanceClickItem", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnAppearanceClickItem));
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x00029A64 File Offset: 0x00027C64
	private static void RemoveSettingsEventListeners()
	{
		EventManager.RemoveEventListener("Event_OnSettingsCameraAngleChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsCameraAngleChanged));
		EventManager.RemoveEventListener("Event_OnSettingsHandednessChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsHandednessChanged));
		EventManager.RemoveEventListener("Event_OnSettingsShowPuckSilhouetteChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsShowPuckSilhouetteChanged));
		EventManager.RemoveEventListener("Event_OnSettingsShowPuckOutlineChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsShowPuckOutlineChanged));
		EventManager.RemoveEventListener("Event_OnSettingsShowPuckElevationChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsShowPuckElevationChanged));
		EventManager.RemoveEventListener("Event_OnSettingsShowPlayerUsernamesChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsShowPlayerUsernamesChanged));
		EventManager.RemoveEventListener("Event_OnSettingsPlayerUsernamesFadeThresholdChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsPlayerUsernamesFadeThresholdChanged));
		EventManager.RemoveEventListener("Event_OnSettingsUseNetworkSmoothingChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsUseNetworkSmoothingChanged));
		EventManager.RemoveEventListener("Event_OnSettingsNetworkSmoothingStrengthChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsNetworkSmoothingStrengthChanged));
		EventManager.RemoveEventListener("Event_OnSettingsMaxMatchmakingPingChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsMaxMatchmakingPingChanged));
		EventManager.RemoveEventListener("Event_OnSettingsFilterChatProfanityChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsFilterChatProfanityChanged));
		EventManager.RemoveEventListener("Event_OnSettingsUnitsChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsUnitsChanged));
		EventManager.RemoveEventListener("Event_OnSettingsShowGameUserInterfaceChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsShowGameUserInterfaceChanged));
		EventManager.RemoveEventListener("Event_OnSettingsUserInterfaceScaleChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsUserInterfaceScaleChanged));
		EventManager.RemoveEventListener("Event_OnSettingsChatOpacityChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsChatOpacityChanged));
		EventManager.RemoveEventListener("Event_OnSettingsChatScaleChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsChatScaleChanged));
		EventManager.RemoveEventListener("Event_OnSettingsMinimapOpacityChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsMinimapOpacityChanged));
		EventManager.RemoveEventListener("Event_OnSettingsMinimapBackgroundOpacityChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsMinimapBackgroundOpacityChanged));
		EventManager.RemoveEventListener("Event_OnSettingsMinimapHorizontalPositionChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsMinimapHorizontalPositionChanged));
		EventManager.RemoveEventListener("Event_OnSettingsMinimapVerticalPositionChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsMinimapVerticalPositionChanged));
		EventManager.RemoveEventListener("Event_OnSettingsMinimapScaleChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsMinimapScaleChanged));
		EventManager.RemoveEventListener("Event_OnSettingsGlobalStickSensitivityChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsGlobalStickSensitivityChanged));
		EventManager.RemoveEventListener("Event_OnSettingsHorizontalStickSensitivityChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsHorizontalStickSensitivityChanged));
		EventManager.RemoveEventListener("Event_OnSettingsVerticalStickSensitivityChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsVerticalStickSensitivityChanged));
		EventManager.RemoveEventListener("Event_OnSettingsLookSensitivityChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsLookSensitivityChanged));
		EventManager.RemoveEventListener("Event_OnSettingsGlobalVolumeChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsGlobalVolumeChanged));
		EventManager.RemoveEventListener("Event_OnSettingsAmbientVolumeChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsAmbientVolumeChanged));
		EventManager.RemoveEventListener("Event_OnSettingsGameVolumeChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsGameVolumeChanged));
		EventManager.RemoveEventListener("Event_OnSettingsVoiceVolumeChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsVoiceVolumeChanged));
		EventManager.RemoveEventListener("Event_OnSettingsUIVolumeChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsUIVolumeChanged));
		EventManager.RemoveEventListener("Event_OnSettingsFullScreenModeChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsFullScreenModeChanged));
		EventManager.RemoveEventListener("Event_OnSettingsDisplayChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsDisplayChanged));
		EventManager.RemoveEventListener("Event_OnSettingsResolutionChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsResolutionChanged));
		EventManager.RemoveEventListener("Event_OnSettingsVSyncChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsVSyncChanged));
		EventManager.RemoveEventListener("Event_OnSettingsFpsLimitChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsFpsLimitChanged));
		EventManager.RemoveEventListener("Event_OnSettingsFovChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsFovChanged));
		EventManager.RemoveEventListener("Event_OnSettingsQualityChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsQualityChanged));
		EventManager.RemoveEventListener("Event_OnSettingsMotionBlurChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnSettingsMotionBlurChanged));
	}

	// Token: 0x0600088C RID: 2188 RVA: 0x00029DB8 File Offset: 0x00027FB8
	private static void RemoveAppearanceEventListeners()
	{
		EventManager.RemoveEventListener("Event_OnAppearanceTeamChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnAppearanceTeamChanged));
		EventManager.RemoveEventListener("Event_OnAppearanceRoleChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnAppearanceRoleChanged));
		EventManager.RemoveEventListener("Event_OnAppearanceApplyForBothTeamsChanged", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnAppearanceApplyForBothTeamsChanged));
		EventManager.RemoveEventListener("Event_OnAppearanceClickItem", new Action<Dictionary<string, object>>(SettingsManagerController.Event_OnAppearanceClickItem));
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x00029E1D File Offset: 0x0002801D
	private static void OnDebug1ActionPerformed(InputAction.CallbackContext context)
	{
		SettingsManager.UpdateDebug(!SettingsManager.Debug);
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x00029E2C File Offset: 0x0002802C
	private static void Event_OnSettingsCameraAngleChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateCameraAngle((float)message["value"]);
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x00029E43 File Offset: 0x00028043
	private static void Event_OnSettingsHandednessChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateHandedness(Utils.GetHandednessFromName((string)message["value"]));
	}

	// Token: 0x06000890 RID: 2192 RVA: 0x00029E5F File Offset: 0x0002805F
	private static void Event_OnSettingsShowPuckSilhouetteChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateShowPuckSilhouette((bool)message["value"]);
	}

	// Token: 0x06000891 RID: 2193 RVA: 0x00029E76 File Offset: 0x00028076
	private static void Event_OnSettingsShowPuckOutlineChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateShowPuckOutline((bool)message["value"]);
	}

	// Token: 0x06000892 RID: 2194 RVA: 0x00029E8D File Offset: 0x0002808D
	private static void Event_OnSettingsShowPuckElevationChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateShowPuckElevation((bool)message["value"]);
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x00029EA4 File Offset: 0x000280A4
	private static void Event_OnSettingsShowPlayerUsernamesChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateShowPlayerUsernames((bool)message["value"]);
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x00029EBB File Offset: 0x000280BB
	private static void Event_OnSettingsPlayerUsernamesFadeThresholdChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdatePlayerUsernamesFadeThreshold((float)message["value"]);
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x00029ED2 File Offset: 0x000280D2
	private static void Event_OnSettingsUseNetworkSmoothingChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateUseNetworkSmoothing((bool)message["value"]);
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x00029EE9 File Offset: 0x000280E9
	private static void Event_OnSettingsNetworkSmoothingStrengthChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateNetworkSmoothingStrength((int)message["value"]);
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x00029F00 File Offset: 0x00028100
	private static void Event_OnSettingsMaxMatchmakingPingChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateMaxMatchmakingPing((int)message["value"]);
	}

	// Token: 0x06000898 RID: 2200 RVA: 0x00029F17 File Offset: 0x00028117
	private static void Event_OnSettingsFilterChatProfanityChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateFilterChatProfanity((bool)message["value"]);
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x00029F2E File Offset: 0x0002812E
	private static void Event_OnSettingsUnitsChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateUnits(Utils.GetUnitsFromName((string)message["value"]));
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x00029F4A File Offset: 0x0002814A
	private static void Event_OnSettingsShowGameUserInterfaceChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateShowGameUserInterface((bool)message["value"]);
	}

	// Token: 0x0600089B RID: 2203 RVA: 0x00029F61 File Offset: 0x00028161
	private static void Event_OnSettingsUserInterfaceScaleChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateUserInterfaceScale((float)message["value"]);
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x00029F78 File Offset: 0x00028178
	private static void Event_OnSettingsChatOpacityChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateChatOpacity((float)message["value"]);
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x00029F8F File Offset: 0x0002818F
	private static void Event_OnSettingsChatScaleChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateChatScale((float)message["value"]);
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x00029FA6 File Offset: 0x000281A6
	private static void Event_OnSettingsMinimapOpacityChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateMinimapOpacity((float)message["value"]);
	}

	// Token: 0x0600089F RID: 2207 RVA: 0x00029FBD File Offset: 0x000281BD
	private static void Event_OnSettingsMinimapBackgroundOpacityChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateMinimapBackgroundOpacity((float)message["value"]);
	}

	// Token: 0x060008A0 RID: 2208 RVA: 0x00029FD4 File Offset: 0x000281D4
	private static void Event_OnSettingsMinimapHorizontalPositionChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateMinimapHorizontalPosition((float)message["value"]);
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x00029FEB File Offset: 0x000281EB
	private static void Event_OnSettingsMinimapVerticalPositionChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateMinimapVerticalPosition((float)message["value"]);
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x0002A002 File Offset: 0x00028202
	private static void Event_OnSettingsMinimapScaleChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateMinimapScale((float)message["value"]);
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x0002A019 File Offset: 0x00028219
	private static void Event_OnSettingsGlobalStickSensitivityChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateGlobalStickSensitivity((float)message["value"]);
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x0002A030 File Offset: 0x00028230
	private static void Event_OnSettingsHorizontalStickSensitivityChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateHorizontalStickSensitivity((float)message["value"]);
	}

	// Token: 0x060008A5 RID: 2213 RVA: 0x0002A047 File Offset: 0x00028247
	private static void Event_OnSettingsVerticalStickSensitivityChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateVerticalStickSensitivity((float)message["value"]);
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x0002A05E File Offset: 0x0002825E
	private static void Event_OnSettingsLookSensitivityChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateLookSensitivity((float)message["value"]);
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x0002A075 File Offset: 0x00028275
	private static void Event_OnSettingsGlobalVolumeChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateGlobalVolume((float)message["value"]);
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x0002A08C File Offset: 0x0002828C
	private static void Event_OnSettingsAmbientVolumeChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateAmbientVolume((float)message["value"]);
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x0002A0A3 File Offset: 0x000282A3
	private static void Event_OnSettingsGameVolumeChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateGameVolume((float)message["value"]);
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x0002A0BA File Offset: 0x000282BA
	private static void Event_OnSettingsVoiceVolumeChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateVoiceVolume((float)message["value"]);
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x0002A0D1 File Offset: 0x000282D1
	private static void Event_OnSettingsUIVolumeChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateUIVolume((float)message["value"]);
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x0002A0E8 File Offset: 0x000282E8
	private static void Event_OnSettingsFullScreenModeChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateFullScreenMode(Utils.GetFullScreenModeFromName((string)message["value"]));
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x0002A104 File Offset: 0x00028304
	private static void Event_OnSettingsDisplayChanged(Dictionary<string, object> message)
	{
		string text = (string)message["value"];
		int displayIndexFromName = Utils.GetDisplayIndexFromName(text);
		if (displayIndexFromName == -1)
		{
			Debug.LogWarning("[SettingsManagerController] Could not find display index for display name " + text + ", skipping update");
			return;
		}
		SettingsManager.UpdateDisplayIndex(displayIndexFromName);
	}

	// Token: 0x060008AE RID: 2222 RVA: 0x0002A14C File Offset: 0x0002834C
	private static void Event_OnSettingsResolutionChanged(Dictionary<string, object> message)
	{
		string text = (string)message["value"];
		int resolutionIndexFromName = Utils.GetResolutionIndexFromName(text);
		if (resolutionIndexFromName == -1)
		{
			Debug.LogWarning("[SettingsManagerController] Could not find resolution index for resolution name " + text + ", skipping update");
			return;
		}
		SettingsManager.UpdateResolutionIndex(resolutionIndexFromName);
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x0002A191 File Offset: 0x00028391
	private static void Event_OnSettingsVSyncChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateVSync((bool)message["value"]);
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x0002A1A8 File Offset: 0x000283A8
	private static void Event_OnSettingsFpsLimitChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateFpsLimit((int)message["value"]);
	}

	// Token: 0x060008B1 RID: 2225 RVA: 0x0002A1BF File Offset: 0x000283BF
	private static void Event_OnSettingsFovChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateFov((float)message["value"]);
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x0002A1D6 File Offset: 0x000283D6
	private static void Event_OnSettingsQualityChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateQuality(Utils.GetApplicationQualityFromName((string)message["value"]));
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x0002A1F2 File Offset: 0x000283F2
	private static void Event_OnSettingsMotionBlurChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateMotionBlur((bool)message["value"]);
	}

	// Token: 0x060008B4 RID: 2228 RVA: 0x0002A209 File Offset: 0x00028409
	private static void Event_OnAppearanceTeamChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateTeam(Utils.GetTeamFromName((string)message["value"]));
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x0002A225 File Offset: 0x00028425
	private static void Event_OnAppearanceRoleChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateRole(Utils.GetRoleFromName((string)message["value"]));
	}

	// Token: 0x060008B6 RID: 2230 RVA: 0x0002A241 File Offset: 0x00028441
	private static void Event_OnAppearanceApplyForBothTeamsChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateApplyForBothTeams((bool)message["value"]);
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x0002A258 File Offset: 0x00028458
	private static void Event_OnAppearanceClickItem(Dictionary<string, object> message)
	{
		Item item = (Item)message["item"];
		AppearanceCategory appearanceCategory = (AppearanceCategory)message["category"];
		AppearanceSubcategory appearanceSubcategory = (AppearanceSubcategory)message["subcategory"];
		PlayerTeam playerTeam = (PlayerTeam)message["team"];
		PlayerRole role = (PlayerRole)message["role"];
		if (!item.IsOwned)
		{
			return;
		}
		PlayerTeam team = (playerTeam == PlayerTeam.Blue) ? PlayerTeam.Red : PlayerTeam.Blue;
		switch (appearanceSubcategory)
		{
		case AppearanceSubcategory.Headgear:
			SettingsManager.UpdateHeadgearID(playerTeam, role, item.id);
			if (SettingsManager.ApplyForBothTeams)
			{
				SettingsManager.UpdateHeadgearID(team, role, item.id);
				return;
			}
			break;
		case AppearanceSubcategory.Flags:
			SettingsManager.UpdateFlagID(item.id);
			return;
		case AppearanceSubcategory.Mustaches:
			SettingsManager.UpdateMustacheID(item.id);
			return;
		case AppearanceSubcategory.Beards:
			SettingsManager.UpdateBeardID(item.id);
			return;
		case AppearanceSubcategory.Jerseys:
			SettingsManager.UpdateJerseyID(playerTeam, role, item.id);
			if (SettingsManager.ApplyForBothTeams)
			{
				SettingsManager.UpdateJerseyID(team, role, item.id);
				return;
			}
			break;
		case AppearanceSubcategory.StickSkins:
			SettingsManager.UpdateStickSkinID(playerTeam, role, item.id);
			if (SettingsManager.ApplyForBothTeams)
			{
				SettingsManager.UpdateStickSkinID(team, role, item.id);
				return;
			}
			break;
		case AppearanceSubcategory.StickShaftTapes:
			SettingsManager.UpdateStickShaftTapeID(playerTeam, role, item.id);
			if (SettingsManager.ApplyForBothTeams)
			{
				SettingsManager.UpdateStickShaftTapeID(team, role, item.id);
				return;
			}
			break;
		case AppearanceSubcategory.StickBladeTapes:
			SettingsManager.UpdateStickBladeTapeID(playerTeam, role, item.id);
			if (SettingsManager.ApplyForBothTeams)
			{
				SettingsManager.UpdateStickBladeTapeID(team, role, item.id);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x0002A3C9 File Offset: 0x000285C9
	private static void Event_OnDisplayIndexChanged(Dictionary<string, object> message)
	{
		SettingsManager.UpdateResolutionIndex(-1);
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x0002A3D1 File Offset: 0x000285D1
	private static void Event_OnIsDisplayChangeInProgressChanged(Dictionary<string, object> message)
	{
		if ((bool)message["isDisplayChangeInProgress"])
		{
			return;
		}
		if (SettingsManager.ResolutionIndex == -1)
		{
			SettingsManager.UpdateResolutionIndex(Utils.GetResolutions().Count - 1);
		}
	}

	// Token: 0x060008BA RID: 2234 RVA: 0x0002A3FF File Offset: 0x000285FF
	private static void Event_OnBaseCameraEnabled(Dictionary<string, object> message)
	{
		((BaseCamera)message["baseCamera"]).SetFieldOfView(SettingsManager.Fov);
	}

	// Token: 0x060008BB RID: 2235 RVA: 0x0002A41B File Offset: 0x0002861B
	private static void Event_OnPopupClickOk(Dictionary<string, object> message)
	{
		if ((string)message["name"] == "settingsResetToDefault")
		{
			SettingsManager.ResetToDefault();
		}
		EventManager.TriggerEvent("Event_OnSettingsResetToDefault", null);
	}
}
