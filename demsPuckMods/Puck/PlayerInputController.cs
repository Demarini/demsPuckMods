using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x020000E7 RID: 231
public class PlayerInputController : NetworkBehaviour
{
	// Token: 0x0600079F RID: 1951 RVA: 0x0000BB8D File Offset: 0x00009D8D
	private void Awake()
	{
		this.playerInput = base.GetComponent<PlayerInput>();
	}

	// Token: 0x060007A0 RID: 1952 RVA: 0x0000BB9B File Offset: 0x00009D9B
	private void Start()
	{
		this.playerInput.InitialLookAngle = MonoBehaviourSingleton<SettingsManager>.Instance.CameraAngle;
	}

	// Token: 0x060007A1 RID: 1953 RVA: 0x0002E034 File Offset: 0x0002C234
	public override void OnNetworkSpawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodySpawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerHandednessChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerHandednessChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnSynchronizeComplete", new Action<Dictionary<string, object>>(this.Event_Server_OnSynchronizeComplete));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnServerConfiguration", new Action<Dictionary<string, object>>(this.Event_Client_OnServerConfiguration));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnCameraAngleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnCameraAngleChanged));
		base.OnNetworkSpawn();
	}

	// Token: 0x060007A2 RID: 1954 RVA: 0x0002E0D0 File Offset: 0x0002C2D0
	public override void OnNetworkDespawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodySpawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerHandednessChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerHandednessChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnSynchronizeComplete", new Action<Dictionary<string, object>>(this.Event_Server_OnSynchronizeComplete));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnServerConfiguration", new Action<Dictionary<string, object>>(this.Event_Client_OnServerConfiguration));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnCameraAngleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnCameraAngleChanged));
		base.OnNetworkDespawn();
	}

	// Token: 0x060007A3 RID: 1955 RVA: 0x0000BBB2 File Offset: 0x00009DB2
	private void Event_OnPlayerBodySpawned(Dictionary<string, object> message)
	{
		if (((PlayerBodyV2)message["playerBody"]).Player.IsLocalPlayer)
		{
			this.playerInput.ResetInputs(false);
		}
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x0000BBDC File Offset: 0x00009DDC
	private void Event_OnPlayerHandednessChanged(Dictionary<string, object> message)
	{
		if (((Player)message["player"]).IsLocalPlayer)
		{
			this.playerInput.ResetInputs(true);
		}
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x0002E16C File Offset: 0x0002C36C
	private void Event_Server_OnSynchronizeComplete(Dictionary<string, object> message)
	{
		ulong clientId = (ulong)message["clientId"];
		this.playerInput.Server_ForceSynchronizeClientId(clientId);
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x0002E198 File Offset: 0x0002C398
	private void Event_Client_OnServerConfiguration(Dictionary<string, object> message)
	{
		Server server = (Server)message["server"];
		this.playerInput.TickRate = server.ClientTickRate;
		this.playerInput.SleepTimeout = server.SleepTimeout;
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x0002E1D8 File Offset: 0x0002C3D8
	private void Event_Client_OnCameraAngleChanged(Dictionary<string, object> message)
	{
		float initialLookAngle = (float)message["value"];
		this.playerInput.InitialLookAngle = initialLookAngle;
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060007AA RID: 1962 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060007AB RID: 1963 RVA: 0x0000BC01 File Offset: 0x00009E01
	protected internal override string __getTypeName()
	{
		return "PlayerInputController";
	}

	// Token: 0x0400046C RID: 1132
	private PlayerInput playerInput;
}
