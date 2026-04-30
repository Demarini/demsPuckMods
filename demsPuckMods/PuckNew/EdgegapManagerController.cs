using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200011C RID: 284
public class EdgegapManagerController : MonoBehaviour
{
	// Token: 0x060007CC RID: 1996 RVA: 0x00025F94 File Offset: 0x00024194
	public void Awake()
	{
		this.edgegapManager = base.GetComponent<EdgegapManager>();
		EventManager.AddEventListener("Event_OnServerStateChanged", new Action<Dictionary<string, object>>(this.Event_OnServerStateChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerAdded", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerAdded));
		EventManager.AddEventListener("Event_Everyone_OnPlayerRemoved", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerRemoved));
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x00025FEF File Offset: 0x000241EF
	private void Start()
	{
		this.edgegapManager.StartDependencyTimeout(EdgegapDependency.IsAuthenticated);
		this.edgegapManager.StartDependencyTimeout(EdgegapDependency.IsOccupied);
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x0002600C File Offset: 0x0002420C
	private void OnDestroy()
	{
		this.edgegapManager.StopDependencyTimeout(EdgegapDependency.IsAuthenticated);
		this.edgegapManager.StopDependencyTimeout(EdgegapDependency.IsOccupied);
		EventManager.RemoveEventListener("Event_OnServerStateChanged", new Action<Dictionary<string, object>>(this.Event_OnServerStateChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerAdded", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerAdded));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerRemoved", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerRemoved));
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x00026074 File Offset: 0x00024274
	private void Event_OnServerStateChanged(Dictionary<string, object> message)
	{
		ref ServerState ptr = (ServerState)message["oldServerState"];
		ServerState serverState = (ServerState)message["newServerState"];
		if (ptr.AuthenticationPhase != serverState.AuthenticationPhase)
		{
			this.edgegapManager.SetDependency(EdgegapDependency.IsAuthenticated, serverState.AuthenticationPhase == AuthenticationPhase.Authenticated);
		}
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x000260C4 File Offset: 0x000242C4
	private void Event_Everyone_OnPlayerAdded(Dictionary<string, object> message)
	{
		this.edgegapManager.SetDependency(EdgegapDependency.IsOccupied, true);
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x000260D3 File Offset: 0x000242D3
	private void Event_Everyone_OnPlayerRemoved(Dictionary<string, object> message)
	{
		if (MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false).Count == 0)
		{
			this.edgegapManager.StartDependencyTimeout(EdgegapDependency.IsOccupied);
		}
	}

	// Token: 0x040004AB RID: 1195
	private EdgegapManager edgegapManager;
}
