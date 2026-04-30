using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x02000051 RID: 81
public class StickController : NetworkBehaviour
{
	// Token: 0x060002B1 RID: 689 RVA: 0x000115BE File Offset: 0x0000F7BE
	private void Awake()
	{
		this.stick = base.GetComponent<Stick>();
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x000115CC File Offset: 0x0000F7CC
	public override void OnNetworkSpawn()
	{
		EventManager.AddEventListener("Event_Everyone_OnPlayerGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerGameStateChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerCustomizationStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerCustomizationStateChanged));
		base.OnNetworkSpawn();
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x00011600 File Offset: 0x0000F800
	public override void OnNetworkDespawn()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerGameStateChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerCustomizationStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerCustomizationStateChanged));
		base.OnNetworkDespawn();
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x00011634 File Offset: 0x0000F834
	private void Event_Everyone_OnPlayerGameStateChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		PlayerGameState playerGameState = (PlayerGameState)message["oldGameState"];
		PlayerGameState playerGameState2 = (PlayerGameState)message["newGameState"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		if (playerGameState.Team == playerGameState2.Team && playerGameState.Role == playerGameState2.Role)
		{
			return;
		}
		this.stick.ApplyCustomizations();
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x000116AC File Offset: 0x0000F8AC
	private void Event_Everyone_OnPlayerCustomizationStateChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId == player.OwnerClientId)
		{
			this.stick.ApplyCustomizations();
		}
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x000116E4 File Offset: 0x0000F8E4
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x000021BE File Offset: 0x000003BE
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x000116FA File Offset: 0x0000F8FA
	protected internal override string __getTypeName()
	{
		return "StickController";
	}

	// Token: 0x040001D6 RID: 470
	private Stick stick;
}
