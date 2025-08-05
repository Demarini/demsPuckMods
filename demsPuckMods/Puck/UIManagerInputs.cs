using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x020000C2 RID: 194
public class UIManagerInputs : MonoBehaviour
{
	// Token: 0x060005D3 RID: 1491 RVA: 0x0000A93F File Offset: 0x00008B3F
	private void Awake()
	{
		this.uiManager = base.GetComponent<UIManager>();
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x00023EE8 File Offset: 0x000220E8
	private void Start()
	{
		MonoBehaviourSingleton<InputManager>.Instance.PauseAction.performed += this.OnPauseActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.AllChatAction.canceled += this.OnAllChatActionCanceled;
		MonoBehaviourSingleton<InputManager>.Instance.TeamChatAction.canceled += this.OnTeamChatActionCanceled;
		MonoBehaviourSingleton<InputManager>.Instance.PositionSelectAction.performed += this.OnPositionSelectActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.ScoreboardAction.started += this.OnScoreboardActionStarted;
		MonoBehaviourSingleton<InputManager>.Instance.ScoreboardAction.canceled += this.OnScoreboardActionCanceled;
		MonoBehaviourSingleton<InputManager>.Instance.QuickChat1Action.performed += this.OnQuickChatAction1Performed;
		MonoBehaviourSingleton<InputManager>.Instance.QuickChat2Action.performed += this.OnQuickChatAction2Performed;
		MonoBehaviourSingleton<InputManager>.Instance.QuickChat3Action.performed += this.OnQuickChatAction3Performed;
		MonoBehaviourSingleton<InputManager>.Instance.QuickChat4Action.performed += this.OnQuickChatAction4Performed;
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x00024004 File Offset: 0x00022204
	private void OnDestroy()
	{
		MonoBehaviourSingleton<InputManager>.Instance.PauseAction.performed -= this.OnPauseActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.AllChatAction.canceled -= this.OnAllChatActionCanceled;
		MonoBehaviourSingleton<InputManager>.Instance.TeamChatAction.canceled -= this.OnTeamChatActionCanceled;
		MonoBehaviourSingleton<InputManager>.Instance.PositionSelectAction.performed -= this.OnPositionSelectActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.ScoreboardAction.started -= this.OnScoreboardActionStarted;
		MonoBehaviourSingleton<InputManager>.Instance.ScoreboardAction.canceled -= this.OnScoreboardActionCanceled;
		MonoBehaviourSingleton<InputManager>.Instance.QuickChat1Action.performed -= this.OnQuickChatAction1Performed;
		MonoBehaviourSingleton<InputManager>.Instance.QuickChat2Action.performed -= this.OnQuickChatAction2Performed;
		MonoBehaviourSingleton<InputManager>.Instance.QuickChat3Action.performed -= this.OnQuickChatAction3Performed;
		MonoBehaviourSingleton<InputManager>.Instance.QuickChat4Action.performed -= this.OnQuickChatAction4Performed;
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x00024120 File Offset: 0x00022320
	private void OnPauseActionPerformed(InputAction.CallbackContext context)
	{
		if (this.uiManager.UIState != UIState.Play)
		{
			return;
		}
		if (this.uiManager.Chat.IsFocused)
		{
			return;
		}
		if (this.uiManager.Chat.IsQuickChatOpen)
		{
			return;
		}
		if (this.uiManager.Settings.IsVisible)
		{
			return;
		}
		this.uiManager.PauseMenu.Toggle();
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x00024188 File Offset: 0x00022388
	private void OnAllChatActionCanceled(InputAction.CallbackContext context)
	{
		if (this.uiManager.UIState != UIState.Play)
		{
			return;
		}
		if (this.uiManager.PauseMenu.IsVisible)
		{
			return;
		}
		if (this.uiManager.Chat.IsFocused)
		{
			return;
		}
		if (this.uiManager.Chat.IsQuickChatOpen)
		{
			return;
		}
		if (this.uiManager.Settings.IsVisible)
		{
			return;
		}
		this.uiManager.Chat.Focus();
		this.uiManager.Chat.UseTeamChat = false;
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x00024214 File Offset: 0x00022414
	private void OnTeamChatActionCanceled(InputAction.CallbackContext context)
	{
		if (this.uiManager.UIState != UIState.Play)
		{
			return;
		}
		if (this.uiManager.PauseMenu.IsVisible)
		{
			return;
		}
		if (this.uiManager.Chat.IsFocused)
		{
			return;
		}
		if (this.uiManager.Chat.IsQuickChatOpen)
		{
			return;
		}
		if (this.uiManager.Settings.IsVisible)
		{
			return;
		}
		this.uiManager.Chat.Focus();
		this.uiManager.Chat.UseTeamChat = true;
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x000242A0 File Offset: 0x000224A0
	private void OnPositionSelectActionPerformed(InputAction.CallbackContext context)
	{
		if (this.uiManager.UIState != UIState.Play)
		{
			return;
		}
		if (this.uiManager.PauseMenu.IsVisible)
		{
			return;
		}
		if (this.uiManager.Chat.IsFocused)
		{
			return;
		}
		if (this.uiManager.Chat.IsQuickChatOpen)
		{
			return;
		}
		if (this.uiManager.TeamSelect.IsVisible)
		{
			return;
		}
		if (this.uiManager.PositionSelect.IsVisible)
		{
			return;
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerRequestPositionSelect", null);
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x0002432C File Offset: 0x0002252C
	private void OnScoreboardActionStarted(InputAction.CallbackContext context)
	{
		if (this.uiManager.UIState != UIState.Play)
		{
			return;
		}
		if (this.uiManager.PauseMenu.IsVisible)
		{
			return;
		}
		if (this.uiManager.Chat.IsFocused)
		{
			return;
		}
		if (this.uiManager.Settings.IsVisible)
		{
			return;
		}
		this.uiManager.Scoreboard.Show();
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x0000A94D File Offset: 0x00008B4D
	private void OnScoreboardActionCanceled(InputAction.CallbackContext context)
	{
		this.uiManager.Scoreboard.Hide(false);
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x00024394 File Offset: 0x00022594
	private void OnQuickChatAction1Performed(InputAction.CallbackContext context)
	{
		if (this.uiManager.UIState != UIState.Play)
		{
			return;
		}
		if (this.uiManager.PauseMenu.IsVisible)
		{
			return;
		}
		if (this.uiManager.Chat.IsFocused)
		{
			return;
		}
		if (this.uiManager.Settings.IsVisible)
		{
			return;
		}
		this.uiManager.Chat.OnQuickChat(0);
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x000243FC File Offset: 0x000225FC
	private void OnQuickChatAction2Performed(InputAction.CallbackContext context)
	{
		if (this.uiManager.UIState != UIState.Play)
		{
			return;
		}
		if (this.uiManager.PauseMenu.IsVisible)
		{
			return;
		}
		if (this.uiManager.Chat.IsFocused)
		{
			return;
		}
		if (this.uiManager.Settings.IsVisible)
		{
			return;
		}
		this.uiManager.Chat.OnQuickChat(1);
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x00024464 File Offset: 0x00022664
	private void OnQuickChatAction3Performed(InputAction.CallbackContext context)
	{
		if (this.uiManager.UIState != UIState.Play)
		{
			return;
		}
		if (this.uiManager.PauseMenu.IsVisible)
		{
			return;
		}
		if (this.uiManager.Chat.IsFocused)
		{
			return;
		}
		if (this.uiManager.Settings.IsVisible)
		{
			return;
		}
		this.uiManager.Chat.OnQuickChat(2);
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x000244CC File Offset: 0x000226CC
	private void OnQuickChatAction4Performed(InputAction.CallbackContext context)
	{
		if (this.uiManager.UIState != UIState.Play)
		{
			return;
		}
		if (this.uiManager.PauseMenu.IsVisible)
		{
			return;
		}
		if (this.uiManager.Chat.IsFocused)
		{
			return;
		}
		if (this.uiManager.Settings.IsVisible)
		{
			return;
		}
		this.uiManager.Chat.OnQuickChat(3);
	}

	// Token: 0x04000339 RID: 825
	private UIManager uiManager;
}
