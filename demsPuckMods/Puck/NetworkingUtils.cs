using System;
using Unity.Netcode;

// Token: 0x0200015C RID: 348
public static class NetworkingUtils
{
	// Token: 0x06000C3A RID: 3130 RVA: 0x000417B4 File Offset: 0x0003F9B4
	public static Player GetPlayerFromNetworkObjectReference(NetworkObjectReference reference)
	{
		NetworkObject networkObject;
		if (reference.TryGet(out networkObject, null))
		{
			return networkObject.GetComponent<Player>();
		}
		return null;
	}
}
