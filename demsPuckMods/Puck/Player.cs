using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000EF RID: 239
public class Player : NetworkBehaviour
{
	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x060007D9 RID: 2009 RVA: 0x0000BDFA File Offset: 0x00009FFA
	public bool IsCharacterFullySpawned
	{
		get
		{
			return this.PlayerCamera && this.PlayerBody && this.StickPositioner && this.Stick;
		}
	}

	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x060007DA RID: 2010 RVA: 0x0000BE30 File Offset: 0x0000A030
	public bool IsCharacterPartiallySpawned
	{
		get
		{
			return this.PlayerCamera || this.PlayerBody || this.StickPositioner || this.Stick;
		}
	}

	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x060007DB RID: 2011 RVA: 0x0000BE66 File Offset: 0x0000A066
	public bool IsSpectatorCameraSpawned
	{
		get
		{
			return this.SpectatorCamera;
		}
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x0000BE73 File Offset: 0x0000A073
	private void Awake()
	{
		this.PlayerInput = base.GetComponent<PlayerInput>();
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x0002EDE8 File Offset: 0x0002CFE8
	public override void OnNetworkSpawn()
	{
		this.State.Initialize(this);
		NetworkVariable<PlayerState> state = this.State;
		state.OnValueChanged = (NetworkVariable<PlayerState>.OnValueChangedDelegate)Delegate.Combine(state.OnValueChanged, new NetworkVariable<PlayerState>.OnValueChangedDelegate(this.OnPlayerStateChanged));
		this.Username.Initialize(this);
		NetworkVariable<FixedString32Bytes> username = this.Username;
		username.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(username.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerUsernameChanged));
		this.Number.Initialize(this);
		NetworkVariable<int> number = this.Number;
		number.OnValueChanged = (NetworkVariable<int>.OnValueChangedDelegate)Delegate.Combine(number.OnValueChanged, new NetworkVariable<int>.OnValueChangedDelegate(this.OnPlayerNumberChanged));
		this.Handedness.Initialize(this);
		NetworkVariable<PlayerHandedness> handedness = this.Handedness;
		handedness.OnValueChanged = (NetworkVariable<PlayerHandedness>.OnValueChangedDelegate)Delegate.Combine(handedness.OnValueChanged, new NetworkVariable<PlayerHandedness>.OnValueChangedDelegate(this.OnPlayerHandednessChanged));
		this.Team.Initialize(this);
		NetworkVariable<PlayerTeam> team = this.Team;
		team.OnValueChanged = (NetworkVariable<PlayerTeam>.OnValueChangedDelegate)Delegate.Combine(team.OnValueChanged, new NetworkVariable<PlayerTeam>.OnValueChangedDelegate(this.OnPlayerTeamChanged));
		this.Role.Initialize(this);
		NetworkVariable<PlayerRole> role = this.Role;
		role.OnValueChanged = (NetworkVariable<PlayerRole>.OnValueChangedDelegate)Delegate.Combine(role.OnValueChanged, new NetworkVariable<PlayerRole>.OnValueChangedDelegate(this.OnPlayerRoleChanged));
		this.Goals.Initialize(this);
		NetworkVariable<int> goals = this.Goals;
		goals.OnValueChanged = (NetworkVariable<int>.OnValueChangedDelegate)Delegate.Combine(goals.OnValueChanged, new NetworkVariable<int>.OnValueChangedDelegate(this.OnPlayerGoalsChanged));
		this.Assists.Initialize(this);
		NetworkVariable<int> assists = this.Assists;
		assists.OnValueChanged = (NetworkVariable<int>.OnValueChangedDelegate)Delegate.Combine(assists.OnValueChanged, new NetworkVariable<int>.OnValueChangedDelegate(this.OnPlayerAssistsChanged));
		this.Ping.Initialize(this);
		NetworkVariable<int> ping = this.Ping;
		ping.OnValueChanged = (NetworkVariable<int>.OnValueChangedDelegate)Delegate.Combine(ping.OnValueChanged, new NetworkVariable<int>.OnValueChangedDelegate(this.OnPlayerPingChanged));
		this.PlayerPositionReference.Initialize(this);
		NetworkVariable<NetworkObjectReference> playerPositionReference = this.PlayerPositionReference;
		playerPositionReference.OnValueChanged = (NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate)Delegate.Combine(playerPositionReference.OnValueChanged, new NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate(this.OnPlayerPlayerPositionReferenceChanged));
		this.Country.Initialize(this);
		NetworkVariable<FixedString32Bytes> country = this.Country;
		country.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(country.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerCountryChanged));
		this.VisorAttackerBlueSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> visorAttackerBlueSkin = this.VisorAttackerBlueSkin;
		visorAttackerBlueSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(visorAttackerBlueSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerVisorChanged));
		this.VisorAttackerRedSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> visorAttackerRedSkin = this.VisorAttackerRedSkin;
		visorAttackerRedSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(visorAttackerRedSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerVisorChanged));
		this.VisorGoalieBlueSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> visorGoalieBlueSkin = this.VisorGoalieBlueSkin;
		visorGoalieBlueSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(visorGoalieBlueSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerVisorChanged));
		this.VisorGoalieRedSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> visorGoalieRedSkin = this.VisorGoalieRedSkin;
		visorGoalieRedSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(visorGoalieRedSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerVisorChanged));
		this.Mustache.Initialize(this);
		NetworkVariable<FixedString32Bytes> mustache = this.Mustache;
		mustache.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(mustache.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerMustacheChanged));
		this.Beard.Initialize(this);
		NetworkVariable<FixedString32Bytes> beard = this.Beard;
		beard.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(beard.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerBeardChanged));
		this.JerseyAttackerBlueSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> jerseyAttackerBlueSkin = this.JerseyAttackerBlueSkin;
		jerseyAttackerBlueSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(jerseyAttackerBlueSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerJerseyChanged));
		this.JerseyAttackerRedSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> jerseyAttackerRedSkin = this.JerseyAttackerRedSkin;
		jerseyAttackerRedSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(jerseyAttackerRedSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerJerseyChanged));
		this.JerseyGoalieBlueSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> jerseyGoalieBlueSkin = this.JerseyGoalieBlueSkin;
		jerseyGoalieBlueSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(jerseyGoalieBlueSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerJerseyChanged));
		this.JerseyGoalieRedSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> jerseyGoalieRedSkin = this.JerseyGoalieRedSkin;
		jerseyGoalieRedSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(jerseyGoalieRedSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerJerseyChanged));
		this.StickAttackerBlueSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> stickAttackerBlueSkin = this.StickAttackerBlueSkin;
		stickAttackerBlueSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(stickAttackerBlueSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickSkinChanged));
		this.StickAttackerRedSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> stickAttackerRedSkin = this.StickAttackerRedSkin;
		stickAttackerRedSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(stickAttackerRedSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickSkinChanged));
		this.StickGoalieBlueSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> stickGoalieBlueSkin = this.StickGoalieBlueSkin;
		stickGoalieBlueSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(stickGoalieBlueSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickSkinChanged));
		this.StickGoalieRedSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> stickGoalieRedSkin = this.StickGoalieRedSkin;
		stickGoalieRedSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(stickGoalieRedSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickSkinChanged));
		this.StickShaftAttackerBlueTapeSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> stickShaftAttackerBlueTapeSkin = this.StickShaftAttackerBlueTapeSkin;
		stickShaftAttackerBlueTapeSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(stickShaftAttackerBlueTapeSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickShaftTapeSkinChanged));
		this.StickShaftAttackerRedTapeSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> stickShaftAttackerRedTapeSkin = this.StickShaftAttackerRedTapeSkin;
		stickShaftAttackerRedTapeSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(stickShaftAttackerRedTapeSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickShaftTapeSkinChanged));
		this.StickShaftGoalieBlueTapeSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> stickShaftGoalieBlueTapeSkin = this.StickShaftGoalieBlueTapeSkin;
		stickShaftGoalieBlueTapeSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(stickShaftGoalieBlueTapeSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickShaftTapeSkinChanged));
		this.StickShaftGoalieRedTapeSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> stickShaftGoalieRedTapeSkin = this.StickShaftGoalieRedTapeSkin;
		stickShaftGoalieRedTapeSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(stickShaftGoalieRedTapeSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickShaftTapeSkinChanged));
		this.StickBladeAttackerBlueTapeSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> stickBladeAttackerBlueTapeSkin = this.StickBladeAttackerBlueTapeSkin;
		stickBladeAttackerBlueTapeSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(stickBladeAttackerBlueTapeSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickBladeTapeSkinChanged));
		this.StickBladeAttackerRedTapeSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> stickBladeAttackerRedTapeSkin = this.StickBladeAttackerRedTapeSkin;
		stickBladeAttackerRedTapeSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(stickBladeAttackerRedTapeSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickBladeTapeSkinChanged));
		this.StickBladeGoalieBlueTapeSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> stickBladeGoalieBlueTapeSkin = this.StickBladeGoalieBlueTapeSkin;
		stickBladeGoalieBlueTapeSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(stickBladeGoalieBlueTapeSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickBladeTapeSkinChanged));
		this.StickBladeGoalieRedTapeSkin.Initialize(this);
		NetworkVariable<FixedString32Bytes> stickBladeGoalieRedTapeSkin = this.StickBladeGoalieRedTapeSkin;
		stickBladeGoalieRedTapeSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(stickBladeGoalieRedTapeSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickBladeTapeSkinChanged));
		this.PatreonLevel.Initialize(this);
		NetworkVariable<int> patreonLevel = this.PatreonLevel;
		patreonLevel.OnValueChanged = (NetworkVariable<int>.OnValueChangedDelegate)Delegate.Combine(patreonLevel.OnValueChanged, new NetworkVariable<int>.OnValueChangedDelegate(this.OnPlayerPatreonLevelChanged));
		this.AdminLevel.Initialize(this);
		NetworkVariable<int> adminLevel = this.AdminLevel;
		adminLevel.OnValueChanged = (NetworkVariable<int>.OnValueChangedDelegate)Delegate.Combine(adminLevel.OnValueChanged, new NetworkVariable<int>.OnValueChangedDelegate(this.OnPlayerAdminLevelChanged));
		this.SteamId.Initialize(this);
		NetworkVariable<FixedString32Bytes> steamId = this.SteamId;
		steamId.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Combine(steamId.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerSteamIdChanged));
		this.Client_InitializeNetworkVariables();
		base.OnNetworkSpawn();
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x0002F530 File Offset: 0x0002D730
	protected override void OnNetworkPostSpawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerSpawned", new Dictionary<string, object>
		{
			{
				"player",
				this
			}
		});
		if (base.IsOwner)
		{
			this.Client_PlayerSubscriptionRpc(MonoBehaviourSingleton<StateManager>.Instance.PlayerData.username, MonoBehaviourSingleton<StateManager>.Instance.PlayerData.number, (MonoBehaviourSingleton<SettingsManager>.Instance.Handedness == "LEFT") ? PlayerHandedness.Left : PlayerHandedness.Right, MonoBehaviourSingleton<SettingsManager>.Instance.Country, MonoBehaviourSingleton<SettingsManager>.Instance.VisorAttackerBlueSkin, MonoBehaviourSingleton<SettingsManager>.Instance.VisorAttackerRedSkin, MonoBehaviourSingleton<SettingsManager>.Instance.VisorGoalieBlueSkin, MonoBehaviourSingleton<SettingsManager>.Instance.VisorGoalieRedSkin, MonoBehaviourSingleton<SettingsManager>.Instance.Mustache, MonoBehaviourSingleton<SettingsManager>.Instance.Beard, MonoBehaviourSingleton<SettingsManager>.Instance.JerseyAttackerBlueSkin, MonoBehaviourSingleton<SettingsManager>.Instance.JerseyAttackerRedSkin, MonoBehaviourSingleton<SettingsManager>.Instance.JerseyGoalieBlueSkin, MonoBehaviourSingleton<SettingsManager>.Instance.JerseyGoalieRedSkin, MonoBehaviourSingleton<SettingsManager>.Instance.StickAttackerBlueSkin, MonoBehaviourSingleton<SettingsManager>.Instance.StickAttackerRedSkin, MonoBehaviourSingleton<SettingsManager>.Instance.StickGoalieBlueSkin, MonoBehaviourSingleton<SettingsManager>.Instance.StickGoalieRedSkin, MonoBehaviourSingleton<SettingsManager>.Instance.StickShaftAttackerBlueTapeSkin, MonoBehaviourSingleton<SettingsManager>.Instance.StickShaftAttackerRedTapeSkin, MonoBehaviourSingleton<SettingsManager>.Instance.StickShaftGoalieBlueTapeSkin, MonoBehaviourSingleton<SettingsManager>.Instance.StickShaftGoalieRedTapeSkin, MonoBehaviourSingleton<SettingsManager>.Instance.StickBladeAttackerBlueTapeSkin, MonoBehaviourSingleton<SettingsManager>.Instance.StickBladeAttackerRedTapeSkin, MonoBehaviourSingleton<SettingsManager>.Instance.StickBladeGoalieBlueTapeSkin, MonoBehaviourSingleton<SettingsManager>.Instance.StickBladeGoalieRedTapeSkin, MonoBehaviourSingleton<StateManager>.Instance.PlayerData.patreonLevel, MonoBehaviourSingleton<StateManager>.Instance.PlayerData.adminLevel, MonoBehaviourSingleton<StateManager>.Instance.PlayerData.steamId, MonoBehaviourSingleton<ModManagerV2>.Instance.EnabledModIds);
		}
		base.OnNetworkPostSpawn();
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x0000BE81 File Offset: 0x0000A081
	protected override void OnNetworkSessionSynchronized()
	{
		this.Client_InitializeNetworkVariables();
		base.OnNetworkSessionSynchronized();
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x0002F748 File Offset: 0x0002D948
	public override void OnNetworkDespawn()
	{
		if (NetworkManager.Singleton.IsServer)
		{
			Tween tween = this.delayedStateTween;
			if (tween != null)
			{
				tween.Kill(false);
			}
			if (this.IsCharacterPartiallySpawned)
			{
				this.Server_DespawnCharacter();
			}
			if (this.IsSpectatorCameraSpawned)
			{
				this.Server_DespawnSpectatorCamera();
			}
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerDespawned", new Dictionary<string, object>
		{
			{
				"player",
				this
			}
		});
		NetworkVariable<PlayerState> state = this.State;
		state.OnValueChanged = (NetworkVariable<PlayerState>.OnValueChangedDelegate)Delegate.Remove(state.OnValueChanged, new NetworkVariable<PlayerState>.OnValueChangedDelegate(this.OnPlayerStateChanged));
		this.State.Dispose();
		NetworkVariable<FixedString32Bytes> username = this.Username;
		username.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(username.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerUsernameChanged));
		this.Username.Dispose();
		NetworkVariable<int> number = this.Number;
		number.OnValueChanged = (NetworkVariable<int>.OnValueChangedDelegate)Delegate.Remove(number.OnValueChanged, new NetworkVariable<int>.OnValueChangedDelegate(this.OnPlayerNumberChanged));
		this.Number.Dispose();
		NetworkVariable<PlayerHandedness> handedness = this.Handedness;
		handedness.OnValueChanged = (NetworkVariable<PlayerHandedness>.OnValueChangedDelegate)Delegate.Remove(handedness.OnValueChanged, new NetworkVariable<PlayerHandedness>.OnValueChangedDelegate(this.OnPlayerHandednessChanged));
		this.Handedness.Dispose();
		NetworkVariable<PlayerTeam> team = this.Team;
		team.OnValueChanged = (NetworkVariable<PlayerTeam>.OnValueChangedDelegate)Delegate.Remove(team.OnValueChanged, new NetworkVariable<PlayerTeam>.OnValueChangedDelegate(this.OnPlayerTeamChanged));
		this.Team.Dispose();
		NetworkVariable<PlayerRole> role = this.Role;
		role.OnValueChanged = (NetworkVariable<PlayerRole>.OnValueChangedDelegate)Delegate.Remove(role.OnValueChanged, new NetworkVariable<PlayerRole>.OnValueChangedDelegate(this.OnPlayerRoleChanged));
		this.Role.Dispose();
		NetworkVariable<int> goals = this.Goals;
		goals.OnValueChanged = (NetworkVariable<int>.OnValueChangedDelegate)Delegate.Remove(goals.OnValueChanged, new NetworkVariable<int>.OnValueChangedDelegate(this.OnPlayerGoalsChanged));
		this.Goals.Dispose();
		NetworkVariable<int> assists = this.Assists;
		assists.OnValueChanged = (NetworkVariable<int>.OnValueChangedDelegate)Delegate.Remove(assists.OnValueChanged, new NetworkVariable<int>.OnValueChangedDelegate(this.OnPlayerAssistsChanged));
		this.Assists.Dispose();
		NetworkVariable<int> ping = this.Ping;
		ping.OnValueChanged = (NetworkVariable<int>.OnValueChangedDelegate)Delegate.Remove(ping.OnValueChanged, new NetworkVariable<int>.OnValueChangedDelegate(this.OnPlayerPingChanged));
		this.Ping.Dispose();
		NetworkVariable<NetworkObjectReference> playerPositionReference = this.PlayerPositionReference;
		playerPositionReference.OnValueChanged = (NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate)Delegate.Remove(playerPositionReference.OnValueChanged, new NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate(this.OnPlayerPlayerPositionReferenceChanged));
		this.PlayerPositionReference.Dispose();
		NetworkVariable<FixedString32Bytes> country = this.Country;
		country.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(country.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerCountryChanged));
		this.Country.Dispose();
		NetworkVariable<FixedString32Bytes> visorAttackerBlueSkin = this.VisorAttackerBlueSkin;
		visorAttackerBlueSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(visorAttackerBlueSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerVisorChanged));
		this.VisorAttackerBlueSkin.Dispose();
		NetworkVariable<FixedString32Bytes> visorAttackerRedSkin = this.VisorAttackerRedSkin;
		visorAttackerRedSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(visorAttackerRedSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerVisorChanged));
		this.VisorAttackerRedSkin.Dispose();
		NetworkVariable<FixedString32Bytes> visorGoalieBlueSkin = this.VisorGoalieBlueSkin;
		visorGoalieBlueSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(visorGoalieBlueSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerVisorChanged));
		this.VisorGoalieBlueSkin.Dispose();
		NetworkVariable<FixedString32Bytes> visorGoalieRedSkin = this.VisorGoalieRedSkin;
		visorGoalieRedSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(visorGoalieRedSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerVisorChanged));
		this.VisorGoalieRedSkin.Dispose();
		NetworkVariable<FixedString32Bytes> mustache = this.Mustache;
		mustache.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(mustache.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerMustacheChanged));
		this.Mustache.Dispose();
		NetworkVariable<FixedString32Bytes> beard = this.Beard;
		beard.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(beard.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerBeardChanged));
		this.Beard.Dispose();
		NetworkVariable<FixedString32Bytes> jerseyAttackerBlueSkin = this.JerseyAttackerBlueSkin;
		jerseyAttackerBlueSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(jerseyAttackerBlueSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerJerseyChanged));
		this.JerseyAttackerBlueSkin.Dispose();
		NetworkVariable<FixedString32Bytes> jerseyAttackerRedSkin = this.JerseyAttackerRedSkin;
		jerseyAttackerRedSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(jerseyAttackerRedSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerJerseyChanged));
		this.JerseyAttackerRedSkin.Dispose();
		NetworkVariable<FixedString32Bytes> jerseyGoalieBlueSkin = this.JerseyGoalieBlueSkin;
		jerseyGoalieBlueSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(jerseyGoalieBlueSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerJerseyChanged));
		this.JerseyGoalieBlueSkin.Dispose();
		NetworkVariable<FixedString32Bytes> jerseyGoalieRedSkin = this.JerseyGoalieRedSkin;
		jerseyGoalieRedSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(jerseyGoalieRedSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerJerseyChanged));
		this.JerseyGoalieRedSkin.Dispose();
		NetworkVariable<FixedString32Bytes> stickAttackerBlueSkin = this.StickAttackerBlueSkin;
		stickAttackerBlueSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(stickAttackerBlueSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickSkinChanged));
		this.StickAttackerBlueSkin.Dispose();
		NetworkVariable<FixedString32Bytes> stickAttackerRedSkin = this.StickAttackerRedSkin;
		stickAttackerRedSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(stickAttackerRedSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickSkinChanged));
		this.StickAttackerRedSkin.Dispose();
		NetworkVariable<FixedString32Bytes> stickGoalieBlueSkin = this.StickGoalieBlueSkin;
		stickGoalieBlueSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(stickGoalieBlueSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickSkinChanged));
		this.StickGoalieBlueSkin.Dispose();
		NetworkVariable<FixedString32Bytes> stickGoalieRedSkin = this.StickGoalieRedSkin;
		stickGoalieRedSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(stickGoalieRedSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickSkinChanged));
		this.StickGoalieRedSkin.Dispose();
		NetworkVariable<FixedString32Bytes> stickShaftAttackerBlueTapeSkin = this.StickShaftAttackerBlueTapeSkin;
		stickShaftAttackerBlueTapeSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(stickShaftAttackerBlueTapeSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickShaftTapeSkinChanged));
		this.StickShaftAttackerBlueTapeSkin.Dispose();
		NetworkVariable<FixedString32Bytes> stickShaftAttackerRedTapeSkin = this.StickShaftAttackerRedTapeSkin;
		stickShaftAttackerRedTapeSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(stickShaftAttackerRedTapeSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickShaftTapeSkinChanged));
		this.StickShaftAttackerRedTapeSkin.Dispose();
		NetworkVariable<FixedString32Bytes> stickShaftGoalieBlueTapeSkin = this.StickShaftGoalieBlueTapeSkin;
		stickShaftGoalieBlueTapeSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(stickShaftGoalieBlueTapeSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickShaftTapeSkinChanged));
		this.StickShaftGoalieBlueTapeSkin.Dispose();
		NetworkVariable<FixedString32Bytes> stickShaftGoalieRedTapeSkin = this.StickShaftGoalieRedTapeSkin;
		stickShaftGoalieRedTapeSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(stickShaftGoalieRedTapeSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickShaftTapeSkinChanged));
		this.StickShaftGoalieRedTapeSkin.Dispose();
		NetworkVariable<FixedString32Bytes> stickBladeAttackerBlueTapeSkin = this.StickBladeAttackerBlueTapeSkin;
		stickBladeAttackerBlueTapeSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(stickBladeAttackerBlueTapeSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickBladeTapeSkinChanged));
		this.StickBladeAttackerBlueTapeSkin.Dispose();
		NetworkVariable<FixedString32Bytes> stickBladeAttackerRedTapeSkin = this.StickBladeAttackerRedTapeSkin;
		stickBladeAttackerRedTapeSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(stickBladeAttackerRedTapeSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickBladeTapeSkinChanged));
		this.StickBladeAttackerRedTapeSkin.Dispose();
		NetworkVariable<FixedString32Bytes> stickBladeGoalieBlueTapeSkin = this.StickBladeGoalieBlueTapeSkin;
		stickBladeGoalieBlueTapeSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(stickBladeGoalieBlueTapeSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickBladeTapeSkinChanged));
		this.StickBladeGoalieBlueTapeSkin.Dispose();
		NetworkVariable<FixedString32Bytes> stickBladeGoalieRedTapeSkin = this.StickBladeGoalieRedTapeSkin;
		stickBladeGoalieRedTapeSkin.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(stickBladeGoalieRedTapeSkin.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerStickBladeTapeSkinChanged));
		this.StickBladeGoalieRedTapeSkin.Dispose();
		NetworkVariable<int> patreonLevel = this.PatreonLevel;
		patreonLevel.OnValueChanged = (NetworkVariable<int>.OnValueChangedDelegate)Delegate.Remove(patreonLevel.OnValueChanged, new NetworkVariable<int>.OnValueChangedDelegate(this.OnPlayerPatreonLevelChanged));
		this.PatreonLevel.Dispose();
		NetworkVariable<int> adminLevel = this.AdminLevel;
		adminLevel.OnValueChanged = (NetworkVariable<int>.OnValueChangedDelegate)Delegate.Remove(adminLevel.OnValueChanged, new NetworkVariable<int>.OnValueChangedDelegate(this.OnPlayerAdminLevelChanged));
		this.AdminLevel.Dispose();
		NetworkVariable<FixedString32Bytes> steamId = this.SteamId;
		steamId.OnValueChanged = (NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate)Delegate.Remove(steamId.OnValueChanged, new NetworkVariable<FixedString32Bytes>.OnValueChangedDelegate(this.OnPlayerSteamIdChanged));
		this.SteamId.Dispose();
		Debug.Log(string.Format("[Player] Despawned player ({0})", base.OwnerClientId));
		base.OnNetworkDespawn();
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x0002FED8 File Offset: 0x0002E0D8
	public FixedString32Bytes GetPlayerJerseySkin()
	{
		PlayerTeam value = this.Team.Value;
		if (value != PlayerTeam.Blue)
		{
			if (value != PlayerTeam.Red)
			{
				return default(FixedString32Bytes);
			}
			if (this.Role.Value != PlayerRole.Attacker)
			{
				return this.JerseyGoalieRedSkin.Value;
			}
			return this.JerseyAttackerRedSkin.Value;
		}
		else
		{
			if (this.Role.Value != PlayerRole.Attacker)
			{
				return this.JerseyGoalieBlueSkin.Value;
			}
			return this.JerseyAttackerBlueSkin.Value;
		}
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x0002FF50 File Offset: 0x0002E150
	public FixedString32Bytes GetPlayerVisorSkin()
	{
		PlayerTeam value = this.Team.Value;
		if (value != PlayerTeam.Blue)
		{
			if (value != PlayerTeam.Red)
			{
				return default(FixedString32Bytes);
			}
			if (this.Role.Value != PlayerRole.Attacker)
			{
				return this.VisorGoalieRedSkin.Value;
			}
			return this.VisorAttackerRedSkin.Value;
		}
		else
		{
			if (this.Role.Value != PlayerRole.Attacker)
			{
				return this.VisorGoalieBlueSkin.Value;
			}
			return this.VisorAttackerBlueSkin.Value;
		}
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x0002FFC8 File Offset: 0x0002E1C8
	public FixedString32Bytes GetPlayerStickSkin()
	{
		PlayerTeam value = this.Team.Value;
		if (value != PlayerTeam.Blue)
		{
			if (value != PlayerTeam.Red)
			{
				return default(FixedString32Bytes);
			}
			if (this.Role.Value != PlayerRole.Attacker)
			{
				return this.StickGoalieRedSkin.Value;
			}
			return this.StickAttackerRedSkin.Value;
		}
		else
		{
			if (this.Role.Value != PlayerRole.Attacker)
			{
				return this.StickGoalieBlueSkin.Value;
			}
			return this.StickAttackerBlueSkin.Value;
		}
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x00030040 File Offset: 0x0002E240
	public FixedString32Bytes GetPlayerStickShaftTapeSkin()
	{
		PlayerTeam value = this.Team.Value;
		if (value != PlayerTeam.Blue)
		{
			if (value != PlayerTeam.Red)
			{
				return default(FixedString32Bytes);
			}
			if (this.Role.Value != PlayerRole.Attacker)
			{
				return this.StickShaftGoalieRedTapeSkin.Value;
			}
			return this.StickShaftAttackerRedTapeSkin.Value;
		}
		else
		{
			if (this.Role.Value != PlayerRole.Attacker)
			{
				return this.StickShaftGoalieBlueTapeSkin.Value;
			}
			return this.StickShaftAttackerBlueTapeSkin.Value;
		}
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x000300B8 File Offset: 0x0002E2B8
	public FixedString32Bytes GetPlayerStickBladeTapeSkin()
	{
		PlayerTeam value = this.Team.Value;
		if (value != PlayerTeam.Blue)
		{
			if (value != PlayerTeam.Red)
			{
				return default(FixedString32Bytes);
			}
			if (this.Role.Value != PlayerRole.Attacker)
			{
				return this.StickBladeGoalieRedTapeSkin.Value;
			}
			return this.StickBladeAttackerRedTapeSkin.Value;
		}
		else
		{
			if (this.Role.Value != PlayerRole.Attacker)
			{
				return this.StickBladeGoalieBlueTapeSkin.Value;
			}
			return this.StickBladeAttackerBlueTapeSkin.Value;
		}
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x00030130 File Offset: 0x0002E330
	private void OnPlayerStateChanged(PlayerState oldState, PlayerState newState)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerStateChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldState",
				oldState
			},
			{
				"newState",
				newState
			}
		});
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x00030180 File Offset: 0x0002E380
	private void OnPlayerUsernameChanged(FixedString32Bytes oldUsername, FixedString32Bytes newUsername)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerUsernameChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldUsername",
				oldUsername
			},
			{
				"newUsername",
				newUsername
			}
		});
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x000301D0 File Offset: 0x0002E3D0
	private void OnPlayerNumberChanged(int oldNumber, int newNumber)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerNumberChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldNumber",
				oldNumber
			},
			{
				"newNumber",
				newNumber
			}
		});
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x00030220 File Offset: 0x0002E420
	private void OnPlayerHandednessChanged(PlayerHandedness oldHandedness, PlayerHandedness newHandedness)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerHandednessChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldHandedness",
				oldHandedness
			},
			{
				"newHandedness",
				newHandedness
			}
		});
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x00030270 File Offset: 0x0002E470
	private void OnPlayerTeamChanged(PlayerTeam oldTeam, PlayerTeam newTeam)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerTeamChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldTeam",
				oldTeam
			},
			{
				"newTeam",
				newTeam
			}
		});
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x000302C0 File Offset: 0x0002E4C0
	private void OnPlayerRoleChanged(PlayerRole oldRole, PlayerRole newRole)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerRoleChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldRole",
				oldRole
			},
			{
				"newRole",
				newRole
			}
		});
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x00030310 File Offset: 0x0002E510
	private void OnPlayerGoalsChanged(int oldGoals, int newGoals)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerGoalsChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldGoals",
				oldGoals
			},
			{
				"newGoals",
				newGoals
			}
		});
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x00030360 File Offset: 0x0002E560
	private void OnPlayerAssistsChanged(int oldAssists, int newAssists)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerAssistsChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldAssists",
				oldAssists
			},
			{
				"newAssists",
				newAssists
			}
		});
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x000303B0 File Offset: 0x0002E5B0
	private void OnPlayerPingChanged(int oldPing, int newPing)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerPingChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldPing",
				oldPing
			},
			{
				"newPing",
				newPing
			}
		});
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x00030400 File Offset: 0x0002E600
	private void OnPlayerPlayerPositionReferenceChanged(NetworkObjectReference oldPlayerPositionReference, NetworkObjectReference newPlayerPositionReference)
	{
		PlayerPosition playerPosition = this.PlayerPosition;
		NetworkObject networkObject;
		if (newPlayerPositionReference.TryGet(out networkObject, null))
		{
			this.PlayerPosition = networkObject.GetComponent<PlayerPosition>();
		}
		else
		{
			this.PlayerPosition = null;
		}
		this.OnPlayerPositionChanged(playerPosition, this.PlayerPosition);
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x0000BE8F File Offset: 0x0000A08F
	private void OnPlayerPositionChanged(PlayerPosition oldPlayerPosition, PlayerPosition newPlayerPosition)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerPositionChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldPlayerPosition",
				oldPlayerPosition
			},
			{
				"newPlayerPosition",
				newPlayerPosition
			}
		});
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x00030444 File Offset: 0x0002E644
	private void OnPlayerCountryChanged(FixedString32Bytes oldCountry, FixedString32Bytes newCountry)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerCountryChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldCountry",
				oldCountry
			},
			{
				"newCountry",
				newCountry
			}
		});
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x00030494 File Offset: 0x0002E694
	private void OnPlayerVisorChanged(FixedString32Bytes oldVisor, FixedString32Bytes newVisor)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerVisorChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldVisor",
				oldVisor
			},
			{
				"newVisor",
				newVisor
			}
		});
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x000304E4 File Offset: 0x0002E6E4
	private void OnPlayerMustacheChanged(FixedString32Bytes oldMustache, FixedString32Bytes newMustache)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerMustacheChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldMustache",
				oldMustache
			},
			{
				"newMustache",
				newMustache
			}
		});
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x00030534 File Offset: 0x0002E734
	private void OnPlayerBeardChanged(FixedString32Bytes oldBeard, FixedString32Bytes newBeard)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerBeardChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldBeard",
				oldBeard
			},
			{
				"newBeard",
				newBeard
			}
		});
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x00030584 File Offset: 0x0002E784
	private void OnPlayerJerseyChanged(FixedString32Bytes oldJersey, FixedString32Bytes newJersey)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerJerseyChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldJersey",
				oldJersey
			},
			{
				"newJersey",
				newJersey
			}
		});
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x000305D4 File Offset: 0x0002E7D4
	private void OnPlayerStickSkinChanged(FixedString32Bytes oldStickGoalieSkin, FixedString32Bytes newStickGoalieSkin)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerStickSkinChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldStickGoalieSkin",
				oldStickGoalieSkin
			},
			{
				"newStickGoalieSkin",
				newStickGoalieSkin
			}
		});
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x00030624 File Offset: 0x0002E824
	private void OnPlayerStickShaftTapeSkinChanged(FixedString32Bytes oldStickShaftTapeSkin, FixedString32Bytes newStickShaftTapeSkin)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerStickShaftTapeSkinChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldStickShaftTapeSkin",
				oldStickShaftTapeSkin
			},
			{
				"newStickShaftTapeSkin",
				newStickShaftTapeSkin
			}
		});
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x00030674 File Offset: 0x0002E874
	private void OnPlayerStickBladeTapeSkinChanged(FixedString32Bytes oldStickBladeTapeSkin, FixedString32Bytes newStickBladeTapeSkin)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerStickBladeTapeSkinChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldStickBladeTapeSkin",
				oldStickBladeTapeSkin
			},
			{
				"newStickBladeTapeSkin",
				newStickBladeTapeSkin
			}
		});
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x000306C4 File Offset: 0x0002E8C4
	private void OnPlayerPatreonLevelChanged(int oldPatreonLevel, int newPatreonLevel)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerPatreonLevelChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldPatreonLevel",
				oldPatreonLevel
			},
			{
				"newPatreonLevel",
				newPatreonLevel
			}
		});
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x00030714 File Offset: 0x0002E914
	private void OnPlayerAdminLevelChanged(int oldAdminLevel, int newAdminLevel)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerAdminLevelChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldAdminLevel",
				oldAdminLevel
			},
			{
				"newAdminLevel",
				newAdminLevel
			}
		});
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x00030764 File Offset: 0x0002E964
	private void OnPlayerSteamIdChanged(FixedString32Bytes oldSteamId, FixedString32Bytes newSteamId)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerSteamIdChanged", new Dictionary<string, object>
		{
			{
				"player",
				this
			},
			{
				"oldSteamId",
				oldSteamId
			},
			{
				"newSteamId",
				newSteamId
			}
		});
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x000307B4 File Offset: 0x0002E9B4
	public void Server_SpawnCharacter(Vector3 position, Quaternion rotation, PlayerRole role)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Server_SpawnPlayerBody(position, rotation, role);
		this.Server_SpawnStick(this.StickPositioner.RaycastOriginPosition, Quaternion.LookRotation(this.PlayerBody.transform.right, Vector3.up), role);
		Debug.Log(string.Format("[Player] Spawned character for {0} ({1})", this.Username.Value, base.OwnerClientId));
	}

	// Token: 0x060007FD RID: 2045 RVA: 0x00030830 File Offset: 0x0002EA30
	public void Server_SpawnPlayerCamera(PlayerBodyV2 playerBody)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		PlayerCamera playerCamera = UnityEngine.Object.Instantiate<PlayerCamera>(this.playerCameraPrefab);
		playerCamera.PlayerReference.Value = new NetworkObjectReference(base.NetworkObject);
		playerCamera.NetworkObject.SpawnWithOwnership(base.OwnerClientId, false);
		playerCamera.NetworkObject.TrySetParent(playerBody.transform, false);
		Debug.Log(string.Format("[Player] Spawned PlayerCamera for {0} ({1})", this.Username.Value, base.OwnerClientId));
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x000308BC File Offset: 0x0002EABC
	public void Server_SpawnPlayerBody(Vector3 position, Quaternion rotation, PlayerRole role)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		PlayerBodyV2 original;
		if (role == PlayerRole.Goalie)
		{
			original = this.playerBodyGoaliePrefab;
		}
		else
		{
			original = this.playerBodyAttackerPrefab;
		}
		PlayerBodyV2 playerBodyV = UnityEngine.Object.Instantiate<PlayerBodyV2>(original, position, rotation);
		playerBodyV.PlayerReference.Value = new NetworkObjectReference(base.NetworkObject);
		playerBodyV.NetworkObject.SpawnWithOwnership(base.OwnerClientId, false);
		playerBodyV.NetworkObject.TrySetParent(base.gameObject.transform, true);
		Debug.Log(string.Format("[Player] Spawned PlayerBody for {0} ({1})", this.Username.Value, base.OwnerClientId));
		this.Server_SpawnPlayerCamera(playerBodyV);
		this.Server_SpawnStickPositioner(playerBodyV);
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x0003096C File Offset: 0x0002EB6C
	public void Server_SpawnStickPositioner(PlayerBodyV2 playerBody)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		StickPositioner stickPositioner = UnityEngine.Object.Instantiate<StickPositioner>(this.stickPositionerPrefab);
		stickPositioner.PlayerReference.Value = new NetworkObjectReference(base.NetworkObject);
		stickPositioner.NetworkObject.SpawnWithOwnership(base.OwnerClientId, false);
		stickPositioner.NetworkObject.TrySetParent(playerBody.transform, false);
		Debug.Log(string.Format("[Player] Spawned StickPositioner for {0} ({1})", this.Username.Value, base.OwnerClientId));
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x000309F8 File Offset: 0x0002EBF8
	public void Server_SpawnStick(Vector3 position, Quaternion rotation, PlayerRole role)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		Stick original;
		if (role == PlayerRole.Goalie)
		{
			original = this.stickGoaliePrefab;
		}
		else
		{
			original = this.stickAttackerPrefab;
		}
		Stick stick = UnityEngine.Object.Instantiate<Stick>(original, position, rotation);
		stick.PlayerReference.Value = new NetworkObjectReference(base.NetworkObject);
		stick.NetworkObject.SpawnWithOwnership(base.OwnerClientId, false);
		stick.NetworkObject.TrySetParent(base.gameObject.transform, true);
		Debug.Log(string.Format("[Player] Spawned Stick for {0} ({1})", this.Username.Value, base.OwnerClientId));
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x0000BEC9 File Offset: 0x0000A0C9
	public void Server_DespawnCharacter()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Server_DespawnPlayerBody();
		this.Server_DespawnStick();
		Debug.Log(string.Format("[Player] Despawned character for ({0})", base.OwnerClientId));
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x00030A98 File Offset: 0x0002EC98
	public void Server_DespawnPlayerCamera()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.PlayerCamera && this.PlayerCamera.NetworkObject.IsSpawned)
		{
			this.PlayerCamera.NetworkObject.Despawn(true);
			Debug.Log(string.Format("[Player] Despawned PlayerCamera for ({0})", base.OwnerClientId));
		}
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x00030AFC File Offset: 0x0002ECFC
	public void Server_DespawnPlayerBody()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Server_DespawnPlayerCamera();
		this.Server_DespawnStickPositioner();
		if (this.PlayerBody && this.PlayerBody.NetworkObject.IsSpawned)
		{
			this.PlayerBody.NetworkObject.Despawn(true);
			Debug.Log(string.Format("[Player] Despawned PlayerBody for ({0})", base.OwnerClientId));
		}
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x00030B6C File Offset: 0x0002ED6C
	public void Server_DespawnStickPositioner()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.StickPositioner && this.StickPositioner.NetworkObject.IsSpawned)
		{
			this.StickPositioner.NetworkObject.Despawn(true);
			Debug.Log(string.Format("[Player] Despawned StickPositioner for ({0})", base.OwnerClientId));
		}
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x00030BD0 File Offset: 0x0002EDD0
	public void Server_DespawnStick()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.Stick && this.Stick.NetworkObject.IsSpawned)
		{
			this.Stick.NetworkObject.Despawn(true);
			Debug.Log(string.Format("[Player] Despawned Stick for ({0})", base.OwnerClientId));
		}
	}

	// Token: 0x06000806 RID: 2054 RVA: 0x00030C34 File Offset: 0x0002EE34
	public void Server_RespawnCharacter(Vector3 position, Quaternion rotation, PlayerRole role)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.IsCharacterPartiallySpawned)
		{
			this.Server_DespawnCharacter();
			this.Server_SpawnCharacter(position, rotation, role);
			Debug.Log(string.Format("[Player] Respawned character for {0} ({1})", this.Username.Value, base.OwnerClientId));
			return;
		}
		this.Server_SpawnCharacter(position, rotation, role);
	}

	// Token: 0x06000807 RID: 2055 RVA: 0x00030C9C File Offset: 0x0002EE9C
	public void Server_SpawnSpectatorCamera(Vector3 position, Quaternion rotation)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		SpectatorCamera spectatorCamera = UnityEngine.Object.Instantiate<SpectatorCamera>(this.spectatorCameraPrefab, position, rotation);
		spectatorCamera.PlayerReference.Value = new NetworkObjectReference(base.NetworkObject);
		spectatorCamera.NetworkObject.SpawnWithOwnership(base.OwnerClientId, false);
		spectatorCamera.NetworkObject.TrySetParent(base.gameObject.transform, true);
		Debug.Log(string.Format("[Player] Spawned spectator camera for {0} ({1})", this.Username.Value, base.OwnerClientId));
	}

	// Token: 0x06000808 RID: 2056 RVA: 0x00030D2C File Offset: 0x0002EF2C
	public void Server_RespawnSpectatorCamera(Vector3 position, Quaternion rotation)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.IsSpectatorCameraSpawned)
		{
			this.Server_DespawnSpectatorCamera();
			this.Server_SpawnSpectatorCamera(position, rotation);
			Debug.Log(string.Format("[Player] Respawned spectator camera for {0} ({1})", this.Username.Value, base.OwnerClientId));
			return;
		}
		this.Server_SpawnSpectatorCamera(position, rotation);
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x00030D90 File Offset: 0x0002EF90
	public void Server_DespawnSpectatorCamera()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.SpectatorCamera && this.SpectatorCamera.NetworkObject.IsSpawned)
		{
			this.SpectatorCamera.NetworkObject.Despawn(true);
			Debug.Log(string.Format("[Player] Despawned spectator camera for {0} ({1})", this.Username.Value, base.OwnerClientId));
		}
	}

	// Token: 0x0600080A RID: 2058 RVA: 0x00030E04 File Offset: 0x0002F004
	public void Server_GoalScored()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		NetworkVariable<int> goals = this.Goals;
		int value = goals.Value;
		goals.Value = value + 1;
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x00030E34 File Offset: 0x0002F034
	public void Server_AssistScored()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		NetworkVariable<int> assists = this.Assists;
		int value = assists.Value;
		assists.Value = value + 1;
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x0000BEFE File Offset: 0x0000A0FE
	public void Server_ResetPoints()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Goals.Value = 0;
		this.Assists.Value = 0;
	}

	// Token: 0x0600080D RID: 2061 RVA: 0x0000BF25 File Offset: 0x0000A125
	public void Server_UpdatePing()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Ping.Value = (int)NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(base.OwnerClientId);
	}

	// Token: 0x0600080E RID: 2062 RVA: 0x00030E64 File Offset: 0x0002F064
	[Rpc(SendTo.Server)]
	public void Client_SetPlayerStateRpc(PlayerState state, float delay = 0f)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2891939837U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<PlayerState>(state2, default(FastBufferWriter.ForEnums));
			fastBufferWriter.WriteValueSafe<float>(delay, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 2891939837U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		PlayerState state = state2;
		Tween tween = this.delayedStateTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		if (delay > 0f)
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnPlayerStateDelayed", new Dictionary<string, object>
			{
				{
					"player",
					this
				},
				{
					"oldState",
					this.State.Value
				},
				{
					"newState",
					state
				},
				{
					"delay",
					delay
				}
			});
			this.delayedStateTween = DOVirtual.DelayedCall(delay, delegate
			{
				this.Client_SetPlayerStateRpc(state, 0f);
			}, true);
			return;
		}
		switch (state)
		{
		case PlayerState.TeamSelect:
			this.Server_DespawnCharacter();
			this.Server_DespawnSpectatorCamera();
			this.Team.Value = PlayerTeam.None;
			break;
		case PlayerState.PositionSelectBlue:
		case PlayerState.PositionSelectRed:
			if (this.Team.Value == PlayerTeam.None || this.Team.Value == PlayerTeam.Spectator)
			{
				return;
			}
			this.Server_DespawnCharacter();
			this.Server_DespawnSpectatorCamera();
			if (this.PlayerPosition)
			{
				this.PlayerPosition.Server_Unclaim();
			}
			break;
		case PlayerState.Play:
			if (this.Team.Value == PlayerTeam.None || this.Team.Value == PlayerTeam.Spectator)
			{
				return;
			}
			if (this.PlayerPosition)
			{
				this.Server_RespawnCharacter(this.PlayerPosition.transform.position, this.PlayerPosition.transform.rotation, this.Role.Value);
			}
			break;
		case PlayerState.Replay:
			if (this.Team.Value == PlayerTeam.None || this.Team.Value == PlayerTeam.Spectator)
			{
				return;
			}
			this.Server_DespawnCharacter();
			break;
		case PlayerState.Spectate:
			this.Server_DespawnCharacter();
			this.Server_RespawnSpectatorCamera(Vector3.zero + Vector3.up, Quaternion.identity);
			break;
		}
		this.State.Value = state;
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x00031140 File Offset: 0x0002F340
	[Rpc(SendTo.Server)]
	public void Client_SetPlayerUsernameRpc(FixedString32Bytes username)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 946455295U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<FixedString32Bytes>(username, default(FastBufferWriter.ForFixedStrings));
			base.__endSendRpc(ref fastBufferWriter, 946455295U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.Username.Value = username;
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x00031220 File Offset: 0x0002F420
	[Rpc(SendTo.Server)]
	public void Client_SetPlayerNumberRpc(int number)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2614219144U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter writer = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			BytePacker.WriteValueBitPacked(writer, number);
			base.__endSendRpc(ref writer, 2614219144U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.Number.Value = number;
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x000312F0 File Offset: 0x0002F4F0
	[Rpc(SendTo.Server)]
	public void Client_SetPlayerHandednessRpc(PlayerHandedness handedness)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 1024064498U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<PlayerHandedness>(handedness, default(FastBufferWriter.ForEnums));
			base.__endSendRpc(ref fastBufferWriter, 1024064498U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.Handedness.Value = handedness;
	}

	// Token: 0x06000812 RID: 2066 RVA: 0x000313D0 File Offset: 0x0002F5D0
	[Rpc(SendTo.Server)]
	public void Client_SetPlayerTeamRpc(PlayerTeam team)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2680549476U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<PlayerTeam>(team, default(FastBufferWriter.ForEnums));
			base.__endSendRpc(ref fastBufferWriter, 2680549476U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.Team.Value = team;
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x000314B0 File Offset: 0x0002F6B0
	[Rpc(SendTo.Server)]
	private void Client_PlayerSubscriptionRpc(FixedString32Bytes username, int number, PlayerHandedness handedness, FixedString32Bytes country, FixedString32Bytes visorAttackerBlueSkin, FixedString32Bytes visorAttackerRedSkin, FixedString32Bytes visorGoalieBlueSkin, FixedString32Bytes visorGoalieRedSkin, FixedString32Bytes mustache, FixedString32Bytes beard, FixedString32Bytes jerseyAttackerBlueSkin, FixedString32Bytes jerseyAttackerRedSkin, FixedString32Bytes jerseyGoalieBlueSkin, FixedString32Bytes jerseyGoalieRedSkin, FixedString32Bytes stickAttackerBlueSkin, FixedString32Bytes stickAttackerRedSkin, FixedString32Bytes stickGoalieBlueSkin, FixedString32Bytes stickGoalieRedSkin, FixedString32Bytes stickShaftAttackerBlueTapeSkin, FixedString32Bytes stickShaftAttackerRedTapeSkin, FixedString32Bytes stickShaftGoalieBlueTapeSkin, FixedString32Bytes stickShaftGoalieRedTapeSkin, FixedString32Bytes stickBladeAttackerBlueTapeSkin, FixedString32Bytes stickBladeAttackerRedTapeSkin, FixedString32Bytes stickBladeGoalieBlueTapeSkin, FixedString32Bytes stickBladeGoalieRedTapeSkin, int patreonLevel, int adminLevel, FixedString32Bytes steamId, ulong[] enabledModIds)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 1379186733U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter writer = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			writer.WriteValueSafe<FixedString32Bytes>(username, default(FastBufferWriter.ForFixedStrings));
			BytePacker.WriteValueBitPacked(writer, number);
			writer.WriteValueSafe<PlayerHandedness>(handedness, default(FastBufferWriter.ForEnums));
			writer.WriteValueSafe<FixedString32Bytes>(country, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(visorAttackerBlueSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(visorAttackerRedSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(visorGoalieBlueSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(visorGoalieRedSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(mustache, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(beard, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(jerseyAttackerBlueSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(jerseyAttackerRedSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(jerseyGoalieBlueSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(jerseyGoalieRedSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(stickAttackerBlueSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(stickAttackerRedSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(stickGoalieBlueSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(stickGoalieRedSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(stickShaftAttackerBlueTapeSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(stickShaftAttackerRedTapeSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(stickShaftGoalieBlueTapeSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(stickShaftGoalieRedTapeSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(stickBladeAttackerBlueTapeSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(stickBladeAttackerRedTapeSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(stickBladeGoalieBlueTapeSkin, default(FastBufferWriter.ForFixedStrings));
			writer.WriteValueSafe<FixedString32Bytes>(stickBladeGoalieRedTapeSkin, default(FastBufferWriter.ForFixedStrings));
			BytePacker.WriteValueBitPacked(writer, patreonLevel);
			BytePacker.WriteValueBitPacked(writer, adminLevel);
			writer.WriteValueSafe<FixedString32Bytes>(steamId, default(FastBufferWriter.ForFixedStrings));
			bool flag = enabledModIds != null;
			writer.WriteValueSafe<bool>(flag, default(FastBufferWriter.ForPrimitives));
			if (flag)
			{
				writer.WriteValueSafe<ulong>(enabledModIds, default(FastBufferWriter.ForPrimitives));
			}
			base.__endSendRpc(ref writer, 1379186733U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.Username.Value = username;
		this.Number.Value = number;
		this.Handedness.Value = handedness;
		this.Country.Value = country;
		this.VisorAttackerBlueSkin.Value = visorAttackerBlueSkin;
		this.VisorAttackerRedSkin.Value = visorAttackerRedSkin;
		this.VisorGoalieBlueSkin.Value = visorGoalieBlueSkin;
		this.VisorGoalieRedSkin.Value = visorGoalieRedSkin;
		this.Mustache.Value = mustache;
		this.Beard.Value = beard;
		this.JerseyAttackerBlueSkin.Value = jerseyAttackerBlueSkin;
		this.JerseyAttackerRedSkin.Value = jerseyAttackerRedSkin;
		this.JerseyGoalieBlueSkin.Value = jerseyGoalieBlueSkin;
		this.JerseyGoalieRedSkin.Value = jerseyGoalieRedSkin;
		this.StickAttackerBlueSkin.Value = stickAttackerBlueSkin;
		this.StickAttackerRedSkin.Value = stickAttackerRedSkin;
		this.StickGoalieBlueSkin.Value = stickGoalieBlueSkin;
		this.StickGoalieRedSkin.Value = stickGoalieRedSkin;
		this.StickShaftAttackerBlueTapeSkin.Value = stickShaftAttackerBlueTapeSkin;
		this.StickShaftAttackerRedTapeSkin.Value = stickShaftAttackerRedTapeSkin;
		this.StickShaftGoalieBlueTapeSkin.Value = stickShaftGoalieBlueTapeSkin;
		this.StickShaftGoalieRedTapeSkin.Value = stickShaftGoalieRedTapeSkin;
		this.StickBladeAttackerBlueTapeSkin.Value = stickBladeAttackerBlueTapeSkin;
		this.StickBladeAttackerRedTapeSkin.Value = stickBladeAttackerRedTapeSkin;
		this.StickBladeGoalieBlueTapeSkin.Value = stickBladeGoalieBlueTapeSkin;
		this.StickBladeGoalieRedTapeSkin.Value = stickBladeGoalieRedTapeSkin;
		this.PatreonLevel.Value = patreonLevel;
		this.AdminLevel.Value = adminLevel;
		this.SteamId.Value = steamId;
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnPlayerSubscription", new Dictionary<string, object>
		{
			{
				"player",
				this
			}
		});
		Debug.Log(string.Format("[Player] Received subscription from {0} ({1})", username, base.OwnerClientId));
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x00031A50 File Offset: 0x0002FC50
	public void Client_InitializeNetworkVariables()
	{
		this.OnPlayerStateChanged(this.State.Value, this.State.Value);
		this.OnPlayerUsernameChanged(this.Username.Value, this.Username.Value);
		this.OnPlayerNumberChanged(this.Number.Value, this.Number.Value);
		this.OnPlayerHandednessChanged(this.Handedness.Value, this.Handedness.Value);
		this.OnPlayerTeamChanged(this.Team.Value, this.Team.Value);
		this.OnPlayerRoleChanged(this.Role.Value, this.Role.Value);
		this.OnPlayerGoalsChanged(this.Goals.Value, this.Goals.Value);
		this.OnPlayerAssistsChanged(this.Assists.Value, this.Assists.Value);
		this.OnPlayerPingChanged(this.Ping.Value, this.Ping.Value);
		this.OnPlayerPlayerPositionReferenceChanged(this.PlayerPositionReference.Value, this.PlayerPositionReference.Value);
		this.OnPlayerCountryChanged(this.Country.Value, this.Country.Value);
		PlayerTeam value = this.Team.Value;
		if (value != PlayerTeam.Blue)
		{
			if (value == PlayerTeam.Red)
			{
				PlayerRole value2 = this.Role.Value;
				if (value2 != PlayerRole.Attacker)
				{
					if (value2 == PlayerRole.Goalie)
					{
						this.OnPlayerVisorChanged(this.VisorGoalieRedSkin.Value, this.VisorGoalieRedSkin.Value);
					}
				}
				else
				{
					this.OnPlayerVisorChanged(this.VisorAttackerRedSkin.Value, this.VisorAttackerRedSkin.Value);
				}
			}
		}
		else
		{
			PlayerRole value2 = this.Role.Value;
			if (value2 != PlayerRole.Attacker)
			{
				if (value2 == PlayerRole.Goalie)
				{
					this.OnPlayerVisorChanged(this.VisorGoalieBlueSkin.Value, this.VisorGoalieBlueSkin.Value);
				}
			}
			else
			{
				this.OnPlayerVisorChanged(this.VisorAttackerBlueSkin.Value, this.VisorAttackerBlueSkin.Value);
			}
		}
		this.OnPlayerMustacheChanged(this.Mustache.Value, this.Mustache.Value);
		this.OnPlayerBeardChanged(this.Beard.Value, this.Beard.Value);
		value = this.Team.Value;
		if (value != PlayerTeam.Blue)
		{
			if (value == PlayerTeam.Red)
			{
				PlayerRole value2 = this.Role.Value;
				if (value2 != PlayerRole.Attacker)
				{
					if (value2 == PlayerRole.Goalie)
					{
						this.OnPlayerJerseyChanged(this.JerseyGoalieRedSkin.Value, this.JerseyGoalieRedSkin.Value);
					}
				}
				else
				{
					this.OnPlayerJerseyChanged(this.JerseyAttackerRedSkin.Value, this.JerseyAttackerRedSkin.Value);
				}
			}
		}
		else
		{
			PlayerRole value2 = this.Role.Value;
			if (value2 != PlayerRole.Attacker)
			{
				if (value2 == PlayerRole.Goalie)
				{
					this.OnPlayerJerseyChanged(this.JerseyGoalieBlueSkin.Value, this.JerseyGoalieBlueSkin.Value);
				}
			}
			else
			{
				this.OnPlayerJerseyChanged(this.JerseyAttackerBlueSkin.Value, this.JerseyAttackerBlueSkin.Value);
			}
		}
		value = this.Team.Value;
		if (value != PlayerTeam.Blue)
		{
			if (value == PlayerTeam.Red)
			{
				PlayerRole value2 = this.Role.Value;
				if (value2 != PlayerRole.Attacker)
				{
					if (value2 == PlayerRole.Goalie)
					{
						this.OnPlayerStickSkinChanged(this.StickGoalieRedSkin.Value, this.StickGoalieRedSkin.Value);
					}
				}
				else
				{
					this.OnPlayerStickSkinChanged(this.StickAttackerRedSkin.Value, this.StickAttackerRedSkin.Value);
				}
			}
		}
		else
		{
			PlayerRole value2 = this.Role.Value;
			if (value2 != PlayerRole.Attacker)
			{
				if (value2 == PlayerRole.Goalie)
				{
					this.OnPlayerStickSkinChanged(this.StickGoalieBlueSkin.Value, this.StickGoalieBlueSkin.Value);
				}
			}
			else
			{
				this.OnPlayerStickSkinChanged(this.StickAttackerBlueSkin.Value, this.StickAttackerBlueSkin.Value);
			}
		}
		value = this.Team.Value;
		if (value != PlayerTeam.Blue)
		{
			if (value == PlayerTeam.Red)
			{
				PlayerRole value2 = this.Role.Value;
				if (value2 != PlayerRole.Attacker)
				{
					if (value2 == PlayerRole.Goalie)
					{
						this.OnPlayerStickShaftTapeSkinChanged(this.StickShaftGoalieRedTapeSkin.Value, this.StickShaftGoalieRedTapeSkin.Value);
					}
				}
				else
				{
					this.OnPlayerStickShaftTapeSkinChanged(this.StickShaftAttackerRedTapeSkin.Value, this.StickShaftAttackerRedTapeSkin.Value);
				}
			}
		}
		else
		{
			PlayerRole value2 = this.Role.Value;
			if (value2 != PlayerRole.Attacker)
			{
				if (value2 == PlayerRole.Goalie)
				{
					this.OnPlayerStickShaftTapeSkinChanged(this.StickShaftGoalieBlueTapeSkin.Value, this.StickShaftGoalieBlueTapeSkin.Value);
				}
			}
			else
			{
				this.OnPlayerStickShaftTapeSkinChanged(this.StickShaftAttackerBlueTapeSkin.Value, this.StickShaftAttackerBlueTapeSkin.Value);
			}
		}
		value = this.Team.Value;
		if (value != PlayerTeam.Blue)
		{
			if (value == PlayerTeam.Red)
			{
				PlayerRole value2 = this.Role.Value;
				if (value2 != PlayerRole.Attacker)
				{
					if (value2 == PlayerRole.Goalie)
					{
						this.OnPlayerStickBladeTapeSkinChanged(this.StickBladeGoalieRedTapeSkin.Value, this.StickBladeGoalieRedTapeSkin.Value);
					}
				}
				else
				{
					this.OnPlayerStickBladeTapeSkinChanged(this.StickBladeAttackerRedTapeSkin.Value, this.StickBladeAttackerRedTapeSkin.Value);
				}
			}
		}
		else
		{
			PlayerRole value2 = this.Role.Value;
			if (value2 != PlayerRole.Attacker)
			{
				if (value2 == PlayerRole.Goalie)
				{
					this.OnPlayerStickBladeTapeSkinChanged(this.StickBladeGoalieBlueTapeSkin.Value, this.StickBladeGoalieBlueTapeSkin.Value);
				}
			}
			else
			{
				this.OnPlayerStickBladeTapeSkinChanged(this.StickBladeAttackerBlueTapeSkin.Value, this.StickBladeAttackerBlueTapeSkin.Value);
			}
		}
		this.OnPlayerPatreonLevelChanged(this.PatreonLevel.Value, this.PatreonLevel.Value);
		this.OnPlayerAdminLevelChanged(this.AdminLevel.Value, this.AdminLevel.Value);
		this.OnPlayerSteamIdChanged(this.SteamId.Value, this.SteamId.Value);
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x000322C0 File Offset: 0x000304C0
	protected override void __initializeVariables()
	{
		bool flag = this.State == null;
		if (flag)
		{
			throw new Exception("Player.State cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.State.Initialize(this);
		base.__nameNetworkVariable(this.State, "State");
		this.NetworkVariableFields.Add(this.State);
		flag = (this.Username == null);
		if (flag)
		{
			throw new Exception("Player.Username cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.Username.Initialize(this);
		base.__nameNetworkVariable(this.Username, "Username");
		this.NetworkVariableFields.Add(this.Username);
		flag = (this.Number == null);
		if (flag)
		{
			throw new Exception("Player.Number cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.Number.Initialize(this);
		base.__nameNetworkVariable(this.Number, "Number");
		this.NetworkVariableFields.Add(this.Number);
		flag = (this.Handedness == null);
		if (flag)
		{
			throw new Exception("Player.Handedness cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.Handedness.Initialize(this);
		base.__nameNetworkVariable(this.Handedness, "Handedness");
		this.NetworkVariableFields.Add(this.Handedness);
		flag = (this.Team == null);
		if (flag)
		{
			throw new Exception("Player.Team cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.Team.Initialize(this);
		base.__nameNetworkVariable(this.Team, "Team");
		this.NetworkVariableFields.Add(this.Team);
		flag = (this.Role == null);
		if (flag)
		{
			throw new Exception("Player.Role cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.Role.Initialize(this);
		base.__nameNetworkVariable(this.Role, "Role");
		this.NetworkVariableFields.Add(this.Role);
		flag = (this.Goals == null);
		if (flag)
		{
			throw new Exception("Player.Goals cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.Goals.Initialize(this);
		base.__nameNetworkVariable(this.Goals, "Goals");
		this.NetworkVariableFields.Add(this.Goals);
		flag = (this.Assists == null);
		if (flag)
		{
			throw new Exception("Player.Assists cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.Assists.Initialize(this);
		base.__nameNetworkVariable(this.Assists, "Assists");
		this.NetworkVariableFields.Add(this.Assists);
		flag = (this.Ping == null);
		if (flag)
		{
			throw new Exception("Player.Ping cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.Ping.Initialize(this);
		base.__nameNetworkVariable(this.Ping, "Ping");
		this.NetworkVariableFields.Add(this.Ping);
		flag = (this.PlayerPositionReference == null);
		if (flag)
		{
			throw new Exception("Player.PlayerPositionReference cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.PlayerPositionReference.Initialize(this);
		base.__nameNetworkVariable(this.PlayerPositionReference, "PlayerPositionReference");
		this.NetworkVariableFields.Add(this.PlayerPositionReference);
		flag = (this.IsReplay == null);
		if (flag)
		{
			throw new Exception("Player.IsReplay cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.IsReplay.Initialize(this);
		base.__nameNetworkVariable(this.IsReplay, "IsReplay");
		this.NetworkVariableFields.Add(this.IsReplay);
		flag = (this.Country == null);
		if (flag)
		{
			throw new Exception("Player.Country cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.Country.Initialize(this);
		base.__nameNetworkVariable(this.Country, "Country");
		this.NetworkVariableFields.Add(this.Country);
		flag = (this.VisorAttackerBlueSkin == null);
		if (flag)
		{
			throw new Exception("Player.VisorAttackerBlueSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.VisorAttackerBlueSkin.Initialize(this);
		base.__nameNetworkVariable(this.VisorAttackerBlueSkin, "VisorAttackerBlueSkin");
		this.NetworkVariableFields.Add(this.VisorAttackerBlueSkin);
		flag = (this.VisorAttackerRedSkin == null);
		if (flag)
		{
			throw new Exception("Player.VisorAttackerRedSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.VisorAttackerRedSkin.Initialize(this);
		base.__nameNetworkVariable(this.VisorAttackerRedSkin, "VisorAttackerRedSkin");
		this.NetworkVariableFields.Add(this.VisorAttackerRedSkin);
		flag = (this.VisorGoalieBlueSkin == null);
		if (flag)
		{
			throw new Exception("Player.VisorGoalieBlueSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.VisorGoalieBlueSkin.Initialize(this);
		base.__nameNetworkVariable(this.VisorGoalieBlueSkin, "VisorGoalieBlueSkin");
		this.NetworkVariableFields.Add(this.VisorGoalieBlueSkin);
		flag = (this.VisorGoalieRedSkin == null);
		if (flag)
		{
			throw new Exception("Player.VisorGoalieRedSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.VisorGoalieRedSkin.Initialize(this);
		base.__nameNetworkVariable(this.VisorGoalieRedSkin, "VisorGoalieRedSkin");
		this.NetworkVariableFields.Add(this.VisorGoalieRedSkin);
		flag = (this.Mustache == null);
		if (flag)
		{
			throw new Exception("Player.Mustache cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.Mustache.Initialize(this);
		base.__nameNetworkVariable(this.Mustache, "Mustache");
		this.NetworkVariableFields.Add(this.Mustache);
		flag = (this.Beard == null);
		if (flag)
		{
			throw new Exception("Player.Beard cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.Beard.Initialize(this);
		base.__nameNetworkVariable(this.Beard, "Beard");
		this.NetworkVariableFields.Add(this.Beard);
		flag = (this.JerseyAttackerBlueSkin == null);
		if (flag)
		{
			throw new Exception("Player.JerseyAttackerBlueSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.JerseyAttackerBlueSkin.Initialize(this);
		base.__nameNetworkVariable(this.JerseyAttackerBlueSkin, "JerseyAttackerBlueSkin");
		this.NetworkVariableFields.Add(this.JerseyAttackerBlueSkin);
		flag = (this.JerseyAttackerRedSkin == null);
		if (flag)
		{
			throw new Exception("Player.JerseyAttackerRedSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.JerseyAttackerRedSkin.Initialize(this);
		base.__nameNetworkVariable(this.JerseyAttackerRedSkin, "JerseyAttackerRedSkin");
		this.NetworkVariableFields.Add(this.JerseyAttackerRedSkin);
		flag = (this.JerseyGoalieBlueSkin == null);
		if (flag)
		{
			throw new Exception("Player.JerseyGoalieBlueSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.JerseyGoalieBlueSkin.Initialize(this);
		base.__nameNetworkVariable(this.JerseyGoalieBlueSkin, "JerseyGoalieBlueSkin");
		this.NetworkVariableFields.Add(this.JerseyGoalieBlueSkin);
		flag = (this.JerseyGoalieRedSkin == null);
		if (flag)
		{
			throw new Exception("Player.JerseyGoalieRedSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.JerseyGoalieRedSkin.Initialize(this);
		base.__nameNetworkVariable(this.JerseyGoalieRedSkin, "JerseyGoalieRedSkin");
		this.NetworkVariableFields.Add(this.JerseyGoalieRedSkin);
		flag = (this.StickAttackerBlueSkin == null);
		if (flag)
		{
			throw new Exception("Player.StickAttackerBlueSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.StickAttackerBlueSkin.Initialize(this);
		base.__nameNetworkVariable(this.StickAttackerBlueSkin, "StickAttackerBlueSkin");
		this.NetworkVariableFields.Add(this.StickAttackerBlueSkin);
		flag = (this.StickAttackerRedSkin == null);
		if (flag)
		{
			throw new Exception("Player.StickAttackerRedSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.StickAttackerRedSkin.Initialize(this);
		base.__nameNetworkVariable(this.StickAttackerRedSkin, "StickAttackerRedSkin");
		this.NetworkVariableFields.Add(this.StickAttackerRedSkin);
		flag = (this.StickGoalieBlueSkin == null);
		if (flag)
		{
			throw new Exception("Player.StickGoalieBlueSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.StickGoalieBlueSkin.Initialize(this);
		base.__nameNetworkVariable(this.StickGoalieBlueSkin, "StickGoalieBlueSkin");
		this.NetworkVariableFields.Add(this.StickGoalieBlueSkin);
		flag = (this.StickGoalieRedSkin == null);
		if (flag)
		{
			throw new Exception("Player.StickGoalieRedSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.StickGoalieRedSkin.Initialize(this);
		base.__nameNetworkVariable(this.StickGoalieRedSkin, "StickGoalieRedSkin");
		this.NetworkVariableFields.Add(this.StickGoalieRedSkin);
		flag = (this.StickShaftAttackerBlueTapeSkin == null);
		if (flag)
		{
			throw new Exception("Player.StickShaftAttackerBlueTapeSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.StickShaftAttackerBlueTapeSkin.Initialize(this);
		base.__nameNetworkVariable(this.StickShaftAttackerBlueTapeSkin, "StickShaftAttackerBlueTapeSkin");
		this.NetworkVariableFields.Add(this.StickShaftAttackerBlueTapeSkin);
		flag = (this.StickShaftAttackerRedTapeSkin == null);
		if (flag)
		{
			throw new Exception("Player.StickShaftAttackerRedTapeSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.StickShaftAttackerRedTapeSkin.Initialize(this);
		base.__nameNetworkVariable(this.StickShaftAttackerRedTapeSkin, "StickShaftAttackerRedTapeSkin");
		this.NetworkVariableFields.Add(this.StickShaftAttackerRedTapeSkin);
		flag = (this.StickShaftGoalieBlueTapeSkin == null);
		if (flag)
		{
			throw new Exception("Player.StickShaftGoalieBlueTapeSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.StickShaftGoalieBlueTapeSkin.Initialize(this);
		base.__nameNetworkVariable(this.StickShaftGoalieBlueTapeSkin, "StickShaftGoalieBlueTapeSkin");
		this.NetworkVariableFields.Add(this.StickShaftGoalieBlueTapeSkin);
		flag = (this.StickShaftGoalieRedTapeSkin == null);
		if (flag)
		{
			throw new Exception("Player.StickShaftGoalieRedTapeSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.StickShaftGoalieRedTapeSkin.Initialize(this);
		base.__nameNetworkVariable(this.StickShaftGoalieRedTapeSkin, "StickShaftGoalieRedTapeSkin");
		this.NetworkVariableFields.Add(this.StickShaftGoalieRedTapeSkin);
		flag = (this.StickBladeAttackerBlueTapeSkin == null);
		if (flag)
		{
			throw new Exception("Player.StickBladeAttackerBlueTapeSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.StickBladeAttackerBlueTapeSkin.Initialize(this);
		base.__nameNetworkVariable(this.StickBladeAttackerBlueTapeSkin, "StickBladeAttackerBlueTapeSkin");
		this.NetworkVariableFields.Add(this.StickBladeAttackerBlueTapeSkin);
		flag = (this.StickBladeAttackerRedTapeSkin == null);
		if (flag)
		{
			throw new Exception("Player.StickBladeAttackerRedTapeSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.StickBladeAttackerRedTapeSkin.Initialize(this);
		base.__nameNetworkVariable(this.StickBladeAttackerRedTapeSkin, "StickBladeAttackerRedTapeSkin");
		this.NetworkVariableFields.Add(this.StickBladeAttackerRedTapeSkin);
		flag = (this.StickBladeGoalieBlueTapeSkin == null);
		if (flag)
		{
			throw new Exception("Player.StickBladeGoalieBlueTapeSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.StickBladeGoalieBlueTapeSkin.Initialize(this);
		base.__nameNetworkVariable(this.StickBladeGoalieBlueTapeSkin, "StickBladeGoalieBlueTapeSkin");
		this.NetworkVariableFields.Add(this.StickBladeGoalieBlueTapeSkin);
		flag = (this.StickBladeGoalieRedTapeSkin == null);
		if (flag)
		{
			throw new Exception("Player.StickBladeGoalieRedTapeSkin cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.StickBladeGoalieRedTapeSkin.Initialize(this);
		base.__nameNetworkVariable(this.StickBladeGoalieRedTapeSkin, "StickBladeGoalieRedTapeSkin");
		this.NetworkVariableFields.Add(this.StickBladeGoalieRedTapeSkin);
		flag = (this.PatreonLevel == null);
		if (flag)
		{
			throw new Exception("Player.PatreonLevel cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.PatreonLevel.Initialize(this);
		base.__nameNetworkVariable(this.PatreonLevel, "PatreonLevel");
		this.NetworkVariableFields.Add(this.PatreonLevel);
		flag = (this.AdminLevel == null);
		if (flag)
		{
			throw new Exception("Player.AdminLevel cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.AdminLevel.Initialize(this);
		base.__nameNetworkVariable(this.AdminLevel, "AdminLevel");
		this.NetworkVariableFields.Add(this.AdminLevel);
		flag = (this.SteamId == null);
		if (flag)
		{
			throw new Exception("Player.SteamId cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.SteamId.Initialize(this);
		base.__nameNetworkVariable(this.SteamId, "SteamId");
		this.NetworkVariableFields.Add(this.SteamId);
		base.__initializeVariables();
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x00032DF8 File Offset: 0x00030FF8
	protected override void __initializeRpcs()
	{
		base.__registerRpc(2891939837U, new NetworkBehaviour.RpcReceiveHandler(Player.__rpc_handler_2891939837), "Client_SetPlayerStateRpc");
		base.__registerRpc(946455295U, new NetworkBehaviour.RpcReceiveHandler(Player.__rpc_handler_946455295), "Client_SetPlayerUsernameRpc");
		base.__registerRpc(2614219144U, new NetworkBehaviour.RpcReceiveHandler(Player.__rpc_handler_2614219144), "Client_SetPlayerNumberRpc");
		base.__registerRpc(1024064498U, new NetworkBehaviour.RpcReceiveHandler(Player.__rpc_handler_1024064498), "Client_SetPlayerHandednessRpc");
		base.__registerRpc(2680549476U, new NetworkBehaviour.RpcReceiveHandler(Player.__rpc_handler_2680549476), "Client_SetPlayerTeamRpc");
		base.__registerRpc(1379186733U, new NetworkBehaviour.RpcReceiveHandler(Player.__rpc_handler_1379186733), "Client_PlayerSubscriptionRpc");
		base.__initializeRpcs();
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x00032EB8 File Offset: 0x000310B8
	private static void __rpc_handler_2891939837(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		PlayerState state;
		reader.ReadValueSafe<PlayerState>(out state, default(FastBufferWriter.ForEnums));
		float delay;
		reader.ReadValueSafe<float>(out delay, default(FastBufferWriter.ForPrimitives));
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((Player)target).Client_SetPlayerStateRpc(state, delay);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000819 RID: 2073 RVA: 0x00032F48 File Offset: 0x00031148
	private static void __rpc_handler_946455295(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		FixedString32Bytes username;
		reader.ReadValueSafe<FixedString32Bytes>(out username, default(FastBufferWriter.ForFixedStrings));
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((Player)target).Client_SetPlayerUsernameRpc(username);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x00032FB8 File Offset: 0x000311B8
	private static void __rpc_handler_2614219144(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		int number;
		ByteUnpacker.ReadValueBitPacked(reader, out number);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((Player)target).Client_SetPlayerNumberRpc(number);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x0003301C File Offset: 0x0003121C
	private static void __rpc_handler_1024064498(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		PlayerHandedness handedness;
		reader.ReadValueSafe<PlayerHandedness>(out handedness, default(FastBufferWriter.ForEnums));
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((Player)target).Client_SetPlayerHandednessRpc(handedness);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x0003308C File Offset: 0x0003128C
	private static void __rpc_handler_2680549476(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		PlayerTeam team;
		reader.ReadValueSafe<PlayerTeam>(out team, default(FastBufferWriter.ForEnums));
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((Player)target).Client_SetPlayerTeamRpc(team);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x000330FC File Offset: 0x000312FC
	private static void __rpc_handler_1379186733(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		FixedString32Bytes username;
		reader.ReadValueSafe<FixedString32Bytes>(out username, default(FastBufferWriter.ForFixedStrings));
		int number;
		ByteUnpacker.ReadValueBitPacked(reader, out number);
		PlayerHandedness handedness;
		reader.ReadValueSafe<PlayerHandedness>(out handedness, default(FastBufferWriter.ForEnums));
		FixedString32Bytes country;
		reader.ReadValueSafe<FixedString32Bytes>(out country, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes visorAttackerBlueSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out visorAttackerBlueSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes visorAttackerRedSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out visorAttackerRedSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes visorGoalieBlueSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out visorGoalieBlueSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes visorGoalieRedSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out visorGoalieRedSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes mustache;
		reader.ReadValueSafe<FixedString32Bytes>(out mustache, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes beard;
		reader.ReadValueSafe<FixedString32Bytes>(out beard, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes jerseyAttackerBlueSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out jerseyAttackerBlueSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes jerseyAttackerRedSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out jerseyAttackerRedSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes jerseyGoalieBlueSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out jerseyGoalieBlueSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes jerseyGoalieRedSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out jerseyGoalieRedSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes stickAttackerBlueSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out stickAttackerBlueSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes stickAttackerRedSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out stickAttackerRedSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes stickGoalieBlueSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out stickGoalieBlueSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes stickGoalieRedSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out stickGoalieRedSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes stickShaftAttackerBlueTapeSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out stickShaftAttackerBlueTapeSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes stickShaftAttackerRedTapeSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out stickShaftAttackerRedTapeSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes stickShaftGoalieBlueTapeSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out stickShaftGoalieBlueTapeSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes stickShaftGoalieRedTapeSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out stickShaftGoalieRedTapeSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes stickBladeAttackerBlueTapeSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out stickBladeAttackerBlueTapeSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes stickBladeAttackerRedTapeSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out stickBladeAttackerRedTapeSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes stickBladeGoalieBlueTapeSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out stickBladeGoalieBlueTapeSkin, default(FastBufferWriter.ForFixedStrings));
		FixedString32Bytes stickBladeGoalieRedTapeSkin;
		reader.ReadValueSafe<FixedString32Bytes>(out stickBladeGoalieRedTapeSkin, default(FastBufferWriter.ForFixedStrings));
		int patreonLevel;
		ByteUnpacker.ReadValueBitPacked(reader, out patreonLevel);
		int adminLevel;
		ByteUnpacker.ReadValueBitPacked(reader, out adminLevel);
		FixedString32Bytes steamId;
		reader.ReadValueSafe<FixedString32Bytes>(out steamId, default(FastBufferWriter.ForFixedStrings));
		bool flag;
		reader.ReadValueSafe<bool>(out flag, default(FastBufferWriter.ForPrimitives));
		ulong[] enabledModIds = null;
		if (flag)
		{
			reader.ReadValueSafe<ulong>(out enabledModIds, default(FastBufferWriter.ForPrimitives));
		}
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((Player)target).Client_PlayerSubscriptionRpc(username, number, handedness, country, visorAttackerBlueSkin, visorAttackerRedSkin, visorGoalieBlueSkin, visorGoalieRedSkin, mustache, beard, jerseyAttackerBlueSkin, jerseyAttackerRedSkin, jerseyGoalieBlueSkin, jerseyGoalieRedSkin, stickAttackerBlueSkin, stickAttackerRedSkin, stickGoalieBlueSkin, stickGoalieRedSkin, stickShaftAttackerBlueTapeSkin, stickShaftAttackerRedTapeSkin, stickShaftGoalieBlueTapeSkin, stickShaftGoalieRedTapeSkin, stickBladeAttackerBlueTapeSkin, stickBladeAttackerRedTapeSkin, stickBladeGoalieBlueTapeSkin, stickBladeGoalieRedTapeSkin, patreonLevel, adminLevel, steamId, enabledModIds);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x0000BF5A File Offset: 0x0000A15A
	protected internal override string __getTypeName()
	{
		return "Player";
	}

	// Token: 0x04000491 RID: 1169
	[Header("Prefabs")]
	[SerializeField]
	private PlayerCamera playerCameraPrefab;

	// Token: 0x04000492 RID: 1170
	[SerializeField]
	private PlayerBodyV2 playerBodyAttackerPrefab;

	// Token: 0x04000493 RID: 1171
	[SerializeField]
	private PlayerBodyV2 playerBodyGoaliePrefab;

	// Token: 0x04000494 RID: 1172
	[SerializeField]
	private StickPositioner stickPositionerPrefab;

	// Token: 0x04000495 RID: 1173
	[SerializeField]
	private SpectatorCamera spectatorCameraPrefab;

	// Token: 0x04000496 RID: 1174
	[SerializeField]
	private Stick stickAttackerPrefab;

	// Token: 0x04000497 RID: 1175
	[SerializeField]
	private Stick stickGoaliePrefab;

	// Token: 0x04000498 RID: 1176
	[HideInInspector]
	public NetworkVariable<PlayerState> State = new NetworkVariable<PlayerState>(PlayerState.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x04000499 RID: 1177
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> Username = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x0400049A RID: 1178
	[HideInInspector]
	public NetworkVariable<int> Number = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x0400049B RID: 1179
	[HideInInspector]
	public NetworkVariable<PlayerHandedness> Handedness = new NetworkVariable<PlayerHandedness>(PlayerHandedness.Left, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x0400049C RID: 1180
	[HideInInspector]
	public NetworkVariable<PlayerTeam> Team = new NetworkVariable<PlayerTeam>(PlayerTeam.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x0400049D RID: 1181
	[HideInInspector]
	public NetworkVariable<PlayerRole> Role = new NetworkVariable<PlayerRole>(PlayerRole.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x0400049E RID: 1182
	[HideInInspector]
	public NetworkVariable<int> Goals = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x0400049F RID: 1183
	[HideInInspector]
	public NetworkVariable<int> Assists = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004A0 RID: 1184
	[HideInInspector]
	public NetworkVariable<int> Ping = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004A1 RID: 1185
	[HideInInspector]
	public NetworkVariable<NetworkObjectReference> PlayerPositionReference = new NetworkVariable<NetworkObjectReference>(default(NetworkObjectReference), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004A2 RID: 1186
	[HideInInspector]
	public NetworkVariable<bool> IsReplay = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004A3 RID: 1187
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> Country = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004A4 RID: 1188
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> VisorAttackerBlueSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004A5 RID: 1189
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> VisorAttackerRedSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004A6 RID: 1190
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> VisorGoalieBlueSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004A7 RID: 1191
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> VisorGoalieRedSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004A8 RID: 1192
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> Mustache = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004A9 RID: 1193
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> Beard = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004AA RID: 1194
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> JerseyAttackerBlueSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004AB RID: 1195
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> JerseyAttackerRedSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004AC RID: 1196
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> JerseyGoalieBlueSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004AD RID: 1197
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> JerseyGoalieRedSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004AE RID: 1198
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> StickAttackerBlueSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004AF RID: 1199
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> StickAttackerRedSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004B0 RID: 1200
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> StickGoalieBlueSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004B1 RID: 1201
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> StickGoalieRedSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004B2 RID: 1202
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> StickShaftAttackerBlueTapeSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004B3 RID: 1203
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> StickShaftAttackerRedTapeSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004B4 RID: 1204
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> StickShaftGoalieBlueTapeSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004B5 RID: 1205
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> StickShaftGoalieRedTapeSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004B6 RID: 1206
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> StickBladeAttackerBlueTapeSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004B7 RID: 1207
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> StickBladeAttackerRedTapeSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004B8 RID: 1208
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> StickBladeGoalieBlueTapeSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004B9 RID: 1209
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> StickBladeGoalieRedTapeSkin = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004BA RID: 1210
	[HideInInspector]
	public NetworkVariable<int> PatreonLevel = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004BB RID: 1211
	[HideInInspector]
	public NetworkVariable<int> AdminLevel = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004BC RID: 1212
	[HideInInspector]
	public NetworkVariable<FixedString32Bytes> SteamId = new NetworkVariable<FixedString32Bytes>(default(FixedString32Bytes), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004BD RID: 1213
	[HideInInspector]
	public PlayerInput PlayerInput;

	// Token: 0x040004BE RID: 1214
	[HideInInspector]
	public SpectatorCamera SpectatorCamera;

	// Token: 0x040004BF RID: 1215
	[HideInInspector]
	public PlayerCamera PlayerCamera;

	// Token: 0x040004C0 RID: 1216
	[HideInInspector]
	public PlayerBodyV2 PlayerBody;

	// Token: 0x040004C1 RID: 1217
	[HideInInspector]
	public StickPositioner StickPositioner;

	// Token: 0x040004C2 RID: 1218
	[HideInInspector]
	public Stick Stick;

	// Token: 0x040004C3 RID: 1219
	[HideInInspector]
	public PlayerPosition PlayerPosition;

	// Token: 0x040004C4 RID: 1220
	private Tween delayedStateTween;
}
