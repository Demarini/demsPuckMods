using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x0200019F RID: 415
public class UIMinimap : UIView
{
	// Token: 0x17000115 RID: 277
	// (get) Token: 0x06000BD9 RID: 3033 RVA: 0x00037C70 File Offset: 0x00035E70
	[HideInInspector]
	public Vector2 Position
	{
		get
		{
			return new Vector2(this.minimap.style.left.value.value, this.minimap.style.top.value.value);
		}
	}

	// Token: 0x06000BDA RID: 3034 RVA: 0x00037CC4 File Offset: 0x00035EC4
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
		foreach (KeyValuePair<PlayerBody, VisualElement> keyValuePair in this.playerBodyVisualElementMap)
		{
			PlayerBody key = keyValuePair.Key;
			VisualElement value = keyValuePair.Value;
			if (key)
			{
				VisualElement visualElement = value.Query("Body", null);
				Vector3 position = (this.Team == PlayerTeam.Blue) ? key.transform.position : (-key.transform.position);
				float value2 = (this.Team == PlayerTeam.Blue) ? key.transform.rotation.eulerAngles.y : (key.transform.rotation.eulerAngles.y + 180f);
				Vector2 vector = this.WorldPositionToMinimapPosition(position, this.Bounds);
				value.style.translate = new Translate(-vector.x, vector.y);
				visualElement.style.rotate = new Rotate(value2);
			}
		}
		foreach (KeyValuePair<Puck, VisualElement> keyValuePair2 in this.puckVisualElementMap)
		{
			Puck key2 = keyValuePair2.Key;
			VisualElement value3 = keyValuePair2.Value;
			if (key2)
			{
				Vector3 position2 = (this.Team == PlayerTeam.Blue) ? key2.transform.position : (-key2.transform.position);
				float value4 = (this.Team == PlayerTeam.Blue) ? key2.transform.rotation.eulerAngles.y : (key2.transform.rotation.eulerAngles.y + 180f);
				Vector2 vector2 = this.WorldPositionToMinimapPosition(position2, this.Bounds);
				value3.style.translate = new Translate(-vector2.x, vector2.y);
				value3.style.rotate = new Rotate(value4);
			}
		}
	}

	// Token: 0x06000BDB RID: 3035 RVA: 0x00037F8C File Offset: 0x0003618C
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("MinimapView", null);
		this.minimap = base.View.Query("Minimap", null);
		this.background = this.minimap.Query("Background", null);
		this.foreground = this.minimap.Query("Foreground", null);
		this.content = this.minimap.Query("Content", null);
		this.content.Clear();
	}

	// Token: 0x06000BDC RID: 3036 RVA: 0x0003233E File Offset: 0x0003053E
	public override bool Show()
	{
		return SettingsManager.ShowGameUserInterface && base.Show();
	}

	// Token: 0x06000BDD RID: 3037 RVA: 0x0003802C File Offset: 0x0003622C
	public void AddPlayerBody(PlayerBody playerBody)
	{
		if (!playerBody)
		{
			return;
		}
		if (this.playerBodyVisualElementMap.ContainsKey(playerBody))
		{
			return;
		}
		TemplateContainer templateContainer = this.playerAsset.Instantiate();
		this.playerBodyVisualElementMap.Add(playerBody, templateContainer);
		this.content.Add(templateContainer);
		templateContainer.SendToBack();
		this.StylePlayer(playerBody);
	}

	// Token: 0x06000BDE RID: 3038 RVA: 0x00038084 File Offset: 0x00036284
	public void StylePlayer(PlayerBody playerBody)
	{
		if (!playerBody)
		{
			return;
		}
		if (!this.playerBodyVisualElementMap.ContainsKey(playerBody))
		{
			return;
		}
		Player player = playerBody.Player;
		if (!player)
		{
			return;
		}
		VisualElement visualElement = this.playerBodyVisualElementMap[playerBody].Query("Player", null);
		Label label = visualElement.Query("NumberLabel", null);
		UIUtils.SetTeamClass(visualElement, player.Team);
		visualElement.EnableInClassList("isLocalPlayer", player.IsLocalPlayer);
		label.text = player.Number.Value.ToString();
	}

	// Token: 0x06000BDF RID: 3039 RVA: 0x0003811C File Offset: 0x0003631C
	public void RemovePlayerBody(PlayerBody playerBody)
	{
		if (!playerBody)
		{
			return;
		}
		if (!this.playerBodyVisualElementMap.ContainsKey(playerBody))
		{
			return;
		}
		this.content.Remove(this.playerBodyVisualElementMap[playerBody]);
		this.playerBodyVisualElementMap.Remove(playerBody);
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x0003815C File Offset: 0x0003635C
	public void AddPuck(Puck puck)
	{
		if (!puck)
		{
			return;
		}
		if (this.puckVisualElementMap.ContainsKey(puck))
		{
			return;
		}
		TemplateContainer templateContainer = this.puckAsset.Instantiate();
		this.puckVisualElementMap.Add(puck, templateContainer);
		this.content.Add(templateContainer);
		templateContainer.BringToFront();
	}

	// Token: 0x06000BE1 RID: 3041 RVA: 0x000381AC File Offset: 0x000363AC
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
		this.content.Remove(this.puckVisualElementMap[puck]);
		this.puckVisualElementMap.Remove(puck);
	}

	// Token: 0x06000BE2 RID: 3042 RVA: 0x000381EC File Offset: 0x000363EC
	private Vector2 WorldPositionToMinimapPosition(Vector3 position, Bounds bounds)
	{
		Vector2 vector = new Vector2((position.x + bounds.center.x) / bounds.size.x, (position.z + bounds.center.z) / bounds.size.z);
		Vector2 vector2 = new Vector2(this.content.resolvedStyle.width, this.content.resolvedStyle.height);
		return new Vector2(vector2.x * vector.x, vector2.y * vector.y);
	}

	// Token: 0x06000BE3 RID: 3043 RVA: 0x00038286 File Offset: 0x00036486
	public void SetOpacity(float opacity)
	{
		if (this.minimap == null)
		{
			return;
		}
		this.minimap.style.opacity = opacity;
	}

	// Token: 0x06000BE4 RID: 3044 RVA: 0x000382A8 File Offset: 0x000364A8
	public void SetPosition(Vector2 position)
	{
		if (this.minimap == null)
		{
			return;
		}
		Length x = new Length(Utils.Map(position.x, 0f, 100f, 0f, -100f), LengthUnit.Percent);
		Length y = new Length(Utils.Map(position.y, 0f, 100f, 0f, -100f), LengthUnit.Percent);
		Length x2 = new Length(-x.value, LengthUnit.Percent);
		Length y2 = new Length(-y.value, LengthUnit.Percent);
		this.minimap.style.left = new Length(position.x, LengthUnit.Percent);
		this.minimap.style.top = new Length(position.y, LengthUnit.Percent);
		this.minimap.style.translate = new Translate(x, y);
		this.minimap.style.transformOrigin = new TransformOrigin(x2, y2);
	}

	// Token: 0x06000BE5 RID: 3045 RVA: 0x000383A6 File Offset: 0x000365A6
	public void SetBackgroundOpacity(float opacity)
	{
		if (this.background == null)
		{
			return;
		}
		this.background.style.opacity = opacity;
	}

	// Token: 0x06000BE6 RID: 3046 RVA: 0x000383C7 File Offset: 0x000365C7
	public void SetScale(float scale)
	{
		if (this.minimap == null)
		{
			return;
		}
		this.minimap.style.scale = new Vector2(scale, scale);
	}

	// Token: 0x040006FE RID: 1790
	[Header("Settings")]
	[SerializeField]
	private int updateRate = 30;

	// Token: 0x040006FF RID: 1791
	[Header("References")]
	[SerializeField]
	private VisualTreeAsset playerAsset;

	// Token: 0x04000700 RID: 1792
	[SerializeField]
	private VisualTreeAsset puckAsset;

	// Token: 0x04000701 RID: 1793
	[HideInInspector]
	public PlayerTeam Team;

	// Token: 0x04000702 RID: 1794
	[HideInInspector]
	public Bounds Bounds;

	// Token: 0x04000703 RID: 1795
	private VisualElement minimap;

	// Token: 0x04000704 RID: 1796
	private VisualElement background;

	// Token: 0x04000705 RID: 1797
	private VisualElement foreground;

	// Token: 0x04000706 RID: 1798
	private VisualElement content;

	// Token: 0x04000707 RID: 1799
	private Dictionary<PlayerBody, VisualElement> playerBodyVisualElementMap = new Dictionary<PlayerBody, VisualElement>();

	// Token: 0x04000708 RID: 1800
	private Dictionary<Puck, VisualElement> puckVisualElementMap = new Dictionary<Puck, VisualElement>();

	// Token: 0x04000709 RID: 1801
	private float updateAccumulator;
}
