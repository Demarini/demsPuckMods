using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x020001BB RID: 443
public class UIPositionSelect : UIView
{
	// Token: 0x1700011B RID: 283
	// (get) Token: 0x06000CC0 RID: 3264 RVA: 0x0003C00A File Offset: 0x0003A20A
	// (set) Token: 0x06000CC1 RID: 3265 RVA: 0x0003C014 File Offset: 0x0003A214
	public PlayerTeam Team
	{
		get
		{
			return this.team;
		}
		set
		{
			if (this.team == value)
			{
				return;
			}
			PlayerTeam oldTeam = this.team;
			this.team = value;
			this.OnTeamChanged(oldTeam, this.team);
		}
	}

	// Token: 0x06000CC2 RID: 3266 RVA: 0x0003C046 File Offset: 0x0003A246
	public void Initialize(VisualElement rootVisualElement)
	{
		this.RootVisualElement = rootVisualElement;
		base.View = rootVisualElement.Query("PositionsView", null);
		this.positions = base.View.Query("Positions", null);
	}

	// Token: 0x06000CC3 RID: 3267 RVA: 0x0003C084 File Offset: 0x0003A284
	private void Update()
	{
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return;
		}
		this.updateAccumulator += Time.deltaTime;
		if (this.updateAccumulator < 1f / (float)this.updateRate)
		{
			return;
		}
		this.updateAccumulator = 0f;
		foreach (KeyValuePair<PlayerPosition, VisualElement> keyValuePair in this.playerPositionVisualElementMap)
		{
			PlayerPosition key = keyValuePair.Key;
			VisualElement value = keyValuePair.Value;
			if (!(key == null))
			{
				this.PositionWorldToScreen(value, key);
			}
		}
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x0003C12C File Offset: 0x0003A32C
	public void AddPosition(PlayerPosition playerPosition)
	{
		if (this.playerPositionVisualElementMap.ContainsKey(playerPosition))
		{
			return;
		}
		VisualElement visualElement = this.positionAsset.Instantiate();
		visualElement.Query(null, null).RegisterCallback<ClickEvent>(delegate(ClickEvent e)
		{
			this.OnPositionClicked(playerPosition);
		}, TrickleDown.NoTrickleDown);
		this.positions.Add(visualElement);
		this.playerPositionVisualElementMap.Add(playerPosition, visualElement);
		this.StylePosition(playerPosition);
	}

	// Token: 0x06000CC5 RID: 3269 RVA: 0x0003C1B8 File Offset: 0x0003A3B8
	public void StylePosition(PlayerPosition playerPosition)
	{
		if (!this.playerPositionVisualElementMap.ContainsKey(playerPosition))
		{
			return;
		}
		VisualElement visualElement = this.playerPositionVisualElementMap[playerPosition];
		VisualElement visualElement2 = visualElement.Query("Position", null);
		Button button = visualElement.Query(null, null);
		Label label = visualElement.Query("UsernameLabel", null);
		UIUtils.SetTeamClass(visualElement2, playerPosition.Team);
		visualElement2.EnableInClassList("claimed", playerPosition.IsClaimed);
		button.text = playerPosition.Name.ToString();
		if (playerPosition.IsClaimed)
		{
			label.text = playerPosition.ClaimedByPlayer.Username.Value.ToString();
		}
		else
		{
			label.text = null;
		}
		visualElement.style.display = ((this.Team == playerPosition.Team) ? DisplayStyle.Flex : DisplayStyle.None);
	}

	// Token: 0x06000CC6 RID: 3270 RVA: 0x0003C296 File Offset: 0x0003A496
	public void RemovePosition(PlayerPosition playerPosition)
	{
		if (!this.playerPositionVisualElementMap.ContainsKey(playerPosition))
		{
			return;
		}
		this.positions.Remove(this.playerPositionVisualElementMap[playerPosition]);
		this.playerPositionVisualElementMap.Remove(playerPosition);
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x0003C2CC File Offset: 0x0003A4CC
	private void PositionWorldToScreen(VisualElement positionVisualElement, PlayerPosition playerPosition)
	{
		if (Camera.main == null)
		{
			return;
		}
		Vector3 vector = Camera.main.WorldToScreenPoint(playerPosition.transform.position);
		vector.y = (float)Screen.height - vector.y;
		RuntimePanelUtils.ScreenToPanel(this.RootVisualElement.panel, vector);
		Vector2 vector2 = RuntimePanelUtils.ScreenToPanel(this.RootVisualElement.panel, vector);
		if (vector.z < 0f)
		{
			positionVisualElement.style.visibility = Visibility.Hidden;
			return;
		}
		positionVisualElement.style.visibility = Visibility.Visible;
		positionVisualElement.style.left = vector2.x;
		positionVisualElement.style.top = vector2.y;
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x0003C39D File Offset: 0x0003A59D
	private void OnPositionClicked(PlayerPosition playerPosition)
	{
		EventManager.TriggerEvent("Event_OnPositionSelectClickPosition", new Dictionary<string, object>
		{
			{
				"playerPosition",
				playerPosition
			}
		});
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x0003C3BC File Offset: 0x0003A5BC
	private void OnTeamChanged(PlayerTeam oldTeam, PlayerTeam newTeam)
	{
		foreach (PlayerPosition playerPosition in this.playerPositionVisualElementMap.Keys)
		{
			this.StylePosition(playerPosition);
		}
	}

	// Token: 0x04000789 RID: 1929
	[Header("Settings")]
	[SerializeField]
	private int updateRate = 30;

	// Token: 0x0400078A RID: 1930
	[Header("References")]
	[SerializeField]
	private VisualTreeAsset positionAsset;

	// Token: 0x0400078B RID: 1931
	private PlayerTeam team;

	// Token: 0x0400078C RID: 1932
	private Dictionary<PlayerPosition, VisualElement> playerPositionVisualElementMap = new Dictionary<PlayerPosition, VisualElement>();

	// Token: 0x0400078D RID: 1933
	private float updateAccumulator;

	// Token: 0x0400078E RID: 1934
	private VisualElement positions;
}
