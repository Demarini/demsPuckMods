using System;
using System.Collections.Generic;
using System.Linq;
using SocketIOClient;
using UnityEngine;

// Token: 0x02000046 RID: 70
public class ItemManagerController : MonoBehaviour
{
	// Token: 0x060001F2 RID: 498 RVA: 0x0000820D File Offset: 0x0000640D
	private void Awake()
	{
		this.itemManager = base.GetComponent<ItemManager>();
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x0000821B File Offset: 0x0000641B
	private void Start()
	{
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("player", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayer));
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x00008238 File Offset: 0x00006438
	private void OnDestroy()
	{
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("player", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayer));
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x00015A94 File Offset: 0x00013C94
	private void WebSocket_Event_OnPlayer(Dictionary<string, object> message)
	{
		PlayerData value = ((SocketIOResponse)message["response"]).GetValue<PlayerData>(0);
		this.itemManager.SetItems((from item in value.items
		select item.itemId).ToArray<int>());
	}

	// Token: 0x0400012C RID: 300
	private ItemManager itemManager;
}
