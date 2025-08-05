using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x020000FD RID: 253
public class UIAppearance : UIComponent<UIAppearance>
{
	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x060008A4 RID: 2212 RVA: 0x0000C4FD File Offset: 0x0000A6FD
	// (set) Token: 0x060008A5 RID: 2213 RVA: 0x0000C505 File Offset: 0x0000A705
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
			this.team = value;
			this.OnTeamChanged();
		}
	}

	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x060008A6 RID: 2214 RVA: 0x0000C51E File Offset: 0x0000A71E
	// (set) Token: 0x060008A7 RID: 2215 RVA: 0x0000C526 File Offset: 0x0000A726
	public PlayerRole Role
	{
		get
		{
			return this.role;
		}
		set
		{
			if (this.role == value)
			{
				return;
			}
			this.role = value;
			this.OnRoleChanged();
		}
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x000358B8 File Offset: 0x00033AB8
	private void Start()
	{
		base.VisibilityRequiresMouse = true;
		this.flagMap.Add("none", new AppearanceItem
		{
			Id = 0,
			Name = "NONE",
			Image = null,
			Purchaseable = false,
			Price = ""
		});
		int num = 0;
		foreach (Texture image in this.flagTextures)
		{
			string key = Utils.CountryDictionary.ElementAt(num).Key;
			this.flagMap.Add(key, new AppearanceItem
			{
				Id = 0,
				Name = Utils.CountryCodeToName(key).ToUpper(),
				Image = image,
				Purchaseable = false,
				Price = ""
			});
			num++;
		}
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x000359C0 File Offset: 0x00033BC0
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("AppearanceContainer", null);
		this.closeButton = this.container.Query("CloseButton", null);
		this.closeButton.clicked += this.OnClickClose;
		this.tabView = this.container.Query("AppearanceTabView", null);
		this.tabView.activeTabChanged += this.OnTabChanged;
		this.flagTab = this.tabView.Query("FlagTab", null);
		this.flagRadioButtonGroup = this.flagTab.Query("AppearanceItemRadioButtonGroup", null);
		this.flagRadioButtonGroup.RegisterValueChangedCallback(new EventCallback<ChangeEvent<int>>(this.OnFlagRadioButtonGroupChanged));
		this.visorTab = this.tabView.Query("VisorTab", null);
		this.visorRadioButtonGroup = this.visorTab.Query("AppearanceItemRadioButtonGroup", null);
		this.visorRadioButtonGroup.RegisterValueChangedCallback(new EventCallback<ChangeEvent<int>>(this.OnVisorRadioButtonGroupChanged));
		this.mustacheTab = this.tabView.Query("MustacheTab", null);
		this.mustacheRadioButtonGroup = this.mustacheTab.Query("AppearanceItemRadioButtonGroup", null);
		this.mustacheRadioButtonGroup.RegisterValueChangedCallback(new EventCallback<ChangeEvent<int>>(this.OnMustacheRadioButtonGroupChanged));
		this.beardTab = this.tabView.Query("BeardTab", null);
		this.beardRadioButtonGroup = this.beardTab.Query("AppearanceItemRadioButtonGroup", null);
		this.beardRadioButtonGroup.RegisterValueChangedCallback(new EventCallback<ChangeEvent<int>>(this.OnBeardRadioButtonGroupChanged));
		this.jerseyTab = this.tabView.Query("JerseyTab", null);
		this.jerseyRadioButtonGroup = this.jerseyTab.Query("AppearanceItemRadioButtonGroup", null);
		this.jerseyRadioButtonGroup.RegisterValueChangedCallback(new EventCallback<ChangeEvent<int>>(this.OnJerseyRadioButtonGroupChanged));
		this.stickSkinTab = this.tabView.Query("StickSkinTab", null);
		this.stickSkinRadioButtonGroup = this.stickSkinTab.Query("AppearanceItemRadioButtonGroup", null);
		this.stickSkinRadioButtonGroup.RegisterValueChangedCallback(new EventCallback<ChangeEvent<int>>(this.OnStickSkinRadioButtonGroupChanged));
		this.stickShaftTapeTab = this.tabView.Query("StickShaftTapeTab", null);
		this.stickShaftTapeRadioButtonGroup = this.stickShaftTapeTab.Query("AppearanceItemRadioButtonGroup", null);
		this.stickShaftTapeRadioButtonGroup.RegisterValueChangedCallback(new EventCallback<ChangeEvent<int>>(this.OnStickShaftTapeSkinRadioButtonGroupChanged));
		this.stickBladeTapeTab = this.tabView.Query("StickBladeTapeTab", null);
		this.stickBladeTapeRadioButtonGroup = this.stickBladeTapeTab.Query("AppearanceItemRadioButtonGroup", null);
		this.stickBladeTapeRadioButtonGroup.RegisterValueChangedCallback(new EventCallback<ChangeEvent<int>>(this.OnStickBladeTapeSkinRadioButtonGroupChanged));
		this.applyForBothTeamsToggle = this.container.Query("AppearanceApplyForBothTeamsToggle", null).First().Query("Toggle", null);
		this.applyForBothTeamsToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnApplyForBothTeamsToggleChanged));
		this.applyForBothTeamsToggle.value = this.ApplyForBothTeams;
		this.teamDropdown = this.container.Query("AppearanceTeamDropdown", null).First().Query("Dropdown", null);
		this.teamDropdown.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnTeamDropdownChanged));
		this.teamDropdown.value = ((this.Team == PlayerTeam.Blue) ? "BLUE" : "RED");
		this.roleDropdown = this.container.Query("AppearanceRoleDropdown", null).First().Query("Dropdown", null);
		this.roleDropdown.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnRoleDropdownChanged));
		this.roleDropdown.value = ((this.Role == PlayerRole.Attacker) ? "SKATER" : "GOALIE");
		this.Reload();
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x00035DF8 File Offset: 0x00033FF8
	public void Reload()
	{
		if (Application.isBatchMode)
		{
			return;
		}
		this.PopulateAppearanceItemsFromMap(this.visorRadioButtonGroup, this.visorMap, this.ownedItemIds);
		this.PopulateAppearanceItemsFromMap(this.flagRadioButtonGroup, this.flagMap, this.ownedItemIds);
		this.PopulateAppearanceItemsFromMap(this.mustacheRadioButtonGroup, this.mustacheMap, this.ownedItemIds);
		this.PopulateAppearanceItemsFromMap(this.beardRadioButtonGroup, this.beardMap, this.ownedItemIds);
		this.PopulateAppearanceItemsFromMap(this.jerseyRadioButtonGroup, this.jerseyMap, this.ownedItemIds);
		this.currentStickSkinMap = ((this.Role == PlayerRole.Attacker) ? this.stickAttackerSkinMap : this.stickGoalieSkinMap);
		this.PopulateAppearanceItemsFromMap(this.stickSkinRadioButtonGroup, this.currentStickSkinMap, this.ownedItemIds);
		this.PopulateAppearanceItemsFromMap(this.stickShaftTapeRadioButtonGroup, this.stickShaftTapeSkinMap, this.ownedItemIds);
		this.PopulateAppearanceItemsFromMap(this.stickBladeTapeRadioButtonGroup, this.stickBladeTapeSkinMap, this.ownedItemIds);
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x00035EEC File Offset: 0x000340EC
	public void ApplyAppearanceValues()
	{
		if (Application.isBatchMode)
		{
			return;
		}
		this.flagRadioButtonGroup.value = this.flagMap.Keys.ToList<string>().IndexOf(MonoBehaviourSingleton<SettingsManager>.Instance.Country);
		this.visorRadioButtonGroup.value = this.visorMap.Keys.ToList<string>().IndexOf(MonoBehaviourSingleton<SettingsManager>.Instance.GetVisorSkin(this.Team, this.Role));
		this.mustacheRadioButtonGroup.value = this.mustacheMap.Keys.ToList<string>().IndexOf(MonoBehaviourSingleton<SettingsManager>.Instance.Mustache);
		this.beardRadioButtonGroup.value = this.beardMap.Keys.ToList<string>().IndexOf(MonoBehaviourSingleton<SettingsManager>.Instance.Beard);
		this.jerseyRadioButtonGroup.value = this.jerseyMap.Keys.ToList<string>().IndexOf(MonoBehaviourSingleton<SettingsManager>.Instance.GetJerseySkin(this.Team, this.Role));
		this.stickSkinRadioButtonGroup.value = this.currentStickSkinMap.Keys.ToList<string>().IndexOf(MonoBehaviourSingleton<SettingsManager>.Instance.GetStickSkin(this.Team, this.Role));
		this.stickShaftTapeRadioButtonGroup.value = this.stickShaftTapeSkinMap.Keys.ToList<string>().IndexOf(MonoBehaviourSingleton<SettingsManager>.Instance.GetStickShaftSkin(this.Team, this.Role));
		this.stickBladeTapeRadioButtonGroup.value = this.stickBladeTapeSkinMap.Keys.ToList<string>().IndexOf(MonoBehaviourSingleton<SettingsManager>.Instance.GetStickBladeSkin(this.Team, this.Role));
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x00036090 File Offset: 0x00034290
	private void PopulateAppearanceItemsFromMap(RadioButtonGroup radioButtonGroup, SerializedDictionary<string, AppearanceItem> map, int[] ownedItemIds)
	{
		radioButtonGroup.contentContainer.Clear();
		int num = 0;
		foreach (KeyValuePair<string, AppearanceItem> keyValuePair in map)
		{
			AppearanceItem value = keyValuePair.Value;
			RadioButton radioButton = this.appearanceItemAsset.Instantiate().Query("AppearanceItemRadioButton", null);
			VisualElement visualElement = radioButton.Query("AppearanceItemImage", null);
			VisualElement visualElement2 = radioButton.Query("AppearanceItemPurchaseOverlay", null);
			radioButton.label = value.Name;
			radioButton.userData = value;
			if (value.Image != null)
			{
				visualElement.style.backgroundImage = (Texture2D)value.Image;
			}
			else if (value.IsTwoTone && value.BlueImage != null && value.RedImage != null)
			{
				if (this.Team == PlayerTeam.Blue)
				{
					visualElement.style.backgroundImage = (Texture2D)value.BlueImage;
				}
				else
				{
					visualElement.style.backgroundImage = (Texture2D)value.RedImage;
				}
			}
			else
			{
				visualElement.style.backgroundImage = null;
				radioButton.EnableInClassList("no-image", true);
			}
			if (radioButtonGroup.value == num)
			{
				radioButton.value = true;
			}
			if (value.Purchaseable && !ownedItemIds.Contains(value.Id) && !ownedItemIds.Contains(-1))
			{
				visualElement2.style.display = DisplayStyle.Flex;
				Button button = visualElement2.Query("PurchaseButton", null);
				button.RegisterCallback<ClickEvent, int>(new EventCallback<ClickEvent, int>(this.OnClickPurchase), value.Id, TrickleDown.NoTrickleDown);
				button.text = (value.Price ?? "");
			}
			else
			{
				visualElement2.style.display = DisplayStyle.None;
			}
			if (value.Hidden && !ownedItemIds.Contains(value.Id) && !ownedItemIds.Contains(-1))
			{
				radioButton.style.display = DisplayStyle.None;
			}
			radioButtonGroup.contentContainer.Add(radioButton);
			num++;
		}
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x0000C53F File Offset: 0x0000A73F
	public void SetOwnedItemIds(int[] itemIds)
	{
		this.ownedItemIds = itemIds;
		this.Reload();
		this.ApplyAppearanceValues();
	}

	// Token: 0x060008AE RID: 2222 RVA: 0x0000C554 File Offset: 0x0000A754
	public override void Show()
	{
		base.Show();
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceShow", null);
		this.OnTabChanged(null, this.tabView.activeTab);
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x0000C57E File Offset: 0x0000A77E
	public override void Hide(bool ignoreAlwaysVisible = false)
	{
		base.Hide(ignoreAlwaysVisible);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceHide", null);
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x0000C597 File Offset: 0x0000A797
	private void OnTeamChanged()
	{
		if (this.teamDropdown == null)
		{
			return;
		}
		this.teamDropdown.value = ((this.Team == PlayerTeam.Blue) ? "BLUE" : "RED");
	}

	// Token: 0x060008B1 RID: 2225 RVA: 0x0000C5C2 File Offset: 0x0000A7C2
	private void OnRoleChanged()
	{
		if (this.roleDropdown == null)
		{
			return;
		}
		this.roleDropdown.value = ((this.Role == PlayerRole.Attacker) ? "SKATER" : "GOALIE");
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x0000C5ED File Offset: 0x0000A7ED
	private void OnClickClose()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceClickClose", null);
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x0000C5FF File Offset: 0x0000A7FF
	private void OnClickPurchase(ClickEvent clickEvent, int itemId)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearancePurchaseItem", new Dictionary<string, object>
		{
			{
				"itemId",
				itemId
			}
		});
	}

	// Token: 0x060008B4 RID: 2228 RVA: 0x000362EC File Offset: 0x000344EC
	private void OnTeamDropdownChanged(ChangeEvent<string> changeEvent)
	{
		this.Team = ((changeEvent.newValue == "BLUE") ? PlayerTeam.Blue : PlayerTeam.Red);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceTeamChanged", new Dictionary<string, object>
		{
			{
				"team",
				this.Team
			}
		});
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x00036340 File Offset: 0x00034540
	private void OnRoleDropdownChanged(ChangeEvent<string> changeEvent)
	{
		this.Role = ((changeEvent.newValue == "SKATER") ? PlayerRole.Attacker : PlayerRole.Goalie);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceRoleChanged", new Dictionary<string, object>
		{
			{
				"role",
				this.Role
			}
		});
	}

	// Token: 0x060008B6 RID: 2230 RVA: 0x0000C626 File Offset: 0x0000A826
	private void OnApplyForBothTeamsToggleChanged(ChangeEvent<bool> changeEvent)
	{
		this.ApplyForBothTeams = changeEvent.newValue;
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x0000C634 File Offset: 0x0000A834
	private void OnTabChanged(Tab oldTab, Tab newTab)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceTabChanged", new Dictionary<string, object>
		{
			{
				"tabName",
				newTab.name
			}
		});
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x00036394 File Offset: 0x00034594
	private void OnFlagRadioButtonGroupChanged(ChangeEvent<int> changeEvent)
	{
		if (this.flagMap.Count <= changeEvent.newValue || changeEvent.newValue < 0)
		{
			return;
		}
		string key = this.flagMap.ElementAt(changeEvent.newValue).Key;
		AppearanceItem value = this.flagMap.ElementAt(changeEvent.newValue).Value;
		bool flag = value.Purchaseable && !this.ownedItemIds.Contains(value.Id) && !this.ownedItemIds.Contains(-1);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceFlagChanged", new Dictionary<string, object>
		{
			{
				"isPreview",
				flag
			},
			{
				"value",
				key
			}
		});
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x00036454 File Offset: 0x00034654
	private void OnVisorRadioButtonGroupChanged(ChangeEvent<int> changeEvent)
	{
		if (this.visorMap.Count <= changeEvent.newValue || changeEvent.newValue < 0)
		{
			return;
		}
		string key = this.visorMap.ElementAt(changeEvent.newValue).Key;
		AppearanceItem value = this.visorMap.ElementAt(changeEvent.newValue).Value;
		bool flag = value.Purchaseable && !this.ownedItemIds.Contains(value.Id) && !this.ownedItemIds.Contains(-1);
		if (this.ApplyForBothTeams)
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceVisorChanged", new Dictionary<string, object>
			{
				{
					"team",
					(this.Team == PlayerTeam.Blue) ? PlayerTeam.Red : PlayerTeam.Blue
				},
				{
					"role",
					this.Role
				},
				{
					"isPreview",
					flag
				},
				{
					"value",
					key
				}
			});
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceVisorChanged", new Dictionary<string, object>
		{
			{
				"team",
				this.Team
			},
			{
				"role",
				this.Role
			},
			{
				"isPreview",
				flag
			},
			{
				"value",
				key
			}
		});
	}

	// Token: 0x060008BA RID: 2234 RVA: 0x000365AC File Offset: 0x000347AC
	private void OnMustacheRadioButtonGroupChanged(ChangeEvent<int> changeEvent)
	{
		if (this.mustacheMap.Count <= changeEvent.newValue || changeEvent.newValue < 0)
		{
			return;
		}
		string key = this.mustacheMap.ElementAt(changeEvent.newValue).Key;
		AppearanceItem value = this.mustacheMap.ElementAt(changeEvent.newValue).Value;
		bool flag = value.Purchaseable && !this.ownedItemIds.Contains(value.Id) && !this.ownedItemIds.Contains(-1);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceMustacheChanged", new Dictionary<string, object>
		{
			{
				"isPreview",
				flag
			},
			{
				"value",
				key
			}
		});
	}

	// Token: 0x060008BB RID: 2235 RVA: 0x0003666C File Offset: 0x0003486C
	private void OnBeardRadioButtonGroupChanged(ChangeEvent<int> changeEvent)
	{
		if (this.beardMap.Count <= changeEvent.newValue || changeEvent.newValue < 0)
		{
			return;
		}
		string key = this.beardMap.ElementAt(changeEvent.newValue).Key;
		AppearanceItem value = this.beardMap.ElementAt(changeEvent.newValue).Value;
		bool flag = value.Purchaseable && !this.ownedItemIds.Contains(value.Id) && !this.ownedItemIds.Contains(-1);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceBeardChanged", new Dictionary<string, object>
		{
			{
				"isPreview",
				flag
			},
			{
				"value",
				key
			}
		});
	}

	// Token: 0x060008BC RID: 2236 RVA: 0x0003672C File Offset: 0x0003492C
	private void OnJerseyRadioButtonGroupChanged(ChangeEvent<int> changeEvent)
	{
		if (this.jerseyMap.Count <= changeEvent.newValue || changeEvent.newValue < 0)
		{
			return;
		}
		AppearanceItem value = this.jerseyMap.ElementAt(changeEvent.newValue).Value;
		string key = this.jerseyMap.ElementAt(changeEvent.newValue).Key;
		bool flag = value.Purchaseable && !this.ownedItemIds.Contains(value.Id) && !this.ownedItemIds.Contains(-1);
		if (this.ApplyForBothTeams)
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceJerseyChanged", new Dictionary<string, object>
			{
				{
					"team",
					(this.Team == PlayerTeam.Blue) ? PlayerTeam.Red : PlayerTeam.Blue
				},
				{
					"role",
					this.Role
				},
				{
					"isPreview",
					flag
				},
				{
					"value",
					key
				}
			});
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceJerseyChanged", new Dictionary<string, object>
		{
			{
				"team",
				this.Team
			},
			{
				"role",
				this.Role
			},
			{
				"isPreview",
				flag
			},
			{
				"value",
				key
			}
		});
	}

	// Token: 0x060008BD RID: 2237 RVA: 0x00036884 File Offset: 0x00034A84
	private void OnStickSkinRadioButtonGroupChanged(ChangeEvent<int> changeEvent)
	{
		if (this.currentStickSkinMap.Count <= changeEvent.newValue || changeEvent.newValue < 0)
		{
			return;
		}
		AppearanceItem value = this.currentStickSkinMap.ElementAt(changeEvent.newValue).Value;
		string key = this.currentStickSkinMap.ElementAt(changeEvent.newValue).Key;
		bool flag = value.Purchaseable && !this.ownedItemIds.Contains(value.Id) && !this.ownedItemIds.Contains(-1);
		if (this.ApplyForBothTeams)
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceStickSkinChanged", new Dictionary<string, object>
			{
				{
					"team",
					(this.Team == PlayerTeam.Blue) ? PlayerTeam.Red : PlayerTeam.Blue
				},
				{
					"role",
					this.Role
				},
				{
					"isPreview",
					flag
				},
				{
					"value",
					key
				}
			});
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceStickSkinChanged", new Dictionary<string, object>
		{
			{
				"team",
				this.Team
			},
			{
				"role",
				this.Role
			},
			{
				"isPreview",
				flag
			},
			{
				"value",
				key
			}
		});
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x000369DC File Offset: 0x00034BDC
	private void OnStickShaftTapeSkinRadioButtonGroupChanged(ChangeEvent<int> changeEvent)
	{
		if (this.stickShaftTapeSkinMap.Count <= changeEvent.newValue || changeEvent.newValue < 0)
		{
			return;
		}
		string key = this.stickShaftTapeSkinMap.ElementAt(changeEvent.newValue).Key;
		AppearanceItem value = this.stickShaftTapeSkinMap.ElementAt(changeEvent.newValue).Value;
		bool flag = value.Purchaseable && !this.ownedItemIds.Contains(value.Id) && !this.ownedItemIds.Contains(-1);
		if (this.ApplyForBothTeams)
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceStickShaftTapeSkinChanged", new Dictionary<string, object>
			{
				{
					"team",
					(this.Team == PlayerTeam.Blue) ? PlayerTeam.Red : PlayerTeam.Blue
				},
				{
					"role",
					this.Role
				},
				{
					"isPreview",
					flag
				},
				{
					"value",
					key
				}
			});
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceStickShaftTapeSkinChanged", new Dictionary<string, object>
		{
			{
				"team",
				this.Team
			},
			{
				"role",
				this.Role
			},
			{
				"isPreview",
				flag
			},
			{
				"value",
				key
			}
		});
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x00036B34 File Offset: 0x00034D34
	private void OnStickBladeTapeSkinRadioButtonGroupChanged(ChangeEvent<int> changeEvent)
	{
		if (this.stickBladeTapeSkinMap.Count <= changeEvent.newValue || changeEvent.newValue < 0)
		{
			return;
		}
		string key = this.stickBladeTapeSkinMap.ElementAt(changeEvent.newValue).Key;
		AppearanceItem value = this.stickBladeTapeSkinMap.ElementAt(changeEvent.newValue).Value;
		bool flag = value.Purchaseable && !this.ownedItemIds.Contains(value.Id) && !this.ownedItemIds.Contains(-1);
		if (this.ApplyForBothTeams)
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceStickBladeTapeSkinChanged", new Dictionary<string, object>
			{
				{
					"team",
					(this.Team == PlayerTeam.Blue) ? PlayerTeam.Red : PlayerTeam.Blue
				},
				{
					"role",
					this.Role
				},
				{
					"isPreview",
					flag
				},
				{
					"value",
					key
				}
			});
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnAppearanceStickBladeTapeSkinChanged", new Dictionary<string, object>
		{
			{
				"team",
				this.Team
			},
			{
				"role",
				this.Role
			},
			{
				"isPreview",
				flag
			},
			{
				"value",
				key
			}
		});
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x00036D34 File Offset: 0x00034F34
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060008C2 RID: 2242 RVA: 0x0000C65B File Offset: 0x0000A85B
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060008C3 RID: 2243 RVA: 0x0000C665 File Offset: 0x0000A865
	protected internal override string __getTypeName()
	{
		return "UIAppearance";
	}

	// Token: 0x0400053B RID: 1339
	[Header("Components")]
	public VisualTreeAsset appearanceItemAsset;

	// Token: 0x0400053C RID: 1340
	[Header("References")]
	[SerializeField]
	private List<Texture> flagTextures = new List<Texture>();

	// Token: 0x0400053D RID: 1341
	[SerializeField]
	private SerializedDictionary<string, AppearanceItem> flagMap = new SerializedDictionary<string, AppearanceItem>();

	// Token: 0x0400053E RID: 1342
	[SerializeField]
	private SerializedDictionary<string, AppearanceItem> visorMap = new SerializedDictionary<string, AppearanceItem>();

	// Token: 0x0400053F RID: 1343
	[SerializeField]
	private SerializedDictionary<string, AppearanceItem> mustacheMap = new SerializedDictionary<string, AppearanceItem>();

	// Token: 0x04000540 RID: 1344
	[SerializeField]
	private SerializedDictionary<string, AppearanceItem> beardMap = new SerializedDictionary<string, AppearanceItem>();

	// Token: 0x04000541 RID: 1345
	[SerializeField]
	private SerializedDictionary<string, AppearanceItem> jerseyMap = new SerializedDictionary<string, AppearanceItem>();

	// Token: 0x04000542 RID: 1346
	[SerializeField]
	private SerializedDictionary<string, AppearanceItem> stickAttackerSkinMap = new SerializedDictionary<string, AppearanceItem>();

	// Token: 0x04000543 RID: 1347
	[SerializeField]
	private SerializedDictionary<string, AppearanceItem> stickGoalieSkinMap = new SerializedDictionary<string, AppearanceItem>();

	// Token: 0x04000544 RID: 1348
	[SerializeField]
	private SerializedDictionary<string, AppearanceItem> stickShaftTapeSkinMap = new SerializedDictionary<string, AppearanceItem>();

	// Token: 0x04000545 RID: 1349
	[SerializeField]
	private SerializedDictionary<string, AppearanceItem> stickBladeTapeSkinMap = new SerializedDictionary<string, AppearanceItem>();

	// Token: 0x04000546 RID: 1350
	private SerializedDictionary<string, AppearanceItem> currentStickSkinMap = new SerializedDictionary<string, AppearanceItem>();

	// Token: 0x04000547 RID: 1351
	public bool ApplyForBothTeams;

	// Token: 0x04000548 RID: 1352
	private PlayerTeam team = PlayerTeam.Blue;

	// Token: 0x04000549 RID: 1353
	private PlayerRole role = PlayerRole.Attacker;

	// Token: 0x0400054A RID: 1354
	private TabView tabView;

	// Token: 0x0400054B RID: 1355
	private Tab flagTab;

	// Token: 0x0400054C RID: 1356
	private Tab visorTab;

	// Token: 0x0400054D RID: 1357
	private Tab mustacheTab;

	// Token: 0x0400054E RID: 1358
	private Tab beardTab;

	// Token: 0x0400054F RID: 1359
	private Tab jerseyTab;

	// Token: 0x04000550 RID: 1360
	private Tab stickSkinTab;

	// Token: 0x04000551 RID: 1361
	private Tab stickShaftTapeTab;

	// Token: 0x04000552 RID: 1362
	private Tab stickBladeTapeTab;

	// Token: 0x04000553 RID: 1363
	private Toggle applyForBothTeamsToggle;

	// Token: 0x04000554 RID: 1364
	private DropdownField teamDropdown;

	// Token: 0x04000555 RID: 1365
	private DropdownField roleDropdown;

	// Token: 0x04000556 RID: 1366
	private Button closeButton;

	// Token: 0x04000557 RID: 1367
	private RadioButtonGroup flagRadioButtonGroup;

	// Token: 0x04000558 RID: 1368
	private RadioButtonGroup visorRadioButtonGroup;

	// Token: 0x04000559 RID: 1369
	private RadioButtonGroup mustacheRadioButtonGroup;

	// Token: 0x0400055A RID: 1370
	private RadioButtonGroup beardRadioButtonGroup;

	// Token: 0x0400055B RID: 1371
	private RadioButtonGroup jerseyRadioButtonGroup;

	// Token: 0x0400055C RID: 1372
	private RadioButtonGroup stickSkinRadioButtonGroup;

	// Token: 0x0400055D RID: 1373
	private RadioButtonGroup stickShaftTapeRadioButtonGroup;

	// Token: 0x0400055E RID: 1374
	private RadioButtonGroup stickBladeTapeRadioButtonGroup;

	// Token: 0x0400055F RID: 1375
	private int[] ownedItemIds = new int[0];
}
