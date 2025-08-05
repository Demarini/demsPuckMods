using System;
using UnityEngine;

// Token: 0x02000168 RID: 360
[Serializable]
public class SnapshotInterpolationSettings
{
	// Token: 0x04000722 RID: 1826
	[Header("Buffering")]
	[Tooltip("Local simulation is behind by sendInterval * multiplier seconds.\n\nThis guarantees that we always have enough snapshots in the buffer to mitigate lags & jitter.\n\nIncrease this if the simulation isn't smooth. By default, it should be around 2.")]
	public double bufferTimeMultiplier = 2.0;

	// Token: 0x04000723 RID: 1827
	[Tooltip("If a client can't process snapshots fast enough, don't store too many.")]
	public int bufferLimit = 32;

	// Token: 0x04000724 RID: 1828
	[Header("Catchup / Slowdown")]
	[Tooltip("Slowdown begins when the local timeline is moving too fast towards remote time. Threshold is in frames worth of snapshots.\n\nThis needs to be negative.\n\nDon't modify unless you know what you are doing.")]
	public float catchupNegativeThreshold = -1f;

	// Token: 0x04000725 RID: 1829
	[Tooltip("Catchup begins when the local timeline is moving too slow and getting too far away from remote time. Threshold is in frames worth of snapshots.\n\nThis needs to be positive.\n\nDon't modify unless you know what you are doing.")]
	public float catchupPositiveThreshold = 1f;

	// Token: 0x04000726 RID: 1830
	[Tooltip("Local timeline acceleration in % while catching up.")]
	[Range(0f, 1f)]
	public double catchupSpeed = 0.019999999552965164;

	// Token: 0x04000727 RID: 1831
	[Tooltip("Local timeline slowdown in % while slowing down.")]
	[Range(0f, 1f)]
	public double slowdownSpeed = 0.03999999910593033;

	// Token: 0x04000728 RID: 1832
	[Tooltip("Catchup/Slowdown is adjusted over n-second exponential moving average.")]
	public int driftEmaDuration = 1;

	// Token: 0x04000729 RID: 1833
	[Header("Dynamic Adjustment")]
	[Tooltip("Automatically adjust bufferTimeMultiplier for smooth results.\nSets a low multiplier on stable connections, and a high multiplier on jittery connections.")]
	public bool dynamicAdjustment = true;

	// Token: 0x0400072A RID: 1834
	[Tooltip("Safety buffer that is always added to the dynamic bufferTimeMultiplier adjustment.")]
	public float dynamicAdjustmentTolerance = 1f;

	// Token: 0x0400072B RID: 1835
	[Tooltip("Dynamic adjustment is computed over n-second exponential moving average standard deviation.")]
	public int deliveryTimeEmaDuration = 2;
}
