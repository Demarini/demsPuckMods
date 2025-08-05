using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000109 RID: 265
public class UIDebugController : NetworkBehaviour
{
	// Token: 0x06000943 RID: 2371 RVA: 0x0000CC8F File Offset: 0x0000AE8F
	private void Awake()
	{
		this.uiDebug = base.GetComponent<UIDebug>();
	}

	// Token: 0x06000944 RID: 2372 RVA: 0x0000CC9D File Offset: 0x0000AE9D
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnDebugChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnDebugChanged));
		this.uiDebug.SetBuildLabelText("B" + Application.version);
	}

	// Token: 0x06000945 RID: 2373 RVA: 0x0000CCD4 File Offset: 0x0000AED4
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnDebugChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnDebugChanged));
		base.OnDestroy();
	}

	// Token: 0x06000946 RID: 2374 RVA: 0x0000CCF7 File Offset: 0x0000AEF7
	private void Event_Client_OnDebugChanged(Dictionary<string, object> message)
	{
		if ((int)message["value"] > 0)
		{
			this.uiDebug.Show();
			return;
		}
		this.uiDebug.Hide(true);
	}

	// Token: 0x06000948 RID: 2376 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000949 RID: 2377 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x0600094A RID: 2378 RVA: 0x0000CD24 File Offset: 0x0000AF24
	protected internal override string __getTypeName()
	{
		return "UIDebugController";
	}

	// Token: 0x040005A1 RID: 1441
	private UIDebug uiDebug;
}
