using System;
using Unity.Netcode;

// Token: 0x0200001A RID: 26
public struct NetworkObjectCollision : INetworkSerializable, IEquatable<NetworkObjectCollision>
{
	// Token: 0x06000092 RID: 146 RVA: 0x00003AD8 File Offset: 0x00001CD8
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

	// Token: 0x06000093 RID: 147 RVA: 0x00003B57 File Offset: 0x00001D57
	public bool Equals(NetworkObjectCollision other)
	{
		return this.NetworkObjectReference.Equals(other.NetworkObjectReference) && this.Time == other.Time;
	}

	// Token: 0x04000041 RID: 65
	public NetworkObjectReference NetworkObjectReference;

	// Token: 0x04000042 RID: 66
	public float Time;
}
