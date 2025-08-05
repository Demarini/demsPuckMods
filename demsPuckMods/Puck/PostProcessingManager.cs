using System;
using Linework.SoftOutline;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Token: 0x02000073 RID: 115
public class PostProcessingManager : MonoBehaviourSingleton<PostProcessingManager>
{
	// Token: 0x0600031A RID: 794 RVA: 0x00008EB4 File Offset: 0x000070B4
	public override void Awake()
	{
		base.Awake();
		this.volume = base.GetComponent<Volume>();
	}

	// Token: 0x0600031B RID: 795 RVA: 0x000187D8 File Offset: 0x000169D8
	public void SetMotionBlur(bool enabled)
	{
		MotionBlur motionBlur;
		this.volume.profile.TryGet<MotionBlur>(out motionBlur);
		if (motionBlur)
		{
			motionBlur.active = enabled;
		}
	}

	// Token: 0x0600031C RID: 796 RVA: 0x00008EC8 File Offset: 0x000070C8
	public void SetMsaaSampleCount(int sampleCount)
	{
		this.renderPipelineAsset.msaaSampleCount = sampleCount;
	}

	// Token: 0x0600031D RID: 797 RVA: 0x00008ED6 File Offset: 0x000070D6
	public void SetObstructedPuck(bool enabled)
	{
		this.universalRendererData.rendererFeatures.Find((ScriptableRendererFeature x) => x.name == "Obstructed Puck").SetActive(enabled);
	}

	// Token: 0x0600031E RID: 798 RVA: 0x00008F0D File Offset: 0x0000710D
	public void SetPuckOutline(bool enabled)
	{
		this.puckOutlineSettings.SetActive(enabled);
	}

	// Token: 0x040001B1 RID: 433
	[Header("References")]
	[SerializeField]
	private UniversalRenderPipelineAsset renderPipelineAsset;

	// Token: 0x040001B2 RID: 434
	[SerializeField]
	private UniversalRendererData universalRendererData;

	// Token: 0x040001B3 RID: 435
	[SerializeField]
	private SoftOutlineSettings puckOutlineSettings;

	// Token: 0x040001B4 RID: 436
	private Volume volume;
}
