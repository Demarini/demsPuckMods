using System;
using UnityEngine;

// Token: 0x020000A4 RID: 164
public class GameManagerController : MonoBehaviour
{
	// Token: 0x0600053C RID: 1340 RVA: 0x0001CF6B File Offset: 0x0001B16B
	private void Awake()
	{
		this.gameManager = base.GetComponent<GameManager>();
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x000020D3 File Offset: 0x000002D3
	private void OnDestroy()
	{
	}

	// Token: 0x04000335 RID: 821
	private GameManager gameManager;
}
