using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A8 RID: 168
public class SpectatorManager : NetworkBehaviourSingleton<SpectatorManager>
{
	// Token: 0x060004DD RID: 1245 RVA: 0x0000A03D File Offset: 0x0000823D
	public void SetSpectatorPositions(List<Transform> spectatorPositions)
	{
		this.spectatorPositions = spectatorPositions;
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnSpectatorPositionsSet", null);
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x0002064C File Offset: 0x0001E84C
	public void SpawnSpectators()
	{
		if (Application.isBatchMode)
		{
			return;
		}
		foreach (Transform transform in this.spectatorPositions)
		{
			if (UnityEngine.Random.value < this.spectatorDensity)
			{
				Spectator item = UnityEngine.Object.Instantiate<Spectator>(this.spectatorPrefab, transform.position, transform.rotation, transform.parent);
				this.spectators.Add(item);
			}
		}
		Debug.Log(string.Format("[SpectatorManager] Spawned {0} spectators", this.spectators.Count));
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x000206F8 File Offset: 0x0001E8F8
	public void ClearSpectators()
	{
		foreach (Spectator spectator in this.spectators)
		{
			UnityEngine.Object.Destroy(spectator.gameObject);
		}
		this.spectators.Clear();
		Debug.Log("[SpectatorManager] Cleared spectators");
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x00020764 File Offset: 0x0001E964
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x0000A07F File Offset: 0x0000827F
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x0000A089 File Offset: 0x00008289
	protected internal override string __getTypeName()
	{
		return "SpectatorManager";
	}

	// Token: 0x040002C4 RID: 708
	[Header("Settings")]
	[SerializeField]
	private float spectatorDensity = 0.25f;

	// Token: 0x040002C5 RID: 709
	[Header("Prefabs")]
	[SerializeField]
	private Spectator spectatorPrefab;

	// Token: 0x040002C6 RID: 710
	private List<Transform> spectatorPositions = new List<Transform>();

	// Token: 0x040002C7 RID: 711
	private List<Spectator> spectators = new List<Spectator>();
}
