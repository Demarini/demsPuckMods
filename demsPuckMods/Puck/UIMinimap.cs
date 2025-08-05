using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000112 RID: 274
public class UIMinimap : UIComponent<UIMinimap>
{
	// Token: 0x170000DB RID: 219
	// (get) Token: 0x060009A4 RID: 2468 RVA: 0x00039EA4 File Offset: 0x000380A4
	[HideInInspector]
	public Vector2 Position
	{
		get
		{
			return new Vector2(this.minimapVisualElement.style.left.value.value, this.minimapVisualElement.style.bottom.value.value);
		}
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x00039EF8 File Offset: 0x000380F8
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
		foreach (KeyValuePair<PlayerBodyV2, VisualElement> keyValuePair in this.playerBodyVisualElementMap)
		{
			PlayerBodyV2 key = keyValuePair.Key;
			VisualElement value = keyValuePair.Value;
			if (key)
			{
				VisualElement visualElement = value.Query("Body", null);
				Vector3 position = (this.Team == PlayerTeam.Blue) ? key.transform.position : (-key.transform.position);
				float num = (this.Team == PlayerTeam.Blue) ? key.transform.rotation.eulerAngles.y : (key.transform.rotation.eulerAngles.y + 180f);
				Vector2 vector = this.WorldPositionToMinimapPosition(position, NetworkBehaviourSingleton<LevelManager>.Instance.IceBounds);
				value.style.translate = new Translate(-vector.x, vector.y);
				visualElement.style.rotate = new Rotate(num);
				VisualElement e = this.playerBodyVisualElementMap[key];
				if (!key.Player)
				{
					return;
				}
				e.Query("Number", null).style.rotate = new Rotate(-num + 180f);
				if (key.IsClient)
				{
					this.minimapVisualElement.style.rotate = new Rotate(-num + 180f);
				}
			}
		}
		foreach (KeyValuePair<Puck, VisualElement> keyValuePair2 in this.puckVisualElementMap)
		{
			Puck key2 = keyValuePair2.Key;
			VisualElement value2 = keyValuePair2.Value;
			if (key2)
			{
				Vector3 position2 = (this.Team == PlayerTeam.Blue) ? key2.transform.position : (-key2.transform.position);
				float value3 = (this.Team == PlayerTeam.Blue) ? key2.transform.rotation.eulerAngles.y : (key2.transform.rotation.eulerAngles.y + 180f);
				Vector2 vector2 = this.WorldPositionToMinimapPosition(position2, NetworkBehaviourSingleton<LevelManager>.Instance.IceBounds);
				value2.style.translate = new Translate(-vector2.x, vector2.y);
				value2.style.rotate = new Rotate(value3);
			}
		}
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x0003A24C File Offset: 0x0003844C
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("MinimapContainer", null);
		this.minimapVisualElement = this.container.Query("Minimap", null);
		this.minimapBackgroundVisualElement = this.minimapVisualElement.Query("MinimapBackground", null);
		this.minimapMarkingsVisualElement = this.minimapVisualElement.Query("MinimapMarkings", null);
	}

	// Token: 0x060009A7 RID: 2471 RVA: 0x0000D242 File Offset: 0x0000B442
	public override void Show()
	{
		if (MonoBehaviourSingleton<SettingsManager>.Instance.ShowGameUserInterface == 0)
		{
			return;
		}
		base.Show();
		this.SetOpacity(MonoBehaviourSingleton<SettingsManager>.Instance.MinimapOpacity);
	}

