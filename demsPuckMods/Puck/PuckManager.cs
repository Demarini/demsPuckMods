using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000076 RID: 118
public class PuckManager : NetworkBehaviourSingleton<PuckManager>
{
	// Token: 0x0600032B RID: 811 RVA: 0x00008F4F File Offset: 0x0000714F
	public void SetPuckPositions(List<PuckPosition> puckPositions)
	{
		this.puckPositions = puckPositions;
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPuckPositionsSet", null);
	}

	// Token: 0x0600032C RID: 812 RVA: 0x00008F68 File Offset: 0x00007168
	public void ClearPuckPositions()
	{
		this.puckPositions.Clear();
	}

	// Token: 0x0600032D RID: 813 RVA: 0x00008F75 File Offset: 0x00007175
	public void AddPuck(Puck puck)
	{
		this.pucks.Add(puck);
	}

	// Token: 0x0600032E RID: 814 RVA: 0x00008F83 File Offset: 0x00007183
	public void RemovePuck(Puck puck)
	{
		this.pucks.Remove(puck);
	}

	// Token: 0x0600032F RID: 815 RVA: 0x00008F92 File Offset: 0x00007192
	public List<Puck> GetPucks(bool includeReplay = false)
	{
		if (includeReplay)
		{
			return this.pucks;
		}
		return (from puck in this.pucks
		where !puck.IsReplay.Value
		select puck).ToList<Puck>();
	}

	// Token: 0x06000330 RID: 816 RVA: 0x00008FCD File Offset: 0x000071CD
	public List<Puck> GetReplayPucks()
	{
		return (from puck in this.pucks
		where puck.IsReplay.Value
		select puck).ToList<Puck>();
	}

	// Token: 0x06000331 RID: 817 RVA: 0x00008FFE File Offset: 0x000071FE
	public Puck GetPuck(bool includeReplay = false)
	{
		return this.GetPucks(includeReplay).FirstOrDefault((Puck puck) => puck);
	}

	// Token: 0x06000332 RID: 818 RVA: 0x00018A0C File Offset: 0x00016C0C
	public Puck GetPlayerPuck(ulong clientId)
	{
		Player playerByClientId = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId);
		if (!playerByClientId)
		{
			return null;
		}
		if (!playerByClientId.Stick)
		{
			return null;
		}
		NetworkObject networkObject;
		if (playerByClientId.Stick.NetworkObjectCollisionBuffer.Buffer.LastOrDefault<NetworkObjectCollision>().NetworkObjectReference.TryGet(out networkObject, null))
		{
			return networkObject.GetComponent<Puck>();
		}
		return null;
	}

	// Token: 0x06000333 RID: 819 RVA: 0x00018A70 File Offset: 0x00016C70
	public Puck GetPuckByNetworkObjectId(ulong networkObjectId)
	{
		return this.GetPucks(false).FirstOrDefault((Puck puck) => puck.NetworkObjectId == networkObjectId);
	}

	// Token: 0x06000334 RID: 820 RVA: 0x00018AA4 File Offset: 0x00016CA4
	public Puck GetReplayPuckByNetworkObjectId(ulong networkObjectId)
	{
		return this.GetReplayPucks().FirstOrDefault((Puck puck) => puck.NetworkObjectId == networkObjectId);
	}

	// Token: 0x06000335 RID: 821 RVA: 0x00018AD8 File Offset: 0x00016CD8
	public Puck Server_SpawnPuck(Vector3 position, Quaternion rotation, Vector3 velocity, bool isReplay = false)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return null;
		}
		Puck puck = UnityEngine.Object.Instantiate<Puck>(this.puckPrefab, position, rotation);
		puck.IsReplay.Value = isReplay;
		puck.Rigidbody.AddForce(velocity, ForceMode.VelocityChange);
		puck.NetworkObject.Spawn(false);
		Debug.Log(string.Format("[PuckManager] Spawned puck !{0}!", puck.NetworkObjectId));
		return puck;
	}

	// Token: 0x06000336 RID: 822 RVA: 0x0000902B File Offset: 0x0000722B
	public void Server_DespawnPuck(Puck puck)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		puck.NetworkObject.Despawn(true);
		Debug.Log(string.Format("[PuckManager] Despawned puck !{0}!", puck.NetworkObjectId));
	}

	// Token: 0x06000337 RID: 823 RVA: 0x00018B44 File Offset: 0x00016D44
	public void Server_DespawnPucks(bool includeReplay = false)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		Debug.Log(string.Format("[PuckManager] Despawning {0} pucks, includeReplay: {1}", this.pucks.Count, includeReplay));
		foreach (Puck puck in this.pucks.ToList<Puck>())
		{
			if (includeReplay || !puck.IsReplay.Value)
			{
				this.Server_DespawnPuck(puck);
			}
		}
	}

	// Token: 0x06000338 RID: 824 RVA: 0x00018BE0 File Offset: 0x00016DE0
	public void Server_SpawnPucksForPhase(GamePhase phase)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		Debug.Log(string.Format("[PuckManager] Spawning {0} pucks for phase {1}", this.puckPositions.Count, phase));
		foreach (PuckPosition puckPosition in this.puckPositions)
		{
			if (puckPosition.Phase == phase)
			{
				this.Server_SpawnPuck(puckPosition.transform.position, puckPosition.transform.rotation, Vector3.zero, false);
			}
		}
	}

	// Token: 0x0600033A RID: 826 RVA: 0x00018C8C File Offset: 0x00016E8C
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x0600033B RID: 827 RVA: 0x0000907E File Offset: 0x0000727E
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x0600033C RID: 828 RVA: 0x00009088 File Offset: 0x00007288
	protected internal override string __getTypeName()
	{
		return "PuckManager";
	}

	// Token: 0x040001B8 RID: 440
	[Header("Prefabs")]
	[SerializeField]
	private Puck puckPrefab;

	// Token: 0x040001B9 RID: 441
	private List<PuckPosition> puckPositions = new List<PuckPosition>();

	// Token: 0x040001BA RID: 442
	private List<Puck> pucks = new List<Puck>();
}
