using System;
using Unity.Netcode;

// Token: 0x020000D4 RID: 212
public class SynchronizedObjectController : NetworkBehaviour
{
	// Token: 0x0600066B RID: 1643 RVA: 0x0000B135 File Offset: 0x00009335
	private void Awake()
	{
		this.synchronizedObject = base.GetComponent<SynchronizedObject>();
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x0000B143 File Offset: 0x00009343
	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x000080B0 File Offset: 0x000062B0
	public override void OnNetworkDespawn()
	{
		base.OnNetworkDespawn();
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x0000B14B File Offset: 0x0000934B
	protected internal override string __getTypeName()
	{
		return "SynchronizedObjectController";
	}

	// Token: 0x04000380 RID: 896
	private SynchronizedObject synchronizedObject;
}
