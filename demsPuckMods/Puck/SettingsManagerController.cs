using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x020000A6 RID: 166
public class SettingsManagerController : MonoBehaviour
{
	// Token: 0x0600049C RID: 1180 RVA: 0x00009F81 File Offset: 0x00008181
	private void Awake()
	{
		this.settingsManager = base.GetComponent<SettingsManager>();
		this.settingsManager.LoadSettings();
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x0001EF64 File Offset: 0x0001D164
	private void Start()
	{
		MonoBehaviourSingleton<InputManager>.Instance.DebugAction.performed += this.OnDebugActionPerformed;
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsCameraAngleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsCameraAngleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsHandednessChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsHandednessChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsShowPuckSilhouetteChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsShowPuckSilhouetteChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsShowPuckOutlineChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsShowPuckOutlineChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsShowPuckElevationChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsShowPuckElevationChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsShowPlayerUsernamesChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsShowPlayerUsernamesChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsPlayerUsernamesFadeThresholdChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsPlayerUsernamesFadeThresholdChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsUseNetworkSmoothingChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsUseNetworkSmoothingChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsNetworkSmoothingStrengthChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsNetworkSmoothingStrengthChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsFilterChatProfanityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsFilterChatProfanityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsUnitsChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsUnitsChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsShowGameUserInterfaceChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsShowGameUserInterfaceChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsUserInterfaceScaleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsUserInterfaceScaleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsChatOpacityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsChatOpacityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsChatScaleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsChatScaleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsMinimapOpacityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsMinimapOpacityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsMinimapBackgroundOpacityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsMinimapBackgroundOpacityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsMinimapHorizontalPositionChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsMinimapHorizontalPositionChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsMinimapVerticalPositionChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsMinimapVerticalPositionChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsMinimapScaleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsMinimapScaleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsGlobalStickSensitivityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsGlobalStickSensitivityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsHorizontalStickSensitivityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsHorizontalStickSensitivityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsVerticalStickSensitivityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsVerticalStickSensitivityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsLookSensitivityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsLookSensitivityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsGlobalVolumeChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsGlobalVolumeChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsAmbientVolumeChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsAmbientVolumeChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsGameVolumeChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsGameVolumeChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsVoiceVolumeChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsVoiceVolumeChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsUIVolumeChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsUIVolumeChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsWindowModeChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsWindowModeChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsDisplayChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsDisplayChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsResolutionChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsResolutionChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsVSyncChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsVSyncChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsFpsLimitChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsFpsLimitChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsFovChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsFovChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsQualityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsQualityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsMotionBlurChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsMotionBlurChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceFlagChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceFlagChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceVisorChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceVisorChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceMustacheChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceMustacheChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceBeardChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceBeardChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceJerseyChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceJerseyChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceStickSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceStickSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceStickShaftTapeSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceStickShaftTapeSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceStickBladeTapeSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceStickBladeTapeSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnBaseCameraEnabled", new Action<Dictionary<string, object>>(this.Event_Client_OnBaseCameraEnabled));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceClickClose));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnOwnedItemIdsUpdated", new Action<Dictionary<string, object>>(this.Event_Client_OnOwnedItemIdsUpdated));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPopupClickOk", new Action<Dictionary<string, object>>(this.Event_Client_OnPopupClickOk));
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x0600049E RID: 1182 RVA: 0x00009F9A File Offset: 0x0000819A
	private IEnumerator LateStart()
	{
		yield return new WaitForEndOfFrame();
		this.settingsManager.ApplySettings();
		yield break;
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x0001F4C4 File Offset: 0x0001D6C4
	private void OnDestroy()
	{
		MonoBehaviourSingleton<InputManager>.Instance.DebugAction.performed -= this.OnDebugActionPerformed;
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsCameraAngleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsCameraAngleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsHandednessChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsHandednessChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsShowPuckSilhouetteChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsShowPuckSilhouetteChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsShowPuckOutlineChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsShowPuckOutlineChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsShowPuckElevationChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsShowPuckElevationChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsShowPlayerUsernamesChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsShowPlayerUsernamesChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsPlayerUsernamesFadeThresholdChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsPlayerUsernamesFadeThresholdChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsUseNetworkSmoothingChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsUseNetworkSmoothingChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsNetworkSmoothingStrengthChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsNetworkSmoothingStrengthChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsFilterChatProfanityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsFilterChatProfanityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsUnitsChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsUnitsChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsShowGameUserInterfaceChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsShowGameUserInterfaceChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsUserInterfaceScaleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsUserInterfaceScaleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsChatOpacityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsChatOpacityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsChatScaleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsChatScaleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsMinimapOpacityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsMinimapOpacityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsMinimapBackgroundOpacityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsMinimapBackgroundOpacityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsMinimapHorizontalPositionChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsMinimapHorizontalPositionChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsMinimapVerticalPositionChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsMinimapVerticalPositionChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsMinimapScaleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsMinimapScaleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsGlobalStickSensitivityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsGlobalStickSensitivityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsHorizontalStickSensitivityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsHorizontalStickSensitivityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsVerticalStickSensitivityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsVerticalStickSensitivityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsLookSensitivityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsLookSensitivityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsGlobalVolumeChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsGlobalVolumeChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsAmbientVolumeChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsAmbientVolumeChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsGameVolumeChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsGameVolumeChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsVoiceVolumeChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsVoiceVolumeChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsUIVolumeChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsUIVolumeChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsWindowModeChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsWindowModeChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsDisplayChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsDisplayChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsResolutionChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsResolutionChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsVSyncChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsVSyncChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsFpsLimitChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsFpsLimitChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsFovChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsFovChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsQualityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsQualityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsMotionBlurChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsMotionBlurChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceFlagChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceFlagChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceVisorChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceVisorChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceMustacheChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceMustacheChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceBeardChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceBeardChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceJerseyChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceJerseyChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceStickSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceStickSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceStickShaftTapeSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceStickShaftTapeSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceStickBladeTapeSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceStickBladeTapeSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnBaseCameraEnabled", new Action<Dictionary<string, object>>(this.Event_Client_OnBaseCameraEnabled));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceClickClose));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnOwnedItemIdsUpdated", new Action<Dictionary<string, object>>(this.Event_Client_OnOwnedItemIdsUpdated));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPopupClickOk", new Action<Dictionary<string, object>>(this.Event_Client_OnPopupClickOk));
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x00009FA9 File Offset: 0x000081A9
	private void OnApplicationQuit()
	{
		this.settingsManager.SaveSettings();
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x00009FB6 File Offset: 0x000081B6
	private void OnDebugActionPerformed(InputAction.CallbackContext context)
	{
		this.settingsManager.UpdateDebug(this.settingsManager.Debug == 0);
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x0001FA18 File Offset: 0x0001DC18
	private void Event_Client_OnSettingsCameraAngleChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateCameraAngle(value);
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x0001FA44 File Offset: 0x0001DC44
	private void Event_Client_OnSettingsHandednessChanged(Dictionary<string, object> message)
	{
		string value = (string)message["value"];
		this.settingsManager.UpdateHandedness(value);
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x0001FA70 File Offset: 0x0001DC70
	private void Event_Client_OnSettingsShowPuckSilhouetteChanged(Dictionary<string, object> message)
	{
		bool value = (bool)message["value"];
		this.settingsManager.UpdateShowPuckSilhouette(value);
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x0001FA9C File Offset: 0x0001DC9C
	private void Event_Client_OnSettingsShowPuckOutlineChanged(Dictionary<string, object> message)
	{
		bool value = (bool)message["value"];
		this.settingsManager.UpdateShowPuckOutline(value);
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x0001FAC8 File Offset: 0x0001DCC8
	private void Event_Client_OnSettingsShowPuckElevationChanged(Dictionary<string, object> message)
	{
		bool value = (bool)message["value"];
		this.settingsManager.UpdateShowPuckElevation(value);
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x0001FAF4 File Offset: 0x0001DCF4
	private void Event_Client_OnSettingsShowPlayerUsernamesChanged(Dictionary<string, object> message)
	{
		bool value = (bool)message["value"];
		this.settingsManager.UpdateShowPlayerUsernames(value);
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x0001FB20 File Offset: 0x0001DD20
	private void Event_Client_OnSettingsPlayerUsernamesFadeThresholdChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdatePlayerUsernamesFadeThreshold(value);
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x0001FB4C File Offset: 0x0001DD4C
	private void Event_Client_OnSettingsUseNetworkSmoothingChanged(Dictionary<string, object> message)
	{
		bool value = (bool)message["value"];
		this.settingsManager.UpdateUseNetworkSmoothing(value);
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x0001FB78 File Offset: 0x0001DD78
	private void Event_Client_OnSettingsNetworkSmoothingStrengthChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateNetworkSmoothingStrength(value);
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x0001FBA4 File Offset: 0x0001DDA4
	private void Event_Client_OnSettingsFilterChatProfanityChanged(Dictionary<string, object> message)
	{
		bool value = (bool)message["value"];
		this.settingsManager.UpdateFilterChatProfanity(value);
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x0001FBD0 File Offset: 0x0001DDD0
	private void Event_Client_OnSettingsUnitsChanged(Dictionary<string, object> message)
	{
		string value = (string)message["value"];
		this.settingsManager.UpdateUnits(value);
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x0001FBFC File Offset: 0x0001DDFC
	private void Event_Client_OnSettingsShowGameUserInterfaceChanged(Dictionary<string, object> message)
	{
		bool value = (bool)message["value"];
		this.settingsManager.UpdateShowGameUserInterface(value);
	}

	// Token: 0x060004AE RID: 1198 RVA: 0x0001FC28 File Offset: 0x0001DE28
	private void Event_Client_OnSettingsUserInterfaceScaleChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateUserInterfaceScale(value);
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x0001FC54 File Offset: 0x0001DE54
	private void Event_Client_OnSettingsChatOpacityChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateChatOpacity(value);
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x0001FC80 File Offset: 0x0001DE80
	private void Event_Client_OnSettingsChatScaleChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateChatScale(value);
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x0001FCAC File Offset: 0x0001DEAC
	private void Event_Client_OnSettingsMinimapOpacityChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateMinimapOpacity(value);
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x0001FCD8 File Offset: 0x0001DED8
	private void Event_Client_OnSettingsMinimapBackgroundOpacityChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateMinimapBackgroundOpacity(value);
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x0001FD04 File Offset: 0x0001DF04
	private void Event_Client_OnSettingsMinimapHorizontalPositionChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateMinimapHorizontalPosition(value);
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x0001FD30 File Offset: 0x0001DF30
	private void Event_Client_OnSettingsMinimapVerticalPositionChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateMinimapVerticalPosition(value);
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x0001FD5C File Offset: 0x0001DF5C
	private void Event_Client_OnSettingsMinimapScaleChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateMinimapScale(value);
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x0001FD88 File Offset: 0x0001DF88
	private void Event_Client_OnSettingsGlobalStickSensitivityChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateGlobalStickSensitivity(value);
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x0001FDB4 File Offset: 0x0001DFB4
	private void Event_Client_OnSettingsHorizontalStickSensitivityChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateHorizontalStickSensitivity(value);
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x0001FDE0 File Offset: 0x0001DFE0
	private void Event_Client_OnSettingsVerticalStickSensitivityChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateVerticalStickSensitivity(value);
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x0001FE0C File Offset: 0x0001E00C
	private void Event_Client_OnSettingsLookSensitivityChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateLookSensitivity(value);
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x0001FE38 File Offset: 0x0001E038
	private void Event_Client_OnSettingsGlobalVolumeChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateGlobalVolume(value);
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x0001FE64 File Offset: 0x0001E064
	private void Event_Client_OnSettingsAmbientVolumeChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateAmbientVolume(value);
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x0001FE90 File Offset: 0x0001E090
	private void Event_Client_OnSettingsGameVolumeChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateGameVolume(value);
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x0001FEBC File Offset: 0x0001E0BC
	private void Event_Client_OnSettingsVoiceVolumeChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateVoiceVolume(value);
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x0001FEE8 File Offset: 0x0001E0E8
	private void Event_Client_OnSettingsUIVolumeChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateUIVolume(value);
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x0001FF14 File Offset: 0x0001E114
	private void Event_Client_OnSettingsWindowModeChanged(Dictionary<string, object> message)
	{
		string value = (string)message["value"];
		this.settingsManager.UpdateWindowMode(value);
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x0001FF40 File Offset: 0x0001E140
	private void Event_Client_OnSettingsDisplayChanged(Dictionary<string, object> message)
	{
		string a = (string)message["value"];
		int num = 0;
		while (num < Display.displays.Length && !(a == Utils.GetDisplayStringFromIndex(num)))
		{
			num++;
		}
		this.settingsManager.UpdateDisplayIndex((num == Display.displays.Length) ? 0 : num, false);
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x0001FF98 File Offset: 0x0001E198
	private void Event_Client_OnSettingsResolutionChanged(Dictionary<string, object> message)
	{
		string a = (string)message["value"];
		int num = 0;
		while (num < Screen.resolutions.Length && !(a == Utils.GetResolutionStringFromIndex(num)))
		{
			num++;
		}
		this.settingsManager.UpdateResolutionIndex((num == Screen.resolutions.Length) ? 0 : num);
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x0001FFF0 File Offset: 0x0001E1F0
	private void Event_Client_OnSettingsVSyncChanged(Dictionary<string, object> message)
	{
		bool value = (bool)message["value"];
		this.settingsManager.UpdateVSync(value);
	}

	// Token: 0x060004C3 RID: 1219 RVA: 0x0002001C File Offset: 0x0001E21C
	private void Event_Client_OnSettingsFpsLimitChanged(Dictionary<string, object> message)
	{
		int value = (int)message["value"];
		this.settingsManager.UpdateFpsLimit(value);
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x00020048 File Offset: 0x0001E248
	private void Event_Client_OnSettingsFovChanged(Dictionary<string, object> message)
	{
		float value = (float)message["value"];
		this.settingsManager.UpdateFov(value);
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x00020074 File Offset: 0x0001E274
	private void Event_Client_OnSettingsQualityChanged(Dictionary<string, object> message)
	{
		string value = (string)message["value"];
		this.settingsManager.UpdateQuality(value);
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x000200A0 File Offset: 0x0001E2A0
	private void Event_Client_OnSettingsMotionBlurChanged(Dictionary<string, object> message)
	{
		bool value = (bool)message["value"];
		this.settingsManager.UpdateMotionBlur(value);
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x000200CC File Offset: 0x0001E2CC
	private void Event_Client_OnAppearanceFlagChanged(Dictionary<string, object> message)
	{
		string value = (string)message["value"];
		if (!(bool)message["isPreview"])
		{
			this.settingsManager.UpdateCountry(value);
		}
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x00020108 File Offset: 0x0001E308
	private void Event_Client_OnAppearanceVisorChanged(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["team"];
		PlayerRole role = (PlayerRole)message["role"];
		string value = (string)message["value"];
		if (!(bool)message["isPreview"])
		{
			this.settingsManager.UpdateVisorSkin(team, role, value);
		}
	}

	// Token: 0x060004C9 RID: 1225 RVA: 0x00020168 File Offset: 0x0001E368
	private void Event_Client_OnAppearanceMustacheChanged(Dictionary<string, object> message)
	{
		string value = (string)message["value"];
		if (!(bool)message["isPreview"])
		{
			this.settingsManager.UpdateMustache(value);
		}
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x000201A4 File Offset: 0x0001E3A4
	private void Event_Client_OnAppearanceBeardChanged(Dictionary<string, object> message)
	{
		string value = (string)message["value"];
		if (!(bool)message["isPreview"])
		{
			this.settingsManager.UpdateBeard(value);
		}
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x000201E0 File Offset: 0x0001E3E0
	private void Event_Client_OnAppearanceJerseyChanged(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["team"];
		PlayerRole role = (PlayerRole)message["role"];
		string value = (string)message["value"];
		if (!(bool)message["isPreview"])
		{
			this.settingsManager.UpdateJerseySkin(team, role, value);
		}
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x00020240 File Offset: 0x0001E440
	private void Event_Client_OnAppearanceStickSkinChanged(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["team"];
		PlayerRole role = (PlayerRole)message["role"];
		string value = (string)message["value"];
		if (!(bool)message["isPreview"])
		{
			this.settingsManager.UpdateStickSkin(team, role, value);
		}
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x000202A0 File Offset: 0x0001E4A0
	private void Event_Client_OnAppearanceStickShaftTapeSkinChanged(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["team"];
		PlayerRole role = (PlayerRole)message["role"];
		string value = (string)message["value"];
		if (!(bool)message["isPreview"])
		{
			this.settingsManager.UpdateStickShaftSkin(team, role, value);
		}
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x00020300 File Offset: 0x0001E500
	private void Event_Client_OnAppearanceStickBladeTapeSkinChanged(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["team"];
		PlayerRole role = (PlayerRole)message["role"];
		string value = (string)message["value"];
		if (!(bool)message["isPreview"])
		{
			this.settingsManager.UpdateStickBladeSkin(team, role, value);
		}
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x00009FD4 File Offset: 0x000081D4
	private void Event_Client_OnBaseCameraEnabled(Dictionary<string, object> message)
	{
		((BaseCamera)message["baseCamera"]).SetFieldOfView(this.settingsManager.Fov);
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x00009FF6 File Offset: 0x000081F6
	private void Event_Client_OnAppearanceClickClose(Dictionary<string, object> message)
	{
		this.settingsManager.ApplySettings();
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x0000A003 File Offset: 0x00008203
	private bool IsItemOwned(string item, List<string> ownedItems, List<string> purchaseableItems)
	{
		return ownedItems.Contains("all") || !purchaseableItems.Contains(item) || ownedItems.Contains(item);
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x00020360 File Offset: 0x0001E560
	private void ValidateStickSkinOwnership(List<string> ownedItems, List<string> purchaseableItems)
	{
		if (!this.IsItemOwned(this.settingsManager.StickAttackerBlueSkin, ownedItems, purchaseableItems))
		{
			this.settingsManager.UpdateStickSkin(PlayerTeam.Blue, PlayerRole.Attacker, "beta_attacker");
		}
		if (!this.IsItemOwned(this.settingsManager.StickAttackerRedSkin, ownedItems, purchaseableItems))
		{
			this.settingsManager.UpdateStickSkin(PlayerTeam.Red, PlayerRole.Attacker, "beta_attacker");
		}
		if (!this.IsItemOwned(this.settingsManager.StickGoalieBlueSkin, ownedItems, purchaseableItems))
		{
			this.settingsManager.UpdateStickSkin(PlayerTeam.Blue, PlayerRole.Goalie, "beta_goalie");
		}
		if (!this.IsItemOwned(this.settingsManager.StickGoalieRedSkin, ownedItems, purchaseableItems))
		{
			this.settingsManager.UpdateStickSkin(PlayerTeam.Red, PlayerRole.Goalie, "beta_goalie");
		}
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x0002040C File Offset: 0x0001E60C
	private void ValidateStickTapeSkinOwnership(List<string> ownedItems, List<string> purchaseableItems)
	{
		if (!this.IsItemOwned(this.settingsManager.StickShaftAttackerBlueTapeSkin, ownedItems, purchaseableItems))
		{
			this.settingsManager.UpdateStickShaftSkin(PlayerTeam.Blue, PlayerRole.Attacker, "gray_cloth");
		}
		if (!this.IsItemOwned(this.settingsManager.StickShaftAttackerRedTapeSkin, ownedItems, purchaseableItems))
		{
			this.settingsManager.UpdateStickShaftSkin(PlayerTeam.Red, PlayerRole.Attacker, "gray_cloth");
		}
		if (!this.IsItemOwned(this.settingsManager.StickShaftGoalieBlueTapeSkin, ownedItems, purchaseableItems))
		{
			this.settingsManager.UpdateStickShaftSkin(PlayerTeam.Blue, PlayerRole.Goalie, "gray_cloth");
		}
		if (!this.IsItemOwned(this.settingsManager.StickShaftGoalieRedTapeSkin, ownedItems, purchaseableItems))
		{
			this.settingsManager.UpdateStickShaftSkin(PlayerTeam.Red, PlayerRole.Goalie, "gray_cloth");
		}
		if (!this.IsItemOwned(this.settingsManager.StickBladeAttackerBlueTapeSkin, ownedItems, purchaseableItems))
		{
			this.settingsManager.UpdateStickBladeSkin(PlayerTeam.Blue, PlayerRole.Attacker, "gray_cloth");
		}
		if (!this.IsItemOwned(this.settingsManager.StickBladeAttackerRedTapeSkin, ownedItems, purchaseableItems))
		{
			this.settingsManager.UpdateStickBladeSkin(PlayerTeam.Red, PlayerRole.Attacker, "gray_cloth");
		}
		if (!this.IsItemOwned(this.settingsManager.StickBladeGoalieBlueTapeSkin, ownedItems, purchaseableItems))
		{
			this.settingsManager.UpdateStickBladeSkin(PlayerTeam.Blue, PlayerRole.Goalie, "gray_cloth");
		}
		if (!this.IsItemOwned(this.settingsManager.StickBladeGoalieRedTapeSkin, ownedItems, purchaseableItems))
		{
			this.settingsManager.UpdateStickBladeSkin(PlayerTeam.Red, PlayerRole.Goalie, "gray_cloth");
		}
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x00020554 File Offset: 0x0001E754
	private void Event_Client_OnOwnedItemIdsUpdated(Dictionary<string, object> message)
	{
		List<string> ownedItems = (List<string>)message["ownedItems"];
		List<string> purchaseableItems = (List<string>)message["purchaseableItems"];
		this.ValidateStickSkinOwnership(ownedItems, purchaseableItems);
		this.ValidateStickTapeSkinOwnership(ownedItems, purchaseableItems);
		this.settingsManager.ApplySettings();
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x000205A0 File Offset: 0x0001E7A0
	private void Event_Client_OnPopupClickOk(Dictionary<string, object> message)
	{
		if ((string)message["name"] != "settingsResetToDefault")
		{
			return;
		}
		PlayerPrefs.DeleteAll();
		this.settingsManager.LoadSettings();
		this.settingsManager.ApplySettings();
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsResetToDefault", null);
	}

	// Token: 0x040002C0 RID: 704
	private SettingsManager settingsManager;
}
