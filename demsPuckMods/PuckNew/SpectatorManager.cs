using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200012B RID: 299
public class SpectatorManager : MonoBehaviourSingleton<SpectatorManager>
{
	// Token: 0x060008BC RID: 2236 RVA: 0x0002A44C File Offset: 0x0002864C
	private void Update()
	{
		int count = this.spectatorPositionSpectatorMap.Values.Count;
		if (count == 0)
		{
			return;
		}
		List<Spectator> list = this.spectatorPositionSpectatorMap.Values.ToList<Spectator>();
		for (int i = 0; i < this.spectatorUpdatesPerFrame; i++)
		{
			int index = (this.updateBatch + i) % count;
			list[index].UpdateAnimation();
		}
		this.updateBatch = (this.updateBatch + this.spectatorUpdatesPerFrame) % count;
	}

	// Token: 0x060008BD RID: 2237 RVA: 0x0002A4BC File Offset: 0x000286BC
	public void RegisterSpectatorPosition(SpectatorPosition position)
	{
		if (this.spectatorPositionSpectatorMap.ContainsKey(position))
		{
			return;
		}
		if (Random.value > this.spectatorDensity)
		{
			return;
		}
		Spectator spectator = Object.Instantiate<Spectator>(this.spectatorPrefab, position.transform.position, position.transform.rotation, base.transform);
		this.spectatorPositionSpectatorMap[position] = spectator;
		spectator.RandomizeAppearance();
		spectator.PlayAnimation(this.currentAnimation);
		spectator.LookTarget = this.currentLookTarget;
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x0002A539 File Offset: 0x00028739
	public void UnregisterSpectatorPosition(SpectatorPosition position)
	{
		if (!this.spectatorPositionSpectatorMap.ContainsKey(position))
		{
			return;
		}
		Object.Destroy(this.spectatorPositionSpectatorMap[position].gameObject);
		this.spectatorPositionSpectatorMap.Remove(position);
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x0002A570 File Offset: 0x00028770
	public void SetSpectatorLookTarget(Transform lookTarget)
	{
		this.currentLookTarget = lookTarget;
		foreach (Spectator spectator in this.spectatorPositionSpectatorMap.Values)
		{
			spectator.LookTarget = this.currentLookTarget;
		}
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x0002A5D4 File Offset: 0x000287D4
	public void SetSpectatorAnimation(string animationName)
	{
		this.currentAnimation = animationName;
		foreach (Spectator spectator in this.spectatorPositionSpectatorMap.Values)
		{
			spectator.PlayAnimation(this.currentAnimation);
		}
	}

	// Token: 0x0400051F RID: 1311
	[Header("Settings")]
	[SerializeField]
	private float spectatorDensity = 0.25f;

	// Token: 0x04000520 RID: 1312
	[SerializeField]
	private int spectatorUpdatesPerFrame = 8;

	// Token: 0x04000521 RID: 1313
	[Header("Prefabs")]
	[SerializeField]
	private Spectator spectatorPrefab;

	// Token: 0x04000522 RID: 1314
	private Dictionary<SpectatorPosition, Spectator> spectatorPositionSpectatorMap = new Dictionary<SpectatorPosition, Spectator>();

	// Token: 0x04000523 RID: 1315
	private Transform currentLookTarget;

	// Token: 0x04000524 RID: 1316
	private string currentAnimation = "Seated";

	// Token: 0x04000525 RID: 1317
	private int updateBatch;
}
