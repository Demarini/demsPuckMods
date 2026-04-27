using System;
using UnityEngine;

// Token: 0x0200001D RID: 29
public class PlayerPositionController : MonoBehaviour
{
	// Token: 0x060000B1 RID: 177 RVA: 0x0000409A File Offset: 0x0000229A
	private void Awake()
	{
		this.playerPosition = base.GetComponent<PlayerPosition>();
	}

	// Token: 0x060000B2 RID: 178 RVA: 0x000020D3 File Offset: 0x000002D3
	public void Start()
	{
	}

	// Token: 0x060000B3 RID: 179 RVA: 0x000020D3 File Offset: 0x000002D3
	private void OnDestroy()
	{
	}

	// Token: 0x0400004D RID: 77
	private PlayerPosition playerPosition;
}
