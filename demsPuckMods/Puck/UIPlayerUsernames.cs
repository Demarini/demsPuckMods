using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000121 RID: 289
public class UIPlayerUsernames : UIComponent<UIPlayerUsernames>
{
	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x06000A30 RID: 2608 RVA: 0x0000D7FC File Offset: 0x0000B9FC
	[HideInInspector]
	public float FadeRange
	{
		get
		{
			return this.maximumDistance / 4f;
		}
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x0000D80A File Offset: 0x0000BA0A
	public override void Awake()
	{
		base.Awake();
		base.AlwaysVisible = true;
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x0000D819 File Offset: 0x0000BA19
	public void Initialize(VisualElement rootVisualElement)
	{
		this.rootVisualElement = rootVisualElement;
		this.container = rootVisualElement.Query("PlayerUsernamesContainer", null);
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x0003BAEC File Offset: 0x00039CEC
	public void Update()
	{
		if (Application.isBatchMode)
		{
			return;
		}
		foreach (KeyValuePair<PlayerBodyV2, VisualElement> keyValuePair in this.playerBodyVisualElementMap)
		{
			PlayerBodyV2 key = keyValuePair.Key;
			VisualElement value = keyValuePair.Value;
			if (!(key == null))
			{
				this.UsernameWorldToScreen(value, key);
			}
		}
	}

	// Token: 0x06000A34 RID: 2612 RVA: 0x0003BB64 File Offset: 0x00039D64
	public void AddPlayerBody(PlayerBodyV2 playerBody)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		VisualElement visualElement = Utils.InstantiateVisualTreeAsset(this.playerUsernameAsset, Position.Absolute).Query("PlayerUsername", null);
		this.playerBodyVisualElementMap.Add(playerBody, visualElement);
		this.UpdatePlayerBody(playerBody);
		this.container.Add(visualElement);
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x0003BBB8 File Offset: 0x00039DB8
	public void RemovePlayerBody(PlayerBodyV2 playerBody)
	{
		if (!this.playerBodyVisualElementMap.ContainsKey(playerBody))
		{
			return;
		}
		VisualElement element = this.playerBodyVisualElementMap[playerBody];
		this.playerBodyVisualElementMap.Remove(playerBody);
		this.container.Remove(element);
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x0003BBFC File Offset: 0x00039DFC
	public void UpdatePlayerBody(PlayerBodyV2 playerBody)
	{
		if (!this.playerBodyVisualElementMap.ContainsKey(playerBody))
		{
			return;
		}
		this.playerBodyVisualElementMap[playerBody].Query("Username", null).text = string.Format("#{0} {1}", playerBody.Player.Number.Value, playerBody.Player.Username.Value);
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x0003BC70 File Offset: 0x00039E70
	private void UsernameWorldToScreen(VisualElement playerVisualElement, PlayerBodyV2 playerBody)
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
		RuntimePanelUtils.ScreenToPanel(this.rootVisualElement.panel, vector);
		Vector2 vector2 = RuntimePanelUtils.ScreenToPanel(this.rootVisualElement.panel, vector);
		if (vector.z < 0f)
		{
			playerVisualElement.style.display = DisplayStyle.None;
			return;
		}
		playerVisualElement.style.display = DisplayStyle.Flex;
		playerVisualElement.style.left = vector2.x;
		playerVisualElement.style.top = vector2.y;
		float num = Utils.Map(value, this.maximumDistance * this.FadeThreshold - this.FadeRange / 2f, this.maximumDistance * this.FadeThreshold + this.FadeRange / 2f, 1f, 0f);
		num = Mathf.Clamp01(num);
		playerVisualElement.style.opacity = new StyleFloat(num);
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x0003BDD0 File Offset: 0x00039FD0
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x0000D86D File Offset: 0x0000BA6D
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x0000D877 File Offset: 0x0000BA77
	protected internal override string __getTypeName()
	{
		return "UIPlayerUsernames";
	}

	// Token: 0x04000604 RID: 1540
	[Header("Components")]
	[SerializeField]
	private VisualTreeAsset playerUsernameAsset;

	// Token: 0x04000605 RID: 1541
	[Header("Settings")]
	[SerializeField]
	private float yOffset = 2.5f;

	// Token: 0x04000606 RID: 1542
	[SerializeField]
	private float maximumDistance = 100f;

	// Token: 0x04000607 RID: 1543
	[HideInInspector]
	public float FadeThreshold = 0.5f;

	// Token: 0x04000608 RID: 1544
	private Dictionary<PlayerBodyV2, VisualElement> playerBodyVisualElementMap = new Dictionary<PlayerBodyV2, VisualElement>();
}
