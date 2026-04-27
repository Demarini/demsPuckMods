using System;

// Token: 0x020001AD RID: 429
public class UIPauseMenuController : UIViewController<UIPauseMenu>
{
	// Token: 0x06000C56 RID: 3158 RVA: 0x0003A574 File Offset: 0x00038774
	public override void Awake()
	{
		base.Awake();
		this.uiPauseMenu = base.GetComponent<UIPauseMenu>();
	}

	// Token: 0x06000C57 RID: 3159 RVA: 0x0003A588 File Offset: 0x00038788
	public override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x04000754 RID: 1876
	private UIPauseMenu uiPauseMenu;
}
