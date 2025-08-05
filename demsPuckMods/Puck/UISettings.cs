using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

// Token: 0x0200013B RID: 315
public class UISettings : UIComponent<UISettings>
{
	// Token: 0x06000B08 RID: 2824 RVA: 0x0000E1BF File Offset: 0x0000C3BF
	private void Start()
	{
		base.VisibilityRequiresMouse = true;
	}

	// Token: 0x06000B09 RID: 2825 RVA: 0x0003F0C4 File Offset: 0x0003D2C4
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("SettingsContainer", null);
		this.cameraAngleSlider = this.container.Query("CameraAngleSlider", null).First().Query("Slider", null);
		this.cameraAngleSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnCameraAngleChanged));
		this.handednessDropdown = this.container.Query("HandednessDropdown", null).First().Query("Dropdown", null);
		this.handednessDropdown.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnHandednessChanged));
		this.showPuckSilhouetteToggle = this.container.Query("ShowPuckSilhouetteToggle", null).First().Query("Toggle", null);
		this.showPuckSilhouetteToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnShowPuckSilhouetteChanged));
		this.showPuckOutlineToggle = this.container.Query("ShowPuckOutlineToggle", null).First().Query("Toggle", null);
		this.showPuckOutlineToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnShowPuckOutlineChanged));
		this.showPuckElevationToggle = this.container.Query("ShowPuckElevationToggle", null).First().Query("Toggle", null);
		this.showPuckElevationToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnShowPuckEleveationChanged));
		this.showPlayerUsernamesToggle = this.container.Query("ShowPlayerUsernamesToggle", null).First().Query("Toggle", null);
		this.showPlayerUsernamesToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnShowPlayerUsernamesChanged));
		this.playerUsernamesFadeThresholdSlider = this.container.Query("PlayerUsernamesFadeThresholdSlider", null).First().Query("Slider", null);
		this.playerUsernamesFadeThresholdSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnPlayerUsernamesFadeThresholdChanged));
		this.useNetworkSmoothingToggle = this.container.Query("UseNetworkSmoothingToggle", null).First().Query("Toggle", null);
		this.useNetworkSmoothingToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnUseNetworkSmoothingChanged));
		this.networkSmoothingStrengthSlider = this.container.Query("NetworkSmoothingStrengthSlider", null).First().Query("Slider", null);
		this.networkSmoothingStrengthSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnNetworkSmoothingStrengthChanged));
		this.filterChatProfanityToggle = this.container.Query("FilterChatProfanityToggle", null).First().Query("Toggle", null);
		this.filterChatProfanityToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnFilterChatProfanityChanged));
		this.unitsDropdown = this.container.Query("UnitsDropdown", null).First().Query("Dropdown", null);
		this.unitsDropdown.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnUnitsChanged));
		this.showGameUserInterfaceToggle = this.container.Query("ShowGameUserInterfaceToggle", null).First().Query("Toggle", null);
		this.showGameUserInterfaceToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnShowGameUserInterfaceChanged));
		this.userInterfaceScaleSlider = this.container.Query("UserInterfaceScaleSlider", null).First().Query("Slider", null);
		this.userInterfaceScaleSlider.Query("unity-text-field", null).First().RegisterCallback<ChangeEvent<string>>(new EventCallback<ChangeEvent<string>>(this.OnUserInterfaceScaleChanged), TrickleDown.NoTrickleDown);
		this.userInterfaceScaleSlider.Query("unity-drag-container", null).First().RegisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(this.OnUserInterfaceScaleMouseUp), TrickleDown.NoTrickleDown);
		this.chatOpacitySlider = this.container.Query("ChatOpacitySlider", null).First().Query("Slider", null);
		this.chatOpacitySlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnChatOpacityChanged));
		this.chatScaleSlider = this.container.Query("ChatScaleSlider", null).First().Query("Slider", null);
		this.chatScaleSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnChatScaleChanged));
		this.minimapOpacitySlider = this.container.Query("MinimapOpacitySlider", null).First().Query("Slider", null);
		this.minimapOpacitySlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnMinimapOpacityChanged));
		this.minimapBackgroundOpacitySlider = this.container.Query("MinimapBackgroundOpacitySlider", null).First().Query("Slider", null);
		this.minimapBackgroundOpacitySlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnMinimapBackgroundOpacityChanged));
		this.minimapHorizontalPositionSlider = this.container.Query("MinimapHorizontalPositionSlider", null).First().Query("Slider", null);
		this.minimapHorizontalPositionSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnMinimapHorizontalPositionChanged));
		this.minimapVerticalPositionSlider = this.container.Query("MinimapVerticalPositionSlider", null).First().Query("Slider", null);
		this.minimapVerticalPositionSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnMinimapVerticalPositionChanged));
		this.minimapScaleSlider = this.container.Query("MinimapScaleSlider", null).First().Query("Slider", null);
		this.minimapScaleSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnMinimapScaleChanged));
		this.globalStickSensitivitySlider = this.container.Query("GlobalStickSensitivitySlider", null).First().Query("Slider", null);
		this.globalStickSensitivitySlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnGlobalStickSensitivityChanged));
		this.horizontalStickSensitivitySlider = this.container.Query("HorizontalStickSensitivitySlider", null).First().Query("Slider", null);
		this.horizontalStickSensitivitySlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnHorizontalStickSensitivityChanged));
		this.verticalStickSensitivitySlider = this.container.Query("VerticalStickSensitivitySlider", null).First().Query("Slider", null);
		this.verticalStickSensitivitySlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnVerticalStickSensitivityChanged));
		this.lookSensitivitySlider = this.container.Query("LookSensitivitySlider", null).First().Query("Slider", null);
		this.lookSensitivitySlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnLookSensitivityChanged));
		this.keyBindControls = new Dictionary<string, KeyBindControl>
		{
			{
				"Move Forward",
				this.container.Query("MoveForwardKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Move Backward",
				this.container.Query("MoveBackwardKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Turn Left",
				this.container.Query("TurnLeftKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Turn Right",
				this.container.Query("TurnRightKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Blade Angle Up",
				this.container.Query("BladeAngleUpKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Blade Angle Down",
				this.container.Query("BladeAngleDownKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Slide",
				this.container.Query("SlideKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Sprint",
				this.container.Query("SprintKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Track",
				this.container.Query("TrackKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Look",
				this.container.Query("LookKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Jump",
				this.container.Query("JumpKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Stop",
				this.container.Query("StopKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Twist Left",
				this.container.Query("TwistLeftKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Twist Right",
				this.container.Query("TwistRightKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Dash Left",
				this.container.Query("DashLeftKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Dash Right",
				this.container.Query("DashRightKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Extend Left",
				this.container.Query("ExtendLeftKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Extend Right",
				this.container.Query("ExtendRightKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Lateral Left",
				this.container.Query("LateralLeftKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Lateral Right",
				this.container.Query("LateralRightKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Talk",
				this.container.Query("TalkKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"All Chat",
				this.container.Query("AllChatKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Team Chat",
				this.container.Query("TeamChatKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Position Select",
				this.container.Query("PositionSelectKeyBind", null).First().Query("KeyBindControl", null)
			},
			{
				"Scoreboard",
				this.container.Query("ScoreboardKeyBind", null).First().Query("KeyBindControl", null)
			}
		};
		using (Dictionary<string, KeyBindControl>.Enumerator enumerator = this.keyBindControls.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, KeyBindControl> keyBindControl = enumerator.Current;
				keyBindControl.Value.OnClicked = delegate()
				{
					this.OnKeyBindClicked(keyBindControl.Key);
				};
				keyBindControl.Value.OnTypeDropdownValueChanged = delegate(string value)
				{
					this.OnKeyBindTypeChanged(keyBindControl.Key, value);
				};
			}
		}
		this.globalVolumeSlider = this.container.Query("GlobalVolumeSlider", null).First().Query("Slider", null);
		this.globalVolumeSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnGlobalVolumeChanged));
		this.ambientVolumeSlider = this.container.Query("AmbientVolumeSlider", null).First().Query("Slider", null);
		this.ambientVolumeSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnAmbientVolumeChanged));
		this.gameVolumeSlider = this.container.Query("GameVolumeSlider", null).First().Query("Slider", null);
		this.gameVolumeSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnGameVolumeChanged));
		this.voiceVolumeSlider = this.container.Query("VoiceVolumeSlider", null).First().Query("Slider", null);
		this.voiceVolumeSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnVoiceVolumeChanged));
		this.uiVolumeSlider = this.container.Query("UIVolumeSlider", null).First().Query("Slider", null);
		this.uiVolumeSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnUIVolumeChanged));
		this.windowModeDropdown = this.container.Query("WindowModeDropdown", null).First().Query("Dropdown", null);
		this.windowModeDropdown.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnWindowModeChanged));
		this.displayDropdown = this.container.Query("DisplayDropdown", null).First().Query("Dropdown", null);
		this.displayDropdown.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnDisplayChanged));
		this.resolutionDropdown = this.container.Query("ResolutionDropdown", null).First().Query("Dropdown", null);
		this.resolutionDropdown.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnResolutionChanged));
		this.vSyncToggle = this.container.Query("VSyncToggle", null).First().Query("Toggle", null);
		this.vSyncToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnVSyncChanged));
		this.fpsLimitSlider = this.container.Query("FPSLimitSlider", null).First().Query("Slider", null);
		this.fpsLimitSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnFpsLimitChanged));
		this.fovSlider = this.container.Query("FOVSlider", null).First().Query("Slider", null);
		this.fovSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnFovChanged));
		this.qualityDropdown = this.container.Query("QualityDropdown", null).First().Query("Dropdown", null);
		this.qualityDropdown.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnQualityChanged));
		this.motionBlurToggle = this.container.Query("MotionBlurToggle", null).First().Query("Toggle", null);
		this.motionBlurToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnMotionBlurChanged));
		this.closeButton = this.container.Query("CloseButton", null);
		this.closeButton.clicked += this.OnClickClose;
		this.resetToDefaultButton = this.container.Query("ResetToDefaultButton", null);
		this.resetToDefaultButton.clicked += this.OnClickResetToDefault;
	}

	// Token: 0x06000B0A RID: 2826 RVA: 0x00040178 File Offset: 0x0003E378
	public void ApplySettingsValues()
	{
		if (Application.isBatchMode)
		{
			return;
		}
		this.cameraAngleSlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.CameraAngle;
		this.handednessDropdown.value = MonoBehaviourSingleton<SettingsManager>.Instance.Handedness;
		this.showPuckSilhouetteToggle.value = (MonoBehaviourSingleton<SettingsManager>.Instance.ShowPuckSilhouette > 0);
		this.showPuckOutlineToggle.value = (MonoBehaviourSingleton<SettingsManager>.Instance.ShowPuckOutline > 0);
		this.showPuckElevationToggle.value = (MonoBehaviourSingleton<SettingsManager>.Instance.ShowPuckElevation > 0);
		this.showPlayerUsernamesToggle.value = (MonoBehaviourSingleton<SettingsManager>.Instance.ShowPlayerUsernames > 0);
		this.playerUsernamesFadeThresholdSlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.PlayerUsernamesFadeThreshold;
		this.useNetworkSmoothingToggle.value = (MonoBehaviourSingleton<SettingsManager>.Instance.UseNetworkSmoothing > 0);
		this.networkSmoothingStrengthSlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.NetworkSmoothingStrength;
		this.filterChatProfanityToggle.value = (MonoBehaviourSingleton<SettingsManager>.Instance.FilterChatProfanity > 0);
		this.unitsDropdown.value = MonoBehaviourSingleton<SettingsManager>.Instance.Units;
		this.showGameUserInterfaceToggle.value = (MonoBehaviourSingleton<SettingsManager>.Instance.ShowGameUserInterface > 0);
		this.userInterfaceScaleSlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.UserInterfaceScale;
		this.chatOpacitySlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.ChatOpacity;
		this.chatScaleSlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.ChatScale;
		this.minimapOpacitySlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.MinimapOpacity;
		this.minimapBackgroundOpacitySlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.MinimapBackgroundOpacity;
		this.minimapHorizontalPositionSlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.MinimapHorizontalPosition;
		this.minimapVerticalPositionSlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.MinimapVerticalPosition;
		this.minimapScaleSlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.MinimapScale;
		this.globalStickSensitivitySlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.GlobalStickSensitivity;
		this.horizontalStickSensitivitySlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.HorizontalStickSensitivity;
		this.verticalStickSensitivitySlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.VerticalStickSensitivity;
		this.lookSensitivitySlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.LookSensitivity;
		this.UpdateKeyBinds(MonoBehaviourSingleton<InputManager>.Instance.KeyBinds);
		this.globalVolumeSlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.GlobalVolume;
		this.ambientVolumeSlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.AmbientVolume;
		this.gameVolumeSlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.GameVolume;
		this.voiceVolumeSlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.VoiceVolume;
		this.uiVolumeSlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.UIVolume;
		this.windowModeDropdown.value = MonoBehaviourSingleton<SettingsManager>.Instance.WindowMode;
		this.vSyncToggle.value = (MonoBehaviourSingleton<SettingsManager>.Instance.VSync > 0);
		this.fpsLimitSlider.value = (float)MonoBehaviourSingleton<SettingsManager>.Instance.FpsLimit;
		this.fovSlider.value = MonoBehaviourSingleton<SettingsManager>.Instance.Fov;
		this.qualityDropdown.value = MonoBehaviourSingleton<SettingsManager>.Instance.Quality;
		this.motionBlurToggle.value = (MonoBehaviourSingleton<SettingsManager>.Instance.MotionBlur > 0);
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x0000E1C8 File Offset: 0x0000C3C8
	private void OnClickClose()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsClickClose", null);
	}

	// Token: 0x06000B0C RID: 2828 RVA: 0x0000E1DA File Offset: 0x0000C3DA
	private void OnClickResetToDefault()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsClickResetToDefault", null);
	}

	// Token: 0x06000B0D RID: 2829 RVA: 0x000404BC File Offset: 0x0003E6BC
	public void UpdateDisplayDropdown(string value)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		this.displayDropdown.value = value;
		List<DisplayInfo> list = new List<DisplayInfo>();
		Screen.GetDisplayLayout(list);
		List<string> list2 = new List<string>();
		for (int i = 0; i < list.Count; i++)
		{
			list2.Add(list[i].name);
		}
		this.displayDropdown.choices = list2;
	}

	// Token: 0x06000B0E RID: 2830 RVA: 0x00040520 File Offset: 0x0003E720
	public void UpdateResolutionsDropdown(string value)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		this.resolutionDropdown.value = value;
		List<string> list = new List<string>();
		for (int i = 0; i < Screen.resolutions.Length; i++)
		{
			list.Add(Utils.GetResolutionStringFromIndex(i));
		}
		this.resolutionDropdown.choices = list;
	}

	// Token: 0x06000B0F RID: 2831 RVA: 0x00040574 File Offset: 0x0003E774
	public void UpdateKeyBinds(Dictionary<string, KeyBind> keyBinds)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		foreach (KeyValuePair<string, KeyBind> keyValuePair in keyBinds)
		{
			if (this.keyBindControls.ContainsKey(keyValuePair.Key))
			{
				KeyBindControl keyBindControl = this.keyBindControls[keyValuePair.Key];
				InputAction inputAction = MonoBehaviourSingleton<InputManager>.Instance.RebindableInputActions[keyValuePair.Key];
				if (inputAction.bindings[0].isComposite)
				{
					string text = null;
					if (!string.IsNullOrEmpty(inputAction.bindings[1].effectivePath))
					{
						text = text + inputAction.GetBindingDisplayString(1, InputBinding.DisplayStringOptions.DontIncludeInteractions).ToUpper() + "+";
					}
					text += inputAction.GetBindingDisplayString(2, InputBinding.DisplayStringOptions.DontIncludeInteractions).ToUpper();
					keyBindControl.PathLabel = text;
				}
				else
				{
					keyBindControl.PathLabel = inputAction.GetBindingDisplayString(0, InputBinding.DisplayStringOptions.DontIncludeInteractions).ToUpper();
				}
				keyBindControl.TypeDropdownValue = Utils.GetHumanizedInteractionFromInteraction(keyValuePair.Value.Interactions, keyBindControl.IsPressable, keyBindControl.IsHoldable);
			}
		}
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x0000E1EC File Offset: 0x0000C3EC
	private void OnCameraAngleChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsCameraAngleChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x0000E218 File Offset: 0x0000C418
	private void OnHandednessChanged(ChangeEvent<string> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsHandednessChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x0000E23F File Offset: 0x0000C43F
	private void OnShowPuckSilhouetteChanged(ChangeEvent<bool> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsShowPuckSilhouetteChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x0000E26B File Offset: 0x0000C46B
	private void OnShowPuckOutlineChanged(ChangeEvent<bool> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsShowPuckOutlineChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x0000E297 File Offset: 0x0000C497
	private void OnShowPuckEleveationChanged(ChangeEvent<bool> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsShowPuckElevationChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x0000E2C3 File Offset: 0x0000C4C3
	private void OnShowPlayerUsernamesChanged(ChangeEvent<bool> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsShowPlayerUsernamesChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B16 RID: 2838 RVA: 0x0000E2EF File Offset: 0x0000C4EF
	private void OnPlayerUsernamesFadeThresholdChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsPlayerUsernamesFadeThresholdChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x000406C8 File Offset: 0x0003E8C8
	private void OnUseNetworkSmoothingChanged(ChangeEvent<bool> changeEvent)
	{
		this.useNetworkSmoothingToggle.value = changeEvent.newValue;
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsUseNetworkSmoothingChanged", new Dictionary<string, object>
		{
			{
				"value",
				this.useNetworkSmoothingToggle.value
			}
		});
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x00040718 File Offset: 0x0003E918
	private void OnNetworkSmoothingStrengthChanged(ChangeEvent<float> changeEvent)
	{
		this.networkSmoothingStrengthSlider.value = (float)Mathf.RoundToInt(changeEvent.newValue);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsNetworkSmoothingStrengthChanged", new Dictionary<string, object>
		{
			{
				"value",
				this.networkSmoothingStrengthSlider.value
			}
		});
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x0000E31B File Offset: 0x0000C51B
	private void OnFilterChatProfanityChanged(ChangeEvent<bool> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsFilterChatProfanityChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x0000E347 File Offset: 0x0000C547
	private void OnUnitsChanged(ChangeEvent<string> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsUnitsChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B1B RID: 2843 RVA: 0x0000E36E File Offset: 0x0000C56E
	private void OnShowGameUserInterfaceChanged(ChangeEvent<bool> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsShowGameUserInterfaceChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B1C RID: 2844 RVA: 0x0000E39A File Offset: 0x0000C59A
	private void OnUserInterfaceScaleChanged(ChangeEvent<string> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsUserInterfaceScaleChanged", new Dictionary<string, object>
		{
			{
				"value",
				this.userInterfaceScaleSlider.value
			}
		});
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x0000E39A File Offset: 0x0000C59A
	private void OnUserInterfaceScaleMouseUp(MouseUpEvent changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsUserInterfaceScaleChanged", new Dictionary<string, object>
		{
			{
				"value",
				this.userInterfaceScaleSlider.value
			}
		});
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x0000E3CB File Offset: 0x0000C5CB
	private void OnChatOpacityChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsChatOpacityChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x0000E3F7 File Offset: 0x0000C5F7
	private void OnChatScaleChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsChatScaleChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x0000E423 File Offset: 0x0000C623
	private void OnMinimapOpacityChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsMinimapOpacityChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B21 RID: 2849 RVA: 0x0000E44F File Offset: 0x0000C64F
	private void OnMinimapBackgroundOpacityChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsMinimapBackgroundOpacityChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B22 RID: 2850 RVA: 0x0000E47B File Offset: 0x0000C67B
	private void OnMinimapHorizontalPositionChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsMinimapHorizontalPositionChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x0000E4A7 File Offset: 0x0000C6A7
	private void OnMinimapVerticalPositionChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsMinimapVerticalPositionChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B24 RID: 2852 RVA: 0x0000E4D3 File Offset: 0x0000C6D3
	private void OnMinimapScaleChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsMinimapScaleChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x0000E4FF File Offset: 0x0000C6FF
	private void OnGlobalStickSensitivityChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsGlobalStickSensitivityChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B26 RID: 2854 RVA: 0x0000E52B File Offset: 0x0000C72B
	private void OnHorizontalStickSensitivityChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsHorizontalStickSensitivityChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B27 RID: 2855 RVA: 0x0000E557 File Offset: 0x0000C757
	private void OnVerticalStickSensitivityChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsVerticalStickSensitivityChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B28 RID: 2856 RVA: 0x0000E583 File Offset: 0x0000C783
	private void OnKeyBindClicked(string actionName)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsKeyBindClicked", new Dictionary<string, object>
		{
			{
				"actionName",
				actionName
			}
		});
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x0000E5A5 File Offset: 0x0000C7A5
	private void OnKeyBindTypeChanged(string actionName, string value)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsKeyBindTypeChanged", new Dictionary<string, object>
		{
			{
				"actionName",
				actionName
			},
			{
				"type",
				value
			}
		});
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x0000E5D3 File Offset: 0x0000C7D3
	private void OnLookSensitivityChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsLookSensitivityChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x0000E5FF File Offset: 0x0000C7FF
	private void OnGlobalVolumeChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsGlobalVolumeChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x0000E62B File Offset: 0x0000C82B
	private void OnAmbientVolumeChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsAmbientVolumeChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B2D RID: 2861 RVA: 0x0000E657 File Offset: 0x0000C857
	private void OnGameVolumeChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsGameVolumeChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x0000E683 File Offset: 0x0000C883
	private void OnVoiceVolumeChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsVoiceVolumeChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x0000E6AF File Offset: 0x0000C8AF
	private void OnUIVolumeChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsUIVolumeChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x0000E6DB File Offset: 0x0000C8DB
	private void OnWindowModeChanged(ChangeEvent<string> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsWindowModeChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x0000E702 File Offset: 0x0000C902
	private void OnDisplayChanged(ChangeEvent<string> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsDisplayChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x0000E729 File Offset: 0x0000C929
	private void OnResolutionChanged(ChangeEvent<string> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsResolutionChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x0000E750 File Offset: 0x0000C950
	private void OnVSyncChanged(ChangeEvent<bool> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsVSyncChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B34 RID: 2868 RVA: 0x0000E77C File Offset: 0x0000C97C
	private void OnFpsLimitChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsFpsLimitChanged", new Dictionary<string, object>
		{
			{
				"value",
				(int)changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x0000E7A9 File Offset: 0x0000C9A9
	private void OnFovChanged(ChangeEvent<float> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsFovChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x0000E7D5 File Offset: 0x0000C9D5
	private void OnQualityChanged(ChangeEvent<string> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsQualityChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x0000E7FC File Offset: 0x0000C9FC
	private void OnMotionBlurChanged(ChangeEvent<bool> changeEvent)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSettingsMotionBlurChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x0004076C File Offset: 0x0003E96C
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x0000E830 File Offset: 0x0000CA30
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x0000E83A File Offset: 0x0000CA3A
	protected internal override string __getTypeName()
	{
		return "UISettings";
	}

	// Token: 0x0400067A RID: 1658
	private Slider cameraAngleSlider;

	// Token: 0x0400067B RID: 1659
	private DropdownField handednessDropdown;

	// Token: 0x0400067C RID: 1660
	private Toggle showPuckSilhouetteToggle;

	// Token: 0x0400067D RID: 1661
	private Toggle showPuckOutlineToggle;

	// Token: 0x0400067E RID: 1662
	private Toggle showPuckElevationToggle;

	// Token: 0x0400067F RID: 1663
	private Toggle showPlayerUsernamesToggle;

	// Token: 0x04000680 RID: 1664
	private Slider playerUsernamesFadeThresholdSlider;

	// Token: 0x04000681 RID: 1665
	private Toggle useNetworkSmoothingToggle;

	// Token: 0x04000682 RID: 1666
	private Slider networkSmoothingStrengthSlider;

	// Token: 0x04000683 RID: 1667
	private Toggle filterChatProfanityToggle;

	// Token: 0x04000684 RID: 1668
	private DropdownField unitsDropdown;

	// Token: 0x04000685 RID: 1669
	private Toggle showGameUserInterfaceToggle;

	// Token: 0x04000686 RID: 1670
	private Slider userInterfaceScaleSlider;

	// Token: 0x04000687 RID: 1671
	private Slider chatOpacitySlider;

	// Token: 0x04000688 RID: 1672
	private Slider chatScaleSlider;

	// Token: 0x04000689 RID: 1673
	private Slider minimapOpacitySlider;

	// Token: 0x0400068A RID: 1674
	private Slider minimapBackgroundOpacitySlider;

	// Token: 0x0400068B RID: 1675
	private Slider minimapHorizontalPositionSlider;

	// Token: 0x0400068C RID: 1676
	private Slider minimapVerticalPositionSlider;

	// Token: 0x0400068D RID: 1677
	private Slider minimapScaleSlider;

	// Token: 0x0400068E RID: 1678
	private Slider globalStickSensitivitySlider;

	// Token: 0x0400068F RID: 1679
	private Slider horizontalStickSensitivitySlider;

	// Token: 0x04000690 RID: 1680
	private Slider verticalStickSensitivitySlider;

	// Token: 0x04000691 RID: 1681
	private Slider lookSensitivitySlider;

	// Token: 0x04000692 RID: 1682
	private Dictionary<string, KeyBindControl> keyBindControls;

	// Token: 0x04000693 RID: 1683
	private Slider globalVolumeSlider;

	// Token: 0x04000694 RID: 1684
	private Slider ambientVolumeSlider;

	// Token: 0x04000695 RID: 1685
	private Slider gameVolumeSlider;

	// Token: 0x04000696 RID: 1686
	private Slider voiceVolumeSlider;

	// Token: 0x04000697 RID: 1687
	private Slider uiVolumeSlider;

	// Token: 0x04000698 RID: 1688
	private DropdownField windowModeDropdown;

	// Token: 0x04000699 RID: 1689
	private DropdownField displayDropdown;

	// Token: 0x0400069A RID: 1690
	private DropdownField resolutionDropdown;

	// Token: 0x0400069B RID: 1691
	private Toggle vSyncToggle;

	// Token: 0x0400069C RID: 1692
	private Slider fpsLimitSlider;

	// Token: 0x0400069D RID: 1693
	private Slider fovSlider;

	// Token: 0x0400069E RID: 1694
	private DropdownField qualityDropdown;

	// Token: 0x0400069F RID: 1695
	private Toggle motionBlurToggle;

	// Token: 0x040006A0 RID: 1696
	private Button closeButton;

	// Token: 0x040006A1 RID: 1697
	private Button resetToDefaultButton;
}
