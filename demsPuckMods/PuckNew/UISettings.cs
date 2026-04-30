using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

// Token: 0x020001CD RID: 461
public class UISettings : UIView
{
	// Token: 0x06000D2E RID: 3374 RVA: 0x0003E59C File Offset: 0x0003C79C
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("SettingsView", null);
		this.settings = base.View.Query("Settings", null);
		this.closeIconButton = this.settings.Query("CloseIconButtonContainer", null).First().Query(null, null);
		this.closeIconButton.clicked += this.OnClickClose;
		this.cameraAngleSlider = this.settings.Query("CameraAngleSlider", null).First().Query(null, null);
		this.cameraAngleSlider.value = SettingsManager.CameraAngle;
		this.cameraAngleSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnCameraAngleChanged));
		this.handednessDropdown = this.settings.Query("HandednessDropdown", null).First().Query(null, null);
		this.handednessDropdown.value = Utils.GetNameFromHandedness(SettingsManager.Handedness);
		this.handednessDropdown.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnHandednessChanged));
		this.showPuckSilhouetteToggle = this.settings.Query("ShowPuckSilhouetteToggle", null).First().Query(null, null);
		this.showPuckSilhouetteToggle.value = SettingsManager.ShowPuckSilhouette;
		this.showPuckSilhouetteToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnShowPuckSilhouetteChanged));
		this.showPuckOutlineToggle = this.settings.Query("ShowPuckOutlineToggle", null).First().Query(null, null);
		this.showPuckOutlineToggle.value = SettingsManager.ShowPuckOutline;
		this.showPuckOutlineToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnShowPuckOutlineChanged));
		this.showPuckElevationToggle = this.settings.Query("ShowPuckElevationToggle", null).First().Query(null, null);
		this.showPuckElevationToggle.value = SettingsManager.ShowPuckElevation;
		this.showPuckElevationToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnShowPuckEleveationChanged));
		this.showPlayerUsernamesToggle = this.settings.Query("ShowPlayerUsernamesToggle", null).First().Query(null, null);
		this.showPlayerUsernamesToggle.value = SettingsManager.ShowPlayerUsernames;
		this.showPlayerUsernamesToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnShowPlayerUsernamesChanged));
		this.playerUsernamesFadeThresholdSlider = this.settings.Query("PlayerUsernamesFadeThresholdSlider", null).First().Query(null, null);
		this.playerUsernamesFadeThresholdSlider.value = SettingsManager.PlayerUsernamesFadeThreshold;
		this.playerUsernamesFadeThresholdSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnPlayerUsernamesFadeThresholdChanged));
		this.useNetworkSmoothingToggle = this.settings.Query("UseNetworkSmoothingToggle", null).First().Query(null, null);
		this.useNetworkSmoothingToggle.value = SettingsManager.UseNetworkSmoothing;
		this.useNetworkSmoothingToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnUseNetworkSmoothingChanged));
		this.networkSmoothingStrengthSliderInt = this.settings.Query("NetworkSmoothingStrengthSliderInt", null).First().Query(null, null);
		this.networkSmoothingStrengthSliderInt.value = SettingsManager.NetworkSmoothingStrength;
		this.networkSmoothingStrengthSliderInt.RegisterValueChangedCallback(new EventCallback<ChangeEvent<int>>(this.OnNetworkSmoothingStrengthChanged));
		this.maxMatchmakingPingSliderInt = this.settings.Query("MaxMatchmakingPingSliderInt", null).First().Query(null, null);
		this.maxMatchmakingPingSliderInt.value = SettingsManager.MaxMatchmakingPing;
		this.maxMatchmakingPingSliderInt.RegisterValueChangedCallback(new EventCallback<ChangeEvent<int>>(this.OnMaxMatchmakingPingChanged));
		this.filterChatProfanityToggle = this.settings.Query("FilterChatProfanityToggle", null).First().Query(null, null);
		this.filterChatProfanityToggle.value = SettingsManager.FilterChatProfanity;
		this.filterChatProfanityToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnFilterChatProfanityChanged));
		this.unitsDropdown = this.settings.Query("UnitsDropdown", null).First().Query(null, null);
		this.unitsDropdown.value = Utils.GetNameFromUnits(SettingsManager.Units);
		this.unitsDropdown.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnUnitsChanged));
		this.showGameUserInterfaceToggle = this.settings.Query("ShowGameUserInterfaceToggle", null).First().Query(null, null);
		this.showGameUserInterfaceToggle.value = SettingsManager.ShowGameUserInterface;
		this.showGameUserInterfaceToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnShowGameUserInterfaceChanged));
		this.userInterfaceScaleSlider = this.settings.Query("UserInterfaceScaleSlider", null).First().Query(null, null);
		this.userInterfaceScaleSlider.value = SettingsManager.UserInterfaceScale;
		this.userInterfaceScaleSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnUserInterfaceScaleChanged));
		this.chatOpacitySlider = this.settings.Query("ChatOpacitySlider", null).First().Query(null, null);
		this.chatOpacitySlider.value = SettingsManager.ChatOpacity;
		this.chatOpacitySlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnChatOpacityChanged));
		this.chatScaleSlider = this.settings.Query("ChatScaleSlider", null).First().Query(null, null);
		this.chatScaleSlider.value = SettingsManager.ChatScale;
		this.chatScaleSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnChatScaleChanged));
		this.minimapOpacitySlider = this.settings.Query("MinimapOpacitySlider", null).First().Query(null, null);
		this.minimapOpacitySlider.value = SettingsManager.MinimapOpacity;
		this.minimapOpacitySlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnMinimapOpacityChanged));
		this.minimapBackgroundOpacitySlider = this.settings.Query("MinimapBackgroundOpacitySlider", null).First().Query(null, null);
		this.minimapBackgroundOpacitySlider.value = SettingsManager.MinimapBackgroundOpacity;
		this.minimapBackgroundOpacitySlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnMinimapBackgroundOpacityChanged));
		this.minimapHorizontalPositionSlider = this.settings.Query("MinimapHorizontalPositionSlider", null).First().Query(null, null);
		this.minimapHorizontalPositionSlider.value = SettingsManager.MinimapHorizontalPosition;
		this.minimapHorizontalPositionSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnMinimapHorizontalPositionChanged));
		this.minimapVerticalPositionSlider = this.settings.Query("MinimapVerticalPositionSlider", null).First().Query(null, null);
		this.minimapVerticalPositionSlider.value = SettingsManager.MinimapVerticalPosition;
		this.minimapVerticalPositionSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnMinimapVerticalPositionChanged));
		this.minimapScaleSlider = this.settings.Query("MinimapScaleSlider", null).First().Query(null, null);
		this.minimapScaleSlider.value = SettingsManager.MinimapScale;
		this.minimapScaleSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnMinimapScaleChanged));
		this.globalStickSensitivitySlider = this.settings.Query("GlobalStickSensitivitySlider", null).First().Query(null, null);
		this.globalStickSensitivitySlider.value = SettingsManager.GlobalStickSensitivity;
		this.globalStickSensitivitySlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnGlobalStickSensitivityChanged));
		this.horizontalStickSensitivitySlider = this.settings.Query("HorizontalStickSensitivitySlider", null).First().Query(null, null);
		this.horizontalStickSensitivitySlider.value = SettingsManager.HorizontalStickSensitivity;
		this.horizontalStickSensitivitySlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnHorizontalStickSensitivityChanged));
		this.verticalStickSensitivitySlider = this.settings.Query("VerticalStickSensitivitySlider", null).First().Query(null, null);
		this.verticalStickSensitivitySlider.value = SettingsManager.VerticalStickSensitivity;
		this.verticalStickSensitivitySlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnVerticalStickSensitivityChanged));
		this.lookSensitivitySlider = this.settings.Query("LookSensitivitySlider", null).First().Query(null, null);
		this.lookSensitivitySlider.value = SettingsManager.LookSensitivity;
		this.lookSensitivitySlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnLookSensitivityChanged));
		this.actionNameKeyBindFieldMap = new Dictionary<string, KeyBindField>
		{
			{
				"Move Forward",
				this.settings.Query("MoveForwardKeyBindInput", null).First().Query(null, null)
			},
			{
				"Move Backward",
				this.settings.Query("MoveBackwardKeyBindInput", null).First().Query(null, null)
			},
			{
				"Turn Left",
				this.settings.Query("TurnLeftKeyBindInput", null).First().Query(null, null)
			},
			{
				"Turn Right",
				this.settings.Query("TurnRightKeyBindInput", null).First().Query(null, null)
			},
			{
				"Blade Angle Up",
				this.settings.Query("BladeAngleUpKeyBindInput", null).First().Query(null, null)
			},
			{
				"Blade Angle Down",
				this.settings.Query("BladeAngleDownKeyBindInput", null).First().Query(null, null)
			},
			{
				"Slide",
				this.settings.Query("SlideKeyBindInput", null).First().Query(null, null)
			},
			{
				"Sprint",
				this.settings.Query("SprintKeyBindInput", null).First().Query(null, null)
			},
			{
				"Track",
				this.settings.Query("TrackKeyBindInput", null).First().Query(null, null)
			},
			{
				"Look",
				this.settings.Query("LookKeyBindInput", null).First().Query(null, null)
			},
			{
				"Jump",
				this.settings.Query("JumpKeyBindInput", null).First().Query(null, null)
			},
			{
				"Stop",
				this.settings.Query("StopKeyBindInput", null).First().Query(null, null)
			},
			{
				"Twist Left",
				this.settings.Query("TwistLeftKeyBindInput", null).First().Query(null, null)
			},
			{
				"Twist Right",
				this.settings.Query("TwistRightKeyBindInput", null).First().Query(null, null)
			},
			{
				"Dash Left",
				this.settings.Query("DashLeftKeyBindInput", null).First().Query(null, null)
			},
			{
				"Dash Right",
				this.settings.Query("DashRightKeyBindInput", null).First().Query(null, null)
			},
			{
				"Extend Left",
				this.settings.Query("ExtendLeftKeyBindInput", null).First().Query(null, null)
			},
			{
				"Extend Right",
				this.settings.Query("ExtendRightKeyBindInput", null).First().Query(null, null)
			},
			{
				"Lateral Left",
				this.settings.Query("LateralLeftKeyBindInput", null).First().Query(null, null)
			},
			{
				"Lateral Right",
				this.settings.Query("LateralRightKeyBindInput", null).First().Query(null, null)
			},
			{
				"Talk",
				this.settings.Query("TalkKeyBindInput", null).First().Query(null, null)
			},
			{
				"All Chat",
				this.settings.Query("AllChatKeyBindInput", null).First().Query(null, null)
			},
			{
				"Team Chat",
				this.settings.Query("TeamChatKeyBindInput", null).First().Query(null, null)
			},
			{
				"Position Select",
				this.settings.Query("PositionSelectKeyBindInput", null).First().Query(null, null)
			},
			{
				"Scoreboard",
				this.settings.Query("ScoreboardKeyBindInput", null).First().Query(null, null)
			}
		};
		this.UpdateKeyBindInputs(InputManager.KeyBinds);
		foreach (KeyValuePair<string, KeyBindField> keyValuePair in this.actionNameKeyBindFieldMap)
		{
			string actionName = keyValuePair.Key;
			KeyBindField value = keyValuePair.Value;
			value.Click = delegate()
			{
				this.OnKeyBindInputClicked(actionName);
			};
			value.InteractionChange = delegate(KeyBindInteraction interaction)
			{
				this.OnKeyBindInputInteractionChanged(actionName, interaction);
			};
		}
		this.globalVolumeSlider = this.settings.Query("GlobalVolumeSlider", null).First().Query(null, null);
		this.globalVolumeSlider.value = SettingsManager.GlobalVolume;
		this.globalVolumeSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnGlobalVolumeChanged));
		this.ambientVolumeSlider = this.settings.Query("AmbientVolumeSlider", null).First().Query(null, null);
		this.ambientVolumeSlider.value = SettingsManager.AmbientVolume;
		this.ambientVolumeSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnAmbientVolumeChanged));
		this.gameVolumeSlider = this.settings.Query("GameVolumeSlider", null).First().Query(null, null);
		this.gameVolumeSlider.value = SettingsManager.GameVolume;
		this.gameVolumeSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnGameVolumeChanged));
		this.voiceVolumeSlider = this.settings.Query("VoiceVolumeSlider", null).First().Query(null, null);
		this.voiceVolumeSlider.value = SettingsManager.VoiceVolume;
		this.voiceVolumeSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnVoiceVolumeChanged));
		this.uiVolumeSlider = this.settings.Query("UIVolumeSlider", null).First().Query(null, null);
		this.uiVolumeSlider.value = SettingsManager.UIVolume;
		this.uiVolumeSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnUIVolumeChanged));
		this.fullScreenModeDropdown = this.settings.Query("FullScreenModeDropdown", null).First().Query(null, null);
		this.fullScreenModeDropdown.choices = Utils.GetFullScreenModeNames();
		this.fullScreenModeDropdown.value = Utils.GetNameFromFullScreenMode(SettingsManager.FullScreenMode);
		this.fullScreenModeDropdown.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnFullScreenModeChanged));
		this.displayDropdown = this.settings.Query("DisplayDropdown", null).First().Query(null, null);
		this.displayDropdown.choices = Utils.GetDisplayNames();
		this.displayDropdown.value = Utils.GetDisplayNameFromIndex(SettingsManager.DisplayIndex);
		this.displayDropdown.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnDisplayChanged));
		this.resolutionDropdown = this.settings.Query("ResolutionDropdown", null).First().Query(null, null);
		this.resolutionDropdown.choices = Utils.GetResolutionNames();
		this.resolutionDropdown.value = Utils.GetResolutionNameFromIndex(SettingsManager.ResolutionIndex);
		this.resolutionDropdown.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnResolutionChanged));
		this.vSyncToggle = this.settings.Query("VSyncToggle", null).First().Query(null, null);
		this.vSyncToggle.value = SettingsManager.VSync;
		this.vSyncToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnVSyncChanged));
		this.fpsLimitSlider = this.settings.Query("FPSLimitSlider", null).First().Query(null, null);
		this.fpsLimitSlider.value = (float)SettingsManager.FpsLimit;
		this.fpsLimitSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnFpsLimitChanged));
		this.fovSlider = this.settings.Query("FOVSlider", null).First().Query(null, null);
		this.fovSlider.value = SettingsManager.Fov;
		this.fovSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnFovChanged));
		this.qualityDropdown = this.settings.Query("QualityDropdown", null).First().Query(null, null);
		this.qualityDropdown.choices = Utils.GetApplicationQualityNames();
		this.qualityDropdown.value = Utils.GetNameFromApplicationQuality(SettingsManager.Quality);
		this.qualityDropdown.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnQualityChanged));
		this.motionBlurToggle = this.settings.Query("MotionBlurToggle", null).First().Query(null, null);
		this.motionBlurToggle.value = SettingsManager.MotionBlur;
		this.motionBlurToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnMotionBlurChanged));
		this.resetToDefaultButton = this.settings.Query("ResetToDefaultButton", null);
		this.resetToDefaultButton.clicked += this.OnClickResetToDefault;
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x0003F850 File Offset: 0x0003DA50
	private void OnClickClose()
	{
		EventManager.TriggerEvent("Event_OnSettingsClickClose", null);
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x0003F85D File Offset: 0x0003DA5D
	private void OnClickResetToDefault()
	{
		EventManager.TriggerEvent("Event_OnSettingsClickResetToDefault", null);
	}

	// Token: 0x06000D31 RID: 3377 RVA: 0x0003F86C File Offset: 0x0003DA6C
	public void UpdateKeyBindInputs(Dictionary<string, KeyBind> keyBinds)
	{
		foreach (KeyValuePair<string, KeyBind> keyValuePair in keyBinds)
		{
			string key = keyValuePair.Key;
			KeyBind value = keyValuePair.Value;
			if (this.actionNameKeyBindFieldMap.ContainsKey(key))
			{
				KeyBindField keyBindField = this.actionNameKeyBindFieldMap[key];
				if (value.IsComposite)
				{
					string text = null;
					if (!string.IsNullOrEmpty(value.ModifierPath))
					{
						text = text + value.InputAction.GetBindingDisplayString(1, InputBinding.DisplayStringOptions.DontIncludeInteractions).ToUpper() + "+";
					}
					text += value.InputAction.GetBindingDisplayString(2, InputBinding.DisplayStringOptions.DontIncludeInteractions).ToUpper();
					keyBindField.Path = text;
				}
				else
				{
					keyBindField.Path = value.InputAction.GetBindingDisplayString(0, InputBinding.DisplayStringOptions.DontIncludeInteractions).ToUpper();
				}
				KeyBindInteraction keyBindInteractionFromInteraction = Utils.GetKeyBindInteractionFromInteraction(value.Interactions, keyBindField.InteractionType);
				keyBindField.Interaction = keyBindInteractionFromInteraction;
			}
		}
	}

	// Token: 0x06000D32 RID: 3378 RVA: 0x0003F97C File Offset: 0x0003DB7C
	private void OnCameraAngleChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsCameraAngleChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x0003F9A3 File Offset: 0x0003DBA3
	public void UpdateCameraAngle(float value)
	{
		this.cameraAngleSlider.value = value;
	}

	// Token: 0x06000D34 RID: 3380 RVA: 0x0003F9B1 File Offset: 0x0003DBB1
	private void OnHandednessChanged(ChangeEvent<string> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsHandednessChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D35 RID: 3381 RVA: 0x0003F9D3 File Offset: 0x0003DBD3
	public void UpdateHandedness(string value)
	{
		this.handednessDropdown.value = value;
	}

	// Token: 0x06000D36 RID: 3382 RVA: 0x0003F9E1 File Offset: 0x0003DBE1
	private void OnShowPuckSilhouetteChanged(ChangeEvent<bool> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsShowPuckSilhouetteChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x0003FA08 File Offset: 0x0003DC08
	public void UpdateShowPuckSilhouette(bool value)
	{
		this.showPuckSilhouetteToggle.value = value;
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x0003FA16 File Offset: 0x0003DC16
	private void OnShowPuckOutlineChanged(ChangeEvent<bool> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsShowPuckOutlineChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x0003FA3D File Offset: 0x0003DC3D
	public void UpdateShowPuckOutline(bool value)
	{
		this.showPuckOutlineToggle.value = value;
	}

	// Token: 0x06000D3A RID: 3386 RVA: 0x0003FA4B File Offset: 0x0003DC4B
	private void OnShowPuckEleveationChanged(ChangeEvent<bool> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsShowPuckElevationChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D3B RID: 3387 RVA: 0x0003FA72 File Offset: 0x0003DC72
	public void UpdateShowPuckElevation(bool value)
	{
		this.showPuckElevationToggle.value = value;
	}

	// Token: 0x06000D3C RID: 3388 RVA: 0x0003FA80 File Offset: 0x0003DC80
	private void OnShowPlayerUsernamesChanged(ChangeEvent<bool> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsShowPlayerUsernamesChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D3D RID: 3389 RVA: 0x0003FAA7 File Offset: 0x0003DCA7
	public void UpdateShowPlayerUsernames(bool value)
	{
		this.showPlayerUsernamesToggle.value = value;
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x0003FAB5 File Offset: 0x0003DCB5
	private void OnPlayerUsernamesFadeThresholdChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsPlayerUsernamesFadeThresholdChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D3F RID: 3391 RVA: 0x0003FADC File Offset: 0x0003DCDC
	public void UpdatePlayerUsernamesFadeThreshold(float value)
	{
		this.playerUsernamesFadeThresholdSlider.value = value;
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x0003FAEA File Offset: 0x0003DCEA
	private void OnUseNetworkSmoothingChanged(ChangeEvent<bool> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsUseNetworkSmoothingChanged", new Dictionary<string, object>
		{
			{
				"value",
				this.useNetworkSmoothingToggle.value
			}
		});
	}

	// Token: 0x06000D41 RID: 3393 RVA: 0x0003FB16 File Offset: 0x0003DD16
	public void UpdateUseNetworkSmoothing(bool value)
	{
		this.useNetworkSmoothingToggle.value = value;
	}

	// Token: 0x06000D42 RID: 3394 RVA: 0x0003FB24 File Offset: 0x0003DD24
	private void OnNetworkSmoothingStrengthChanged(ChangeEvent<int> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsNetworkSmoothingStrengthChanged", new Dictionary<string, object>
		{
			{
				"value",
				this.networkSmoothingStrengthSliderInt.value
			}
		});
	}

	// Token: 0x06000D43 RID: 3395 RVA: 0x0003FB50 File Offset: 0x0003DD50
	public void UpdateNetworkSmoothingStrength(int value)
	{
		this.networkSmoothingStrengthSliderInt.value = value;
	}

	// Token: 0x06000D44 RID: 3396 RVA: 0x0003FB5E File Offset: 0x0003DD5E
	private void OnMaxMatchmakingPingChanged(ChangeEvent<int> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsMaxMatchmakingPingChanged", new Dictionary<string, object>
		{
			{
				"value",
				this.maxMatchmakingPingSliderInt.value
			}
		});
	}

	// Token: 0x06000D45 RID: 3397 RVA: 0x0003FB8A File Offset: 0x0003DD8A
	public void UpdateMaxMatchmakingPing(int value)
	{
		this.maxMatchmakingPingSliderInt.value = value;
	}

	// Token: 0x06000D46 RID: 3398 RVA: 0x0003FB98 File Offset: 0x0003DD98
	private void OnFilterChatProfanityChanged(ChangeEvent<bool> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsFilterChatProfanityChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D47 RID: 3399 RVA: 0x0003FBBF File Offset: 0x0003DDBF
	public void UpdateFilterChatProfanity(bool value)
	{
		this.filterChatProfanityToggle.value = value;
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x0003FBCD File Offset: 0x0003DDCD
	private void OnUnitsChanged(ChangeEvent<string> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsUnitsChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x0003FBEF File Offset: 0x0003DDEF
	public void UpdateUnits(string value)
	{
		this.unitsDropdown.value = value;
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x0003FBFD File Offset: 0x0003DDFD
	private void OnShowGameUserInterfaceChanged(ChangeEvent<bool> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsShowGameUserInterfaceChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D4B RID: 3403 RVA: 0x0003FC24 File Offset: 0x0003DE24
	public void UpdateShowGameUserInterface(bool value)
	{
		this.showGameUserInterfaceToggle.value = value;
	}

	// Token: 0x06000D4C RID: 3404 RVA: 0x0003FC34 File Offset: 0x0003DE34
	private void OnUserInterfaceScaleChanged(ChangeEvent<float> changeEvent)
	{
		float newValue = changeEvent.newValue;
		Tween tween = this.debounceTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.debounceTween = DOVirtual.DelayedCall(1f, delegate
		{
			EventManager.TriggerEvent("Event_OnSettingsUserInterfaceScaleChanged", new Dictionary<string, object>
			{
				{
					"value",
					newValue
				}
			});
		}, true);
	}

	// Token: 0x06000D4D RID: 3405 RVA: 0x0003FC82 File Offset: 0x0003DE82
	public void UpdateUserInterfaceScale(float value)
	{
		this.userInterfaceScaleSlider.value = value;
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x0003FC90 File Offset: 0x0003DE90
	private void OnChatOpacityChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsChatOpacityChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D4F RID: 3407 RVA: 0x0003FCB7 File Offset: 0x0003DEB7
	public void UpdateChatOpacity(float value)
	{
		this.chatOpacitySlider.value = value;
	}

	// Token: 0x06000D50 RID: 3408 RVA: 0x0003FCC5 File Offset: 0x0003DEC5
	private void OnChatScaleChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsChatScaleChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D51 RID: 3409 RVA: 0x0003FCEC File Offset: 0x0003DEEC
	public void UpdateChatScale(float value)
	{
		this.chatScaleSlider.value = value;
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x0003FCFA File Offset: 0x0003DEFA
	private void OnMinimapOpacityChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsMinimapOpacityChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x0003FD21 File Offset: 0x0003DF21
	public void UpdateMinimapOpacity(float value)
	{
		this.minimapOpacitySlider.value = value;
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x0003FD2F File Offset: 0x0003DF2F
	private void OnMinimapBackgroundOpacityChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsMinimapBackgroundOpacityChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x0003FD56 File Offset: 0x0003DF56
	public void UpdateMinimapBackgroundOpacity(float value)
	{
		this.minimapBackgroundOpacitySlider.value = value;
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x0003FD64 File Offset: 0x0003DF64
	private void OnMinimapHorizontalPositionChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsMinimapHorizontalPositionChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D57 RID: 3415 RVA: 0x0003FD8B File Offset: 0x0003DF8B
	public void UpdateMinimapHorizontalPosition(float value)
	{
		this.minimapHorizontalPositionSlider.value = value;
	}

	// Token: 0x06000D58 RID: 3416 RVA: 0x0003FD99 File Offset: 0x0003DF99
	private void OnMinimapVerticalPositionChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsMinimapVerticalPositionChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D59 RID: 3417 RVA: 0x0003FDC0 File Offset: 0x0003DFC0
	public void UpdateMinimapVerticalPosition(float value)
	{
		this.minimapVerticalPositionSlider.value = value;
	}

	// Token: 0x06000D5A RID: 3418 RVA: 0x0003FDCE File Offset: 0x0003DFCE
	private void OnMinimapScaleChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsMinimapScaleChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D5B RID: 3419 RVA: 0x0003FDF5 File Offset: 0x0003DFF5
	public void UpdateMinimapScale(float value)
	{
		this.minimapScaleSlider.value = value;
	}

	// Token: 0x06000D5C RID: 3420 RVA: 0x0003FE03 File Offset: 0x0003E003
	private void OnGlobalStickSensitivityChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsGlobalStickSensitivityChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D5D RID: 3421 RVA: 0x0003FE2A File Offset: 0x0003E02A
	public void UpdateGlobalStickSensitivity(float value)
	{
		this.globalStickSensitivitySlider.value = value;
	}

	// Token: 0x06000D5E RID: 3422 RVA: 0x0003FE38 File Offset: 0x0003E038
	private void OnHorizontalStickSensitivityChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsHorizontalStickSensitivityChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D5F RID: 3423 RVA: 0x0003FE5F File Offset: 0x0003E05F
	public void UpdateHorizontalStickSensitivity(float value)
	{
		this.horizontalStickSensitivitySlider.value = value;
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x0003FE6D File Offset: 0x0003E06D
	private void OnVerticalStickSensitivityChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsVerticalStickSensitivityChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D61 RID: 3425 RVA: 0x0003FE94 File Offset: 0x0003E094
	public void UpdateVerticalStickSensitivity(float value)
	{
		this.verticalStickSensitivitySlider.value = value;
	}

	// Token: 0x06000D62 RID: 3426 RVA: 0x0003FEA2 File Offset: 0x0003E0A2
	private void OnKeyBindInputClicked(string actionName)
	{
		EventManager.TriggerEvent("Event_OnSettingsKeyBindInputClicked", new Dictionary<string, object>
		{
			{
				"actionName",
				actionName
			}
		});
	}

	// Token: 0x06000D63 RID: 3427 RVA: 0x0003FEBF File Offset: 0x0003E0BF
	private void OnKeyBindInputInteractionChanged(string actionName, KeyBindInteraction interaction)
	{
		EventManager.TriggerEvent("Event_OnSettingsKeyBindInputInteractionChanged", new Dictionary<string, object>
		{
			{
				"actionName",
				actionName
			},
			{
				"interaction",
				interaction
			}
		});
	}

	// Token: 0x06000D64 RID: 3428 RVA: 0x0003FEED File Offset: 0x0003E0ED
	private void OnLookSensitivityChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsLookSensitivityChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D65 RID: 3429 RVA: 0x0003FF14 File Offset: 0x0003E114
	public void UpdateLookSensitivity(float value)
	{
		this.lookSensitivitySlider.value = value;
	}

	// Token: 0x06000D66 RID: 3430 RVA: 0x0003FF22 File Offset: 0x0003E122
	private void OnGlobalVolumeChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsGlobalVolumeChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x0003FF49 File Offset: 0x0003E149
	public void UpdateGlobalVolume(float value)
	{
		this.globalVolumeSlider.value = value;
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x0003FF57 File Offset: 0x0003E157
	private void OnAmbientVolumeChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsAmbientVolumeChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x0003FF7E File Offset: 0x0003E17E
	public void UpdateAmbientVolume(float value)
	{
		this.ambientVolumeSlider.value = value;
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x0003FF8C File Offset: 0x0003E18C
	private void OnGameVolumeChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsGameVolumeChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x0003FFB3 File Offset: 0x0003E1B3
	public void UpdateGameVolume(float value)
	{
		this.gameVolumeSlider.value = value;
	}

	// Token: 0x06000D6C RID: 3436 RVA: 0x0003FFC1 File Offset: 0x0003E1C1
	private void OnVoiceVolumeChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsVoiceVolumeChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D6D RID: 3437 RVA: 0x0003FFE8 File Offset: 0x0003E1E8
	public void UpdateVoiceVolume(float value)
	{
		this.voiceVolumeSlider.value = value;
	}

	// Token: 0x06000D6E RID: 3438 RVA: 0x0003FFF6 File Offset: 0x0003E1F6
	private void OnUIVolumeChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsUIVolumeChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D6F RID: 3439 RVA: 0x0004001D File Offset: 0x0003E21D
	public void UpdateUIVolume(float value)
	{
		this.uiVolumeSlider.value = value;
	}

	// Token: 0x06000D70 RID: 3440 RVA: 0x0004002B File Offset: 0x0003E22B
	private void OnFullScreenModeChanged(ChangeEvent<string> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsFullScreenModeChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D71 RID: 3441 RVA: 0x0004004D File Offset: 0x0003E24D
	public void UpdateFullScreenMode(string value)
	{
		this.fullScreenModeDropdown.value = value;
	}

	// Token: 0x06000D72 RID: 3442 RVA: 0x0004005B File Offset: 0x0003E25B
	private void OnDisplayChanged(ChangeEvent<string> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsDisplayChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D73 RID: 3443 RVA: 0x0004007D File Offset: 0x0003E27D
	public void UpdateDisplay(string value)
	{
		this.displayDropdown.value = value;
	}

	// Token: 0x06000D74 RID: 3444 RVA: 0x0004008B File Offset: 0x0003E28B
	public void UpdateDisplayChoices(List<string> choices)
	{
		this.displayDropdown.choices = choices;
	}

	// Token: 0x06000D75 RID: 3445 RVA: 0x00040099 File Offset: 0x0003E299
	private void OnResolutionChanged(ChangeEvent<string> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsResolutionChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D76 RID: 3446 RVA: 0x000400BB File Offset: 0x0003E2BB
	public void UpdateResolution(string value)
	{
		this.resolutionDropdown.value = value;
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x000400C9 File Offset: 0x0003E2C9
	public void UpdateResolutionChoices(List<string> choices)
	{
		this.resolutionDropdown.choices = choices;
	}

	// Token: 0x06000D78 RID: 3448 RVA: 0x000400D7 File Offset: 0x0003E2D7
	private void OnVSyncChanged(ChangeEvent<bool> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsVSyncChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D79 RID: 3449 RVA: 0x000400FE File Offset: 0x0003E2FE
	public void UpdateVSync(bool value)
	{
		this.vSyncToggle.value = value;
	}

	// Token: 0x06000D7A RID: 3450 RVA: 0x0004010C File Offset: 0x0003E30C
	private void OnFpsLimitChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsFpsLimitChanged", new Dictionary<string, object>
		{
			{
				"value",
				(int)changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x00040134 File Offset: 0x0003E334
	public void UpdateFpsLimit(int value)
	{
		this.fpsLimitSlider.value = (float)value;
	}

	// Token: 0x06000D7C RID: 3452 RVA: 0x00040143 File Offset: 0x0003E343
	private void OnFovChanged(ChangeEvent<float> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsFovChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D7D RID: 3453 RVA: 0x0004016A File Offset: 0x0003E36A
	public void UpdateFov(float value)
	{
		this.fovSlider.value = value;
	}

	// Token: 0x06000D7E RID: 3454 RVA: 0x00040178 File Offset: 0x0003E378
	private void OnQualityChanged(ChangeEvent<string> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsQualityChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D7F RID: 3455 RVA: 0x0004019A File Offset: 0x0003E39A
	public void UpdateQuality(string value)
	{
		this.qualityDropdown.value = value;
	}

	// Token: 0x06000D80 RID: 3456 RVA: 0x000401A8 File Offset: 0x0003E3A8
	private void OnMotionBlurChanged(ChangeEvent<bool> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnSettingsMotionBlurChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000D81 RID: 3457 RVA: 0x000401CF File Offset: 0x0003E3CF
	public void UpdateMotionBlur(bool value)
	{
		this.motionBlurToggle.value = value;
	}

	// Token: 0x040007D9 RID: 2009
	private VisualElement settings;

	// Token: 0x040007DA RID: 2010
	private IconButton closeIconButton;

	// Token: 0x040007DB RID: 2011
	private Slider cameraAngleSlider;

	// Token: 0x040007DC RID: 2012
	private DropdownField handednessDropdown;

	// Token: 0x040007DD RID: 2013
	private Toggle showPuckSilhouetteToggle;

	// Token: 0x040007DE RID: 2014
	private Toggle showPuckOutlineToggle;

	// Token: 0x040007DF RID: 2015
	private Toggle showPuckElevationToggle;

	// Token: 0x040007E0 RID: 2016
	private Toggle showPlayerUsernamesToggle;

	// Token: 0x040007E1 RID: 2017
	private Slider playerUsernamesFadeThresholdSlider;

	// Token: 0x040007E2 RID: 2018
	private Toggle useNetworkSmoothingToggle;

	// Token: 0x040007E3 RID: 2019
	private SliderInt networkSmoothingStrengthSliderInt;

	// Token: 0x040007E4 RID: 2020
	private SliderInt maxMatchmakingPingSliderInt;

	// Token: 0x040007E5 RID: 2021
	private Toggle filterChatProfanityToggle;

	// Token: 0x040007E6 RID: 2022
	private DropdownField unitsDropdown;

	// Token: 0x040007E7 RID: 2023
	private Toggle showGameUserInterfaceToggle;

	// Token: 0x040007E8 RID: 2024
	private Slider userInterfaceScaleSlider;

	// Token: 0x040007E9 RID: 2025
	private Slider chatOpacitySlider;

	// Token: 0x040007EA RID: 2026
	private Slider chatScaleSlider;

	// Token: 0x040007EB RID: 2027
	private Slider minimapOpacitySlider;

	// Token: 0x040007EC RID: 2028
	private Slider minimapBackgroundOpacitySlider;

	// Token: 0x040007ED RID: 2029
	private Slider minimapHorizontalPositionSlider;

	// Token: 0x040007EE RID: 2030
	private Slider minimapVerticalPositionSlider;

	// Token: 0x040007EF RID: 2031
	private Slider minimapScaleSlider;

	// Token: 0x040007F0 RID: 2032
	private Slider globalStickSensitivitySlider;

	// Token: 0x040007F1 RID: 2033
	private Slider horizontalStickSensitivitySlider;

	// Token: 0x040007F2 RID: 2034
	private Slider verticalStickSensitivitySlider;

	// Token: 0x040007F3 RID: 2035
	private Slider lookSensitivitySlider;

	// Token: 0x040007F4 RID: 2036
	private Dictionary<string, KeyBindField> actionNameKeyBindFieldMap;

	// Token: 0x040007F5 RID: 2037
	private Slider globalVolumeSlider;

	// Token: 0x040007F6 RID: 2038
	private Slider ambientVolumeSlider;

	// Token: 0x040007F7 RID: 2039
	private Slider gameVolumeSlider;

	// Token: 0x040007F8 RID: 2040
	private Slider voiceVolumeSlider;

	// Token: 0x040007F9 RID: 2041
	private Slider uiVolumeSlider;

	// Token: 0x040007FA RID: 2042
	private DropdownField fullScreenModeDropdown;

	// Token: 0x040007FB RID: 2043
	private DropdownField displayDropdown;

	// Token: 0x040007FC RID: 2044
	private DropdownField resolutionDropdown;

	// Token: 0x040007FD RID: 2045
	private Toggle vSyncToggle;

	// Token: 0x040007FE RID: 2046
	private Slider fpsLimitSlider;

	// Token: 0x040007FF RID: 2047
	private Slider fovSlider;

	// Token: 0x04000800 RID: 2048
	private DropdownField qualityDropdown;

	// Token: 0x04000801 RID: 2049
	private Toggle motionBlurToggle;

	// Token: 0x04000802 RID: 2050
	private Button resetToDefaultButton;

	// Token: 0x04000803 RID: 2051
	private Tween debounceTween;
}
