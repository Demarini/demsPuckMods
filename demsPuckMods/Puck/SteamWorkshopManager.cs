using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using UnityEngine;

// Token: 0x020000B1 RID: 177
public class SteamWorkshopManager : MonoBehaviourSingleton<SteamWorkshopManager>
{
	// Token: 0x0600053A RID: 1338 RVA: 0x0000A47C File Offset: 0x0000867C
	private void Start()
	{
		this.RegisterCallbacks();
	}

	// Token: 0x0600053B RID: 1339 RVA: 0x00021750 File Offset: 0x0001F950
	private void RegisterCallbacks()
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		if (Application.isBatchMode)
		{
			this.DownloadItemResult = Callback<DownloadItemResult_t>.CreateGameServer(new Callback<DownloadItemResult_t>.DispatchDelegate(this.OnDownloadItemResult));
			this.UserSubscribedItemsListChanged = Callback<UserSubscribedItemsListChanged_t>.CreateGameServer(new Callback<UserSubscribedItemsListChanged_t>.DispatchDelegate(this.OnUserSubscribedItemsListChanged));
			this.RemoteStorageSubscribePublishedFileResult = Callback<RemoteStorageSubscribePublishedFileResult_t>.CreateGameServer(new Callback<RemoteStorageSubscribePublishedFileResult_t>.DispatchDelegate(this.OnRemoteStorageSubscribePublishedFileResult));
			this.RemoteStorageUnsubscribePublishedFileResult = Callback<RemoteStorageUnsubscribePublishedFileResult_t>.CreateGameServer(new Callback<RemoteStorageUnsubscribePublishedFileResult_t>.DispatchDelegate(this.OnRemoteStorageUnsubscribePublishedFileResult));
			this.DeleteItemResult = Callback<DeleteItemResult_t>.CreateGameServer(new Callback<DeleteItemResult_t>.DispatchDelegate(this.OnDeleteItemResult));
			return;
		}
		this.DownloadItemResult = Callback<DownloadItemResult_t>.Create(new Callback<DownloadItemResult_t>.DispatchDelegate(this.OnDownloadItemResult));
		this.UserSubscribedItemsListChanged = Callback<UserSubscribedItemsListChanged_t>.Create(new Callback<UserSubscribedItemsListChanged_t>.DispatchDelegate(this.OnUserSubscribedItemsListChanged));
		this.RemoteStorageSubscribePublishedFileResult = Callback<RemoteStorageSubscribePublishedFileResult_t>.Create(new Callback<RemoteStorageSubscribePublishedFileResult_t>.DispatchDelegate(this.OnRemoteStorageSubscribePublishedFileResult));
		this.RemoteStorageUnsubscribePublishedFileResult = Callback<RemoteStorageUnsubscribePublishedFileResult_t>.Create(new Callback<RemoteStorageUnsubscribePublishedFileResult_t>.DispatchDelegate(this.OnRemoteStorageUnsubscribePublishedFileResult));
		this.DeleteItemResult = Callback<DeleteItemResult_t>.Create(new Callback<DeleteItemResult_t>.DispatchDelegate(this.OnDeleteItemResult));
	}

	// Token: 0x0600053C RID: 1340 RVA: 0x00021858 File Offset: 0x0001FA58
	public void AddInstalledItem(ulong id)
	{
		if (this.GetInstalledItemById(id) != null)
		{
			return;
		}
		string path;
		if (!this.GetItemInstallInfo(id, out path))
		{
			return;
		}
		InstalledItem installedItem = new InstalledItem(id, path);
		this.InstalledItems.Add(installedItem);
		Debug.Log(string.Format("[SteamWorkshopManager] Installed item {0} added", id));
		this.GetItemDetails(new ulong[]
		{
			id
		});
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnInstalledItemAdded", new Dictionary<string, object>
		{
			{
				"installedItem",
				installedItem
			}
		});
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x000218D4 File Offset: 0x0001FAD4
	public void RemoveInstalledItem(ulong id)
	{
		InstalledItem installedItemById = this.GetInstalledItemById(id);
		if (installedItemById == null)
		{
			return;
		}
		this.InstalledItems.Remove(installedItemById);
		Debug.Log(string.Format("[SteamWorkshopManager] Installed item {0} removed", id));
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnInstalledItemRemoved", new Dictionary<string, object>
		{
			{
				"installedItem",
				installedItemById
			}
		});
	}

	// Token: 0x0600053E RID: 1342 RVA: 0x00021930 File Offset: 0x0001FB30
	public InstalledItem GetInstalledItemById(ulong id)
	{
		return this.InstalledItems.Find((InstalledItem item) => item.Id == id);
	}

	// Token: 0x0600053F RID: 1343 RVA: 0x00021964 File Offset: 0x0001FB64
	public void VerifyItemIntegrity()
	{
		if (Application.isBatchMode)
		{
			return;
		}
		Debug.Log("[SteamWorkshopManager] Verifying item integrity");
		foreach (ulong num in this.GetSubscribedItemIds())
		{
			if (this.IsItemInstalled(num))
			{
				if (this.IsItemNeedsUpdate(num))
				{
					this.DownloadItem(num);
				}
				else
				{
					this.AddInstalledItem(num);
				}
			}
			else
			{
				this.DownloadItem(num);
			}
		}
		foreach (ulong num2 in (from item in this.InstalledItems
		select item.Id).ToArray<ulong>())
		{
			if (!this.IsItemSubscribed(num2))
			{
				this.RemoveInstalledItem(num2);
			}
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnItemIntegrityVerified", new Dictionary<string, object>
		{
			{
				"installedItems",
				this.InstalledItems
			}
		});
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x0000A484 File Offset: 0x00008684
	public bool IsItemInstalled(ulong itemId)
	{
		return MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized && (this.GetItemState(itemId) & 4U) > 0U;
	}

	// Token: 0x06000541 RID: 1345 RVA: 0x0000A4A0 File Offset: 0x000086A0
	public bool IsItemSubscribed(ulong itemId)
	{
		return MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized && (this.GetItemState(itemId) & 1U) > 0U;
	}

	// Token: 0x06000542 RID: 1346 RVA: 0x0000A4BC File Offset: 0x000086BC
	public bool IsItemNeedsUpdate(ulong itemId)
	{
		return MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized && (this.GetItemState(itemId) & 8U) > 0U;
	}

	// Token: 0x06000543 RID: 1347 RVA: 0x0000A4D8 File Offset: 0x000086D8
	public uint GetNumSubscribedItems()
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return 0U;
		}
		if (Application.isBatchMode)
		{
			return SteamGameServerUGC.GetNumSubscribedItems();
		}
		return SteamUGC.GetNumSubscribedItems();
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x00021A40 File Offset: 0x0001FC40
	public ulong[] GetSubscribedItemIds()
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return new ulong[0];
		}
		uint numSubscribedItems = this.GetNumSubscribedItems();
		PublishedFileId_t[] array = new PublishedFileId_t[numSubscribedItems];
		if (Application.isBatchMode)
		{
			SteamGameServerUGC.GetSubscribedItems(array, numSubscribedItems);
		}
		else
		{
			SteamUGC.GetSubscribedItems(array, numSubscribedItems);
		}
		return (from id in array
		select id.m_PublishedFileId).ToArray<ulong>();
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x00021AB4 File Offset: 0x0001FCB4
	public bool GetItemInstallInfo(ulong itemId, out string path)
	{
		path = null;
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return false;
		}
		if (Application.isBatchMode)
		{
			ulong num;
			uint num2;
			return SteamGameServerUGC.GetItemInstallInfo(new PublishedFileId_t(itemId), out num, out path, 256U, out num2);
		}
		ulong num3;
		uint num4;
		return SteamUGC.GetItemInstallInfo(new PublishedFileId_t(itemId), out num3, out path, 256U, out num4);
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x0000A4FA File Offset: 0x000086FA
	public uint GetItemState(ulong itemId)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return 0U;
		}
		if (Application.isBatchMode)
		{
			return SteamGameServerUGC.GetItemState(new PublishedFileId_t(itemId));
		}
		return SteamUGC.GetItemState(new PublishedFileId_t(itemId));
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x00021B04 File Offset: 0x0001FD04
	public void GetItemDetails(ulong[] itemIds)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		UGCQueryHandle_t ugcqueryHandle_t = this.CreateQueryUGCDetailsRequest(itemIds);
		CallResult<SteamUGCQueryCompleted_t> callResult = CallResult<SteamUGCQueryCompleted_t>.Create(new CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate(this.OnUGCQueryCompleted));
		this.UGCQueryCompletedCallResultMap.Add(ugcqueryHandle_t, callResult);
		SteamAPICall_t steamAPICall_t = this.SendQueryUGCRequest(ugcqueryHandle_t);
		if (steamAPICall_t == SteamAPICall_t.Invalid)
		{
			return;
		}
		callResult.Set(steamAPICall_t, null);
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x00021B64 File Offset: 0x0001FD64
	public UGCQueryHandle_t CreateQueryUGCDetailsRequest(ulong[] itemIds)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return UGCQueryHandle_t.Invalid;
		}
		PublishedFileId_t[] array = new PublishedFileId_t[itemIds.Length];
		for (int i = 0; i < itemIds.Length; i++)
		{
			array[i] = new PublishedFileId_t
			{
				m_PublishedFileId = itemIds[i]
			};
		}
		if (Application.isBatchMode)
		{
			return SteamGameServerUGC.CreateQueryUGCDetailsRequest(array, (uint)array.Length);
		}
		return SteamUGC.CreateQueryUGCDetailsRequest(array, (uint)array.Length);
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x0000A528 File Offset: 0x00008728
	public SteamAPICall_t SendQueryUGCRequest(UGCQueryHandle_t queryHandle)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return SteamAPICall_t.Invalid;
		}
		if (Application.isBatchMode)
		{
			return SteamGameServerUGC.SendQueryUGCRequest(queryHandle);
		}
		return SteamUGC.SendQueryUGCRequest(queryHandle);
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x0000A550 File Offset: 0x00008750
	private bool GetQueryUGCResult(UGCQueryHandle_t queryHandle, uint index, out SteamUGCDetails_t details)
	{
		details = default(SteamUGCDetails_t);
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return false;
		}
		if (Application.isBatchMode)
		{
			return SteamGameServerUGC.GetQueryUGCResult(queryHandle, index, out details);
		}
		return SteamUGC.GetQueryUGCResult(queryHandle, index, out details);
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x0000A57F File Offset: 0x0000877F
	private bool GetQueryUGCPreviewURL(UGCQueryHandle_t queryHandle, uint index, out string previewUrl)
	{
		previewUrl = null;
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return false;
		}
		if (Application.isBatchMode)
		{
			return SteamGameServerUGC.GetQueryUGCPreviewURL(queryHandle, index, out previewUrl, 256U);
		}
		return SteamUGC.GetQueryUGCPreviewURL(queryHandle, index, out previewUrl, 256U);
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x00021BD0 File Offset: 0x0001FDD0
	public void DownloadItem(ulong itemId)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		Debug.Log(string.Format("[SteamWorkshopManager] Downloading item {0}", itemId));
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnItemDownloadStarted", new Dictionary<string, object>
		{
			{
				"itemId",
				itemId
			}
		});
		bool flag;
		if (Application.isBatchMode)
		{
			flag = SteamGameServerUGC.DownloadItem(new PublishedFileId_t(itemId), true);
		}
		else
		{
			flag = SteamUGC.DownloadItem(new PublishedFileId_t(itemId), true);
		}
		if (!flag)
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnItemDownloadFailed", new Dictionary<string, object>
			{
				{
					"itemId",
					itemId
				}
			});
		}
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x00021C70 File Offset: 0x0001FE70
	public void DeleteItem(ulong itemId)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		Debug.Log(string.Format("[SteamWorkshopManager] Deleting item {0}", itemId));
		if (Application.isBatchMode)
		{
			SteamGameServerUGC.DeleteItem(new PublishedFileId_t(itemId));
			return;
		}
		SteamUGC.DeleteItem(new PublishedFileId_t(itemId));
	}

	// Token: 0x0600054E RID: 1358 RVA: 0x00021CC0 File Offset: 0x0001FEC0
	public void SubscribeItem(ulong itemId)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		Debug.Log(string.Format("[SteamWorkshopManager] Subscribing item {0}", itemId));
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnItemSubscribeStarted", new Dictionary<string, object>
		{
			{
				"itemId",
				itemId
			}
		});
		if (Application.isBatchMode)
		{
			SteamGameServerUGC.SubscribeItem(new PublishedFileId_t(itemId));
			return;
		}
		SteamUGC.SubscribeItem(new PublishedFileId_t(itemId));
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x00021D34 File Offset: 0x0001FF34
	public void UnsubscribeItem(ulong itemId)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		Debug.Log(string.Format("[SteamWorkshopManager] Unsubscribing item {0}", itemId));
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnItemUnsubscribeStarted", new Dictionary<string, object>
		{
			{
				"itemId",
				itemId
			}
		});
		if (Application.isBatchMode)
		{
			SteamGameServerUGC.UnsubscribeItem(new PublishedFileId_t(itemId));
			return;
		}
		SteamUGC.UnsubscribeItem(new PublishedFileId_t(itemId));
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x00021DA8 File Offset: 0x0001FFA8
	private void OnDownloadItemResult(DownloadItemResult_t response)
	{
		if (response.m_unAppID != new AppId_t(2994020U))
		{
			return;
		}
		if (response.m_eResult == EResult.k_EResultOK)
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnItemDownloadSucceeded", new Dictionary<string, object>
			{
				{
					"itemId",
					(ulong)response.m_nPublishedFileId
				}
			});
			this.AddInstalledItem((ulong)response.m_nPublishedFileId);
			return;
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnItemDownloadFailed", new Dictionary<string, object>
		{
			{
				"itemId",
				(ulong)response.m_nPublishedFileId
			}
		});
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x0000A5B4 File Offset: 0x000087B4
	private void OnUserSubscribedItemsListChanged(UserSubscribedItemsListChanged_t response)
	{
		if (response.m_nAppID != new AppId_t(2994020U))
		{
			return;
		}
		this.VerifyItemIntegrity();
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x00021E48 File Offset: 0x00020048
	private void OnRemoteStorageSubscribePublishedFileResult(RemoteStorageSubscribePublishedFileResult_t response)
	{
		if (response.m_eResult == EResult.k_EResultOK)
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnItemSubscribeSucceeded", new Dictionary<string, object>
			{
				{
					"itemId",
					(ulong)response.m_nPublishedFileId
				}
			});
			return;
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnItemSubscribeFailed", new Dictionary<string, object>
		{
			{
				"itemId",
				(ulong)response.m_nPublishedFileId
			}
		});
	}

	// Token: 0x06000553 RID: 1363 RVA: 0x00021EC0 File Offset: 0x000200C0
	private void OnRemoteStorageUnsubscribePublishedFileResult(RemoteStorageUnsubscribePublishedFileResult_t response)
	{
		if (response.m_eResult == EResult.k_EResultOK)
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnItemUnsubscribeSucceeded", new Dictionary<string, object>
			{
				{
					"itemId",
					(ulong)response.m_nPublishedFileId
				}
			});
			if (this.IsItemInstalled((ulong)response.m_nPublishedFileId))
			{
				this.DeleteItem((ulong)response.m_nPublishedFileId);
				return;
			}
		}
		else
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnItemUnsubscribeFailed", new Dictionary<string, object>
			{
				{
					"itemId",
					(ulong)response.m_nPublishedFileId
				}
			});
		}
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x0000A5D4 File Offset: 0x000087D4
	private void OnDeleteItemResult(DeleteItemResult_t response)
	{
		if (response.m_eResult == EResult.k_EResultOK)
		{
			this.RemoveInstalledItem((ulong)response.m_nPublishedFileId);
		}
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x00021F5C File Offset: 0x0002015C
	private void OnUGCQueryCompleted(SteamUGCQueryCompleted_t response, bool bIOFailure)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		if (response.m_eResult != EResult.k_EResultOK)
		{
			return;
		}
		Debug.Log(string.Format("[SteamWorkshopManager] OnUGCQueryCompleted: {0}", response.m_unNumResultsReturned));
		for (uint num = 0U; num < response.m_unNumResultsReturned; num += 1U)
		{
			SteamUGCDetails_t steamUGCDetails_t;
			if (this.GetQueryUGCResult(response.m_handle, num, out steamUGCDetails_t))
			{
				PublishedFileId_t nPublishedFileId = steamUGCDetails_t.m_nPublishedFileId;
				string rgchTitle = steamUGCDetails_t.m_rgchTitle;
				string rgchDescription = steamUGCDetails_t.m_rgchDescription;
				string value;
				this.GetQueryUGCPreviewURL(response.m_handle, num, out value);
				MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnItemDetails", new Dictionary<string, object>
				{
					{
						"id",
						(ulong)nPublishedFileId
					},
					{
						"title",
						rgchTitle
					},
					{
						"description",
						rgchDescription
					},
					{
						"previewUrl",
						value
					}
				});
			}
		}
		if (this.UGCQueryCompletedCallResultMap.ContainsKey(response.m_handle))
		{
			this.UGCQueryCompletedCallResultMap[response.m_handle].Dispose();
			this.UGCQueryCompletedCallResultMap.Remove(response.m_handle);
		}
		if (Application.isBatchMode)
		{
			SteamGameServerUGC.ReleaseQueryUGCRequest(response.m_handle);
			return;
		}
		SteamUGC.ReleaseQueryUGCRequest(response.m_handle);
	}

	// Token: 0x040002DD RID: 733
	public List<InstalledItem> InstalledItems = new List<InstalledItem>();

	// Token: 0x040002DE RID: 734
	private Callback<DownloadItemResult_t> DownloadItemResult;

	// Token: 0x040002DF RID: 735
	private Callback<UserSubscribedItemsListChanged_t> UserSubscribedItemsListChanged;

	// Token: 0x040002E0 RID: 736
	private Callback<RemoteStorageSubscribePublishedFileResult_t> RemoteStorageSubscribePublishedFileResult;

	// Token: 0x040002E1 RID: 737
	private Callback<RemoteStorageUnsubscribePublishedFileResult_t> RemoteStorageUnsubscribePublishedFileResult;

	// Token: 0x040002E2 RID: 738
	private Callback<DeleteItemResult_t> DeleteItemResult;

	// Token: 0x040002E3 RID: 739
	private Dictionary<UGCQueryHandle_t, CallResult<SteamUGCQueryCompleted_t>> UGCQueryCompletedCallResultMap = new Dictionary<UGCQueryHandle_t, CallResult<SteamUGCQueryCompleted_t>>();
}
