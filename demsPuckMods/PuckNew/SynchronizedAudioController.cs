using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x0200006D RID: 109
public class SynchronizedAudioController : NetworkBehaviour
{
	// Token: 0x0600039B RID: 923 RVA: 0x00015272 File Offset: 0x00013472
	private void Awake()
	{
		this.synchronizedAudio = base.GetComponent<SynchronizedAudio>();
	}

	// Token: 0x0600039C RID: 924 RVA: 0x00015280 File Offset: 0x00013480
	public override void OnNetworkSpawn()
	{
		EventManager.AddEventListener("Event_Server_OnClientSceneSynchronizeComplete", new Action<Dictionary<string, object>>(this.Event_Server_OnClientSceneSynchronizeComplete));
		base.OnNetworkSpawn();
	}

	// Token: 0x0600039D RID: 925 RVA: 0x0001529E File Offset: 0x0001349E
	public override void OnNetworkDespawn()
	{
		EventManager.RemoveEventListener("Event_Server_OnClientSceneSynchronizeComplete", new Action<Dictionary<string, object>>(this.Event_Server_OnClientSceneSynchronizeComplete));
		base.OnNetworkDespawn();
	}

	// Token: 0x0600039E RID: 926 RVA: 0x000152BC File Offset: 0x000134BC
	private void Event_Server_OnClientSceneSynchronizeComplete(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["clientId"];
		if (num == 0UL)
		{
			return;
		}
		this.synchronizedAudio.Server_ForceSynchronizeClientId(num);
	}

	// Token: 0x060003A0 RID: 928 RVA: 0x000152EC File Offset: 0x000134EC
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x000021BE File Offset: 0x000003BE
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x00015302 File Offset: 0x00013502
	protected internal override string __getTypeName()
	{
		return "SynchronizedAudioController";
	}

	// Token: 0x04000284 RID: 644
	private SynchronizedAudio synchronizedAudio;
}
