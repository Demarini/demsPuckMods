using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000102 RID: 258
public class ReplayManager : MonoBehaviourSingleton<ReplayManager>
{
	// Token: 0x06000709 RID: 1801 RVA: 0x0002268A File Offset: 0x0002088A
	public override void Awake()
	{
		base.Awake();
		this.ReplayRecorder = base.GetComponent<ReplayRecorder>();
		this.ReplayPlayer = base.GetComponent<ReplayPlayer>();
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x000226AA File Offset: 0x000208AA
	public void Server_StartRecording()
	{
		this.ReplayRecorder.Server_StartRecording(this.tickRate);
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x000226BD File Offset: 0x000208BD
	public void Server_StopRecording()
	{
		this.ReplayRecorder.Server_StopRecording();
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x000226CC File Offset: 0x000208CC
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

	// Token: 0x0600070D RID: 1805 RVA: 0x0002271F File Offset: 0x0002091F
	public void Server_StopReplaying()
	{
		this.ReplayPlayer.Server_StopReplay();
	}

	// Token: 0x0400045F RID: 1119
	[Header("Settings")]
	[SerializeField]
	private int tickRate = 15;

	// Token: 0x04000460 RID: 1120
	public ReplayRecorder ReplayRecorder;

	// Token: 0x04000461 RID: 1121
	public ReplayPlayer ReplayPlayer;
}
