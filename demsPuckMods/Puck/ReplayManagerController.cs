using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x0200008A RID: 138
public class ReplayManagerController : NetworkBehaviour
{
	// Token: 0x06000365 RID: 869 RVA: 0x0000928C File Offset: 0x0000748C
	private void Awake()
	{
		this.replayManager = base.GetComponent<ReplayManager>();
	}

	// Token: 0x06000366 RID: 870 RVA: 0x0000929A File Offset: 0x0000749A
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnGamePhaseChanged", new Action<Dictionary<string, object>>(this.Event_OnGamePhaseChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
	}

	// Token: 0x06000367 RID: 871 RVA: 0x000092D2 File Offset: 0x000074D2
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnGamePhaseChanged", new Action<Dictionary<string, object>>(this.Event_OnGamePhaseChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
		base.OnDestroy();
	}

	// Token: 0x06000368 RID: 872 RVA: 0x00018F68 File Offset: 0x00017168
	private void Event_OnGamePhaseChanged(Dictionary<string, object> message)
	{
		GamePhase gamePhase = (GamePhase)message["newGamePhase"];
		int num = (int)message["time"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		switch (gamePhase)
		{
		case GamePhase.Warmup:
			this.replayManager.Server_StopRecording();
			this.replayManager.Server_StopReplaying();
			return;
		case GamePhase.FaceOff:
			this.replayManager.Server_StopReplaying();
			this.replayManager.Server_StartRecording();
			return;
		case GamePhase.Playing:
		case GamePhase.BlueScore:
		case GamePhase.RedScore:
			break;
		case GamePhase.Replay:
			this.replayManager.Server_StopRecording();
			this.replayManager.Server_StartReplaying((float)num);
			return;
		default:
			this.replayManager.Server_StopRecording();
			this.replayManager.Server_StopReplaying();
			break;
		}
	}

	// Token: 0x06000369 RID: 873 RVA: 0x00009310 File Offset: 0x00007510
	private void Event_Server_OnServerStopped(Dictionary<string, object> message)
	{
		this.replayManager.Server_StopReplaying();
		this.replayManager.Server_StopRecording();
	}

	// Token: 0x0600036B RID: 875 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x0600036C RID: 876 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x0600036D RID: 877 RVA: 0x00009328 File Offset: 0x00007528
	protected internal override string __getTypeName()
	{
		return "ReplayManagerController";
	}

	// Token: 0x04000204 RID: 516
	private ReplayManager replayManager;
}
