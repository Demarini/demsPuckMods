using System;
using System.Collections.Generic;
using DG.Tweening;

// Token: 0x02000123 RID: 291
public class TimeoutManager : MonoBehaviourSingleton<TimeoutManager>
{
	// Token: 0x0600082E RID: 2094 RVA: 0x00027868 File Offset: 0x00025A68
	public void Dispose()
	{
		foreach (Tween t in this.steamIdTweenMap.Values)
		{
			t.Kill(false);
		}
		this.steamIdTweenMap.Clear();
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x000278CC File Offset: 0x00025ACC
	public void AddSteamIdTimeout(string steamId, float timeout)
	{
		if (this.steamIdTweenMap.ContainsKey(steamId))
		{
			return;
		}
		this.steamIdTweenMap[steamId] = DOVirtual.DelayedCall(timeout, delegate
		{
			this.RemoveSteamIdTimeout(steamId);
		}, true);
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x00027925 File Offset: 0x00025B25
	public void RemoveSteamIdTimeout(string steamId)
	{
		if (!this.steamIdTweenMap.ContainsKey(steamId))
		{
			return;
		}
		this.steamIdTweenMap[steamId].Kill(false);
		this.steamIdTweenMap.Remove(steamId);
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x00027955 File Offset: 0x00025B55
	public bool IsSteamIdTimedOut(string steamId)
	{
		return this.steamIdTweenMap.ContainsKey(steamId);
	}

	// Token: 0x040004D3 RID: 1235
	private Dictionary<string, Tween> steamIdTweenMap = new Dictionary<string, Tween>();
}
