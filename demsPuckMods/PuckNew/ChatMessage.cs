using System;
using Unity.Collections;
using Unity.Netcode;

// Token: 0x0200009A RID: 154
public class ChatMessage : INetworkSerializable, IEquatable<ChatMessage>
{
	// Token: 0x060004F1 RID: 1265 RVA: 0x0001AE94 File Offset: 0x00019094
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		bool flag = this.SteamID != null;
		serializer.SerializeValue<bool>(ref flag, default(FastBufferWriter.ForPrimitives));
		if (flag)
		{
			FixedString32Bytes valueOrDefault = this.SteamID.GetValueOrDefault();
			serializer.SerializeValue<FixedString32Bytes>(ref valueOrDefault, default(FastBufferWriter.ForFixedStrings));
			this.SteamID = new FixedString32Bytes?(valueOrDefault);
		}
		bool flag2 = this.Username != null;
		serializer.SerializeValue<bool>(ref flag2, default(FastBufferWriter.ForPrimitives));
		if (flag2)
		{
			FixedString32Bytes valueOrDefault2 = this.Username.GetValueOrDefault();
			serializer.SerializeValue<FixedString32Bytes>(ref valueOrDefault2, default(FastBufferWriter.ForFixedStrings));
			this.Username = new FixedString32Bytes?(valueOrDefault2);
		}
		bool flag3 = this.Team != null;
		serializer.SerializeValue<bool>(ref flag3, default(FastBufferWriter.ForPrimitives));
		if (flag3)
		{
			PlayerTeam valueOrDefault3 = this.Team.GetValueOrDefault();
			serializer.SerializeValue<PlayerTeam>(ref valueOrDefault3, default(FastBufferWriter.ForEnums));
			this.Team = new PlayerTeam?(valueOrDefault3);
		}
		serializer.SerializeValue<FixedString512Bytes>(ref this.Content, default(FastBufferWriter.ForFixedStrings));
		serializer.SerializeValue<double>(ref this.Timestamp, default(FastBufferWriter.ForPrimitives));
		serializer.SerializeValue<bool>(ref this.IsQuickChat, default(FastBufferWriter.ForPrimitives));
		serializer.SerializeValue<bool>(ref this.IsTeamChat, default(FastBufferWriter.ForPrimitives));
		serializer.SerializeValue<bool>(ref this.IsSystem, default(FastBufferWriter.ForPrimitives));
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x0001AFFC File Offset: 0x000191FC
	public bool Equals(ChatMessage other)
	{
		FixedString32Bytes? fixedString32Bytes = this.SteamID;
		FixedString32Bytes? fixedString32Bytes2 = other.SteamID;
		bool flag;
		if (fixedString32Bytes != null != (fixedString32Bytes2 != null))
		{
			flag = false;
		}
		else if (fixedString32Bytes == null)
		{
			flag = true;
		}
		else
		{
			FixedString32Bytes valueOrDefault = fixedString32Bytes.GetValueOrDefault();
			FixedString32Bytes valueOrDefault2 = fixedString32Bytes2.GetValueOrDefault();
			flag = (valueOrDefault == valueOrDefault2);
		}
		if (flag)
		{
			fixedString32Bytes2 = this.Username;
			fixedString32Bytes = other.Username;
			bool flag2;
			if (fixedString32Bytes2 != null != (fixedString32Bytes != null))
			{
				flag2 = false;
			}
			else if (fixedString32Bytes2 == null)
			{
				flag2 = true;
			}
			else
			{
				FixedString32Bytes valueOrDefault3 = fixedString32Bytes2.GetValueOrDefault();
				FixedString32Bytes valueOrDefault4 = fixedString32Bytes.GetValueOrDefault();
				flag2 = (valueOrDefault3 == valueOrDefault4);
			}
			if (flag2 && this.Content == other.Content && this.Timestamp == other.Timestamp && this.IsQuickChat == other.IsQuickChat && this.IsTeamChat == other.IsTeamChat)
			{
				return this.IsSystem == other.IsSystem;
			}
		}
		return false;
	}

	// Token: 0x04000305 RID: 773
	public FixedString32Bytes? SteamID;

	// Token: 0x04000306 RID: 774
	public FixedString32Bytes? Username;

	// Token: 0x04000307 RID: 775
	public PlayerTeam? Team;

	// Token: 0x04000308 RID: 776
	public FixedString512Bytes Content;

	// Token: 0x04000309 RID: 777
	public double Timestamp;

	// Token: 0x0400030A RID: 778
	public bool IsQuickChat;

	// Token: 0x0400030B RID: 779
	public bool IsTeamChat;

	// Token: 0x0400030C RID: 780
	public bool IsSystem;
}
