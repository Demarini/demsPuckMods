using System;

// Token: 0x020001B2 RID: 434
public class UIPlayerMenuController : UIViewController<UIPlayerMenu>
{
	// Token: 0x06000C7A RID: 3194 RVA: 0x0003AC64 File Offset: 0x00038E64
	public override void Awake()
	{
		base.Awake();
		this.uiPlayerMenu = base.GetComponent<UIPlayerMenu>();
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x0003AC78 File Offset: 0x00038E78
	public override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x04000765 RID: 1893
	private UIPlayerMenu uiPlayerMenu;
}
