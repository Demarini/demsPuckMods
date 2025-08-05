using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000129 RID: 297
public class UIPositionSelect : UIComponent<UIPositionSelect>
{
	// Token: 0x06000A70 RID: 2672 RVA: 0x0000DA38 File Offset: 0x0000BC38
	public void Start()
	{
		base.VisibilityRequiresMouse = true;
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x0000DA41 File Offset: 0x0000BC41
	public void Initialize(VisualElement rootVisualElement)
	{
		this.rootVisualElement = rootVisualElement;
		this.container = rootVisualElement.Query("PositionSelectContainer", null);
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x0003CB3C File Offset: 0x0003AD3C
	private void Update()
	{
		if (Application.isBatchMode)
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

	// Token: 0x06000A73 RID: 2675 RVA: 0x0003CBE4 File Offset: 0x0003ADE4
	public void AddPosition(PlayerPosition playerPosition)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		if (this.playerPositionVisualElementMap.ContainsKey(playerPosition))
		{
			return;
		}
		TemplateContainer templateContainer = Utils.InstantiateVisualTreeAsset(this.claimPositionButtonAsset, Position.Absolute);
		VisualElement visualElement = templateContainer.Query("Position", null);
		visualElement.Query("PositionClaimButton", null).RegisterCallback<ClickEvent>(delegate(ClickEvent e)
		{
			this.OnPositionClicked(playerPosition);
		}, TrickleDown.NoTrickleDown);
		this.container.Add(templateContainer);
		this.playerPositionVisualElementMap.Add(playerPosition, templateContainer);
		this.StylePosition(visualElement, playerPosition);
	}

	// Token: 0x06000A74 RID: 2676 RVA: 0x0000DA61 File Offset: 0x0000BC61
	public void UpdatePosition(PlayerPosition playerPosition)
	{
		if (!this.playerPositionVisualElementMap.ContainsKey(playerPosition))
		{
			return;
		}
		this.StylePosition(this.playerPositionVisualElementMap[playerPosition], playerPosition);
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x0003CC90 File Offset: 0x0003AE90
	private void StylePosition(VisualElement positionVisualElement, PlayerPosition playerPosition)
	{
		Button button = positionVisualElement.Query("PositionClaimButton", null);
		Label label = positionVisualElement.Query("PositionUsernameLabel", null);
		PlayerTeam team = playerPosition.Team;
		string className;
		if (team != PlayerTeam.Blue)
		{
			if (team != PlayerTeam.Red)
			{
				className = null;
			}
			else
			{
				className = "team-red";
			}
		}
		else
		{
			className = "team-blue";
		}
		button.RemoveFromClassList("team-blue");
		button.RemoveFromClassList("team-red");
		button.AddToClassList(className);
		button.text = playerPosition.Name.ToString();
		button.enabledSelf = !playerPosition.IsClaimed;
		if (playerPosition.IsClaimed)
		{
			Player playerByClientId = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(playerPosition.ClaimedBy.OwnerClientId);
			if (playerByClientId)
			{
				label.text = playerByClientId.Username.Value.ToString();
				return;
			}
		}
		else
		{
			label.text = " ";
		}
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x0000DA85 File Offset: 0x0000BC85
	private void RemovePosition(PlayerPosition playerPosition)
	{
		if (!this.playerPositionVisualElementMap.ContainsKey(playerPosition))
		{
			return;
		}
		this.container.Remove(this.playerPositionVisualElementMap[playerPosition]);
		this.playerPositionVisualElementMap.Remove(playerPosition);
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x0003CD78 File Offset: 0x0003AF78
	public void ClearPositions()
	{
		foreach (KeyValuePair<PlayerPosition, VisualElement> keyValuePair in this.playerPositionVisualElementMap.ToList<KeyValuePair<PlayerPosition, VisualElement>>())
		{
			this.RemovePosition(keyValuePair.Key);
		}
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x0003CDD8 File Offset: 0x0003AFD8
	private void PositionWorldToScreen(VisualElement positionVisualElement, PlayerPosition playerPosition)
	{
		if (Camera.main == null)
		{
			return;
		}
		Vector3 vector = Camera.main.WorldToScreenPoint(playerPosition.transform.position);
		vector.y = (float)Screen.height - vector.y;
		RuntimePanelUtils.ScreenToPanel(this.rootVisualElement.panel, vector);
		Vector2 vector2 = RuntimePanelUtils.ScreenToPanel(this.rootVisualElement.panel, vector);
		if (vector.z < 0f)
		{
			positionVisualElement.style.display = DisplayStyle.None;
			return;
		}
		positionVisualElement.style.display = DisplayStyle.Flex;
		positionVisualElement.style.left = vector2.x;
		positionVisualElement.style.top = vector2.y;
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x0000DABA File Offset: 0x0000BCBA
	private void OnPositionClicked(PlayerPosition playerPosition)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPositionSelectClickPosition", new Dictionary<string, object>
		{
			{
				"playerPosition",
				playerPosition
			}
		});
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x0003CEAC File Offset: 0x0003B0AC
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x0000DAF7 File Offset: 0x0000BCF7
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000A7D RID: 2685 RVA: 0x0000DB01 File Offset: 0x0000BD01
	protected internal override string __getTypeName()
	{
		return "UIPositionSelect";
	}

	// Token: 0x04000621 RID: 1569
	[Header("Components")]
	[SerializeField]
	private VisualTreeAsset claimPositionButtonAsset;

	// Token: 0x04000622 RID: 1570
	[Header("Settings")]
	[SerializeField]
	private int updateRate = 30;

	// Token: 0x04000623 RID: 1571
	private Dictionary<PlayerPosition, VisualElement> playerPositionVisualElementMap = new Dictionary<PlayerPosition, VisualElement>();

	// Token: 0x04000624 RID: 1572
	private float updateAccumulator;
}
