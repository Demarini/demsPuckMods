using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020000D6 RID: 214
public static class PatchManager
{
	// Token: 0x0600069B RID: 1691 RVA: 0x0002150C File Offset: 0x0001F70C
	public static void Initialize()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		VisualElementHarmonyPatch.Patch();
		stopwatch.Stop();
		Debug.Log(string.Format("[PatchManager] Patching took {0}ms", stopwatch.ElapsedMilliseconds));
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x00021544 File Offset: 0x0001F744
	public static void Dispose()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		VisualElementHarmonyPatch.Unpatch();
		stopwatch.Stop();
		Debug.Log(string.Format("[PatchManager] Unpatching took {0}ms", stopwatch.ElapsedMilliseconds));
	}
}
