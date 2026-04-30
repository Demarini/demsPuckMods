using System;

// Token: 0x020001F1 RID: 497
public struct ExponentialMovingAverage
{
	// Token: 0x06000E59 RID: 3673 RVA: 0x00042DD0 File Offset: 0x00040FD0
	public ExponentialMovingAverage(int n)
	{
		this.alpha = 2.0 / (double)(n + 1);
		this.initialized = false;
		this.Value = 0.0;
		this.Variance = 0.0;
		this.StandardDeviation = 0.0;
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x00042E28 File Offset: 0x00041028
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

	// Token: 0x06000E5B RID: 3675 RVA: 0x00042EA3 File Offset: 0x000410A3
	public void Reset()
	{
		this.initialized = false;
		this.Value = 0.0;
		this.Variance = 0.0;
		this.StandardDeviation = 0.0;
	}

	// Token: 0x040008AF RID: 2223
	private readonly double alpha;

	// Token: 0x040008B0 RID: 2224
	private bool initialized;

	// Token: 0x040008B1 RID: 2225
	public double Value;

	// Token: 0x040008B2 RID: 2226
	public double Variance;

	// Token: 0x040008B3 RID: 2227
	public double StandardDeviation;
}
