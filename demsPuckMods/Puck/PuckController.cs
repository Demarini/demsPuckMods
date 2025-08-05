using System;
using UnityEngine;

// Token: 0x02000025 RID: 37
public class PuckController : MonoBehaviour
{
	// Token: 0x0600010F RID: 271 RVA: 0x00007873 File Offset: 0x00005A73
	private void Awake()
	{
		this.puck = base.GetComponent<Puck>();
	}

	// Token: 0x06000110 RID: 272 RVA: 0x00006C1B File Offset: 0x00004E1B
	private void Start()
	{
	}

	// Token: 0x06000111 RID: 273 RVA: 0x00006C1B File Offset: 0x00004E1B
	private void OnDestroy()
	{
	}

	// Token: 0x04000090 RID: 144
	private Puck puck;
}
