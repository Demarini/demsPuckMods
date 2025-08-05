using System;
using UnityEngine;

// Token: 0x02000173 RID: 371
public class Vector3PIDController
{
	// Token: 0x06000CAC RID: 3244 RVA: 0x0004BCC4 File Offset: 0x00049EC4
	public Vector3PIDController(float proportionalGain = 0f, float integralGain = 0f, float derivativeGain = 0f)
	{
		this.proportionalGain = proportionalGain;
		this.integralGain = integralGain;
		this.derivativeGain = derivativeGain;
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x0004BD44 File Offset: 0x00049F44
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

	// Token: 0x06000CAE RID: 3246 RVA: 0x0004BE70 File Offset: 0x0004A070
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

	// Token: 0x06000CAF RID: 3247 RVA: 0x0000F444 File Offset: 0x0000D644
	public void Reset()
	{
		this.derivativeInitialized = false;
		this.errorLast = Vector3.zero;
		this.valueLast = Vector3.zero;
		this.integrationStored = Vector3.zero;
		this.derivativeLast = Vector3.zero;
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x0000F479 File Offset: 0x0000D679
	private Vector3 AngleDifference(Vector3 a, Vector3 b)
	{
		return new Vector3(Mathf.DeltaAngle(b.x, a.x), Mathf.DeltaAngle(b.y, a.y), Mathf.DeltaAngle(b.z, a.z));
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x0000F4B3 File Offset: 0x0000D6B3
	private Vector3 ClampVector3(Vector3 value, float min, float max)
	{
		return new Vector3(Mathf.Clamp(value.x, min, max), Mathf.Clamp(value.y, min, max), Mathf.Clamp(value.z, min, max));
	}

	// Token: 0x0400074F RID: 1871
	public float proportionalGain;

	// Token: 0x04000750 RID: 1872
	public float integralGain;

	// Token: 0x04000751 RID: 1873
	public float integralSaturation = float.MaxValue;

	// Token: 0x04000752 RID: 1874
	public float derivativeGain;

	// Token: 0x04000753 RID: 1875
	public float derivativeSmoothing = 1f;

	// Token: 0x04000754 RID: 1876
	public float outputMin = float.MinValue;

	// Token: 0x04000755 RID: 1877
	public float outputMax = float.MaxValue;

	// Token: 0x04000756 RID: 1878
	private Vector3 errorLast = Vector3.zero;

	// Token: 0x04000757 RID: 1879
	private Vector3 valueLast = Vector3.zero;

	// Token: 0x04000758 RID: 1880
	private Vector3 integrationStored = Vector3.zero;

	// Token: 0x04000759 RID: 1881
	private Vector3 derivativeLast = Vector3.zero;

	// Token: 0x0400075A RID: 1882
	private bool derivativeInitialized;

	// Token: 0x0400075B RID: 1883
	public DerivativeMeasurement derivativeMeasurement;
}
