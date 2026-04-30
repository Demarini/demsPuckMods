using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x020001B3 RID: 435
public class UIPlayerUsernames : UIView
{
	// Token: 0x17000116 RID: 278
	// (get) Token: 0x06000C7D RID: 3197 RVA: 0x0003AC88 File Offset: 0x00038E88
	[HideInInspector]
	public float MaximumDistance
	{
		get
		{
			return Mathf.Max(this.Bounds.size.x, this.Bounds.size.z);
		}
	}

	// Token: 0x17000117 RID: 279
	// (get) Token: 0x06000C7E RID: 3198 RVA: 0x0003ACAF File Offset: 0x00038EAF
	[HideInInspector]
	public float FadeRange
	{
		get
		{
			return this.MaximumDistance / 4f;
		}
	}

	// Token: 0x06000C7F RID: 3199 RVA: 0x0003ACC0 File Offset: 0x00038EC0
	public void Initialize(VisualElement rootVisualElement)
	{
		this.RootVisualElement = rootVisualElement;
		base.View = rootVisualElement.Query("UsernamesView", null);
		this.usernames = base.View.Query("Usernames", null);
		this.usernames.Clear();
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x0003AD14 File Offset: 0x00038F14
	private void Update()
	{
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return;
		}
		foreach (KeyValuePair<PlayerBody, VisualElement> keyValuePair in this.playerBodyVisualElementMap)
		{
			PlayerBody key = keyValuePair.Key;
			VisualElement value = keyValuePair.Value;
			if (!(key == null))
			{
				this.UsernameWorldToScreen(value, key);
			}
		}
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x0003AD8C File Offset: 0x00038F8C
	public void AddPlayerBody(PlayerBody playerBody)
	{
		TemplateContainer templateContainer = this.playerUsernameAsset.Instantiate();
		this.playerBodyVisualElementMap.Add(playerBody, templateContainer);
		this.StyleUsername(playerBody);
		this.usernames.Add(templateContainer);
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x0003ADC8 File Offset: 0x00038FC8
	public void RemovePlayerBody(PlayerBody playerBody)
	{
		if (!this.playerBodyVisualElementMap.ContainsKey(playerBody))
		{
			return;
		}
		VisualElement element = this.playerBodyVisualElementMap[playerBody];
		this.playerBodyVisualElementMap.Remove(playerBody);
		this.usernames.Remove(element);
	}

	// Token: 0x06000C83 RID: 3203 RVA: 0x0003AE0C File Offset: 0x0003900C
	public void StyleUsername(PlayerBody playerBody)
	{
		if (!this.playerBodyVisualElementMap.ContainsKey(playerBody))
		{
			return;
		}
		this.playerBodyVisualElementMap[playerBody].Query("UsernameLabel", null).text = string.Format("#{0} {1}", playerBody.Player.Number.Value, playerBody.Player.Username.Value);
	}

	// Token: 0x06000C84 RID: 3204 RVA: 0x0003AE80 File Offset: 0x00039080
	private void UsernameWorldToScreen(VisualElement playerVisualElement, PlayerBody playerBody)
	{
		if (Camera.main == null)
		{
			return;
		}
		Vector3 position = Camera.main.transform.position;
		Vector3 position2 = playerBody.transform.position;
		float value = Vector3.Distance(position, position2);
		Vector3 vector = Camera.main.WorldToScreenPoint(position2 + Vector3.up * this.yOffset);
		vector.y = (float)Screen.height - vector.y;
		RuntimePanelUtils.ScreenToPanel(this.RootVisualElement.panel, vector);
		Vector2 vector2 = RuntimePanelUtils.ScreenToPanel(this.RootVisualElement.panel, vector);
		if (vector.z < 0f)
		{
			playerVisualElement.style.display = DisplayStyle.None;
			return;
		}
		float num = Utils.Map(value, this.MaximumDistance * this.FadeThreshold, this.MaximumDistance * this.FadeThreshold + this.FadeRange, 1f, 0f);
		num = Mathf.Clamp01(num);
		playerVisualElement.style.display = DisplayStyle.Flex;
		playerVisualElement.style.left = vector2.x;
		playerVisualElement.style.top = vector2.y;
		playerVisualElement.style.opacity = new StyleFloat(num);
	}

	// Token: 0x04000766 RID: 1894
	[Header("Settings")]
	[SerializeField]
	private float yOffset = 2.5f;

	// Token: 0x04000767 RID: 1895
	[Header("References")]
	[SerializeField]
	private VisualTreeAsset playerUsernameAsset;

	// Token: 0x04000768 RID: 1896
	[HideInInspector]
	public float FadeThreshold = 0.5f;

	// Token: 0x04000769 RID: 1897
	[HideInInspector]
	public Bounds Bounds;

	// Token: 0x0400076A RID: 1898
	private Dictionary<PlayerBody, VisualElement> playerBodyVisualElementMap = new Dictionary<PlayerBody, VisualElement>();

	// Token: 0x0400076B RID: 1899
	private VisualElement usernames;
}
