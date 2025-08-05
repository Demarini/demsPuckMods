using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

// Token: 0x0200016C RID: 364
public class UDPSocket
{
	// Token: 0x06000C78 RID: 3192 RVA: 0x0000F1E5 File Offset: 0x0000D3E5
	public void StartSocket(ushort port)
	{
		this.Listen(port);
	}

	// Token: 0x06000C79 RID: 3193 RVA: 0x000421DC File Offset: 0x000403DC
	public void StopSocket()
	{
		if (this.udpClient == null || this.udpClient.Client == null || !this.udpClient.Client.IsBound)
		{
			return;
		}
		this.udpClient.Close();
		MonoBehaviourSingleton<ThreadManager>.Instance.Enqueue(delegate()
		{
			Action onSocketStopped = this.OnSocketStopped;
			if (onSocketStopped == null)
			{
				return;
			}
			onSocketStopped();
		});
	}

	// Token: 0x06000C7A RID: 3194 RVA: 0x00042234 File Offset: 0x00040434
	public void SendCallback(IAsyncResult asyncResult)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)asyncResult.AsyncState;
		string ipAddress = (string)dictionary["ipAddress"];
		ushort port = (ushort)dictionary["port"];
		string message = (string)dictionary["message"];
		try
		{
			this.udpClient.EndSend(asyncResult);
			long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			MonoBehaviourSingleton<ThreadManager>.Instance.Enqueue(delegate()
			{
				Action<string, ushort, string, long> onUdpMessageSent = this.OnUdpMessageSent;
				if (onUdpMessageSent == null)
				{
					return;
				}
				onUdpMessageSent(ipAddress, port, message, timestamp);
			});
		}
		catch (ObjectDisposedException)
		{
			Debug.Log("[UDPSocket] Cancelled send UDP message due to socket closure");
		}
		catch (Exception arg)
		{
			Debug.Log(string.Format("[UDPSocket] Failed to end send UDP message: {0}", arg));
		}
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x00042310 File Offset: 0x00040510
	public void ReceiveCallback(IAsyncResult asyncResult)
	{
		IPEndPoint ipEndPoint = null;
		try
		{
			this.udpClient.BeginReceive(new AsyncCallback(this.ReceiveCallback), null);
			byte[] bytes = this.udpClient.EndReceive(asyncResult, ref ipEndPoint);
			string message = Encoding.ASCII.GetString(bytes);
			long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			MonoBehaviourSingleton<ThreadManager>.Instance.Enqueue(delegate()
			{
				Action<string, ushort, string, long> onUdpMessageReceived = this.OnUdpMessageReceived;
				if (onUdpMessageReceived == null)
				{
					return;
				}
				onUdpMessageReceived(ipEndPoint.Address.ToString(), (ushort)ipEndPoint.Port, message, timestamp);
			});
		}
		catch (ObjectDisposedException)
		{
			Debug.Log("[UDPSocket] Cancelled receive UDP message due to socket closure");
		}
		catch (Exception arg)
		{
			Debug.Log(string.Format("[UDPSocket] Failed to receive UDP message: {0}", arg));
		}
	}

	// Token: 0x06000C7C RID: 3196 RVA: 0x000423D8 File Offset: 0x000405D8
	private void Listen(ushort port)
	{
		try
		{
			IPEndPoint localEP = new IPEndPoint(IPAddress.Any, (int)port);
			this.udpClient = new UdpClient(localEP);
			this.udpClient.BeginReceive(new AsyncCallback(this.ReceiveCallback), null);
			MonoBehaviourSingleton<ThreadManager>.Instance.Enqueue(delegate()
			{
				Action<ushort> onSocketStarted = this.OnSocketStarted;
				if (onSocketStarted == null)
				{
					return;
				}
				onSocketStarted(port);
			});
		}
		catch (Exception arg)
		{
			Debug.Log(string.Format("[UDPSocket] Failed to open UDP port: {0}", arg));
			MonoBehaviourSingleton<ThreadManager>.Instance.Enqueue(delegate()
			{
				Action<ushort> onSocketFailed = this.OnSocketFailed;
				if (onSocketFailed == null)
				{
					return;
				}
				onSocketFailed(port);
			});
		}
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x00042480 File Offset: 0x00040680
	public void Send(string ipAddress, ushort port, string message)
	{
		try
		{
			byte[] bytes = Encoding.ASCII.GetBytes(message);
			this.udpClient.BeginSend(bytes, bytes.Length, ipAddress, (int)port, new AsyncCallback(this.SendCallback), new Dictionary<string, object>
			{
				{
					"ipAddress",
					ipAddress
				},
				{
					"port",
					port
				},
				{
					"message",
					message
				}
			});
		}
		catch (Exception arg)
		{
			Debug.Log(string.Format("[UDPSocket] Failed to start send UDP message: {0}", arg));
		}
	}

	// Token: 0x04000735 RID: 1845
	public Action<ushort> OnSocketStarted;

	// Token: 0x04000736 RID: 1846
	public Action<ushort> OnSocketFailed;

	// Token: 0x04000737 RID: 1847
	public Action OnSocketStopped;

	// Token: 0x04000738 RID: 1848
	public Action<string, ushort, string, long> OnUdpMessageReceived;

	// Token: 0x04000739 RID: 1849
	public Action<string, ushort, string, long> OnUdpMessageSent;

	// Token: 0x0400073A RID: 1850
	private UdpClient udpClient;
}
