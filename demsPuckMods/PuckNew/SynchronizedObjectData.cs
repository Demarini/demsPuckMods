using System;
using Unity.Netcode;

// Token: 0x02000139 RID: 313
public struct SynchronizedObjectData : INetworkSerializable, IEquatable<SynchronizedObjectData>
{
	// Token: 0x0600094A RID: 2378 RVA: 0x0002CC30 File Offset: 0x0002AE30
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

	// Token: 0x0600094B RID: 2379 RVA: 0x0002CDBC File Offset: 0x0002AFBC
	public bool Equals(SynchronizedObjectData other)
	{
		return this.NetworkObjectId == other.NetworkObjectId && this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.Rx == other.Rx && this.Ry == other.Ry && this.Rz == other.Rz && this.Rw == other.Rw;
	}

	// Token: 0x04000553 RID: 1363
	public ushort NetworkObjectId;

	// Token: 0x04000554 RID: 1364
	public short X;

	// Token: 0x04000555 RID: 1365
	public short Y;

	// Token: 0x04000556 RID: 1366
	public short Z;

	// Token: 0x04000557 RID: 1367
	public short Rx;

	// Token: 0x04000558 RID: 1368
	public short Ry;

	// Token: 0x04000559 RID: 1369
	public short Rz;

	// Token: 0x0400055A RID: 1370
	public short Rw;
}
