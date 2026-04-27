using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200009D RID: 157
public class ChatManager : NetworkBehaviourSingleton<ChatManager>
{
	// Token: 0x060004F4 RID: 1268 RVA: 0x0001B0EC File Offset: 0x000192EC
	public void AddChatMessage(ChatMessage chatMessage)
	{
		if (this.chatMessages.Count >= this.maxChatMessages)
		{
			this.RemoveChatMessage(this.chatMessages[0]);
		}
		this.chatMessages.Add(chatMessage);
		EventManager.TriggerEvent("Event_OnChatMessageAdded", new Dictionary<string, object>
		{
			{
				"chatMessage",
				chatMessage
			}
		});
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x0001B145 File Offset: 0x00019345
	public void RemoveChatMessage(ChatMessage chatMessage)
	{
		this.chatMessages.Remove(chatMessage);
		EventManager.TriggerEvent("Event_OnChatMessageRemoved", new Dictionary<string, object>
		{
			{
				"chatMessage",
				chatMessage
			}
		});
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x0001B16F File Offset: 0x0001936F
	public void ClearChatMessages()
	{
		this.chatMessages.Clear();
		EventManager.TriggerEvent("Event_OnChatMessagesCleared", null);
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x0001B187 File Offset: 0x00019387
	public string ParseChatMessageContent(string content)
	{
		content = content.Trim();
		if (content.Length > this.maxChatMessageLength)
		{
			content = content.Substring(0, this.maxChatMessageLength);
		}
		return content;
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x0001B1B0 File Offset: 0x000193B0
	public void SetQuickChatEnabled(bool isEnabled, QuickChatCategory? category = null)
	{
		this.isQuickChatEnabled = isEnabled;
		this.quickChatCategory = category;
		if (this.isQuickChatEnabled && this.quickChatCategory != null && this.quickChats.ContainsKey(this.quickChatCategory.Value))
		{
			EventManager.TriggerEvent("Event_OnQuickChatEnabled", new Dictionary<string, object>
			{
				{
					"category",
					this.quickChatCategory.Value
				},
				{
					"quickChats",
					this.quickChats[this.quickChatCategory.Value]
				}
			});
			return;
		}
		EventManager.TriggerEvent("Event_OnQuickChatDisabled", null);
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x0001B250 File Offset: 0x00019450
	public void Client_SendChatMessage(string content, bool isQuickChat, bool isTeamChat)
	{
		content = this.ParseChatMessageContent(content);
		if (string.IsNullOrEmpty(content))
		{
			return;
		}
		this.Client_SendChatMessageRpc(content, isQuickChat, isTeamChat, default(RpcParams));
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x0001B284 File Offset: 0x00019484
	public void Client_QuickChatAction(int index)
	{
		if (this.isQuickChatEnabled)
		{
			if (this.quickChatCategory == null)
			{
				return;
			}
			if (!this.quickChats.ContainsKey(this.quickChatCategory.Value))
			{
				return;
			}
			if (index < 0 || index >= this.quickChats[this.quickChatCategory.Value].Length)
			{
				return;
			}
			QuickChat quickChat = this.quickChats[this.quickChatCategory.Value][index];
			this.SetQuickChatEnabled(false, null);
			this.Client_SendChatMessageRpc(quickChat.Content, true, quickChat.IsTeamChat, default(RpcParams));
			return;
		}
		else
		{
			if (!Enum.IsDefined(typeof(QuickChatCategory), index))
			{
				return;
			}
			this.SetQuickChatEnabled(true, new QuickChatCategory?((QuickChatCategory)index));
			Tween tween = this.quickChatTimeoutTween;
			if (tween != null)
			{
				tween.Kill(false);
			}
			this.quickChatTimeoutTween = DOVirtual.DelayedCall(5f, delegate
			{
				this.SetQuickChatEnabled(false, null);
			}, true);
			return;
		}
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x0001B384 File Offset: 0x00019584
	[Rpc(SendTo.Server, DeferLocal = true)]
	private void Client_SendChatMessageRpc(string content, bool isQuickChat, bool isTeamChat, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3638797367U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			bool flag = content != null;
			fastBufferWriter.WriteValueSafe<bool>(flag, default(FastBufferWriter.ForPrimitives));
			if (flag)
			{
				fastBufferWriter.WriteValueSafe(content, false);
			}
			fastBufferWriter.WriteValueSafe<bool>(isQuickChat, default(FastBufferWriter.ForPrimitives));
			fastBufferWriter.WriteValueSafe<bool>(isTeamChat, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 3638797367U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		content = this.ParseChatMessageContent(content.ToString());
		if (string.IsNullOrEmpty(content.ToString()))
		{
			return;
		}
		ulong senderClientId = rpcParams.Receive.SenderClientId;
		Player component = NetworkManager.Singleton.ConnectedClients[senderClientId].PlayerObject.GetComponent<Player>();
		if (!component)
		{
			return;
		}
		if (!component.IsChatAvailable)
		{
			this.Server_SendChatMessageToClients(string.Format("Chat disabled for {0} seconds", Mathf.RoundToInt(1f / component.ChatTicketsPerSecond)), new ulong[]
			{
				senderClientId
			});
			return;
		}
		component.Server_ConsumeChatTicket();
		Debug.Log(string.Format("[ChatManager] Received chat message from player {0} ({1}): {2}", component.Username.Value, senderClientId, content));
		if (content[0] == '/')
		{
			string[] array = content.ToString().Split(" ", StringSplitOptions.None);
			string value = array[0];
			string[] value2 = array.Skip(1).ToArray<string>();
			EventManager.TriggerEvent("Event_Server_OnChatCommand", new Dictionary<string, object>
			{
				{
					"clientId",
					senderClientId
				},
				{
					"command",
					value
				},
				{
					"args",
					value2
				}
			});
			return;
		}
		if (component.IsMuted.Value)
		{
			Debug.Log(string.Format("[ChatManager] Player {0} ({1}) attempted to send a chat message but is muted", component.Username.Value, senderClientId));
			this.Server_SendChatMessageToClients("Chat disabled due to active mute", new ulong[]
			{
				senderClientId
			});
			return;
		}
		ChatMessage value3 = new ChatMessage
		{
			SteamID = new FixedString32Bytes?(component.SteamId.Value),
			Username = new FixedString32Bytes?(component.Username.Value),
			Team = new PlayerTeam?(component.Team),
			Content = content,
			Timestamp = Utils.GetTimestamp(),
			IsQuickChat = isQuickChat,
			IsTeamChat = isTeamChat,
			IsSystem = false
		};
		EventManager.TriggerEvent("Event_Server_OnChatMessageReceived", new Dictionary<string, object>
		{
			{
				"chatMessage",
				value3
			}
		});
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x0001B6AF File Offset: 0x000198AF
	public void Server_SendChatMessageToClients(ChatMessage chatMessage, ulong[] clientIds)
	{
		this.Server_SendChatMessageRpc(chatMessage, base.RpcTarget.Group(clientIds, RpcTargetUse.Temp));
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x0001B6CC File Offset: 0x000198CC
	public void Server_SendChatMessageToClients(string content, ulong[] clientIds)
	{
		ChatMessage chatMessage = new ChatMessage
		{
			SteamID = null,
			Username = null,
			Team = null,
			Content = content,
			Timestamp = Utils.GetTimestamp(),
			IsQuickChat = false,
			IsTeamChat = false,
			IsSystem = true
		};
		this.Server_SendChatMessageToClients(chatMessage, clientIds);
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x0001B737 File Offset: 0x00019937
	public void Server_BroadcastChatMessage(ChatMessage chatMessage)
	{
		this.Server_SendChatMessageRpc(chatMessage, base.RpcTarget.Everyone);
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x0001B750 File Offset: 0x00019950
	public void Server_BroadcastChatMessage(string content)
	{
		ChatMessage chatMessage = new ChatMessage
		{
			SteamID = null,
			Username = null,
			Team = null,
			Content = content,
			Timestamp = Utils.GetTimestamp(),
			IsQuickChat = false,
			IsTeamChat = false,
			IsSystem = true
		};
		this.Server_BroadcastChatMessage(chatMessage);
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x0001B7BC File Offset: 0x000199BC
	[Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server, DeferLocal = true)]
	private void Server_SendChatMessageRpc(ChatMessage chatMessage, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 846499610U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
			bool flag = chatMessage != null;
			fastBufferWriter.WriteValueSafe<bool>(flag, default(FastBufferWriter.ForPrimitives));
			if (flag)
			{
				fastBufferWriter.WriteValueSafe<ChatMessage>(chatMessage, default(FastBufferWriter.ForNetworkSerializable));
			}
			base.__endSendRpc(ref fastBufferWriter, 846499610U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.AddChatMessage(chatMessage);
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x0001B938 File Offset: 0x00019B38
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x0001B950 File Offset: 0x00019B50
	protected override void __initializeRpcs()
	{
		base.__registerRpc(3638797367U, new NetworkBehaviour.RpcReceiveHandler(ChatManager.__rpc_handler_3638797367), "Client_SendChatMessageRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(846499610U, new NetworkBehaviour.RpcReceiveHandler(ChatManager.__rpc_handler_846499610), "Server_SendChatMessageRpc", RpcInvokePermission.Server);
		base.__initializeRpcs();
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x0001B9A8 File Offset: 0x00019BA8
	private static void __rpc_handler_3638797367(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool flag;
		reader.ReadValueSafe<bool>(out flag, default(FastBufferWriter.ForPrimitives));
		string content = null;
		if (flag)
		{
			reader.ReadValueSafe(out content, false);
		}
		bool isQuickChat;
		reader.ReadValueSafe<bool>(out isQuickChat, default(FastBufferWriter.ForPrimitives));
		bool isTeamChat;
		reader.ReadValueSafe<bool>(out isTeamChat, default(FastBufferWriter.ForPrimitives));
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((ChatManager)target).Client_SendChatMessageRpc(content, isQuickChat, isTeamChat, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x0001BA84 File Offset: 0x00019C84
	private static void __rpc_handler_846499610(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool flag;
		reader.ReadValueSafe<bool>(out flag, default(FastBufferWriter.ForPrimitives));
		ChatMessage chatMessage = null;
		if (flag)
		{
			reader.ReadValueSafe<ChatMessage>(out chatMessage, default(FastBufferWriter.ForNetworkSerializable));
		}
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((ChatManager)target).Server_SendChatMessageRpc(chatMessage, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x0001BB2C File Offset: 0x00019D2C
	protected internal override string __getTypeName()
	{
		return "ChatManager";
	}

	// Token: 0x04000315 RID: 789
	[Header("Settings")]
	[SerializeField]
	private int maxChatMessageLength = 128;

	// Token: 0x04000316 RID: 790
	[SerializeField]
	private int maxChatMessages = 100;

	// Token: 0x04000317 RID: 791
	[SerializeField]
	private SerializedDictionary<QuickChatCategory, QuickChat[]> quickChats = new SerializedDictionary<QuickChatCategory, QuickChat[]>();

	// Token: 0x04000318 RID: 792
	private List<ChatMessage> chatMessages = new List<ChatMessage>();

	// Token: 0x04000319 RID: 793
	private bool isQuickChatEnabled;

	// Token: 0x0400031A RID: 794
	private QuickChatCategory? quickChatCategory;

	// Token: 0x0400031B RID: 795
	private Tween quickChatTimeoutTween;
}
