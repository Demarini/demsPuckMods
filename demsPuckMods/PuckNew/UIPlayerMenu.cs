using System;
using UnityEngine.UIElements;

// Token: 0x020001B1 RID: 433
public class UIPlayerMenu : UIView
{
	// Token: 0x06000C73 RID: 3187 RVA: 0x0003AB38 File Offset: 0x00038D38
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("PlayerMenuView", null);
		this.playerMenu = base.View.Query("PlayerMenu", null);
		this.identityButton = this.playerMenu.Query("IdentityButton", null);
		this.identityButton.clicked += this.OnClickIdentity;
		this.appearanceButton = this.playerMenu.Query("AppearanceButton", null);
		this.appearanceButton.clicked += this.OnClickAppearance;
		this.backButton = this.playerMenu.Query("BackButton", null);
		this.backButton.clicked += this.OnClickBack;
	}

	// Token: 0x06000C74 RID: 3188 RVA: 0x0003AC11 File Offset: 0x00038E11
	public override bool Show()
	{
		bool flag = base.Show();
		if (flag)
		{
			EventManager.TriggerEvent("Event_OnPlayerMenuShow", null);
		}
		return flag;
	}

	// Token: 0x06000C75 RID: 3189 RVA: 0x0003AC27 File Offset: 0x00038E27
	public override bool Hide()
	{
		bool flag = base.Hide();
		if (flag)
		{
			EventManager.TriggerEvent("Event_OnPlayerMenuHide", null);
		}
		return flag;
	}

	// Token: 0x06000C76 RID: 3190 RVA: 0x0003AC3D File Offset: 0x00038E3D
	private void OnClickIdentity()
	{
		EventManager.TriggerEvent("Event_OnPlayerMenuClickIdentity", null);
	}

	// Token: 0x06000C77 RID: 3191 RVA: 0x0003AC4A File Offset: 0x00038E4A
	private void OnClickAppearance()
	{
		EventManager.TriggerEvent("Event_OnPlayerMenuClickAppearance", null);
	}

	// Token: 0x06000C78 RID: 3192 RVA: 0x0003AC57 File Offset: 0x00038E57
	private void OnClickBack()
	{
		EventManager.TriggerEvent("Event_OnPlayerMenuClickBack", null);
	}

	// Token: 0x04000761 RID: 1889
	private VisualElement playerMenu;

	// Token: 0x04000762 RID: 1890
	private Button identityButton;

	// Token: 0x04000763 RID: 1891
	private Button appearanceButton;

	// Token: 0x04000764 RID: 1892
	private Button backButton;
}
