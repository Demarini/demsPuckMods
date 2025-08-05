using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

// Token: 0x0200000B RID: 11
public class EventDependencyTrigger : MonoBehaviour
{
	// Token: 0x0600006A RID: 106 RVA: 0x000105F4 File Offset: 0x0000E7F4
	private void Start()
	{
		foreach (KeyValuePair<string, bool> keyValuePair in this.dependencyEvents.ToList<KeyValuePair<string, bool>>())
		{
			string key = keyValuePair.Key;
			MonoBehaviourSingleton<EventManager>.Instance.AddEventListener(key, new Action<Dictionary<string, object>>(this.OnDependencyEvent));
		}
	}

	// Token: 0x0600006B RID: 107 RVA: 0x00010664 File Offset: 0x0000E864
	private void OnDestroy()
	{
		foreach (KeyValuePair<string, bool> keyValuePair in this.dependencyEvents.ToList<KeyValuePair<string, bool>>())
		{
			string key = keyValuePair.Key;
			MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener(key, new Action<Dictionary<string, object>>(this.OnDependencyEvent));
		}
	}

	// Token: 0x0600006C RID: 108 RVA: 0x000106D4 File Offset: 0x0000E8D4
	private void Update()
	{
		if (!this.isStarted)
		{
			return;
		}
		if (Time.time - this.startTime > this.timeout)
		{
			Debug.LogWarning("EventDependencyTrigger on " + base.gameObject.name + ": Timeout");
			foreach (KeyValuePair<string, bool> keyValuePair in this.dependencyEvents.ToList<KeyValuePair<string, bool>>())
			{
				string key = keyValuePair.Key;
				bool value = keyValuePair.Value;
				Debug.Log(string.Format("Dependency event {0} is met: {1}", key, value));
			}
			return;
		}
	}

	// Token: 0x0600006D RID: 109 RVA: 0x00010788 File Offset: 0x0000E988
	private void OnDependencyEvent(Dictionary<string, object> message)
	{
		string key = (string)message["eventName"];
		if (!this.dependencyEvents.ContainsValue(true))
		{
			this.isStarted = true;
			this.startTime = Time.time;
		}
		if (this.dependencyEvents.ContainsKey(key))
		{
			this.dependencyEvents[key] = true;
		}
		if (!this.dependencyEvents.ContainsValue(false) && this.isStarted)
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent(this.triggerEventName, null);
			this.isStarted = false;
			if (this.isRepeating)
			{
				foreach (KeyValuePair<string, bool> keyValuePair in this.dependencyEvents.ToList<KeyValuePair<string, bool>>())
				{
					string key2 = keyValuePair.Key;
					this.dependencyEvents[key2] = false;
				}
			}
		}
	}

	// Token: 0x0400002D RID: 45
	[Header("Settings")]
	[SerializeField]
	private SerializedDictionary<string, bool> dependencyEvents = new SerializedDictionary<string, bool>();

	// Token: 0x0400002E RID: 46
	[SerializeField]
	private string triggerEventName;

	// Token: 0x0400002F RID: 47
	[SerializeField]
	private float timeout = 3f;

	// Token: 0x04000030 RID: 48
	[SerializeField]
	private bool isRepeating = true;

	// Token: 0x04000031 RID: 49
	private bool isStarted;

	// Token: 0x04000032 RID: 50
	private float startTime;
}
