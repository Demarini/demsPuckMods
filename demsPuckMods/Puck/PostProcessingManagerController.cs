using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000075 RID: 117
public class PostProcessingManagerController : MonoBehaviour
{
	// Token: 0x06000323 RID: 803 RVA: 0x00008F41 File Offset: 0x00007141
	private void Awake()
	{
		this.postProcessingManager = base.GetComponent<PostProcessingManager>();
	}

	// Token: 0x06000324 RID: 804 RVA: 0x00018808 File Offset: 0x00016A08
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnMotionBlurChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnMotionBlurChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnQualityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnQualityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnShowPuckSilhouetteChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnShowPuckSilhouetteChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnShowPuckOutlineChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnShowPuckOutlineChanged));
	}

	// Token: 0x06000325 RID: 805 RVA: 0x00018884 File Offset: 0x00016A84
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnMotionBlurChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnMotionBlurChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnQualityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnQualityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnShowPuckSilhouetteChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnShowPuckSilhouetteChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnShowPuckOutlineChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnShowPuckOutlineChanged));
	}

	// Token: 0x06000326 RID: 806 RVA: 0x00018900 File Offset: 0x00016B00
	private void Event_Client_OnMotionBlurChanged(Dictionary<string, object> message)
	{
		bool motionBlur = (bool)message["value"];
		this.postProcessingManager.SetMotionBlur(motionBlur);
	}

	// Token: 0x06000327 RID: 807 RVA: 0x0001892C File Offset: 0x00016B2C
	private void Event_Client_OnQualityChanged(Dictionary<string, object> message)
	{
		string a = (string)message["value"];
		if (a == "LOW")
		{
			this.postProcessingManager.SetMsaaSampleCount(1);
			return;
		}
		if (a == "MEDIUM")
		{
			this.postProcessingManager.SetMsaaSampleCount(2);
			return;
		}
		if (a == "HIGH")
		{
			this.postProcessingManager.SetMsaaSampleCount(4);
			return;
		}
		if (!(a == "ULTRA"))
		{
			return;
		}
		this.postProcessingManager.SetMsaaSampleCount(8);
	}

	// Token: 0x06000328 RID: 808 RVA: 0x000189B4 File Offset: 0x00016BB4
	private void Event_Client_OnShowPuckSilhouetteChanged(Dictionary<string, object> message)
	{
		bool obstructedPuck = (bool)message["value"];
		this.postProcessingManager.SetObstructedPuck(obstructedPuck);
	}

	// Token: 0x06000329 RID: 809 RVA: 0x000189E0 File Offset: 0x00016BE0
	private void Event_Client_OnShowPuckOutlineChanged(Dictionary<string, object> message)
	{
		bool puckOutline = (bool)message["value"];
		this.postProcessingManager.SetPuckOutline(puckOutline);
	}

	// Token: 0x040001B7 RID: 439
	private PostProcessingManager postProcessingManager;
}
