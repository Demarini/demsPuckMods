using System;
using UnityEngine;

// Token: 0x02000120 RID: 288
public class UIPlayerMenuController : MonoBehaviour
{
	// Token: 0x06000A2C RID: 2604 RVA: 0x0000D7EE File Offset: 0x0000B9EE
	private void Awake()
	{
		this.uiPlayerMenu = base.GetComponent<UIPlayerMenu>();
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x00006C1B File Offset: 0x00004E1B
	private void Start()
	{
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x00006C1B File Offset: 0x00004E1B
	private void OnDestroy()
	{
	}

	// Token: 0x04000603 RID: 1539
	private UIPlayerMenu uiPlayerMenu;
}
