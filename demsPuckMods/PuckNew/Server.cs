using System;
using Unity.Collections;
using Unity.Netcode;

// Token: 0x0200011F RID: 287
public struct Server : INetworkSerializable, IEquatable<Server>
{
	// Token: 0x060007F1 RID: 2033 RVA: 0x00026248 File Offset: 0x00024448
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		if (serializer.IsReader)
		{
			FastBufferReader fastBufferReader = serializer.GetFastBufferReader();
			fastBufferReader.ReadValueSafe<FixedString32Bytes>(out this.IpAddress, default(FastBufferWriter.ForFixedStrings));
			fastBufferReader.ReadValueSafe<ushort>(out this.Port, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<FixedString128Bytes>(out this.Name, default(FastBufferWriter.ForFixedStrings));
			fastBufferReader.ReadValueSafe<int>(out this.MaxPlayers, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.TickRate, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<bool>(out this.UseVoip, default(FastBufferWriter.ForPrimitives));
			return;
		}
		FastBufferWriter fastBufferWriter = serializer.GetFastBufferWriter();
		fastBufferWriter.WriteValueSafe<FixedString32Bytes>(this.IpAddress, default(FastBufferWriter.ForFixedStrings));
		fastBufferWriter.WriteValueSafe<ushort>(this.Port, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<FixedString128Bytes>(this.Name, default(FastBufferWriter.ForFixedStrings));
		fastBufferWriter.WriteValueSafe<int>(this.MaxPlayers, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.TickRate, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<bool>(this.UseVoip, default(FastBufferWriter.ForPrimitives));
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x0002637C File Offset: 0x0002457C
	public bool Equals(Server other)
	{
		return this.IpAddress == other.IpAddress && this.Port == other.Port && this.Name == other.Name && this.MaxPlayers == other.MaxPlayers && this.TickRate == other.TickRate && this.UseVoip == other.UseVoip;
	}

	// Token: 0x040004BA RID: 1210
	public FixedString32Bytes IpAddress;

	// Token: 0x040004BB RID: 1211
	public ushort Port;

	// Token: 0x040004BC RID: 1212
	public FixedString128Bytes Name;

	// Token: 0x040004BD RID: 1213
	public int MaxPlayers;

	// Token: 0x040004BE RID: 1214
	public int TickRate;

	// Token: 0x040004BF RID: 1215
	public bool UseVoip;
}
