using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000002 RID: 2
public class BaseCamera : NetworkBehaviour
{
	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000001 RID: 1 RVA: 0x00006BC8 File Offset: 0x00004DC8
	public bool IsEnabled
	{
		get
		{
			return this.CameraComponent.enabled;
		}
	}

	// Token: 0x06000002 RID: 2 RVA: 0x00006BD5 File Offset: 0x00004DD5
	public virtual void Awake()
	{
		this.CameraComponent = base.GetComponent<Camera>();
		this.AudioListener = base.GetComponent<AudioListener>();
	}

	// Token: 0x06000003 RID: 3 RVA: 0x00006BEF File Offset: 0x00004DEF
	public override void OnNetworkDespawn()
	{
		if (this.IsEnabled)
		{
			this.Disable();
		}
		base.OnNetworkDespawn();
	}

	// Token: 0x06000004 RID: 4 RVA: 0x00006C05 File Offset: 0x00004E05
	public override void OnDestroy()
	{
		if (this.IsEnabled)
		{
			this.Disable();
		}
		base.OnDestroy();
	}

	// Token: 0x06000005 RID: 5 RVA: 0x00006C1B File Offset: 0x00004E1B
	public virtual void OnTick(float deltaTime)
	{
	}

	// Token: 0x06000006 RID: 6 RVA: 0x0000F518 File Offset: 0x0000D718
	public virtual void Enable()
	{
		if (this.IsEnabled)
		{
			return;
		}
		this.CameraComponent.enabled = true;
		this.AudioListener.enabled = true;
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnBaseCameraEnabled", new Dictionary<string, object>
		{
			{
				"baseCamera",
				this
			}
		});
	}

	// Token: 0x06000007 RID: 7 RVA: 0x0000F568 File Offset: 0x0000D768
	public virtual void Disable()
	{
		if (!this.IsEnabled)
		{
			return;
		}
		this.CameraComponent.enabled = false;
		this.AudioListener.enabled = false;
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnBaseCameraDisabled", new Dictionary<string, object>
		{
			{
				"baseCamera",
				this
			}
		});
	}

	// Token: 0x06000008 RID: 8 RVA: 0x00006C1D File Offset: 0x00004E1D
	public virtual void SetFieldOfView(float fieldOfView)
	{
		this.CameraComponent.fieldOfView = fieldOfView;
	}

	// Token: 0x0600000A RID: 10 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x0600000B RID: 11 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x0600000C RID: 12 RVA: 0x00006C3D File Offset: 0x00004E3D
	protected internal override string __getTypeName()
	{
		return "BaseCamera";
	}

	// Token: 0x04000001 RID: 1
	public Camera CameraComponent;

	// Token: 0x04000002 RID: 2
	public AudioListener AudioListener;
}
