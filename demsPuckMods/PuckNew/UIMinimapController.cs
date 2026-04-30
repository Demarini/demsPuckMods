using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A0 RID: 416
internal class UIMinimapController : UIViewController<UIMinimap>
{
	// Token: 0x06000BE8 RID: 3048 RVA: 0x00038414 File Offset: 0x00036614
	public override void Awake()
	{
		base.Awake();
		this.uiMinimap = base.GetComponent<UIMinimap>();
		EventManager.AddEventListener("Event_Everyone_OnLevelSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnLevelSpawned));
		EventManager.AddEventListener("Event_Everyone_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodySpawned));
		EventManager.AddEventListener("Event_Everyone_OnPlayerGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerGameStateChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerNumberChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerNumberChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerBodyDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodyDespawned));
		EventManager.AddEventListener("Event_Everyone_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckSpawned));
		EventManager.AddEventListener("Event_Everyone_OnPuckDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckDespawned));
		EventManager.AddEventListener("Event_OnShowMinimapChanged", new Action<Dictionary<string, object>>(this.Event_OnShowMinimapChanged));
		EventManager.AddEventListener("Event_OnMinimapOpacityChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapOpacityChanged));
		EventManager.AddEventListener("Event_OnShowGameUserInterfaceChanged", new Action<Dictionary<string, object>>(this.Event_OnShowGameUserInterfaceChanged));
		EventManager.AddEventListener("Event_OnMinimapHorizontalPositionChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapHorizontalPositionChanged));
		EventManager.AddEventListener("Event_OnMinimapVerticalPositionChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapVerticalPositionChanged));
		EventManager.AddEventListener("Event_OnMinimapBackgroundOpacityChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapBackgroundOpacityChanged));
		EventManager.AddEventListener("Event_OnMinimapScaleChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapScaleChanged));
	}

	// Token: 0x06000BE9 RID: 3049 RVA: 0x00038568 File Offset: 0x00036768
	private void Start()
	{
		this.uiMinimap.SetOpacity(SettingsManager.MinimapOpacity);
		this.uiMinimap.SetBackgroundOpacity(SettingsManager.MinimapBackgroundOpacity);
		this.uiMinimap.SetPosition(new Vector2(SettingsManager.MinimapHorizontalPosition, SettingsManager.MinimapVerticalPosition));
		this.uiMinimap.SetScale(SettingsManager.MinimapScale);
	}

	// Token: 0x06000BEA RID: 3050 RVA: 0x000385C0 File Offset: 0x000367C0
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnLevelSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnLevelSpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodySpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerGameStateChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerNumberChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerNumberChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerBodyDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodyDespawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckSpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPuckDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckDespawned));
		EventManager.RemoveEventListener("Event_OnShowMinimapChanged", new Action<Dictionary<string, object>>(this.Event_OnShowMinimapChanged));
		EventManager.RemoveEventListener("Event_OnMinimapOpacityChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapOpacityChanged));
		EventManager.RemoveEventListener("Event_OnShowGameUserInterfaceChanged", new Action<Dictionary<string, object>>(this.Event_OnShowGameUserInterfaceChanged));
		EventManager.RemoveEventListener("Event_OnMinimapHorizontalPositionChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapHorizontalPositionChanged));
		EventManager.RemoveEventListener("Event_OnMinimapVerticalPositionChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapVerticalPositionChanged));
		EventManager.RemoveEventListener("Event_OnMinimapBackgroundOpacityChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapBackgroundOpacityChanged));
		EventManager.RemoveEventListener("Event_OnMinimapScaleChanged", new Action<Dictionary<string, object>>(this.Event_OnMinimapScaleChanged));
		base.OnDestroy();
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x00038708 File Offset: 0x00036908
	private void Event_Everyone_OnLevelSpawned(Dictionary<string, object> message)
	{
		Level level = (Level)message["level"];
		this.uiMinimap.Bounds = level.Bounds;
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x00038738 File Offset: 0x00036938
	private void Event_Everyone_OnPlayerBodySpawned(Dictionary<string, object> message)
	{
		PlayerBody playerBody = (PlayerBody)message["playerBody"];
		this.uiMinimap.AddPlayerBody(playerBody);
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x00038764 File Offset: 0x00036964
	private void Event_Everyone_OnPlayerGameStateChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		PlayerGameState playerGameState = (PlayerGameState)message["newGameState"];
		if (player.IsLocalPlayer)
		{
			this.uiMinimap.Team = playerGameState.Team;
		}
		this.uiMinimap.StylePlayer(player.PlayerBody);
	}

	// Token: 0x06000BEE RID: 3054 RVA: 0x000387C0 File Offset: 0x000369C0
	private void Event_Everyone_OnPlayerNumberChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiMinimap.StylePlayer(player.PlayerBody);
	}

	// Token: 0x06000BEF RID: 3055 RVA: 0x000387F0 File Offset: 0x000369F0
	private void Event_Everyone_OnPlayerBodyDespawned(Dictionary<string, object> message)
	{
		PlayerBody playerBody = (PlayerBody)message["playerBody"];
		this.uiMinimap.RemovePlayerBody(playerBody);
	}

	// Token: 0x06000BF0 RID: 3056 RVA: 0x0003881C File Offset: 0x00036A1C
	private void Event_Everyone_OnPuckSpawned(Dictionary<string, object> message)
	{
		Puck puck = (Puck)message["puck"];
		this.uiMinimap.AddPuck(puck);
	}

	// Token: 0x06000BF1 RID: 3057 RVA: 0x00038848 File Offset: 0x00036A48
	private void Event_Everyone_OnPuckDespawned(Dictionary<string, object> message)
	{
		Puck puck = (Puck)message["puck"];
		this.uiMinimap.RemovePuck(puck);
	}

	// Token: 0x06000BF2 RID: 3058 RVA: 0x00038874 File Offset: 0x00036A74
	private void Event_OnMinimapOpacityChanged(Dictionary<string, object> message)
	{
		float opacity = (float)message["value"];
		this.uiMinimap.SetOpacity(opacity);
	}

	// Token: 0x06000BF3 RID: 3059 RVA: 0x0003889E File Offset: 0x00036A9E
	private void Event_OnShowMinimapChanged(Dictionary<string, object> message)
	{
		if (GlobalStateManager.UIState.Phase == UIPhase.LockerRoom)
		{
			return;
		}
		if ((bool)message["value"])
		{
			this.uiMinimap.Show();
			return;
		}
		this.uiMinimap.Hide();
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x0003889E File Offset: 0x00036A9E
	private void Event_OnShowGameUserInterfaceChanged(Dictionary<string, object> message)
	{
		if (GlobalStateManager.UIState.Phase == UIPhase.LockerRoom)
		{
			return;
		}
		if ((bool)message["value"])
		{
			this.uiMinimap.Show();
			return;
		}
		this.uiMinimap.Hide();
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x000388DC File Offset: 0x00036ADC
	private void Event_OnMinimapHorizontalPositionChanged(Dictionary<string, object> message)
	{
		float x = (float)message["value"];
		this.uiMinimap.SetPosition(new Vector2(x, this.uiMinimap.Position.y));
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x0003891C File Offset: 0x00036B1C
	private void Event_OnMinimapVerticalPositionChanged(Dictionary<string, object> message)
	{
		float y = (float)message["value"];
		this.uiMinimap.SetPosition(new Vector2(this.uiMinimap.Position.x, y));
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x0003895C File Offset: 0x00036B5C
	private void Event_OnMinimapBackgroundOpacityChanged(Dictionary<string, object> message)
	{
		float backgroundOpacity = (float)message["value"];
		this.uiMinimap.SetBackgroundOpacity(backgroundOpacity);
	}

	// Token: 0x06000BF8 RID: 3064 RVA: 0x00038988 File Offset: 0x00036B88
	private void Event_OnMinimapScaleChanged(Dictionary<string, object> message)
	{
		float scale = (float)message["value"];
		this.uiMinimap.SetScale(scale);
	}

	// Token: 0x0400070A RID: 1802
	private UIMinimap uiMinimap;
}
