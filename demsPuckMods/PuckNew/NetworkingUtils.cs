using System;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020001E8 RID: 488
public static class NetworkingUtils
{
	// Token: 0x06000E2F RID: 3631 RVA: 0x0004279C File Offset: 0x0004099C
	public static Player GetPlayerFromNetworkObjectReference(NetworkObjectReference reference)
	{
		NetworkObject networkObject;
		if (reference.TryGet(out networkObject, null))
		{
			return networkObject.GetComponent<Player>();
		}
		return null;
	}

	// Token: 0x06000E30 RID: 3632 RVA: 0x000427C0 File Offset: 0x000409C0
	public static PlayerPosition GetPlayerPositionFromNetworkObjectReference(NetworkObjectReference reference)
	{
		NetworkObject networkObject;
		if (reference.TryGet(out networkObject, null))
		{
			return networkObject.GetComponent<PlayerPosition>();
		}
		return null;
	}

	// Token: 0x06000E31 RID: 3633 RVA: 0x000427E4 File Offset: 0x000409E4
	public static Puck GetPuckFromNetworkObjectReference(NetworkObjectReference reference)
	{
		NetworkObject networkObject;
		if (reference.TryGet(out networkObject, null))
		{
			return networkObject.GetComponent<Puck>();
		}
		return null;
	}

	// Token: 0x06000E32 RID: 3634 RVA: 0x00042808 File Offset: 0x00040A08
	public static byte CompressFloatToByte(float value, float minValue, float maxValue)
	{
		int num = -128;
		int num2 = 127;
		float t = Mathf.InverseLerp(minValue, maxValue, value);
		return (byte)((sbyte)Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp((float)num, (float)num2, t)), num, num2));
	}

	// Token: 0x06000E33 RID: 3635 RVA: 0x0004283C File Offset: 0x00040A3C
	public static short CompressFloatToShort(float value, float minValue, float maxValue)
	{
		int num = -32768;
		int num2 = 32767;
		float t = Mathf.InverseLerp(minValue, maxValue, value);
		return (short)Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp((float)num, (float)num2, t)), num, num2);
	}

	// Token: 0x06000E34 RID: 3636 RVA: 0x00042878 File Offset: 0x00040A78
	public static float DecompressByteToFloat(byte compressedValue, float minValue, float maxValue)
	{
		float num = (float)-128;
		int num2 = 127;
		float t = Mathf.InverseLerp(num, (float)num2, (float)((sbyte)compressedValue));
		return Mathf.Lerp(minValue, maxValue, t);
	}

	// Token: 0x06000E35 RID: 3637 RVA: 0x000428A0 File Offset: 0x00040AA0
	public static float DecompressShortToFloat(short compressedValue, float minValue, float maxValue)
	{
		float num = (float)-32768;
		int num2 = 32767;
		float t = Mathf.InverseLerp(num, (float)num2, (float)compressedValue);
		return Mathf.Lerp(minValue, maxValue, t);
	}
}
