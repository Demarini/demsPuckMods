using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

// Token: 0x02000045 RID: 69
public class ItemManager : MonoBehaviourSingleton<ItemManager>
{
	// Token: 0x060001EF RID: 495 RVA: 0x000081BA File Offset: 0x000063BA
	public override void Awake()
	{
		base.Awake();
		this.PurchaseableItems = new List<string>(this.itemIdMap.Values);
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x000159F4 File Offset: 0x00013BF4
	public void SetItems(int[] itemIds)
	{
		this.OwnedItemIds = itemIds;
		this.OwnedItems.Clear();
		foreach (int key in itemIds)
		{
			if (this.itemIdMap.ContainsKey(key))
			{
				this.OwnedItems.Add(this.itemIdMap[key]);
			}
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnOwnedItemIdsUpdated", new Dictionary<string, object>
		{
			{
				"ownedItemIds",
				this.OwnedItemIds
			},
			{
				"ownedItems",
				this.OwnedItems
			},
			{
				"purchaseableItems",
				this.PurchaseableItems
			}
		});
	}

	// Token: 0x04000128 RID: 296
	[Header("Settings")]
	[SerializeField]
	private SerializedDictionary<int, string> itemIdMap = new SerializedDictionary<int, string>();

	// Token: 0x04000129 RID: 297
	[HideInInspector]
	public int[] OwnedItemIds = new int[0];

	// Token: 0x0400012A RID: 298
	[HideInInspector]
	public List<string> OwnedItems = new List<string>();

	// Token: 0x0400012B RID: 299
	[HideInInspector]
	public List<string> PurchaseableItems = new List<string>();
}
