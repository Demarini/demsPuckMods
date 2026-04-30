using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000003 RID: 3
public class BaseCameraController : MonoBehaviour
{
	// Token: 0x0600000C RID: 12 RVA: 0x000021CF File Offset: 0x000003CF
	public virtual void Awake()
	{
		this.baseCamera = base.GetComponent<BaseCamera>();
		EventManager.AddEventListener("Event_OnSynchronizeObjects", new Action<Dictionary<string, object>>(this.Event_OnSynchronizeObjects));
		EventManager.AddEventListener("Event_OnFovChanged", new Action<Dictionary<string, object>>(this.Event_OnFovChanged));
	}

	// Token: 0x0600000D RID: 13 RVA: 0x00002209 File Offset: 0x00000409
	public virtual void Start()
	{
		this.baseCamera.SetFieldOfView(SettingsManager.Fov);
	}

	// Token: 0x0600000E RID: 14 RVA: 0x0000221B File Offset: 0x0000041B
	public virtual void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnSynchronizeObjects", new Action<Dictionary<string, object>>(this.Event_OnSynchronizeObjects));
		EventManager.RemoveEventListener("Event_OnFovChanged", new Action<Dictionary<string, object>>(this.Event_OnFovChanged));
	}

	// Token: 0x0600000F RID: 15 RVA: 0x00002249 File Offset: 0x00000449
	public virtual void LateUpdate()
	{
		if (NetworkManager.Singleton == null || NetworkManager.Singleton.IsHost || SettingsManager.UseNetworkSmoothing)
		{
			this.baseCamera.OnTick(Time.deltaTime);
		}
	}

	// Token: 0x06000010 RID: 16 RVA: 0x0000227C File Offset: 0x0000047C
	private void Event_OnSynchronizeObjects(Dictionary<string, object> message)
	{
		if (SettingsManager.UseNetworkSmoothing)
		{
			return;
		}
		float deltaTime = (float)message["serverDeltaTime"];
		this.baseCamera.OnTick(deltaTime);
	}

	// Token: 0x06000011 RID: 17 RVA: 0x000022B0 File Offset: 0x000004B0
	private void Event_OnFovChanged(Dictionary<string, object> message)
	{
		float fieldOfView = (float)message["value"];
		this.baseCamera.SetFieldOfView(fieldOfView);
	}

	// Token: 0x04000005 RID: 5
	private BaseCamera baseCamera;
}
