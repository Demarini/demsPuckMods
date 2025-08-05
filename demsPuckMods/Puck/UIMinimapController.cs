using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000113 RID: 275
internal class UIMinimapController : NetworkBehaviour
{
	// Token: 0x060009B7 RID: 2487 RVA: 0x0000D370 File Offset: 0x0000B570
	private void Awake()
	{
		this.uiMinimap = base.GetComponent<UIMinimap>();
	}

	// Token: 0x060009B8 RID: 2488 RVA: 0x0003A6E4 File Offset: 0x000388E4
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodySpawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerRoleChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerRoleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerNumberChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerNumberChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerBodyDespawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodyDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_OnPuckSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPuckDespawned", new Action<Dictionary<string, object>>(this.Event_OnPuckDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnShowMinimapChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnShowMinimapChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnMinimapOpacityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnMinimapOpacityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnShowGameUserInterfaceChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnShowGameUserInterfaceChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnMinimapHorizontalPositionChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnMinimapHorizontalPositionChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnMinimapVerticalPositionChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnMinimapVerticalPositionChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnMinimapBackgroundOpacityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnMinimapBackgroundOpacityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnMinimapScaleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnMinimapScaleChanged));
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x0003A86C File Offset: 0x00038A6C
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodySpawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerRoleChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerRoleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerNumberChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerNumberChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerBodyDespawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodyDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_OnPuckSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPuckDespawned", new Action<Dictionary<string, object>>(this.Event_OnPuckDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnShowMinimapChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnShowMinimapChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnMinimapOpacityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnMinimapOpacityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnShowGameUserInterfaceChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnShowGameUserInterfaceChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnMinimapHorizontalPositionChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnMinimapHorizontalPositionChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnMinimapVerticalPositionChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnMinimapVerticalPositionChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnMinimapBackgroundOpacityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnMinimapBackgroundOpacityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnMinimapScaleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnMinimapScaleChanged));
		base.OnDestroy();
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x0003A9FC File Offset: 0x00038BFC
	private void Event_OnPlayerBodySpawned(Dictionary<string, object> message)
	{
		PlayerBodyV2 playerBody = (PlayerBodyV2)message["playerBody"];
		this.uiMinimap.AddPlayerBody(playerBody);
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x0003AA28 File Offset: 0x00038C28
	private void Event_OnPlayerTeamChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		PlayerTeam team = (PlayerTeam)message["newTeam"];
		if (player.IsLocalPlayer)
		{
			this.uiMinimap.Team = team;
		}
		this.uiMinimap.UpdatePlayerBody(player.PlayerBody);
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x0003AA7C File Offset: 0x00038C7C
	private void Event_OnPlayerRoleChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiMinimap.UpdatePlayerBody(player.PlayerBody);
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x0003AA7C File Offset: 0x00038C7C
	private void Event_OnPlayerNumberChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiMinimap.UpdatePlayerBody(player.PlayerBody);
	}

	// Token: 0x060009BE RID: 2494 RVA: 0x0003AAAC File Offset: 0x00038CAC
	private void Event_OnPlayerBodyDespawned(Dictionary<string, object> message)
	{
		PlayerBodyV2 playerBody = (PlayerBodyV2)message["playerBody"];
		this.uiMinimap.RemovePlayerBody(playerBody);
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x0003AAD8 File Offset: 0x00038CD8
	private void Event_OnPuckSpawned(Dictionary<string, object> message)
	{
		Puck puck = (Puck)message["puck"];
		this.uiMinimap.AddPuck(puck);
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x0003AB04 File Offset: 0x00038D04
	private void Event_OnPuckDespawned(Dictionary<string, object> message)
	{
		Puck puck = (Puck)message["puck"];
		this.uiMinimap.RemovePuck(puck);
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x0003AB30 File Offset: 0x00038D30
	private void Event_Client_OnMinimapOpacityChanged(Dictionary<string, object> message)
	{
		float opacity = (float)message["value"];
		this.uiMinimap.SetOpacity(opacity);
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x0000D37E File Offset: 0x0000B57E
	private void Event_Client_OnShowMinimapChanged(Dictionary<string, object> message)
	{
		if (NetworkBehaviourSingleton<UIManager>.Instance.UIState == UIState.MainMenu)
		{
			return;
		}
		if ((bool)message["value"])
		{
			this.uiMinimap.Show();
			return;
		}
		this.uiMinimap.Hide(false);
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x0000D37E File Offset: 0x0000B57E
	private void Event_Client_OnShowGameUserInterfaceChanged(Dictionary<string, object> message)
	{
		if (NetworkBehaviourSingleton<UIManager>.Instance.UIState == UIState.MainMenu)
		{
			return;
		}
		if ((bool)message["value"])
		{
			this.uiMinimap.Show();
			return;
		}
		this.uiMinimap.Hide(false);
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x0003AB5C File Offset: 0x00038D5C
	private void Event_Client_OnMinimapHorizontalPositionChanged(Dictionary<string, object> message)
	{
		float x = (float)message["value"];
		this.uiMinimap.SetPosition(new Vector2(x, this.uiMinimap.Position.y));
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x0003AB9C File Offset: 0x00038D9C
	private void Event_Client_OnMinimapVerticalPositionChanged(Dictionary<string, object> message)
	{
		float y = (float)message["value"];
		this.uiMinimap.SetPosition(new Vector2(this.uiMinimap.Position.x, y));
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x0003ABDC File Offset: 0x00038DDC
	private void Event_Client_OnMinimapBackgroundOpacityChanged(Dictionary<string, object> message)
	{
		float backgroundOpacity = (float)message["value"];
		this.uiMinimap.SetBackgroundOpacity(backgroundOpacity);
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x0003AC08 File Offset: 0x00038E08
	private void Event_Client_OnMinimapScaleChanged(Dictionary<string, object> message)
	{
		float scale = (float)message["value"];
		this.uiMinimap.SetScale(scale);
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x0000D3B7 File Offset: 0x0000B5B7
	protected internal override string __getTypeName()
	{
		return "UIMinimapController";
	}

	// Token: 0x040005D4 RID: 1492
	private UIMinimap uiMinimap;
}
