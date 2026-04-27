using System;
using Unity.Netcode;

// Token: 0x02000045 RID: 69
public struct PlayerCustomizationState : INetworkSerializable, IEquatable<PlayerCustomizationState>
{
	// Token: 0x06000211 RID: 529 RVA: 0x0000D9FC File Offset: 0x0000BBFC
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		if (serializer.IsReader)
		{
			FastBufferReader fastBufferReader = serializer.GetFastBufferReader();
			fastBufferReader.ReadValueSafe<int>(out this.FlagID, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.HeadgearIDBlueAttacker, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.HeadgearIDRedAttacker, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.HeadgearIDBlueGoalie, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.HeadgearIDRedGoalie, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.MustacheID, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.BeardID, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.JerseyIDBlueAttacker, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.JerseyIDRedAttacker, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.JerseyIDBlueGoalie, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.JerseyIDRedGoalie, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.StickSkinIDBlueAttacker, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.StickSkinIDRedAttacker, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.StickSkinIDBlueGoalie, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.StickSkinIDRedGoalie, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.StickShaftTapeIDBlueAttacker, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.StickShaftTapeIDRedAttacker, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.StickShaftTapeIDBlueGoalie, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.StickShaftTapeIDRedGoalie, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.StickBladeTapeIDBlueAttacker, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.StickBladeTapeIDRedAttacker, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.StickBladeTapeIDBlueGoalie, default(FastBufferWriter.ForPrimitives));
			fastBufferReader.ReadValueSafe<int>(out this.StickBladeTapeIDRedGoalie, default(FastBufferWriter.ForPrimitives));
			return;
		}
		FastBufferWriter fastBufferWriter = serializer.GetFastBufferWriter();
		fastBufferWriter.WriteValueSafe<int>(this.FlagID, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.HeadgearIDBlueAttacker, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.HeadgearIDRedAttacker, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.HeadgearIDBlueGoalie, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.HeadgearIDRedGoalie, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.MustacheID, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.BeardID, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.JerseyIDBlueAttacker, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.JerseyIDRedAttacker, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.JerseyIDBlueGoalie, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.JerseyIDRedGoalie, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.StickSkinIDBlueAttacker, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.StickSkinIDRedAttacker, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.StickSkinIDBlueGoalie, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.StickSkinIDRedGoalie, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.StickShaftTapeIDBlueAttacker, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.StickShaftTapeIDRedAttacker, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.StickShaftTapeIDBlueGoalie, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.StickShaftTapeIDRedGoalie, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.StickBladeTapeIDBlueAttacker, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.StickBladeTapeIDRedAttacker, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.StickBladeTapeIDBlueGoalie, default(FastBufferWriter.ForPrimitives));
		fastBufferWriter.WriteValueSafe<int>(this.StickBladeTapeIDRedGoalie, default(FastBufferWriter.ForPrimitives));
	}

	// Token: 0x06000212 RID: 530 RVA: 0x0000DE1C File Offset: 0x0000C01C
	public bool Equals(PlayerCustomizationState other)
	{
		return this.FlagID == other.FlagID && this.HeadgearIDBlueAttacker == other.HeadgearIDBlueAttacker && this.HeadgearIDRedAttacker == other.HeadgearIDRedAttacker && this.HeadgearIDBlueGoalie == other.HeadgearIDBlueGoalie && this.HeadgearIDRedGoalie == other.HeadgearIDRedGoalie && this.MustacheID == other.MustacheID && this.BeardID == other.BeardID && this.JerseyIDBlueAttacker == other.JerseyIDBlueAttacker && this.JerseyIDRedAttacker == other.JerseyIDRedAttacker && this.JerseyIDBlueGoalie == other.JerseyIDBlueGoalie && this.JerseyIDRedGoalie == other.JerseyIDRedGoalie && this.StickSkinIDBlueAttacker == other.StickSkinIDBlueAttacker && this.StickSkinIDRedAttacker == other.StickSkinIDRedAttacker && this.StickSkinIDBlueGoalie == other.StickSkinIDBlueGoalie && this.StickSkinIDRedGoalie == other.StickSkinIDRedGoalie && this.StickShaftTapeIDBlueAttacker == other.StickShaftTapeIDBlueAttacker && this.StickShaftTapeIDRedAttacker == other.StickShaftTapeIDRedAttacker && this.StickShaftTapeIDBlueGoalie == other.StickShaftTapeIDBlueGoalie && this.StickShaftTapeIDRedGoalie == other.StickShaftTapeIDRedGoalie && this.StickBladeTapeIDBlueAttacker == other.StickBladeTapeIDBlueAttacker && this.StickBladeTapeIDRedAttacker == other.StickBladeTapeIDRedAttacker && this.StickBladeTapeIDBlueGoalie == other.StickBladeTapeIDBlueGoalie && this.StickBladeTapeIDRedGoalie == other.StickBladeTapeIDRedGoalie;
	}

	// Token: 0x06000213 RID: 531 RVA: 0x0000DF94 File Offset: 0x0000C194
	public override string ToString()
	{
		return string.Format("FlagID: {0}, HeadgearIDs: [{1}, {2}, {3}, {4}], MustacheID: {5}, BeardID: {6}, JerseyIDs: [{7}, {8}, {9}, {10}], StickSkinIDs: [{11}, {12}, {13}, {14}], StickShaftTapeIDs: [{15}, {16}, {17}, {18}], StickBladeTapeIDs: [{19}, {20}, {21}, {22}]", new object[]
		{
			this.FlagID,
			this.HeadgearIDBlueAttacker,
			this.HeadgearIDRedAttacker,
			this.HeadgearIDBlueGoalie,
			this.HeadgearIDRedGoalie,
			this.MustacheID,
			this.BeardID,
			this.JerseyIDBlueAttacker,
			this.JerseyIDRedAttacker,
			this.JerseyIDBlueGoalie,
			this.JerseyIDRedGoalie,
			this.StickSkinIDBlueAttacker,
			this.StickSkinIDRedAttacker,
			this.StickSkinIDBlueGoalie,
			this.StickSkinIDRedGoalie,
			this.StickShaftTapeIDBlueAttacker,
			this.StickShaftTapeIDRedAttacker,
			this.StickShaftTapeIDBlueGoalie,
			this.StickShaftTapeIDRedGoalie,
			this.StickBladeTapeIDBlueAttacker,
			this.StickBladeTapeIDRedAttacker,
			this.StickBladeTapeIDBlueGoalie,
			this.StickBladeTapeIDRedGoalie
		});
	}

	// Token: 0x04000165 RID: 357
	public int FlagID;

	// Token: 0x04000166 RID: 358
	public int HeadgearIDBlueAttacker;

	// Token: 0x04000167 RID: 359
	public int HeadgearIDRedAttacker;

	// Token: 0x04000168 RID: 360
	public int HeadgearIDBlueGoalie;

	// Token: 0x04000169 RID: 361
	public int HeadgearIDRedGoalie;

	// Token: 0x0400016A RID: 362
	public int MustacheID;

	// Token: 0x0400016B RID: 363
	public int BeardID;

	// Token: 0x0400016C RID: 364
	public int JerseyIDBlueAttacker;

	// Token: 0x0400016D RID: 365
	public int JerseyIDRedAttacker;

	// Token: 0x0400016E RID: 366
	public int JerseyIDBlueGoalie;

	// Token: 0x0400016F RID: 367
	public int JerseyIDRedGoalie;

	// Token: 0x04000170 RID: 368
	public int StickSkinIDBlueAttacker;

	// Token: 0x04000171 RID: 369
	public int StickSkinIDRedAttacker;

	// Token: 0x04000172 RID: 370
	public int StickSkinIDBlueGoalie;

	// Token: 0x04000173 RID: 371
	public int StickSkinIDRedGoalie;

	// Token: 0x04000174 RID: 372
	public int StickShaftTapeIDBlueAttacker;

	// Token: 0x04000175 RID: 373
	public int StickShaftTapeIDRedAttacker;

	// Token: 0x04000176 RID: 374
	public int StickShaftTapeIDBlueGoalie;

	// Token: 0x04000177 RID: 375
	public int StickShaftTapeIDRedGoalie;

	// Token: 0x04000178 RID: 376
	public int StickBladeTapeIDBlueAttacker;

	// Token: 0x04000179 RID: 377
	public int StickBladeTapeIDRedAttacker;

	// Token: 0x0400017A RID: 378
	public int StickBladeTapeIDBlueGoalie;

	// Token: 0x0400017B RID: 379
	public int StickBladeTapeIDRedGoalie;
}
