using System;
using Unity.Netcode;

// Token: 0x02000015 RID: 21
public struct NetworkObjectCollision : INetworkSerializable, IEquatable<NetworkObjectCollision>
{
	// Token: 0x0600009A RID: 154 RVA: 0x00010CCC File Offset: 0x0000EECC
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		if (serializer.IsReader)
		{
			FastBufferReader fastBufferReader = serializer.GetFastBufferReader();
			fastBufferReader.ReadValueSafe<NetworkObjectReference>(out this.NetworkObjectReference, default(FastBufferWriter.ForNetworkSerializable));
			fastBufferReader.ReadValueSafe<float>(out this.Time, default(FastBufferWriter.ForPrimitives));
			return;
		}
		FastBufferWriter fastBufferWriter = serializer.GetFastBufferWriter();
		fastBufferWriter.WriteValueSafe<NetworkObjectReference>(this.NetworkObjectReference, default(FastBufferWriter.ForNetworkSerializable));
		fastBufferWriter.WriteValueSafe<float>(this.Time, default(FastBufferWriter.ForPrimitives));
	}

	// Token: 0x0600009B RID: 155 RVA: 0x00007207 File Offset: 0x00005407
	public bool Equals(NetworkObjectCollision other)
	{
		return this.NetworkObjectReference.Equals(other.NetworkObjectReference) && this.Time == other.Time;
	}

	// Token: 0x04000042 RID: 66
	public NetworkObjectReference NetworkObjectReference;

	// Token: 0x04000043 RID: 67
	public float Time;
}
