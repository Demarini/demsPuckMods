using System;
using System.Text;
using System.Threading.Tasks;
using SuperSimpleTcp;
using UnityEngine;

// Token: 0x020001FE RID: 510
public class TCPServer
{
	// Token: 0x1400000D RID: 13
	// (add) Token: 0x06000EA1 RID: 3745 RVA: 0x00043FB4 File Offset: 0x000421B4
	// (remove) Token: 0x06000EA2 RID: 3746 RVA: 0x00043FEC File Offset: 0x000421EC
	public event Action<ushort> OnServerStarted;

	// Token: 0x1400000E RID: 14
	// (add) Token: 0x06000EA3 RID: 3747 RVA: 0x00044024 File Offset: 0x00042224
	// (remove) Token: 0x06000EA4 RID: 3748 RVA: 0x0004405C File Offset: 0x0004225C
	public event Action<Exception> OnServerStartFailed;

	// Token: 0x1400000F RID: 15
	// (add) Token: 0x06000EA5 RID: 3749 RVA: 0x00044094 File Offset: 0x00042294
	// (remove) Token: 0x06000EA6 RID: 3750 RVA: 0x000440CC File Offset: 0x000422CC
	public event Action<ushort> OnServerStopped;

	// Token: 0x14000010 RID: 16
	// (add) Token: 0x06000EA7 RID: 3751 RVA: 0x00044104 File Offset: 0x00042304
	// (remove) Token: 0x06000EA8 RID: 3752 RVA: 0x0004413C File Offset: 0x0004233C
	public event Action<string> OnClientConnected;

	// Token: 0x14000011 RID: 17
	// (add) Token: 0x06000EA9 RID: 3753 RVA: 0x00044174 File Offset: 0x00042374
	// (remove) Token: 0x06000EAA RID: 3754 RVA: 0x000441AC File Offset: 0x000423AC
	public event Action<string> OnClientDisconnected;

	// Token: 0x14000012 RID: 18
	// (add) Token: 0x06000EAB RID: 3755 RVA: 0x000441E4 File Offset: 0x000423E4
	// (remove) Token: 0x06000EAC RID: 3756 RVA: 0x0004421C File Offset: 0x0004241C
	public event Action<string, string> OnMessageReceived;

	// Token: 0x14000013 RID: 19
	// (add) Token: 0x06000EAD RID: 3757 RVA: 0x00044254 File Offset: 0x00042454
	// (remove) Token: 0x06000EAE RID: 3758 RVA: 0x0004428C File Offset: 0x0004248C
	public event Action<string, string> OnMessageSent;

	// Token: 0x06000EAF RID: 3759 RVA: 0x000442C4 File Offset: 0x000424C4
	public TCPServer(ushort port)
	{
		this.Server = new SimpleTcpServer("0.0.0.0", (int)port);
		this.Server.Settings.IdleClientTimeoutMs = 1000;
		this.Server.Settings.NoDelay = true;
		this.Server.Settings.UseAsyncDataReceivedEvents = false;
		this.Server.Events.ClientConnected += delegate(object sender, ConnectionEventArgs args)
		{
			Action<string> onClientConnected = this.OnClientConnected;
			if (onClientConnected == null)
			{
				return;
			}
			onClientConnected(args.IpPort);
		};
		this.Server.Events.ClientDisconnected += delegate(object sender, ConnectionEventArgs args)
		{
			Action<string> onClientDisconnected = this.OnClientDisconnected;
			if (onClientDisconnected == null)
			{
				return;
			}
			onClientDisconnected(args.IpPort);
		};
		this.Server.Events.DataReceived += delegate(object sender, DataReceivedEventArgs args)
		{
			this.OnDataReceived(sender, args);
		};
	}

	// Token: 0x06000EB0 RID: 3760 RVA: 0x00044374 File Offset: 0x00042574
	public void Start()
	{
		try
		{
			if (!this.Server.IsListening)
			{
				this.Server.Start();
				Debug.Log(string.Format("[TCPServer] Server started on port {0}", this.Server.Port));
				Action<ushort> onServerStarted = this.OnServerStarted;
				if (onServerStarted != null)
				{
					onServerStarted((ushort)this.Server.Port);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("[TCPServer] Server start failed on port {0}: {1}", this.Server.Port, ex.Message));
			Action<Exception> onServerStartFailed = this.OnServerStartFailed;
			if (onServerStartFailed != null)
			{
				onServerStartFailed(ex);
			}
		}
	}

	// Token: 0x06000EB1 RID: 3761 RVA: 0x00044428 File Offset: 0x00042628
	public void StartAsync()
	{
		Task.Run(delegate()
		{
			this.Start();
		});
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x0004443C File Offset: 0x0004263C
	public void Stop()
	{
		this.Server.Stop();
		Debug.Log(string.Format("[TCPServer] Server stopped on port {0}", this.Server.Port));
		Action<ushort> onServerStopped = this.OnServerStopped;
		if (onServerStopped == null)
		{
			return;
		}
		onServerStopped((ushort)this.Server.Port);
	}

	// Token: 0x06000EB3 RID: 3763 RVA: 0x0004448F File Offset: 0x0004268F
	public void StopAsync()
	{
		Task.Run(delegate()
		{
			this.Stop();
		});
	}

	// Token: 0x06000EB4 RID: 3764 RVA: 0x000444A4 File Offset: 0x000426A4
	public void SendMessage(string ipPort, string message)
	{
		try
		{
			byte[] bytes = Encoding.UTF8.GetBytes(message);
			this.Server.Send(ipPort, bytes);
			Action<string, string> onMessageSent = this.OnMessageSent;
			if (onMessageSent != null)
			{
				onMessageSent(ipPort, message);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[TCPServer] Error sending message to client " + ipPort + ": " + ex.Message);
		}
	}

	// Token: 0x06000EB5 RID: 3765 RVA: 0x00044510 File Offset: 0x00042710
	public void SendMessageAsync(string ipPort, string message)
	{
		Task.Run(delegate()
		{
			this.SendMessage(ipPort, message);
		});
	}

	// Token: 0x06000EB6 RID: 3766 RVA: 0x00044540 File Offset: 0x00042740
	public void DisconnectClient(string ipPort)
	{
		try
		{
			this.Server.DisconnectClient(ipPort);
		}
		catch (Exception ex)
		{
			Debug.LogError("[TCPServer] Error disconnecting client " + ipPort + ": " + ex.Message);
		}
	}

	// Token: 0x06000EB7 RID: 3767 RVA: 0x0004458C File Offset: 0x0004278C
	private void OnDataReceived(object sender, DataReceivedEventArgs args)
	{
		try
		{
			string @string = Encoding.UTF8.GetString(args.Data);
			Action<string, string> onMessageReceived = this.OnMessageReceived;
			if (onMessageReceived != null)
			{
				onMessageReceived(args.IpPort, @string);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[TCPServer] Error deserializing message from client " + args.IpPort + ": " + ex.Message);
		}
	}

	// Token: 0x040008DF RID: 2271
	public SimpleTcpServer Server;
}
