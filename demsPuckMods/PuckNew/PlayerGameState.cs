using System;
using Unity.Netcode;

// Token: 0x02000044 RID: 68
public struct PlayerGameState : INetworkSerializable, IEquatable<PlayerGameState>
{
	// Token: 0x0600020E RID: 526 RVA: 0x0000D8F4 File Offset: 0x0000BAF4
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		if (serializer.IsReader)
		{
			FastBufferReader fastBufferReader = serializer.GetFastBufferReader();
			fastBufferReader.ReadValueSafe<PlayerPhase>(out this.Phase, default(FastBufferWriter.ForEnums));
			fastBufferReader.ReadValueSafe<PlayerTeam>(out this.Team, default(FastBufferWriter.ForEnums));
			fastBufferReader.ReadValueSafe<PlayerRole>(out this.Role, default(FastBufferWriter.ForEnums));
			return;
		}
		FastBufferWriter fastBufferWriter = serializer.GetFastBufferWriter();
		fastBufferWriter.WriteValueSafe<PlayerPhase>(this.Phase, default(FastBufferWriter.ForEnums));
		fastBufferWriter.WriteValueSafe<PlayerTeam>(this.Team, default(FastBufferWriter.ForEnums));
		fastBufferWriter.WriteValueSafe<PlayerRole>(this.Role, default(FastBufferWriter.ForEnums));
	}

	// Token: 0x0600020F RID: 527 RVA: 0x0000D99F File Offset: 0x0000BB9F
	public bool Equals(PlayerGameState other)
	{
		return this.Phase == other.Phase && this.Team == other.Team && this.Role == other.Role;
	}

	// Token: 0x06000210 RID: 528 RVA: 0x0000D9CD File Offset: 0x0000BBCD
	public override string ToString()
	{
		return string.Format("Phase: {0}, Team: {1}, Role: {2}", this.Phase, this.Team, this.Role);
	}

	// Token: 0x04000162 RID: 354
	public PlayerPhase Phase;

	// Token: 0x04000163 RID: 355
	public PlayerTeam Team;

	// Token: 0x04000164 RID: 356
	public PlayerRole Role;
}
