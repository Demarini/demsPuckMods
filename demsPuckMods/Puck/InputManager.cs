using System;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x02000041 RID: 65
public class InputManager : MonoBehaviourSingleton<InputManager>
{
	// Token: 0x060001D3 RID: 467 RVA: 0x000145F4 File Offset: 0x000127F4
	public override void Awake()
	{
		base.Awake();
		this.playerInput = base.GetComponent<UnityEngine.InputSystem.PlayerInput>();
		this.actions = this.playerInput.actions;
		InputSystem.RegisterInteraction<DoublePressInteraction>(null);
		InputSystem.RegisterInteraction<ToggleInteraction>(null);
		this.MoveForwardAction = this.actions.FindAction("Move Forward", false);
		this.MoveBackwardAction = this.actions.FindAction("Move Backward", false);
		this.TurnLeftAction = this.actions.FindAction("Turn Left", false);
		this.TurnRightAction = this.actions.FindAction("Turn Right", false);
		this.StickAction = this.actions.FindAction("Stick", false);
		this.BladeAngleUpAction = this.actions.FindAction("Blade Angle Up", false);
		this.BladeAngleDownAction = this.actions.FindAction("Blade Angle Down", false);
		this.SlideAction = this.actions.FindAction("Slide", false);
		this.SprintAction = this.actions.FindAction("Sprint", false);
		this.TrackAction = this.actions.FindAction("Track", false);
		this.LookAction = this.actions.FindAction("Look", false);
		this.JumpAction = this.actions.FindAction("Jump", false);
		this.StopAction = this.actions.FindAction("Stop", false);
		this.TwistLeftAction = this.actions.FindAction("Twist Left", false);
		this.TwistRightAction = this.actions.FindAction("Twist Right", false);
		this.DashLeftAction = this.actions.FindAction("Dash Left", false);
		this.DashRightAction = this.actions.FindAction("Dash Right", false);
		this.ExtendLeftAction = this.actions.FindAction("Extend Left", false);
		this.ExtendRightAction = this.actions.FindAction("Extend Right", false);
		this.LateralLeftAction = this.actions.FindAction("Lateral Left", false);
		this.LateralRightAction = this.actions.FindAction("Lateral Right", false);
		this.TalkAction = this.actions.FindAction("Talk", false);
		this.AllChatAction = this.actions.FindAction("All Chat", false);
		this.TeamChatAction = this.actions.FindAction("Team Chat", false);
		this.PauseAction = this.actions.FindAction("Pause", false);
		this.PositionSelectAction = this.actions.FindAction("Position Select", false);
		this.ScoreboardAction = this.actions.FindAction("Scoreboard", false);
		this.QuickChat1Action = this.actions.FindAction("Quick Chat 1", false);
		this.QuickChat2Action = this.actions.FindAction("Quick Chat 2", false);
		this.QuickChat3Action = this.actions.FindAction("Quick Chat 3", false);
		this.QuickChat4Action = this.actions.FindAction("Quick Chat 4", false);
		this.DebugAction = this.actions.FindAction("Debug", false);
		this.DebugInputsAction = this.actions.FindAction("Debug Inputs", false);
		this.DebugTackleAction = this.actions.FindAction("Debug Tackle", false);
		this.DebugGameStateAction = this.actions.FindAction("Debug Game State", false);
		this.DebugShootAction = this.actions.FindAction("Debug Shoot", false);
		this.PointAction = this.actions.FindAction("Point", false);
		this.ClickAction = this.actions.FindAction("Click", false);
		this.InputActions = new Dictionary<string, InputAction>
		{
			{
				"Move Forward",
				this.MoveForwardAction
			},
			{
				"Move Backward",
				this.MoveBackwardAction
			},
			{
				"Turn Left",
				this.TurnLeftAction
			},
			{
				"Turn Right",
				this.TurnRightAction
			},
			{
				"Stick",
				this.StickAction
			},
			{
				"Blade Angle Up",
				this.BladeAngleUpAction
			},
			{
				"Blade Angle Down",
				this.BladeAngleDownAction
			},
			{
				"Slide",
				this.SlideAction
			},
			{
				"Sprint",
				this.SprintAction
			},
			{
				"Track",
				this.TrackAction
			},
			{
				"Look",
				this.LookAction
			},
			{
				"Jump",
				this.JumpAction
			},
			{
				"Stop",
				this.StopAction
			},
			{
				"Twist Left",
				this.TwistLeftAction
			},
			{
				"Twist Right",
				this.TwistRightAction
			},
			{
				"Dash Left",
				this.DashLeftAction
			},
			{
				"Dash Right",
				this.DashRightAction
			},
			{
				"Extend Left",
				this.ExtendLeftAction
			},
			{
				"Extend Right",
				this.ExtendRightAction
			},
			{
				"Lateral Left",
				this.LateralLeftAction
			},
			{
				"Lateral Right",
				this.LateralRightAction
			},
			{
				"Talk",
				this.TalkAction
			},
			{
				"All Chat",
				this.AllChatAction
			},
			{
				"Team Chat",
				this.TeamChatAction
			},
			{
				"Pause",
				this.PauseAction
			},
			{
				"Position Select",
				this.PositionSelectAction
			},
			{
				"Scoreboard",
				this.ScoreboardAction
			},
			{
				"Quick Chat 1",
				this.QuickChat1Action
			},
			{
				"Quick Chat 2",
				this.QuickChat2Action
			},
			{
				"Quick Chat 3",
				this.QuickChat3Action
			},
			{
				"Quick Chat 4",
				this.QuickChat4Action
			},
			{
				"Debug",
				this.DebugAction
			},
			{
				"Debug Inputs",
				this.DebugInputsAction
			},
			{
				"Debug Tackle",
				this.DebugTackleAction
			},
			{
				"Debug Game State",
				this.DebugGameStateAction
			},
			{
				"Debug Shoot",
				this.DebugShootAction
			},
			{
				"Point",
				this.PointAction
			},
			{
				"Click",
				this.ClickAction
			}
		};
		this.RebindableInputActions = new Dictionary<string, InputAction>
		{
			{
				"Move Forward",
				this.MoveForwardAction
			},
			{
				"Move Backward",
				this.MoveBackwardAction
			},
			{
				"Turn Left",
				this.TurnLeftAction
			},
			{
				"Turn Right",
				this.TurnRightAction
			},
			{
				"Stick",
				this.StickAction
			},
			{
				"Blade Angle Up",
				this.BladeAngleUpAction
			},
			{
				"Blade Angle Down",
				this.BladeAngleDownAction
			},
			{
				"Slide",
				this.SlideAction
			},
			{
				"Sprint",
				this.SprintAction
			},
			{
				"Track",
				this.TrackAction
			},
			{
				"Look",
				this.LookAction
			},
			{
				"Jump",
				this.JumpAction
			},
			{
				"Stop",
				this.StopAction
			},
			{
				"Twist Left",
				this.TwistLeftAction
			},
			{
				"Twist Right",
				this.TwistRightAction
			},
			{
				"Dash Left",
				this.DashLeftAction
			},
			{
				"Dash Right",
				this.DashRightAction
			},
			{
				"Extend Left",
				this.ExtendLeftAction
			},
			{
				"Extend Right",
				this.ExtendRightAction
			},
			{
				"Lateral Left",
				this.LateralLeftAction
			},
			{
				"Lateral Right",
				this.LateralRightAction
			},
			{
				"Talk",
				this.TalkAction
			},
			{
				"All Chat",
				this.AllChatAction
			},
			{
				"Team Chat",
				this.TeamChatAction
			},
			{
				"Position Select",
				this.PositionSelectAction
			},
			{
				"Scoreboard",
				this.ScoreboardAction
			}
		};
		this.Reset();
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x00014DF8 File Offset: 0x00012FF8
	public void LoadKeyBinds()
	{
		if (Application.isBatchMode)
		{
			return;
		}
		string @string = PlayerPrefs.GetString("keyBinds", null);
		if (string.IsNullOrEmpty(@string))
		{
			Debug.Log("[InputManager] No keybinds founds during loading, saving default keybinds");
			this.SaveKeyBinds();
			return;
		}
		this.KeyBinds = JsonSerializer.Deserialize<Dictionary<string, KeyBind>>(@string, null);
		Debug.Log("[InputManager] Loading keybinds: " + @string);
		bool flag = false;
		foreach (KeyValuePair<string, InputAction> keyValuePair in this.RebindableInputActions)
		{
			if (this.KeyBinds.ContainsKey(keyValuePair.Key))
			{
				KeyBind keyBind = this.KeyBinds[keyValuePair.Key];
				this.RebindButton(keyValuePair.Key, keyBind.ModifierPath, keyBind.Path);
				this.SetActionInteractions(keyValuePair.Key, keyBind.Interactions);
			}
			else
			{
				flag = true;
			}
		}
		if (flag)
		{
			Debug.Log("[InputManager] Some keybinds were missing, saving default keybinds");
			this.SaveKeyBinds();
			return;
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnKeyBindsLoaded", new Dictionary<string, object>
		{
			{
				"keyBinds",
				this.KeyBinds
			}
		});
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x00014F24 File Offset: 0x00013124
	public void SaveKeyBinds()
	{
		if (Application.isBatchMode)
		{
			return;
		}
		Dictionary<string, KeyBind> dictionary = new Dictionary<string, KeyBind>();
		foreach (KeyValuePair<string, InputAction> keyValuePair in this.RebindableInputActions)
		{
			if (keyValuePair.Value.bindings[0].isComposite)
			{
				dictionary.Add(keyValuePair.Key, new KeyBind
				{
					ModifierPath = keyValuePair.Value.bindings[1].effectivePath,
					Path = keyValuePair.Value.bindings[2].effectivePath,
					Interactions = keyValuePair.Value.bindings[0].effectiveInteractions
				});
			}
			else
			{
				dictionary.Add(keyValuePair.Key, new KeyBind
				{
					ModifierPath = null,
					Path = keyValuePair.Value.bindings[0].effectivePath,
					Interactions = keyValuePair.Value.bindings[0].effectiveInteractions
				});
			}
		}
		string value = JsonSerializer.Serialize<Dictionary<string, KeyBind>>(dictionary, new JsonSerializerOptions
		{
			WriteIndented = true
		});
		PlayerPrefs.SetString("keyBinds", value);
		Debug.Log("[InputManager] Saved key binds, reloading...");
		this.LoadKeyBinds();
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x000150C8 File Offset: 0x000132C8
	public void ResetToDefault()
	{
		PlayerPrefs.DeleteKey("keyBinds");
		foreach (KeyValuePair<string, InputAction> keyValuePair in this.RebindableInputActions)
		{
			keyValuePair.Value.RemoveAllBindingOverrides();
		}
		this.LoadKeyBinds();
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x00015130 File Offset: 0x00013330
	public void RebindButtonInteractively(string actionName)
	{
		InputManager.<>c__DisplayClass47_0 CS$<>8__locals1 = new InputManager.<>c__DisplayClass47_0();
		CS$<>8__locals1.actionName = actionName;
		CS$<>8__locals1.<>4__this = this;
		if (!this.RebindableInputActions.ContainsKey(CS$<>8__locals1.actionName))
		{
			return;
		}
		CS$<>8__locals1.inputAction = this.actions.FindAction(CS$<>8__locals1.actionName, false);
		CS$<>8__locals1.inputAction.Disable();
		CS$<>8__locals1.interactions = CS$<>8__locals1.inputAction.bindings[0].effectiveInteractions;
		InputActionRebindingExtensions.RebindingOperation rebindingOperation = CS$<>8__locals1.<RebindButtonInteractively>g__GenerateRebindingOperation|0();
		Debug.Log("[InputManager] Rebinding " + CS$<>8__locals1.actionName + "...");
		if (CS$<>8__locals1.inputAction.bindings[0].isComposite)
		{
			rebindingOperation.WithTargetBinding(1).OnComplete(delegate(InputActionRebindingExtensions.RebindingOperation operation)
			{
				Debug.Log("[InputManager] Rebound " + CS$<>8__locals1.actionName + " modifierPath to " + operation.action.bindings[1].effectivePath);
				InputActionRebindingExtensions.RebindingOperation rebindingOperation2 = base.<RebindButtonInteractively>g__GenerateRebindingOperation|0().WithControlsExcluding(operation.action.bindings[1].effectivePath).WithTargetBinding(2).WithTimeout(0.5f);
				Action<InputActionRebindingExtensions.RebindingOperation> callback;
				if ((callback = CS$<>8__locals1.<>9__5) == null)
				{
					callback = (CS$<>8__locals1.<>9__5 = delegate(InputActionRebindingExtensions.RebindingOperation operation)
					{
						Debug.Log("[InputManager] Rebound " + CS$<>8__locals1.actionName + " path to " + operation.action.bindings[2].effectivePath);
						CS$<>8__locals1.inputAction.Enable();
						CS$<>8__locals1.<>4__this.RebindButton(CS$<>8__locals1.actionName, operation.action.bindings[1].effectivePath, operation.action.bindings[2].effectivePath);
						CS$<>8__locals1.<>4__this.SetActionInteractions(CS$<>8__locals1.actionName, CS$<>8__locals1.interactions);
						CS$<>8__locals1.<>4__this.SaveKeyBinds();
						MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnKeyBindRebindComplete", new Dictionary<string, object>
						{
							{
								"actionName",
								CS$<>8__locals1.actionName
							}
						});
					});
				}
				InputActionRebindingExtensions.RebindingOperation rebindingOperation3 = rebindingOperation2.OnComplete(callback);
				Action<InputActionRebindingExtensions.RebindingOperation> callback2;
				if ((callback2 = CS$<>8__locals1.<>9__6) == null)
				{
					callback2 = (CS$<>8__locals1.<>9__6 = delegate(InputActionRebindingExtensions.RebindingOperation operation)
					{
						Debug.Log("[InputManager] Rebinding " + CS$<>8__locals1.actionName + " path was cancelled, using modifier path as path " + operation.action.bindings[1].effectivePath);
						CS$<>8__locals1.inputAction.Enable();
						CS$<>8__locals1.<>4__this.RebindButton(CS$<>8__locals1.actionName, null, operation.action.bindings[1].effectivePath);
						CS$<>8__locals1.<>4__this.SetActionInteractions(CS$<>8__locals1.actionName, CS$<>8__locals1.interactions);
						CS$<>8__locals1.<>4__this.SaveKeyBinds();
						MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnKeyBindRebindComplete", new Dictionary<string, object>
						{
							{
								"actionName",
								CS$<>8__locals1.actionName
							}
						});
					});
				}
				rebindingOperation3.OnCancel(callback2).Start();
			}).OnCancel(delegate(InputActionRebindingExtensions.RebindingOperation operation)
			{
				CS$<>8__locals1.inputAction.Enable();
				MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnKeyBindRebindCancel", new Dictionary<string, object>
				{
					{
						"actionName",
						CS$<>8__locals1.actionName
					}
				});
				Debug.Log("[InputManager] Rebinding " + CS$<>8__locals1.actionName + " was cancelled");
			});
		}
		else
		{
			rebindingOperation.OnComplete(delegate(InputActionRebindingExtensions.RebindingOperation operation)
			{
				CS$<>8__locals1.inputAction.Enable();
				CS$<>8__locals1.<>4__this.RebindButton(CS$<>8__locals1.actionName, null, operation.action.bindings[0].effectivePath);
				CS$<>8__locals1.<>4__this.SetActionInteractions(CS$<>8__locals1.actionName, CS$<>8__locals1.interactions);
				CS$<>8__locals1.<>4__this.SaveKeyBinds();
				MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnKeyBindRebindComplete", new Dictionary<string, object>
				{
					{
						"actionName",
						CS$<>8__locals1.actionName
					}
				});
				Debug.Log("[InputManager] Rebound " + CS$<>8__locals1.actionName + " to " + operation.action.bindings[0].effectivePath);
			}).OnCancel(delegate(InputActionRebindingExtensions.RebindingOperation operation)
			{
				CS$<>8__locals1.inputAction.Enable();
				MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnKeyBindRebindCancel", new Dictionary<string, object>
				{
					{
						"actionName",
						CS$<>8__locals1.actionName
					}
				});
				Debug.Log("[InputManager] Rebinding " + CS$<>8__locals1.actionName + " was cancelled");
			});
		}
		rebindingOperation.Start();
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnKeyBindRebindStart", new Dictionary<string, object>
		{
			{
				"actionName",
				CS$<>8__locals1.actionName
			},
			{
				"isComposite",
				CS$<>8__locals1.inputAction.bindings[0].isComposite
			}
		});
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x00015294 File Offset: 0x00013494
	public void RebindButton(string actionName, string modifierPath = null, string path = null)
	{
		if (!this.RebindableInputActions.ContainsKey(actionName))
		{
			return;
		}
		InputAction inputAction = this.actions.FindAction(actionName, false);
		if (inputAction.bindings[0].isComposite)
		{
			inputAction.ApplyBindingOverride(1, new InputBinding
			{
				overridePath = modifierPath
			});
			inputAction.ApplyBindingOverride(2, new InputBinding
			{
				overridePath = path
			});
			return;
		}
		inputAction.ApplyBindingOverride(0, new InputBinding
		{
			overridePath = path
		});
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x00015320 File Offset: 0x00013520
	public void SetActionInteractions(string actionName, string interactions)
	{
		if (!this.RebindableInputActions.ContainsKey(actionName))
		{
			return;
		}
		InputAction inputAction = this.RebindableInputActions[actionName];
		if (inputAction.bindings[0].effectiveInteractions == interactions)
		{
			return;
		}
		inputAction.ApplyBindingOverride(0, new InputBinding
		{
			overrideInteractions = interactions
		});
	}

	// Token: 0x060001DA RID: 474 RVA: 0x00015384 File Offset: 0x00013584
	public void Reset()
	{
		foreach (KeyValuePair<string, InputAction> keyValuePair in this.InputActions)
		{
			keyValuePair.Value.Reset();
			keyValuePair.Value.Disable();
			keyValuePair.Value.Enable();
		}
		if (!Application.isEditor)
		{
			this.DebugInputsAction.Disable();
			this.DebugTackleAction.Disable();
			this.DebugGameStateAction.Disable();
			this.DebugShootAction.Disable();
		}
	}

	// Token: 0x040000F5 RID: 245
	private UnityEngine.InputSystem.PlayerInput playerInput;

	// Token: 0x040000F6 RID: 246
	private InputActionAsset actions;

	// Token: 0x040000F7 RID: 247
	[HideInInspector]
	public InputAction MoveForwardAction;

	// Token: 0x040000F8 RID: 248
	[HideInInspector]
	public InputAction MoveBackwardAction;

	// Token: 0x040000F9 RID: 249
	[HideInInspector]
	public InputAction TurnLeftAction;

	// Token: 0x040000FA RID: 250
	[HideInInspector]
	public InputAction TurnRightAction;

	// Token: 0x040000FB RID: 251
	[HideInInspector]
	public InputAction StickAction;

	// Token: 0x040000FC RID: 252
	[HideInInspector]
	public InputAction BladeAngleUpAction;

	// Token: 0x040000FD RID: 253
	[HideInInspector]
	public InputAction BladeAngleDownAction;

	// Token: 0x040000FE RID: 254
	[HideInInspector]
	public InputAction SlideAction;

	// Token: 0x040000FF RID: 255
	[HideInInspector]
	public InputAction SprintAction;

	// Token: 0x04000100 RID: 256
	[HideInInspector]
	public InputAction TrackAction;

	// Token: 0x04000101 RID: 257
	[HideInInspector]
	public InputAction LookAction;

	// Token: 0x04000102 RID: 258
	[HideInInspector]
	public InputAction JumpAction;

	// Token: 0x04000103 RID: 259
	[HideInInspector]
	public InputAction StopAction;

	// Token: 0x04000104 RID: 260
	[HideInInspector]
	public InputAction TwistLeftAction;

	// Token: 0x04000105 RID: 261
	[HideInInspector]
	public InputAction TwistRightAction;

	// Token: 0x04000106 RID: 262
	[HideInInspector]
	public InputAction DashLeftAction;

	// Token: 0x04000107 RID: 263
	[HideInInspector]
	public InputAction DashRightAction;

	// Token: 0x04000108 RID: 264
	[HideInInspector]
	public InputAction ExtendLeftAction;

	// Token: 0x04000109 RID: 265
	[HideInInspector]
	public InputAction ExtendRightAction;

	// Token: 0x0400010A RID: 266
	[HideInInspector]
	public InputAction LateralLeftAction;

	// Token: 0x0400010B RID: 267
	[HideInInspector]
	public InputAction LateralRightAction;

	// Token: 0x0400010C RID: 268
	[HideInInspector]
	public InputAction TalkAction;

	// Token: 0x0400010D RID: 269
	[HideInInspector]
	public InputAction AllChatAction;

	// Token: 0x0400010E RID: 270
	[HideInInspector]
	public InputAction TeamChatAction;

	// Token: 0x0400010F RID: 271
	[HideInInspector]
	public InputAction PauseAction;

	// Token: 0x04000110 RID: 272
	[HideInInspector]
	public InputAction PositionSelectAction;

	// Token: 0x04000111 RID: 273
	[HideInInspector]
	public InputAction ScoreboardAction;

	// Token: 0x04000112 RID: 274
	[HideInInspector]
	public InputAction QuickChat1Action;

	// Token: 0x04000113 RID: 275
	[HideInInspector]
	public InputAction QuickChat2Action;

	// Token: 0x04000114 RID: 276
	[HideInInspector]
	public InputAction QuickChat3Action;

	// Token: 0x04000115 RID: 277
	[HideInInspector]
	public InputAction QuickChat4Action;

	// Token: 0x04000116 RID: 278
	[HideInInspector]
	public InputAction DebugAction;

	// Token: 0x04000117 RID: 279
	[HideInInspector]
	public InputAction DebugInputsAction;

	// Token: 0x04000118 RID: 280
	[HideInInspector]
	public InputAction DebugTackleAction;

	// Token: 0x04000119 RID: 281
	[HideInInspector]
	public InputAction DebugGameStateAction;

	// Token: 0x0400011A RID: 282
	[HideInInspector]
	public InputAction DebugShootAction;

	// Token: 0x0400011B RID: 283
	[HideInInspector]
	public InputAction PointAction;

	// Token: 0x0400011C RID: 284
	[HideInInspector]
	public InputAction ClickAction;

	// Token: 0x0400011D RID: 285
	public Dictionary<string, InputAction> InputActions = new Dictionary<string, InputAction>();

	// Token: 0x0400011E RID: 286
	public Dictionary<string, InputAction> RebindableInputActions = new Dictionary<string, InputAction>();

	// Token: 0x0400011F RID: 287
	public Dictionary<string, KeyBind> KeyBinds = new Dictionary<string, KeyBind>();
}
