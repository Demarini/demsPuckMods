using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace __GEN
{
	// Token: 0x02000241 RID: 577
	internal class NetworkVariableSerializationHelper
	{
		// Token: 0x06000FEC RID: 4076 RVA: 0x0004607C File Offset: 0x0004427C
		[RuntimeInitializeOnLoadMethod]
		internal static void InitializeSerialization()
		{
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_UnmanagedINetworkSerializable<NetworkObjectCollision>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedIEquatable<NetworkObjectCollision>();
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_UnmanagedINetworkSerializable<NetworkObjectReference>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedIEquatable<NetworkObjectReference>();
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_UnmanagedByMemcpy<byte>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedIEquatable<byte>();
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_UnmanagedByMemcpy<bool>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedIEquatable<bool>();
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_UnmanagedINetworkSerializable<PlayerGameState>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedIEquatable<PlayerGameState>();
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_UnmanagedINetworkSerializable<PlayerCustomizationState>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedIEquatable<PlayerCustomizationState>();
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_UnmanagedByMemcpy<PlayerHandedness>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedValueEquals<PlayerHandedness>();
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_FixedString<FixedString32Bytes>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedIEquatable<FixedString32Bytes>();
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_UnmanagedINetworkSerializable<GameState>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedIEquatable<GameState>();
			NetworkVariableSerializationTypedInitializers.InitializeSerializer_UnmanagedINetworkSerializable<Server>();
			NetworkVariableSerializationTypedInitializers.InitializeEqualityChecker_UnmanagedIEquatable<Server>();
		}
	}
}
