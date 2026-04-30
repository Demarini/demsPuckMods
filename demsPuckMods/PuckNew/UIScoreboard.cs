using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x020001BE RID: 446
public class UIScoreboard : UIView
{
	// Token: 0x06000CD4 RID: 3284 RVA: 0x0003C61C File Offset: 0x0003A81C
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("ScoreboardView", null);
		this.scoreboard = base.View.Query("Scoreboard", null);
		this.header = this.scoreboard.Query("Header", null);
		this.players = this.scoreboard.Query("Players", null);
		this.nameLabel = this.header.Query("NameLabel", null);
		this.playersLabel = this.header.Query("PlayersLabel", null);
		this.players.Clear();
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x0003C6D8 File Offset: 0x0003A8D8
	public void AddPlayer(Player player)
	{
		if (this.playerVisualElementMap.ContainsKey(player))
		{
			return;
		}
		VisualElement visualElement = this.playerAsset.Instantiate();
		this.players.Add(visualElement);
		visualElement.Query(null, null).RegisterCallback<ClickEvent, Player>(new EventCallback<ClickEvent, Player>(this.OnPlayerClicked), player, TrickleDown.NoTrickleDown);
		this.playerVisualElementMap.Add(player, visualElement);
		this.StylePlayer(player);
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x0003C740 File Offset: 0x0003A940
	public void RemovePlayer(Player player)
	{
		if (!this.playerVisualElementMap.ContainsKey(player))
		{
			return;
		}
		this.playerVisualElementMap[player].Query(null, null).UnregisterCallback<ClickEvent, Player>(new EventCallback<ClickEvent, Player>(this.OnPlayerClicked), TrickleDown.NoTrickleDown);
		this.players.Remove(this.playerVisualElementMap[player]);
		this.playerVisualElementMap.Remove(player);
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x0003C7AC File Offset: 0x0003A9AC
	public void StylePlayer(Player player)
	{
		if (!this.playerVisualElementMap.ContainsKey(player))
		{
			return;
		}
		VisualElement visualElement = this.playerVisualElementMap[player].Query("Player", null);
		UIUtils.SetTeamClass(visualElement, player.Team);
		visualElement.EnableInClassList("patreon", player.PatreonLevel.Value > 0);
		visualElement.EnableInClassList("moderator", player.AdminLevel.Value == 1);
		visualElement.EnableInClassList("admin", player.AdminLevel.Value == 2);
		visualElement.EnableInClassList("developer", player.AdminLevel.Value == 3);
		Label label = visualElement.Query("PositionLabel", null);
		Label label2 = visualElement.Query("UsernameLabel", null);
		Label label3 = visualElement.Query("GoalsLabel", null);
		Label label4 = visualElement.Query("AssistsLabel", null);
		Label label5 = visualElement.Query("PointsLabel", null);
		TextElement textElement = visualElement.Query("PingLabel", null);
		bool flag = player.Team != PlayerTeam.Blue && player.Team != PlayerTeam.Red;
		label.text = (player.PlayerPosition ? player.PlayerPosition.Name.ToString() : string.Empty);
		label2.text = string.Format("#{0} {1}", player.Number.Value, player.Username.Value);
		label3.text = (flag ? string.Empty : player.Goals.Value.ToString());
		label4.text = (flag ? string.Empty : player.Assists.Value.ToString());
		label5.text = (flag ? string.Empty : (player.Goals.Value + player.Assists.Value).ToString());
		textElement.text = string.Format("{0}ms", player.Ping.Value);
		this.SortPlayers();
	}

	// Token: 0x06000CD8 RID: 3288 RVA: 0x0003C9D4 File Offset: 0x0003ABD4
	public void StyleServer(Server server, int playerCount)
	{
		this.nameLabel.text = server.Name.Value;
		this.playersLabel.text = string.Format("{0}/{1}", playerCount, server.MaxPlayers);
	}

	// Token: 0x06000CD9 RID: 3289 RVA: 0x0003CA14 File Offset: 0x0003AC14
	public void SortPlayers()
	{
		this.players.hierarchy.Sort(delegate(VisualElement a, VisualElement b)
		{
			Player key = this.playerVisualElementMap.FirstOrDefault((KeyValuePair<Player, VisualElement> x) => x.Value == a).Key;
			Player key2 = this.playerVisualElementMap.FirstOrDefault((KeyValuePair<Player, VisualElement> x) => x.Value == b).Key;
			int num = UIScoreboard.<SortPlayers>g__GetTeamOrder|12_1(key.Team);
			int num2 = UIScoreboard.<SortPlayers>g__GetTeamOrder|12_1(key2.Team);
			int num3 = key.Goals.Value + key.Assists.Value;
			int num4 = key2.Goals.Value + key2.Assists.Value;
			if (num != num2)
			{
				return num.CompareTo(num2);
			}
			if (num3 != num4)
			{
				return num4.CompareTo(num3);
			}
			return key.Username.Value.CompareTo(key2.Username.Value);
		});
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x0003CA40 File Offset: 0x0003AC40
	private void OnPlayerClicked(ClickEvent clickEvent, Player player)
	{
		EventManager.TriggerEvent("Event_OnScoreboardClickPlayer", new Dictionary<string, object>
		{
			{
				"player",
				player
			}
		});
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x0003CB5C File Offset: 0x0003AD5C
	[CompilerGenerated]
	internal static int <SortPlayers>g__GetTeamOrder|12_1(PlayerTeam team)
	{
		if (team == PlayerTeam.Blue)
		{
			return 0;
		}
		if (team != PlayerTeam.Red)
		{
			return 2;
		}
		return 1;
	}

	// Token: 0x04000792 RID: 1938
	[Header("References")]
	public VisualTreeAsset playerAsset;

	// Token: 0x04000793 RID: 1939
	private VisualElement scoreboard;

	// Token: 0x04000794 RID: 1940
	private VisualElement header;

	// Token: 0x04000795 RID: 1941
	private VisualElement players;

	// Token: 0x04000796 RID: 1942
	private Label nameLabel;

	// Token: 0x04000797 RID: 1943
	private Label playersLabel;

	// Token: 0x04000798 RID: 1944
	private Dictionary<Player, VisualElement> playerVisualElementMap = new Dictionary<Player, VisualElement>();
}
