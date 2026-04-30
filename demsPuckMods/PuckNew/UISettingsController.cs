using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001D0 RID: 464
public class UISettingsController : UIViewController<UISettings>
{
	// Token: 0x06000D88 RID: 3464 RVA: 0x0004022C File Offset: 0x0003E42C
	public override void Awake()
	{
		base.Awake();
		this.uiSettings = base.GetComponent<UISettings>();
		EventManager.AddEventListener("Event_OnCameraAngleChanged", new Action<Dictionary<string, object>>(this.Event_OnCameraAngleChanged));
		EventManager.AddEventListener("Event_OnHandednessChanged", new Action<Dictionary<string, object>>(this.Event_OnHandednessChanged));
		EventManager.AddEventListener("Event_OnShowPuckSilhouetteChanged", new Action<Dictionary<string, object>>(this.Event_OnShowPuckSilhouetteChanged));
		EventManager.AddEventListener("Event_OnShowPuckOutlineChanged", new Action<Dictionary<string, object>>(this.Event_OnShowPuckOutlineChanged));
		EventManager.AddEventListener("Event_OnShowPuckElevationChanged", new Action<Dictionary<string, object>>(this.Event_OnShowPuckElevationChanged));
		EventManager.AddEventListener("Event_OnShowPlayerUsernamesChanged", new Action<Dictionary<string, object>>(this.Event_OnShowPlayerUsernamesChanged));
		EventManager.AddEventListener("Event_OnPlayerUsernamesFadeThresholdChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerUsernamesFadeThresholdChanged));
		EventManager.AddEventListener("Event_OnUseNetworkSmoothingChanged", new Action<Dictionary<string, object>>(this.Event_OnUseNetworkSmoothingChanged));
		EventManager.AddEventListener("Event_OnNetworkSmoothingStrengthChanged", new Action<Dictionary<string, object>>(this.Event_OnNetworkSmoothingStrengthChanged));
		EventManager.AddEventListener("Event_OnMaxMatchmakingPingChanged", new Action<Dictionary<string, object>>(this.Event_OnMaxMatchmakingPingChanged));
		EventManager.AddEventListener("Event_OnFilterChatProfanityChanged", new Action<Dictionary<string, object>>(this.Event_OnFilterChatProfanityChanged));
		EventManager.AddEventListener("Event_OnUnitsChanged", new Action<Dictionary<string, object>>(this.Event_OnUnitsChanged));
		EventManager.AddEventListener("Event_OnShowGameUserInterfaceChanged", new Action<Dictionary<string, object>>(this.Event_OnShowGameUserInterfaceChanged));
		EventManager.AddEventListener("Event_OnUserInterfaceScaleChanged", new Action<Dictionary<string, object>>(this.Event_OnUserInterfaceScaleChanged));
		EventManager.AddEventListener("Event_OnChatOpacityChanged", new Action<Dictionary<string, object>>(this.Event_OnChatOpacityChanged));
		EventManager.AddEventListener("Event_OnChatScaleChanged", new Action<Dictionary<string, object>>(this.Event_OnChatScaleChanged));
		EventManager.AddEventListener("Event_OnMinimapOpacityChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapOpacityChanged));
		EventManager.AddEventListener("Event_OnMinimapBackgroundOpacityChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapBackgroundOpacityChanged));
		EventManager.AddEventListener("Event_OnMinimapHorizontalPositionChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapHorizontalPositionChanged));
		EventManager.AddEventListener("Event_OnMinimapVerticalPositionChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapVerticalPositionChanged));
		EventManager.AddEventListener("Event_OnMinimapScaleChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapScaleChanged));
		EventManager.AddEventListener("Event_OnGlobalStickSensitivityChanged", new Action<Dictionary<string, object>>(this.Event_OnGlobalStickSensitivityChanged));
		EventManager.AddEventListener("Event_OnHorizontalStickSensitivityChanged", new Action<Dictionary<string, object>>(this.Event_OnHorizontalStickSensitivityChanged));
		EventManager.AddEventListener("Event_OnVerticalStickSensitivityChanged", new Action<Dictionary<string, object>>(this.Event_OnVerticalStickSensitivityChanged));
		EventManager.AddEventListener("Event_OnLookSensitivityChanged", new Action<Dictionary<string, object>>(this.Event_OnLookSensitivityChanged));
		EventManager.AddEventListener("Event_OnKeyBindsLoaded", new Action<Dictionary<string, object>>(this.Event_OnKeyBindsLoaded));
		EventManager.AddEventListener("Event_OnKeyBindsSaved", new Action<Dictionary<string, object>>(this.Event_OnKeyBindsSaved));
		EventManager.AddEventListener("Event_OnGlobalVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnGlobalVolumeChanged));
		EventManager.AddEventListener("Event_OnAmbientVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnAmbientVolumeChanged));
		EventManager.AddEventListener("Event_OnGameVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnGameVolumeChanged));
		EventManager.AddEventListener("Event_OnVoiceVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnVoiceVolumeChanged));
		EventManager.AddEventListener("Event_OnUIVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnUIVolumeChanged));
		EventManager.AddEventListener("Event_OnFullScreenModeChanged", new Action<Dictionary<string, object>>(this.Event_OnFullScreenModeChanged));
		EventManager.AddEventListener("Event_OnDisplayIndexChanged", new Action<Dictionary<string, object>>(this.Event_OnDisplayIndexChanged));
		EventManager.AddEventListener("Event_OnResolutionIndexChanged", new Action<Dictionary<string, object>>(this.Event_OnResolutionIndexChanged));
		EventManager.AddEventListener("Event_OnVSyncChanged", new Action<Dictionary<string, object>>(this.Event_OnVSyncChanged));
		EventManager.AddEventListener("Event_OnFpsLimitChanged", new Action<Dictionary<string, object>>(this.Event_OnFpsLimitChanged));
		EventManager.AddEventListener("Event_OnFovChanged", new Action<Dictionary<string, object>>(this.Event_OnFovChanged));
		EventManager.AddEventListener("Event_OnQualityChanged", new Action<Dictionary<string, object>>(this.Event_OnQualityChanged));
		EventManager.AddEventListener("Event_OnMotionBlurChanged", new Action<Dictionary<string, object>>(this.Event_OnMotionBlurChanged));
		EventManager.AddEventListener("Event_OnIsDisplayChangeInProgressChanged", new Action<Dictionary<string, object>>(this.Event_OnIsDisplayChangeInProgressChanged));
	}

	// Token: 0x06000D89 RID: 3465 RVA: 0x000405D4 File Offset: 0x0003E7D4
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnCameraAngleChanged", new Action<Dictionary<string, object>>(this.Event_OnCameraAngleChanged));
		EventManager.RemoveEventListener("Event_OnHandednessChanged", new Action<Dictionary<string, object>>(this.Event_OnHandednessChanged));
		EventManager.RemoveEventListener("Event_OnShowPuckSilhouetteChanged", new Action<Dictionary<string, object>>(this.Event_OnShowPuckSilhouetteChanged));
		EventManager.RemoveEventListener("Event_OnShowPuckOutlineChanged", new Action<Dictionary<string, object>>(this.Event_OnShowPuckOutlineChanged));
		EventManager.RemoveEventListener("Event_OnShowPuckElevationChanged", new Action<Dictionary<string, object>>(this.Event_OnShowPuckElevationChanged));
		EventManager.RemoveEventListener("Event_OnShowPlayerUsernamesChanged", new Action<Dictionary<string, object>>(this.Event_OnShowPlayerUsernamesChanged));
		EventManager.RemoveEventListener("Event_OnPlayerUsernamesFadeThresholdChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerUsernamesFadeThresholdChanged));
		EventManager.RemoveEventListener("Event_OnUseNetworkSmoothingChanged", new Action<Dictionary<string, object>>(this.Event_OnUseNetworkSmoothingChanged));
		EventManager.RemoveEventListener("Event_OnNetworkSmoothingStrengthChanged", new Action<Dictionary<string, object>>(this.Event_OnNetworkSmoothingStrengthChanged));
		EventManager.RemoveEventListener("Event_OnMaxMatchmakingPingChanged", new Action<Dictionary<string, object>>(this.Event_OnMaxMatchmakingPingChanged));
		EventManager.RemoveEventListener("Event_OnFilterChatProfanityChanged", new Action<Dictionary<string, object>>(this.Event_OnFilterChatProfanityChanged));
		EventManager.RemoveEventListener("Event_OnUnitsChanged", new Action<Dictionary<string, object>>(this.Event_OnUnitsChanged));
		EventManager.RemoveEventListener("Event_OnShowGameUserInterfaceChanged", new Action<Dictionary<string, object>>(this.Event_OnShowGameUserInterfaceChanged));
		EventManager.RemoveEventListener("Event_OnUserInterfaceScaleChanged", new Action<Dictionary<string, object>>(this.Event_OnUserInterfaceScaleChanged));
		EventManager.RemoveEventListener("Event_OnChatOpacityChanged", new Action<Dictionary<string, object>>(this.Event_OnChatOpacityChanged));
		EventManager.RemoveEventListener("Event_OnChatScaleChanged", new Action<Dictionary<string, object>>(this.Event_OnChatScaleChanged));
		EventManager.RemoveEventListener("Event_OnMinimapOpacityChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapOpacityChanged));
		EventManager.RemoveEventListener("Event_OnMinimapBackgroundOpacityChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapBackgroundOpacityChanged));
		EventManager.RemoveEventListener("Event_OnMinimapHorizontalPositionChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapHorizontalPositionChanged));
		EventManager.RemoveEventListener("Event_OnMinimapVerticalPositionChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapVerticalPositionChanged));
		EventManager.RemoveEventListener("Event_OnMinimapScaleChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapScaleChanged));
		EventManager.RemoveEventListener("Event_OnGlobalStickSensitivityChanged", new Action<Dictionary<string, object>>(this.Event_OnGlobalStickSensitivityChanged));
		EventManager.RemoveEventListener("Event_OnHorizontalStickSensitivityChanged", new Action<Dictionary<string, object>>(this.Event_OnHorizontalStickSensitivityChanged));
		EventManager.RemoveEventListener("Event_OnVerticalStickSensitivityChanged", new Action<Dictionary<string, object>>(this.Event_OnVerticalStickSensitivityChanged));
		EventManager.RemoveEventListener("Event_OnLookSensitivityChanged", new Action<Dictionary<string, object>>(this.Event_OnLookSensitivityChanged));
		EventManager.RemoveEventListener("Event_OnKeyBindsLoaded", new Action<Dictionary<string, object>>(this.Event_OnKeyBindsLoaded));
		EventManager.RemoveEventListener("Event_OnKeyBindsSaved", new Action<Dictionary<string, object>>(this.Event_OnKeyBindsSaved));
		EventManager.RemoveEventListener("Event_OnGlobalVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnGlobalVolumeChanged));
		EventManager.RemoveEventListener("Event_OnAmbientVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnAmbientVolumeChanged));
		EventManager.RemoveEventListener("Event_OnGameVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnGameVolumeChanged));
		EventManager.RemoveEventListener("Event_OnVoiceVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnVoiceVolumeChanged));
		EventManager.RemoveEventListener("Event_OnUIVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnUIVolumeChanged));
		EventManager.RemoveEventListener("Event_OnFullScreenModeChanged", new Action<Dictionary<string, object>>(this.Event_OnFullScreenModeChanged));
		EventManager.RemoveEventListener("Event_OnDisplayIndexChanged", new Action<Dictionary<string, object>>(this.Event_OnDisplayIndexChanged));
		EventManager.RemoveEventListener("Event_OnResolutionIndexChanged", new Action<Dictionary<string, object>>(this.Event_OnResolutionIndexChanged));
		EventManager.RemoveEventListener("Event_OnVSyncChanged", new Action<Dictionary<string, object>>(this.Event_OnVSyncChanged));
		EventManager.RemoveEventListener("Event_OnFpsLimitChanged", new Action<Dictionary<string, object>>(this.Event_OnFpsLimitChanged));
		EventManager.RemoveEventListener("Event_OnFovChanged", new Action<Dictionary<string, object>>(this.Event_OnFovChanged));
		EventManager.RemoveEventListener("Event_OnQualityChanged", new Action<Dictionary<string, object>>(this.Event_OnQualityChanged));
		EventManager.RemoveEventListener("Event_OnMotionBlurChanged", new Action<Dictionary<string, object>>(this.Event_OnMotionBlurChanged));
		EventManager.RemoveEventListener("Event_OnIsDisplayChangeInProgressChanged", new Action<Dictionary<string, object>>(this.Event_OnIsDisplayChangeInProgressChanged));
		base.OnDestroy();
	}

	// Token: 0x06000D8A RID: 3466 RVA: 0x00040970 File Offset: 0x0003EB70
	private void Event_OnCameraAngleChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateCameraAngle(value);
	}

	// Token: 0x06000D8B RID: 3467 RVA: 0x0004099C File Offset: 0x0003EB9C
	private void Event_OnHandednessChanged(Dictionary<string, object> message)
	{
		string nameFromHandedness = Utils.GetNameFromHandedness((PlayerHandedness)message["value"]);
		this.uiSettings.UpdateHandedness(nameFromHandedness);
	}

	// Token: 0x06000D8C RID: 3468 RVA: 0x000409CC File Offset: 0x0003EBCC
	private void Event_OnShowPuckSilhouetteChanged(Dictionary<string, object> message)
	{
		bool value = (bool)message["value"];
		this.uiSettings.UpdateShowPuckSilhouette(value);
	}

	// Token: 0x06000D8D RID: 3469 RVA: 0x000409F8 File Offset: 0x0003EBF8
	private void Event_OnShowPuckOutlineChanged(Dictionary<string, object> message)
	{
		bool value = (bool)message["value"];
		this.uiSettings.UpdateShowPuckOutline(value);
	}

	// Token: 0x06000D8E RID: 3470 RVA: 0x00040A24 File Offset: 0x0003EC24
	private void Event_OnShowPuckElevationChanged(Dictionary<string, object> message)
	{
		bool value = (bool)message["value"];
		this.uiSettings.UpdateShowPuckElevation(value);
	}

	// Token: 0x06000D8F RID: 3471 RVA: 0x00040A50 File Offset: 0x0003EC50
	private void Event_OnShowPlayerUsernamesChanged(Dictionary<string, object> message)
	{
		bool value = (bool)message["value"];
		this.uiSettings.UpdateShowPlayerUsernames(value);
	}

	// Token: 0x06000D90 RID: 3472 RVA: 0x00040A7C File Offset: 0x0003EC7C
	private void Event_OnPlayerUsernamesFadeThresholdChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdatePlayerUsernamesFadeThreshold(value);
	}

	// Token: 0x06000D91 RID: 3473 RVA: 0x00040AA8 File Offset: 0x0003ECA8
	private void Event_OnUseNetworkSmoothingChanged(Dictionary<string, object> message)
	{
		bool value = (bool)message["value"];
		this.uiSettings.UpdateUseNetworkSmoothing(value);
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x00040AD4 File Offset: 0x0003ECD4
	private void Event_OnNetworkSmoothingStrengthChanged(Dictionary<string, object> message)
	{
		int value = (int)message["value"];
		this.uiSettings.UpdateNetworkSmoothingStrength(value);
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x00040B00 File Offset: 0x0003ED00
	private void Event_OnMaxMatchmakingPingChanged(Dictionary<string, object> message)
	{
		int value = (int)message["value"];
		this.uiSettings.UpdateMaxMatchmakingPing(value);
	}

	// Token: 0x06000D94 RID: 3476 RVA: 0x00040B2C File Offset: 0x0003ED2C
	private void Event_OnFilterChatProfanityChanged(Dictionary<string, object> message)
	{
		bool value = (bool)message["value"];
		this.uiSettings.UpdateFilterChatProfanity(value);
	}

	// Token: 0x06000D95 RID: 3477 RVA: 0x00040B58 File Offset: 0x0003ED58
	private void Event_OnUnitsChanged(Dictionary<string, object> message)
	{
		string nameFromUnits = Utils.GetNameFromUnits((Units)message["value"]);
		this.uiSettings.UpdateUnits(nameFromUnits);
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x00040B88 File Offset: 0x0003ED88
	private void Event_OnShowGameUserInterfaceChanged(Dictionary<string, object> message)
	{
		bool value = (bool)message["value"];
		this.uiSettings.UpdateShowGameUserInterface(value);
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x00040BB4 File Offset: 0x0003EDB4
	private void Event_OnUserInterfaceScaleChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateUserInterfaceScale(value);
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x00040BE0 File Offset: 0x0003EDE0
	private void Event_OnChatOpacityChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateChatOpacity(value);
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x00040C0C File Offset: 0x0003EE0C
	private void Event_OnChatScaleChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateChatScale(value);
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x00040C38 File Offset: 0x0003EE38
	private void Event_OnMinimapOpacityChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateMinimapOpacity(value);
	}

	// Token: 0x06000D9B RID: 3483 RVA: 0x00040C64 File Offset: 0x0003EE64
	private void Event_OnMinimapBackgroundOpacityChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateMinimapBackgroundOpacity(value);
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x00040C90 File Offset: 0x0003EE90
	private void Event_OnMinimapHorizontalPositionChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateMinimapHorizontalPosition(value);
	}

	// Token: 0x06000D9D RID: 3485 RVA: 0x00040CBC File Offset: 0x0003EEBC
	private void Event_OnMinimapVerticalPositionChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateMinimapVerticalPosition(value);
	}

	// Token: 0x06000D9E RID: 3486 RVA: 0x00040CE8 File Offset: 0x0003EEE8
	private void Event_OnMinimapScaleChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateMinimapScale(value);
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x00040D14 File Offset: 0x0003EF14
	private void Event_OnGlobalStickSensitivityChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateGlobalStickSensitivity(value);
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x00040D40 File Offset: 0x0003EF40
	private void Event_OnHorizontalStickSensitivityChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateHorizontalStickSensitivity(value);
	}

	// Token: 0x06000DA1 RID: 3489 RVA: 0x00040D6C File Offset: 0x0003EF6C
	private void Event_OnVerticalStickSensitivityChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateVerticalStickSensitivity(value);
	}

	// Token: 0x06000DA2 RID: 3490 RVA: 0x00040D98 File Offset: 0x0003EF98
	private void Event_OnLookSensitivityChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateLookSensitivity(value);
	}

	// Token: 0x06000DA3 RID: 3491 RVA: 0x00040DC4 File Offset: 0x0003EFC4
	private void Event_OnKeyBindsLoaded(Dictionary<string, object> message)
	{
		Dictionary<string, KeyBind> keyBinds = (Dictionary<string, KeyBind>)message["keyBinds"];
		this.uiSettings.UpdateKeyBindInputs(keyBinds);
	}

	// Token: 0x06000DA4 RID: 3492 RVA: 0x00040DF0 File Offset: 0x0003EFF0
	private void Event_OnKeyBindsSaved(Dictionary<string, object> message)
	{
		Dictionary<string, KeyBind> keyBinds = (Dictionary<string, KeyBind>)message["keyBinds"];
		this.uiSettings.UpdateKeyBindInputs(keyBinds);
	}

	// Token: 0x06000DA5 RID: 3493 RVA: 0x00040E1C File Offset: 0x0003F01C
	private void Event_OnGlobalVolumeChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateGlobalVolume(value);
	}

	// Token: 0x06000DA6 RID: 3494 RVA: 0x00040E48 File Offset: 0x0003F048
	private void Event_OnAmbientVolumeChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateAmbientVolume(value);
	}

	// Token: 0x06000DA7 RID: 3495 RVA: 0x00040E74 File Offset: 0x0003F074
	private void Event_OnGameVolumeChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateGameVolume(value);
	}

	// Token: 0x06000DA8 RID: 3496 RVA: 0x00040EA0 File Offset: 0x0003F0A0
	private void Event_OnVoiceVolumeChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateVoiceVolume(value);
	}

	// Token: 0x06000DA9 RID: 3497 RVA: 0x00040ECC File Offset: 0x0003F0CC
	private void Event_OnUIVolumeChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateUIVolume(value);
	}

	// Token: 0x06000DAA RID: 3498 RVA: 0x00040EF8 File Offset: 0x0003F0F8
	private void Event_OnFullScreenModeChanged(Dictionary<string, object> message)
	{
		string nameFromFullScreenMode = Utils.GetNameFromFullScreenMode((FullScreenMode)message["value"]);
		this.uiSettings.UpdateFullScreenMode(nameFromFullScreenMode);
	}

	// Token: 0x06000DAB RID: 3499 RVA: 0x00040F28 File Offset: 0x0003F128
	private void Event_OnDisplayIndexChanged(Dictionary<string, object> message)
	{
		string displayNameFromIndex = Utils.GetDisplayNameFromIndex((int)message["value"]);
		this.uiSettings.UpdateDisplay(displayNameFromIndex);
	}

	// Token: 0x06000DAC RID: 3500 RVA: 0x00040F58 File Offset: 0x0003F158
	private void Event_OnResolutionIndexChanged(Dictionary<string, object> message)
	{
		string resolutionNameFromIndex = Utils.GetResolutionNameFromIndex((int)message["value"]);
		this.uiSettings.UpdateResolution(resolutionNameFromIndex);
	}

	// Token: 0x06000DAD RID: 3501 RVA: 0x00040F88 File Offset: 0x0003F188
	private void Event_OnVSyncChanged(Dictionary<string, object> message)
	{
		bool value = (bool)message["value"];
		this.uiSettings.UpdateVSync(value);
	}

	// Token: 0x06000DAE RID: 3502 RVA: 0x00040FB4 File Offset: 0x0003F1B4
	private void Event_OnFpsLimitChanged(Dictionary<string, object> message)
	{
		int value = (int)message["value"];
		this.uiSettings.UpdateFpsLimit(value);
	}

	// Token: 0x06000DAF RID: 3503 RVA: 0x00040FE0 File Offset: 0x0003F1E0
	private void Event_OnFovChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.uiSettings.UpdateFov(value);
	}

	// Token: 0x06000DB0 RID: 3504 RVA: 0x0004100C File Offset: 0x0003F20C
	private void Event_OnQualityChanged(Dictionary<string, object> message)
	{
		string nameFromApplicationQuality = Utils.GetNameFromApplicationQuality((ApplicationQuality)message["value"]);
		this.uiSettings.UpdateQuality(nameFromApplicationQuality);
	}

	// Token: 0x06000DB1 RID: 3505 RVA: 0x0004103C File Offset: 0x0003F23C
	private void Event_OnMotionBlurChanged(Dictionary<string, object> message)
	{
		bool value = (bool)message["value"];
		this.uiSettings.UpdateMotionBlur(value);
	}

	// Token: 0x06000DB2 RID: 3506 RVA: 0x00041068 File Offset: 0x0003F268
	private void Event_OnIsDisplayChangeInProgressChanged(Dictionary<string, object> message)
	{
		if ((bool)message["isDisplayChangeInProgress"])
		{
			return;
		}
		List<string> resolutionNames = Utils.GetResolutionNames();
		this.uiSettings.UpdateResolutionChoices(resolutionNames);
	}

	// Token: 0x04000807 RID: 2055
	private UISettings uiSettings;
}
