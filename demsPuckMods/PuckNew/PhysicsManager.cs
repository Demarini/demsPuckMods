using System;
using UnityEngine;

// Token: 0x020000E1 RID: 225
public class PhysicsManager : MonoBehaviourSingleton<PhysicsManager>
{
	// Token: 0x170000CF RID: 207
	// (get) Token: 0x060006AA RID: 1706 RVA: 0x000217B5 File Offset: 0x0001F9B5
	[HideInInspector]
	public int TickRate
	{
		get
		{
			return this.tickRate;
		}
	}

	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x060006AB RID: 1707 RVA: 0x000217BD File Offset: 0x0001F9BD
	[HideInInspector]
	public float TickInterval
	{
		get
		{
			return 1f / (float)this.TickRate;
		}
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x000217CC File Offset: 0x0001F9CC
	public override void Awake()
	{
		base.Awake();
		Physics.simulationMode = this.simulationMode;
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x000217E0 File Offset: 0x0001F9E0
	private void Update()
	{
		if (this.simulationMode != SimulationMode.Script)
		{
			return;
		}
		this.tickAccumulator += Time.deltaTime;
		if (this.tickAccumulator >= this.TickInterval)
		{
			Time.fixedDeltaTime = this.TickInterval;
			Physics.Simulate(Time.fixedDeltaTime);
			this.tickAccumulator -= this.TickInterval;
		}
	}

	// Token: 0x0400040B RID: 1035
	[Header("Settings")]
	[SerializeField]
	private SimulationMode simulationMode = SimulationMode.Script;

	// Token: 0x0400040C RID: 1036
	[SerializeField]
	private int tickRate = 50;

	// Token: 0x0400040D RID: 1037
	private float tickAccumulator;
}
