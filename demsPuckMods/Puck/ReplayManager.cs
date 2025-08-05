using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000089 RID: 137
public class ReplayManager : NetworkBehaviourSingleton<ReplayManager>
{
	// Token: 0x0600035C RID: 860 RVA: 0x0000921E File Offset: 0x0000741E
	public override void Awake()
	{
		base.Awake();
		this.ReplayRecorder = base.GetComponent<ReplayRecorder>();
		this.ReplayPlayer = base.GetComponent<ReplayPlayer>();
	}

	// Token: 0x0600035D RID: 861 RVA: 0x0000923E File Offset: 0x0000743E
	public void Server_StartRecording()
	{
		this.ReplayRecorder.Server_StartRecording(this.tickRate);
	}

	// Token: 0x0600035E RID: 862 RVA: 0x00009251 File Offset: 0x00007451
	public void Server_StopRecording()
	{
		this.ReplayRecorder.Server_StopRecording();
	}

	// Token: 0x0600035F RID: 863 RVA: 0x00018EFC File Offset: 0x000170FC
	public void Server_StartReplaying(float secondsToReplay)
	{
		SortedList<int, List<ValueTuple<string, object>>> sortedList = new SortedList<int, List<ValueTuple<string, object>>>(this.ReplayRecorder.EventMap);
		if (sortedList.Count == 0)
		{
			return;
		}
		int num = sortedList.Keys.Max();
		int num2 = (int)((float)this.tickRate * secondsToReplay);
		int fromTick = num - num2;
		this.ReplayPlayer.Server_StartReplay(sortedList, this.tickRate, fromTick);
	}

	// Token: 0x06000360 RID: 864 RVA: 0x0000925E File Offset: 0x0000745E
	public void Server_StopReplaying()
	{
		this.ReplayPlayer.Server_StopReplay();
	}

	// Token: 0x06000362 RID: 866 RVA: 0x00018F50 File Offset: 0x00017150
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000363 RID: 867 RVA: 0x0000927B File Offset: 0x0000747B
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000364 RID: 868 RVA: 0x00009285 File Offset: 0x00007485
	protected internal override string __getTypeName()
	{
		return "ReplayManager";
	}

	// Token: 0x04000201 RID: 513
	[Header("Settings")]
	[SerializeField]
	private int tickRate = 15;

	// Token: 0x04000202 RID: 514
	public ReplayRecorder ReplayRecorder;

	// Token: 0x04000203 RID: 515
	public ReplayPlayer ReplayPlayer;
}
