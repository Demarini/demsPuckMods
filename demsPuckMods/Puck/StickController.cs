using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x020000F5 RID: 245
public class StickController : NetworkBehaviour
{
	// Token: 0x06000856 RID: 2134 RVA: 0x0000C1AF File Offset: 0x0000A3AF
	private void Awake()
	{
		this.stick = base.GetComponent<Stick>();
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x000346A4 File Offset: 0x000328A4
	public override void OnNetworkSpawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerRoleChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerRoleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerStickSkinChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerStickSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerStickShaftTapeSkinChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerStickShaftTapeSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerStickBladeTapeSkinChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerStickBladeTapeSkinChanged));
		base.OnNetworkSpawn();
	}

	// Token: 0x06000858 RID: 2136 RVA: 0x00034740 File Offset: 0x00032940
	public override void OnNetworkDespawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerRoleChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerRoleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerStickSkinChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerStickSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerStickShaftTapeSkinChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerStickShaftTapeSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerStickBladeTapeSkinChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerStickBladeTapeSkinChanged));
		base.OnNetworkDespawn();
	}

	// Token: 0x06000859 RID: 2137 RVA: 0x000347DC File Offset: 0x000329DC
	private void Event_OnPlayerRoleChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId == player.OwnerClientId)
		{
			this.stick.UpdateStick();
		}
	}

	// Token: 0x0600085A RID: 2138 RVA: 0x000347DC File Offset: 0x000329DC
	private void Event_OnPlayerTeamChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId == player.OwnerClientId)
		{
			this.stick.UpdateStick();
		}
	}

	// Token: 0x0600085B RID: 2139 RVA: 0x000347DC File Offset: 0x000329DC
	private void Event_OnPlayerStickSkinChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId == player.OwnerClientId)
		{
			this.stick.UpdateStick();
		}
	}

	// Token: 0x0600085C RID: 2140 RVA: 0x000347DC File Offset: 0x000329DC
	private void Event_OnPlayerStickShaftTapeSkinChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId == player.OwnerClientId)
		{
			this.stick.UpdateStick();
		}
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x000347DC File Offset: 0x000329DC
	private void Event_OnPlayerStickBladeTapeSkinChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId == player.OwnerClientId)
		{
			this.stick.UpdateStick();
		}
	}

	// Token: 0x0600085F RID: 2143 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x0000C1BD File Offset: 0x0000A3BD
	protected internal override string __getTypeName()
	{
		return "StickController";
	}

	// Token: 0x040004F2 RID: 1266
	private Stick stick;
}
