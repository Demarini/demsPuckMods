using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x02000059 RID: 89
public class PuckPosition : NetworkBehaviour
{
	// Token: 0x06000302 RID: 770 RVA: 0x00012B5E File Offset: 0x00010D5E
	protected override void OnNetworkPostSpawn()
	{
		EventManager.TriggerEvent("Event_Everyone_OnPuckPositionSpawned", new Dictionary<string, object>
		{
			{
				"puckPosition",
				this
			}
		});
		base.OnNetworkPostSpawn();
	}

	// Token: 0x06000303 RID: 771 RVA: 0x00012B81 File Offset: 0x00010D81
	public override void OnNetworkDespawn()
	{
		EventManager.TriggerEvent("Event_Everyone_OnPuckPositionDespawned", new Dictionary<string, object>
		{
			{
				"puckPosition",
				this
			}
		});
		base.OnNetworkDespawn();
	}

	// Token: 0x06000305 RID: 773 RVA: 0x00012BA4 File Offset: 0x00010DA4
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000306 RID: 774 RVA: 0x000021BE File Offset: 0x000003BE
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000307 RID: 775 RVA: 0x00012BBA File Offset: 0x00010DBA
	protected internal override string __getTypeName()
	{
		return "PuckPosition";
	}

	// Token: 0x04000212 RID: 530
	public GamePhase Phase;
}