	// Token: 0x060009A8 RID: 2472 RVA: 0x0003A2C4 File Offset: 0x000384C4
	public void AddPlayerBody(PlayerBodyV2 playerBody)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		if (!playerBody)
		{
			return;
		}
		if (this.playerBodyVisualElementMap.ContainsKey(playerBody))
		{
			return;
		}
		VisualElement visualElement = Utils.InstantiateVisualTreeAsset(this.playerAsset, UnityEngine.UIElements.Position.Absolute).Query("MinimapPlayer", null);
		this.playerBodyVisualElementMap.Add(playerBody, visualElement);
		this.minimapMarkingsVisualElement.Add(visualElement);
		visualElement.SendToBack();
		this.UpdatePlayerBody(playerBody);
	}

	// Token: 0x060009A9 RID: 2473 RVA: 0x0003A334 File Offset: 0x00038534
	public void UpdatePlayerBody(PlayerBodyV2 playerBody)
	{
		if (!playerBody)
		{
			return;
		}
		if (!this.playerBodyVisualElementMap.ContainsKey(playerBody))
		{
			return;
		}
		VisualElement e = this.playerBodyVisualElementMap[playerBody];
		Player player = playerBody.Player;
		if (!player)
		{
			return;
		}
		VisualElement visualElement = e.Query("Body", null);
		VisualElement visualElement2 = e.Query("Local", null);
		Label label = e.Query("Number", null);
		PlayerTeam value = player.Team.Value;
		if (value != PlayerTeam.Blue)
		{
			if (value != PlayerTeam.Red)
			{
				visualElement.style.unityBackgroundImageTintColor = new StyleColor(Color.gray);
			}
			else
			{
				visualElement.style.unityBackgroundImageTintColor = new StyleColor(this.teamRedColor);
			}
		}
		else
		{
			visualElement.style.unityBackgroundImageTintColor = new StyleColor(this.teamBlueColor);
		}
		label.text = player.Number.Value.ToString();
		label.style.visibility = (player.IsLocalPlayer ? Visibility.Hidden : Visibility.Visible);
		visualElement2.style.visibility = (player.IsLocalPlayer ? Visibility.Visible : Visibility.Hidden);
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x0000D267 File Offset: 0x0000B467
	public void RemovePlayerBody(PlayerBodyV2 playerBody)
	{
		if (!playerBody)
		{
			return;
		}
		if (!this.playerBodyVisualElementMap.ContainsKey(playerBody))
		{
			return;
		}
		this.minimapMarkingsVisualElement.Remove(this.playerBodyVisualElementMap[playerBody]);
		this.playerBodyVisualElementMap.Remove(playerBody);
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x0003A458 File Offset: 0x00038658
	public void AddPuck(Puck puck)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		if (!puck)
		{
			return;
		}
		if (this.puckVisualElementMap.ContainsKey(puck))
		{
			return;
		}
		VisualElement visualElement = Utils.InstantiateVisualTreeAsset(this.puckAsset, UnityEngine.UIElements.Position.Absolute).Query("MinimapPuck", null);
		this.puckVisualElementMap.Add(puck, visualElement);
		this.minimapMarkingsVisualElement.Add(visualElement);
		visualElement.BringToFront();
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x0000D2A5 File Offset: 0x0000B4A5
	public void RemovePuck(Puck puck)
	{
		if (!puck)
		{
			return;
		}
		if (!this.puckVisualElementMap.ContainsKey(puck))
		{
			return;
		}
		this.minimapMarkingsVisualElement.Remove(this.puckVisualElementMap[puck]);
		this.puckVisualElementMap.Remove(puck);
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x0003A4C4 File Offset: 0x000386C4
	private Vector2 WorldPositionToMinimapPosition(Vector3 position, Bounds bounds)
	{
		Vector2 vector = new Vector2((position.x + bounds.center.x) / bounds.size.x, (position.z + bounds.center.z) / bounds.size.z);
		Vector2 vector2 = new Vector2(this.minimapMarkingsVisualElement.resolvedStyle.width, this.minimapMarkingsVisualElement.resolvedStyle.height);
		return new Vector2(vector2.x * vector.x, vector2.y * vector.y);
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x0000D2E3 File Offset: 0x0000B4E3
	public void SetOpacity(float opacity)
	{
		if (this.minimapVisualElement == null)
		{
			return;
		}
		this.minimapVisualElement.style.opacity = opacity;
	}

	// Token: 0x060009AF RID: 2479 RVA: 0x0003A560 File Offset: 0x00038760
	public void SetPosition(Vector2 position)
	{
		if (this.minimapVisualElement == null)
		{
			return;
		}
		this.minimapVisualElement.style.left = new Length(position.x, LengthUnit.Percent);
		this.minimapVisualElement.style.bottom = new Length(position.y, LengthUnit.Percent);
		float value = Utils.Map(position.x, 0f, 100f, 0f, -100f);
		float value2 = Utils.Map(position.y, 0f, 100f, 0f, 100f);
		this.minimapVisualElement.style.translate = new Translate(new Length(value, LengthUnit.Percent), new Length(value2, LengthUnit.Percent));
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x0000D304 File Offset: 0x0000B504
	public void SetBackgroundOpacity(float opacity)
	{
		if (this.minimapBackgroundVisualElement == null)
		{
			return;
		}
		this.minimapBackgroundVisualElement.style.opacity = 0f;
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x0003A620 File Offset: 0x00038820
	public void SetScale(float scale)
	{
		if (this.minimapVisualElement == null)
		{
			return;
		}
		this.minimapVisualElement.style.width = new Length(this.size.x * scale, LengthUnit.Pixel);
		this.minimapVisualElement.style.height = new Length(this.size.y * scale, LengthUnit.Pixel);
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x0000D329 File Offset: 0x0000B529
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x0000D331 File Offset: 0x0000B531
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x0000D339 File Offset: 0x0000B539
	protected internal override string __getTypeName()
	{
		return "UIMinimap";
	}

	// Token: 0x060009B6 RID: 2486 RVA: 0x0000D340 File Offset: 0x0000B540
	private void Start()
	{
		if (this.Team == PlayerTeam.Blue)
		{
			this.minimapVisualElement.style.rotate = new Rotate(new Angle(180f, AngleUnit.Degree));
		}
	}

	// Token: 0x040005C7 RID: 1479
	[Header("Components")]
	[SerializeField]
	private VisualTreeAsset playerAsset;

	// Token: 0x040005C8 RID: 1480
	[SerializeField]
	private VisualTreeAsset puckAsset;

	// Token: 0x040005C9 RID: 1481
	[Header("Settings")]
	[SerializeField]
	private int updateRate = 30;

	// Token: 0x040005CA RID: 1482
	[SerializeField]
	private Color teamBlueColor = Color.blue;

	// Token: 0x040005CB RID: 1483
	[SerializeField]
	private Color teamRedColor = Color.red;

	// Token: 0x040005CC RID: 1484
	[SerializeField]
	private Vector2 size = new Vector2(256f, 512f);

	// Token: 0x040005CD RID: 1485
	[HideInInspector]
	public PlayerTeam Team;

	// Token: 0x040005CE RID: 1486
	private VisualElement minimapVisualElement;

	// Token: 0x040005CF RID: 1487
	private VisualElement minimapBackgroundVisualElement;

	// Token: 0x040005D0 RID: 1488
	private VisualElement minimapMarkingsVisualElement;

	// Token: 0x040005D1 RID: 1489
	private Dictionary<PlayerBodyV2, VisualElement> playerBodyVisualElementMap = new Dictionary<PlayerBodyV2, VisualElement>();

	// Token: 0x040005D2 RID: 1490
	private Dictionary<Puck, VisualElement> puckVisualElementMap = new Dictionary<Puck, VisualElement>();

	// Token: 0x040005D3 RID: 1491
	private float updateAccumulator;
}
