using System;

// Token: 0x02000067 RID: 103
public class SpectatorCameraController : BaseCameraController
{
	// Token: 0x0600036D RID: 877 RVA: 0x000144C6 File Offset: 0x000126C6
	public override void Awake()
	{
		base.Awake();
		this.spectatorCamera = base.GetComponent<SpectatorCamera>();
	}

	// Token: 0x04000267 RID: 615
	private SpectatorCamera spectatorCamera;
}
