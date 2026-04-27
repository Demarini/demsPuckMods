using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000F0 RID: 240
public class PuckManager : MonoBehaviourSingleton<PuckManager>
{
	// Token: 0x060006E7 RID: 1767 RVA: 0x00022118 File Offset: 0x00020318
	public void AddPuckPosition(PuckPosition puckPosition)
	{
		Debug.Log(string.Format("[PuckManager] Added puck position for phase {0}", puckPosition.Phase));
		this.puckPositions.Add(puckPosition);
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x00022140 File Offset: 0x00020340
	public void RemovePuckPosition(PuckPosition puckPosition)
	{
		this.puckPositions.Remove(puckPosition);
	}

	// Token: 0x060006E9 RID: 1769 RVA: 0x0002214F File Offset: 0x0002034F
	public void AddPuck(Puck puck)
	{
		this.pucks.Add(puck);
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x0002215D File Offset: 0x0002035D
	public void RemovePuck(Puck puck)
	{
		this.pucks.Remove(puck);
	}

	// Token: 0x060006EB RID: 1771 RVA: 0x0002216C File Offset: 0x0002036C
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

	// Token: 0x060006EC RID: 1772 RVA: 0x000221A7 File Offset: 0x000203A7
	public List<Puck> GetReplayPucks()
	{
		return (from puck in this.pucks
		where puck.IsReplay.Value
		select puck).ToList<Puck>();
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x000221D8 File Offset: 0x000203D8
	public Puck GetPuck(bool includeReplay = false)
	{
		return this.GetPucks(includeReplay).FirstOrDefault((Puck puck) => puck);
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x00022208 File Offset: 0x00020408
	public Puck GetPlayerPuck(ulong clientId)
	{
		Player playerByClientId = MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId);
		if (!playerByClientId)
		{
			return null;
		}
		if (!playerByClientId.Stick)
		{
			return null;
		}
		return NetworkingUtils.GetPuckFromNetworkObjectReference(playerByClientId.Stick.NetworkObjectCollisionRecorder.NetworkObjectCollisions.LastOrDefault<NetworkObjectCollision>().NetworkObjectReference);
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x0002225C File Offset: 0x0002045C
	public Puck GetPuckByNetworkObjectId(ulong networkObjectId)
	{
		return this.GetPucks(false).FirstOrDefault((Puck puck) => puck.NetworkObjectId == networkObjectId);
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x00022290 File Offset: 0x00020490
	public Puck GetReplayPuckByNetworkObjectId(ulong networkObjectId)
	{
		return this.GetReplayPucks().FirstOrDefault((Puck puck) => puck.NetworkObjectId == networkObjectId);
	}

	// Token: 0x060006F1 RID: 1777 RVA: 0x000222C4 File Offset: 0x000204C4
	public Puck Server_SpawnPuck(Vector3 position, Quaternion rotation, bool isReplay = false)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return null;
		}
		Puck puck = Object.Instantiate<Puck>(this.puckPrefab, position, rotation);
		puck.InitializeNetworkVariables(isReplay);
		puck.NetworkObject.Spawn(false);
		Debug.Log(string.Format("[PuckManager] Spawned puck {0}", puck.NetworkObjectId));
		return puck;
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x0002231B File Offset: 0x0002051B
	public void Server_DespawnPuck(Puck puck)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		puck.NetworkObject.Despawn(true);
		Debug.Log(string.Format("[PuckManager] Despawned puck {0}", puck.NetworkObjectId));
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x00022350 File Offset: 0x00020550
	public void Server_DespawnPucks(bool includeReplay = false)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		Debug.Log(string.Format("[PuckManager] Despawning {0} pucks (includeReplay: {1})", this.pucks.Count, includeReplay));
		foreach (Puck puck in this.pucks.ToList<Puck>())
		{
			if (includeReplay || !puck.IsReplay.Value)
			{
				this.Server_DespawnPuck(puck);
			}
		}
	}

	// Token: 0x060006F4 RID: 1780 RVA: 0x000223EC File Offset: 0x000205EC
	public void Server_SpawnPucksForPhase(GamePhase phase)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		Debug.Log(string.Format("[PuckManager] Spawning pucks for phase {0}", phase));
		this.puckPositions.FindAll((PuckPosition puckPosition) => puckPosition.Phase == phase).ForEach(delegate(PuckPosition puckPosition)
		{
			this.Server_SpawnPuck(puckPosition.transform.position, puckPosition.transform.rotation, false);
		});
	}

	// Token: 0x04000423 RID: 1059
	[Header("Prefabs")]
	[SerializeField]
	private Puck puckPrefab;

	// Token: 0x04000424 RID: 1060
	private List<PuckPosition> puckPositions = new List<PuckPosition>();

	// Token: 0x04000425 RID: 1061
	private List<Puck> pucks = new List<Puck>();
}
