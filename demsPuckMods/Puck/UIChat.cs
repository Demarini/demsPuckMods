using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000101 RID: 257
public class UIChat : UIComponent<UIChat>
{
	// Token: 0x060008D6 RID: 2262 RVA: 0x00037060 File Offset: 0x00035260
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("ChatContainer", null);
		this.chatScrollView = this.container.Query("ChatScrollView", null);
		this.chatTextField = this.container.Query("ChatTextField", null);
		this.quickChatContainer = this.container.Query("QuickChatContainer", null);
		this.chatTextField.RegisterCallback<NavigationSubmitEvent>(delegate(NavigationSubmitEvent e)
		{
			this.SubmitChatMessage();
		}, TrickleDown.TrickleDown);
		this.chatTextField.RegisterCallback<NavigationCancelEvent>(delegate(NavigationCancelEvent e)
		{
			this.Blur();
		}, TrickleDown.TrickleDown);
		this.container.RegisterCallback<FocusOutEvent>(delegate(FocusOutEvent e)
		{
			if (Utils.GetVisualElementChildrenRecursive(this.container).Contains(e.relatedTarget))
			{
				return;
			}
			this.Blur();
		}, TrickleDown.TrickleDown);
		this.chatScrollView.contentViewport.RegisterCallback<GeometryChangedEvent>(delegate(GeometryChangedEvent e)
		{
			this.ScrollToBottom();
		}, TrickleDown.NoTrickleDown);
		this.chatScrollView.contentContainer.RegisterCallback<GeometryChangedEvent>(delegate(GeometryChangedEvent e)
		{
			if (!base.IsFocused)
			{
				this.ScrollToBottom();
			}
		}, TrickleDown.NoTrickleDown);
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x0000C740 File Offset: 0x0000A940
	private void Start()
	{
		base.FocusRequiresMouse = true;
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x0003715C File Offset: 0x0003535C
	private void Update()
	{
		foreach (Player player in this.playerRateLimits.Keys.ToList<Player>())
		{
			Dictionary<Player, float> dictionary = this.playerRateLimits;
			Player key = player;
			dictionary[key] -= Time.deltaTime / this.messageRatePeriod;
			if (this.playerRateLimits[player] <= 0f)
			{
				this.playerRateLimits.Remove(player);
			}
		}
		if (Application.isBatchMode)
		{
			return;
		}
		foreach (ChatMessage chatMessage in this.chatMessages.Values.ToList<ChatMessage>())
		{
			chatMessage.Update(Time.time);
		}
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x0000C749 File Offset: 0x0000A949
	public override void Show()
	{
		if (MonoBehaviourSingleton<SettingsManager>.Instance.ShowGameUserInterface == 0)
		{
			return;
		}
		base.Show();
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x0000C75E File Offset: 0x0000A95E
	public void Focus()
	{
		base.IsFocused = true;
		this.ShowChatInput();
		this.ShowChatMessages();
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x0000C773 File Offset: 0x0000A973
	public void Blur()
	{
		base.IsFocused = false;
		this.HideChatInput();
		this.HideChatMessages();
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x0000C788 File Offset: 0x0000A988
	private void ShowChatInput()
	{
		this.chatTextField.style.display = DisplayStyle.Flex;
		this.chatTextField.value = "";
		this.chatTextField.Focus();
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x0000C7BB File Offset: 0x0000A9BB
	private void HideChatInput()
	{
		this.chatTextField.style.display = DisplayStyle.None;
		this.chatTextField.value = "";
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x0000C7E3 File Offset: 0x0000A9E3
	private void SubmitChatMessage()
	{
		this.Client_SendClientChatMessage(this.chatTextField.value, this.UseTeamChat);
		this.chatTextField.value = "";
		this.chatTextField.Blur();
		this.HideChatInput();
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x00037250 File Offset: 0x00035450
	private void ShowChatMessages()
	{
		foreach (ChatMessage chatMessage in this.chatMessages.Values)
		{
			chatMessage.Show(0f, false);
		}
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x000372AC File Offset: 0x000354AC
	private void HideChatMessages()
	{
		foreach (ChatMessage chatMessage in this.chatMessages.Values)
		{
			chatMessage.Hide();
		}
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x00037304 File Offset: 0x00035504
	private void ScrollToBottom()
	{
		float to = Mathf.Max(this.chatScrollView.contentContainer.resolvedStyle.height - this.chatScrollView.contentViewport.resolvedStyle.height, 0f);
		Tween tween = this.scrollToBottomTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.scrollToBottomTween = DOVirtual.Float(this.chatScrollView.scrollOffset.y, to, 0.25f, delegate(float value)
		{
			this.chatScrollView.scrollOffset = new Vector2(0f, value);
		});
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x00037388 File Offset: 0x00035588
	public void AddChatMessage(string message)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		if (this.chatScrollView.childCount >= this.messageHistory)
		{
			this.chatScrollView.RemoveAt(0);
		}
		TemplateContainer templateContainer = Utils.InstantiateVisualTreeAsset(this.chatMessageAsset, Position.Absolute);
		Label label = templateContainer.Query("ChatMessageLabel", null);
		this.chatScrollView.Add(label);
		ChatMessage value = new ChatMessage(label, Time.time, message);
		this.chatMessages.Add(templateContainer, value);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnUserInterfaceNotification", null);
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x00037414 File Offset: 0x00035614
	public void ClearChatMessages()
	{
		foreach (ChatMessage chatMessage in this.chatMessages.Values)
		{
			chatMessage.Dispose();
		}
		this.playerRateLimits.Clear();
		this.chatMessages.Clear();
		this.chatScrollView.Clear();
		Tween tween = this.quickChatTimeoutTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.CloseQuickChat();
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x0000C81D File Offset: 0x0000AA1D
	public void SetOpacity(float opacity)
	{
		if (this.container == null)
		{
			return;
		}
		this.container.style.opacity = new StyleFloat(opacity);
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x000374A4 File Offset: 0x000356A4
	public void SetScale(float scale)
	{
		if (this.container == null)
		{
			return;
		}
		if (this.chatScrollView == null)
		{
			return;
		}
		this.container.style.fontSize = new Length(this.fontSize * scale, LengthUnit.Pixel);
		this.container.style.width = new Length(this.size.x * scale, LengthUnit.Pixel);
		this.chatScrollView.style.height = new Length(this.size.y * scale, LengthUnit.Pixel);
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x00037538 File Offset: 0x00035738
	private string ParseClientChatMessage(string message)
	{
		string newValue;
		if (MonoBehaviourSingleton<SettingsManager>.Instance.Units == "METRIC")
		{
			newValue = "KPH";
		}
		else
		{
			newValue = "MPH";
		}
		foreach (object obj in new Regex(string.Concat(new string[]
		{
			"(?<=",
			Regex.Escape("<united>"),
			")[^>]+(?=",
			Regex.Escape("</united>"),
			")"
		})).Matches(message))
		{
			Match match = (Match)obj;
			float value;
			if (float.TryParse(match.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
			{
				float num = (MonoBehaviourSingleton<SettingsManager>.Instance.Units == "METRIC") ? Utils.GameUnitsToMetric(value) : Utils.GameUnitsToImperial(value);
				message = message.Replace("<united>" + match.Value + "</united>", num.ToString("F1"));
			}
		}
		message = message.Replace("&units", newValue);
		return message;
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x0003766C File Offset: 0x0003586C
	public string WrapInTeamColor(PlayerTeam team, string message)
	{
		string text;
		if (team != PlayerTeam.Blue)
		{
			if (team != PlayerTeam.Red)
			{
				text = "#" + ColorUtility.ToHtmlStringRGB(this.teamSpectatorColor);
			}
			else
			{
				text = "#" + ColorUtility.ToHtmlStringRGB(this.teamRedColor);
			}
		}
		else
		{
			text = "#" + ColorUtility.ToHtmlStringRGB(this.teamBlueColor);
		}
		return string.Concat(new string[]
		{
			"<b><color=",
			text,
			"><noparse>",
			message,
			"</noparse></color></b>"
		});
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x000376F4 File Offset: 0x000358F4
	public string WrapPlayerUsername(Player player)
	{
		if (!player)
		{
			return "";
		}
		return this.WrapInTeamColor(player.Team.Value, string.Format("#{0} {1}", player.Number.Value, player.Username.Value));
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x0000C83E File Offset: 0x0000AA3E
	public void Server_SendSystemChatMessage(string message)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Server_ChatMessageRpc(message, base.RpcTarget.ClientsAndHost);
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x0000C864 File Offset: 0x0000AA64
	public void Server_SendSystemChatMessage(string message, ulong clientId)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Server_ChatMessageRpc(message, base.RpcTarget.Single(clientId, RpcTargetUse.Temp));
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x0003774C File Offset: 0x0003594C
	[Rpc(SendTo.SpecifiedInParams)]
	public void Server_ChatMessageRpc(string message, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3914580275U;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
			bool flag = message != null;
			fastBufferWriter.WriteValueSafe<bool>(flag, default(FastBufferWriter.ForPrimitives));
			if (flag)
			{
				fastBufferWriter.WriteValueSafe(message, false);
			}
			base.__endSendRpc(ref fastBufferWriter, 3914580275U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		message = this.ParseClientChatMessage(message);
		if (MonoBehaviourSingleton<SettingsManager>.Instance.FilterChatProfanity > 0)
		{
			message = Utils.FilterStringProfanity(message, true);
		}
		this.AddChatMessage(message);
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x00037868 File Offset: 0x00035A68
	private void Server_ProcessPlayerChatMessage(Player player, string message, ulong clientId, bool useTeamChat, bool isMuted)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!player)
		{
			return;
		}
		string text = useTeamChat ? "[TEAM]" : "[ALL]";
		Debug.Log(string.Format("[UIChat] {0} {1} ({2}) [{3}]: {4}", new object[]
		{
			text,
			player.Username.Value,
			player.OwnerClientId,
			player.SteamId.Value,
			message
		}));
		if (message[0] == '/')
		{
			string[] source = message.Split(" ", StringSplitOptions.None);
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnChatCommand", new Dictionary<string, object>
			{
				{
					"clientId",
					clientId
				},
				{
					"command",
					source.First<string>()
				},
				{
					"args",
					source.Skip(1).ToArray<string>()
				}
			});
			return;
		}
		if (!isMuted)
		{
			if (!this.playerRateLimits.ContainsKey(player))
			{
				this.playerRateLimits.Add(player, 0f);
			}
			if (this.playerRateLimits[player] + 1f >= this.messageRateLimit)
			{
				Debug.LogWarning(string.Format("[UIChat] {0} ({1}) [{2}] is rate limited. Ignoring message: {3}", new object[]
				{
					player.Username.Value,
					player.OwnerClientId,
					player.SteamId.Value,
					message
				}));
				return;
			}
			Dictionary<Player, float> dictionary = this.playerRateLimits;
			dictionary[player] += 1f;
			if (useTeamChat)
			{
				List<Player> list = new List<Player>();
				if (player.Team.Value == PlayerTeam.None || player.Team.Value == PlayerTeam.Spectator)
				{
					list.AddRange(NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayersByTeam(PlayerTeam.None, false));
					list.AddRange(NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayersByTeam(PlayerTeam.Spectator, false));
				}
				else
				{
					list.AddRange(NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayersByTeam(player.Team.Value, false));
				}
				ulong[] clientIds = (from p in list
				select p.OwnerClientId).ToArray<ulong>();
				this.Server_ChatMessageRpc(string.Concat(new string[]
				{
					text,
					" ",
					this.WrapPlayerUsername(player),
					": <noparse>",
					message,
					"</noparse>"
				}), base.RpcTarget.Group(clientIds, RpcTargetUse.Temp));
				return;
			}
			this.Server_ChatMessageRpc(this.WrapPlayerUsername(player) + ": <noparse>" + message + "</noparse>", base.RpcTarget.ClientsAndHost);
		}
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x00037B18 File Offset: 0x00035D18
	public void Client_SendClientChatMessage(string message, bool useTeamChat)
	{
		message = Utils.FilterStringSpecialCharacters(message);
		if (message.Length == 0)
		{
			return;
		}
		this.Client_ChatMessageRpc(message, useTeamChat, MonoBehaviourSingleton<StateManager>.Instance.IsMuted, default(RpcParams));
	}

	// Token: 0x060008EE RID: 2286 RVA: 0x00037B54 File Offset: 0x00035D54
	[Rpc(SendTo.Server)]
	public void Client_ChatMessageRpc(string message, bool useTeamChat, bool isMuted, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 4195669213U;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			bool flag = message != null;
			fastBufferWriter.WriteValueSafe<bool>(flag, default(FastBufferWriter.ForPrimitives));
			if (flag)
			{
				fastBufferWriter.WriteValueSafe(message, false);
			}
			fastBufferWriter.WriteValueSafe<bool>(useTeamChat, default(FastBufferWriter.ForPrimitives));
			fastBufferWriter.WriteValueSafe<bool>(isMuted, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 4195669213U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		ulong senderClientId = rpcParams.Receive.SenderClientId;
		Player component = NetworkManager.Singleton.ConnectedClients[senderClientId].PlayerObject.GetComponent<Player>();
		this.Server_ProcessPlayerChatMessage(component, message, senderClientId, useTeamChat, isMuted);
	}

	// Token: 0x060008EF RID: 2287 RVA: 0x00037CB4 File Offset: 0x00035EB4
	public void OnQuickChat(int index)
	{
		if (!this.IsQuickChatOpen)
		{
			this.OpenQuickChat(index);
			return;
		}
		if (this.quickChatIndex < 0 || this.quickChatIndex >= this.quickChatMessages.Count)
		{
			return;
		}
		if (index < 0 || index >= this.quickChatMessages[this.quickChatIndex].Length)
		{
			return;
		}
		string message = this.quickChatMessages[this.quickChatIndex][index];
		this.Client_SendClientChatMessage(message, false);
		this.CloseQuickChat();
	}

	// Token: 0x060008F0 RID: 2288 RVA: 0x00037D2C File Offset: 0x00035F2C
	public void OpenQuickChat(int index)
	{
		if (this.IsQuickChatOpen)
		{
			return;
		}
		if (index < 0 || index >= this.quickChatMessages.Count)
		{
			return;
		}
		if (this.quickChatMessages[index].Length == 0)
		{
			return;
		}
		this.quickChatIndex = index;
		this.quickChatContainer.style.display = DisplayStyle.Flex;
		this.quickChatContainer.Clear();
		int num = 1;
		foreach (string arg in this.quickChatMessages[index])
		{
			Label label = Utils.InstantiateVisualTreeAsset(this.chatMessageAsset, Position.Absolute).Query("ChatMessageLabel", null);
			label.text = string.Format("<b>{0}</b> <indent=5%>{1}", num, arg);
			this.quickChatContainer.Add(label);
			num++;
		}
		this.IsQuickChatOpen = true;
		Tween tween = this.quickChatTimeoutTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.quickChatTimeoutTween = DOVirtual.DelayedCall(this.quickChatTimeout, delegate
		{
			this.CloseQuickChat();
		}, true);
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x0000C88C File Offset: 0x0000AA8C
	public void CloseQuickChat()
	{
		if (!this.IsQuickChatOpen)
		{
			return;
		}
		this.quickChatContainer.style.display = DisplayStyle.None;
		this.quickChatIndex = -1;
		this.quickChatContainer.Clear();
		this.IsQuickChatOpen = false;
	}

	// Token: 0x060008FA RID: 2298 RVA: 0x00037ED4 File Offset: 0x000360D4
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x00037EEC File Offset: 0x000360EC
	protected override void __initializeRpcs()
	{
		base.__registerRpc(3914580275U, new NetworkBehaviour.RpcReceiveHandler(UIChat.__rpc_handler_3914580275), "Server_ChatMessageRpc");
		base.__registerRpc(4195669213U, new NetworkBehaviour.RpcReceiveHandler(UIChat.__rpc_handler_4195669213), "Client_ChatMessageRpc");
		base.__initializeRpcs();
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x00037F3C File Offset: 0x0003613C
	private static void __rpc_handler_3914580275(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool flag;
		reader.ReadValueSafe<bool>(out flag, default(FastBufferWriter.ForPrimitives));
		string message = null;
		if (flag)
		{
			reader.ReadValueSafe(out message, false);
		}
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((UIChat)target).Server_ChatMessageRpc(message, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x00037FD8 File Offset: 0x000361D8
	private static void __rpc_handler_4195669213(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool flag;
		reader.ReadValueSafe<bool>(out flag, default(FastBufferWriter.ForPrimitives));
		string message = null;
		if (flag)
		{
			reader.ReadValueSafe(out message, false);
		}
		bool useTeamChat;
		reader.ReadValueSafe<bool>(out useTeamChat, default(FastBufferWriter.ForPrimitives));
		bool isMuted;
		reader.ReadValueSafe<bool>(out isMuted, default(FastBufferWriter.ForPrimitives));
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((UIChat)target).Client_ChatMessageRpc(message, useTeamChat, isMuted, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x0000C92F File Offset: 0x0000AB2F
	protected internal override string __getTypeName()
	{
		return "UIChat";
	}

	// Token: 0x0400056D RID: 1389
	[Header("Components")]
	[SerializeField]
	private VisualTreeAsset chatMessageAsset;

	// Token: 0x0400056E RID: 1390
	[SerializeField]
	private VisualTreeAsset quickChatMessageAsset;

	// Token: 0x0400056F RID: 1391
	[Header("Settings")]
	[SerializeField]
	private float messageRateLimit = 3f;

	// Token: 0x04000570 RID: 1392
	[SerializeField]
	private float messageRatePeriod = 5f;

	// Token: 0x04000571 RID: 1393
	[SerializeField]
	private int messageHistory = 50;

	// Token: 0x04000572 RID: 1394
	[SerializeField]
	private Color teamBlueColor = Color.blue;

	// Token: 0x04000573 RID: 1395
	[SerializeField]
	private Color teamRedColor = Color.red;

	// Token: 0x04000574 RID: 1396
	[SerializeField]
	private Color teamSpectatorColor = Color.gray;

	// Token: 0x04000575 RID: 1397
	[SerializeField]
	private Vector2 size = new Vector2(700f, 250f);

	// Token: 0x04000576 RID: 1398
	[SerializeField]
	private float fontSize = 24f;

	// Token: 0x04000577 RID: 1399
	[SerializeField]
	private SerializedDictionary<int, string[]> quickChatMessages = new SerializedDictionary<int, string[]>();

	// Token: 0x04000578 RID: 1400
	[SerializeField]
	private float quickChatTimeout = 3f;

	// Token: 0x04000579 RID: 1401
	[HideInInspector]
	public bool UseTeamChat;

	// Token: 0x0400057A RID: 1402
	[HideInInspector]
	public bool IsQuickChatOpen;

	// Token: 0x0400057B RID: 1403
	private ScrollView chatScrollView;

	// Token: 0x0400057C RID: 1404
	private TextField chatTextField;

	// Token: 0x0400057D RID: 1405
	private VisualElement quickChatContainer;

	// Token: 0x0400057E RID: 1406
	private Dictionary<VisualElement, ChatMessage> chatMessages = new Dictionary<VisualElement, ChatMessage>();

	// Token: 0x0400057F RID: 1407
	private Dictionary<Player, float> playerRateLimits = new Dictionary<Player, float>();

	// Token: 0x04000580 RID: 1408
	private Tween scrollToBottomTween;

	// Token: 0x04000581 RID: 1409
	private Tween quickChatTimeoutTween;

	// Token: 0x04000582 RID: 1410
	private int quickChatIndex = -1;
}
