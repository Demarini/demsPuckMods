using System;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x020000B0 RID: 176
public static class InputManager
{
	// Token: 0x06000578 RID: 1400 RVA: 0x0001DAA4 File Offset: 0x0001BCA4
	public static void Initialize()
	{
		InputSystem.RegisterInteraction<DoublePressInteraction>(null);
		InputSystem.RegisterInteraction<ToggleInteraction>(null);
		InputActionAsset actions = InputSystem.actions;
		InputManager.MoveForwardAction = actions.FindAction("Move Forward", false);
		InputManager.MoveBackwardAction = actions.FindAction("Move Backward", false);
		InputManager.TurnLeftAction = actions.FindAction("Turn Left", false);
		InputManager.TurnRightAction = actions.FindAction("Turn Right", false);
		InputManager.StickAction = actions.FindAction("Stick", false);
		InputManager.BladeAngleUpAction = actions.FindAction("Blade Angle Up", false);
		InputManager.BladeAngleDownAction = actions.FindAction("Blade Angle Down", false);
		InputManager.SlideAction = actions.FindAction("Slide", false);
		InputManager.SprintAction = actions.FindAction("Sprint", false);
		InputManager.TrackAction = actions.FindAction("Track", false);
		InputManager.LookAction = actions.FindAction("Look", false);
		InputManager.JumpAction = actions.FindAction("Jump", false);
		InputManager.StopAction = actions.FindAction("Stop", false);
		InputManager.TwistLeftAction = actions.FindAction("Twist Left", false);
		InputManager.TwistRightAction = actions.FindAction("Twist Right", false);
		InputManager.DashLeftAction = actions.FindAction("Dash Left", false);
		InputManager.DashRightAction = actions.FindAction("Dash Right", false);
		InputManager.ExtendLeftAction = actions.FindAction("Extend Left", false);
		InputManager.ExtendRightAction = actions.FindAction("Extend Right", false);
		InputManager.LateralLeftAction = actions.FindAction("Lateral Left", false);
		InputManager.LateralRightAction = actions.FindAction("Lateral Right", false);
		InputManager.TalkAction = actions.FindAction("Talk", false);
		InputManager.AllChatAction = actions.FindAction("All Chat", false);
		InputManager.TeamChatAction = actions.FindAction("Team Chat", false);
		InputManager.PauseAction = actions.FindAction("Pause", false);
		InputManager.PositionSelectAction = actions.FindAction("Position Select", false);
		InputManager.ScoreboardAction = actions.FindAction("Scoreboard", false);
		InputManager.QuickChat1Action = actions.FindAction("Quick Chat 1", false);
		InputManager.QuickChat2Action = actions.FindAction("Quick Chat 2", false);
		InputManager.QuickChat3Action = actions.FindAction("Quick Chat 3", false);
		InputManager.QuickChat4Action = actions.FindAction("Quick Chat 4", false);
		InputManager.QuickChat5Action = actions.FindAction("Quick Chat 5", false);
		InputManager.Debug1Action = actions.FindAction("Debug 1", false);
		InputManager.Debug2Action = actions.FindAction("Debug 2", false);
		InputManager.Debug3Action = actions.FindAction("Debug 3", false);
		InputManager.Debug4Action = actions.FindAction("Debug 4", false);
		InputManager.PointAction = actions.FindAction("Point", false);
		InputManager.ClickAction = actions.FindAction("Click", false);
		InputManager.InputActions = new List<InputAction>
		{
			InputManager.MoveForwardAction,
			InputManager.MoveBackwardAction,
			InputManager.TurnLeftAction,
			InputManager.TurnRightAction,
			InputManager.StickAction,
			InputManager.BladeAngleUpAction,
			InputManager.BladeAngleDownAction,
			InputManager.SlideAction,
			InputManager.SprintAction,
			InputManager.TrackAction,
			InputManager.LookAction,
			InputManager.JumpAction,
			InputManager.StopAction,
			InputManager.TwistLeftAction,
			InputManager.TwistRightAction,
			InputManager.DashLeftAction,
			InputManager.DashRightAction,
			InputManager.ExtendLeftAction,
			InputManager.ExtendRightAction,
			InputManager.LateralLeftAction,
			InputManager.LateralRightAction,
			InputManager.TalkAction,
			InputManager.AllChatAction,
			InputManager.TeamChatAction,
			InputManager.PauseAction,
			InputManager.PositionSelectAction,
			InputManager.ScoreboardAction,
			InputManager.QuickChat1Action,
			InputManager.QuickChat2Action,
			InputManager.QuickChat3Action,
			InputManager.QuickChat4Action,
			InputManager.QuickChat5Action,
			InputManager.Debug1Action,
			InputManager.Debug2Action,
			InputManager.Debug3Action,
			InputManager.Debug4Action,
			InputManager.PointAction,
			InputManager.ClickAction
		};
		InputManager.RebindableInputActions = new List<InputAction>
		{
			InputManager.MoveForwardAction,
			InputManager.MoveBackwardAction,
			InputManager.TurnLeftAction,
			InputManager.TurnRightAction,
			InputManager.BladeAngleUpAction,
			InputManager.BladeAngleDownAction,
			InputManager.SlideAction,
			InputManager.SprintAction,
			InputManager.TrackAction,
			InputManager.LookAction,
			InputManager.JumpAction,
			InputManager.StopAction,
			InputManager.TwistLeftAction,
			InputManager.TwistRightAction,
			InputManager.DashLeftAction,
			InputManager.DashRightAction,
			InputManager.ExtendLeftAction,
			InputManager.ExtendRightAction,
			InputManager.LateralLeftAction,
			InputManager.LateralRightAction,
			InputManager.TalkAction,
			InputManager.AllChatAction,
			InputManager.TeamChatAction,
			InputManager.PositionSelectAction,
			InputManager.ScoreboardAction
		};
		InputManagerController.Initialize();
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x0001E015 File Offset: 0x0001C215
	public static void Dispose()
	{
		InputManagerController.Dispose();
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x0001E01C File Offset: 0x0001C21C
	public static void LoadKeyBinds()
	{
		try
		{
			string @string = SaveManager.GetString("keyBinds", null);
			if (string.IsNullOrEmpty(@string))
			{
				throw new Exception("No saved key binds found");
			}
			Dictionary<string, KeyBind> dictionary = JsonSerializer.Deserialize<Dictionary<string, KeyBind>>(@string, null);
			List<string> list = new List<string>();
			foreach (InputAction inputAction in InputManager.RebindableInputActions)
			{
				if (!dictionary.ContainsKey(inputAction.name))
				{
					list.Add(inputAction.name);
				}
			}
			if (list.Count > 0)
			{
				throw new Exception("Missing keys in loaded key binds (" + string.Join(", ", list) + ")");
			}
			InputManager.KeyBinds.Clear();
			foreach (KeyValuePair<string, KeyBind> keyValuePair in dictionary)
			{
				string actionName = keyValuePair.Key;
				KeyBind value = keyValuePair.Value;
				InputAction inputAction2 = InputManager.RebindableInputActions.Find((InputAction action) => action.name == actionName);
				value.InputAction = inputAction2;
				if (value.InputAction == null)
				{
					Debug.LogWarning("[InputManager] Cannot load key bind for " + actionName + " because it is not rebindable");
				}
				else
				{
					InputManager.KeyBinds.Add(actionName, value);
					InputManager.ApplyKeyBind(value);
				}
			}
			Debug.Log(string.Format("[InputManager] Loaded {0} key binds: {1}", InputManager.KeyBinds.Count, @string));
			EventManager.TriggerEvent("Event_OnKeyBindsLoaded", new Dictionary<string, object>
			{
				{
					"keyBinds",
					InputManager.KeyBinds
				}
			});
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[InputManager] Failed to load key binds: " + ex.Message);
			InputManager.SaveKeyBinds();
			InputManager.LoadKeyBinds();
		}
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x0001E238 File Offset: 0x0001C438
	public static void ApplyKeyBinds()
	{
		foreach (KeyBind keyBind in InputManager.KeyBinds.Values)
		{
			InputManager.ApplyKeyBind(keyBind);
		}
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x0001E28C File Offset: 0x0001C48C
	public static void ApplyKeyBind(KeyBind keyBind)
	{
		string name = keyBind.InputAction.name;
		InputManager.RebindAction(name, keyBind.ModifierPath, keyBind.Path);
		InputManager.SetActionInteractions(name, keyBind.Interactions);
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x0001E2B8 File Offset: 0x0001C4B8
	public static void SaveKeyBinds()
	{
		try
		{
			foreach (InputAction inputAction in InputManager.RebindableInputActions)
			{
				if (!InputManager.KeyBinds.ContainsKey(inputAction.name))
				{
					KeyBind value = new KeyBind(inputAction);
					InputManager.KeyBinds.Add(inputAction.name, value);
				}
				else
				{
					InputManager.KeyBinds[inputAction.name].Update(inputAction);
				}
			}
			string text = JsonSerializer.Serialize<Dictionary<string, KeyBind>>(InputManager.KeyBinds, new JsonSerializerOptions
			{
				WriteIndented = true
			});
			SaveManager.SetString("keyBinds", text);
			Debug.Log(string.Format("[InputManager] Saved {0} key binds: {1}", InputManager.KeyBinds.Count, text));
			EventManager.TriggerEvent("Event_OnKeyBindsSaved", new Dictionary<string, object>
			{
				{
					"keyBinds",
					InputManager.KeyBinds
				}
			});
		}
		catch (Exception ex)
		{
			Debug.LogError("[InputManager] Failed to save key binds: " + ex.Message);
		}
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x0001E3D0 File Offset: 0x0001C5D0
	public static void ResetToDefault()
	{
		Debug.Log("[InputManager] Resetting key binds to default");
		foreach (InputAction action in InputManager.RebindableInputActions)
		{
			action.RemoveAllBindingOverrides();
		}
		InputManager.SaveKeyBinds();
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x0001E430 File Offset: 0x0001C630
	public static void RebindButtonInteractively(string actionName)
	{
		InputManager.<>c__DisplayClass48_0 CS$<>8__locals1 = new InputManager.<>c__DisplayClass48_0();
		CS$<>8__locals1.actionName = actionName;
		CS$<>8__locals1.inputAction = InputManager.RebindableInputActions.Find((InputAction action) => action.name == CS$<>8__locals1.actionName);
		if (CS$<>8__locals1.inputAction == null)
		{
			Debug.LogWarning("[InputManager] Cannot rebind action " + CS$<>8__locals1.actionName + " because it is not rebindable");
			return;
		}
		CS$<>8__locals1.inputAction.Disable();
		InputActionRebindingExtensions.RebindingOperation rebindingOperation = CS$<>8__locals1.<RebindButtonInteractively>g__GenerateRebindingOperation|1();
		CS$<>8__locals1.rebindingOperation = CS$<>8__locals1.<RebindButtonInteractively>g__GenerateRebindingOperation|1();
		bool isComposite = CS$<>8__locals1.inputAction.bindings[0].isComposite;
		CS$<>8__locals1.interactions = CS$<>8__locals1.inputAction.bindings[0].effectiveInteractions;
		Debug.Log("[InputManager] Rebinding " + CS$<>8__locals1.actionName);
		if (isComposite)
		{
			Debug.Log("[InputManager] Rebinding " + CS$<>8__locals1.actionName + " as composite");
			rebindingOperation.WithTargetBinding(1).OnComplete(delegate(InputActionRebindingExtensions.RebindingOperation modifierOperation)
			{
				string modifierPath = modifierOperation.action.bindings[1].effectivePath;
				Debug.Log("[InputManager] Rebound " + CS$<>8__locals1.actionName + " modifierPath to " + modifierPath);
				CS$<>8__locals1.rebindingOperation.WithControlsExcluding(modifierPath).WithTargetBinding(2).WithTimeout(0.5f).OnComplete(delegate(InputActionRebindingExtensions.RebindingOperation operation)
				{
					CS$<>8__locals1.inputAction.Enable();
					string effectivePath = operation.action.bindings[2].effectivePath;
					InputManager.RebindAction(CS$<>8__locals1.actionName, modifierPath, effectivePath);
					InputManager.SetActionInteractions(CS$<>8__locals1.actionName, CS$<>8__locals1.interactions);
					Debug.Log(string.Concat(new string[]
					{
						"[InputManager] Rebound ",
						CS$<>8__locals1.actionName,
						" to ",
						modifierPath,
						" + ",
						effectivePath
					}));
					EventManager.TriggerEvent("Event_OnKeyBindRebindComplete", new Dictionary<string, object>
					{
						{
							"actionName",
							CS$<>8__locals1.actionName
						}
					});
					InputManager.SaveKeyBinds();
				}).OnCancel(delegate(InputActionRebindingExtensions.RebindingOperation operation)
				{
					CS$<>8__locals1.inputAction.Enable();
					InputManager.RebindAction(CS$<>8__locals1.actionName, null, modifierPath);
					InputManager.SetActionInteractions(CS$<>8__locals1.actionName, CS$<>8__locals1.interactions);
					Debug.Log("[InputManager] Rebinding " + CS$<>8__locals1.actionName + " path was cancelled, using modifier path as path " + modifierPath);
					EventManager.TriggerEvent("Event_OnKeyBindRebindComplete", new Dictionary<string, object>
					{
						{
							"actionName",
							CS$<>8__locals1.actionName
						}
					});
					InputManager.SaveKeyBinds();
				}).Start();
			}).OnCancel(delegate(InputActionRebindingExtensions.RebindingOperation operation)
			{
				CS$<>8__locals1.inputAction.Enable();
				Debug.Log("[InputManager] Rebinding " + CS$<>8__locals1.actionName + " was cancelled");
				EventManager.TriggerEvent("Event_OnKeyBindRebindCancel", new Dictionary<string, object>
				{
					{
						"actionName",
						CS$<>8__locals1.actionName
					}
				});
			});
		}
		else
		{
			rebindingOperation.OnComplete(delegate(InputActionRebindingExtensions.RebindingOperation operation)
			{
				CS$<>8__locals1.inputAction.Enable();
				string effectivePath = operation.action.bindings[0].effectivePath;
				InputManager.RebindAction(CS$<>8__locals1.actionName, effectivePath, null);
				InputManager.SetActionInteractions(CS$<>8__locals1.actionName, CS$<>8__locals1.interactions);
				Debug.Log("[InputManager] Rebound " + CS$<>8__locals1.actionName + " to " + effectivePath);
				EventManager.TriggerEvent("Event_OnKeyBindRebindComplete", new Dictionary<string, object>
				{
					{
						"actionName",
						CS$<>8__locals1.actionName
					}
				});
				InputManager.SaveKeyBinds();
			}).OnCancel(delegate(InputActionRebindingExtensions.RebindingOperation operation)
			{
				CS$<>8__locals1.inputAction.Enable();
				Debug.Log("[InputManager] Rebinding " + CS$<>8__locals1.actionName + " was cancelled");
				EventManager.TriggerEvent("Event_OnKeyBindRebindCancel", new Dictionary<string, object>
				{
					{
						"actionName",
						CS$<>8__locals1.actionName
					}
				});
			});
		}
		rebindingOperation.Start();
		EventManager.TriggerEvent("Event_OnKeyBindRebindStart", new Dictionary<string, object>
		{
			{
				"actionName",
				CS$<>8__locals1.actionName
			},
			{
				"isComposite",
				isComposite
			}
		});
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x0001E5A4 File Offset: 0x0001C7A4
	public static void RebindAction(string actionName, string modifierPath = null, string path = null)
	{
		InputAction inputAction = InputManager.RebindableInputActions.Find((InputAction action) => action.name == actionName);
		if (inputAction == null)
		{
			Debug.LogWarning("[InputManager] Cannot rebind action " + actionName + " because it is not rebindable");
			return;
		}
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

	// Token: 0x06000581 RID: 1409 RVA: 0x0001E658 File Offset: 0x0001C858
	public static void SetActionInteractions(string actionName, string interactions)
	{
		InputAction inputAction = InputManager.RebindableInputActions.Find((InputAction action) => action.name == actionName);
		if (inputAction == null)
		{
			Debug.LogWarning("[InputManager] Cannot set interactions for action " + actionName + " because it is not rebindable");
			return;
		}
		inputAction.ApplyBindingOverride(0, new InputBinding
		{
			overrideInteractions = interactions
		});
	}

	// Token: 0x0400035D RID: 861
	public static InputAction MoveForwardAction;

	// Token: 0x0400035E RID: 862
	public static InputAction MoveBackwardAction;

	// Token: 0x0400035F RID: 863
	public static InputAction TurnLeftAction;

	// Token: 0x04000360 RID: 864
	public static InputAction TurnRightAction;

	// Token: 0x04000361 RID: 865
	public static InputAction StickAction;

	// Token: 0x04000362 RID: 866
	public static InputAction BladeAngleUpAction;

	// Token: 0x04000363 RID: 867
	public static InputAction BladeAngleDownAction;

	// Token: 0x04000364 RID: 868
	public static InputAction SlideAction;

	// Token: 0x04000365 RID: 869
	public static InputAction SprintAction;

	// Token: 0x04000366 RID: 870
	public static InputAction TrackAction;

	// Token: 0x04000367 RID: 871
	public static InputAction LookAction;

	// Token: 0x04000368 RID: 872
	public static InputAction JumpAction;

	// Token: 0x04000369 RID: 873
	public static InputAction StopAction;

	// Token: 0x0400036A RID: 874
	public static InputAction TwistLeftAction;

	// Token: 0x0400036B RID: 875
	public static InputAction TwistRightAction;

	// Token: 0x0400036C RID: 876
	public static InputAction DashLeftAction;

	// Token: 0x0400036D RID: 877
	public static InputAction DashRightAction;

	// Token: 0x0400036E RID: 878
	public static InputAction ExtendLeftAction;

	// Token: 0x0400036F RID: 879
	public static InputAction ExtendRightAction;

	// Token: 0x04000370 RID: 880
	public static InputAction LateralLeftAction;

	// Token: 0x04000371 RID: 881
	public static InputAction LateralRightAction;

	// Token: 0x04000372 RID: 882
	public static InputAction TalkAction;

	// Token: 0x04000373 RID: 883
	public static InputAction AllChatAction;

	// Token: 0x04000374 RID: 884
	public static InputAction TeamChatAction;

	// Token: 0x04000375 RID: 885
	public static InputAction PauseAction;

	// Token: 0x04000376 RID: 886
	public static InputAction PositionSelectAction;

	// Token: 0x04000377 RID: 887
	public static InputAction ScoreboardAction;

	// Token: 0x04000378 RID: 888
	public static InputAction QuickChat1Action;

	// Token: 0x04000379 RID: 889
	public static InputAction QuickChat2Action;

	// Token: 0x0400037A RID: 890
	public static InputAction QuickChat3Action;

	// Token: 0x0400037B RID: 891
	public static InputAction QuickChat4Action;

	// Token: 0x0400037C RID: 892
	public static InputAction QuickChat5Action;

	// Token: 0x0400037D RID: 893
	public static InputAction Debug1Action;

	// Token: 0x0400037E RID: 894
	public static InputAction Debug2Action;

	// Token: 0x0400037F RID: 895
	public static InputAction Debug3Action;

	// Token: 0x04000380 RID: 896
	public static InputAction Debug4Action;

	// Token: 0x04000381 RID: 897
	public static InputAction PointAction;

	// Token: 0x04000382 RID: 898
	public static InputAction ClickAction;

	// Token: 0x04000383 RID: 899
	public static Dictionary<string, KeyBind> KeyBinds = new Dictionary<string, KeyBind>();

	// Token: 0x04000384 RID: 900
	public static List<InputAction> InputActions = new List<InputAction>();

	// Token: 0x04000385 RID: 901
	public static List<InputAction> RebindableInputActions = new List<InputAction>();
}
