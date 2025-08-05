using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000018 RID: 24
public class PlayerPositionController : MonoBehaviour
{
	// Token: 0x060000B4 RID: 180 RVA: 0x00007372 File Offset: 0x00005572
	private void Awake()
	{
		this.playerPosition = base.GetComponent<PlayerPosition>();
	}

	// Token: 0x060000B5 RID: 181 RVA: 0x00007380 File Offset: 0x00005580
	public void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerDespawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerTeamChanged));
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x000073B8 File Offset: 0x000055B8
	public void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerDespawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerTeamChanged));
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x000110A4 File Offset: 0x0000F2A4
	private void Event_OnPlayerDespawned(Dictionary<string, object> message)
	{
		Player x = (Player)message["player"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (x == this.playerPosition.ClaimedBy)
		{
			this.playerPosition.Server_Unclaim();
		}
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x000110A4 File Offset: 0x0000F2A4
	private void Event_OnPlayerTeamChanged(Dictionary<string, object> message)
	{
		Player x = (Player)message["player"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (x == this.playerPosition.ClaimedBy)
		{
			this.playerPosition.Server_Unclaim();
		}
	}

	// Token: 0x0400004D RID: 77
	private PlayerPosition playerPosition;
}
