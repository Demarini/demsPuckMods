using System;
using System.Collections.Generic;

// Token: 0x0200007B RID: 123
public class PurchaseManager : MonoBehaviourSingleton<PurchaseManager>
{
	// Token: 0x06000352 RID: 850 RVA: 0x00009108 File Offset: 0x00007308
	public void StartPurchase(int itemId)
	{
		MonoBehaviourSingleton<WebSocketManager>.Instance.Emit("playerStartPurchaseRequest", new Dictionary<string, object>
		{
			{
				"itemId",
				itemId
			}
		}, "playerStartPurchaseResponse");
	}

	// Token: 0x06000353 RID: 851 RVA: 0x00009134 File Offset: 0x00007334
	public void CompletePurchase(ulong orderId)
	{
		MonoBehaviourSingleton<WebSocketManager>.Instance.Emit("playerCompletePurchaseRequest", new Dictionary<string, object>
		{
			{
				"orderId",
				orderId
			}
		}, "playerCompletePurchaseResponse");
	}

	// Token: 0x06000354 RID: 852 RVA: 0x00009160 File Offset: 0x00007360
	public void CancelPurchase(ulong orderId)
	{
		MonoBehaviourSingleton<WebSocketManager>.Instance.Emit("playerCancelPurchaseRequest", new Dictionary<string, object>
		{
			{
				"orderId",
				orderId
			}
		}, null);
	}
}
