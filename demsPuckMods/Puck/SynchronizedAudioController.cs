using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x020000D2 RID: 210
public class SynchronizedAudioController : NetworkBehaviour
{
	// Token: 0x06000657 RID: 1623 RVA: 0x0000B09D File Offset: 0x0000929D
	private void Awake()
	{
		this.synchronizedAudio = base.GetComponent<SynchronizedAudio>();
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x0000B0AB File Offset: 0x000092AB
	public override void OnNetworkSpawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnSynchronizeComplete", new Action<Dictionary<string, object>>(this.Event_Server_OnSynchronizeComplete));
		base.OnNetworkSpawn();
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x0000B0CE File Offset: 0x000092CE
	public override void OnNetworkDespawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnSynchronizeComplete", new Action<Dictionary<string, object>>(this.Event_Server_OnSynchronizeComplete));
		base.OnNetworkDespawn();
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x00026050 File Offset: 0x00024250
	private void Event_Server_OnSynchronizeComplete(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["clientId"];
		if (num == 0UL)
		{
			return;
		}
		this.synchronizedAudio.Server_ForceSynchronizeClientId(num);
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x0000B0F1 File Offset: 0x000092F1
	protected internal override string __getTypeName()
	{
		return "SynchronizedAudioController";
	}

	// Token: 0x04000376 RID: 886
	private SynchronizedAudio synchronizedAudio;
}
