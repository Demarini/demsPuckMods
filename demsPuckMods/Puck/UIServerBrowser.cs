using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000130 RID: 304
public class UIServerBrowser : UIComponent<UIServerBrowser>
{
	// Token: 0x06000AB1 RID: 2737 RVA: 0x0000DC7E File Offset: 0x0000BE7E
	private void Start()
	{
		base.VisibilityRequiresMouse = true;
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x0003D9CC File Offset: 0x0003BBCC
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("ServerBrowserContainer", null);
		this.serverContainer = this.container.Query("ServerContainer", null);
		this.nameHeaderButton = this.container.Query("NameHeaderButton", null);
		this.nameHeaderButton.clicked += this.OnClickNameHeader;
		this.playersHeaderButton = this.container.Query("PlayersHeaderButton", null);
		this.playersHeaderButton.clicked += this.OnClickPlayersHeader;
		this.pingHeaderButton = this.container.Query("PingHeaderButton", null);
		this.pingHeaderButton.clicked += this.OnClickPingHeader;
		this.searchTextField = this.container.Query("SearchTextField", null).First().Query("TextField", null);
		this.searchTextField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnSearchChanged));
		this.searchTextField.value = this.searchNeedle;
		this.maximumPingIntegerField = this.container.Query("MaximumPingIntegerField", null).First().Query("IntegerField", null);
		this.maximumPingIntegerField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<int>>(this.OnMaximumPingChanged));
		this.maximumPingIntegerField.value = this.maximumPing;
		this.showFullToggle = this.container.Query("ShowFullToggle", null).First().Query("Toggle", null);
		this.showFullToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnShowFullChanged));
		this.showFullToggle.value = this.showFull;
		this.showEmptyToggle = this.container.Query("ShowEmptyToggle", null).First().Query("Toggle", null);
		this.showEmptyToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnShowEmptyChanged));
		this.showEmptyToggle.value = this.showEmpty;
		this.closeButton = this.container.Query("CloseButton", null);
		this.closeButton.clicked += this.OnClickClose;
		this.refreshButton = this.container.Query("RefreshButton", null);
		this.refreshButton.clicked += this.OnClickRefresh;
		this.serverLauncherButton = this.container.Query("ServerLauncherButton", null);
		this.serverLauncherButton.clicked += this.OnClickServerLauncher;
		this.ClearServers();
		this.OnClickPlayersHeader();
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x0000DC87 File Offset: 0x0000BE87
	public override void Show()
	{
		if (!base.IsVisible && this.servers.Count == 0)
		{
			this.Refresh();
		}
		base.Show();
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x0000DCAA File Offset: 0x0000BEAA
	public void Refresh()
	{
		this.DisableRefreshButton();
		MonoBehaviourSingleton<WebSocketManager>.Instance.Emit("playerGetServerBrowserServersRequest", null, "playerGetServerBrowserServersResponse");
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x0003DCAC File Offset: 0x0003BEAC
	private void ClearSortTypes()
	{
		this.nameSortType = SortType.None;
		this.playersSortType = SortType.None;
		this.pingSortType = SortType.None;
		this.nameHeaderButton.text = "NAME";
		this.playersHeaderButton.text = "PLAYERS";
		this.pingHeaderButton.text = "PING";
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x0003DD00 File Offset: 0x0003BF00
	private void OnClickNameHeader()
	{
		if (this.nameSortType == SortType.None)
		{
			this.ClearSortTypes();
		}
		this.nameSortType = ((this.nameSortType == SortType.Down) ? SortType.Up : SortType.Down);
		this.nameHeaderButton.text = ((this.nameSortType == SortType.Up) ? "▲ NAME" : "▼ NAME");
		this.SortServers();
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x0003DD54 File Offset: 0x0003BF54
	private void OnClickPlayersHeader()
	{
		if (this.playersSortType == SortType.None)
		{
			this.ClearSortTypes();
		}
		this.playersSortType = ((this.playersSortType == SortType.Down) ? SortType.Up : SortType.Down);
		this.playersHeaderButton.text = ((this.playersSortType == SortType.Up) ? "▲ PLAYERS" : "▼ PLAYERS");
		this.SortServers();
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x0003DDA8 File Offset: 0x0003BFA8
	private void OnClickPingHeader()
	{
		if (this.pingSortType == SortType.None)
		{
			this.ClearSortTypes();
		}
		this.pingSortType = ((this.pingSortType == SortType.Down) ? SortType.Up : SortType.Down);
		this.pingHeaderButton.text = ((this.pingSortType == SortType.Up) ? "▲ PING" : "▼ PING");
		this.SortServers();
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x0000DCC7 File Offset: 0x0000BEC7
	private void OnSearchChanged(ChangeEvent<string> changeEvent)
	{
		this.searchNeedle = changeEvent.newValue;
		this.SortServers();
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x0000DCDB File Offset: 0x0000BEDB
	private void OnMaximumPingChanged(ChangeEvent<int> changeEvent)
	{
		this.maximumPing = changeEvent.newValue;
		this.SortServers();
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x0000DCEF File Offset: 0x0000BEEF
	private void OnShowFullChanged(ChangeEvent<bool> changeEvent)
	{
		this.showFull = changeEvent.newValue;
		this.SortServers();
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x0000DD03 File Offset: 0x0000BF03
	private void OnShowEmptyChanged(ChangeEvent<bool> changeEvent)
	{
		this.showEmpty = changeEvent.newValue;
		this.SortServers();
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x0000DD17 File Offset: 0x0000BF17
	private void OnClickClose()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnServerBrowserClickClose", null);
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x0000DD29 File Offset: 0x0000BF29
	private void OnClickRefresh()
	{
		this.Refresh();
	}

	// Token: 0x06000ABF RID: 2751 RVA: 0x0000DD31 File Offset: 0x0000BF31
	private void OnClickServerLauncher()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnServerBrowserClickServerLauncher", null);
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x0000DD43 File Offset: 0x0000BF43
	public void EnableRefreshButton()
	{
		this.refreshButton.enabledSelf = true;
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x0000DD51 File Offset: 0x0000BF51
	public void DisableRefreshButton()
	{
		this.refreshButton.enabledSelf = false;
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x0000DD5F File Offset: 0x0000BF5F
	public void ClearServers()
	{
		this.servers.Clear();
		this.serverButtons.Clear();
		this.serverContainer.Clear();
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x0003DDFC File Offset: 0x0003BFFC
	public void UpdateServers(List<ServerBrowserServer> serverBrowserServers)
	{
		UIServerBrowser.<>c__DisplayClass39_0 CS$<>8__locals1 = new UIServerBrowser.<>c__DisplayClass39_0();
		CS$<>8__locals1.<>4__this = this;
		this.servers = serverBrowserServers;
		CS$<>8__locals1.serverPingSentMap = new Dictionary<ServerBrowserServer, long>();
		ushort port2 = 8886;
		CS$<>8__locals1.retries = 0;
		CS$<>8__locals1.maxRetries = 3;
		CS$<>8__locals1.udpSocket = new UDPSocket();
		UDPSocket udpSocket = CS$<>8__locals1.udpSocket;
		udpSocket.OnSocketStarted = (Action<ushort>)Delegate.Combine(udpSocket.OnSocketStarted, new Action<ushort>(delegate(ushort port)
		{
			UIServerBrowser.<>c__DisplayClass39_0.<<UpdateServers>b__0>d <<UpdateServers>b__0>d;
			<<UpdateServers>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
			<<UpdateServers>b__0>d.<>4__this = CS$<>8__locals1;
			<<UpdateServers>b__0>d.port = port;
			<<UpdateServers>b__0>d.<>1__state = -1;
			<<UpdateServers>b__0>d.<>t__builder.Start<UIServerBrowser.<>c__DisplayClass39_0.<<UpdateServers>b__0>d>(ref <<UpdateServers>b__0>d);
		}));
		UDPSocket udpSocket2 = CS$<>8__locals1.udpSocket;
		udpSocket2.OnSocketStopped = (Action)Delegate.Combine(udpSocket2.OnSocketStopped, new Action(delegate()
		{
			Debug.Log("UDP socket stopped");
			CS$<>8__locals1.<>4__this.EnableRefreshButton();
		}));
		UDPSocket udpSocket3 = CS$<>8__locals1.udpSocket;
		udpSocket3.OnSocketFailed = (Action<ushort>)Delegate.Combine(udpSocket3.OnSocketFailed, new Action<ushort>(delegate(ushort port)
		{
			Debug.Log(string.Format("[UIServerBrowser] UDP socket failed on port {0}", port));
			CS$<>8__locals1.<>4__this.EnableRefreshButton();
			if (CS$<>8__locals1.retries >= CS$<>8__locals1.maxRetries)
			{
				return;
			}
			int retries = CS$<>8__locals1.retries;
			CS$<>8__locals1.retries = retries + 1;
			port += 1;
			CS$<>8__locals1.udpSocket.StartSocket(port);
		}));
		UDPSocket udpSocket4 = CS$<>8__locals1.udpSocket;
		udpSocket4.OnUdpMessageSent = (Action<string, ushort, string, long>)Delegate.Combine(udpSocket4.OnUdpMessageSent, new Action<string, ushort, string, long>(delegate(string ipAddress, ushort port, string message, long timestamp)
		{
			ServerBrowserServer key = CS$<>8__locals1.<>4__this.servers.Find((ServerBrowserServer server) => server.ipAddress == ipAddress && server.pingPort == port);
			if (!CS$<>8__locals1.serverPingSentMap.ContainsKey(key))
			{
				CS$<>8__locals1.serverPingSentMap.Add(key, timestamp);
			}
		}));
		UDPSocket udpSocket5 = CS$<>8__locals1.udpSocket;
		udpSocket5.OnUdpMessageReceived = (Action<string, ushort, string, long>)Delegate.Combine(udpSocket5.OnUdpMessageReceived, new Action<string, ushort, string, long>(delegate(string ipAddress, ushort port, string message, long timestamp)
		{
			ServerBrowserServer serverBrowserServer = CS$<>8__locals1.serverPingSentMap.Keys.FirstOrDefault((ServerBrowserServer server) => server.ipAddress == ipAddress && server.pingPort == port);
			if (serverBrowserServer == null)
			{
				return;
			}
			long num = CS$<>8__locals1.serverPingSentMap[serverBrowserServer];
			if (CS$<>8__locals1.serverPingSentMap.ContainsKey(serverBrowserServer))
			{
				CS$<>8__locals1.serverPingSentMap.Remove(serverBrowserServer);
			}
			CS$<>8__locals1.<>4__this.AddServerButton(serverBrowserServer, timestamp - num);
		}));
		CS$<>8__locals1.udpSocket.StartSocket(port2);
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x0003DF18 File Offset: 0x0003C118
	private void AddServerButton(ServerBrowserServer server, long ping)
	{
		TemplateContainer templateContainer = Utils.InstantiateVisualTreeAsset(this.serverButtonAsset, Position.Relative);
		Dictionary<string, object> userData = new Dictionary<string, object>
		{
			{
				"server",
				server
			},
			{
				"ping",
				ping
			}
		};
		templateContainer.userData = userData;
		Button button = templateContainer.Query("ServerButton", null);
		VisualElement visualElement = button.Query("PasswordProtectedVisualElement", null).First();
		Label label = button.Query("NameLabel", null);
		Label label2 = button.Query("PlayersLabel", null);
		TextElement textElement = button.Query("PingLabel", null);
		visualElement.visible = server.isPasswordProtected;
		label.text = server.name;
		label2.text = string.Format("{0}/{1}", server.players, server.maxPlayers);
		textElement.text = ((ping == long.MaxValue) ? "<color=red>?</color>" : string.Format("{0}ms", ping));
		button.RegisterCallback<ClickEvent>(delegate(ClickEvent e)
		{
			this.OnClickServer(server);
		}, TrickleDown.NoTrickleDown);
		this.serverContainer.Add(templateContainer);
		this.serverButtons.Add(templateContainer);
		this.SortServers();
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x0003E080 File Offset: 0x0003C280
	private void SortServers()
	{
		this.serverButtons.Sort(delegate(TemplateContainer a, TemplateContainer b)
		{
			Dictionary<string, object> dictionary2 = a.userData as Dictionary<string, object>;
			Dictionary<string, object> dictionary3 = b.userData as Dictionary<string, object>;
			ServerBrowserServer serverBrowserServer2 = dictionary2["server"] as ServerBrowserServer;
			ServerBrowserServer serverBrowserServer3 = dictionary3["server"] as ServerBrowserServer;
			long value = (long)dictionary2["ping"];
			long value2 = (long)dictionary3["ping"];
			if (this.nameSortType == SortType.Down)
			{
				return serverBrowserServer2.name.CompareTo(serverBrowserServer3.name);
			}
			if (this.nameSortType == SortType.Up)
			{
				return serverBrowserServer3.name.CompareTo(serverBrowserServer2.name);
			}
			if (this.playersSortType == SortType.Up)
			{
				int num2 = serverBrowserServer2.players.CompareTo(serverBrowserServer3.players);
				if (num2 != 0)
				{
					return num2;
				}
				return serverBrowserServer2.name.CompareTo(serverBrowserServer3.name);
			}
			else if (this.playersSortType == SortType.Down)
			{
				int num3 = serverBrowserServer3.players.CompareTo(serverBrowserServer2.players);
				if (num3 != 0)
				{
					return num3;
				}
				return serverBrowserServer2.name.CompareTo(serverBrowserServer3.name);
			}
			else if (this.pingSortType == SortType.Down)
			{
				int num4 = value.CompareTo(value2);
				if (num4 != 0)
				{
					return num4;
				}
				return serverBrowserServer2.name.CompareTo(serverBrowserServer3.name);
			}
			else
			{
				if (this.pingSortType != SortType.Up)
				{
					return 0;
				}
				int num5 = value2.CompareTo(value);
				if (num5 != 0)
				{
					return num5;
				}
				return serverBrowserServer2.name.CompareTo(serverBrowserServer3.name);
			}
		});
		this.serverContainer.Clear();
		foreach (TemplateContainer templateContainer in this.serverButtons)
		{
			Dictionary<string, object> dictionary = templateContainer.userData as Dictionary<string, object>;
			ServerBrowserServer serverBrowserServer = dictionary["server"] as ServerBrowserServer;
			long num = (long)dictionary["ping"];
			if ((string.IsNullOrEmpty(this.searchNeedle) || serverBrowserServer.name.ToLower().Contains(this.searchNeedle.ToLower())) && (num <= (long)this.maximumPing || this.maximumPing == 0) && (serverBrowserServer.players > 0 || this.showEmpty) && (serverBrowserServer.players < serverBrowserServer.maxPlayers || this.showFull))
			{
				templateContainer.style.display = DisplayStyle.Flex;
			}
			else
			{
				templateContainer.style.display = DisplayStyle.None;
			}
			this.serverContainer.Add(templateContainer);
		}
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x0000DD82 File Offset: 0x0000BF82
	private void OnClickServer(ServerBrowserServer serverBrowserServer)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnServerBrowserClickServer", new Dictionary<string, object>
		{
			{
				"serverBrowserServer",
				serverBrowserServer
			}
		});
	}

	// Token: 0x06000AC9 RID: 2761 RVA: 0x0003E320 File Offset: 0x0003C520
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000ACA RID: 2762 RVA: 0x0000DDD8 File Offset: 0x0000BFD8
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000ACB RID: 2763 RVA: 0x0000DDE2 File Offset: 0x0000BFE2
	protected internal override string __getTypeName()
	{
		return "UIServerBrowser";
	}

	// Token: 0x04000634 RID: 1588
	[Header("Components")]
	public VisualTreeAsset serverButtonAsset;

	// Token: 0x04000635 RID: 1589
	private VisualElement serverContainer;

	// Token: 0x04000636 RID: 1590
	private Button nameHeaderButton;

	// Token: 0x04000637 RID: 1591
	private Button playersHeaderButton;

	// Token: 0x04000638 RID: 1592
	private Button pingHeaderButton;

	// Token: 0x04000639 RID: 1593
	private TextField searchTextField;

	// Token: 0x0400063A RID: 1594
	private IntegerField maximumPingIntegerField;

	// Token: 0x0400063B RID: 1595
	private Toggle showFullToggle;

	// Token: 0x0400063C RID: 1596
	private Toggle showEmptyToggle;

	// Token: 0x0400063D RID: 1597
	private Button closeButton;

	// Token: 0x0400063E RID: 1598
	private Button refreshButton;

	// Token: 0x0400063F RID: 1599
	private Button serverLauncherButton;

	// Token: 0x04000640 RID: 1600
	private SortType nameSortType;

	// Token: 0x04000641 RID: 1601
	private SortType playersSortType;

	// Token: 0x04000642 RID: 1602
	private SortType pingSortType;

	// Token: 0x04000643 RID: 1603
	private string searchNeedle;

	// Token: 0x04000644 RID: 1604
	private int maximumPing = 100;

	// Token: 0x04000645 RID: 1605
	private bool showFull = true;

	// Token: 0x04000646 RID: 1606
	private bool showEmpty = true;

	// Token: 0x04000647 RID: 1607
	private List<ServerBrowserServer> servers = new List<ServerBrowserServer>();

	// Token: 0x04000648 RID: 1608
	private List<TemplateContainer> serverButtons = new List<TemplateContainer>();
}
