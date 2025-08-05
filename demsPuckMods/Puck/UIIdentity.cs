using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UIElements;

// Token: 0x0200010E RID: 270
public class UIIdentity : UIComponent<UIIdentity>
{
	// Token: 0x06000973 RID: 2419 RVA: 0x0000CED7 File Offset: 0x0000B0D7
	private void Start()
	{
		base.VisibilityRequiresMouse = true;
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x00039734 File Offset: 0x00037934
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("IdentityContainer", null);
		this.confirmButton = this.container.Query("ConfirmButton", null);
		this.confirmButton.clicked += this.OnClickConfirm;
		this.closeButton = this.container.Query("CloseButton", null);
		this.closeButton.clicked += this.OnClickClose;
		this.nameTextField = this.container.Query("NameTextField", null).First().Query("TextField", null);
		this.nameTextField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnNameChanged));
		this.nameTextField.RegisterCallback<FocusOutEvent>(new EventCallback<FocusOutEvent>(this.OnNameFocusOut), TrickleDown.NoTrickleDown);
		this.numberTextField = this.container.Query("NumberTextField", null).First().Query("TextField", null);
		this.numberTextField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnNumberChanged));
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x00039864 File Offset: 0x00037A64
	public void ApplyIdentityValues()
	{
		this.nameTextField.value = MonoBehaviourSingleton<StateManager>.Instance.PlayerData.username;
		this.numberTextField.value = MonoBehaviourSingleton<StateManager>.Instance.PlayerData.number.ToString();
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x0000CEE0 File Offset: 0x0000B0E0
	public override void Show()
	{
		base.Show();
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnIdentityShow", null);
	}

	// Token: 0x06000977 RID: 2423 RVA: 0x0000CEF8 File Offset: 0x0000B0F8
	public override void Hide(bool ignoreAlwaysVisible = false)
	{
		base.Hide(ignoreAlwaysVisible);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnIdentityHide", null);
	}

	// Token: 0x06000978 RID: 2424 RVA: 0x000398B0 File Offset: 0x00037AB0
	private void ResetName()
	{
		this.username = "PLAYER";
		this.nameTextField.value = "PLAYER";
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnIdentityNameChanged", new Dictionary<string, object>
		{
			{
				"value",
				this.username
			}
		});
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x00039900 File Offset: 0x00037B00
	private void OnNameChanged(ChangeEvent<string> changeEvent)
	{
		this.username = changeEvent.newValue;
		if (!string.IsNullOrEmpty(this.username))
		{
			this.username = Utils.FilterStringSpecialCharacters(this.username);
			this.username = Utils.FilterStringNotLetters(this.username);
			this.nameTextField.value = this.username;
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnIdentityNameChanged", new Dictionary<string, object>
		{
			{
				"value",
				this.username
			}
		});
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x00039980 File Offset: 0x00037B80
	private void OnNameFocusOut(FocusOutEvent focusOutEvent)
	{
		if (string.IsNullOrEmpty(this.username))
		{
			this.ResetName();
			return;
		}
		this.username = Utils.FilterStringSpecialCharacters(this.username);
		this.username = Utils.FilterStringNotLetters(this.username);
		this.username = Utils.FilterStringProfanity(this.username, false);
		if (string.IsNullOrEmpty(this.username))
		{
			this.ResetName();
			return;
		}
		this.nameTextField.value = this.username;
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnIdentityNameChanged", new Dictionary<string, object>
		{
			{
				"value",
				this.username
			}
		});
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x00039A20 File Offset: 0x00037C20
	private void OnNumberChanged(ChangeEvent<string> changeEvent)
	{
		string text = new Regex("[^0-9]").Replace(changeEvent.newValue, "");
		this.numberTextField.value = text;
		int num;
		if (int.TryParse(text, out num))
		{
			this.number = num;
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnIdentityNumberChanged", new Dictionary<string, object>
			{
				{
					"value",
					this.number
				}
			});
		}
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x0000CF11 File Offset: 0x0000B111
	private void OnClickClose()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnIdentityClickClose", null);
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x0000CF23 File Offset: 0x0000B123
	private void OnClickConfirm()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnIdentityClickConfirm", null);
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x00039A90 File Offset: 0x00037C90
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x0000CF3D File Offset: 0x0000B13D
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x0000CF47 File Offset: 0x0000B147
	protected internal override string __getTypeName()
	{
		return "UIIdentity";
	}

	// Token: 0x040005AD RID: 1453
	private TextField nameTextField;

	// Token: 0x040005AE RID: 1454
	private TextField numberTextField;

	// Token: 0x040005AF RID: 1455
	private Button confirmButton;

	// Token: 0x040005B0 RID: 1456
	private Button closeButton;

	// Token: 0x040005B1 RID: 1457
	private string username;

	// Token: 0x040005B2 RID: 1458
	private int number;
}
