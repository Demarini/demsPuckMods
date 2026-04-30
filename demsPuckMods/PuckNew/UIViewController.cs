using System;
using UnityEngine;

// Token: 0x0200016B RID: 363
public class UIViewController<T> : MonoBehaviour where T : UIView
{
	// Token: 0x06000A97 RID: 2711 RVA: 0x0003227A File Offset: 0x0003047A
	public virtual void Awake()
	{
		this.uiView = base.GetComponent<T>();
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x000020D3 File Offset: 0x000002D3
	public virtual void OnDestroy()
	{
	}

	// Token: 0x0400061E RID: 1566
	private T uiView;
}
