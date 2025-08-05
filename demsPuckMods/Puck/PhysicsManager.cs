using System;
using UnityEngine;

// Token: 0x02000062 RID: 98
public class PhysicsManager : MonoBehaviourSingleton<PhysicsManager>
{
	// Token: 0x17000049 RID: 73
	// (get) Token: 0x060002BA RID: 698 RVA: 0x00008A98 File Offset: 0x00006C98
	[HideInInspector]
	public int TickRate
	{
		get
		{
			return this.tickRate;
		}
	}

	// Token: 0x1700004A RID: 74
	// (get) Token: 0x060002BB RID: 699 RVA: 0x00008AA0 File Offset: 0x00006CA0
	[HideInInspector]
	public float TickInterval
	{
		get
		{
			return 1f / (float)this.TickRate;
		}
	}

	// Token: 0x060002BC RID: 700 RVA: 0x00008AAF File Offset: 0x00006CAF
	public override void Awake()
	{
		base.Awake();
		Physics.simulationMode = this.simulationMode;
	}

	// Token: 0x060002BD RID: 701 RVA: 0x00017ED4 File Offset: 0x000160D4
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

	// Token: 0x04000192 RID: 402
	[Header("Settings")]
	[SerializeField]
	private SimulationMode simulationMode = SimulationMode.Script;

	// Token: 0x04000193 RID: 403
	[SerializeField]
	private int tickRate = 50;

	// Token: 0x04000194 RID: 404
	private float tickAccumulator;
}
