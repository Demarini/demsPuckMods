using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine.InputSystem;

// Token: 0x02000048 RID: 72
public class PlayerController : NetworkBehaviour
{
	// Token: 0x06000273 RID: 627 RVA: 0x0001061F File Offset: 0x0000E81F
	private void Awake()
	{
		this.player = base.GetComponent<Player>();
	}

	// Token: 0x06000274 RID: 628 RVA: 0x00010630 File Offset: 0x0000E830
	public override void OnNetworkSpawn()
	{
		InputManager.PositionSelectAction.performed += this.OnPositionSelectActionPerformed;
		EventManager.AddEventListener("Event_OnTeamSelectClickTeam", new Action<Dictionary<string, object>>(this.Event_OnTeamSelectClickTeam));
		EventManager.AddEventListener("Event_OnPositionSelectClickPosition", new Action<Dictionary<string, object>>(this.Event_OnPositionSelectClickPosition));
		EventManager.AddEventListener("Event_OnPauseMenuClickSelectTeam", new Action<Dictionary<string, object>>(this.Event_OnPauseMenuClickSelectTeam));
		EventManager.AddEventListener("Event_OnPauseMenuClickSelectPosition", new Action<Dictionary<string, object>>(this.Event_OnPauseMenuClickSelectPosition));
		EventManager.AddEventListener("Event_OnHandednessChanged", new Action<Dictionary<string, object>>(this.Event_OnHandednessChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerPositionClaimedByPlayerChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerPositionClaimedByPlayerChanged));
		if (NetworkManager.Singleton.IsServer)
		{
			this.pingTween = DOVirtual.DelayedCall(1f, delegate
			{
				this.player.Server_UpdatePing();
			}, true).SetLoops(-1);
		}
		base.OnNetworkSpawn();
	}

	// Token: 0x06000275 RID: 629 RVA: 0x0001070C File Offset: 0x0000E90C
	public override void OnNetworkDespawn()
	{
		InputManager.PositionSelectAction.performed -= this.OnPositionSelectActionPerformed;
		EventManager.RemoveEventListener("Event_OnTeamSelectClickTeam", new Action<Dictionary<string, object>>(this.Event_OnTeamSelectClickTeam));
		EventManager.RemoveEventListener("Event_OnPositionSelectClickPosition", new Action<Dictionary<string, object>>(this.Event_OnPositionSelectClickPosition));
		EventManager.RemoveEventListener("Event_OnPauseMenuClickSelectTeam", new Action<Dictionary<string, object>>(this.Event_OnPauseMenuClickSelectTeam));
		EventManager.RemoveEventListener("Event_OnPauseMenuClickSelectPosition", new Action<Dictionary<string, object>>(this.Event_OnPauseMenuClickSelectPosition));
		EventManager.RemoveEventListener("Event_OnHandednessChanged", new Action<Dictionary<string, object>>(this.Event_OnHandednessChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerPositionClaimedByPlayerChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerPositionClaimedByPlayerChanged));
		if (NetworkManager.Singleton.IsServer)
		{
			Tween tween = this.pingTween;
			if (tween != null)
			{
				tween.Kill(false);
			}
		}
		base.OnNetworkDespawn();
	}

	// Token: 0x06000276 RID: 630 RVA: 0x000107D8 File Offset: 0x0000E9D8
	private void OnPositionSelectActionPerformed(InputAction.CallbackContext context)
	{
		if (GlobalStateManager.UIState.Phase != UIPhase.Playing)
		{
			return;
		}
		if (GlobalStateManager.UIState.IsInteracting)
		{
			return;
		}
		if (!this.player.IsLocalPlayer)
		{
			return;
		}
		this.player.Client_RequestPositionSelectRpc(default(RpcParams));
	}

	// Token: 0x06000277 RID: 631 RVA: 0x00010828 File Offset: 0x0000EA28
	private void Event_OnTeamSelectClickTeam(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["team"];
		if (!this.player.IsLocalPlayer)
		{
			return;
		}
		this.player.Client_RequestTeamRpc(team, default(RpcParams));
	}

	// Token: 0x06000278 RID: 632 RVA: 0x0001086C File Offset: 0x0000EA6C
	private void Event_OnPositionSelectClickPosition(Dictionary<string, object> message)
	{
		PlayerPosition playerPosition = (PlayerPosition)message["playerPosition"];
		if (!this.player.IsLocalPlayer)
		{
			return;
		}
		NetworkObjectReference playerPositionReference = new NetworkObjectReference(playerPosition.NetworkObject);
		this.player.Client_RequestClaimPositionRpc(playerPositionReference, default(RpcParams));
	}

	// Token: 0x06000279 RID: 633 RVA: 0x000108BC File Offset: 0x0000EABC
	private void Event_OnPauseMenuClickSelectTeam(Dictionary<string, object> message)
	{
		if (!this.player.IsLocalPlayer)
		{
			return;
		}
		this.player.Client_RequestTeamSelectRpc(default(RpcParams));
	}

	// Token: 0x0600027A RID: 634 RVA: 0x000108EC File Offset: 0x0000EAEC
	private void Event_OnPauseMenuClickSelectPosition(Dictionary<string, object> message)
	{
		if (!this.player.IsLocalPlayer)
		{
			return;
		}
		this.player.Client_RequestPositionSelectRpc(default(RpcParams));
	}

	// Token: 0x0600027B RID: 635 RVA: 0x0001091C File Offset: 0x0000EB1C
	private void Event_OnHandednessChanged(Dictionary<string, object> message)
	{
		PlayerHandedness handedness = (PlayerHandedness)message["value"];
		if (!this.player.IsLocalPlayer)
		{
			return;
		}
		this.player.Client_RequestHandednessRpc(handedness, default(RpcParams));
	}

	// Token: 0x0600027C RID: 636 RVA: 0x00010960 File Offset: 0x0000EB60
	private void Event_Everyone_OnPlayerPositionClaimedByPlayerChanged(Dictionary<string, object> message)
	{
		PlayerPosition playerPosition = (PlayerPosition)message["playerPosition"];
		Player x = (Player)message["oldClaimedByPlayer"];
		Player x2 = (Player)message["newClaimedByPlayer"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (x2 == this.player)
		{
			this.player.PlayerPositionReference.Value = new NetworkObjectReference(playerPosition.NetworkObject);
			return;
		}
		if (x == this.player && playerPosition == this.player.PlayerPosition)
		{
			this.player.PlayerPositionReference.Value = default(NetworkObjectReference);
		}
	}

	// Token: 0x0600027F RID: 639 RVA: 0x00010A20 File Offset: 0x0000EC20
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000280 RID: 640 RVA: 0x000021BE File Offset: 0x000003BE
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000281 RID: 641 RVA: 0x00010A36 File Offset: 0x0000EC36
	protected internal override string __getTypeName()
	{
		return "PlayerController";
	}

	// Token: 0x040001A1 RID: 417
	private Player player;

	// Token: 0x040001A2 RID: 418
	private Tween pingTween;
}
