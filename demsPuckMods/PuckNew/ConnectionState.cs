using System;

// Token: 0x020000A9 RID: 169
public struct ConnectionState
{
	// Token: 0x0600055A RID: 1370 RVA: 0x0001D498 File Offset: 0x0001B698
	public bool Equals(ConnectionState other)
	{
		return this.IsConnecting == other.IsConnecting && this.IsConnected == other.IsConnected && this.Connection == other.Connection && this.LastConnection == other.LastConnection && this.PendingConnection == other.PendingConnection;
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x0001D4F0 File Offset: 0x0001B6F0
	public override bool Equals(object obj)
	{
		if (obj is ConnectionState)
		{
			ConnectionState other = (ConnectionState)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x0001D515 File Offset: 0x0001B715
	public override int GetHashCode()
	{
		return HashCode.Combine<bool, bool, Connection, Connection, Connection>(this.IsConnecting, this.IsConnected, this.Connection, this.LastConnection, this.PendingConnection);
	}

	// Token: 0x0600055D RID: 1373 RVA: 0x0001D53C File Offset: 0x0001B73C
	public override string ToString()
	{
		return string.Format("IsConnecting: {0}, IsConnected: {1}, Connection: {2}, LastConnection: {3}, PendingConnection: {4}", new object[]
		{
			this.IsConnecting,
			this.IsConnected,
			this.Connection,
			this.LastConnection,
			this.PendingConnection
		});
	}

	// Token: 0x04000344 RID: 836
	public bool IsConnecting;

	// Token: 0x04000345 RID: 837
	public bool IsConnected;

	// Token: 0x04000346 RID: 838
	public Connection Connection;

	// Token: 0x04000347 RID: 839
	public Connection LastConnection;

	// Token: 0x04000348 RID: 840
	public Connection PendingConnection;
}
