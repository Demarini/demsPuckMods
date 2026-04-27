using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000036 RID: 54
public class PlayerBodyController : NetworkBehaviour
{
	// Token: 0x0600014E RID: 334 RVA: 0x0000743D File Offset: 0x0000563D
	private void Awake()
	{
		this.playerBody = base.GetComponent<PlayerBody>();
	}

	// Token: 0x0600014F RID: 335 RVA: 0x0000744C File Offset: 0x0000564C
	public override void OnNetworkSpawn()
	{
		EventManager.AddEventListener("Event_Everyone_OnPlayerGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerGameStateChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerUsernameChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerUsernameChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerNumberChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerNumberChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerCustomizationStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerCustomizationStateChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerVoiceStarted", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerVoiceStarted));
		EventManager.AddEventListener("Event_Everyone_OnPlayerVoiceStopped", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerVoiceStopped));
		EventManager.AddEventListener("Event_Server_OnPlayerJumpInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerJumpInput));
		EventManager.AddEventListener("Event_Server_OnPlayerDashLeftInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerDashLeftInput));
		EventManager.AddEventListener("Event_Server_OnPlayerDashRightInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerDashRightInput));
		EventManager.AddEventListener("Event_Server_OnPlayerTwistLeftInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerTwistLeftInput));
		EventManager.AddEventListener("Event_Server_OnPlayerTwistRightInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerTwistRightInput));
		EventManager.AddEventListener("Event_OnPlayerCameraEnabled", new Action<Dictionary<string, object>>(this.Event_OnPlayerCameraEnabled));
		EventManager.AddEventListener("Event_OnPlayerCameraDisabled", new Action<Dictionary<string, object>>(this.Event_OnPlayerCameraDisabled));
		base.OnNetworkSpawn();
	}

	// Token: 0x06000150 RID: 336 RVA: 0x00007580 File Offset: 0x00005780
	public override void OnNetworkDespawn()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerGameStateChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerUsernameChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerUsernameChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerNumberChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerNumberChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerCustomizationStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerCustomizationStateChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerVoiceStarted", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerVoiceStarted));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerVoiceStopped", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerVoiceStopped));
		EventManager.RemoveEventListener("Event_Server_OnPlayerJumpInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerJumpInput));
		EventManager.RemoveEventListener("Event_Server_OnPlayerDashLeftInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerDashLeftInput));
		EventManager.RemoveEventListener("Event_Server_OnPlayerDashRightInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerDashRightInput));
		EventManager.RemoveEventListener("Event_Server_OnPlayerTwistLeftInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerTwistLeftInput));
		EventManager.RemoveEventListener("Event_Server_OnPlayerTwistRightInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerTwistRightInput));
		EventManager.RemoveEventListener("Event_OnPlayerCameraEnabled", new Action<Dictionary<string, object>>(this.Event_OnPlayerCameraEnabled));
		EventManager.RemoveEventListener("Event_OnPlayerCameraDisabled", new Action<Dictionary<string, object>>(this.Event_OnPlayerCameraDisabled));
		base.OnNetworkDespawn();
	}

	// Token: 0x06000151 RID: 337 RVA: 0x000076B4 File Offset: 0x000058B4
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
		this.playerBody.ApplyCustomizations();
	}

	// Token: 0x06000152 RID: 338 RVA: 0x0000772C File Offset: 0x0000592C
	private void Event_Everyone_OnPlayerUsernameChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId == player.OwnerClientId)
		{
			this.playerBody.ApplyCustomizations();
		}
	}

	// Token: 0x06000153 RID: 339 RVA: 0x00007764 File Offset: 0x00005964
	private void Event_Everyone_OnPlayerNumberChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId == player.OwnerClientId)
		{
			this.playerBody.ApplyCustomizations();
		}
	}

	// Token: 0x06000154 RID: 340 RVA: 0x0000779C File Offset: 0x0000599C
	private void Event_Everyone_OnPlayerCustomizationStateChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId == player.OwnerClientId)
		{
			this.playerBody.ApplyCustomizations();
		}
	}

	// Token: 0x06000155 RID: 341 RVA: 0x000077D4 File Offset: 0x000059D4
	private void Event_Everyone_OnPlayerVoiceStarted(Dictionary<string, object> message)
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

	// Token: 0x06000156 RID: 342 RVA: 0x00007850 File Offset: 0x00005A50
	private void Event_Everyone_OnPlayerVoiceStopped(Dictionary<string, object> message)
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

	// Token: 0x06000157 RID: 343 RVA: 0x00007898 File Offset: 0x00005A98
	private void Event_Server_OnPlayerJumpInput(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.Jump();
	}

	// Token: 0x06000158 RID: 344 RVA: 0x000078D0 File Offset: 0x00005AD0
	private void Event_Server_OnPlayerTwistLeftInput(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.TwistLeft();
	}

	// Token: 0x06000159 RID: 345 RVA: 0x00007908 File Offset: 0x00005B08
	private void Event_Server_OnPlayerTwistRightInput(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.TwistRight();
	}

	// Token: 0x0600015A RID: 346 RVA: 0x00007940 File Offset: 0x00005B40
	private void Event_Server_OnPlayerDashLeftInput(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.DashLeft();
	}

	// Token: 0x0600015B RID: 347 RVA: 0x00007978 File Offset: 0x00005B78
	private void Event_Server_OnPlayerDashRightInput(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		this.playerBody.DashRight();
	}

	// Token: 0x0600015C RID: 348 RVA: 0x000079B0 File Offset: 0x00005BB0
	private void Event_OnPlayerCameraEnabled(Dictionary<string, object> message)
	{
		PlayerCamera playerCamera = (PlayerCamera)message["playerCamera"];
		if (base.OwnerClientId != playerCamera.OwnerClientId)
		{
			return;
		}
		this.playerBody.MeshRendererHider.HideMeshRenderers();
	}

	// Token: 0x0600015D RID: 349 RVA: 0x000079F0 File Offset: 0x00005BF0
	private void Event_OnPlayerCameraDisabled(Dictionary<string, object> message)
	{
		PlayerCamera playerCamera = (PlayerCamera)message["playerCamera"];
		if (base.OwnerClientId != playerCamera.OwnerClientId)
		{
			return;
		}
		this.playerBody.MeshRendererHider.ShowMeshRenderers();
	}

	// Token: 0x0600015F RID: 351 RVA: 0x00007A30 File Offset: 0x00005C30
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000160 RID: 352 RVA: 0x000021BE File Offset: 0x000003BE
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000161 RID: 353 RVA: 0x00007A46 File Offset: 0x00005C46
	protected internal override string __getTypeName()
	{
		return "PlayerBodyController";
	}

	// Token: 0x0400010D RID: 269
	private PlayerBody playerBody;
}
