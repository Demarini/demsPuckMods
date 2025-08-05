using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SocketIOClient;
using SocketIOClient.Transport.Http;
using UnityEngine;

// Token: 0x020000CA RID: 202
public class WebSocketManager : MonoBehaviourSingleton<WebSocketManager>
{
	// Token: 0x17000089 RID: 137
	// (get) Token: 0x0600061A RID: 1562 RVA: 0x0000AD71 File Offset: 0x00008F71
	[HideInInspector]
	public string SocketId
	{
		get
		{
			return this.socket.Id;
		}
	}

	// Token: 0x0600061B RID: 1563 RVA: 0x0000AD7E File Offset: 0x00008F7E
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x0000AD86 File Offset: 0x00008F86
	private void Start()
	{
		this.Connect("wss://puck1.nasejevs.com");
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x00024FFC File Offset: 0x000231FC
	private void Connect(string uri)
	{
		WebSocketManager.<Connect>d__9 <Connect>d__;
		<Connect>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Connect>d__.<>4__this = this;
		<Connect>d__.uri = uri;
		<Connect>d__.<>1__state = -1;
		<Connect>d__.<>t__builder.Start<WebSocketManager.<Connect>d__9>(ref <Connect>d__);
	}

	// Token: 0x0600061E RID: 1566 RVA: 0x0002503C File Offset: 0x0002323C
	private Task Disconnect()
	{
		if (this.socket == null)
		{
			return Task.CompletedTask;
		}
		Debug.Log("[WebSocketManager] WebSocket disconnecting from " + this.uri + "...");
		this.socket.OnConnected -= this.OnConnected;
		this.socket.OnDisconnected -= this.OnDisconnected;
		this.socket.OnError -= this.OnError;
		this.socket.OnPing -= this.OnPing;
		this.socket.OnPong -= this.OnPong;
		this.socket.OnReconnectAttempt -= this.OnReconnectAttempt;
		this.socket.OnReconnected -= this.OnReconnected;
		this.socket.OnReconnectError -= this.OnReconnectError;
		this.socket.OnReconnectFailed -= this.OnReconnectFailed;
		this.socket.OffAny(new OnAnyHandler(this.OnAny));
		if (this.socket.Connected)
		{
			return this.socket.DisconnectAsync();
		}
		this.cancellationTokenSource.Cancel();
		return Task.CompletedTask;
	}

	// Token: 0x0600061F RID: 1567 RVA: 0x00025180 File Offset: 0x00023380
	public void Emit(string messageName, Dictionary<string, object> data = null, string responseMessageName = null)
	{
		Action<SocketIOResponse> <>9__1;
		MonoBehaviourSingleton<ThreadManager>.Instance.Enqueue(delegate()
		{
			string text = (responseMessageName != null) ? " request " : " ";
			Debug.Log(string.Concat(new string[]
			{
				"[WebSocketManager] WebSocket sent",
				text,
				"message ",
				messageName,
				" ",
				JsonSerializer.Serialize<Dictionary<string, object>>(data, null)
			}));
			Action<SocketIOResponse> action;
			if ((action = <>9__1) == null)
			{
				action = (<>9__1 = delegate(SocketIOResponse response)
				{
					Debug.Log(string.Format("[WebSocketManager] WebSocket received response message {0} {1}", responseMessageName, response));
					if (responseMessageName != null)
					{
						this.TriggerMessage(responseMessageName, new Dictionary<string, object>
						{
							{
								"response",
								response
							}
						});
					}
				});
			}
			Action<SocketIOResponse> ack = action;
			this.socket.EmitAsync(messageName, ack, new object[]
			{
				data
			});
			this.TriggerMessage("emit", new Dictionary<string, object>
			{
				{
					"messageName",
					messageName
				}
			});
		});
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x0000AD93 File Offset: 0x00008F93
	private void OnConnected(object sender, EventArgs args)
	{
		Debug.Log("[WebSocketManager] WebSocket connected to " + this.uri);
		this.TriggerMessage("connect", new Dictionary<string, object>
		{
			{
				"socket",
				this.socket
			}
		});
	}

	// Token: 0x06000621 RID: 1569 RVA: 0x0000ADCB File Offset: 0x00008FCB
	private void OnDisconnected(object sender, string reason)
	{
		Debug.Log("[WebSocketManager] WebSocket disconnected (" + reason + ")");
		this.TriggerMessage("disconnect", null);
	}

	// Token: 0x06000622 RID: 1570 RVA: 0x0000ADEE File Offset: 0x00008FEE
	private void OnError(object sender, string error)
	{
		Debug.Log("[WebSocketManager] WebSocket error: " + error);
	}

	// Token: 0x06000623 RID: 1571 RVA: 0x00006C1B File Offset: 0x00004E1B
	private void OnPing(object sender, EventArgs args)
	{
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x00006C1B File Offset: 0x00004E1B
	private void OnPong(object sender, TimeSpan timeSpan)
	{
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x0000AE00 File Offset: 0x00009000
	private void OnReconnectAttempt(object sender, int attempt)
	{
		Debug.Log(string.Format("[WebSocketManager] WebSocket reconnect attempt: {0}", attempt));
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x0000AE17 File Offset: 0x00009017
	private void OnReconnected(object sender, int attempt)
	{
		Debug.Log(string.Format("[WebSocketManager] WebSocket reconnected on attempt {0}", attempt));
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x000251C8 File Offset: 0x000233C8
	private void OnReconnectError(object sender, Exception exception)
	{
		Debug.Log("[WebSocketManager] WebSocket reconnect error: " + exception.Message);
		if (exception.ToString().Contains("UNITYTLS_X509VERIFY_FLAG_NOT_TRUSTED"))
		{
			Debug.Log("[WebSocketManager] UNITYTLS_X509VERIFY_FLAG_NOT_TRUSTED, disconnecting...");
			this.options.AutoUpgrade = false;
			this.options.RemoteCertificateValidationCallback = ((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true);
			this.socket.Options.AutoUpgrade = this.options.AutoUpgrade;
			this.socket.HttpClient = new DefaultHttpClient(this.options.RemoteCertificateValidationCallback);
		}
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x0000AE2E File Offset: 0x0000902E
	private void OnReconnectFailed(object sender, EventArgs args)
	{
		Debug.Log(string.Format("[WebSocketManager] WebSocket reconnect failed {0}", args));
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x0000AE40 File Offset: 0x00009040
	private void OnAny(string messageName, SocketIOResponse response)
	{
		Debug.Log(string.Format("[WebSocketManager] WebSocket received message {0} {1}", messageName, response));
		this.TriggerMessage(messageName, new Dictionary<string, object>
		{
			{
				"response",
				response
			}
		});
	}

	// Token: 0x0600062A RID: 1578 RVA: 0x00025274 File Offset: 0x00023474
	public void AddMessageListener(string messageName, Action<Dictionary<string, object>> listener)
	{
		if (this.events.ContainsKey(messageName))
		{
			Dictionary<string, Action<Dictionary<string, object>>> dictionary = this.events;
			dictionary[messageName] = (Action<Dictionary<string, object>>)Delegate.Combine(dictionary[messageName], listener);
			return;
		}
		Action<Dictionary<string, object>> action = null;
		action = (Action<Dictionary<string, object>>)Delegate.Combine(action, listener);
		this.events.Add(messageName, action);
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x000252D0 File Offset: 0x000234D0
	public void RemoveMessageListener(string messageName, Action<Dictionary<string, object>> listener)
	{
		if (this.events.ContainsKey(messageName))
		{
			Dictionary<string, Action<Dictionary<string, object>>> dictionary = this.events;
			dictionary[messageName] = (Action<Dictionary<string, object>>)Delegate.Remove(dictionary[messageName], listener);
		}
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x00025310 File Offset: 0x00023510
	public void TriggerMessage(string messageName, Dictionary<string, object> message = null)
	{
		MonoBehaviourSingleton<ThreadManager>.Instance.Enqueue(delegate()
		{
			if (this.events.ContainsKey(messageName))
			{
				Action<Dictionary<string, object>> action = this.events[messageName];
				if (action == null)
				{
					return;
				}
				action(message);
			}
		});
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x00025350 File Offset: 0x00023550
	private void OnApplicationQuit()
	{
		WebSocketManager.<OnApplicationQuit>d__25 <OnApplicationQuit>d__;
		<OnApplicationQuit>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnApplicationQuit>d__.<>4__this = this;
		<OnApplicationQuit>d__.<>1__state = -1;
		<OnApplicationQuit>d__.<>t__builder.Start<WebSocketManager.<OnApplicationQuit>d__25>(ref <OnApplicationQuit>d__);
	}

	// Token: 0x0400034D RID: 845
	private string uri;

	// Token: 0x0400034E RID: 846
	private SocketIO socket;

	// Token: 0x0400034F RID: 847
	private SocketIOOptions options = new SocketIOOptions
	{
		ReconnectionDelay = 2500.0,
		ReconnectionDelayMax = 5000
	};

	// Token: 0x04000350 RID: 848
	private CancellationTokenSource cancellationTokenSource;

	// Token: 0x04000351 RID: 849
	private readonly Dictionary<string, Action<Dictionary<string, object>>> events = new Dictionary<string, Action<Dictionary<string, object>>>();
}
