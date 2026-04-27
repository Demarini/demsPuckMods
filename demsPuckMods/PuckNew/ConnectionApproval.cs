using System;
using Unity.Netcode;

// Token: 0x02000113 RID: 275
public class ConnectionApproval
{
	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x0600078D RID: 1933 RVA: 0x00024FC2 File Offset: 0x000231C2
	public ulong ClientID
	{
		get
		{
			return this.Request.ClientNetworkId;
		}
	}

	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x0600078E RID: 1934 RVA: 0x00024FCF File Offset: 0x000231CF
	public bool IsHost
	{
		get
		{
			return this.ClientID == 0UL;
		}
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x00024FDB File Offset: 0x000231DB
	public void Halt()
	{
		this.IsInProgress = true;
		this.Response.Pending = this.IsInProgress;
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x00024FF5 File Offset: 0x000231F5
	public void Approve(PlayerData playerData)
	{
		this.PlayerData = playerData;
		this.IsApproved = true;
		this.IsInProgress = false;
		this.Response.Approved = this.IsApproved;
		this.Response.Pending = this.IsInProgress;
	}

	// Token: 0x06000791 RID: 1937 RVA: 0x0002502E File Offset: 0x0002322E
	public void Reject(string reason)
	{
		this.IsApproved = false;
		this.IsInProgress = false;
		this.Response.Reason = reason;
		this.Response.Approved = this.IsApproved;
		this.Response.Pending = this.IsInProgress;
	}

	// Token: 0x04000487 RID: 1159
	public NetworkManager.ConnectionApprovalRequest Request;

	// Token: 0x04000488 RID: 1160
	public NetworkManager.ConnectionApprovalResponse Response;

	// Token: 0x04000489 RID: 1161
	public ConnectionData ConnectionData;

	// Token: 0x0400048A RID: 1162
	public PlayerData PlayerData;

	// Token: 0x0400048B RID: 1163
	public string IpAddress;

	// Token: 0x0400048C RID: 1164
	public bool IsApproved;

	// Token: 0x0400048D RID: 1165
	public bool IsInProgress;
}
