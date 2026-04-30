using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000002 RID: 2
public class BaseCamera : NetworkBehaviour
{
	// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
	public virtual void Awake()
	{
		this.UnityCamera = base.GetComponent<Camera>();
		this.AudioListener = base.GetComponent<AudioListener>();
		this.UnityCamera.enabled = this.IsEnabled;
		this.AudioListener.enabled = this.IsEnabled;
	}

	// Token: 0x06000002 RID: 2 RVA: 0x0000208C File Offset: 0x0000028C
	public virtual void Start()
	{
		EventManager.TriggerEvent("Event_OnBaseCameraStarted", new Dictionary<string, object>
		{
			{
				"baseCamera",
				this
			}
		});
	}

	// Token: 0x06000003 RID: 3 RVA: 0x000020A9 File Offset: 0x000002A9
	public override void OnDestroy()
	{
		this.Disable();
		EventManager.TriggerEvent("Event_OnBaseCameraDestroyed", new Dictionary<string, object>
		{
			{
				"baseCamera",
				this
			}
		});
		base.OnDestroy();
	}

	// Token: 0x06000004 RID: 4 RVA: 0x000020D3 File Offset: 0x000002D3
	public virtual void OnTick(float deltaTime)
	{
	}

	// Token: 0x06000005 RID: 5 RVA: 0x000020D8 File Offset: 0x000002D8
	public virtual bool Enable()
	{
		if (this.IsEnabled)
		{
			return false;
		}
		this.IsEnabled = true;
		this.UnityCamera.enabled = this.IsEnabled;
		this.AudioListener.enabled = this.IsEnabled;
		EventManager.TriggerEvent("Event_OnBaseCameraEnabled", new Dictionary<string, object>
		{
			{
				"baseCamera",
				this
			}
		});
		return true;
	}

	// Token: 0x06000006 RID: 6 RVA: 0x00002134 File Offset: 0x00000334
	public virtual bool Disable()
	{
		if (!this.IsEnabled)
		{
			return false;
		}
		this.IsEnabled = false;
		this.UnityCamera.enabled = this.IsEnabled;
		this.AudioListener.enabled = this.IsEnabled;
		EventManager.TriggerEvent("Event_OnBaseCameraDisabled", new Dictionary<string, object>
		{
			{
				"baseCamera",
				this
			}
		});
		return true;
	}

	// Token: 0x06000007 RID: 7 RVA: 0x00002190 File Offset: 0x00000390
	public virtual void SetFieldOfView(float fieldOfView)
	{
		this.UnityCamera.fieldOfView = fieldOfView;
	}

	// Token: 0x06000009 RID: 9 RVA: 0x000021A8 File Offset: 0x000003A8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x0600000A RID: 10 RVA: 0x000021BE File Offset: 0x000003BE
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x0600000B RID: 11 RVA: 0x000021C8 File Offset: 0x000003C8
	protected internal override string __getTypeName()
	{
		return "BaseCamera";
	}

	// Token: 0x04000001 RID: 1
	[Header("Settings")]
	public global::CameraType Type;

	// Token: 0x04000002 RID: 2
	[HideInInspector]
	public Camera UnityCamera;

	// Token: 0x04000003 RID: 3
	[HideInInspector]
	public AudioListener AudioListener;

	// Token: 0x04000004 RID: 4
	[HideInInspector]
	public bool IsEnabled;
}
