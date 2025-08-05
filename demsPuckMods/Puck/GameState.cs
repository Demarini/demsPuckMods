using System;
using Unity.Netcode;

// Token: 0x0200003A RID: 58
public struct GameState : INetworkSerializable, IEquatable<GameState>
{
	// Token: 0x0600018F RID: 399 RVA: 0x00012FC0 File Offset: 0x000111C0
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		if (serializer.IsReader)
		{
			FastBufferReader fastBufferReader = serializer.GetFastBufferReader();
			fastBufferReader.ReadValueSafe<GamePhase>(out this.Phase, default(FastBufferWriter.ForEnums));
			fastBufferReader.ReadValueSafe<int>(out this.Time, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.Period, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.BlueScore, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.RedScore, default(FastBufferWriter.ForPrimitives));
			return;
		}
		FastBufferWriter fastBufferWriter = serializer.GetFastBufferWriter();
		fastBufferWriter.WriteValueSafe<GamePhase>(this.Phase, default(FastBufferWriter.ForEnums));
		fastBufferWriter.WriteValueSafe<int>(this.Time, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.Period, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.BlueScore, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.RedScore, default(FastBufferWriter.ForPrimitives));
	}

	// Token: 0x06000190 RID: 400 RVA: 0x000130C4 File Offset: 0x000112C4
	public bool Equals(GameState other)
	{
		return this.Phase.Equals(other.Phase) && this.Time == other.Time && this.Period == other.Period && this.BlueScore == other.BlueScore && this.RedScore == other.RedScore;
	}

	// Token: 0x040000DC RID: 220
	public GamePhase Phase;

	// Token: 0x040000DD RID: 221
	public int Time;

	// Token: 0x040000DE RID: 222
	public int Period;

	// Token: 0x040000DF RID: 223
	public int BlueScore;

	// Token: 0x040000E0 RID: 224
	public int RedScore;
}
