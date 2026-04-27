using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x020001A9 RID: 425
public class UIOverlayManager : UIView
{
	// Token: 0x06000C3B RID: 3131 RVA: 0x00039E5B File Offset: 0x0003805B
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("OverlaysView", null);
		this.overlays = base.View.Query("Overlays", null);
		this.overlays.Clear();
	}

	// Token: 0x06000C3C RID: 3132 RVA: 0x00039E9C File Offset: 0x0003809C
	private void OnDestroy()
	{
		foreach (Overlay overlay in this.identifierOverlaysMap.Values)
		{
			overlay.Dispose();
		}
	}

	// Token: 0x06000C3D RID: 3133 RVA: 0x00039EF4 File Offset: 0x000380F4
	public void ShowOverlay(string identifier, bool requiresSpinner = false, bool fadeIn = false, bool fadeOut = false, float fadeTime = 0.25f, bool autoHide = false, float hideTimeout = 0.25f)
	{
		Overlay overlay;
		if (!this.identifierOverlaysMap.ContainsKey(identifier))
		{
			overlay = new Overlay(this.overlayAsset.Instantiate(), identifier, requiresSpinner);
			Overlay overlay2 = overlay;
			overlay2.Hidden = (Action)Delegate.Combine(overlay2.Hidden, new Action(delegate()
			{
				overlay.Dispose();
				this.identifierOverlaysMap.Remove(identifier);
				this.overlays.Remove(overlay.VisualElement);
			}));
			this.identifierOverlaysMap.Add(identifier, overlay);
			this.overlays.Add(overlay.VisualElement);
		}
		else
		{
			overlay = this.identifierOverlaysMap[identifier];
		}
		overlay.FadeIn = fadeIn;
		overlay.FadeOut = fadeOut;
		overlay.FadeTime = fadeTime;
		overlay.AutoHide = autoHide;
		overlay.HideTimeout = hideTimeout;
		overlay.Show();
	}

	// Token: 0x06000C3E RID: 3134 RVA: 0x00039FFD File Offset: 0x000381FD
	public void HideOverlay(string identifier)
	{
		if (!this.identifierOverlaysMap.ContainsKey(identifier))
		{
			return;
		}
		this.identifierOverlaysMap[identifier].Hide();
	}

	// Token: 0x04000746 RID: 1862
	[Header("References")]
	public VisualTreeAsset overlayAsset;

	// Token: 0x04000747 RID: 1863
	private VisualElement overlays;

	// Token: 0x04000748 RID: 1864
	private Dictionary<string, Overlay> identifierOverlaysMap = new Dictionary<string, Overlay>();
}
