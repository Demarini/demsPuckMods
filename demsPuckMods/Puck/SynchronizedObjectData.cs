using System;
using Unity.Netcode;

// Token: 0x020000B6 RID: 182
public struct SynchronizedObjectData : INetworkSerializable, IEquatable<SynchronizedObjectData>
{
	// Token: 0x0600056B RID: 1387 RVA: 0x00022240 File Offset: 0x00020440
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		if (serializer.IsReader)
		{
			FastBufferReader fastBufferReader = serializer.GetFastBufferReader();
			fastBufferReader.ReadValueSafe<ushort>(out this.NetworkObjectId, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<short>(out this.X, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<short>(out this.Y, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<short>(out this.Z, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<short>(out this.Rx, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<short>(out this.Ry, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<short>(out this.Rz, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<short>(out this.Rw, default(FastBufferWriter.ForPrimitives));
			return;
		}
		FastBufferWriter fastBufferWriter = serializer.GetFastBufferWriter();
		fastBufferWriter.WriteValueSafe<ushort>(this.NetworkObjectId, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<short>(this.X, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<short>(this.Y, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<short>(this.Z, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<short>(this.Rx, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<short>(this.Ry, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<short>(this.Rz, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<short>(this.Rw, default(FastBufferWriter.ForPrimitives));
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x000223CC File Offset: 0x000205CC
	public bool Equals(SynchronizedObjectData other)
	{
		return this.NetworkObjectId == other.NetworkObjectId && this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.Rx == other.Rx && this.Ry == other.Ry && this.Rz == other.Rz && this.Rw == other.Rw;
	}

	// Token: 0x040002EC RID: 748
	public ushort NetworkObjectId;

	// Token: 0x040002ED RID: 749
	public short X;

	// Token: 0x040002EE RID: 750
	public short Y;

	// Token: 0x040002EF RID: 751
	public short Z;

	// Token: 0x040002F0 RID: 752
	public short Rx;

	// Token: 0x040002F1 RID: 753
	public short Ry;

	// Token: 0x040002F2 RID: 754
	public short Rz;

	// Token: 0x040002F3 RID: 755
	public short Rw;
}
