using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using SocketIO.Serializer.SystemTextJson;
using SocketIOClient;
using SocketIOClient.Transport;
using UnityEngine;

// Token: 0x0200014C RID: 332
public static class WebSocketManager
{
	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x060009E5 RID: 2533 RVA: 0x0002FE74 File Offset: 0x0002E074
	public static bool IsConnected
	{
		get
		{
			return WebSocketManager.socket != null && WebSocketManager.socket.Connected;
		}
	}

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x060009E6 RID: 2534 RVA: 0x0002FE89 File Offset: 0x0002E089
	public static bool IsReconnecting
	{
		get
		{
			return WebSocketManager.socket != null && !WebSocketManager.socket.Connected && !WebSocketManager.IsConnectionInProgress;
		}
	}

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x060009E7 RID: 2535 RVA: 0x0002FEA8 File Offset: 0x0002E0A8
	public static bool IsConnectionInProgress
	{
		get
		{
			return WebSocketManager.cancellationTokenSource != null;
		}
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x0002FEB4 File Offset: 0x0002E0B4
	public static void Initialize()
	{
		bool flag;
		WebSocketManager.forcePolling = (bool.TryParse(Utils.GetCommandLineArgument("--polling", null), out flag) && flag);
		WebSocketManagerController.Initialize();
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x0002FEE3 File Offset: 0x0002E0E3
	public static void Dispose()
	{
		WebSocketManagerController.Dispose();
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x0002FEEC File Offset: 0x0002E0EC
	public static Task Connect(string url)
	{
		WebSocketManager.<Connect>d__13 <Connect>d__;
		<Connect>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<Connect>d__.url = url;
		<Connect>d__.<>1__state = -1;
		<Connect>d__.<>t__builder.Start<WebSocketManager.<Connect>d__13>(ref <Connect>d__);
		return <Connect>d__.<>t__builder.Task;
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x0002FF30 File Offset: 0x0002E130
	public static Task CancelConnection()
	{
		WebSocketManager.<CancelConnection>d__14 <CancelConnection>d__;
		<CancelConnection>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<CancelConnection>d__.<>1__state = -1;
		<CancelConnection>d__.<>t__builder.Start<WebSocketManager.<CancelConnection>d__14>(ref <CancelConnection>d__);
		return <CancelConnection>d__.<>t__builder.Task;
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x0002FF6C File Offset: 0x0002E16C
	public static Task Disconnect()
	{
		WebSocketManager.<Disconnect>d__15 <Disconnect>d__;
		<Disconnect>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<Disconnect>d__.<>1__state = -1;
		<Disconnect>d__.<>t__builder.Start<WebSocketManager.<Disconnect>d__15>(ref <Disconnect>d__);
		return <Disconnect>d__.<>t__builder.Task;
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x0002FFA8 File Offset: 0x0002E1A8
	public static void CreateSocket(string url)
	{
		if (WebSocketManager.socket != null)
		{
			WebSocketManager.DisposeSocket();
		}
		bool flag = url.StartsWith("wss://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
		TransportProtocol transportProtocol = WebSocketManager.forcePolling ? TransportProtocol.Polling : TransportProtocol.WebSocket;
		RemoteCertificateValidationCallback remoteCertificateValidationCallback;
		if (!flag)
		{
			remoteCertificateValidationCallback = ((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true);
		}
		else
		{
			remoteCertificateValidationCallback = null;
		}
		RemoteCertificateValidationCallback remoteCertificateValidationCallback2 = remoteCertificateValidationCallback;
		Debug.Log(string.Format("[WebsocketManager] Creating WebSocket (url: {0}, transport: {1}, certificate validation: {2})", url, transportProtocol, remoteCertificateValidationCallback2 == null));
		SocketIOOptions options = new SocketIOOptions
		{
			Transport = transportProtocol,
			RemoteCertificateValidationCallback = remoteCertificateValidationCallback2,
			AutoUpgrade = false,
			ReconnectionDelay = 5000.0,
			ReconnectionDelayMax = 10000,
			RandomizationFactor = 0.5,
			ConnectionTimeout = TimeSpan.FromMilliseconds(5000.0)
		};
		WebSocketManager.socket = new SocketIO(url, options);
		WebSocketManager.socket.Serializer = new SystemTextJsonSerializer(WebSocketManager.JsonOptions);
		WebSocketManager.socket.OnConnected += WebSocketManager.OnConnected;
		WebSocketManager.socket.OnDisconnected += WebSocketManager.OnDisconnected;
		WebSocketManager.socket.OnError += WebSocketManager.OnError;
		WebSocketManager.socket.OnPing += WebSocketManager.OnPing;
		WebSocketManager.socket.OnPong += WebSocketManager.OnPong;
		WebSocketManager.socket.OnReconnectAttempt += WebSocketManager.OnReconnectAttempt;
		WebSocketManager.socket.OnReconnected += WebSocketManager.OnReconnected;
		WebSocketManager.socket.OnReconnectError += WebSocketManager.OnReconnectError;
		WebSocketManager.socket.OnReconnectFailed += WebSocketManager.OnReconnectFailed;
		WebSocketManager.socket.OnAny(new OnAnyHandler(WebSocketManager.OnAny));
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x00030184 File Offset: 0x0002E384
	public static void DisposeSocket()
	{
		if (WebSocketManager.socket == null)
		{
			return;
		}
		Debug.Log("[WebsocketManager] Disposing WebSocket");
		WebSocketManager.socket.OnConnected -= WebSocketManager.OnConnected;
		WebSocketManager.socket.OnDisconnected -= WebSocketManager.OnDisconnected;
		WebSocketManager.socket.OnError -= WebSocketManager.OnError;
		WebSocketManager.socket.OnPing -= WebSocketManager.OnPing;
		WebSocketManager.socket.OnPong -= WebSocketManager.OnPong;
		WebSocketManager.socket.OnReconnectAttempt -= WebSocketManager.OnReconnectAttempt;
		WebSocketManager.socket.OnReconnected -= WebSocketManager.OnReconnected;
		WebSocketManager.socket.OnReconnectError -= WebSocketManager.OnReconnectError;
		WebSocketManager.socket.OnReconnectFailed -= WebSocketManager.OnReconnectFailed;
		WebSocketManager.socket.OffAny(new OnAnyHandler(WebSocketManager.OnAny));
		WebSocketManager.socket = null;
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x00030288 File Offset: 0x0002E488
	public static void Emit(string messageName, Dictionary<string, object> data = null, string responseMessageName = null)
	{
		OutMessage outMessage = new OutMessage(messageName, data, responseMessageName);
		if (outMessage.IsRequestMessage)
		{
			Debug.Log(string.Format("[WebsocketManager] WebSocket sending request message {0} ({1})", outMessage.MessageName, outMessage));
			Action<SocketIOResponse> ack = delegate(SocketIOResponse response)
			{
				InMessage inMessage = new InMessage(outMessage.ResponseMessageName, response);
				Debug.Log(string.Format("[WebsocketManager] WebSocket received response to message {0} -> {1} ({2})", outMessage.MessageName, inMessage.MessageName, inMessage));
				WebSocketManager.TriggerMessage(inMessage.MessageName, new Dictionary<string, object>
				{
					{
						"outMessage",
						outMessage
					},
					{
						"inMessage",
						inMessage
					}
				});
			};
			WebSocketManager.socket.EmitAsync(outMessage.MessageName, ack, new object[]
			{
				outMessage.Data
			});
		}
		else
		{
			Debug.Log(string.Format("[WebsocketManager] WebSocket sending message {0} ({1})", outMessage.MessageName, outMessage));
			WebSocketManager.socket.EmitAsync(outMessage.MessageName, new object[]
			{
				outMessage.Data
			});
		}
		WebSocketManager.TriggerMessage("emit", new Dictionary<string, object>
		{
			{
				"messageName",
				outMessage.MessageName
			}
		});
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x0003037F File Offset: 0x0002E57F
	private static void OnConnected(object sender, EventArgs args)
	{
		Debug.Log("[WebsocketManager] WebSocket connected");
		WebSocketManager.TriggerMessage("connected", new Dictionary<string, object>
		{
			{
				"socket",
				WebSocketManager.socket
			}
		});
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x000303AA File Offset: 0x0002E5AA
	private static void OnDisconnected(object sender, string reason)
	{
		Debug.LogWarning("[WebsocketManager] WebSocket disconnected (" + reason + ")");
		WebSocketManager.TriggerMessage("disconnected", null);
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x000303CC File Offset: 0x0002E5CC
	private static void OnError(object sender, string error)
	{
		Debug.LogError("[WebsocketManager] WebSocket error: " + error);
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x000020D3 File Offset: 0x000002D3
	private static void OnPing(object sender, EventArgs args)
	{
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x000020D3 File Offset: 0x000002D3
	private static void OnPong(object sender, TimeSpan timeSpan)
	{
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x000303DE File Offset: 0x0002E5DE
	private static void OnReconnectAttempt(object sender, int attempt)
	{
		Debug.LogWarning(string.Format("[WebsocketManager] WebSocket reconnect attempt: {0}", attempt));
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x000303F5 File Offset: 0x0002E5F5
	private static void OnReconnected(object sender, int attempt)
	{
		Debug.Log("[WebsocketManager] WebSocket reconnected");
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x00030401 File Offset: 0x0002E601
	private static void OnReconnectError(object sender, Exception exception)
	{
		Debug.LogError(string.Format("[WebsocketManager] WebSocket reconnect error: {0}", exception));
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x00030413 File Offset: 0x0002E613
	private static void OnReconnectFailed(object sender, EventArgs args)
	{
		Debug.LogError(string.Format("[WebsocketManager] WebSocket reconnect failed ({0})", args));
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x00030428 File Offset: 0x0002E628
	private static void OnAny(string messageName, SocketIOResponse response)
	{
		InMessage inMessage = new InMessage(messageName, response);
		Debug.Log(string.Format("[WebsocketManager] WebSocket received message {0} ({1})", messageName, inMessage));
		WebSocketManager.TriggerMessage(messageName, new Dictionary<string, object>
		{
			{
				"inMessage",
				inMessage
			}
		});
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x00030468 File Offset: 0x0002E668
	public static void AddMessageListener(string messageName, Action<Dictionary<string, object>> listener)
	{
		if (WebSocketManager.events.ContainsKey(messageName))
		{
			Dictionary<string, Action<Dictionary<string, object>>> dictionary = WebSocketManager.events;
			dictionary[messageName] = (Action<Dictionary<string, object>>)Delegate.Combine(dictionary[messageName], listener);
			return;
		}
		Action<Dictionary<string, object>> action = null;
		action = (Action<Dictionary<string, object>>)Delegate.Combine(action, listener);
		WebSocketManager.events.Add(messageName, action);
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x000304C0 File Offset: 0x0002E6C0
	public static void RemoveMessageListener(string messageName, Action<Dictionary<string, object>> listener)
	{
		if (WebSocketManager.events.ContainsKey(messageName))
		{
			Dictionary<string, Action<Dictionary<string, object>>> dictionary = WebSocketManager.events;
			dictionary[messageName] = (Action<Dictionary<string, object>>)Delegate.Remove(dictionary[messageName], listener);
		}
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x000304FC File Offset: 0x0002E6FC
	public static void TriggerMessage(string messageName, Dictionary<string, object> message = null)
	{
		MonoBehaviourSingleton<ThreadManager>.Instance.Enqueue(delegate()
		{
			if (WebSocketManager.events.ContainsKey(messageName))
			{
				Action<Dictionary<string, object>> action = WebSocketManager.events[messageName];
				if (action == null)
				{
					return;
				}
				action(message);
			}
		});
	}

	// Token: 0x040005B6 RID: 1462
	public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
	{
		Converters = 
		{
			new JsonStringEnumConverter()
		}
	};

	// Token: 0x040005B7 RID: 1463
	private static Dictionary<string, Action<Dictionary<string, object>>> events = new Dictionary<string, Action<Dictionary<string, object>>>();

	// Token: 0x040005B8 RID: 1464
	private static SocketIO socket = null;

	// Token: 0x040005B9 RID: 1465
	private static CancellationTokenSource cancellationTokenSource = null;

	// Token: 0x040005BA RID: 1466
	private static bool forcePolling = false;
}
