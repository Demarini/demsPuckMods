using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200007C RID: 124
public class PurchaseManagerController : MonoBehaviour
{
	// Token: 0x06000356 RID: 854 RVA: 0x00009190 File Offset: 0x00007390
	private void Awake()
	{
		this.purchaseManager = base.GetComponent<PurchaseManager>();
	}

	// Token: 0x06000357 RID: 855 RVA: 0x0000919E File Offset: 0x0000739E
	private void Start()
	{
		if (Application.isBatchMode)
		{
			return;
		}
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearancePurchaseItem", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearancePurchaseItem));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnMicroTxnAuthorizationResponse", new Action<Dictionary<string, object>>(this.Event_Client_OnMicroTxnAuthorizationResponse));
	}

	// Token: 0x06000358 RID: 856 RVA: 0x000091DE File Offset: 0x000073DE
	private void OnDestroy()
	{
		if (Application.isBatchMode)
		{
			return;
		}
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearancePurchaseItem", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearancePurchaseItem));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnMicroTxnAuthorizationResponse", new Action<Dictionary<string, object>>(this.Event_Client_OnMicroTxnAuthorizationResponse));
	}

	// Token: 0x06000359 RID: 857 RVA: 0x00018E84 File Offset: 0x00017084
	private void Event_Client_OnAppearancePurchaseItem(Dictionary<string, object> message)
	{
		int itemId = (int)message["itemId"];
		this.purchaseManager.StartPurchase(itemId);
	}

	// Token: 0x0600035A RID: 858 RVA: 0x00018EB0 File Offset: 0x000170B0
	private void Event_Client_OnMicroTxnAuthorizationResponse(Dictionary<string, object> message)
	{
		bool flag = (bool)message["authorized"];
		ulong orderId = (ulong)message["orderId"];
		if (flag)
		{
			this.purchaseManager.CompletePurchase(orderId);
			return;
		}
		this.purchaseManager.CancelPurchase(orderId);
	}

	// Token: 0x040001C2 RID: 450
	private PurchaseManager purchaseManager;
}
