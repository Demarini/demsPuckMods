using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000137 RID: 311
public class UIServerLauncher : UIComponent<UIServerLauncher>
{
	// Token: 0x06000ADF RID: 2783 RVA: 0x0000DEB3 File Offset: 0x0000C0B3
	private void Start()
	{
		base.VisibilityRequiresMouse = true;
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x0003E768 File Offset: 0x0003C968
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("ServerLauncherContainer", null);
		this.closeButton = this.container.Query("CloseButton", null);
		this.closeButton.clicked += this.OnClickClose;
		this.startButton = this.container.Query("StartButton", null);
		this.startButton.clicked += this.OnClickLaunch;
		this.tabView = this.container.Query("TabView", null);
		this.dedicatedTab = this.container.Query("DedicatedTab", null);
		this.dedicatedNameTextField = this.dedicatedTab.Query("NameTextField", null).First().Query("TextField", null);
		this.dedicatedNameTextField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnDedicatedNameChanged));
		this.dedicatedNameTextField.RegisterCallback<FocusOutEvent>(new EventCallback<FocusOutEvent>(this.OnDedicatedNameFocusOut), TrickleDown.NoTrickleDown);
		this.dedicatedNameTextField.value = this.dedicatedName;
		this.dedicatedLocationDropdown = this.dedicatedTab.Query("LocationDropdown", null).First().Query("Dropdown", null);
		this.dedicatedLocationDropdown.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnDedicatedLocationChanged));
		this.dedicatedMaxPlayerSlider = this.dedicatedTab.Query("MaxPlayersSlider", null).First().Query("Slider", null);
		this.dedicatedMaxPlayerSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnDedicatedMaxPlayersChanged));
		this.dedicatedMaxPlayerSlider.value = (float)this.dedicatedMaxPlayers;
		this.dedicatedPasswordProtectedToggle = this.dedicatedTab.Query("PasswordProtectedToggle", null).First().Query("Toggle", null);
		this.dedicatedPasswordProtectedToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnDedicatedPasswordProtectedChanged));
		this.dedicatedPasswordProtectedToggle.value = !string.IsNullOrEmpty(this.dedicatedPassword);
		this.dedicatedPasswordTextField = this.dedicatedTab.Query("PasswordTextField", null).First().Query("TextField", null);
		this.dedicatedPasswordTextField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnDedicatedPasswordChanged));
		this.dedicatedPasswordTextField.SetEnabled(this.dedicatedPasswordProtectedToggle.value);
		this.dedicatedPasswordTextField.value = this.dedicatedPassword;
		this.dedicatedPasswordProtectionCoverVisualElement = this.dedicatedTab.Query("PasswordProtectionCover", null).First();
		this.selfHostedTab = this.container.Query("SelfHostedTab", null);
		this.selfHostedNameTextField = this.selfHostedTab.Query("NameTextField", null).First().Query("TextField", null);
		this.selfHostedNameTextField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnSelfHostedNameChanged));
		this.selfHostedNameTextField.RegisterCallback<FocusOutEvent>(new EventCallback<FocusOutEvent>(this.OnSelfHostedNameFocusOut), TrickleDown.NoTrickleDown);
		this.selfHostedNameTextField.value = this.selfHostedName;
		this.selfHostedPortIntegerField = this.selfHostedTab.Query("PortIntegerField", null).First().Query("IntegerField", null);
		this.selfHostedPortIntegerField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<int>>(this.OnSelfHostedPortChanged));
		this.selfHostedPortIntegerField.value = this.selfHostedPort;
		this.selfHostedMaxPlayerSlider = this.selfHostedTab.Query("MaxPlayersSlider", null).First().Query("Slider", null);
		this.selfHostedMaxPlayerSlider.RegisterValueChangedCallback(new EventCallback<ChangeEvent<float>>(this.OnSelfHostedMaxPlayersChanged));
		this.selfHostedMaxPlayerSlider.value = (float)this.selfHostedMaxPlayers;
		this.selfHostedPasswordProtectedToggle = this.selfHostedTab.Query("PasswordProtectedToggle", null).First().Query("Toggle", null);
		this.selfHostedPasswordProtectedToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnSelfHostedPasswordProtectedChanged));
		this.selfHostedPasswordProtectedToggle.value = !string.IsNullOrEmpty(this.selfHostedPassword);
		this.selfHostedPasswordTextField = this.selfHostedTab.Query("PasswordTextField", null).First().Query("TextField", null);
		this.selfHostedPasswordTextField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnSelfHostedPasswordChanged));
		this.selfHostedPasswordTextField.SetEnabled(this.selfHostedPasswordProtectedToggle.value);
		this.selfHostedPasswordTextField.value = this.selfHostedPassword;
		this.selfHostedVoipToggle = this.selfHostedTab.Query("VoipToggle", null).First().Query("Toggle", null);
		this.selfHostedVoipToggle.RegisterValueChangedCallback(new EventCallback<ChangeEvent<bool>>(this.OnSelfHostedVoipChanged));
		this.selfHostedVoipToggle.value = this.selfHostedVoip;
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x0000DEBC File Offset: 0x0000C0BC
	public override void Show()
	{
		if (!base.IsVisible)
		{
			this.Refresh();
		}
		base.Show();
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x0003EC90 File Offset: 0x0003CE90
	public void Refresh()
	{
		this.startButton.SetEnabled(false);
		this.dedicatedLauncherLocations = new ServerLauncherLocation[0];
		this.dedicatedLocationDropdown.choices = new List<string>
		{
			"LOADING..."
		};
		this.dedicatedLocationDropdown.value = this.dedicatedLocationDropdown.choices.First<string>();
		MonoBehaviourSingleton<WebSocketManager>.Instance.Emit("playerGetServerLauncherLocationsRequest", null, "playerGetServerLauncherLocationsResponse");
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x0003ED00 File Offset: 0x0003CF00
	public void LaunchDedicatedServer()
	{
		MonoBehaviourSingleton<WebSocketManager>.Instance.Emit("playerLaunchServerRequest", new Dictionary<string, object>
		{
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
				"location",
				this.dedicatedLocation
			}
		}, "playerLaunchServerResponse");
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x0003ED70 File Offset: 0x0003CF70
	public void LaunchSelfHostedServer()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnServerLauncherClickStartSelfHostedServer", new Dictionary<string, object>
		{
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
				"voip",
				this.selfHostedVoip
			}
		});
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x0000DED2 File Offset: 0x0000C0D2
	public void HideDedicatedPasswordProtection()
	{
		this.dedicatedPasswordProtectedToggle.SetEnabled(false);
		this.dedicatedPasswordTextField.SetEnabled(false);
		this.dedicatedPasswordProtectionCoverVisualElement.visible = true;
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x0003EDF8 File Offset: 0x0003CFF8
	public void ShowDedicatedPasswordProtection()
	{
		this.dedicatedPasswordProtectedToggle.SetEnabled(true);
		this.dedicatedPasswordProtectedToggle.value = false;
		this.dedicatedPasswordTextField.SetEnabled(false);
		this.dedicatedPasswordTextField.value = "";
		this.dedicatedPasswordProtectionCoverVisualElement.visible = false;
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x0003EE48 File Offset: 0x0003D048
	public void SetDedicatedLocations(ServerLauncherLocation[] locations)
	{
		this.dedicatedLauncherLocations = locations;
		List<string> list = (from location in this.dedicatedLauncherLocations
		select (location.continent + ": " + location.city).ToUpper()).ToList<string>();
		list.Sort();
		this.dedicatedLocationDropdown.choices = list;
		this.dedicatedLocationDropdown.value = this.dedicatedLocationDropdown.choices.First<string>();
		this.startButton.SetEnabled(true);
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x0000DEF8 File Offset: 0x0000C0F8
	private void ResetDedicatedName()
	{
		this.dedicatedName = "MY PUCK SERVER";
		this.dedicatedNameTextField.value = this.dedicatedName;
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x0000DF16 File Offset: 0x0000C116
	private void ResetSelfHostedName()
	{
		this.selfHostedName = "MY PUCK SERVER";
		this.selfHostedNameTextField.value = this.selfHostedName;
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x0000DF34 File Offset: 0x0000C134
	private void OnClickClose()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnServerLauncherClickClose", null);
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x0003EEC8 File Offset: 0x0003D0C8
	private void OnClickLaunch()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnServerLauncherClickClose", null);
		if (this.tabView.activeTab == this.dedicatedTab)
		{
			this.LaunchDedicatedServer();
			return;
		}
		if (this.tabView.activeTab == this.selfHostedTab)
		{
			this.LaunchSelfHostedServer();
		}
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x0000DF46 File Offset: 0x0000C146
	private void OnDedicatedNameChanged(ChangeEvent<string> changeEvent)
	{
		this.dedicatedName = Utils.FilterStringSpecialCharacters(changeEvent.newValue);
		this.dedicatedNameTextField.value = this.dedicatedName;
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x0003EF18 File Offset: 0x0003D118
	private void OnDedicatedNameFocusOut(FocusOutEvent focusOutEvent)
	{
		this.dedicatedName = Utils.FilterStringSpecialCharacters(this.dedicatedName);
		this.dedicatedName = Utils.FilterStringProfanity(this.dedicatedName, false);
		if (string.IsNullOrEmpty(this.dedicatedName))
		{
			this.ResetDedicatedName();
			return;
		}
		this.dedicatedNameTextField.value = this.dedicatedName;
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x0003EF70 File Offset: 0x0003D170
	private void OnDedicatedLocationChanged(ChangeEvent<string> changeEvent)
	{
		ServerLauncherLocation serverLauncherLocation = this.dedicatedLauncherLocations.FirstOrDefault((ServerLauncherLocation location) => (location.continent + ": " + location.city).ToUpper() == changeEvent.newValue);
		if (serverLauncherLocation == null)
		{
			return;
		}
		this.dedicatedLocation = serverLauncherLocation;
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x0000DF6A File Offset: 0x0000C16A
	private void OnDedicatedMaxPlayersChanged(ChangeEvent<float> changeEvent)
	{
		this.dedicatedMaxPlayers = Mathf.RoundToInt(changeEvent.newValue);
		this.dedicatedMaxPlayerSlider.value = (float)this.dedicatedMaxPlayers;
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x0000DF8F File Offset: 0x0000C18F
	private void OnDedicatedPasswordProtectedChanged(ChangeEvent<bool> changeEvent)
	{
		if (changeEvent.newValue)
		{
			this.dedicatedPasswordTextField.SetEnabled(true);
			return;
		}
		this.dedicatedPasswordTextField.SetEnabled(false);
		this.dedicatedPasswordTextField.value = "";
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x0000DFC2 File Offset: 0x0000C1C2
	private void OnDedicatedPasswordChanged(ChangeEvent<string> changeEvent)
	{
		this.dedicatedPassword = changeEvent.newValue;
		this.dedicatedPasswordTextField.value = this.dedicatedPassword;
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x0000DFE1 File Offset: 0x0000C1E1
	private void OnSelfHostedNameChanged(ChangeEvent<string> changeEvent)
	{
		this.selfHostedName = Utils.FilterStringSpecialCharacters(changeEvent.newValue);
		this.selfHostedNameTextField.value = this.selfHostedName;
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x0003EFB0 File Offset: 0x0003D1B0
	private void OnSelfHostedNameFocusOut(FocusOutEvent focusOutEvent)
	{
		this.selfHostedName = Utils.FilterStringSpecialCharacters(this.selfHostedName);
		this.selfHostedName = Utils.FilterStringProfanity(this.selfHostedName, false);
		if (string.IsNullOrEmpty(this.selfHostedName))
		{
			this.ResetSelfHostedName();
			return;
		}
		this.selfHostedNameTextField.value = this.selfHostedName;
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x0000E005 File Offset: 0x0000C205
	private void OnSelfHostedPortChanged(ChangeEvent<int> changeEvent)
	{
		this.selfHostedPort = changeEvent.newValue;
		this.selfHostedPortIntegerField.value = this.selfHostedPort;
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x0000E024 File Offset: 0x0000C224
	private void OnSelfHostedMaxPlayersChanged(ChangeEvent<float> changeEvent)
	{
		this.selfHostedMaxPlayers = Mathf.RoundToInt(changeEvent.newValue);
		this.selfHostedMaxPlayerSlider.value = (float)this.selfHostedMaxPlayers;
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x0000E049 File Offset: 0x0000C249
	private void OnSelfHostedPasswordProtectedChanged(ChangeEvent<bool> changeEvent)
	{
		if (changeEvent.newValue)
		{
			this.selfHostedPasswordTextField.SetEnabled(true);
			return;
		}
		this.selfHostedPasswordTextField.SetEnabled(false);
		this.selfHostedPasswordTextField.value = "";
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x0000E07C File Offset: 0x0000C27C
	private void OnSelfHostedPasswordChanged(ChangeEvent<string> changeEvent)
	{
		this.selfHostedPassword = changeEvent.newValue;
		this.selfHostedPasswordTextField.value = this.selfHostedPassword;
	}

	// Token: 0x06000AF8 RID: 2808 RVA: 0x0000E09B File Offset: 0x0000C29B
	private void OnSelfHostedVoipChanged(ChangeEvent<bool> changeEvent)
	{
		this.selfHostedVoip = changeEvent.newValue;
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x0003F074 File Offset: 0x0003D274
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x0000E0A9 File Offset: 0x0000C2A9
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x0000E0B3 File Offset: 0x0000C2B3
	protected internal override string __getTypeName()
	{
		return "UIServerLauncher";
	}

	// Token: 0x0400065B RID: 1627
	private Button closeButton;

	// Token: 0x0400065C RID: 1628
	private Button startButton;

	// Token: 0x0400065D RID: 1629
	private TabView tabView;

	// Token: 0x0400065E RID: 1630
	private Tab dedicatedTab;

	// Token: 0x0400065F RID: 1631
	private Tab selfHostedTab;

	// Token: 0x04000660 RID: 1632
	private TextField dedicatedNameTextField;

	// Token: 0x04000661 RID: 1633
	private DropdownField dedicatedLocationDropdown;

	// Token: 0x04000662 RID: 1634
	private Slider dedicatedMaxPlayerSlider;

	// Token: 0x04000663 RID: 1635
	private Toggle dedicatedPasswordProtectedToggle;

	// Token: 0x04000664 RID: 1636
	private TextField dedicatedPasswordTextField;

	// Token: 0x04000665 RID: 1637
	private VisualElement dedicatedPasswordProtectionCoverVisualElement;

	// Token: 0x04000666 RID: 1638
	private TextField selfHostedNameTextField;

	// Token: 0x04000667 RID: 1639
	private IntegerField selfHostedPortIntegerField;

	// Token: 0x04000668 RID: 1640
	private Slider selfHostedMaxPlayerSlider;

	// Token: 0x04000669 RID: 1641
	private Toggle selfHostedPasswordProtectedToggle;

	// Token: 0x0400066A RID: 1642
	private TextField selfHostedPasswordTextField;

	// Token: 0x0400066B RID: 1643
	private Toggle selfHostedVoipToggle;

	// Token: 0x0400066C RID: 1644
	private ServerLauncherLocation[] dedicatedLauncherLocations = new ServerLauncherLocation[0];

	// Token: 0x0400066D RID: 1645
	private string dedicatedName = "MY DEDICATED PUCK SERVER";

	// Token: 0x0400066E RID: 1646
	private ServerLauncherLocation dedicatedLocation;

	// Token: 0x0400066F RID: 1647
	private int dedicatedMaxPlayers = 6;

	// Token: 0x04000670 RID: 1648
	private string dedicatedPassword = "";

	// Token: 0x04000671 RID: 1649
	private int selfHostedPort = 7777;

	// Token: 0x04000672 RID: 1650
	private string selfHostedName = "MY SELF HOSTED PUCK SERVER";

	// Token: 0x04000673 RID: 1651
	private int selfHostedMaxPlayers = 12;

	// Token: 0x04000674 RID: 1652
	private string selfHostedPassword = "";

	// Token: 0x04000675 RID: 1653
	private bool selfHostedVoip = true;
}
