using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000A0 RID: 160
public static class EventManager
{
	// Token: 0x0600051D RID: 1309 RVA: 0x000020D3 File Offset: 0x000002D3
	public static void Initialize()
	{
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x000020D3 File Offset: 0x000002D3
	public static void Dispose()
	{
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x0001C504 File Offset: 0x0001A704
	public static void AddEventListener(string eventName, Action<Dictionary<string, object>> listener)
	{
		if (!EventManager.events.ContainsKey(eventName))
		{
			EventManager.events.Add(eventName, null);
		}
		Dictionary<string, Action<Dictionary<string, object>>> dictionary = EventManager.events;
		dictionary[eventName] = (Action<Dictionary<string, object>>)Delegate.Combine(dictionary[eventName], listener);
	}

	// Token: 0x06000520 RID: 1312 RVA: 0x0001C54C File Offset: 0x0001A74C
	public static void RemoveEventListener(string eventName, Action<Dictionary<string, object>> listener)
	{
		if (!EventManager.events.ContainsKey(eventName))
		{
			Debug.LogWarning("[EventManager] Tried to remove listener for event " + eventName + ", but no listener was registered for it");
			return;
		}
		Dictionary<string, Action<Dictionary<string, object>>> dictionary = EventManager.events;
		dictionary[eventName] = (Action<Dictionary<string, object>>)Delegate.Remove(dictionary[eventName], listener);
		if (EventManager.events[eventName] == null)
		{
			EventManager.events.Remove(eventName);
		}
	}

	// Token: 0x06000521 RID: 1313 RVA: 0x0001C5B8 File Offset: 0x0001A7B8
	public static void TriggerEvent(string eventName, Dictionary<string, object> message = null)
	{
		if (message == null)
		{
			message = new Dictionary<string, object>
			{
				{
					"eventName",
					eventName
				}
			};
		}
		else if (!message.ContainsKey("eventName"))
		{
			message.Add("eventName", eventName);
		}
		if (!EventManager.events.ContainsKey(eventName))
		{
			return;
		}
		bool flag = eventName.StartsWith("Event_Server_");
		bool flag2 = eventName.StartsWith("Event_Client_");
		bool flag3 = eventName.StartsWith("Event_Everyone_");
		if ((flag2 || flag || flag3) && (!NetworkManager.Singleton || !NetworkManager.Singleton.IsListening))
		{
			Debug.LogWarning("[EventManager] Triggering network event " + eventName + " without a NetworkManager listening");
		}
		Action<Dictionary<string, object>> action = EventManager.events[eventName];
		if (action == null)
		{
			return;
		}
		action(message);
	}

	// Token: 0x04000320 RID: 800
	private static Dictionary<string, Action<Dictionary<string, object>>> events = new Dictionary<string, Action<Dictionary<string, object>>>();
}
