using System;
using UnityEngine;

// Token: 0x02000236 RID: 566
public class Vector3PIDController
{
	// Token: 0x06000FDF RID: 4063 RVA: 0x00045BE0 File Offset: 0x00043DE0
	public Vector3PIDController(float proportionalGain = 0f, float integralGain = 0f, float derivativeGain = 0f)
	{
		this.proportionalGain = proportionalGain;
		this.integralGain = integralGain;
		this.derivativeGain = derivativeGain;
	}

	// Token: 0x06000FE0 RID: 4064 RVA: 0x00045C60 File Offset: 0x00043E60
	public Vector3 Update(float deltaTime, Vector3 currentValue, Vector3 targetValue)
	{
		if (deltaTime <= 0f)
		{
			return Vector3.zero;
		}
		Vector3 a = targetValue - currentValue;
		Vector3 vector = (a - this.errorLast) / deltaTime;
		this.errorLast = a;
		Vector3 a2 = (currentValue - this.valueLast) / deltaTime;
		this.valueLast = currentValue;
		Vector3 value = this.integrationStored + a * deltaTime;
		this.integrationStored = this.ClampVector3(value, -this.integralSaturation, this.integralSaturation);
		Vector3 vector2 = Vector3.zero;
		if (this.derivativeInitialized)
		{
			if (this.derivativeMeasurement == DerivativeMeasurement.Velocity)
			{
				vector2 = -a2;
			}
			else
			{
				vector2 = vector;
			}
			vector2 = Vector3.Lerp(this.derivativeLast, vector2, this.derivativeSmoothing);
			this.derivativeLast = vector2;
		}
		else
		{
			this.derivativeInitialized = true;
			this.derivativeLast = Vector3.zero;
		}
		Vector3 a3 = this.proportionalGain * a;
		Vector3 b = this.integralGain * this.integrationStored;
		Vector3 b2 = this.derivativeGain * vector2;
		Vector3 value2 = a3 + b + b2;
		return this.ClampVector3(value2, this.outputMin, this.outputMax);
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x00045D8C File Offset: 0x00043F8C
	public Vector3 UpdateAngle(float deltaTime, Vector3 currentValue, Vector3 targetValue)
	{
		if (deltaTime <= 0f)
		{
			return Vector3.zero;
		}
		Vector3 a = this.AngleDifference(targetValue, currentValue);
		Vector3 vector = this.AngleDifference(a, this.errorLast) / deltaTime;
		this.errorLast = a;
		Vector3 a2 = this.AngleDifference(currentValue, this.valueLast) / deltaTime;
		this.valueLast = currentValue;
		Vector3 value = this.integrationStored + a * deltaTime;
		this.integrationStored = this.ClampVector3(value, -this.integralSaturation, this.integralSaturation);
		Vector3 vector2 = Vector3.zero;
		if (this.derivativeInitialized)
		{
			if (this.derivativeMeasurement == DerivativeMeasurement.Velocity)
			{
				vector2 = -a2;
			}
			else
			{
				vector2 = vector;
			}
			vector2 = Vector3.Lerp(this.derivativeLast, vector2, this.derivativeSmoothing);
			this.derivativeLast = vector2;
		}
		else
		{
			this.derivativeInitialized = true;
			this.derivativeLast = Vector3.zero;
		}
		Vector3 a3 = this.proportionalGain * a;
		Vector3 b = this.integralGain * this.integrationStored;
		Vector3 b2 = this.derivativeGain * vector2;
		Vector3 value2 = a3 + b + b2;
		return this.ClampVector3(value2, this.outputMin, this.outputMax);
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x00045EBA File Offset: 0x000440BA
	public void Reset()
	{
		this.derivativeInitialized = false;
		this.errorLast = Vector3.zero;
		this.valueLast = Vector3.zero;
		this.integrationStored = Vector3.zero;
		this.derivativeLast = Vector3.zero;
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x00045EEF File Offset: 0x000440EF
	private Vector3 AngleDifference(Vector3 a, Vector3 b)
	{
		return new Vector3(Mathf.DeltaAngle(b.x, a.x), Mathf.DeltaAngle(b.y, a.y), Mathf.DeltaAngle(b.z, a.z));
	}

	// Token: 0x06000FE4 RID: 4068 RVA: 0x00045F29 File Offset: 0x00044129
	private Vector3 ClampVector3(Vector3 value, float min, float max)
	{
		return new Vector3(Mathf.Clamp(value.x, min, max), Mathf.Clamp(value.y, min, max), Mathf.Clamp(value.z, min, max));
	}

	// Token: 0x04000940 RID: 2368
	public float proportionalGain;

	// Token: 0x04000941 RID: 2369
	public float integralGain;

	// Token: 0x04000942 RID: 2370
	public float integralSaturation = float.MaxValue;

	// Token: 0x04000943 RID: 2371
	public float derivativeGain;

	// Token: 0x04000944 RID: 2372
	public float derivativeSmoothing = 1f;

	// Token: 0x04000945 RID: 2373
	public float outputMin = float.MinValue;

	// Token: 0x04000946 RID: 2374
	public float outputMax = float.MaxValue;

	// Token: 0x04000947 RID: 2375
	private Vector3 errorLast = Vector3.zero;

	// Token: 0x04000948 RID: 2376
	private Vector3 valueLast = Vector3.zero;

	// Token: 0x04000949 RID: 2377
	private Vector3 integrationStored = Vector3.zero;

	// Token: 0x0400094A RID: 2378
	private Vector3 derivativeLast = Vector3.zero;

	// Token: 0x0400094B RID: 2379
	private bool derivativeInitialized;

	// Token: 0x0400094C RID: 2380
	public DerivativeMeasurement derivativeMeasurement;
}
