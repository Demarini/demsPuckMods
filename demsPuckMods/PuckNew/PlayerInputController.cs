using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x0200003D RID: 61
public class PlayerInputController : NetworkBehaviour
{
	// Token: 0x060001E0 RID: 480 RVA: 0x0000C911 File Offset: 0x0000AB11
	private void Awake()
	{
		this.playerInput = base.GetComponent<PlayerInput>();
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x0000C91F File Offset: 0x0000AB1F
	private void Start()
	{
		this.playerInput.InitialLookAngle = SettingsManager.CameraAngle;
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x0000C934 File Offset: 0x0000AB34
	public override void OnNetworkSpawn()
	{
		EventManager.AddEventListener("Event_Everyone_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodySpawned));
		EventManager.AddEventListener("Event_Everyone_OnPlayerHandednessChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerHandednessChanged));
		EventManager.AddEventListener("Event_Everyone_OnServerChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnServerChanged));
		EventManager.AddEventListener("Event_Server_OnClientSceneSynchronizeComplete", new Action<Dictionary<string, object>>(this.Event_Server_OnClientSceneSynchronizeComplete));
		EventManager.AddEventListener("Event_OnCameraAngleChanged", new Action<Dictionary<string, object>>(this.Event_OnCameraAngleChanged));
		base.OnNetworkSpawn();
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x0000C9B8 File Offset: 0x0000ABB8
	public override void OnNetworkDespawn()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodySpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerHandednessChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerHandednessChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnServerChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnServerChanged));
		EventManager.RemoveEventListener("Event_Server_OnClientSceneSynchronizeComplete", new Action<Dictionary<string, object>>(this.Event_Server_OnClientSceneSynchronizeComplete));
		EventManager.RemoveEventListener("Event_OnCameraAngleChanged", new Action<Dictionary<string, object>>(this.Event_OnCameraAngleChanged));
		base.OnNetworkDespawn();
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x0000CA3C File Offset: 0x0000AC3C
	private void Event_Everyone_OnPlayerBodySpawned(Dictionary<string, object> message)
	{
		PlayerBody playerBody = (PlayerBody)message["playerBody"];
		if (playerBody.Player.IsLocalPlayer)
		{
			this.playerInput.ResetInputs(playerBody.Player.Handedness.Value);
		}
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x0000CA84 File Offset: 0x0000AC84
	private void Event_Everyone_OnPlayerHandednessChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (player.IsLocalPlayer)
		{
			this.playerInput.ResetInputs(player.Handedness.Value);
		}
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x0000CAC0 File Offset: 0x0000ACC0
	private void Event_Everyone_OnServerChanged(Dictionary<string, object> message)
	{
		this.playerInput.TickRate = NetworkBehaviourSingleton<ServerManager>.Instance.Server.Value.TickRate;
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x0000CAE4 File Offset: 0x0000ACE4
	private void Event_Server_OnClientSceneSynchronizeComplete(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["clientId"];
		if (num == 0UL)
		{
			return;
		}
		this.playerInput.Server_ForceSynchronizeClientId(num);
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x0000CB14 File Offset: 0x0000AD14
	private void Event_OnCameraAngleChanged(Dictionary<string, object> message)
	{
		float initialLookAngle = (float)message["value"];
		this.playerInput.InitialLookAngle = initialLookAngle;
	}

	// Token: 0x060001EA RID: 490 RVA: 0x0000CB40 File Offset: 0x0000AD40
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060001EB RID: 491 RVA: 0x000021BE File Offset: 0x000003BE
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060001EC RID: 492 RVA: 0x0000CB56 File Offset: 0x0000AD56
	protected internal override string __getTypeName()
	{
		return "PlayerInputController";
	}

	// Token: 0x04000147 RID: 327
	private PlayerInput playerInput;
}
