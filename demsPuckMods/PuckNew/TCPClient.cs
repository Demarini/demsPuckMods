using System;
using System.Text;
using System.Threading.Tasks;
using SuperSimpleTcp;
using UnityEngine;

// Token: 0x020001FC RID: 508
public class TCPClient
{
	// Token: 0x14000008 RID: 8
	// (add) Token: 0x06000E85 RID: 3717 RVA: 0x00043A24 File Offset: 0x00041C24
	// (remove) Token: 0x06000E86 RID: 3718 RVA: 0x00043A5C File Offset: 0x00041C5C
	public event Action OnConnected;

	// Token: 0x14000009 RID: 9
	// (add) Token: 0x06000E87 RID: 3719 RVA: 0x00043A94 File Offset: 0x00041C94
	// (remove) Token: 0x06000E88 RID: 3720 RVA: 0x00043ACC File Offset: 0x00041CCC
	public event Action OnConnectionFailed;

	// Token: 0x1400000A RID: 10
	// (add) Token: 0x06000E89 RID: 3721 RVA: 0x00043B04 File Offset: 0x00041D04
	// (remove) Token: 0x06000E8A RID: 3722 RVA: 0x00043B3C File Offset: 0x00041D3C
	public event Action OnDisconnected;

	// Token: 0x1400000B RID: 11
	// (add) Token: 0x06000E8B RID: 3723 RVA: 0x00043B74 File Offset: 0x00041D74
	// (remove) Token: 0x06000E8C RID: 3724 RVA: 0x00043BAC File Offset: 0x00041DAC
	public event Action<string> OnMessageReceived;

	// Token: 0x1400000C RID: 12
	// (add) Token: 0x06000E8D RID: 3725 RVA: 0x00043BE4 File Offset: 0x00041DE4
	// (remove) Token: 0x06000E8E RID: 3726 RVA: 0x00043C1C File Offset: 0x00041E1C
	public event Action<string> OnMessageSent;

	// Token: 0x1700012E RID: 302
	// (get) Token: 0x06000E8F RID: 3727 RVA: 0x00043C51 File Offset: 0x00041E51
	// (set) Token: 0x06000E90 RID: 3728 RVA: 0x00043C59 File Offset: 0x00041E59
	public bool IsConnecting { get; private set; }

	// Token: 0x1700012F RID: 303
	// (get) Token: 0x06000E91 RID: 3729 RVA: 0x00043C62 File Offset: 0x00041E62
	public bool IsConnected
	{
		get
		{
			return this.Client.IsConnected;
		}
	}

	// Token: 0x06000E92 RID: 3730 RVA: 0x00043C70 File Offset: 0x00041E70
	public TCPClient(EndPoint endPoint, int connectTimeoutMs = 1000, int readTimeoutMs = 1000)
	{
		this.EndPoint = endPoint;
		this.Client = new SimpleTcpClient(this.EndPoint.ipAddress, (int)this.EndPoint.port);
		this.Client.Settings.NoDelay = true;
		this.Client.Settings.UseAsyncDataReceivedEvents = false;
		this.Client.Settings.ConnectTimeoutMs = connectTimeoutMs;
		this.Client.Settings.ReadTimeoutMs = readTimeoutMs;
		this.Client.Events.Connected += delegate(object sender, ConnectionEventArgs args)
		{
			Action onConnected = this.OnConnected;
			if (onConnected == null)
			{
				return;
			}
			onConnected();
		};
		this.Client.Events.Disconnected += delegate(object sender, ConnectionEventArgs args)
		{
			Action onDisconnected = this.OnDisconnected;
			if (onDisconnected == null)
			{
				return;
			}
			onDisconnected();
		};
		this.Client.Events.DataReceived += delegate(object sender, DataReceivedEventArgs args)
		{
			this.OnDataReceived(sender, args);
		};
	}

	// Token: 0x06000E93 RID: 3731 RVA: 0x00043D44 File Offset: 0x00041F44
	public void Connect()
	{
		try
		{
			if (!this.IsConnecting)
			{
				this.IsConnecting = true;
				this.Client.Connect();
				this.IsConnecting = false;
			}
		}
		catch (TimeoutException)
		{
			this.IsConnecting = false;
			Debug.LogError(string.Format("[TCPClient] Connection to server {0} timed out", this.EndPoint));
			Action onConnectionFailed = this.OnConnectionFailed;
			if (onConnectionFailed != null)
			{
				onConnectionFailed();
			}
		}
		catch (Exception ex)
		{
			this.IsConnecting = false;
			Debug.LogError(string.Format("[TCPClient] Connection to server {0} failed: {1}", this.EndPoint, ex.Message));
			Action onConnectionFailed2 = this.OnConnectionFailed;
			if (onConnectionFailed2 != null)
			{
				onConnectionFailed2();
			}
		}
	}

	// Token: 0x06000E94 RID: 3732 RVA: 0x00043DFC File Offset: 0x00041FFC
	public void ConnectAsync()
	{
		Task.Run(delegate()
		{
			this.Connect();
		});
	}

	// Token: 0x06000E95 RID: 3733 RVA: 0x00043E10 File Offset: 0x00042010
	public void Disconnect()
	{
		try
		{
			this.Client.Disconnect();
		}
		catch (Exception ex)
		{
			Debug.LogError("[TCPClient] Error disconnecting: " + ex.Message);
		}
	}

	// Token: 0x06000E96 RID: 3734 RVA: 0x00043E54 File Offset: 0x00042054
	public void DisconnectAsync()
	{
		Task.Run(delegate()
		{
			this.Disconnect();
		});
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x00043E68 File Offset: 0x00042068
	public void SendMessage(string message)
	{
		try
		{
			byte[] bytes = Encoding.UTF8.GetBytes(message);
			this.Client.Send(bytes);
			Action<string> onMessageSent = this.OnMessageSent;
			if (onMessageSent != null)
			{
				onMessageSent(message);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("[TCPClient] Error sending message to server {0}: {1}", this.EndPoint, ex.Message));
		}
	}

	// Token: 0x06000E98 RID: 3736 RVA: 0x00043ED0 File Offset: 0x000420D0
	public void SendMessageAsync(string message)
	{
		Task.Run(delegate()
		{
			this.SendMessage(message);
		});
	}

	// Token: 0x06000E99 RID: 3737 RVA: 0x00043EF8 File Offset: 0x000420F8
	private void OnDataReceived(object sender, DataReceivedEventArgs args)
	{
		try
		{
			string @string = Encoding.UTF8.GetString(args.Data);
			Action<string> onMessageReceived = this.OnMessageReceived;
			if (onMessageReceived != null)
			{
				onMessageReceived(@string);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("[TCPClient] Error deserializing message from server {0}: {1}", this.EndPoint, ex.Message));
		}
	}

	// Token: 0x040008D3 RID: 2259
	public SimpleTcpClient Client;

	// Token: 0x040008D4 RID: 2260
	public EndPoint EndPoint;
}
