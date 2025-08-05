using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace __GEN
{
	// Token: 0x0200017A RID: 378
	internal class NetworkVariableSerializationHelper
	{
		// Token: 0x06000CB9 RID: 3257 RVA: 0x0004C090 File Offset: 0x0004A290
		[RuntimeInitializeOnLoadMethod]
		internal static void InitializeSerialization()
		{
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_UnmanagedINetworkSerializable<NetworkObjectCollision>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedIEquatable<NetworkObjectCollision>();
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_UnmanagedINetworkSerializable<NetworkObjectReference>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedIEquatable<NetworkObjectReference>();
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_UnmanagedByMemcpy<bool>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedIEquatable<bool>();
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_UnmanagedINetworkSerializable<GameState>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedIEquatable<GameState>();
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_UnmanagedByMemcpy<byte>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedIEquatable<byte>();
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_UnmanagedByMemcpy<PlayerState>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedValueEquals<PlayerState>();
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_FixedString<FixedString32Bytes>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedIEquatable<FixedString32Bytes>();
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_UnmanagedByMemcpy<PlayerHandedness>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedValueEquals<PlayerHandedness>();
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_UnmanagedByMemcpy<PlayerTeam>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedValueEquals<PlayerTeam>();
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_UnmanagedByMemcpy<PlayerRole>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedValueEquals<PlayerRole>();
		}
	}
}
