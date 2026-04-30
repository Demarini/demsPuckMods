using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000199 RID: 409
public class UIIdentity : UIView
{
	// Token: 0x06000B9B RID: 2971 RVA: 0x00036D60 File Offset: 0x00034F60
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("IdentityView", null);
		this.identity = base.View.Query("Identity", null);
		this.closeIconButton = this.identity.Query("CloseIconButtonContainer", null).First().Query(null, null);
		this.closeIconButton.clicked += this.OnClickClose;
		this.confirmButton = this.identity.Query("ConfirmButton", null);
		this.confirmButton.clicked += this.OnClickConfirm;
		this.usernameTextField = this.identity.Query("UsernameTextField", null).First().Query(null, null);
		this.usernameTextField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnNameChanged));
		this.usernameTextField.RegisterCallback<FocusOutEvent>(new EventCallback<FocusOutEvent>(this.OnNameFocusOut), TrickleDown.NoTrickleDown);
		this.numberIntegerField = this.identity.Query("NumberIntegerField", null).First().Query(null, null);
		this.numberIntegerField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<int>>(this.OnNumberChanged));
		this.numberIntegerField.RegisterCallback<FocusOutEvent>(new EventCallback<FocusOutEvent>(this.OnNumberFocusOut), TrickleDown.NoTrickleDown);
	}

	// Token: 0x06000B9C RID: 2972 RVA: 0x00036ECB File Offset: 0x000350CB
	public override bool Show()
	{
		bool flag = base.Show();
		if (flag)
		{
			EventManager.TriggerEvent("Event_OnIdentityShow", null);
		}
		return flag;
	}

	// Token: 0x06000B9D RID: 2973 RVA: 0x00036EE1 File Offset: 0x000350E1
	public override bool Hide()
	{
		bool flag = base.Hide();
		if (flag)
		{
			EventManager.TriggerEvent("Event_OnIdentityHide", null);
		}
		return flag;
	}

	// Token: 0x06000B9E RID: 2974 RVA: 0x00036EF7 File Offset: 0x000350F7
	public void SetIdentity(string username, int number)
	{
		this.username = username;
		this.usernameTextField.value = this.username;
		this.number = number;
		this.numberIntegerField.value = this.number;
	}

	// Token: 0x06000B9F RID: 2975 RVA: 0x00036F29 File Offset: 0x00035129
	private void OnNameChanged(ChangeEvent<string> changeEvent)
	{
		this.username = changeEvent.newValue;
		if (!string.IsNullOrEmpty(this.username))
		{
			this.username = StringUtils.FilterStringNotLetters(this.username);
			this.usernameTextField.value = this.username;
		}
	}

	// Token: 0x06000BA0 RID: 2976 RVA: 0x00036F68 File Offset: 0x00035168
	private void OnNameFocusOut(FocusOutEvent focusOutEvent)
	{
		this.username = this.usernameTextField.value;
		if (!string.IsNullOrEmpty(this.username))
		{
			this.username = StringUtils.FilterStringNotLetters(this.username);
			this.username = StringUtils.FilterStringProfanity(this.username, false);
			this.usernameTextField.value = this.username;
		}
	}

	// Token: 0x06000BA1 RID: 2977 RVA: 0x00036FC7 File Offset: 0x000351C7
	private void OnNumberChanged(ChangeEvent<int> changeEvent)
	{
		this.number = changeEvent.newValue;
	}

	// Token: 0x06000BA2 RID: 2978 RVA: 0x00036FD5 File Offset: 0x000351D5
	private void OnNumberFocusOut(FocusOutEvent focusOutEvent)
	{
		this.number = Mathf.Clamp(this.numberIntegerField.value, 1, 99);
		this.numberIntegerField.value = this.number;
	}

	// Token: 0x06000BA3 RID: 2979 RVA: 0x00037001 File Offset: 0x00035201
	private void OnClickClose()
	{
		EventManager.TriggerEvent("Event_OnIdentityClickClose", null);
	}

	// Token: 0x06000BA4 RID: 2980 RVA: 0x0003700E File Offset: 0x0003520E
	private void OnClickConfirm()
	{
		EventManager.TriggerEvent("Event_OnIdentityClickConfirm", new Dictionary<string, object>
		{
			{
				"username",
				this.username
			},
			{
				"number",
				this.number
			}
		});
	}

	// Token: 0x040006DC RID: 1756
	private VisualElement identity;

	// Token: 0x040006DD RID: 1757
	private TextField usernameTextField;

	// Token: 0x040006DE RID: 1758
	private IntegerField numberIntegerField;

	// Token: 0x040006DF RID: 1759
	private IconButton closeIconButton;

	// Token: 0x040006E0 RID: 1760
	private Button confirmButton;

	// Token: 0x040006E1 RID: 1761
	private string username;

	// Token: 0x040006E2 RID: 1762
	private int number;
}
