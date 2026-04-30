using System;

// Token: 0x0200003A RID: 58
public class PlayerCameraController : BaseCameraController
{
	// Token: 0x06000179 RID: 377 RVA: 0x00007EFE File Offset: 0x000060FE
	public override void Awake()
	{
		base.Awake();
		this.playerCamera = base.GetComponent<PlayerCamera>();
	}

	// Token: 0x0400011D RID: 285
	private PlayerCamera playerCamera;
}
