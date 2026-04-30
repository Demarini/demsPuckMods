using System;
using UnityEngine;

// Token: 0x0200005E RID: 94
public class PuckController : MonoBehaviour
{
	// Token: 0x06000338 RID: 824 RVA: 0x000136A7 File Offset: 0x000118A7
	private void Awake()
	{
		this.puck = base.GetComponent<Puck>();
	}

	// Token: 0x06000339 RID: 825 RVA: 0x000020D3 File Offset: 0x000002D3
	private void Start()
	{
	}

	// Token: 0x0600033A RID: 826 RVA: 0x000020D3 File Offset: 0x000002D3
	private void OnDestroy()
	{
	}

	// Token: 0x0400023C RID: 572
	private Puck puck;
}
