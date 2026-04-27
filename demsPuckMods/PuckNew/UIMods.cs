using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x020001A1 RID: 417
public class UIMods : UIView
{
	// Token: 0x06000BFA RID: 3066 RVA: 0x000389BC File Offset: 0x00036BBC
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("ModsView", null);
		this.mods = base.View.Query("Mods", null);
		this.modsList = this.mods.Query("ModsList", null);
		this.noMods = this.mods.Query("NoMods", null);
		this.closeIconButton = this.mods.Query("CloseIconButtonContainer", null).First().Query(null, null);
		this.closeIconButton.clicked += this.OnClickClose;
		this.findModsButton = this.mods.Query("FindModsButton", null);
		this.findModsButton.clicked += this.OnClickFindMods;
		this.refreshButton = this.mods.Query("RefreshButton", null);
		this.refreshButton.clicked += this.OnClickRefresh;
		this.modsList.Clear();
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x00038AE7 File Offset: 0x00036CE7
	public override bool Show()
	{
		bool flag = base.Show();
		if (flag)
		{
			EventManager.TriggerEvent("Event_OnModsShow", null);
		}
		return flag;
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x00038B00 File Offset: 0x00036D00
	public void AddMod(Mod mod)
	{
		if (this.modVisualElementMap.ContainsKey(mod))
		{
			return;
		}
		VisualElement visualElement = this.modAsset.Instantiate();
		visualElement.Query("EnableButton", null).clicked += delegate()
		{
			this.OnClickModEnable(mod);
		};
		visualElement.Query("DisableButton", null).clicked += delegate()
		{
			this.OnClickModDisable(mod);
		};
		this.modVisualElementMap.Add(mod, visualElement);
		this.modsList.Add(visualElement);
		this.UpdateMod(mod);
		this.UpdateNoMods();
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x00038BB8 File Offset: 0x00036DB8
	public void UpdateMod(Mod mod)
	{
		if (!this.modVisualElementMap.ContainsKey(mod))
		{
			return;
		}
		InstalledItem installedItem = mod.InstalledItem;
		ItemDetails itemDetails = installedItem.ItemDetails;
		VisualElement e = this.modVisualElementMap[mod];
		e.Query("TitleLabel", null).text = ((itemDetails != null) ? itemDetails.Title : installedItem.Id.ToString());
		Label label = e.Query("DescriptionLabel", null);
		if (itemDetails != null && !string.IsNullOrEmpty(itemDetails.Description))
		{
			label.text = ((itemDetails.Description.Length > 256) ? (itemDetails.Description.Substring(0, 256) + "...") : itemDetails.Description);
		}
		VisualElement visualElement = e.Query("Preview", null);
		if (mod.PreviewTexture != null)
		{
			visualElement.style.backgroundImage = Background.FromTexture2D(mod.PreviewTexture);
		}
		e.Query("EnableButton", null).style.display = (mod.IsAssemblyMod ? ((!mod.IsEnabled) ? DisplayStyle.Flex : DisplayStyle.None) : DisplayStyle.None);
		e.Query("DisableButton", null).style.display = (mod.IsAssemblyMod ? (mod.IsEnabled ? DisplayStyle.Flex : DisplayStyle.None) : DisplayStyle.None);
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x00038D2C File Offset: 0x00036F2C
	public void RemoveMod(Mod mod)
	{
		if (!this.modVisualElementMap.ContainsKey(mod))
		{
			return;
		}
		VisualElement element = this.modVisualElementMap[mod];
		this.modsList.Remove(element);
		this.modVisualElementMap.Remove(mod);
		this.UpdateNoMods();
	}

	// Token: 0x06000BFF RID: 3071 RVA: 0x00038D74 File Offset: 0x00036F74
	private void UpdateNoMods()
	{
		this.noMods.style.display = ((this.modVisualElementMap.Count > 0) ? DisplayStyle.None : DisplayStyle.Flex);
	}

	// Token: 0x06000C00 RID: 3072 RVA: 0x00038D9D File Offset: 0x00036F9D
	private void OnClickClose()
	{
		EventManager.TriggerEvent("Event_OnModsClickClose", null);
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x00038DAA File Offset: 0x00036FAA
	private void OnClickFindMods()
	{
		EventManager.TriggerEvent("Event_OnModsClickFindMods", null);
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x00038DB7 File Offset: 0x00036FB7
	private void OnClickRefresh()
	{
		EventManager.TriggerEvent("Event_OnModsClickRefresh", null);
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x00038DC4 File Offset: 0x00036FC4
	private void OnClickModEnable(Mod mod)
	{
		EventManager.TriggerEvent("Event_OnModClickEnable", new Dictionary<string, object>
		{
			{
				"mod",
				mod
			}
		});
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x00038DE1 File Offset: 0x00036FE1
	private void OnClickModDisable(Mod mod)
	{
		EventManager.TriggerEvent("Event_OnModClickDisable", new Dictionary<string, object>
		{
			{
				"mod",
				mod
			}
		});
	}

	// Token: 0x0400070B RID: 1803
	[Header("References")]
	public VisualTreeAsset modAsset;

	// Token: 0x0400070C RID: 1804
	private VisualElement mods;

	// Token: 0x0400070D RID: 1805
	private VisualElement modsList;

	// Token: 0x0400070E RID: 1806
	private VisualElement noMods;

	// Token: 0x0400070F RID: 1807
	private IconButton closeIconButton;

	// Token: 0x04000710 RID: 1808
	private Button findModsButton;

	// Token: 0x04000711 RID: 1809
	private Button refreshButton;

	// Token: 0x04000712 RID: 1810
	private Dictionary<Mod, VisualElement> modVisualElementMap = new Dictionary<Mod, VisualElement>();
}
