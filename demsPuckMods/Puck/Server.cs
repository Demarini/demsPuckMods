using System;
using Unity.Collections;
using Unity.Netcode;

// Token: 0x0200009C RID: 156
public struct Server : INetworkSerializable, IEquatable<Server>
{
	// Token: 0x0600040F RID: 1039 RVA: 0x0001B480 File Offset: 0x00019680
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		if (serializer.IsReader)
		{
			FastBufferReader fastBufferReader = serializer.GetFastBufferReader();
			fastBufferReader.ReadValueSafe<FixedString32Bytes>(out this.IpAddress, default(FastBufferWriter.ForFixedStrings));
			fastBufferReader.ReadValueSafe<ushort>(out this.Port, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<ushort>(out this.PingPort, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<FixedString128Bytes>(out this.Name, default(FastBufferWriter.ForFixedStrings));
			fastBufferReader.ReadValueSafe<int>(out this.MaxPlayers, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<FixedString32Bytes>(out this.Password, default(FastBufferWriter.ForFixedStrings));
			fastBufferReader.ReadValueSafe<bool>(out this.Voip, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<bool>(out this.IsPublic, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<bool>(out this.IsDedicated, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<bool>(out this.IsHosted, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<bool>(out this.IsAuthenticated, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<FixedString32Bytes>(out this.OwnerSteamId, default(FastBufferWriter.ForFixedStrings));
			fastBufferReader.ReadValueSafe<float>(out this.SleepTimeout, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.ClientTickRate, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<ulong>(out this.ClientRequiredModIds, default(FastBufferWriter.ForPrimitives));
			return;
		}
		FastBufferWriter fastBufferWriter = serializer.GetFastBufferWriter();
		fastBufferWriter.WriteValueSafe<FixedString32Bytes>(this.IpAddress, default(FastBufferWriter.ForFixedStrings));
		fastBufferWriter.WriteValueSafe<ushort>(this.Port, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<ushort>(this.PingPort, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<FixedString128Bytes>(this.Name, default(FastBufferWriter.ForFixedStrings));
		fastBufferWriter.WriteValueSafe<int>(this.MaxPlayers, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<FixedString32Bytes>(this.Password, default(FastBufferWriter.ForFixedStrings));
		fastBufferWriter.WriteValueSafe<bool>(this.Voip, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<bool>(this.IsPublic, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<bool>(this.IsDedicated, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<bool>(this.IsHosted, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<bool>(this.IsAuthenticated, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<FixedString32Bytes>(this.OwnerSteamId, default(FastBufferWriter.ForFixedStrings));
		fastBufferWriter.WriteValueSafe<float>(this.SleepTimeout, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.ClientTickRate, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<ulong>(this.ClientRequiredModIds, default(FastBufferWriter.ForPrimitives));
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x0001B740 File Offset: 0x00019940
	public bool Equals(Server other)
	{
		return this.IpAddress == other.IpAddress && this.Port == other.Port && this.PingPort == other.PingPort && this.Name == other.Name && this.MaxPlayers == other.MaxPlayers && this.Password == other.Password && this.Voip == other.Voip && this.IsPublic == other.IsPublic && this.IsDedicated == other.IsDedicated && this.IsHosted == other.IsHosted && this.IsAuthenticated == other.IsAuthenticated && this.OwnerSteamId == other.OwnerSteamId && this.SleepTimeout == other.SleepTimeout && this.ClientTickRate == other.ClientTickRate && this.ClientRequiredModIds == other.ClientRequiredModIds;
	}

	// Token: 0x0400024D RID: 589
	public FixedString32Bytes IpAddress;

	// Token: 0x0400024E RID: 590
	public ushort Port;

	// Token: 0x0400024F RID: 591
	public ushort PingPort;

	// Token: 0x04000250 RID: 592
	public FixedString128Bytes Name;

	// Token: 0x04000251 RID: 593
	public int MaxPlayers;

	// Token: 0x04000252 RID: 594
	public FixedString32Bytes Password;

	// Token: 0x04000253 RID: 595
	public bool Voip;

	// Token: 0x04000254 RID: 596
	public bool IsPublic;

	// Token: 0x04000255 RID: 597
	public bool IsDedicated;

	// Token: 0x04000256 RID: 598
	public bool IsHosted;

	// Token: 0x04000257 RID: 599
	public bool IsAuthenticated;

	// Token: 0x04000258 RID: 600
	public FixedString32Bytes OwnerSteamId;

	// Token: 0x04000259 RID: 601
	public float SleepTimeout;

	// Token: 0x0400025A RID: 602
	public int ClientTickRate;

	// Token: 0x0400025B RID: 603
	public ulong[] ClientRequiredModIds;
}
