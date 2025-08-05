using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000003 RID: 3
public class BaseCameraController : NetworkBehaviour
{
	// Token: 0x0600000D RID: 13 RVA: 0x00006C44 File Offset: 0x00004E44
	public virtual void Awake()
	{
		this.baseCamera = base.GetComponent<BaseCamera>();
	}

	// Token: 0x0600000E RID: 14 RVA: 0x00006C52 File Offset: 0x00004E52
	public virtual void Start()
	{
		this.useNetworkSmoothing = (MonoBehaviourSingleton<SettingsManager>.Instance.UseNetworkSmoothing > 0);
	}

	// Token: 0x0600000F RID: 15 RVA: 0x0000F5D0 File Offset: 0x0000D7D0
	public override void OnNetworkSpawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnBaseCameraEnabled", new Action<Dictionary<string, object>>(this.Event_Client_OnBaseCameraEnabled));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnFovChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnFovChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSynchronizeObjects", new Action<Dictionary<string, object>>(this.Event_Client_OnSynchronizeObjects));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnUseNetworkSmoothingChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnUseNetworkSmoothingChanged));
		base.OnNetworkSpawn();
	}

	// Token: 0x06000010 RID: 16 RVA: 0x0000F650 File Offset: 0x0000D850
	public override void OnNetworkDespawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnBaseCameraEnabled", new Action<Dictionary<string, object>>(this.Event_Client_OnBaseCameraEnabled));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnFovChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnFovChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSynchronizeObjects", new Action<Dictionary<string, object>>(this.Event_Client_OnSynchronizeObjects));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnUseNetworkSmoothingChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnUseNetworkSmoothingChanged));
		base.OnNetworkDespawn();
	}

	// Token: 0x06000011 RID: 17 RVA: 0x00006C67 File Offset: 0x00004E67
	private void LateUpdate()
	{
		if (!base.IsSpawned)
		{
			return;
		}
		if (this.useNetworkSmoothing || NetworkManager.Singleton.IsHost)
		{
			this.baseCamera.OnTick(Time.deltaTime);
		}
	}

	// Token: 0x06000012 RID: 18 RVA: 0x00006C96 File Offset: 0x00004E96
	private void Event_Client_OnBaseCameraEnabled(Dictionary<string, object> message)
	{
		if ((BaseCamera)message["baseCamera"] != this.baseCamera)
		{
			this.baseCamera.Disable();
		}
	}

	// Token: 0x06000013 RID: 19 RVA: 0x0000F6D0 File Offset: 0x0000D8D0
	private void Event_Client_OnFovChanged(Dictionary<string, object> message)
	{
		float fieldOfView = (float)message["value"];
		this.baseCamera.SetFieldOfView(fieldOfView);
	}

	// Token: 0x06000014 RID: 20 RVA: 0x0000F6FC File Offset: 0x0000D8FC
	private void Event_Client_OnSynchronizeObjects(Dictionary<string, object> message)
	{
		if (this.useNetworkSmoothing)
		{
			return;
		}
		float deltaTime = (float)message["serverDeltaTime"];
		this.baseCamera.OnTick(deltaTime);
	}

	// Token: 0x06000015 RID: 21 RVA: 0x0000F730 File Offset: 0x0000D930
	private void Event_Client_OnUseNetworkSmoothingChanged(Dictionary<string, object> message)
	{
		bool flag = (bool)message["value"];
		this.useNetworkSmoothing = flag;
	}

	// Token: 0x06000017 RID: 23 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000018 RID: 24 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000019 RID: 25 RVA: 0x00006CC0 File Offset: 0x00004EC0
	protected internal override string __getTypeName()
	{
		return "BaseCameraController";
	}

	// Token: 0x04000003 RID: 3
	private BaseCamera baseCamera;

	// Token: 0x04000004 RID: 4
	private bool useNetworkSmoothing;
}
