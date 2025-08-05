using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x0200012C RID: 300
public class UIScoreboard : UIComponent<UIScoreboard>
{
	// Token: 0x06000A8C RID: 2700 RVA: 0x0000DBC1 File Offset: 0x0000BDC1
	private void Start()
	{
		base.VisibilityRequiresMouse = true;
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x0003CF70 File Offset: 0x0003B170
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("ScoreboardContainer", null);
		this.teamBlueContainer = this.container.Query("TeamBlueContainer", null);
		this.teamRedContainer = this.container.Query("TeamRedContainer", null);
		this.teamSpectatorContainer = this.container.Query("TeamSpectatorContainer", null);
		this.teamBlueContainer.Clear();
		this.teamRedContainer.Clear();
		this.teamSpectatorContainer.Clear();
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x0003D00C File Offset: 0x0003B20C
	public void AddPlayer(Player player)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		if (this.playerVisualElementMap.ContainsKey(player))
		{
			return;
		}
		VisualElement visualElement = Utils.InstantiateVisualTreeAsset(this.scoreboardPlayerAsset, Position.Relative);
		this.teamSpectatorContainer.Add(visualElement);
		visualElement.Query("ScoreboardPlayerButton", null).RegisterCallback<ClickEvent, Player>(new EventCallback<ClickEvent, Player>(this.OnPlayerClicked), player, TrickleDown.NoTrickleDown);
		this.playerVisualElementMap.Add(player, visualElement);
		this.UpdatePlayer(player);
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x0003D084 File Offset: 0x0003B284
	public void RemovePlayer(Player player)
	{
		if (!this.playerVisualElementMap.ContainsKey(player))
		{
			return;
		}
		this.playerVisualElementMap[player].Query("ScoreboardPlayerButton", null).UnregisterCallback<ClickEvent, Player>(new EventCallback<ClickEvent, Player>(this.OnPlayerClicked), TrickleDown.NoTrickleDown);
		this.playerVisualElementMap[player].RemoveFromHierarchy();
		this.playerVisualElementMap.Remove(player);
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x0003D0EC File Offset: 0x0003B2EC
	public void UpdatePlayer(Player player)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		if (!this.playerVisualElementMap.ContainsKey(player))
		{
			return;
		}
		VisualElement visualElement = this.playerVisualElementMap[player];
		VisualElement visualElement2 = visualElement.Query("PatreonVisualElement", null);
		Label label = visualElement.Query("UsernameLabel", null);
		Label label2 = visualElement.Query("GoalsLabel", null);
		Label label3 = visualElement.Query("AssistsLabel", null);
		Label label4 = visualElement.Query("PointsLabel", null);
		Label label5 = visualElement.Query("PingLabel", null);
		Label label6 = visualElement.Query("PositionLabel", null);
		string arg = (player.AdminLevel.Value == 1) ? "<b><color=#206694>*</color></b>" : ((player.AdminLevel.Value == 2) ? "<b><color=#992d22>*</color></b>" : ((player.AdminLevel.Value > 2) ? "<b><color=#71368a>*</color></b>" : ""));
		visualElement2.style.display = ((player.PatreonLevel.Value > 0) ? DisplayStyle.Flex : DisplayStyle.None);
		label.style.color = ((player.PatreonLevel.Value > 0) ? this.patreonColor : Color.white);
		label6.text = (player.PlayerPosition ? player.PlayerPosition.Name.ToString() : "N/A");
		label.text = string.Format("{0}<noparse>#{1} {2}</noparse>", arg, player.Number.Value, player.Username.Value);
		label2.text = player.Goals.Value.ToString();
		label3.text = player.Assists.Value.ToString();
		label4.text = (player.Goals.Value + player.Assists.Value).ToString();
		label5.text = player.Ping.Value.ToString();
		visualElement.RemoveFromHierarchy();
		PlayerTeam value = player.Team.Value;
		if (value != PlayerTeam.Blue)
		{
			if (value != PlayerTeam.Red)
			{
				label2.text = "";
				label3.text = "";
				label4.text = "";
				if (visualElement.parent == this.teamSpectatorContainer)
				{
					return;
				}
				this.teamSpectatorContainer.Add(visualElement);
				return;
			}
			else
			{
				if (visualElement.parent == this.teamRedContainer)
				{
					return;
				}
				this.teamRedContainer.Add(visualElement);
				return;
			}
		}
		else
		{
			if (visualElement.parent == this.teamBlueContainer)
			{
				return;
			}
			this.teamBlueContainer.Add(visualElement);
			return;
		}
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x0003D398 File Offset: 0x0003B598
	public void UpdateServer(Server server, int playerCount)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		VisualElement e = this.container.Query("ServerContainer", null);
		Label label = e.Query("NameLabel", null);
		TextElement textElement = e.Query("PlayersLabel", null);
		label.text = server.Name.Value;
		textElement.text = string.Format("{0}/{1}", playerCount, server.MaxPlayers);
	}

	// Token: 0x06000A92 RID: 2706 RVA: 0x0003D418 File Offset: 0x0003B618
	public void Clear()
	{
		foreach (KeyValuePair<Player, VisualElement> keyValuePair in this.playerVisualElementMap.ToList<KeyValuePair<Player, VisualElement>>())
		{
			this.RemovePlayer(keyValuePair.Key);
		}
	}

	// Token: 0x06000A93 RID: 2707 RVA: 0x0000DBCA File Offset: 0x0000BDCA
	private void OnPlayerClicked(ClickEvent clickEvent, Player player)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnScoreboardClickPlayer", new Dictionary<string, object>
		{
			{
				"player",
				player
			}
		});
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x0003D478 File Offset: 0x0003B678
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000A96 RID: 2710 RVA: 0x0000DC1E File Offset: 0x0000BE1E
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x0000DC28 File Offset: 0x0000BE28
	protected internal override string __getTypeName()
	{
		return "UIScoreboard";
	}

	// Token: 0x04000628 RID: 1576
	[Header("Components")]
	public VisualTreeAsset scoreboardPlayerAsset;

	// Token: 0x04000629 RID: 1577
	[Header("Settings")]
	[SerializeField]
	private Color patreonColor = new Color(1f, 1f, 1f, 1f);

	// Token: 0x0400062A RID: 1578
	private VisualElement teamBlueContainer;

	// Token: 0x0400062B RID: 1579
	private VisualElement teamRedContainer;

	// Token: 0x0400062C RID: 1580
	private VisualElement teamSpectatorContainer;

	// Token: 0x0400062D RID: 1581
	private Dictionary<Player, VisualElement> playerVisualElementMap = new Dictionary<Player, VisualElement>();
}
