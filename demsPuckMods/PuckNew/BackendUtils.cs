using System;
using System.Linq;

// Token: 0x020001D7 RID: 471
public static class BackendUtils
{
	// Token: 0x06000DE5 RID: 3557 RVA: 0x00041D95 File Offset: 0x0003FF95
	public static PlayerBan GetActivePlayerDataBan(PlayerData playerData)
	{
		if (playerData == null)
		{
			return null;
		}
		return playerData.bans.FirstOrDefault((PlayerBan ban) => Utils.GetTimestamp() <= ban.expiresAt);
	}

	// Token: 0x06000DE6 RID: 3558 RVA: 0x00041DC6 File Offset: 0x0003FFC6
	public static PlayerMute GetActivePlayerDataMute(PlayerData playerData)
	{
		if (playerData == null)
		{
			return null;
		}
		return playerData.mutes.FirstOrDefault((PlayerMute mute) => Utils.GetTimestamp() <= mute.expiresAt);
	}

	// Token: 0x06000DE7 RID: 3559 RVA: 0x00041DF7 File Offset: 0x0003FFF7
	public static PlayerCooldown GetActivePlayerDataCooldown(PlayerData playerData)
	{
		if (playerData == null)
		{
			return null;
		}
		return playerData.cooldowns.FirstOrDefault((PlayerCooldown cooldown) => Utils.GetTimestamp() <= cooldown.expiresAt);
	}

	// Token: 0x06000DE8 RID: 3560 RVA: 0x00041E28 File Offset: 0x00040028
	public static bool IsConnectedToMatchEndPoint()
	{
		Connection connection = GlobalStateManager.ConnectionState.Connection;
		EndPoint a = (connection != null) ? connection.EndPoint : null;
		PlayerMatchData matchData = BackendManager.PlayerState.MatchData;
		EndPoint b = (matchData != null) ? matchData.endPoint : null;
		return a != null && a == b;
	}
}
