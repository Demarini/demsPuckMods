using System;

// Token: 0x0200002D RID: 45
public class SpectatorCameraController : BaseCameraController
{
	// Token: 0x0600013D RID: 317 RVA: 0x00007A9D File Offset: 0x00005C9D
	public override void Awake()
	{
		base.Awake();
		this.spectatorCamera = base.GetComponent<SpectatorCamera>();
	}

	// Token: 0x0600013F RID: 319 RVA: 0x00011D00 File Offset: 0x0000FF00
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000140 RID: 320 RVA: 0x0000798A File Offset: 0x00005B8A
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000141 RID: 321 RVA: 0x00007AB1 File Offset: 0x00005CB1
	protected internal override string __getTypeName()
	{
		return "SpectatorCameraController";
	}

	// Token: 0x040000AB RID: 171
	private SpectatorCamera spectatorCamera;
}
