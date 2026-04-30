using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000056 RID: 86
public class PostProcessingController : MonoBehaviour
{
	// Token: 0x060002EE RID: 750 RVA: 0x00012720 File Offset: 0x00010920
	private void Awake()
	{
		this.postProcessing = base.GetComponent<PostProcessing>();
		EventManager.AddEventListener("Event_OnShowPuckSilhouetteChanged", new Action<Dictionary<string, object>>(this.Event_OnShowPuckSilhouetteChanged));
		EventManager.AddEventListener("Event_OnShowPuckOutlineChanged", new Action<Dictionary<string, object>>(this.Event_OnShowPuckOutlineChanged));
		EventManager.AddEventListener("Event_OnQualityChanged", new Action<Dictionary<string, object>>(this.Event_OnQualityChanged));
		EventManager.AddEventListener("Event_OnMotionBlurChanged", new Action<Dictionary<string, object>>(this.Event_OnMotionBlurChanged));
	}

	// Token: 0x060002EF RID: 751 RVA: 0x00012794 File Offset: 0x00010994
	private void Start()
	{
		this.postProcessing.SetPuckSilhouette(SettingsManager.ShowPuckSilhouette);
		this.postProcessing.SetPuckOutline(SettingsManager.ShowPuckOutline);
		this.postProcessing.SetQuality(SettingsManager.Quality);
		this.postProcessing.SetMotionBlur(SettingsManager.MotionBlur);
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x000127E4 File Offset: 0x000109E4
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnShowPuckSilhouetteChanged", new Action<Dictionary<string, object>>(this.Event_OnShowPuckSilhouetteChanged));
		EventManager.RemoveEventListener("Event_OnShowPuckOutlineChanged", new Action<Dictionary<string, object>>(this.Event_OnShowPuckOutlineChanged));
		EventManager.RemoveEventListener("Event_OnQualityChanged", new Action<Dictionary<string, object>>(this.Event_OnQualityChanged));
		EventManager.RemoveEventListener("Event_OnMotionBlurChanged", new Action<Dictionary<string, object>>(this.Event_OnMotionBlurChanged));
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x0001284C File Offset: 0x00010A4C
	private void Event_OnShowPuckSilhouetteChanged(Dictionary<string, object> message)
	{
		bool puckSilhouette = (bool)message["value"];
		this.postProcessing.SetPuckSilhouette(puckSilhouette);
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x00012878 File Offset: 0x00010A78
	private void Event_OnShowPuckOutlineChanged(Dictionary<string, object> message)
	{
		bool puckOutline = (bool)message["value"];
		this.postProcessing.SetPuckOutline(puckOutline);
	}

	// Token: 0x060002F3 RID: 755 RVA: 0x000128A4 File Offset: 0x00010AA4
	private void Event_OnQualityChanged(Dictionary<string, object> message)
	{
		ApplicationQuality quality = (ApplicationQuality)message["value"];
		this.postProcessing.SetQuality(quality);
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x000128D0 File Offset: 0x00010AD0
	private void Event_OnMotionBlurChanged(Dictionary<string, object> message)
	{
		bool motionBlur = (bool)message["value"];
		this.postProcessing.SetMotionBlur(motionBlur);
	}

	// Token: 0x04000209 RID: 521
	private PostProcessing postProcessing;
}
