using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

// Token: 0x02000033 RID: 51
public class StartDependencyTrigger : MonoBehaviour
{
	// Token: 0x06000163 RID: 355 RVA: 0x00007C8D File Offset: 0x00005E8D
	private void Start()
	{
		this.startTime = Time.time;
	}

	// Token: 0x06000164 RID: 356 RVA: 0x00012944 File Offset: 0x00010B44
	private void Update()
	{
		if (this.isTriggered)
		{
			return;
		}
		if (Time.time - this.startTime > this.timeout)
		{
			Debug.LogWarning("StartDependencyTrigger: Timeout for event " + this.eventName);
			this.isTriggered = true;
			return;
		}
		foreach (KeyValuePair<MonoBehaviour, bool> keyValuePair in this.dependencies.ToList<KeyValuePair<MonoBehaviour, bool>>())
		{
			MonoBehaviour key = keyValuePair.Key;
			this.dependencies[key] = key.didStart;
		}
		if (!this.dependencies.ContainsValue(false))
		{
			this.isTriggered = true;
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent(this.eventName, null);
		}
	}

	// Token: 0x040000BF RID: 191
	[Header("Settings")]
	[SerializeField]
	private SerializedDictionary<MonoBehaviour, bool> dependencies = new SerializedDictionary<MonoBehaviour, bool>();

	// Token: 0x040000C0 RID: 192
	[SerializeField]
	private string eventName;

	// Token: 0x040000C1 RID: 193
	[SerializeField]
	private float timeout = 3f;

	// Token: 0x040000C2 RID: 194
	private float startTime;

	// Token: 0x040000C3 RID: 195
	private bool isTriggered;
}
