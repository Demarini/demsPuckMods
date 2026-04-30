using System;
using Linework.SoftOutline;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Token: 0x02000054 RID: 84
public class PostProcessing : MonoBehaviour
{
	// Token: 0x060002E5 RID: 741 RVA: 0x0001262D File Offset: 0x0001082D
	private void Awake()
	{
		this.volume = base.GetComponent<Volume>();
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x0001263B File Offset: 0x0001083B
	public void SetPuckSilhouette(bool enabled)
	{
		this.universalRendererData.rendererFeatures.Find((ScriptableRendererFeature x) => x.name == "Puck Silhouette").SetActive(enabled);
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x00012672 File Offset: 0x00010872
	public void SetPuckOutline(bool enabled)
	{
		this.puckOutlineSettings.SetActive(enabled);
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x00012680 File Offset: 0x00010880
	public void SetQuality(ApplicationQuality quality)
	{
		switch (quality)
		{
		case ApplicationQuality.Low:
			this.renderPipelineAsset.msaaSampleCount = 1;
			return;
		case ApplicationQuality.Medium:
			this.renderPipelineAsset.msaaSampleCount = 2;
			return;
		case ApplicationQuality.High:
			this.renderPipelineAsset.msaaSampleCount = 4;
			return;
		case ApplicationQuality.Ultra:
			this.renderPipelineAsset.msaaSampleCount = 8;
			return;
		default:
			return;
		}
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x000126D8 File Offset: 0x000108D8
	public void SetMotionBlur(bool enabled)
	{
		MotionBlur motionBlur;
		if (this.volume.profile.TryGet<MotionBlur>(out motionBlur))
		{
			motionBlur.active = enabled;
		}
	}

	// Token: 0x04000203 RID: 515
	[Header("References")]
	[SerializeField]
	private UniversalRenderPipelineAsset renderPipelineAsset;

	// Token: 0x04000204 RID: 516
	[SerializeField]
	private UniversalRendererData universalRendererData;

	// Token: 0x04000205 RID: 517
	[SerializeField]
	private SoftOutlineSettings puckOutlineSettings;

	// Token: 0x04000206 RID: 518
	private Volume volume;
}
