using System;
using System.Collections.Generic;
using Steamworks;
using Unity.Netcode;

// Token: 0x0200003F RID: 63
public class PlayerVoiceRecorderController : NetworkBehaviour
{
	// Token: 0x06000204 RID: 516 RVA: 0x0000D7C4 File Offset: 0x0000B9C4
	private void Awake()
	{
		this.playerVoiceRecorder = base.GetComponent<PlayerVoiceRecorder>();
	}

	// Token: 0x06000205 RID: 517 RVA: 0x0000D7D2 File Offset: 0x0000B9D2
	private void Start()
	{
		this.playerVoiceRecorder.IsEnabled = NetworkBehaviourSingleton<ServerManager>.Instance.Server.Value.UseVoip;
	}

	// Token: 0x06000206 RID: 518 RVA: 0x0000D7F3 File Offset: 0x0000B9F3
	public override void OnNetworkSpawn()
	{
		EventManager.AddEventListener("Event_Everyone_OnPlayerTalkInput", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerTalkInput));
		EventManager.AddEventListener("Event_Everyone_OnServerChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnServerChanged));
		base.OnNetworkSpawn();
	}

	// Token: 0x06000207 RID: 519 RVA: 0x0000D827 File Offset: 0x0000BA27
	public override void OnNetworkDespawn()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerTalkInput", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerTalkInput));
		EventManager.RemoveEventListener("Event_Everyone_OnServerChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnServerChanged));
		base.OnNetworkDespawn();
	}

	// Token: 0x06000208 RID: 520 RVA: 0x0000D85C File Offset: 0x0000BA5C
	private void Event_Everyone_OnPlayerTalkInput(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		bool flag = (bool)message["value"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		if (!player.IsLocalPlayer)
		{
			return;
		}
		if (flag)
		{
			this.playerVoiceRecorder.Client_RequestVoiceStartRpc(SteamUser.GetVoiceOptimalSampleRate(), default(RpcParams));
			return;
		}
		this.playerVoiceRecorder.Client_RequestVoiceStopRpc(default(RpcParams));
	}

	// Token: 0x06000209 RID: 521 RVA: 0x0000D7D2 File Offset: 0x0000B9D2
	private void Event_Everyone_OnServerChanged(Dictionary<string, object> message)
	{
		this.playerVoiceRecorder.IsEnabled = NetworkBehaviourSingleton<ServerManager>.Instance.Server.Value.UseVoip;
	}

	// Token: 0x0600020B RID: 523 RVA: 0x0000D8D4 File Offset: 0x0000BAD4
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x0600020C RID: 524 RVA: 0x000021BE File Offset: 0x000003BE
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x0600020D RID: 525 RVA: 0x0000D8EA File Offset: 0x0000BAEA
	protected internal override string __getTypeName()
	{
		return "PlayerVoiceRecorderController";
	}

	// Token: 0x0400014D RID: 333
	private PlayerVoiceRecorder playerVoiceRecorder;
}
