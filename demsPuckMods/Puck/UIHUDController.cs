using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x0200010D RID: 269
public class UIHUDController : NetworkBehaviour
{
	// Token: 0x06000966 RID: 2406 RVA: 0x0000CEC2 File Offset: 0x0000B0C2
	private void Awake()
	{
		this.uiHud = base.GetComponent<UIHUD>();
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x000393E4 File Offset: 0x000375E4
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodySpawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerBodyStaminaChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodyStaminaChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerBodySpeedChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodySpeedChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnUnitsChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnUnitsChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPlayerCameraEnabled", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerCameraEnabled));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPlayerCameraDisabled", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerCameraDisabled));
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x00039494 File Offset: 0x00037694
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodySpawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerBodyStaminaChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodyStaminaChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerBodySpeedChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodySpeedChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnUnitsChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnUnitsChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPlayerCameraEnabled", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerCameraEnabled));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPlayerCameraDisabled", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerCameraDisabled));
		base.OnDestroy();
	}

	// Token: 0x06000969 RID: 2409 RVA: 0x0003954C File Offset: 0x0003774C
	private void Event_OnPlayerBodySpawned(Dictionary<string, object> message)
	{
		PlayerBodyV2 playerBodyV = (PlayerBodyV2)message["playerBody"];
		if (!playerBodyV.Player.IsLocalPlayer)
		{
			return;
		}
		this.hasTarget = true;
		this.targetClientId = playerBodyV.OwnerClientId;
		this.uiHud.Show();
		this.uiHud.SetStamina(playerBodyV.Stamina);
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x000395A8 File Offset: 0x000377A8
	private void Event_OnPlayerBodyStaminaChanged(Dictionary<string, object> message)
	{
		PlayerBodyV2 playerBodyV = (PlayerBodyV2)message["playerBody"];
		float stamina = (float)message["newStamina"];
		if (!this.hasTarget)
		{
			return;
		}
		if (playerBodyV.OwnerClientId != this.targetClientId)
		{
			return;
		}
		this.uiHud.SetStamina(stamina);
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x000395FC File Offset: 0x000377FC
	private void Event_OnPlayerBodySpeedChanged(Dictionary<string, object> message)
	{
		PlayerBodyV2 playerBodyV = (PlayerBodyV2)message["playerBody"];
		float speed = (float)message["newSpeed"];
		if (!this.hasTarget)
		{
			return;
		}
		if (playerBodyV.OwnerClientId != this.targetClientId)
		{
			return;
		}
		this.uiHud.SetSpeed(speed);
	}

	// Token: 0x0600096C RID: 2412 RVA: 0x00039650 File Offset: 0x00037850
	private void Event_Client_OnUnitsChanged(Dictionary<string, object> message)
	{
		string a = (string)message["value"];
		if (a == "METRIC")
		{
			this.uiHud.SetUnits("KPH");
			return;
		}
		if (!(a == "FREEDOM"))
		{
			return;
		}
		this.uiHud.SetUnits("MPH");
	}

	// Token: 0x0600096D RID: 2413 RVA: 0x000396AC File Offset: 0x000378AC
	private void Event_Client_OnPlayerCameraEnabled(Dictionary<string, object> message)
	{
		PlayerCamera playerCamera = (PlayerCamera)message["playerCamera"];
		if (!this.hasTarget)
		{
			return;
		}
		if (playerCamera.OwnerClientId != this.targetClientId)
		{
			return;
		}
		this.uiHud.Show();
	}

	// Token: 0x0600096E RID: 2414 RVA: 0x000396F0 File Offset: 0x000378F0
	private void Event_Client_OnPlayerCameraDisabled(Dictionary<string, object> message)
	{
		PlayerCamera playerCamera = (PlayerCamera)message["playerCamera"];
		if (!this.hasTarget)
		{
			return;
		}
		if (playerCamera.OwnerClientId != this.targetClientId)
		{
			return;
		}
		this.uiHud.Hide(false);
	}

	// Token: 0x06000970 RID: 2416 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000971 RID: 2417 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x0000CED0 File Offset: 0x0000B0D0
	protected internal override string __getTypeName()
	{
		return "UIHUDController";
	}

	// Token: 0x040005AA RID: 1450
	private UIHUD uiHud;

	// Token: 0x040005AB RID: 1451
	private bool hasTarget;

	// Token: 0x040005AC RID: 1452
	private ulong targetClientId;
}
