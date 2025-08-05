using System;

// Token: 0x02000157 RID: 343
public struct ExponentialMovingAverage
{
	// Token: 0x06000C26 RID: 3110 RVA: 0x000415FC File Offset: 0x0003F7FC
	public ExponentialMovingAverage(int n)
	{
		this.alpha = 2.0 / (double)(n + 1);
		this.initialized = false;
		this.Value = 0.0;
		this.Variance = 0.0;
		this.StandardDeviation = 0.0;
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x00041654 File Offset: 0x0003F854
	public void Add(double newValue)
	{
		if (this.initialized)
		{
			double num = newValue - this.Value;
			this.Value += this.alpha * num;
			this.Variance = (1.0 - this.alpha) * (this.Variance + this.alpha * num * num);
			this.StandardDeviation = Math.Sqrt(this.Variance);
			return;
		}
		this.Value = newValue;
		this.initialized = true;
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x0000EFA0 File Offset: 0x0000D1A0
	public void Reset()
	{
		this.initialized = false;
		this.Value = 0.0;
		this.Variance = 0.0;
		this.StandardDeviation = 0.0;
	}

	// Token: 0x040006FC RID: 1788
	private readonly double alpha;

	// Token: 0x040006FD RID: 1789
	private bool initialized;

	// Token: 0x040006FE RID: 1790
	public double Value;

	// Token: 0x040006FF RID: 1791
	public double Variance;

	// Token: 0x04000700 RID: 1792
	public double StandardDeviation;
}
