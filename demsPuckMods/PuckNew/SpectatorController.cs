using System;
using UnityEngine;

// Token: 0x0200006B RID: 107
public class SpectatorController : MonoBehaviour
{
	// Token: 0x0600037D RID: 893 RVA: 0x000147DF File Offset: 0x000129DF
	private void Awake()
	{
		this.spectator = base.GetComponent<Spectator>();
	}

	// Token: 0x0600037E RID: 894 RVA: 0x000020D3 File Offset: 0x000002D3
	private void OnDestroy()
	{
	}

	// Token: 0x04000272 RID: 626
	private Spectator spectator;
}
