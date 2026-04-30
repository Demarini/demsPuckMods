using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using UnityEngine;

// Token: 0x02000135 RID: 309
public static class SteamWorkshopManager
{
	// Token: 0x0600091F RID: 2335 RVA: 0x0002C052 File Offset: 0x0002A252
	public static void Initialize()
	{
		SteamWorkshopManager.RegisterCallbacks();
		SteamWorkshopManagerController.Initialize();
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x0002C05E File Offset: 0x0002A25E
	public static void Dispose()
	{
		SteamWorkshopManagerController.Dispose();
		SteamWorkshopManager.UnregisterCallbacks();
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x0002C06C File Offset: 0x0002A26C
	private static void RegisterCallbacks()
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		if (ApplicationManager.IsDedicatedGameServer)
		{
			SteamWorkshopManager.DownloadItemResult = Callback<DownloadItemResult_t>.CreateGameServer(new Callback<DownloadItemResult_t>.DispatchDelegate(SteamWorkshopManager.OnDownloadItemResult));
			SteamWorkshopManager.UserSubscribedItemsListChanged = Callback<UserSubscribedItemsListChanged_t>.CreateGameServer(new Callback<UserSubscribedItemsListChanged_t>.DispatchDelegate(SteamWorkshopManager.OnUserSubscribedItemsListChanged));
			SteamWorkshopManager.RemoteStorageSubscribePublishedFileResult = Callback<RemoteStorageSubscribePublishedFileResult_t>.CreateGameServer(new Callback<RemoteStorageSubscribePublishedFileResult_t>.DispatchDelegate(SteamWorkshopManager.OnRemoteStorageSubscribePublishedFileResult));
			SteamWorkshopManager.RemoteStorageUnsubscribePublishedFileResult = Callback<RemoteStorageUnsubscribePublishedFileResult_t>.CreateGameServer(new Callback<RemoteStorageUnsubscribePublishedFileResult_t>.DispatchDelegate(SteamWorkshopManager.OnRemoteStorageUnsubscribePublishedFileResult));
			SteamWorkshopManager.DeleteItemResult = Callback<DeleteItemResult_t>.CreateGameServer(new Callback<DeleteItemResult_t>.DispatchDelegate(SteamWorkshopManager.OnDeleteItemResult));
			return;
		}
		SteamWorkshopManager.DownloadItemResult = Callback<DownloadItemResult_t>.Create(new Callback<DownloadItemResult_t>.DispatchDelegate(SteamWorkshopManager.OnDownloadItemResult));
		SteamWorkshopManager.UserSubscribedItemsListChanged = Callback<UserSubscribedItemsListChanged_t>.Create(new Callback<UserSubscribedItemsListChanged_t>.DispatchDelegate(SteamWorkshopManager.OnUserSubscribedItemsListChanged));
		SteamWorkshopManager.RemoteStorageSubscribePublishedFileResult = Callback<RemoteStorageSubscribePublishedFileResult_t>.Create(new Callback<RemoteStorageSubscribePublishedFileResult_t>.DispatchDelegate(SteamWorkshopManager.OnRemoteStorageSubscribePublishedFileResult));
		SteamWorkshopManager.RemoteStorageUnsubscribePublishedFileResult = Callback<RemoteStorageUnsubscribePublishedFileResult_t>.Create(new Callback<RemoteStorageUnsubscribePublishedFileResult_t>.DispatchDelegate(SteamWorkshopManager.OnRemoteStorageUnsubscribePublishedFileResult));
		SteamWorkshopManager.DeleteItemResult = Callback<DeleteItemResult_t>.Create(new Callback<DeleteItemResult_t>.DispatchDelegate(SteamWorkshopManager.OnDeleteItemResult));
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x0002C165 File Offset: 0x0002A365
	private static void UnregisterCallbacks()
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		SteamWorkshopManager.DownloadItemResult.Unregister();
		SteamWorkshopManager.UserSubscribedItemsListChanged.Unregister();
		SteamWorkshopManager.RemoteStorageSubscribePublishedFileResult.Unregister();
		SteamWorkshopManager.RemoteStorageUnsubscribePublishedFileResult.Unregister();
		SteamWorkshopManager.DeleteItemResult.Unregister();
	}

	// Token: 0x06000923 RID: 2339 RVA: 0x0002C1A4 File Offset: 0x0002A3A4
	public static void AddInstalledItem(ulong id)
	{
		if (SteamWorkshopManager.GetInstalledItemById(id) != null)
		{
			return;
		}
		string path;
		if (!SteamWorkshopManager.GetItemInstallInfo(id, out path))
		{
			return;
		}
		InstalledItem installedItem = new InstalledItem(id, path);
		SteamWorkshopManager.InstalledItems.Add(installedItem);
		Debug.Log(string.Format("[SteamWorkshopManager] Installed item {0} added", id));
		SteamWorkshopManager.GetItemDetails(new ulong[]
		{
			id
		});
		EventManager.TriggerEvent("Event_OnInstalledItemAdded", new Dictionary<string, object>
		{
			{
				"installedItem",
				installedItem
			}
		});
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x0002C218 File Offset: 0x0002A418
	public static void RemoveInstalledItem(ulong id)
	{
		InstalledItem installedItemById = SteamWorkshopManager.GetInstalledItemById(id);
		if (installedItemById == null)
		{
			return;
		}
		SteamWorkshopManager.InstalledItems.Remove(installedItemById);
		Debug.Log(string.Format("[SteamWorkshopManager] Installed item {0} removed", id));
		EventManager.TriggerEvent("Event_OnInstalledItemRemoved", new Dictionary<string, object>
		{
			{
				"installedItem",
				installedItemById
			}
		});
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x0002C26C File Offset: 0x0002A46C
	public static InstalledItem GetInstalledItemById(ulong id)
	{
		return SteamWorkshopManager.InstalledItems.Find((InstalledItem item) => item.Id == id);
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x0002C29C File Offset: 0x0002A49C
	public static void VerifyItemIntegrity()
	{
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return;
		}
		Debug.Log("[SteamWorkshopManager] Verifying item integrity");
		foreach (ulong num in SteamWorkshopManager.GetSubscribedItemIds())
		{
			if (SteamWorkshopManager.IsItemInstalled(num))
			{
				if (SteamWorkshopManager.IsItemNeedsUpdate(num))
				{
					SteamWorkshopManager.DownloadItem(num);
				}
				else
				{
					SteamWorkshopManager.AddInstalledItem(num);
				}
			}
			else
			{
				SteamWorkshopManager.DownloadItem(num);
			}
		}
		foreach (ulong num2 in (from item in SteamWorkshopManager.InstalledItems
		select item.Id).ToArray<ulong>())
		{
			if (!SteamWorkshopManager.IsItemSubscribed(num2))
			{
				SteamWorkshopManager.RemoveInstalledItem(num2);
			}
		}
		EventManager.TriggerEvent("Event_OnItemIntegrityVerified", new Dictionary<string, object>
		{
			{
				"installedItems",
				SteamWorkshopManager.InstalledItems
			}
		});
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x0002C367 File Offset: 0x0002A567
	public static bool IsItemInstalled(ulong itemId)
	{
		return SteamManager.IsInitialized && (SteamWorkshopManager.GetItemState(itemId) & 4U) > 0U;
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x0002C37D File Offset: 0x0002A57D
	public static bool IsItemSubscribed(ulong itemId)
	{
		return SteamManager.IsInitialized && (SteamWorkshopManager.GetItemState(itemId) & 1U) > 0U;
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x0002C393 File Offset: 0x0002A593
	public static bool IsItemNeedsUpdate(ulong itemId)
	{
		return SteamManager.IsInitialized && (SteamWorkshopManager.GetItemState(itemId) & 8U) > 0U;
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x0002C3A9 File Offset: 0x0002A5A9
	public static uint GetNumSubscribedItems()
	{
		if (!SteamManager.IsInitialized)
		{
			return 0U;
		}
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return SteamGameServerUGC.GetNumSubscribedItems(false);
		}
		return SteamUGC.GetNumSubscribedItems(false);
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x0002C3C8 File Offset: 0x0002A5C8
	public static ulong[] GetSubscribedItemIds()
	{
		if (!SteamManager.IsInitialized)
		{
			return new ulong[0];
		}
		uint numSubscribedItems = SteamWorkshopManager.GetNumSubscribedItems();
		PublishedFileId_t[] array = new PublishedFileId_t[numSubscribedItems];
		if (ApplicationManager.IsDedicatedGameServer)
		{
			SteamGameServerUGC.GetSubscribedItems(array, numSubscribedItems, false);
		}
		else
		{
			SteamUGC.GetSubscribedItems(array, numSubscribedItems, false);
		}
		return (from id in array
		select id.m_PublishedFileId).ToArray<ulong>();
	}

	// Token: 0x0600092C RID: 2348 RVA: 0x0002C438 File Offset: 0x0002A638
	public static bool GetItemInstallInfo(ulong itemId, out string path)
	{
		path = null;
		if (!SteamManager.IsInitialized)
		{
			return false;
		}
		if (ApplicationManager.IsDedicatedGameServer)
		{
			ulong num;
			uint num2;
			return SteamGameServerUGC.GetItemInstallInfo(new PublishedFileId_t(itemId), out num, out path, 4096U, out num2);
		}
		ulong num3;
		uint num4;
		return SteamUGC.GetItemInstallInfo(new PublishedFileId_t(itemId), out num3, out path, 4096U, out num4);
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x0002C483 File Offset: 0x0002A683
	public static uint GetItemState(ulong itemId)
	{
		if (!SteamManager.IsInitialized)
		{
			return 0U;
		}
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return SteamGameServerUGC.GetItemState(new PublishedFileId_t(itemId));
		}
		return SteamUGC.GetItemState(new PublishedFileId_t(itemId));
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x0002C4AC File Offset: 0x0002A6AC
	public static void GetItemDetails(ulong[] itemIds)
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		UGCQueryHandle_t ugcqueryHandle_t = SteamWorkshopManager.CreateQueryUGCDetailsRequest(itemIds);
		CallResult<SteamUGCQueryCompleted_t> callResult = CallResult<SteamUGCQueryCompleted_t>.Create(new CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate(SteamWorkshopManager.OnUGCQueryCompleted));
		SteamWorkshopManager.UGCQueryCompletedCallResultMap.Add(ugcqueryHandle_t, callResult);
		SteamAPICall_t steamAPICall_t = SteamWorkshopManager.SendQueryUGCRequest(ugcqueryHandle_t);
		if (steamAPICall_t == SteamAPICall_t.Invalid)
		{
			return;
		}
		callResult.Set(steamAPICall_t, null);
	}

	// Token: 0x0600092F RID: 2351 RVA: 0x0002C504 File Offset: 0x0002A704
	public static UGCQueryHandle_t CreateQueryUGCDetailsRequest(ulong[] itemIds)
	{
		if (!SteamManager.IsInitialized)
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
		UGCQueryHandle_t ugcqueryHandle_t;
		if (ApplicationManager.IsDedicatedGameServer)
		{
			ugcqueryHandle_t = SteamGameServerUGC.CreateQueryUGCDetailsRequest(array, (uint)array.Length);
			SteamGameServerUGC.SetReturnLongDescription(ugcqueryHandle_t, true);
		}
		else
		{
			ugcqueryHandle_t = SteamUGC.CreateQueryUGCDetailsRequest(array, (uint)array.Length);
			SteamUGC.SetReturnLongDescription(ugcqueryHandle_t, true);
		}
		return ugcqueryHandle_t;
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x0002C57D File Offset: 0x0002A77D
	public static SteamAPICall_t SendQueryUGCRequest(UGCQueryHandle_t queryHandle)
	{
		if (!SteamManager.IsInitialized)
		{
			return SteamAPICall_t.Invalid;
		}
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return SteamGameServerUGC.SendQueryUGCRequest(queryHandle);
		}
		return SteamUGC.SendQueryUGCRequest(queryHandle);
	}

	// Token: 0x06000931 RID: 2353 RVA: 0x0002C5A0 File Offset: 0x0002A7A0
	private static bool GetQueryUGCResult(UGCQueryHandle_t queryHandle, uint index, out SteamUGCDetails_t details)
	{
		details = default(SteamUGCDetails_t);
		if (!SteamManager.IsInitialized)
		{
			return false;
		}
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return SteamGameServerUGC.GetQueryUGCResult(queryHandle, index, out details);
		}
		return SteamUGC.GetQueryUGCResult(queryHandle, index, out details);
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x0002C5CA File Offset: 0x0002A7CA
	private static bool GetQueryUGCPreviewURL(UGCQueryHandle_t queryHandle, uint index, out string previewUrl)
	{
		previewUrl = null;
		if (!SteamManager.IsInitialized)
		{
			return false;
		}
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return SteamGameServerUGC.GetQueryUGCPreviewURL(queryHandle, index, out previewUrl, 2048U);
		}
		return SteamUGC.GetQueryUGCPreviewURL(queryHandle, index, out previewUrl, 2048U);
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x0002C5FA File Offset: 0x0002A7FA
	private static bool GetQueryUGCMetadata(UGCQueryHandle_t queryHandle, uint index, out string metadata)
	{
		metadata = null;
		if (!SteamManager.IsInitialized)
		{
			return false;
		}
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return SteamGameServerUGC.GetQueryUGCMetadata(queryHandle, index, out metadata, 8000U);
		}
		return SteamUGC.GetQueryUGCMetadata(queryHandle, index, out metadata, 8000U);
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x0002C62C File Offset: 0x0002A82C
	public static void DownloadItem(ulong itemId)
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		Debug.Log(string.Format("[SteamWorkshopManager] Downloading item {0}", itemId));
		EventManager.TriggerEvent("Event_OnItemDownloadStarted", new Dictionary<string, object>
		{
			{
				"itemId",
				itemId
			}
		});
		bool flag;
		if (ApplicationManager.IsDedicatedGameServer)
		{
			flag = SteamGameServerUGC.DownloadItem(new PublishedFileId_t(itemId), true);
		}
		else
		{
			flag = SteamUGC.DownloadItem(new PublishedFileId_t(itemId), true);
		}
		if (!flag)
		{
			EventManager.TriggerEvent("Event_OnItemDownloadFailed", new Dictionary<string, object>
			{
				{
					"itemId",
					itemId
				}
			});
		}
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x0002C6BC File Offset: 0x0002A8BC
	public static void DeleteItem(ulong itemId)
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		Debug.Log(string.Format("[SteamWorkshopManager] Deleting item {0}", itemId));
		if (ApplicationManager.IsDedicatedGameServer)
		{
			SteamGameServerUGC.DeleteItem(new PublishedFileId_t(itemId));
			return;
		}
		SteamUGC.DeleteItem(new PublishedFileId_t(itemId));
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x0002C6FC File Offset: 0x0002A8FC
	public static void SubscribeItem(ulong itemId)
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		Debug.Log(string.Format("[SteamWorkshopManager] Subscribing item {0}", itemId));
		EventManager.TriggerEvent("Event_OnItemSubscribeStarted", new Dictionary<string, object>
		{
			{
				"itemId",
				itemId
			}
		});
		if (ApplicationManager.IsDedicatedGameServer)
		{
			SteamGameServerUGC.SubscribeItem(new PublishedFileId_t(itemId));
			return;
		}
		SteamUGC.SubscribeItem(new PublishedFileId_t(itemId));
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x0002C768 File Offset: 0x0002A968
	public static void UnsubscribeItem(ulong itemId)
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		Debug.Log(string.Format("[SteamWorkshopManager] Unsubscribing item {0}", itemId));
		EventManager.TriggerEvent("Event_OnItemUnsubscribeStarted", new Dictionary<string, object>
		{
			{
				"itemId",
				itemId
			}
		});
		if (ApplicationManager.IsDedicatedGameServer)
		{
			SteamGameServerUGC.UnsubscribeItem(new PublishedFileId_t(itemId));
			return;
		}
		SteamUGC.UnsubscribeItem(new PublishedFileId_t(itemId));
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x0002C7D4 File Offset: 0x0002A9D4
	private static void OnDownloadItemResult(DownloadItemResult_t response)
	{
		if (response.m_unAppID != new AppId_t(2994020U))
		{
			return;
		}
		if (response.m_eResult == EResult.k_EResultOK)
		{
			EventManager.TriggerEvent("Event_OnItemDownloadSucceeded", new Dictionary<string, object>
			{
				{
					"itemId",
					(ulong)response.m_nPublishedFileId
				}
			});
			SteamWorkshopManager.AddInstalledItem((ulong)response.m_nPublishedFileId);
			return;
		}
		EventManager.TriggerEvent("Event_OnItemDownloadFailed", new Dictionary<string, object>
		{
			{
				"itemId",
				(ulong)response.m_nPublishedFileId
			}
		});
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x0002C867 File Offset: 0x0002AA67
	private static void OnUserSubscribedItemsListChanged(UserSubscribedItemsListChanged_t response)
	{
		if (response.m_nAppID != new AppId_t(2994020U))
		{
			return;
		}
		SteamWorkshopManager.VerifyItemIntegrity();
	}

	// Token: 0x0600093A RID: 2362 RVA: 0x0002C888 File Offset: 0x0002AA88
	private static void OnRemoteStorageSubscribePublishedFileResult(RemoteStorageSubscribePublishedFileResult_t response)
	{
		if (response.m_eResult == EResult.k_EResultOK)
		{
			EventManager.TriggerEvent("Event_OnItemSubscribeSucceeded", new Dictionary<string, object>
			{
				{
					"itemId",
					(ulong)response.m_nPublishedFileId
				}
			});
			return;
		}
		EventManager.TriggerEvent("Event_OnItemSubscribeFailed", new Dictionary<string, object>
		{
			{
				"itemId",
				(ulong)response.m_nPublishedFileId
			}
		});
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x0002C8F4 File Offset: 0x0002AAF4
	private static void OnRemoteStorageUnsubscribePublishedFileResult(RemoteStorageUnsubscribePublishedFileResult_t response)
	{
		if (response.m_eResult == EResult.k_EResultOK)
		{
			EventManager.TriggerEvent("Event_OnItemUnsubscribeSucceeded", new Dictionary<string, object>
			{
				{
					"itemId",
					(ulong)response.m_nPublishedFileId
				}
			});
			if (SteamWorkshopManager.IsItemInstalled((ulong)response.m_nPublishedFileId))
			{
				SteamWorkshopManager.DeleteItem((ulong)response.m_nPublishedFileId);
				return;
			}
		}
		else
		{
			EventManager.TriggerEvent("Event_OnItemUnsubscribeFailed", new Dictionary<string, object>
			{
				{
					"itemId",
					(ulong)response.m_nPublishedFileId
				}
			});
		}
	}

	// Token: 0x0600093C RID: 2364 RVA: 0x0002C981 File Offset: 0x0002AB81
	private static void OnDeleteItemResult(DeleteItemResult_t response)
	{
		if (response.m_eResult == EResult.k_EResultOK)
		{
			SteamWorkshopManager.RemoveInstalledItem((ulong)response.m_nPublishedFileId);
		}
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x0002C99C File Offset: 0x0002AB9C
	private static void OnUGCQueryCompleted(SteamUGCQueryCompleted_t response, bool bIOFailure)
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		if (response.m_eResult != EResult.k_EResultOK)
		{
			return;
		}
		for (uint num = 0U; num < response.m_unNumResultsReturned; num += 1U)
		{
			SteamUGCDetails_t steamUGCDetails_t;
			if (SteamWorkshopManager.GetQueryUGCResult(response.m_handle, num, out steamUGCDetails_t))
			{
				PublishedFileId_t nPublishedFileId = steamUGCDetails_t.m_nPublishedFileId;
				string rgchTitle = steamUGCDetails_t.m_rgchTitle;
				string rgchDescription = steamUGCDetails_t.m_rgchDescription;
				string value;
				SteamWorkshopManager.GetQueryUGCMetadata(response.m_handle, num, out value);
				string value2;
				SteamWorkshopManager.GetQueryUGCPreviewURL(response.m_handle, num, out value2);
				EventManager.TriggerEvent("Event_OnItemDetails", new Dictionary<string, object>
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
						value2
					},
					{
						"metadata",
						value
					}
				});
			}
		}
		if (SteamWorkshopManager.UGCQueryCompletedCallResultMap.ContainsKey(response.m_handle))
		{
			SteamWorkshopManager.UGCQueryCompletedCallResultMap[response.m_handle].Dispose();
			SteamWorkshopManager.UGCQueryCompletedCallResultMap.Remove(response.m_handle);
		}
		if (ApplicationManager.IsDedicatedGameServer)
		{
			SteamGameServerUGC.ReleaseQueryUGCRequest(response.m_handle);
			return;
		}
		SteamUGC.ReleaseQueryUGCRequest(response.m_handle);
	}

	// Token: 0x04000548 RID: 1352
	public static List<InstalledItem> InstalledItems = new List<InstalledItem>();

	// Token: 0x04000549 RID: 1353
	private static Callback<DownloadItemResult_t> DownloadItemResult;

	// Token: 0x0400054A RID: 1354
	private static Callback<UserSubscribedItemsListChanged_t> UserSubscribedItemsListChanged;

	// Token: 0x0400054B RID: 1355
	private static Callback<RemoteStorageSubscribePublishedFileResult_t> RemoteStorageSubscribePublishedFileResult;

	// Token: 0x0400054C RID: 1356
	private static Callback<RemoteStorageUnsubscribePublishedFileResult_t> RemoteStorageUnsubscribePublishedFileResult;

	// Token: 0x0400054D RID: 1357
	private static Callback<DeleteItemResult_t> DeleteItemResult;

	// Token: 0x0400054E RID: 1358
	private static Dictionary<UGCQueryHandle_t, CallResult<SteamUGCQueryCompleted_t>> UGCQueryCompletedCallResultMap = new Dictionary<UGCQueryHandle_t, CallResult<SteamUGCQueryCompleted_t>>();
}
