using System;

// Token: 0x020001D2 RID: 466
public class UITeamSelectController : UIViewController<UITeamSelect>
{
	// Token: 0x06000DB9 RID: 3513 RVA: 0x000411E3 File Offset: 0x0003F3E3
	public override void Awake()
	{
		base.Awake();
		this.uiTeamSelect = base.GetComponent<UITeamSelect>();
	}

	// Token: 0x0400080C RID: 2060
	private UITeamSelect uiTeamSelect;
}
