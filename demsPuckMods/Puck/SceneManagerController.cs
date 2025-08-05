using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000094 RID: 148
public class SceneManagerController : MonoBehaviour
{
	// Token: 0x060003AE RID: 942 RVA: 0x00009501 File Offset: 0x00007701
	private void Awake()
	{
		this.sceneManager = base.GetComponent<SceneManager>();
	}

	// Token: 0x060003AF RID: 943 RVA: 0x0001AD9C File Offset: 0x00018F9C
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnBeforeServerStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnBeforeServerStarted));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnTransportFailure", new Action<Dictionary<string, object>>(this.Event_Client_OnTransportFailure));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_Client_OnClientStopped));
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x0001ADFC File Offset: 0x00018FFC
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnBeforeServerStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnBeforeServerStarted));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnTransportFailure", new Action<Dictionary<string, object>>(this.Event_Client_OnTransportFailure));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_Client_OnClientStopped));
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x0000950F File Offset: 0x0000770F
	private void Event_Server_OnBeforeServerStarted(Dictionary<string, object> message)
	{
		this.sceneManager.LoadLevel1Scene();
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x0000951C File Offset: 0x0000771C
	private void Event_Client_OnTransportFailure(Dictionary<string, object> message)
	{
		this.sceneManager.LoadChangingRoomScene();
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x0000951C File Offset: 0x0000771C
	private void Event_Client_OnClientStopped(Dictionary<string, object> message)
	{
		this.sceneManager.LoadChangingRoomScene();
	}

	// Token: 0x0400021D RID: 541
	private SceneManager sceneManager;
}
