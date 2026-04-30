using System;
using Unity.Netcode;

// Token: 0x020000A2 RID: 162
public struct GameState : INetworkSerializable, IEquatable<GameState>
{
	// Token: 0x06000523 RID: 1315 RVA: 0x0001C67C File Offset: 0x0001A87C
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		if (serializer.IsReader)
		{
			FastBufferReader fastBufferReader = serializer.GetFastBufferReader();
			fastBufferReader.ReadValueSafe<GamePhase>(out this.Phase, default(FastBufferWriter.ForEnums));
			fastBufferReader.ReadValueSafe<int>(out this.Tick, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.Period, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.BlueScore, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.RedScore, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<bool>(out this.IsOvertime, default(FastBufferWriter.ForPrimitives));
			return;
		}
		FastBufferWriter fastBufferWriter = serializer.GetFastBufferWriter();
		fastBufferWriter.WriteValueSafe<GamePhase>(this.Phase, default(FastBufferWriter.ForEnums));
		fastBufferWriter.WriteValueSafe<int>(this.Tick, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.Period, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.BlueScore, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.RedScore, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<bool>(this.IsOvertime, default(FastBufferWriter.ForPrimitives));
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x0001C7B0 File Offset: 0x0001A9B0
	public bool Equals(GameState other)
	{
		return this.Phase.Equals(other.Phase) && this.Tick == other.Tick && this.Period == other.Period && this.BlueScore == other.BlueScore && this.RedScore == other.RedScore && this.IsOvertime == other.IsOvertime;
	}

	// Token: 0x0400032D RID: 813
	public GamePhase Phase;

	// Token: 0x0400032E RID: 814
	public int Tick;

	// Token: 0x0400032F RID: 815
	public int Period;

	// Token: 0x04000330 RID: 816
	public int BlueScore;

	// Token: 0x04000331 RID: 817
	public int RedScore;

	// Token: 0x04000332 RID: 818
	public bool IsOvertime;
}
