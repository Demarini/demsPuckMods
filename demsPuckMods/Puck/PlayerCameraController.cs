using System;

// Token: 0x020000E4 RID: 228
public class PlayerCameraController : BaseCameraController
{
	// Token: 0x0600072B RID: 1835 RVA: 0x0000B972 File Offset: 0x00009B72
	public override void Awake()
	{
		base.Awake();
		this.playerCamera = base.GetComponent<PlayerCamera>();
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x0000B986 File Offset: 0x00009B86
	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x0000B98E File Offset: 0x00009B8E
	public override void OnNetworkDespawn()
	{
		base.OnNetworkDespawn();
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x00011D00 File Offset: 0x0000FF00
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x0000798A File Offset: 0x00005B8A
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x0000B996 File Offset: 0x00009B96
	protected internal override string __getTypeName()
	{
		return "PlayerCameraController";
	}

	// Token: 0x0400043A RID: 1082
	public PlayerCamera playerCamera;
}
