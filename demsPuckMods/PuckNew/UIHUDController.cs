using System;
using System.Collections.Generic;

// Token: 0x02000198 RID: 408
public class UIHUDController : UIViewController<UIHUD>
{
	// Token: 0x06000B94 RID: 2964 RVA: 0x00036B4C File Offset: 0x00034D4C
	public override void Awake()
	{
		base.Awake();
		this.uiHud = base.GetComponent<UIHUD>();
		EventManager.AddEventListener("Event_Everyone_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodySpawned));
		EventManager.AddEventListener("Event_Everyone_OnPlayerBodyStaminaChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodyStaminaChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerBodySpeedChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodySpeedChanged));
		EventManager.AddEventListener("Event_OnUnitsChanged", new Action<Dictionary<string, object>>(this.Event_OnUnitsChanged));
	}

	// Token: 0x06000B95 RID: 2965 RVA: 0x00036BC4 File Offset: 0x00034DC4
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodySpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerBodyStaminaChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodyStaminaChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerBodySpeedChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodySpeedChanged));
		EventManager.RemoveEventListener("Event_OnUnitsChanged", new Action<Dictionary<string, object>>(this.Event_OnUnitsChanged));
		base.OnDestroy();
	}

	// Token: 0x06000B96 RID: 2966 RVA: 0x00036C30 File Offset: 0x00034E30
	private void Event_Everyone_OnPlayerBodySpawned(Dictionary<string, object> message)
	{
		PlayerBody playerBody = (PlayerBody)message["playerBody"];
		if (!playerBody.Player.IsLocalPlayer)
		{
			return;
		}
		this.uiHud.Show();
		this.uiHud.SetStamina(playerBody.Stamina.Value);
	}

	// Token: 0x06000B97 RID: 2967 RVA: 0x00036C80 File Offset: 0x00034E80
	private void Event_Everyone_OnPlayerBodyStaminaChanged(Dictionary<string, object> message)
	{
		PlayerBody playerBody = (PlayerBody)message["playerBody"];
		float stamina = (float)message["newStamina"];
		if (!playerBody.Player.IsLocalPlayer)
		{
			return;
		}
		this.uiHud.SetStamina(stamina);
	}

	// Token: 0x06000B98 RID: 2968 RVA: 0x00036CC8 File Offset: 0x00034EC8
	private void Event_Everyone_OnPlayerBodySpeedChanged(Dictionary<string, object> message)
	{
		PlayerBody playerBody = (PlayerBody)message["playerBody"];
		float speed = (float)message["newSpeed"];
		if (!playerBody.Player.IsLocalPlayer)
		{
			return;
		}
		this.uiHud.SetSpeed(speed);
	}

	// Token: 0x06000B99 RID: 2969 RVA: 0x00036D10 File Offset: 0x00034F10
	private void Event_OnUnitsChanged(Dictionary<string, object> message)
	{
		Units units = (Units)message["value"];
		if (units == Units.Metric)
		{
			this.uiHud.SetUnits("KPH");
			return;
		}
		if (units != Units.Imperial)
		{
			return;
		}
		this.uiHud.SetUnits("MPH");
	}

	// Token: 0x040006DB RID: 1755
	private UIHUD uiHud;
}
