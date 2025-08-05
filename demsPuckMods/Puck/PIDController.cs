using System;
using UnityEngine;

// Token: 0x02000164 RID: 356
public class PIDController
{
	// Token: 0x06000C59 RID: 3161 RVA: 0x000419FC File Offset: 0x0003FBFC
	public PIDController(float proportionalGain = 0f, float integralGain = 0f, float derivativeGain = 0f)
	{
		this.proportionalGain = proportionalGain;
		this.integralGain = integralGain;
		this.derivativeGain = derivativeGain;
	}

	// Token: 0x06000C5A RID: 3162 RVA: 0x00041A50 File Offset: 0x0003FC50
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

	// Token: 0x06000C5B RID: 3163 RVA: 0x00041B44 File Offset: 0x0003FD44
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

	// Token: 0x06000C5C RID: 3164 RVA: 0x0000F0EE File Offset: 0x0000D2EE
	public void Reset()
	{
		this.derivativeInitialized = false;
		this.errorLast = 0f;
		this.valueLast = 0f;
		this.integrationStored = 0f;
		this.derivativeLast = 0f;
	}

	// Token: 0x06000C5D RID: 3165 RVA: 0x0000F123 File Offset: 0x0000D323
	private float AngleDifference(float a, float b)
	{
		return Mathf.DeltaAngle(b, a);
	}

	// Token: 0x04000715 RID: 1813
	public float proportionalGain;

	// Token: 0x04000716 RID: 1814
	public float integralGain;

	// Token: 0x04000717 RID: 1815
	public float integralSaturation = float.MaxValue;

	// Token: 0x04000718 RID: 1816
	public float derivativeGain;

	// Token: 0x04000719 RID: 1817
	public float derivativeSmoothing = 1f;

	// Token: 0x0400071A RID: 1818
	public float outputMin = float.MinValue;

	// Token: 0x0400071B RID: 1819
	public float outputMax = float.MaxValue;

	// Token: 0x0400071C RID: 1820
	private float errorLast;

	// Token: 0x0400071D RID: 1821
	private float valueLast;

	// Token: 0x0400071E RID: 1822
	private float integrationStored;

	// Token: 0x0400071F RID: 1823
	private float derivativeLast;

	// Token: 0x04000720 RID: 1824
	private bool derivativeInitialized;

	// Token: 0x04000721 RID: 1825
	public DerivativeMeasurement derivativeMeasurement;
}
