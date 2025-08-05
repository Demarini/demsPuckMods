using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x020000AA RID: 170
public class StateManager : MonoBehaviourSingleton<StateManager>
{
	// Token: 0x17000079 RID: 121
	// (get) Token: 0x060004ED RID: 1261 RVA: 0x0000A128 File Offset: 0x00008328
	// (set) Token: 0x060004EE RID: 1262 RVA: 0x000207B4 File Offset: 0x0001E9B4
	public PlayerData PlayerData
	{
		get
		{
			if (this.playerData != null)
			{
				return this.playerData;
			}
			return new PlayerData();
		}
		set
		{
			if (this.playerData == value)
			{
				return;
			}
			PlayerData oldPlayerData = this.playerData;
			this.playerData = value;
			this.OnPlayerDataChanged(oldPlayerData);
		}
	}

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x060004EF RID: 1263 RVA: 0x000207E0 File Offset: 0x0001E9E0
	public double CurrentUnixTimestamp
	{
		get
		{
			return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
		}
	}

	// Token: 0x1700007B RID: 123
	// (get) Token: 0x060004F0 RID: 1264 RVA: 0x0000A13E File Offset: 0x0000833E
	public bool IsBanned
	{
		get
		{
			return this.Ban(this.playerData) != null;
		}
	}

	// Token: 0x1700007C RID: 124
	// (get) Token: 0x060004F1 RID: 1265 RVA: 0x0000A14F File Offset: 0x0000834F
	public bool IsMuted
	{
		get
		{
			return this.Mute(this.playerData) != null;
		}
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x0000A160 File Offset: 0x00008360
	public PlayerBan Ban(PlayerData playerData)
	{
		return playerData.bans.FirstOrDefault((PlayerBan ban) => this.CurrentUnixTimestamp <= ban.until);
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x0000A179 File Offset: 0x00008379
	public PlayerMute Mute(PlayerData playerData)
	{
		return playerData.mutes.FirstOrDefault((PlayerMute mute) => this.CurrentUnixTimestamp <= mute.until);
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x0000A192 File Offset: 0x00008392
	public PlayerBan IsPlayerDataBanned(PlayerData playerData)
	{
		return playerData.bans.FirstOrDefault((PlayerBan ban) => this.CurrentUnixTimestamp <= ban.until);
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x0000A1AB File Offset: 0x000083AB
	public PlayerMute IsPlayerDataMuted(PlayerData playerData)
	{
		return playerData.mutes.FirstOrDefault((PlayerMute mute) => this.CurrentUnixTimestamp <= mute.until);
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x00020810 File Offset: 0x0001EA10
	private void OnPlayerDataChanged(PlayerData oldPlayerData)
	{
		if (this.playerData == null)
		{
			return;
		}
		if (oldPlayerData == null)
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerDataReady", new Dictionary<string, object>
			{
				{
					"oldPlayerData",
					oldPlayerData
				},
				{
					"newPlayerData",
					this.PlayerData
				}
			});
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerDataChanged", new Dictionary<string, object>
		{
			{
				"oldPlayerData",
				oldPlayerData
			},
			{
				"newPlayerData",
				this.PlayerData
			}
		});
		PlayerBan playerBan = this.Ban(this.playerData);
		PlayerMute playerMute = this.Mute(this.playerData);
		bool flag = oldPlayerData != null && this.Ban(oldPlayerData) != null;
		bool flag2 = oldPlayerData != null && this.Mute(oldPlayerData) != null;
		if (this.IsBanned && !flag)
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerBanned", new Dictionary<string, object>
			{
				{
					"reason",
					playerBan.reason
				},
				{
					"until",
					playerBan.until
				}
			});
		}
		else if (!this.IsBanned && flag)
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerUnbanned", null);
		}
		if (this.IsMuted && !flag2)
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerMuted", new Dictionary<string, object>
			{
				{
					"reason",
					playerMute.reason
				},
				{
					"until",
					playerMute.until
				}
			});
			return;
		}
		if (!this.IsMuted && flag2)
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerUnmuted", null);
		}
	}

	// Token: 0x040002C9 RID: 713
	private PlayerData playerData;
}
