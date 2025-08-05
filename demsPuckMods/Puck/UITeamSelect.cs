using System;
using UnityEngine.UIElements;

// Token: 0x02000145 RID: 325
public class UITeamSelect : UIComponent<UITeamSelect>
{
	// Token: 0x06000B9B RID: 2971 RVA: 0x0000EB7F File Offset: 0x0000CD7F
	private void Start()
	{
		base.VisibilityRequiresMouse = true;
	}

	// Token: 0x06000B9C RID: 2972 RVA: 0x000414CC File Offset: 0x0003F6CC
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("TeamSelectContainer", null);
		this.teamBlueButton = this.container.Query("TeamBlueButton", null);
		this.teamBlueButton.clicked += this.OnClickTeamBlue;
		this.teamRedButton = this.container.Query("TeamRedButton", null);
		this.teamRedButton.clicked += this.OnClickTeamRed;
		this.teamSpectatorButton = this.container.Query("TeamSpectatorButton", null);
		this.teamSpectatorButton.clicked += this.OnClickTeamSpectator;
	}

	// Token: 0x06000B9D RID: 2973 RVA: 0x0000EB88 File Offset: 0x0000CD88
	private void OnClickTeamBlue()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnTeamSelectClickTeamBlue", null);
	}

	// Token: 0x06000B9E RID: 2974 RVA: 0x0000EB9A File Offset: 0x0000CD9A
	private void OnClickTeamRed()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnTeamSelectClickTeamRed", null);
	}

	// Token: 0x06000B9F RID: 2975 RVA: 0x0000EBAC File Offset: 0x0000CDAC
	private void OnClickTeamSpectator()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnTeamSelectClickTeamSpectator", null);
	}

	// Token: 0x06000BA1 RID: 2977 RVA: 0x0004158C File Offset: 0x0003F78C
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000BA2 RID: 2978 RVA: 0x0000EBC6 File Offset: 0x0000CDC6
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000BA3 RID: 2979 RVA: 0x0000EBD0 File Offset: 0x0000CDD0
	protected internal override string __getTypeName()
	{
		return "UITeamSelect";
	}

	// Token: 0x040006BF RID: 1727
	private Button teamBlueButton;

	// Token: 0x040006C0 RID: 1728
	private Button teamRedButton;

	// Token: 0x040006C1 RID: 1729
	private Button teamSpectatorButton;
}