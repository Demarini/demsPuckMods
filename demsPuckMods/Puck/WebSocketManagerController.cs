using System;
using UnityEngine;

// Token: 0x020000D0 RID: 208
public class WebSocketManagerController : MonoBehaviour
{
	// Token: 0x0600063B RID: 1595 RVA: 0x0000AF0E File Offset: 0x0000910E
	private void Awake()
	{
		this.webSocketManager = base.GetComponent<WebSocketManager>();
	}

	// Token: 0x0600063C RID: 1596 RVA: 0x00006C1B File Offset: 0x00004E1B
	private void Start()
	{
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x00006C1B File Offset: 0x00004E1B
	private void OnDestroy()
	{
	}

	// Token: 0x04000365 RID: 869
	private WebSocketManager webSocketManager;
}
