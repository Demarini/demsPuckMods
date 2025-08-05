using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000E0 RID: 224
public class PlayerBodyV2Controller : NetworkBehaviour
{
	// Token: 0x060006FF RID: 1791 RVA: 0x0000B84F File Offset: 0x00009A4F
	private void Awake()
	{
		this.playerBody = base.GetComponent<PlayerBodyV2>();
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x000291D4 File Offset: 0x000273D4
	public override void OnNetworkSpawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodySpawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerRoleChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerRoleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerUsernameChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerUsernameChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerNumberChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerNumberChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerCountryChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerCountryChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerVisorChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerVisorChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerMustacheChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerMustacheChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerBeardChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerBeardChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerJerseyChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerJerseyChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnGamePhaseChanged", new Action<Dictionary<string, object>>(this.Event_OnGamePhaseChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerVoiceStarted", new Action<Dictionary<string, object>>(this.Event_OnPlayerVoiceStarted));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerVoiceStopped", new Action<Dictionary<string, object>>(this.Event_OnPlayerVoiceStopped));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnPlayerJumpInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerJumpInput));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnPlayerDashLeftInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerDashLeftInput));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnPlayerDashRightInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerDashRightInput));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnPlayerTwistLeftInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerTwistLeftInput));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnPlayerTwistRightInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerTwistRightInput));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPlayerCameraEnabled", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerCameraEnabled));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPlayerCameraDisabled", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerCameraDisabled));
		if (NetworkManager.Singleton.IsServer)
		{
			this.playerBody.Stamina = 1f;
		}
		base.OnNetworkSpawn();
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x00029420 File Offset: 0x00027620
	public override void OnNetworkDespawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodySpawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerRoleChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerRoleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerUsernameChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerUsernameChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerNumberChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerNumberChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerCountryChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerCountryChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerVisorChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerVisorChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerMustacheChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerMustacheChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerBeardChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerBeardChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerJerseyChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerJerseyChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnGamePhaseChanged", new Action<Dictionary<string, object>>(this.Event_OnGamePhaseChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerVoiceStarted", new Action<Dictionary<string, object>>(this.Event_OnPlayerVoiceStarted));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerVoiceStopped", new Action<Dictionary<string, object>>(this.Event_OnPlayerVoiceStopped));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnPlayerJumpInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerJumpInput));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnPlayerDashLeftInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerDashLeftInput));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnPlayerDashRightInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerDashRightInput));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnPlayerTwistLeftInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerTwistLeftInput));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnPlayerTwistRightInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerTwistRightInput));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPlayerCameraEnabled", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerCameraEnabled));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPlayerCameraDisabled", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerCameraDisabled));
		base.OnNetworkDespawn();
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x00029650 File Offset: 0x00027850
	private void Event_OnPlayerBodySpawned(Dictionary<string, object> message)
	{
		PlayerBodyV2 playerBodyV = (PlayerBodyV2)message["playerBody"];
		if (base.OwnerClientId == playerBodyV.OwnerClientId)
		{
			playerBodyV.UpdateMesh();
		}
		if (NetworkBehaviourSingleton<GameManager>.Instance.GameState.Value.Phase == GamePhase.FaceOff)
		{
			playerBodyV.Server_Freeze();
			return;
		}
		playerBodyV.Server_Unfreeze();
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x000296A8 File Offset: 0x000278A8
	private void Event_OnPlayerTeamChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.UpdateMesh();
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x000296A8 File Offset: 0x000278A8
	private void Event_OnPlayerRoleChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.UpdateMesh();
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x000296A8 File Offset: 0x000278A8
	private void Event_OnPlayerUsernameChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.UpdateMesh();
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x000296A8 File Offset: 0x000278A8
	private void Event_OnPlayerNumberChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.UpdateMesh();
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x000296A8 File Offset: 0x000278A8
	private void Event_OnPlayerCountryChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.UpdateMesh();
	}

	// Token: 0x06000708 RID: 1800 RVA: 0x000296A8 File Offset: 0x000278A8
	private void Event_OnPlayerVisorChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.UpdateMesh();
	}

	// Token: 0x06000709 RID: 1801 RVA: 0x000296A8 File Offset: 0x000278A8
	private void Event_OnPlayerMustacheChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.UpdateMesh();
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x000296A8 File Offset: 0x000278A8
	private void Event_OnPlayerBeardChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.UpdateMesh();
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x000296A8 File Offset: 0x000278A8
	private void Event_OnPlayerJerseyChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.UpdateMesh();
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x000296E0 File Offset: 0x000278E0
	private void Event_OnGamePhaseChanged(Dictionary<string, object> message)
	{
		GamePhase gamePhase = (GamePhase)message["newGamePhase"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!this.playerBody.Player)
		{
			return;
		}
		if (gamePhase == GamePhase.FaceOff)
		{
			this.playerBody.Server_Freeze();
			return;
		}
		this.playerBody.Server_Unfreeze();
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x0002973C File Offset: 0x0002793C
	private void Event_OnPlayerVoiceStarted(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		AudioClip clip = (AudioClip)message["audioClip"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		if (player.IsLocalPlayer)
		{
			return;
		}
		this.playerBody.VoiceAudioSource.clip = clip;
		this.playerBody.VoiceAudioSource.loop = true;
		this.playerBody.VoiceAudioSource.Play();
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x000297B8 File Offset: 0x000279B8
	private void Event_OnPlayerVoiceStopped(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		if (player.IsLocalPlayer)
		{
			return;
		}
		this.playerBody.VoiceAudioSource.Stop();
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x00029800 File Offset: 0x00027A00
	private void Event_Server_OnPlayerJumpInput(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.Jump();
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x00029838 File Offset: 0x00027A38
	private void Event_Server_OnPlayerTwistLeftInput(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.TwistLeft();
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x00029870 File Offset: 0x00027A70
	private void Event_Server_OnPlayerTwistRightInput(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.TwistRight();
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x000298A8 File Offset: 0x00027AA8
	private void Event_Server_OnPlayerDashLeftInput(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.DashLeft();
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x000298E0 File Offset: 0x00027AE0
	private void Event_Server_OnPlayerDashRightInput(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.DashRight();
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x00029918 File Offset: 0x00027B18
	private void Event_Client_OnPlayerCameraEnabled(Dictionary<string, object> message)
	{
		PlayerCamera playerCamera = (PlayerCamera)message["playerCamera"];
		if (base.OwnerClientId != playerCamera.OwnerClientId)
		{
			return;
		}
		this.playerBody.MeshRendererHider.HideMeshRenderers();
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x00029958 File Offset: 0x00027B58
	private void Event_Client_OnPlayerCameraDisabled(Dictionary<string, object> message)
	{
		PlayerCamera playerCamera = (PlayerCamera)message["playerCamera"];
		if (base.OwnerClientId != playerCamera.OwnerClientId)
		{
			return;
		}
		this.playerBody.MeshRendererHider.ShowMeshRenderers();
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000719 RID: 1817 RVA: 0x0000B85D File Offset: 0x00009A5D
	protected internal override string __getTypeName()
	{
		return "PlayerBodyV2Controller";
	}

	// Token: 0x0400042A RID: 1066
	private PlayerBodyV2 playerBody;
}
