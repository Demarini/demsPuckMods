using System;
using UnityEngine;

// Token: 0x02000069 RID: 105
public class SpectatorPositionController : MonoBehaviour
{
	// Token: 0x06000373 RID: 883 RVA: 0x00014562 File Offset: 0x00012762
	private void Awake()
	{
		this.spectatorPosition = base.GetComponent<SpectatorPosition>();
	}

	// Token: 0x06000374 RID: 884 RVA: 0x000020D3 File Offset: 0x000002D3
	public void Start()
	{
	}

	// Token: 0x06000375 RID: 885 RVA: 0x000020D3 File Offset: 0x000002D3
	private void OnDestroy()
	{
	}

	// Token: 0x04000268 RID: 616
	private SpectatorPosition spectatorPosition;
}
