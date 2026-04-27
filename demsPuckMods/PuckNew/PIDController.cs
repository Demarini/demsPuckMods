using System;
using UnityEngine;

// Token: 0x020001F0 RID: 496
public class PIDController
{
	// Token: 0x06000E54 RID: 3668 RVA: 0x00042B48 File Offset: 0x00040D48
	public PIDController(float proportionalGain = 0f, float integralGain = 0f, float derivativeGain = 0f)
	{
		this.proportionalGain = proportionalGain;
		this.integralGain = integralGain;
		this.derivativeGain = derivativeGain;
	}

	// Token: 0x06000E55 RID: 3669 RVA: 0x00042B9C File Offset: 0x00040D9C
	public float Update(float deltaTime, float currentValue, float targetValue)
	{
		if (deltaTime <= 0f)
		{
			return 0f;
		}
		float num = targetValue - currentValue;
		float num2 = (num - this.errorLast) / deltaTime;
		this.errorLast = num;
		float num3 = (currentValue - this.valueLast) / deltaTime;
		this.valueLast = currentValue;
		float value = this.integrationStored + num * deltaTime;
		this.integrationStored = Mathf.Clamp(value, -this.integralSaturation, this.integralSaturation);
		float num4 = 0f;
		if (this.derivativeInitialized)
		{
			if (this.derivativeMeasurement == DerivativeMeasurement.Velocity)
			{
				num4 = -num3;
			}
			else
			{
				num4 = num2;
			}
			num4 = Mathf.Lerp(this.derivativeLast, num4, this.derivativeSmoothing);
			this.derivativeLast = num4;
		}
		else
		{
			this.derivativeInitialized = true;
			this.derivativeLast = 0f;
		}
		float num5 = this.proportionalGain * num;
		float num6 = this.integralGain * this.integrationStored;
		float num7 = this.derivativeGain * num4;
		return Mathf.Clamp(num5 + num6 + num7, this.outputMin, this.outputMax);
	}

	// Token: 0x06000E56 RID: 3670 RVA: 0x00042C90 File Offset: 0x00040E90
	public float UpdateAngle(float deltaTime, float currentValue, float targetValue)
	{
		if (deltaTime <= 0f)
		{
			return 0f;
		}
		float num = this.AngleDifference(targetValue, currentValue);
		float num2 = this.AngleDifference(num, this.errorLast) / deltaTime;
		this.errorLast = num;
		float num3 = this.AngleDifference(currentValue, this.valueLast) / deltaTime;
		this.valueLast = currentValue;
		float value = this.integrationStored + num * deltaTime;
		this.integrationStored = Mathf.Clamp(value, -this.integralSaturation, this.integralSaturation);
		float num4 = 0f;
		if (this.derivativeInitialized)
		{
			if (this.derivativeMeasurement == DerivativeMeasurement.Velocity)
			{
				num4 = -num3;
			}
			else
			{
				num4 = num2;
			}
			num4 = Mathf.Lerp(this.derivativeLast, num4, this.derivativeSmoothing);
			this.derivativeLast = num4;
		}
		else
		{
			this.derivativeInitialized = true;
			this.derivativeLast = 0f;
		}
		float num5 = this.proportionalGain * num;
		float num6 = this.integralGain * this.integrationStored;
		float num7 = this.derivativeGain * num4;
		return Mathf.Clamp(num5 + num6 + num7, this.outputMin, this.outputMax);
	}

	// Token: 0x06000E57 RID: 3671 RVA: 0x00042D90 File Offset: 0x00040F90
	public void Reset()
	{
		this.derivativeInitialized = false;
		this.errorLast = 0f;
		this.valueLast = 0f;
		this.integrationStored = 0f;
		this.derivativeLast = 0f;
	}

	// Token: 0x06000E58 RID: 3672 RVA: 0x00042DC5 File Offset: 0x00040FC5
	private float AngleDifference(float a, float b)
	{
		return Mathf.DeltaAngle(b, a);
	}

	// Token: 0x040008A2 RID: 2210
	public float proportionalGain;

	// Token: 0x040008A3 RID: 2211
	public float integralGain;

	// Token: 0x040008A4 RID: 2212
	public float integralSaturation = float.MaxValue;

	// Token: 0x040008A5 RID: 2213
	public float derivativeGain;

	// Token: 0x040008A6 RID: 2214
	public float derivativeSmoothing = 1f;

	// Token: 0x040008A7 RID: 2215
	public float outputMin = float.MinValue;

	// Token: 0x040008A8 RID: 2216
	public float outputMax = float.MaxValue;

	// Token: 0x040008A9 RID: 2217
	private float errorLast;

	// Token: 0x040008AA RID: 2218
	private float valueLast;

	// Token: 0x040008AB RID: 2219
	private float integrationStored;

	// Token: 0x040008AC RID: 2220
	private float derivativeLast;

	// Token: 0x040008AD RID: 2221
	private bool derivativeInitialized;

	// Token: 0x040008AE RID: 2222
	public DerivativeMeasurement derivativeMeasurement;
}
