using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000171 RID: 369
public class UIAppearance : UIView
{
	// Token: 0x06000AA4 RID: 2724 RVA: 0x000325F4 File Offset: 0x000307F4
	public void Initialize(VisualElement rootVisualElement)
	{
		this.ValidateAppearanceItems();
		base.View = rootVisualElement.Query("AppearanceView", null);
		this.appearance = base.View.Query("Appearance", null);
		this.closeIconButton = this.appearance.Query("CloseIconButtonContainer", null).First().Query(null, null);
		this.closeIconButton.clicked += this.OnClickClose;
		this.categoryTabView = this.appearance.Query(null, null);
		this.categoryTabView.activeTabChanged += this.OnCategoryTabChanged;
		this.headTab = this.categoryTabView.Query("HeadTab", null);
		this.headTabView = this.headTab.Query(null, null);
		this.headTabView.activeTabChanged += this.OnSubcategoryTabChanged;
		this.flagsTab = this.headTabView.Query("FlagsTab", null);
		this.flagsRadioButtonGroup = this.flagsTab.Query("AppearanceItemRadioButtonGroup", null);
		this.headgearTab = this.headTabView.Query("HeadgearTab", null);
		this.headgearRadioButtonGroup = this.headgearTab.Query("AppearanceItemRadioButtonGroup", null);
		this.mustachesTab = this.headTabView.Query("MustachesTab", null);
		this.mustachesRadioButtonGroup = this.mustachesTab.Query("AppearanceItemRadioButtonGroup", null);
		this.beardsTab = this.headTabView.Query("BeardsTab", null);
		this.beardsRadioButtonGroup = this.beardsTab.Query("AppearanceItemRadioButtonGroup", null);
		this.bodyTab = this.categoryTabView.Query("BodyTab", null);
		this.bodyTabView = this.bodyTab.Query(null, null);
		this.bodyTabView.activeTabChanged += this.OnSubcategoryTabChanged;
		this.jerseysTab = this.bodyTabView.Query("JerseysTab", null);
		this.jerseysRadioButtonGroup = this.jerseysTab.Query("AppearanceItemRadioButtonGroup", null);
		this.stickTab = this.categoryTabView.Query("StickTab", null);
		this.stickTabView = this.stickTab.Query(null, null);
		this.stickTabView.activeTabChanged += this.OnSubcategoryTabChanged;
		this.stickSkinsTab = this.stickTabView.Query("SkinsTab", null);
		this.stickSkinsRadioButtonGroup = this.stickSkinsTab.Query("AppearanceItemRadioButtonGroup", null);
		this.stickShaftTapesTab = this.stickTabView.Query("ShaftTapesTab", null);
		this.stickShaftTapesRadioButtonGroup = this.stickShaftTapesTab.Query("AppearanceItemRadioButtonGroup", null);
		this.stickBladeTapesTab = this.stickTabView.Query("BladeTapesTab", null);
		this.stickBladeTapesRadioButtonGroup = this.stickBladeTapesTab.Query("AppearanceItemRadioButtonGroup", null);
		this.teamDropdown = this.appearance.Query("TeamInput", null).First().Query(null, null);
		this.teamDropdown.choices = Utils.GetTeamNames();
		this.teamDropdown.value = Utils.GetNameFromTeam(SettingsManager.Team);
		this.teamDropdown.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnTeamDropdownChanged));
		this.roleDropdown = this.appearance.Query("RoleInput", null).First().Query(null, null);
		this.roleDropdown.choices = Utils.GetRoleNames();
		this.roleDropdown.value = Utils.GetNameFromRole(SettingsManager.Role);
		this.roleDropdown.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnRoleDropdownChanged));
		this.applyForBothTeamsToggle = this.appearance.Query("ApplyForBothTeamsInput", null).First().Query(null, null);
		this.applyForBothTeamsToggle.value = SettingsManager.ApplyForBothTeams;
		this.applyForBothTeamsToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnApplyForBothTeamsToggleChanged));
		this.categoryTabView.activeTab = this.headTab;
		this.headTabView.activeTab = this.flagsTab;
		this.PopulateRadioButtonGroups();
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x00032A98 File Offset: 0x00030C98
	public override bool Show()
	{
		bool flag = base.Show();
		if (flag)
		{
			EventManager.TriggerEvent("Event_OnAppearanceShow", new Dictionary<string, object>
			{
				{
					"category",
					this.category
				},
				{
					"subcategory",
					this.categorySubcategoryMap[this.category]
				}
			});
		}
		return flag;
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x00032AF4 File Offset: 0x00030CF4
	public override bool Hide()
	{
		bool flag = base.Hide();
		if (flag)
		{
			EventManager.TriggerEvent("Event_OnAppearanceHide", null);
		}
		return flag;
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x00032B0A File Offset: 0x00030D0A
	public void SetTeam(PlayerTeam value)
	{
		this.team = value;
		this.StyleRadioButtonGroups();
		this.UpdateRadioButtons();
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x00032B1F File Offset: 0x00030D1F
	public void SetRole(PlayerRole value)
	{
		this.role = value;
		this.StyleRadioButtonGroups();
		this.UpdateRadioButtons();
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x00032B34 File Offset: 0x00030D34
	public void SetApplyForBothTeams(bool value)
	{
		this.applyForBothTeams = value;
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x00032B3D File Offset: 0x00030D3D
	public void SetFlagID(int value)
	{
		this.flagID = value;
		this.UpdateFlagsRadioButtons();
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x00032B4C File Offset: 0x00030D4C
	public void SetHeadgearID(PlayerTeam team, PlayerRole role, int value)
	{
		if (team == PlayerTeam.Blue && role == PlayerRole.Attacker)
		{
			this.headgearIDBlueAttacker = value;
		}
		else if (team == PlayerTeam.Red && role == PlayerRole.Attacker)
		{
			this.headgearIDRedAttacker = value;
		}
		else if (team == PlayerTeam.Blue && role == PlayerRole.Goalie)
		{
			this.headgearIDBlueGoalie = value;
		}
		else if (team == PlayerTeam.Red && role == PlayerRole.Goalie)
		{
			this.headgearIDRedGoalie = value;
		}
		this.UpdateHeadgearRadioButtons();
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x00032BA1 File Offset: 0x00030DA1
	public void SetMustacheID(int value)
	{
		this.mustacheID = value;
		this.UpdateMustacheRadioButtons();
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x00032BB0 File Offset: 0x00030DB0
	public void SetBeardID(int value)
	{
		this.beardID = value;
		this.UpdateBeardRadioButtons();
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x00032BC0 File Offset: 0x00030DC0
	public void SetJerseyID(PlayerTeam team, PlayerRole role, int value)
	{
		if (team == PlayerTeam.Blue && role == PlayerRole.Attacker)
		{
			this.jerseyIDBlueAttacker = value;
		}
		else if (team == PlayerTeam.Red && role == PlayerRole.Attacker)
		{
			this.jerseyIDRedAttacker = value;
		}
		else if (team == PlayerTeam.Blue && role == PlayerRole.Goalie)
		{
			this.jerseyIDBlueGoalie = value;
		}
		else if (team == PlayerTeam.Red && role == PlayerRole.Goalie)
		{
			this.jerseyIDRedGoalie = value;
		}
		this.UpdateJerseyRadioButtons();
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x00032C18 File Offset: 0x00030E18
	public void SetStickSkinID(PlayerTeam team, PlayerRole role, int value)
	{
		if (team == PlayerTeam.Blue && role == PlayerRole.Attacker)
		{
			this.stickSkinIDBlueAttacker = value;
		}
		else if (team == PlayerTeam.Red && role == PlayerRole.Attacker)
		{
			this.stickSkinIDRedAttacker = value;
		}
		else if (team == PlayerTeam.Blue && role == PlayerRole.Goalie)
		{
			this.stickSkinIDBlueGoalie = value;
		}
		else if (team == PlayerTeam.Red && role == PlayerRole.Goalie)
		{
			this.stickSkinIDRedGoalie = value;
		}
		this.UpdateStickSkinRadioButtons();
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x00032C70 File Offset: 0x00030E70
	public void SetStickShaftTapeID(PlayerTeam team, PlayerRole role, int value)
	{
		if (team == PlayerTeam.Blue && role == PlayerRole.Attacker)
		{
			this.stickShaftTapeIDBlueAttacker = value;
		}
		else if (team == PlayerTeam.Red && role == PlayerRole.Attacker)
		{
			this.stickShaftTapeIDRedAttacker = value;
		}
		else if (team == PlayerTeam.Blue && role == PlayerRole.Goalie)
		{
			this.stickShaftTapeIDBlueGoalie = value;
		}
		else if (team == PlayerTeam.Red && role == PlayerRole.Goalie)
		{
			this.stickShaftTapeIDRedGoalie = value;
		}
		this.UpdateStickShaftTapeRadioButtons();
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x00032CC8 File Offset: 0x00030EC8
	public void SetStickBladeTapeID(PlayerTeam team, PlayerRole role, int value)
	{
		if (team == PlayerTeam.Blue && role == PlayerRole.Attacker)
		{
			this.stickBladeTapeIDBlueAttacker = value;
		}
		else if (team == PlayerTeam.Red && role == PlayerRole.Attacker)
		{
			this.stickBladeTapeIDRedAttacker = value;
		}
		else if (team == PlayerTeam.Blue && role == PlayerRole.Goalie)
		{
			this.stickBladeTapeIDBlueGoalie = value;
		}
		else if (team == PlayerTeam.Red && role == PlayerRole.Goalie)
		{
			this.stickBladeTapeIDRedGoalie = value;
		}
		this.UpdateStickBladeTapeRadioButtons();
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x00032D20 File Offset: 0x00030F20
	public void StyleRadioButtonGroups()
	{
		this.StyleRadioButtonGroup(this.flagsRadioButtonGroup);
		this.StyleRadioButtonGroup(this.headgearRadioButtonGroup);
		this.StyleRadioButtonGroup(this.mustachesRadioButtonGroup);
		this.StyleRadioButtonGroup(this.beardsRadioButtonGroup);
		this.StyleRadioButtonGroup(this.jerseysRadioButtonGroup);
		this.StyleRadioButtonGroup(this.stickSkinsRadioButtonGroup);
		this.StyleRadioButtonGroup(this.stickShaftTapesRadioButtonGroup);
		this.StyleRadioButtonGroup(this.stickBladeTapesRadioButtonGroup);
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x00032D8D File Offset: 0x00030F8D
	public void UpdateRadioButtons()
	{
		this.UpdateFlagsRadioButtons();
		this.UpdateHeadgearRadioButtons();
		this.UpdateMustacheRadioButtons();
		this.UpdateBeardRadioButtons();
		this.UpdateJerseyRadioButtons();
		this.UpdateStickSkinRadioButtons();
		this.UpdateStickShaftTapeRadioButtons();
		this.UpdateStickBladeTapeRadioButtons();
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x00032DC0 File Offset: 0x00030FC0
	private void ValidateAppearanceItems()
	{
		ItemManager.GetItemsByCategories(new string[]
		{
			"flag"
		}).ForEach(delegate(Item item)
		{
			if (!this.flags.Any((AppearanceItem appearanceItem) => appearanceItem.Id == item.id))
			{
				Debug.LogWarning(string.Format("[UIAppearance] Flag item {0} ({1}) is missing from the appearance flags list", item.name, item.id));
			}
		});
		ItemManager.GetItemsByCategories(new string[]
		{
			"headgear"
		}).ForEach(delegate(Item item)
		{
			if (!this.headgear.Any((AppearanceItem appearanceItem) => appearanceItem.Id == item.id))
			{
				Debug.LogWarning(string.Format("[UIAppearance] Headgear item {0} ({1}) is missing from the appearance headgear list", item.name, item.id));
			}
		});
		ItemManager.GetItemsByCategories(new string[]
		{
			"mustache"
		}).ForEach(delegate(Item item)
		{
			if (!this.mustaches.Any((AppearanceItem appearanceItem) => appearanceItem.Id == item.id))
			{
				Debug.LogWarning(string.Format("[UIAppearance] Mustache item {0} ({1}) is missing from the appearance mustaches list", item.name, item.id));
			}
		});
		ItemManager.GetItemsByCategories(new string[]
		{
			"beard"
		}).ForEach(delegate(Item item)
		{
			if (!this.beards.Any((AppearanceItem appearanceItem) => appearanceItem.Id == item.id))
			{
				Debug.LogWarning(string.Format("[UIAppearance] Beard item {0} ({1}) is missing from the appearance beards list", item.name, item.id));
			}
		});
		ItemManager.GetItemsByCategories(new string[]
		{
			"jersey"
		}).ForEach(delegate(Item item)
		{
			if (!this.jerseys.Any((AppearanceItem appearanceItem) => appearanceItem.Id == item.id))
			{
				Debug.LogWarning(string.Format("[UIAppearance] Jersey item {0} ({1}) is missing from the appearance jerseys list", item.name, item.id));
			}
		});
		ItemManager.GetItemsByCategories(new string[]
		{
			"stickSkin"
		}).ForEach(delegate(Item item)
		{
			if (!this.stickSkins.Any((AppearanceItem appearanceItem) => appearanceItem.Id == item.id))
			{
				Debug.LogWarning(string.Format("[UIAppearance] Stick skin item {0} ({1}) is missing from the appearance stick skins list", item.name, item.id));
			}
		});
		ItemManager.GetItemsByCategories(new string[]
		{
			"stickShaftTape"
		}).ForEach(delegate(Item item)
		{
			if (!this.stickShaftTapes.Any((AppearanceItem appearanceItem) => appearanceItem.Id == item.id))
			{
				Debug.LogWarning(string.Format("[UIAppearance] Stick shaft tape item {0} ({1}) is missing from the appearance stick shaft tapes list", item.name, item.id));
			}
		});
		ItemManager.GetItemsByCategories(new string[]
		{
			"stickBladeTape"
		}).ForEach(delegate(Item item)
		{
			if (!this.stickBladeTapes.Any((AppearanceItem appearanceItem) => appearanceItem.Id == item.id))
			{
				Debug.LogWarning(string.Format("[UIAppearance] Stick blade tape item {0} ({1}) is missing from the appearance stick blade tapes list", item.name, item.id));
			}
		});
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x00032EF0 File Offset: 0x000310F0
	private void PopulateRadioButtonGroups()
	{
		this.PopulateRadioButtonGroup(this.flagsRadioButtonGroup, this.flags);
		this.PopulateRadioButtonGroup(this.headgearRadioButtonGroup, this.headgear);
		this.PopulateRadioButtonGroup(this.mustachesRadioButtonGroup, this.mustaches);
		this.PopulateRadioButtonGroup(this.beardsRadioButtonGroup, this.beards);
		this.PopulateRadioButtonGroup(this.jerseysRadioButtonGroup, this.jerseys);
		this.PopulateRadioButtonGroup(this.stickSkinsRadioButtonGroup, this.stickSkins);
		this.PopulateRadioButtonGroup(this.stickShaftTapesRadioButtonGroup, this.stickShaftTapes);
		this.PopulateRadioButtonGroup(this.stickBladeTapesRadioButtonGroup, this.stickBladeTapes);
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x00032F90 File Offset: 0x00031190
	private void PopulateRadioButtonGroup(RadioButtonGroup radioButtonGroup, List<AppearanceItem> appearanceItems)
	{
		VisualElement visualElement = radioButtonGroup.Query("AppearanceItemList", null);
		visualElement.Clear();
		foreach (AppearanceItem appearanceItem in appearanceItems)
		{
			RadioButton radioButton = this.appearanceItemAsset.Instantiate().Query("AppearanceItemRadioButton", null);
			Button button = radioButton.Query("PurchaseButton", null).First().Query(null, null);
			Item item;
			if (appearanceItem.Id == -1)
			{
				item = new Item
				{
					id = -1,
					name = "NONE"
				};
			}
			else
			{
				item = ItemManager.GetItemById(appearanceItem.Id);
				if (item == null)
				{
					Debug.LogError(string.Format("[UIAppearance] Could not populate appearance item with ID {0} because the item was not found in ItemManager", appearanceItem.Id));
					continue;
				}
				button.RegisterCallback<ClickEvent>(delegate(ClickEvent _)
				{
					this.OnClickPurchase(item);
				}, TrickleDown.NoTrickleDown);
				button.text = "BUY $" + ((float)item.price / 100f).ToString("F2", CultureInfo.InvariantCulture);
			}
			radioButton.label = item.name.ToUpper();
			radioButton.userData = new Dictionary<string, object>
			{
				{
					"item",
					item
				}
			};
			radioButton.RegisterCallback<ClickEvent>(delegate(ClickEvent _)
			{
				this.OnClickAppearanceItem(item);
			}, TrickleDown.NoTrickleDown);
			visualElement.Add(radioButton);
		}
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x00033150 File Offset: 0x00031350
	private void StyleRadioButtonGroup(RadioButtonGroup radioButtonGroup)
	{
		VisualElement visualElement = radioButtonGroup.Query("AppearanceItemList", null);
		visualElement.hierarchy.Sort(delegate(VisualElement a, VisualElement b)
		{
			Item item = (a.userData as Dictionary<string, object>)["item"] as Item;
			Item item2 = (b.userData as Dictionary<string, object>)["item"] as Item;
			if ((item.IsOwned && !item2.IsOwned) || item.id == -1)
			{
				return -1;
			}
			if ((!item.IsOwned && item2.IsOwned) || item2.id == -1)
			{
				return 1;
			}
			return string.Compare(item.name, item2.name, StringComparison.OrdinalIgnoreCase);
		});
		foreach (RadioButton radioButton in visualElement.Query(null, null).ToList())
		{
			this.StyleRadioButton(radioButton);
		}
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x000331EC File Offset: 0x000313EC
	private void StyleRadioButton(RadioButton radioButton)
	{
		Item item = (radioButton.userData as Dictionary<string, object>)["item"] as Item;
		bool flag = (this.role == PlayerRole.Attacker) ? item.IsAttackerItem : item.IsGoalieItem;
		bool flag2 = (item.IsUnlisted && !item.IsPurchased) || !flag;
		radioButton.EnableInClassList("owned", item.IsOwned);
		radioButton.style.display = (flag2 ? DisplayStyle.None : DisplayStyle.Flex);
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x0003326C File Offset: 0x0003146C
	private void UpdateFlagsRadioButtons()
	{
		List<RadioButton> radioButtons = this.flagsRadioButtonGroup.Query(null, null).ToList();
		radioButtons.ForEach(delegate(RadioButton radioButton)
		{
			if (((radioButton.userData as Dictionary<string, object>)["item"] as Item).id == this.flagID)
			{
				this.flagsRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
			}
		});
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x000332B8 File Offset: 0x000314B8
	private void UpdateHeadgearRadioButtons()
	{
		List<RadioButton> radioButtons = this.headgearRadioButtonGroup.Query(null, null).ToList();
		radioButtons.ForEach(delegate(RadioButton radioButton)
		{
			Item item = (radioButton.userData as Dictionary<string, object>)["item"] as Item;
			if (this.team == PlayerTeam.Blue && this.role == PlayerRole.Attacker && item.id == this.headgearIDBlueAttacker)
			{
				this.headgearRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
				return;
			}
			if (this.team == PlayerTeam.Red && this.role == PlayerRole.Attacker && item.id == this.headgearIDRedAttacker)
			{
				this.headgearRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
				return;
			}
			if (this.team == PlayerTeam.Blue && this.role == PlayerRole.Goalie && item.id == this.headgearIDBlueGoalie)
			{
				this.headgearRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
				return;
			}
			if (this.team == PlayerTeam.Red && this.role == PlayerRole.Goalie && item.id == this.headgearIDRedGoalie)
			{
				this.headgearRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
			}
		});
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x00033304 File Offset: 0x00031504
	private void UpdateMustacheRadioButtons()
	{
		List<RadioButton> radioButtons = this.mustachesRadioButtonGroup.Query(null, null).ToList();
		radioButtons.ForEach(delegate(RadioButton radioButton)
		{
			if (((radioButton.userData as Dictionary<string, object>)["item"] as Item).id == this.mustacheID)
			{
				this.mustachesRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
			}
		});
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x00033350 File Offset: 0x00031550
	private void UpdateBeardRadioButtons()
	{
		List<RadioButton> radioButtons = this.beardsRadioButtonGroup.Query(null, null).ToList();
		radioButtons.ForEach(delegate(RadioButton radioButton)
		{
			if (((radioButton.userData as Dictionary<string, object>)["item"] as Item).id == this.beardID)
			{
				this.beardsRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
			}
		});
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x0003339C File Offset: 0x0003159C
	private void UpdateJerseyRadioButtons()
	{
		List<RadioButton> radioButtons = this.jerseysRadioButtonGroup.Query(null, null).ToList();
		radioButtons.ForEach(delegate(RadioButton radioButton)
		{
			Item item = (radioButton.userData as Dictionary<string, object>)["item"] as Item;
			if (this.team == PlayerTeam.Blue && this.role == PlayerRole.Attacker && item.id == this.jerseyIDBlueAttacker)
			{
				this.jerseysRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
				return;
			}
			if (this.team == PlayerTeam.Red && this.role == PlayerRole.Attacker && item.id == this.jerseyIDRedAttacker)
			{
				this.jerseysRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
				return;
			}
			if (this.team == PlayerTeam.Blue && this.role == PlayerRole.Goalie && item.id == this.jerseyIDBlueGoalie)
			{
				this.jerseysRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
				return;
			}
			if (this.team == PlayerTeam.Red && this.role == PlayerRole.Goalie && item.id == this.jerseyIDRedGoalie)
			{
				this.jerseysRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
			}
		});
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x000333E8 File Offset: 0x000315E8
	private void UpdateStickSkinRadioButtons()
	{
		List<RadioButton> radioButtons = this.stickSkinsRadioButtonGroup.Query(null, null).ToList();
		radioButtons.ForEach(delegate(RadioButton radioButton)
		{
			Item item = (radioButton.userData as Dictionary<string, object>)["item"] as Item;
			if (this.team == PlayerTeam.Blue && this.role == PlayerRole.Attacker && item.id == this.stickSkinIDBlueAttacker)
			{
				this.stickSkinsRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
				return;
			}
			if (this.team == PlayerTeam.Red && this.role == PlayerRole.Attacker && item.id == this.stickSkinIDRedAttacker)
			{
				this.stickSkinsRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
				return;
			}
			if (this.team == PlayerTeam.Blue && this.role == PlayerRole.Goalie && item.id == this.stickSkinIDBlueGoalie)
			{
				this.stickSkinsRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
				return;
			}
			if (this.team == PlayerTeam.Red && this.role == PlayerRole.Goalie && item.id == this.stickSkinIDRedGoalie)
			{
				this.stickSkinsRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
			}
		});
	}

	// Token: 0x06000ABF RID: 2751 RVA: 0x00033434 File Offset: 0x00031634
	private void UpdateStickShaftTapeRadioButtons()
	{
		List<RadioButton> radioButtons = this.stickShaftTapesRadioButtonGroup.Query(null, null).ToList();
		radioButtons.ForEach(delegate(RadioButton radioButton)
		{
			Item item = (radioButton.userData as Dictionary<string, object>)["item"] as Item;
			if (this.team == PlayerTeam.Blue && this.role == PlayerRole.Attacker && item.id == this.stickShaftTapeIDBlueAttacker)
			{
				this.stickShaftTapesRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
				return;
			}
			if (this.team == PlayerTeam.Red && this.role == PlayerRole.Attacker && item.id == this.stickShaftTapeIDRedAttacker)
			{
				this.stickShaftTapesRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
				return;
			}
			if (this.team == PlayerTeam.Blue && this.role == PlayerRole.Goalie && item.id == this.stickShaftTapeIDBlueGoalie)
			{
				this.stickShaftTapesRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
				return;
			}
			if (this.team == PlayerTeam.Red && this.role == PlayerRole.Goalie && item.id == this.stickShaftTapeIDRedGoalie)
			{
				this.stickShaftTapesRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
			}
		});
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x00033480 File Offset: 0x00031680
	private void UpdateStickBladeTapeRadioButtons()
	{
		List<RadioButton> radioButtons = this.stickBladeTapesRadioButtonGroup.Query(null, null).ToList();
		radioButtons.ForEach(delegate(RadioButton radioButton)
		{
			Item item = (radioButton.userData as Dictionary<string, object>)["item"] as Item;
			if (this.team == PlayerTeam.Blue && this.role == PlayerRole.Attacker && item.id == this.stickBladeTapeIDBlueAttacker)
			{
				this.stickBladeTapesRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
				return;
			}
			if (this.team == PlayerTeam.Red && this.role == PlayerRole.Attacker && item.id == this.stickBladeTapeIDRedAttacker)
			{
				this.stickBladeTapesRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
				return;
			}
			if (this.team == PlayerTeam.Blue && this.role == PlayerRole.Goalie && item.id == this.stickBladeTapeIDBlueGoalie)
			{
				this.stickBladeTapesRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
				return;
			}
			if (this.team == PlayerTeam.Red && this.role == PlayerRole.Goalie && item.id == this.stickBladeTapeIDRedGoalie)
			{
				this.stickBladeTapesRadioButtonGroup.value = radioButtons.IndexOf(radioButton);
			}
		});
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x000334CC File Offset: 0x000316CC
	private void OnClickClose()
	{
		EventManager.TriggerEvent("Event_OnAppearanceClickClose", null);
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x000334D9 File Offset: 0x000316D9
	private void OnClickPurchase(Item item)
	{
		EventManager.TriggerEvent("Event_OnAppearanceClickPurchaseItem", new Dictionary<string, object>
		{
			{
				"item",
				item
			}
		});
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x000334F6 File Offset: 0x000316F6
	private void OnTeamDropdownChanged(ChangeEvent<string> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnAppearanceTeamChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x00033518 File Offset: 0x00031718
	private void OnRoleDropdownChanged(ChangeEvent<string> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnAppearanceRoleChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x0003353A File Offset: 0x0003173A
	private void OnApplyForBothTeamsToggleChanged(ChangeEvent<bool> changeEvent)
	{
		EventManager.TriggerEvent("Event_OnAppearanceApplyForBothTeamsChanged", new Dictionary<string, object>
		{
			{
				"value",
				changeEvent.newValue
			}
		});
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x00033564 File Offset: 0x00031764
	private void OnCategoryTabChanged(Tab oldTab, Tab newTab)
	{
		string name = newTab.name;
		if (!(name == "HeadTab"))
		{
			if (!(name == "BodyTab"))
			{
				if (name == "StickTab")
				{
					this.category = AppearanceCategory.Stick;
				}
			}
			else
			{
				this.category = AppearanceCategory.Body;
			}
		}
		else
		{
			this.category = AppearanceCategory.Head;
		}
		EventManager.TriggerEvent("Event_OnAppearanceCategoryChanged", new Dictionary<string, object>
		{
			{
				"category",
				this.category
			},
			{
				"subcategory",
				this.categorySubcategoryMap[this.category]
			}
		});
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x00033600 File Offset: 0x00031800
	private void OnSubcategoryTabChanged(Tab oldTab, Tab newTab)
	{
		Dictionary<AppearanceCategory, AppearanceSubcategory> dictionary = this.categorySubcategoryMap;
		AppearanceCategory key = this.category;
		string name = newTab.name;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
		AppearanceSubcategory value;
		if (num <= 2933584645U)
		{
			if (num <= 154799819U)
			{
				if (num != 108854953U)
				{
					if (num == 154799819U)
					{
						if (name == "FlagsTab")
						{
							value = AppearanceSubcategory.Flags;
							goto IL_14A;
						}
					}
				}
				else if (name == "JerseysTab")
				{
					value = AppearanceSubcategory.Jerseys;
					goto IL_14A;
				}
			}
			else if (num != 2528141619U)
			{
				if (num == 2933584645U)
				{
					if (name == "BladeTapesTab")
					{
						value = AppearanceSubcategory.StickBladeTapes;
						goto IL_14A;
					}
				}
			}
			else if (name == "ShaftTapesTab")
			{
				value = AppearanceSubcategory.StickShaftTapes;
				goto IL_14A;
			}
		}
		else if (num <= 3172671651U)
		{
			if (num != 3105626632U)
			{
				if (num == 3172671651U)
				{
					if (name == "MustachesTab")
					{
						value = AppearanceSubcategory.Mustaches;
						goto IL_14A;
					}
				}
			}
			else if (name == "SkinsTab")
			{
				value = AppearanceSubcategory.StickSkins;
				goto IL_14A;
			}
		}
		else if (num != 3753505521U)
		{
			if (num == 3900078259U)
			{
				if (name == "HeadgearTab")
				{
					value = AppearanceSubcategory.Headgear;
					goto IL_14A;
				}
			}
		}
		else if (name == "BeardsTab")
		{
			value = AppearanceSubcategory.Beards;
			goto IL_14A;
		}
		value = this.categorySubcategoryMap[this.category];
		IL_14A:
		dictionary[key] = value;
		EventManager.TriggerEvent("Event_OnAppearanceCategoryChanged", new Dictionary<string, object>
		{
			{
				"category",
				this.category
			},
			{
				"subcategory",
				this.categorySubcategoryMap[this.category]
			}
		});
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x000337A8 File Offset: 0x000319A8
	private void OnClickAppearanceItem(Item item)
	{
		EventManager.TriggerEvent("Event_OnAppearanceClickItem", new Dictionary<string, object>
		{
			{
				"item",
				item
			},
			{
				"category",
				this.category
			},
			{
				"subcategory",
				this.categorySubcategoryMap[this.category]
			},
			{
				"team",
				this.team
			},
			{
				"role",
				this.role
			}
		});
	}

	// Token: 0x04000634 RID: 1588
	[Header("Settings")]
	[SerializeField]
	private List<AppearanceItem> flags = new List<AppearanceItem>();

	// Token: 0x04000635 RID: 1589
	[SerializeField]
	private List<AppearanceItem> headgear = new List<AppearanceItem>();

	// Token: 0x04000636 RID: 1590
	[SerializeField]
	private List<AppearanceItem> mustaches = new List<AppearanceItem>();

	// Token: 0x04000637 RID: 1591
	[SerializeField]
	private List<AppearanceItem> beards = new List<AppearanceItem>();

	// Token: 0x04000638 RID: 1592
	[SerializeField]
	private List<AppearanceItem> jerseys = new List<AppearanceItem>();

	// Token: 0x04000639 RID: 1593
	[SerializeField]
	private List<AppearanceItem> stickSkins = new List<AppearanceItem>();

	// Token: 0x0400063A RID: 1594
	[SerializeField]
	private List<AppearanceItem> stickShaftTapes = new List<AppearanceItem>();

	// Token: 0x0400063B RID: 1595
	[SerializeField]
	private List<AppearanceItem> stickBladeTapes = new List<AppearanceItem>();

	// Token: 0x0400063C RID: 1596
	[Header("References")]
	public VisualTreeAsset appearanceItemAsset;

	// Token: 0x0400063D RID: 1597
	private AppearanceCategory category;

	// Token: 0x0400063E RID: 1598
	private Dictionary<AppearanceCategory, AppearanceSubcategory> categorySubcategoryMap = new Dictionary<AppearanceCategory, AppearanceSubcategory>
	{
		{
			AppearanceCategory.Head,
			AppearanceSubcategory.Flags
		},
		{
			AppearanceCategory.Body,
			AppearanceSubcategory.Jerseys
		},
		{
			AppearanceCategory.Stick,
			AppearanceSubcategory.StickSkins
		}
	};

	// Token: 0x0400063F RID: 1599
	private PlayerTeam team;

	// Token: 0x04000640 RID: 1600
	private PlayerRole role;

	// Token: 0x04000641 RID: 1601
	private bool applyForBothTeams;

	// Token: 0x04000642 RID: 1602
	private int flagID;

	// Token: 0x04000643 RID: 1603
	private int headgearIDBlueAttacker;

	// Token: 0x04000644 RID: 1604
	private int headgearIDRedAttacker;

	// Token: 0x04000645 RID: 1605
	private int headgearIDBlueGoalie;

	// Token: 0x04000646 RID: 1606
	private int headgearIDRedGoalie;

	// Token: 0x04000647 RID: 1607
	private int mustacheID;

	// Token: 0x04000648 RID: 1608
	private int beardID;

	// Token: 0x04000649 RID: 1609
	private int jerseyIDBlueAttacker;

	// Token: 0x0400064A RID: 1610
	private int jerseyIDRedAttacker;

	// Token: 0x0400064B RID: 1611
	private int jerseyIDBlueGoalie;

	// Token: 0x0400064C RID: 1612
	private int jerseyIDRedGoalie;

	// Token: 0x0400064D RID: 1613
	private int stickSkinIDBlueAttacker;

	// Token: 0x0400064E RID: 1614
	private int stickSkinIDRedAttacker;

	// Token: 0x0400064F RID: 1615
	private int stickSkinIDBlueGoalie;

	// Token: 0x04000650 RID: 1616
	private int stickSkinIDRedGoalie;

	// Token: 0x04000651 RID: 1617
	private int stickShaftTapeIDBlueAttacker;

	// Token: 0x04000652 RID: 1618
	private int stickShaftTapeIDRedAttacker;

	// Token: 0x04000653 RID: 1619
	private int stickShaftTapeIDBlueGoalie;

	// Token: 0x04000654 RID: 1620
	private int stickShaftTapeIDRedGoalie;

	// Token: 0x04000655 RID: 1621
	private int stickBladeTapeIDBlueAttacker;

	// Token: 0x04000656 RID: 1622
	private int stickBladeTapeIDRedAttacker;

	// Token: 0x04000657 RID: 1623
	private int stickBladeTapeIDBlueGoalie;

	// Token: 0x04000658 RID: 1624
	private int stickBladeTapeIDRedGoalie;

	// Token: 0x04000659 RID: 1625
	private VisualElement appearance;

	// Token: 0x0400065A RID: 1626
	private IconButton closeIconButton;

	// Token: 0x0400065B RID: 1627
	private TabView categoryTabView;

	// Token: 0x0400065C RID: 1628
	private Tab headTab;

	// Token: 0x0400065D RID: 1629
	private Tab bodyTab;

	// Token: 0x0400065E RID: 1630
	private Tab stickTab;

	// Token: 0x0400065F RID: 1631
	private TabView headTabView;

	// Token: 0x04000660 RID: 1632
	private Tab flagsTab;

	// Token: 0x04000661 RID: 1633
	private Tab headgearTab;

	// Token: 0x04000662 RID: 1634
	private Tab mustachesTab;

	// Token: 0x04000663 RID: 1635
	private Tab beardsTab;

	// Token: 0x04000664 RID: 1636
	private TabView bodyTabView;

	// Token: 0x04000665 RID: 1637
	private Tab jerseysTab;

	// Token: 0x04000666 RID: 1638
	private TabView stickTabView;

	// Token: 0x04000667 RID: 1639
	private Tab stickSkinsTab;

	// Token: 0x04000668 RID: 1640
	private Tab stickShaftTapesTab;

	// Token: 0x04000669 RID: 1641
	private Tab stickBladeTapesTab;

	// Token: 0x0400066A RID: 1642
	private Toggle applyForBothTeamsToggle;

	// Token: 0x0400066B RID: 1643
	private DropdownField teamDropdown;

	// Token: 0x0400066C RID: 1644
	private DropdownField roleDropdown;

	// Token: 0x0400066D RID: 1645
	private RadioButtonGroup flagsRadioButtonGroup;

	// Token: 0x0400066E RID: 1646
	private RadioButtonGroup headgearRadioButtonGroup;

	// Token: 0x0400066F RID: 1647
	private RadioButtonGroup mustachesRadioButtonGroup;

	// Token: 0x04000670 RID: 1648
	private RadioButtonGroup beardsRadioButtonGroup;

	// Token: 0x04000671 RID: 1649
	private RadioButtonGroup jerseysRadioButtonGroup;

	// Token: 0x04000672 RID: 1650
	private RadioButtonGroup stickSkinsRadioButtonGroup;

	// Token: 0x04000673 RID: 1651
	private RadioButtonGroup stickShaftTapesRadioButtonGroup;

	// Token: 0x04000674 RID: 1652
	private RadioButtonGroup stickBladeTapesRadioButtonGroup;
}
