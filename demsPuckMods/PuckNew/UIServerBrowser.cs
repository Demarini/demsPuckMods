using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x020001C4 RID: 452
public class UIServerBrowser : UIView
{
	// Token: 0x1700011C RID: 284
	// (get) Token: 0x06000CF1 RID: 3313 RVA: 0x0003D05A File Offset: 0x0003B25A
	public int ServerCount
	{
		get
		{
			return this.endPointVisualElementMap.Count;
		}
	}

	// Token: 0x06000CF2 RID: 3314 RVA: 0x0003D068 File Offset: 0x0003B268
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("ServerBrowserView", null);
		this.serverBrowser = base.View.Query("ServerBrowser", null);
		this.serverList = this.serverBrowser.Query("ServerList", null);
		this.filters = base.View.Query("Filters", null);
		this.closeIconButton = this.serverBrowser.Query("CloseIconButtonContainer", null).First().Query(null, null);
		this.closeIconButton.clicked += this.OnServerBrowserClickClose;
		this.nameButton = this.serverBrowser.Query("NameButton", null);
		this.nameButton.clicked += this.OnClickNameSort;
		this.playersButton = this.serverBrowser.Query("PlayersButton", null);
		this.playersButton.clicked += this.OnClickPlayersSort;
		this.pingButton = this.serverBrowser.Query("PingButton", null);
		this.pingButton.clicked += this.OnClickPingSort;
		this.refreshButton = this.serverBrowser.Query("RefreshButton", null);
		this.refreshButton.clicked += this.OnClickRefresh;
		this.newServerButton = this.serverBrowser.Query("NewServerButton", null);
		this.newServerButton.clicked += this.OnClickNewServer;
		this.filtersButton = this.serverBrowser.Query("FiltersButton", null);
		this.filtersButton.clicked += this.OnClickFilters;
		this.filtersCloseIconButton = this.filters.Query("CloseIconButtonContainer", null).First().Query(null, null);
		this.filtersCloseIconButton.clicked += this.OnFiltersClickClose;
		this.searchTextField = this.filters.Query("SearchTextField", null).First().Query(null, null);
		this.searchTextField.value = string.Empty;
		this.searchTextField.RegisterCallback<ChangeEvent<string>>(new EventCallback<ChangeEvent<string>>(this.OnChangeSearchTextField), TrickleDown.NoTrickleDown);
		this.maxPingTextField = this.filters.Query("MaxPingIntegerField", null).First().Query(null, null);
		this.maxPingTextField.value = 100;
		this.maxPingTextField.RegisterCallback<ChangeEvent<int>>(new EventCallback<ChangeEvent<int>>(this.OnChangeMaxPingTextField), TrickleDown.NoTrickleDown);
		this.showFullToggle = this.filters.Query("ShowFullToggle", null).First().Query(null, null);
		this.showFullToggle.value = true;
		this.showFullToggle.RegisterCallback<ChangeEvent<bool>>(new EventCallback<ChangeEvent<bool>>(this.OnChangeShowFullToggle), TrickleDown.NoTrickleDown);
		this.showEmptyToggle = this.filters.Query("ShowEmptyToggle", null).First().Query(null, null);
		this.showEmptyToggle.value = true;
		this.showEmptyToggle.RegisterCallback<ChangeEvent<bool>>(new EventCallback<ChangeEvent<bool>>(this.OnChangeShowEmptyToggle), TrickleDown.NoTrickleDown);
		this.showPasswordProtectedToggle = this.filters.Query("ShowPasswordProtectedToggle", null).First().Query(null, null);
		this.showPasswordProtectedToggle.value = true;
		this.showPasswordProtectedToggle.RegisterCallback<ChangeEvent<bool>>(new EventCallback<ChangeEvent<bool>>(this.OnChangeShowPasswordProtectedToggle), TrickleDown.NoTrickleDown);
		this.showModdedToggle = this.filters.Query("ShowModdedToggle", null).First().Query(null, null);
		this.showModdedToggle.value = true;
		this.showModdedToggle.RegisterCallback<ChangeEvent<bool>>(new EventCallback<ChangeEvent<bool>>(this.OnChangeShowModdedToggle), TrickleDown.NoTrickleDown);
		this.showUnreachableToggle = this.filters.Query("ShowUnreachableToggle", null).First().Query(null, null);
		this.showUnreachableToggle.value = false;
		this.showUnreachableToggle.RegisterCallback<ChangeEvent<bool>>(new EventCallback<ChangeEvent<bool>>(this.OnChangeShowUnreachableToggle), TrickleDown.NoTrickleDown);
		this.serverList.Clear();
		this.StyleSortButtons();
	}

	// Token: 0x06000CF3 RID: 3315 RVA: 0x0003D4D5 File Offset: 0x0003B6D5
	public override bool Show()
	{
		bool flag = base.Show();
		if (flag)
		{
			EventManager.TriggerEvent("Event_OnServerBrowserShow", null);
		}
		return flag;
	}

	// Token: 0x06000CF4 RID: 3316 RVA: 0x0003D4EB File Offset: 0x0003B6EB
	public void Refresh()
	{
		WebSocketManager.Emit("playerGetServerBrowserEndPointsRequest", null, "playerGetServerBrowserEndPointsResponse");
	}

	// Token: 0x06000CF5 RID: 3317 RVA: 0x0003D500 File Offset: 0x0003B700
	public void UpdateEndPoints(EndPoint[] endPoints)
	{
		UIServerBrowser.<>c__DisplayClass27_0 CS$<>8__locals1 = new UIServerBrowser.<>c__DisplayClass27_0();
		CS$<>8__locals1.endPoints = endPoints;
		CS$<>8__locals1.<>4__this = this;
		this.RemoveAllServers();
		this.refreshButton.SetEnabled(false);
		foreach (EndPoint endPoint in CS$<>8__locals1.endPoints)
		{
			this.AddServer(endPoint);
		}
		this.FilterServers();
		this.SortServers();
		Task.Run(delegate()
		{
			UIServerBrowser.<>c__DisplayClass27_0.<<UpdateEndPoints>b__0>d <<UpdateEndPoints>b__0>d;
			<<UpdateEndPoints>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
			<<UpdateEndPoints>b__0>d.<>4__this = CS$<>8__locals1;
			<<UpdateEndPoints>b__0>d.<>1__state = -1;
			<<UpdateEndPoints>b__0>d.<>t__builder.Start<UIServerBrowser.<>c__DisplayClass27_0.<<UpdateEndPoints>b__0>d>(ref <<UpdateEndPoints>b__0>d);
			return <<UpdateEndPoints>b__0>d.<>t__builder.Task;
		});
	}

	// Token: 0x06000CF6 RID: 3318 RVA: 0x0003D574 File Offset: 0x0003B774
	private void AddServer(EndPoint endPoint)
	{
		if (this.endPointVisualElementMap.ContainsKey(endPoint))
		{
			return;
		}
		VisualElement visualElement = this.serverAsset.Instantiate();
		visualElement.userData = new Dictionary<string, object>();
		VisualElement visualElement2 = visualElement.Query("Server", null);
		visualElement2.userData = new Dictionary<string, object>();
		visualElement2.Query(null, null).RegisterCallback<ClickEvent, EndPoint>(new EventCallback<ClickEvent, EndPoint>(this.OnClickServer), endPoint, TrickleDown.NoTrickleDown);
		this.endPointVisualElementMap.Add(endPoint, visualElement);
		this.serverList.Add(visualElement);
		this.StyleServer(endPoint);
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x0003D604 File Offset: 0x0003B804
	private EndPoint GetServerEndPoint(VisualElement visualElement)
	{
		if (!this.endPointVisualElementMap.ContainsValue(visualElement))
		{
			return null;
		}
		return this.endPointVisualElementMap.FirstOrDefault((KeyValuePair<EndPoint, VisualElement> x) => x.Value == visualElement).Key;
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x0003D654 File Offset: 0x0003B854
	private void SetServerPreviewData(EndPoint endPoint, ServerPreviewData previewData)
	{
		if (!this.endPointVisualElementMap.ContainsKey(endPoint))
		{
			return;
		}
		(this.endPointVisualElementMap[endPoint].Query("Server", null).userData as Dictionary<string, object>)["previewData"] = previewData;
	}

	// Token: 0x06000CF9 RID: 3321 RVA: 0x0003D6A4 File Offset: 0x0003B8A4
	private ServerPreviewData GetServerPreviewData(EndPoint endPoint)
	{
		if (!this.endPointVisualElementMap.ContainsKey(endPoint))
		{
			return null;
		}
		return (this.endPointVisualElementMap[endPoint].Query("Server", null).userData as Dictionary<string, object>).GetValueOrDefault("previewData", null) as ServerPreviewData;
	}

	// Token: 0x06000CFA RID: 3322 RVA: 0x0003D6F8 File Offset: 0x0003B8F8
	private void StyleServer(EndPoint endPoint)
	{
		if (!this.endPointVisualElementMap.ContainsKey(endPoint))
		{
			return;
		}
		VisualElement visualElement = this.endPointVisualElementMap[endPoint].Query("Server", null);
		ServerPreviewData serverPreviewData = this.GetServerPreviewData(endPoint);
		Label label = visualElement.Query("NameLabel", null);
		Label label2 = visualElement.Query("PlayersLabel", null);
		Label label3 = visualElement.Query("PingLabel", null);
		if (serverPreviewData == null)
		{
			visualElement.EnableInClassList("passwordProtected", false);
			visualElement.EnableInClassList("modded", false);
			visualElement.EnableInClassList("unreachable", true);
			label.text = endPoint.ToString();
			label2.text = "?";
			label3.text = "?";
			return;
		}
		visualElement.EnableInClassList("passwordProtected", serverPreviewData.isPasswordProtected);
		visualElement.EnableInClassList("modded", serverPreviewData.clientRequiredModIds.Length != 0);
		visualElement.EnableInClassList("unreachable", false);
		label.text = serverPreviewData.name;
		label2.text = string.Format("{0}/{1}", serverPreviewData.players, serverPreviewData.maxPlayers);
		label3.text = string.Format("{0}ms", serverPreviewData.ping);
	}

	// Token: 0x06000CFB RID: 3323 RVA: 0x0003D83C File Offset: 0x0003BA3C
	private void RemoveServer(EndPoint endPoint)
	{
		if (!this.endPointVisualElementMap.ContainsKey(endPoint))
		{
			return;
		}
		this.endPointVisualElementMap[endPoint].Query("Server", null).Query(null, null).UnregisterCallback<ClickEvent, EndPoint>(new EventCallback<ClickEvent, EndPoint>(this.OnClickServer), TrickleDown.NoTrickleDown);
		this.serverList.Remove(this.endPointVisualElementMap[endPoint]);
		this.endPointVisualElementMap.Remove(endPoint);
	}

	// Token: 0x06000CFC RID: 3324 RVA: 0x0003D8B8 File Offset: 0x0003BAB8
	private void RemoveAllServers()
	{
		foreach (EndPoint endPoint in this.endPointVisualElementMap.Keys.ToList<EndPoint>())
		{
			this.RemoveServer(endPoint);
		}
	}

	// Token: 0x06000CFD RID: 3325 RVA: 0x0003D918 File Offset: 0x0003BB18
	private void StyleSortButtons()
	{
		this.nameButton.text = ((this.sortType == ServerSortType.Name) ? ((this.sortDirection == ServerSortDirection.Ascending) ? "▼ NAME" : "▲ NAME") : "NAME");
		this.playersButton.text = ((this.sortType == ServerSortType.Players) ? ((this.sortDirection == ServerSortDirection.Ascending) ? "▼ PLAYERS" : "▲ PLAYERS") : "PLAYERS");
		this.pingButton.text = ((this.sortType == ServerSortType.Ping) ? ((this.sortDirection == ServerSortDirection.Ascending) ? "▼ PING" : "▲ PING") : "PING");
	}

	// Token: 0x06000CFE RID: 3326 RVA: 0x0003D9B4 File Offset: 0x0003BBB4
	private void SortServers()
	{
		this.serverList.hierarchy.Sort(delegate(VisualElement a, VisualElement b)
		{
			EndPoint serverEndPoint = this.GetServerEndPoint(a);
			EndPoint serverEndPoint2 = this.GetServerEndPoint(b);
			ServerPreviewData serverPreviewData = this.GetServerPreviewData(serverEndPoint);
			ServerPreviewData serverPreviewData2 = this.GetServerPreviewData(serverEndPoint2);
			string text = (serverPreviewData != null) ? serverPreviewData.name : serverEndPoint.ToString();
			string strB = (serverPreviewData2 != null) ? serverPreviewData2.name : serverEndPoint2.ToString();
			int num = (serverPreviewData != null) ? serverPreviewData.players : 0;
			int value = (serverPreviewData2 != null) ? serverPreviewData2.players : 0;
			int num2 = (serverPreviewData != null) ? serverPreviewData.ping : int.MaxValue;
			int value2 = (serverPreviewData2 != null) ? serverPreviewData2.ping : int.MaxValue;
			int num3 = 0;
			switch (this.sortType)
			{
			case ServerSortType.Name:
				num3 = text.CompareTo(strB) * ((this.sortDirection == ServerSortDirection.Ascending) ? 1 : -1);
				break;
			case ServerSortType.Players:
				num3 = num.CompareTo(value) * ((this.sortDirection == ServerSortDirection.Ascending) ? 1 : -1);
				if (num3 == 0)
				{
					num3 = text.CompareTo(strB);
				}
				break;
			case ServerSortType.Ping:
				num3 = num2.CompareTo(value2) * ((this.sortDirection == ServerSortDirection.Ascending) ? 1 : -1);
				if (num3 == 0)
				{
					num3 = text.CompareTo(strB);
				}
				break;
			}
			return num3;
		});
	}

	// Token: 0x06000CFF RID: 3327 RVA: 0x0003D9E0 File Offset: 0x0003BBE0
	private void FilterServers()
	{
		foreach (EndPoint endPoint in this.endPointVisualElementMap.Keys)
		{
			this.FilterServer(endPoint);
		}
	}

	// Token: 0x06000D00 RID: 3328 RVA: 0x0003DA38 File Offset: 0x0003BC38
	private void FilterServer(EndPoint endPoint)
	{
		if (!this.endPointVisualElementMap.ContainsKey(endPoint))
		{
			return;
		}
		VisualElement visualElement = this.endPointVisualElementMap[endPoint];
		ServerPreviewData serverPreviewData = this.GetServerPreviewData(endPoint);
		bool flag;
		if (serverPreviewData == null)
		{
			string text = endPoint.ipAddress.ToLower();
			string value = string.IsNullOrEmpty(this.searchTextField.value) ? null : this.searchTextField.value.ToLower();
			flag = ((string.IsNullOrEmpty(value) || text.Contains(value)) && this.showUnreachableToggle.value);
		}
		else
		{
			string text2 = serverPreviewData.name.ToLower();
			string value2 = string.IsNullOrEmpty(this.searchTextField.value) ? null : this.searchTextField.value.ToLower();
			flag = ((string.IsNullOrEmpty(value2) || text2.Contains(value2)) && serverPreviewData.ping <= this.maxPingTextField.value && (serverPreviewData.players > 0 || this.showEmptyToggle.value) && (serverPreviewData.players < serverPreviewData.maxPlayers || this.showFullToggle.value) && (!serverPreviewData.isPasswordProtected || this.showPasswordProtectedToggle.value) && (serverPreviewData.clientRequiredModIds.Length == 0 || this.showModdedToggle.value));
		}
		visualElement.style.display = (flag ? DisplayStyle.Flex : DisplayStyle.None);
	}

	// Token: 0x06000D01 RID: 3329 RVA: 0x0003DB94 File Offset: 0x0003BD94
	public void ShowFilters()
	{
		this.filters.style.display = DisplayStyle.Flex;
	}

	// Token: 0x06000D02 RID: 3330 RVA: 0x0003DBAC File Offset: 0x0003BDAC
	public void HideFilters()
	{
		this.filters.style.display = DisplayStyle.None;
	}

	// Token: 0x06000D03 RID: 3331 RVA: 0x0003DBC4 File Offset: 0x0003BDC4
	private ServerPreviewData PingServer(EndPoint endPoint, int connectTimeout, int responseTimeout)
	{
		TCPClient tcpClient = new TCPClient(endPoint, connectTimeout, 1000);
		double pingTimestamp = 0.0;
		ServerPreviewData previewData = null;
		ManualResetEventSlim responseEvent = new ManualResetEventSlim(false);
		tcpClient.OnConnected += delegate()
		{
			string message = JsonSerializer.Serialize<TCPServerPreviewRequest>(new TCPServerPreviewRequest(), null);
			tcpClient.SendMessage(message);
		};
		tcpClient.OnMessageSent += delegate(string message)
		{
			try
			{
				if (JsonSerializer.Deserialize<TCPServerMessage>(message, null).type == TCPServerMessageType.PreviewRequest)
				{
					pingTimestamp = Utils.GetTimestamp();
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("[UIServerBrowser] Error parsing message sent to {0}: {1}", endPoint, ex.Message));
			}
		};
		tcpClient.OnMessageReceived += delegate(string message)
		{
			try
			{
				if (JsonSerializer.Deserialize<TCPServerMessage>(message, null).type == TCPServerMessageType.PreviewResponse)
				{
					TCPServerPreviewResponse tcpserverPreviewResponse = JsonSerializer.Deserialize<TCPServerPreviewResponse>(message, null);
					int ping = (int)(Utils.GetTimestamp() - pingTimestamp);
					previewData = new ServerPreviewData
					{
						name = tcpserverPreviewResponse.name,
						players = tcpserverPreviewResponse.players,
						maxPlayers = tcpserverPreviewResponse.maxPlayers,
						isPasswordProtected = tcpserverPreviewResponse.isPasswordProtected,
						clientRequiredModIds = tcpserverPreviewResponse.clientRequiredModIds,
						ping = ping
					};
					responseEvent.Set();
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("[UIServerBrowser] Error parsing message from {0}: {1}", endPoint, ex.Message));
			}
		};
		tcpClient.Connect();
		if (tcpClient.IsConnected)
		{
			responseEvent.Wait(responseTimeout);
			tcpClient.Disconnect();
		}
		return previewData;
	}

	// Token: 0x06000D04 RID: 3332 RVA: 0x0003DC92 File Offset: 0x0003BE92
	private void OnClickNameSort()
	{
		if (this.sortType == ServerSortType.Name)
		{
			this.sortDirection = ((this.sortDirection == ServerSortDirection.Ascending) ? ServerSortDirection.Descending : ServerSortDirection.Ascending);
		}
		else
		{
			this.sortType = ServerSortType.Name;
			this.sortDirection = ServerSortDirection.Ascending;
		}
		this.StyleSortButtons();
		this.SortServers();
	}

	// Token: 0x06000D05 RID: 3333 RVA: 0x0003DCCA File Offset: 0x0003BECA
	private void OnClickPlayersSort()
	{
		if (this.sortType == ServerSortType.Players)
		{
			this.sortDirection = ((this.sortDirection == ServerSortDirection.Ascending) ? ServerSortDirection.Descending : ServerSortDirection.Ascending);
		}
		else
		{
			this.sortType = ServerSortType.Players;
			this.sortDirection = ServerSortDirection.Descending;
		}
		this.StyleSortButtons();
		this.SortServers();
	}

	// Token: 0x06000D06 RID: 3334 RVA: 0x0003DD03 File Offset: 0x0003BF03
	private void OnClickPingSort()
	{
		if (this.sortType == ServerSortType.Ping)
		{
			this.sortDirection = ((this.sortDirection == ServerSortDirection.Ascending) ? ServerSortDirection.Descending : ServerSortDirection.Ascending);
		}
		else
		{
			this.sortType = ServerSortType.Ping;
			this.sortDirection = ServerSortDirection.Ascending;
		}
		this.StyleSortButtons();
		this.SortServers();
	}

	// Token: 0x06000D07 RID: 3335 RVA: 0x0003DD3C File Offset: 0x0003BF3C
	private void OnServerBrowserClickClose()
	{
		EventManager.TriggerEvent("Event_OnServerBrowserClickClose", null);
	}

	// Token: 0x06000D08 RID: 3336 RVA: 0x0003DD49 File Offset: 0x0003BF49
	private void OnClickRefresh()
	{
		EventManager.TriggerEvent("Event_OnServerBrowserClickRefresh", null);
	}

	// Token: 0x06000D09 RID: 3337 RVA: 0x0003DD56 File Offset: 0x0003BF56
	private void OnClickNewServer()
	{
		EventManager.TriggerEvent("Event_OnServerBrowserClickNewServer", null);
	}

	// Token: 0x06000D0A RID: 3338 RVA: 0x0003DD63 File Offset: 0x0003BF63
	private void OnClickFilters()
	{
		EventManager.TriggerEvent("Event_OnServerBrowserClickFilters", null);
	}

	// Token: 0x06000D0B RID: 3339 RVA: 0x0003DD70 File Offset: 0x0003BF70
	private void OnClickServer(ClickEvent e, EndPoint endPoint)
	{
		EventManager.TriggerEvent("Event_OnServerBrowserClickEndPoint", new Dictionary<string, object>
		{
			{
				"endPoint",
				endPoint
			}
		});
	}

	// Token: 0x06000D0C RID: 3340 RVA: 0x0003DD8D File Offset: 0x0003BF8D
	private void OnFiltersClickClose()
	{
		EventManager.TriggerEvent("Event_OnServerBrowserFiltersClickClose", null);
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x0003DD9A File Offset: 0x0003BF9A
	private void OnChangeSearchTextField(ChangeEvent<string> e)
	{
		this.FilterServers();
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x0003DD9A File Offset: 0x0003BF9A
	private void OnChangeMaxPingTextField(ChangeEvent<int> e)
	{
		this.FilterServers();
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x0003DD9A File Offset: 0x0003BF9A
	private void OnChangeShowFullToggle(ChangeEvent<bool> e)
	{
		this.FilterServers();
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x0003DD9A File Offset: 0x0003BF9A
	private void OnChangeShowEmptyToggle(ChangeEvent<bool> e)
	{
		this.FilterServers();
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x0003DD9A File Offset: 0x0003BF9A
	private void OnChangeShowPasswordProtectedToggle(ChangeEvent<bool> e)
	{
		this.FilterServers();
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x0003DD9A File Offset: 0x0003BF9A
	private void OnChangeShowModdedToggle(ChangeEvent<bool> e)
	{
		this.FilterServers();
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x0003DD9A File Offset: 0x0003BF9A
	private void OnChangeShowUnreachableToggle(ChangeEvent<bool> e)
	{
		this.FilterServers();
	}

	// Token: 0x040007A9 RID: 1961
	[Header("References")]
	[SerializeField]
	private VisualTreeAsset serverAsset;

	// Token: 0x040007AA RID: 1962
	private VisualElement serverBrowser;

	// Token: 0x040007AB RID: 1963
	private VisualElement filters;

	// Token: 0x040007AC RID: 1964
	private IconButton closeIconButton;

	// Token: 0x040007AD RID: 1965
	private VisualElement serverList;

	// Token: 0x040007AE RID: 1966
	private Button nameButton;

	// Token: 0x040007AF RID: 1967
	private Button playersButton;

	// Token: 0x040007B0 RID: 1968
	private Button pingButton;

	// Token: 0x040007B1 RID: 1969
	private Button refreshButton;

	// Token: 0x040007B2 RID: 1970
	private Button newServerButton;

	// Token: 0x040007B3 RID: 1971
	private Button filtersButton;

	// Token: 0x040007B4 RID: 1972
	private IconButton filtersCloseIconButton;

	// Token: 0x040007B5 RID: 1973
	private TextField searchTextField;

	// Token: 0x040007B6 RID: 1974
	private IntegerField maxPingTextField;

	// Token: 0x040007B7 RID: 1975
	private Toggle showFullToggle;

	// Token: 0x040007B8 RID: 1976
	private Toggle showEmptyToggle;

	// Token: 0x040007B9 RID: 1977
	private Toggle showPasswordProtectedToggle;

	// Token: 0x040007BA RID: 1978
	private Toggle showModdedToggle;

	// Token: 0x040007BB RID: 1979
	private Toggle showUnreachableToggle;

	// Token: 0x040007BC RID: 1980
	private Dictionary<EndPoint, VisualElement> endPointVisualElementMap = new Dictionary<EndPoint, VisualElement>();

	// Token: 0x040007BD RID: 1981
	private ServerSortType sortType;

	// Token: 0x040007BE RID: 1982
	private ServerSortDirection sortDirection;
}
