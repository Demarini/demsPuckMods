using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using UnityEngine;

// Token: 0x02000007 RID: 7
public class EventDependencyTrigger : MonoBehaviour
{
	// Token: 0x06000023 RID: 35 RVA: 0x000024DC File Offset: 0x000006DC
	private void Start()
	{
		foreach (string eventName in this.dependencyEvents.Keys)
		{
			EventManager.AddEventListener(eventName, new Action<Dictionary<string, object>>(this.OnDependencyEvent));
		}
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00002540 File Offset: 0x00000740
	private void OnDestroy()
	{
		Tween tween = this.timeoutTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		foreach (string eventName in this.dependencyEvents.Keys)
		{
			EventManager.RemoveEventListener(eventName, new Action<Dictionary<string, object>>(this.OnDependencyEvent));
		}
	}

	// Token: 0x06000025 RID: 37 RVA: 0x000025B4 File Offset: 0x000007B4
	private void OnDependencyEvent(Dictionary<string, object> message)
	{
		string key = (string)message["eventName"];
		if (!this.dependencyEvents.ContainsValue(true))
		{
			Tween tween = this.timeoutTween;
			if (tween != null)
			{
				tween.Kill(false);
			}
			this.timeoutTween = DOVirtual.DelayedCall(this.timeout, delegate
			{
				Debug.LogWarning("[EventDependencyTrigger] Event " + this.triggerEventName + " timed out waiting for dependencies");
				if (this.isRepeating)
				{
					this.Reset();
				}
			}, true);
		}
		this.dependencyEvents[key] = true;
		if (!this.dependencyEvents.ContainsValue(false))
		{
			Debug.Log("[EventDependencyTrigger] All dependencies met, triggering event " + this.triggerEventName);
			EventManager.TriggerEvent(this.triggerEventName, null);
			if (this.isRepeating)
			{
				this.Reset();
			}
		}
	}

	// Token: 0x06000026 RID: 38 RVA: 0x0000265A File Offset: 0x0000085A
	private void Reset()
	{
		Tween tween = this.timeoutTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.dependencyEvents.Keys.ToList<string>().ForEach(delegate(string key)
		{
			this.dependencyEvents[key] = false;
		});
	}

	// Token: 0x04000012 RID: 18
	[Header("Settings")]
	[SerializeField]
	private SerializedDictionary<string, bool> dependencyEvents = new SerializedDictionary<string, bool>();

	// Token: 0x04000013 RID: 19
	[SerializeField]
	private string triggerEventName;

	// Token: 0x04000014 RID: 20
	[SerializeField]
	private float timeout = 3f;

	// Token: 0x04000015 RID: 21
	[SerializeField]
	private bool isRepeating = true;

	// Token: 0x04000016 RID: 22
	private Tween timeoutTween;
}
