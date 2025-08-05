using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x020000EA RID: 234
public class PlayerVoiceRecorderController : NetworkBehaviour
{
	// Token: 0x060007CF RID: 1999 RVA: 0x0000BD4D File Offset: 0x00009F4D
	private void Awake()
	{
		this.playerVoiceRecorder = base.GetComponent<PlayerVoiceRecorder>();
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x0000BD5B File Offset: 0x00009F5B
	private void Start()
	{
		this.playerVoiceRecorder.IsEnabled = NetworkBehaviourSingleton<ServerManager>.Instance.Server.Voip;
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x0000BD77 File Offset: 0x00009F77
	public override void OnNetworkSpawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerTalkInput", new Action<Dictionary<string, object>>(this.Event_OnPlayerTalkInput));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnServerConfiguration", new Action<Dictionary<string, object>>(this.Event_Client_OnServerConfiguration));
		base.OnNetworkSpawn();
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x0000BDB5 File Offset: 0x00009FB5
	public override void OnNetworkDespawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerTalkInput", new Action<Dictionary<string, object>>(this.Event_OnPlayerTalkInput));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnServerConfiguration", new Action<Dictionary<string, object>>(this.Event_Client_OnServerConfiguration));
		base.OnNetworkDespawn();
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x0002ED54 File Offset: 0x0002CF54
	private void Event_OnPlayerTalkInput(Dictionary<string, object> message)
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
			this.playerVoiceRecorder.StartRecording();
			return;
		}
		this.playerVoiceRecorder.StopRecording();
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x0002EDB8 File Offset: 0x0002CFB8
	private void Event_Client_OnServerConfiguration(Dictionary<string, object> message)
	{
		Server server = (Server)message["server"];
		this.playerVoiceRecorder.IsEnabled = server.Voip;
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060007D7 RID: 2007 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x0000BDF3 File Offset: 0x00009FF3
	protected internal override string __getTypeName()
	{
		return "PlayerVoiceRecorderController";
	}

	// Token: 0x0400047C RID: 1148
	private PlayerVoiceRecorder playerVoiceRecorder;
}
