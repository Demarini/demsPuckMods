using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x020001A4 RID: 420
public class UINewServer : UIView
{
	// Token: 0x06000C0F RID: 3087 RVA: 0x00038F80 File Offset: 0x00037180
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("NewServerView", null);
		this.newServer = base.View.Query("NewServer", null);
		this.closeIconButton = this.newServer.Query("CloseIconButtonContainer", null).First().Query(null, null);
		this.closeIconButton.clicked += this.OnClickClose;
		this.startButton = this.newServer.Query("StartButton", null);
		this.startButton.clicked += this.OnClickStart;
		this.tabView = this.newServer.Query("TabView", null);
		this.dedicatedTab = this.newServer.Query("DedicatedTab", null);
		this.dedicatedNameTextField = this.dedicatedTab.Query("NameTextFieldInput", null).First().Query(null, null);
		this.dedicatedNameTextField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnDedicatedNameChanged));
		this.dedicatedNameTextField.RegisterCallback<FocusOutEvent>(new EventCallback<FocusOutEvent>(this.OnDedicatedNameFocusOut), TrickleDown.NoTrickleDown);
		this.dedicatedNameTextField.value = this.dedicatedName;
		this.dedicatedLocationDropdown = this.dedicatedTab.Query("LocationDropdownInput", null).First().Query(null, null);
		this.dedicatedLocationDropdown.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnDedicatedLocationChanged));
		this.dedicatedMaxPlayerSlider = this.dedicatedTab.Query("MaxPlayersSliderInput", null).First().Query(null, null);
		this.dedicatedMaxPlayerSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnDedicatedMaxPlayersChanged));
		this.dedicatedMaxPlayerSlider.value = (float)this.dedicatedMaxPlayers;
		this.dedicatedPasswordTextField = this.dedicatedTab.Query("PasswordTextFieldInput", null).First().Query(null, null);
		this.dedicatedPasswordTextField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnDedicatedPasswordChanged));
		this.dedicatedPasswordTextField.value = this.dedicatedPassword;
		this.dedicatedPasswordProtectedToggle = this.dedicatedTab.Query("PasswordProtectedToggleInput", null).First().Query(null, null);
		this.dedicatedPasswordProtectedToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnDedicatedPasswordProtectedChanged));
		this.dedicatedPasswordProtectedToggle.value = !string.IsNullOrEmpty(this.dedicatedPassword);
		this.patreonOverlay = this.dedicatedTab.Query("PatreonOverlay", null);
		this.selfHostedTab = this.newServer.Query("SelfHostedTab", null);
		this.selfHostedNameTextField = this.selfHostedTab.Query("NameTextFieldInput", null).First().Query(null, null);
		this.selfHostedNameTextField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnSelfHostedNameChanged));
		this.selfHostedNameTextField.RegisterCallback<FocusOutEvent>(new EventCallback<FocusOutEvent>(this.OnSelfHostedNameFocusOut), TrickleDown.NoTrickleDown);
		this.selfHostedNameTextField.value = this.selfHostedName;
		this.selfHostedPortIntegerField = this.selfHostedTab.Query("PortIntegerFieldInput", null).First().Query(null, null);
		this.selfHostedPortIntegerField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<int>>(this.OnSelfHostedPortChanged));
		this.selfHostedPortIntegerField.value = this.selfHostedPort;
		this.selfHostedMaxPlayerSlider = this.selfHostedTab.Query("MaxPlayersSliderInput", null).First().Query(null, null);
		this.selfHostedMaxPlayerSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnSelfHostedMaxPlayersChanged));
		this.selfHostedMaxPlayerSlider.value = (float)this.selfHostedMaxPlayers;
		this.selfHostedPasswordTextField = this.selfHostedTab.Query("PasswordTextFieldInput", null).First().Query(null, null);
		this.selfHostedPasswordTextField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnSelfHostedPasswordChanged));
		this.selfHostedPasswordTextField.value = this.selfHostedPassword;
		this.selfHostedPasswordProtectedToggle = this.selfHostedTab.Query("PasswordProtectedToggleInput", null).First().Query(null, null);
		this.selfHostedPasswordProtectedToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnSelfHostedPasswordProtectedChanged));
		this.selfHostedPasswordProtectedToggle.value = !string.IsNullOrEmpty(this.selfHostedPassword);
		this.selfHostedVoipToggle = this.selfHostedTab.Query("VOIPToggleInput", null).First().Query(null, null);
		this.selfHostedVoipToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnSelfHostedVoipChanged));
		this.selfHostedVoipToggle.value = this.selfHostedUseVoip;
	}

	// Token: 0x06000C10 RID: 3088 RVA: 0x00039478 File Offset: 0x00037678
	public override bool Show()
	{
		if (!base.IsVisible)
		{
			this.Refresh();
		}
		return base.Show();
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x00039490 File Offset: 0x00037690
	public void Refresh()
	{
		this.startButton.SetEnabled(false);
		this.dedicatedLauncherLocations = new Location[0];
		this.dedicatedLocationDropdown.choices = new List<string>();
		this.dedicatedLocationDropdown.value = null;
		this.dedicatedLocationDropdown.SetEnabled(false);
		WebSocketManager.Emit("playerGetLocationsRequest", null, "playerGetLocationsResponse");
	}

	// Token: 0x06000C12 RID: 3090 RVA: 0x000394ED File Offset: 0x000376ED
	public void HidePatreonOverlay()
	{
		this.dedicatedPasswordProtectedToggle.SetEnabled(true);
		this.patreonOverlay.style.display = DisplayStyle.None;
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x00039511 File Offset: 0x00037711
	public void ShowPatreonOverlay()
	{
		this.dedicatedPasswordProtectedToggle.value = false;
		this.dedicatedPasswordProtectedToggle.SetEnabled(false);
		this.patreonOverlay.style.display = DisplayStyle.Flex;
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x00039544 File Offset: 0x00037744
	public void SetDedicatedLocations(Location[] locations)
	{
		this.dedicatedLauncherLocations = locations;
		List<string> list = (from location in this.dedicatedLauncherLocations
		select (location.continent + ": " + location.city).ToUpper()).ToList<string>();
		list.Sort();
		this.dedicatedLocationDropdown.choices = list;
		this.dedicatedLocationDropdown.value = this.dedicatedLocationDropdown.choices.First<string>();
		this.dedicatedLocationDropdown.SetEnabled(true);
		this.startButton.SetEnabled(true);
	}

	// Token: 0x06000C15 RID: 3093 RVA: 0x000395CD File Offset: 0x000377CD
	private void ResetDedicatedName()
	{
		this.dedicatedName = "MY PUCK SERVER";
		this.dedicatedNameTextField.value = this.dedicatedName;
	}

	// Token: 0x06000C16 RID: 3094 RVA: 0x000395EB File Offset: 0x000377EB
	private void ResetSelfHostedName()
	{
		this.selfHostedName = "MY PUCK SERVER";
		this.selfHostedNameTextField.value = this.selfHostedName;
	}

	// Token: 0x06000C17 RID: 3095 RVA: 0x00039609 File Offset: 0x00037809
	private void OnClickClose()
	{
		EventManager.TriggerEvent("Event_OnNewServerClickClose", null);
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x00039618 File Offset: 0x00037818
	private void OnClickStart()
	{
		if (this.tabView.activeTab == this.dedicatedTab)
		{
			EventManager.TriggerEvent("Event_OnNewServerClickStart", new Dictionary<string, object>
			{
				{
					"type",
					"dedicated"
				},
				{
					"name",
					this.dedicatedName
				},
				{
					"maxPlayers",
					this.dedicatedMaxPlayers
				},
				{
					"password",
					this.dedicatedPassword
				},
				{
					"locationId",
					this.dedicatedLocation.id
				}
			});
			return;
		}
		if (this.tabView.activeTab == this.selfHostedTab)
		{
			EventManager.TriggerEvent("Event_OnNewServerClickStart", new Dictionary<string, object>
			{
				{
					"type",
					"selfHosted"
				},
				{
					"port",
					this.selfHostedPort
				},
				{
					"name",
					this.selfHostedName
				},
				{
					"maxPlayers",
					this.selfHostedMaxPlayers
				},
				{
					"password",
					this.selfHostedPassword
				},
				{
					"useVoip",
					this.selfHostedUseVoip
				}
			});
		}
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x0003973F File Offset: 0x0003793F
	private void OnDedicatedNameChanged(ChangeEvent<string> changeEvent)
	{
		this.dedicatedName = StringUtils.FilterStringSpecialCharacters(changeEvent.newValue, null, null);
		this.dedicatedNameTextField.value = this.dedicatedName;
	}

	// Token: 0x06000C1A RID: 3098 RVA: 0x00039765 File Offset: 0x00037965
	private void OnDedicatedNameFocusOut(FocusOutEvent focusOutEvent)
	{
		this.dedicatedName = StringUtils.FilterStringProfanity(this.dedicatedName, false);
		if (string.IsNullOrEmpty(this.dedicatedName))
		{
			this.ResetDedicatedName();
			return;
		}
		this.dedicatedNameTextField.value = this.dedicatedName;
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x000397A0 File Offset: 0x000379A0
	private void OnDedicatedLocationChanged(ChangeEvent<string> changeEvent)
	{
		Location location2 = this.dedicatedLauncherLocations.FirstOrDefault((Location location) => (location.continent + ": " + location.city).ToUpper() == changeEvent.newValue);
		if (location2 == null)
		{
			return;
		}
		this.dedicatedLocation = location2;
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x000397DD File Offset: 0x000379DD
	private void OnDedicatedMaxPlayersChanged(ChangeEvent<float> changeEvent)
	{
		this.dedicatedMaxPlayers = Mathf.RoundToInt(changeEvent.newValue);
		this.dedicatedMaxPlayerSlider.value = (float)this.dedicatedMaxPlayers;
	}

	// Token: 0x06000C1D RID: 3101 RVA: 0x00039802 File Offset: 0x00037A02
	private void OnDedicatedPasswordProtectedChanged(ChangeEvent<bool> changeEvent)
	{
		if (changeEvent.newValue)
		{
			this.dedicatedPasswordTextField.SetEnabled(true);
			return;
		}
		this.dedicatedPasswordTextField.SetEnabled(false);
		this.dedicatedPasswordTextField.value = string.Empty;
	}

	// Token: 0x06000C1E RID: 3102 RVA: 0x00039835 File Offset: 0x00037A35
	private void OnDedicatedPasswordChanged(ChangeEvent<string> changeEvent)
	{
		this.dedicatedPassword = changeEvent.newValue;
		this.dedicatedPasswordTextField.value = this.dedicatedPassword;
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x00039854 File Offset: 0x00037A54
	private void OnSelfHostedNameChanged(ChangeEvent<string> changeEvent)
	{
		this.selfHostedName = StringUtils.FilterStringSpecialCharacters(changeEvent.newValue, null, null);
		this.selfHostedNameTextField.value = this.selfHostedName;
	}

	// Token: 0x06000C20 RID: 3104 RVA: 0x0003987A File Offset: 0x00037A7A
	private void OnSelfHostedNameFocusOut(FocusOutEvent focusOutEvent)
	{
		this.selfHostedName = StringUtils.FilterStringProfanity(this.selfHostedName, false);
		if (string.IsNullOrEmpty(this.selfHostedName))
		{
			this.ResetSelfHostedName();
			return;
		}
		this.selfHostedNameTextField.value = this.selfHostedName;
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x000398B3 File Offset: 0x00037AB3
	private void OnSelfHostedPortChanged(ChangeEvent<int> changeEvent)
	{
		this.selfHostedPort = changeEvent.newValue;
		this.selfHostedPortIntegerField.value = this.selfHostedPort;
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x000398D2 File Offset: 0x00037AD2
	private void OnSelfHostedMaxPlayersChanged(ChangeEvent<float> changeEvent)
	{
		this.selfHostedMaxPlayers = Mathf.RoundToInt(changeEvent.newValue);
		this.selfHostedMaxPlayerSlider.value = (float)this.selfHostedMaxPlayers;
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x000398F7 File Offset: 0x00037AF7
	private void OnSelfHostedPasswordProtectedChanged(ChangeEvent<bool> changeEvent)
	{
		if (changeEvent.newValue)
		{
			this.selfHostedPasswordTextField.SetEnabled(true);
			return;
		}
		this.selfHostedPasswordTextField.SetEnabled(false);
		this.selfHostedPasswordTextField.value = string.Empty;
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x0003992A File Offset: 0x00037B2A
	private void OnSelfHostedPasswordChanged(ChangeEvent<string> changeEvent)
	{
		this.selfHostedPassword = changeEvent.newValue;
		this.selfHostedPasswordTextField.value = this.selfHostedPassword;
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x00039949 File Offset: 0x00037B49
	private void OnSelfHostedVoipChanged(ChangeEvent<bool> changeEvent)
	{
		this.selfHostedUseVoip = changeEvent.newValue;
	}

	// Token: 0x04000716 RID: 1814
	private VisualElement newServer;

	// Token: 0x04000717 RID: 1815
	private IconButton closeIconButton;

	// Token: 0x04000718 RID: 1816
	private Button startButton;

	// Token: 0x04000719 RID: 1817
	private TabView tabView;

	// Token: 0x0400071A RID: 1818
	private Tab dedicatedTab;

	// Token: 0x0400071B RID: 1819
	private Tab selfHostedTab;

	// Token: 0x0400071C RID: 1820
	private TextField dedicatedNameTextField;

	// Token: 0x0400071D RID: 1821
	private DropdownField dedicatedLocationDropdown;

	// Token: 0x0400071E RID: 1822
	private Slider dedicatedMaxPlayerSlider;

	// Token: 0x0400071F RID: 1823
	private Toggle dedicatedPasswordProtectedToggle;

	// Token: 0x04000720 RID: 1824
	private TextField dedicatedPasswordTextField;

	// Token: 0x04000721 RID: 1825
	private TextField selfHostedNameTextField;

	// Token: 0x04000722 RID: 1826
	private IntegerField selfHostedPortIntegerField;

	// Token: 0x04000723 RID: 1827
	private Slider selfHostedMaxPlayerSlider;

	// Token: 0x04000724 RID: 1828
	private Toggle selfHostedPasswordProtectedToggle;

	// Token: 0x04000725 RID: 1829
	private TextField selfHostedPasswordTextField;

	// Token: 0x04000726 RID: 1830
	private Toggle selfHostedVoipToggle;

	// Token: 0x04000727 RID: 1831
	private VisualElement patreonOverlay;

	// Token: 0x04000728 RID: 1832
	private Location[] dedicatedLauncherLocations = new Location[0];

	// Token: 0x04000729 RID: 1833
	private string dedicatedName = "MY PUCK SERVER";

	// Token: 0x0400072A RID: 1834
	private Location dedicatedLocation;

	// Token: 0x0400072B RID: 1835
	private int dedicatedMaxPlayers = 6;

	// Token: 0x0400072C RID: 1836
	private string dedicatedPassword;

	// Token: 0x0400072D RID: 1837
	private int selfHostedPort = 30609;

	// Token: 0x0400072E RID: 1838
	private string selfHostedName = "MY PUCK SERVER";

	// Token: 0x0400072F RID: 1839
	private int selfHostedMaxPlayers = 12;

	// Token: 0x04000730 RID: 1840
	private string selfHostedPassword;

	// Token: 0x04000731 RID: 1841
	private bool selfHostedUseVoip;
}
