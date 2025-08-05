using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000114 RID: 276
public class UIMods : UIComponent<UIMods>
{
	// Token: 0x060009CC RID: 2508 RVA: 0x0000D3BE File Offset: 0x0000B5BE
	private void Start()
	{
		base.VisibilityRequiresMouse = true;
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x0003AC34 File Offset: 0x00038E34
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("ModsContainer", null);
		this.scrollView = this.container.Query("ScrollView", null);
		this.noModsContainer = this.container.Query("NoModsContainer", null);
		this.closeButton = this.container.Query("CloseButton", null);
		this.closeButton.clicked += this.OnClickClose;
		this.findModsButton = this.container.Query("FindModsButton", null);
		this.findModsButton.clicked += this.OnClickFindMods;
		this.refreshButton = this.container.Query("RefreshButton", null);
		this.refreshButton.clicked += this.OnClickRefresh;
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x0000D3C7 File Offset: 0x0000B5C7
	public override void Show()
	{
		base.Show();
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnModsShow", null);
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x0003AD2C File Offset: 0x00038F2C
	public void AddMod(Mod mod)
	{
		if (this.modVisualElementMap.ContainsKey(mod))
		{
			return;
		}
		VisualElement visualElement = Utils.InstantiateVisualTreeAsset(this.modAsset, Position.Relative);
		visualElement.Query("EnableButton", null).clicked += delegate()
		{
			this.OnClickEnable(mod);
		};
		visualElement.Query("DisableButton", null).clicked += delegate()
		{
			this.OnClickDisable(mod);
		};
		this.modVisualElementMap.Add(mod, visualElement);
		this.scrollView.contentContainer.Add(visualElement);
		this.UpdateMod(mod);
		this.UpdateNoModsContainer();
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x0003ADE8 File Offset: 0x00038FE8
	public void UpdateMod(Mod mod)
	{
		if (!this.modVisualElementMap.ContainsKey(mod))
		{
			return;
		}
		InstalledItem installedItem = mod.InstalledItem;
		ItemDetails itemDetails = installedItem.ItemDetails;
		VisualElement e = this.modVisualElementMap[mod];
		VisualElement visualElement = e.Query("PreviewVisualElement", null);
		if (mod.PreviewTexture != null)
		{
			visualElement.style.backgroundImage = Background.FromTexture2D(mod.PreviewTexture);
		}
		e.Query("TitleLabel", null).text = ((itemDetails != null) ? itemDetails.Title : installedItem.Id.ToString());
		TextElement textElement = e.Query("DescriptionLabel", null);
		string text;
		if (itemDetails == null)
		{
			text = "";
		}
		else
		{
			string description = itemDetails.Description;
			text = ((description != null && description.Length > 128) ? (itemDetails.Description.Substring(0, 256) + "...") : itemDetails.Description);
		}
		textElement.text = text;
		e.Query("EnableButton", null).style.display = ((mod.IsAssemblyMod && !mod.IsEnabled) ? DisplayStyle.Flex : DisplayStyle.None);
		e.Query("DisableButton", null).style.display = ((mod.IsAssemblyMod && mod.IsEnabled) ? DisplayStyle.Flex : DisplayStyle.None);
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x0003AF50 File Offset: 0x00039150
	public void RemoveMod(Mod mod)
	{
		if (!this.modVisualElementMap.ContainsKey(mod))
		{
			return;
		}
		VisualElement element = this.modVisualElementMap[mod];
		this.scrollView.contentContainer.Remove(element);
		this.modVisualElementMap.Remove(mod);
		this.UpdateNoModsContainer();
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x0000D3DF File Offset: 0x0000B5DF
	public void ClearMods()
	{
		this.scrollView.contentContainer.Clear();
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x0000D3F1 File Offset: 0x0000B5F1
	private void UpdateNoModsContainer()
	{
		this.noModsContainer.style.display = ((this.modVisualElementMap.Count > 0) ? DisplayStyle.None : DisplayStyle.Flex);
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x0000D41A File Offset: 0x0000B61A
	private void OnClickClose()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnModsClickClose", null);
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x0000D42C File Offset: 0x0000B62C
	private void OnClickFindMods()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnModsClickFindMods", null);
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x0000D43E File Offset: 0x0000B63E
	private void OnClickRefresh()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnModsClickRefresh", null);
	}

	// Token: 0x060009D7 RID: 2519 RVA: 0x0000D450 File Offset: 0x0000B650
	private void OnClickEnable(Mod mod)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnModClickEnable", new Dictionary<string, object>
		{
			{
				"mod",
				mod
			}
		});
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x0000D472 File Offset: 0x0000B672
	private void OnClickDisable(Mod mod)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnModClickDisable", new Dictionary<string, object>
		{
			{
				"mod",
				mod
			}
		});
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x0003AFA0 File Offset: 0x000391A0
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060009DB RID: 2523 RVA: 0x0000D4A7 File Offset: 0x0000B6A7
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x0000D4B1 File Offset: 0x0000B6B1
	protected internal override string __getTypeName()
	{
		return "UIMods";
	}

	// Token: 0x040005D5 RID: 1493
	[Header("Components")]
	public VisualTreeAsset modAsset;

	// Token: 0x040005D6 RID: 1494
	private ScrollView scrollView;

	// Token: 0x040005D7 RID: 1495
	private VisualElement noModsContainer;

	// Token: 0x040005D8 RID: 1496
	private Button closeButton;

	// Token: 0x040005D9 RID: 1497
	private Button findModsButton;

	// Token: 0x040005DA RID: 1498
	private Button refreshButton;

	// Token: 0x040005DB RID: 1499
	private Dictionary<Mod, VisualElement> modVisualElementMap = new Dictionary<Mod, VisualElement>();
}
