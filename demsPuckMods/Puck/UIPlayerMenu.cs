using System;
using UnityEngine.UIElements;

// Token: 0x0200011F RID: 287
public class UIPlayerMenu : UIComponent<UIPlayerMenu>
{
	// Token: 0x06000A20 RID: 2592 RVA: 0x0000D753 File Offset: 0x0000B953
	private void Start()
	{
		base.VisibilityRequiresMouse = true;
	}

	// Token: 0x06000A21 RID: 2593 RVA: 0x0003B9E4 File Offset: 0x00039BE4
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("PlayerMenuContainer", null);
		this.identityButton = this.container.Query("IdentityButton", null);
		this.identityButton.clicked += this.OnClickIdentity;
		this.appearanceButton = this.container.Query("AppearanceButton", null);
		this.appearanceButton.clicked += this.OnClickAppearance;
		this.statisticsButton = this.container.Query("StatisticsButton", null);
		this.statisticsButton.clicked += this.OnClickStatistics;
		this.backButton = this.container.Query("BackButton", null);
		this.backButton.clicked += this.OnClickBack;
	}

	// Token: 0x06000A22 RID: 2594 RVA: 0x0000D75C File Offset: 0x0000B95C
	public override void Show()
	{
		base.Show();
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerMenuShow", null);
	}

	// Token: 0x06000A23 RID: 2595 RVA: 0x0000D774 File Offset: 0x0000B974
	public override void Hide(bool ignoreAlwaysVisible = false)
	{
		base.Hide(ignoreAlwaysVisible);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerMenuHide", null);
	}

	// Token: 0x06000A24 RID: 2596 RVA: 0x0000D78D File Offset: 0x0000B98D
	private void OnClickIdentity()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerMenuClickIdentity", null);
	}

	// Token: 0x06000A25 RID: 2597 RVA: 0x0000D79F File Offset: 0x0000B99F
	private void OnClickAppearance()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerMenuClickAppearance", null);
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x0000D7B1 File Offset: 0x0000B9B1
	private void OnClickStatistics()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerMenuClickStatistics", null);
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x0000D7C3 File Offset: 0x0000B9C3
	private void OnClickBack()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerMenuClickBack", null);
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x0003BAD4 File Offset: 0x00039CD4
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x0000D7DD File Offset: 0x0000B9DD
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x0000D7E7 File Offset: 0x0000B9E7
	protected internal override string __getTypeName()
	{
		return "UIPlayerMenu";
	}

	// Token: 0x040005FF RID: 1535
	private Button identityButton;

	// Token: 0x04000600 RID: 1536
	private Button appearanceButton;

	// Token: 0x04000601 RID: 1537
	private Button statisticsButton;

	// Token: 0x04000602 RID: 1538
	private Button backButton;
}
