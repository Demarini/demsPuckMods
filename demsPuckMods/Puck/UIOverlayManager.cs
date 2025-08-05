using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000118 RID: 280
public class UIOverlayManager : UIComponent<UIOverlayManager>
{
	// Token: 0x060009E8 RID: 2536 RVA: 0x0000D4EC File Offset: 0x0000B6EC
	public override void Awake()
	{
		base.Awake();
		base.AlwaysVisible = true;
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x0000D4FB File Offset: 0x0000B6FB
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("OverlaysContainer", null);
		this.spinnerContainer = this.container.Query("SpinnerContainer", null);
		this.UpdateSpinnerVisibility(false);
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x0003B128 File Offset: 0x00039328
	public void ShowOverlay(string overlayName, bool showSpinner = false, bool fade = false, bool autoHide = false, bool autoHideFade = false)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		if (!this.activeOverlays.ContainsKey(overlayName))
		{
			TemplateContainer templateContainer = Utils.InstantiateVisualTreeAsset(this.overlayAsset, Position.Absolute);
			VisualElement visualElement = templateContainer.Query("Overlay", null);
			Overlay overlay = new Overlay
			{
				Name = overlayName,
				ShowSpinner = showSpinner,
				VisualElement = visualElement,
				TemplateContainer = templateContainer
			};
			this.activeOverlays.Add(overlayName, overlay);
			base.StartCoroutine(this.IShowOverlay(overlay, fade, autoHide, autoHideFade));
		}
		this.UpdateSpinnerVisibility(fade);
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x0000D537 File Offset: 0x0000B737
	private IEnumerator IShowOverlay(Overlay overlay, bool fade, bool autoHide, bool autoHideFade)
	{
		this.container.Add(overlay.TemplateContainer);
		overlay.TemplateContainer.SendToBack();
		overlay.VisualElement.style.opacity = 0f;
		overlay.VisualElement.EnableInClassList("fade", fade);
		if (fade)
		{
			yield return new WaitForEndOfFrame();
		}
		overlay.VisualElement.style.opacity = 1f;
		if (autoHide)
		{
			if (autoHideFade)
			{
				yield return new WaitForSeconds(0.2f);
			}
			else
			{
				yield return new WaitForEndOfFrame();
			}
			this.HideOverlay(overlay.Name, autoHideFade);
		}
		yield return null;
		yield break;
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x0000D563 File Offset: 0x0000B763
	public void HideOverlay(string overlayName, bool fade = false)
	{
		if (this.activeOverlays.ContainsKey(overlayName))
		{
			base.StartCoroutine(this.IHideOverlay(this.activeOverlays[overlayName], fade));
			this.activeOverlays.Remove(overlayName);
		}
		this.UpdateSpinnerVisibility(fade);
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x0000D5A1 File Offset: 0x0000B7A1
	private IEnumerator IHideOverlay(Overlay overlay, bool fade)
	{
		overlay.VisualElement.style.opacity = 1f;
		overlay.VisualElement.EnableInClassList("fade", fade);
		if (fade)
		{
			yield return new WaitForEndOfFrame();
		}
		overlay.VisualElement.style.opacity = 0f;
		yield return new WaitForSeconds(0.2f);
		this.container.Remove(overlay.TemplateContainer);
		yield return null;
		yield break;
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x0003B1B4 File Offset: 0x000393B4
	public void UpdateSpinnerVisibility(bool fade)
	{
		if (this.activeOverlays.Values.Any((Overlay overlay) => overlay.ShowSpinner))
		{
			this.ShowSpinner(fade);
			return;
		}
		this.HideSpinner(fade);
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x0003B204 File Offset: 0x00039404
	private void ShowSpinner(bool fade = false)
	{
		if (this.spinnerShowCoroutine != null)
		{
			base.StopCoroutine(this.spinnerShowCoroutine);
		}
		if (this.spinnerHideCoroutine != null)
		{
			base.StopCoroutine(this.spinnerHideCoroutine);
		}
		this.spinnerShowCoroutine = this.IShowSpinner(fade);
		base.StartCoroutine(this.spinnerShowCoroutine);
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x0000D5BE File Offset: 0x0000B7BE
	private IEnumerator IShowSpinner(bool fade)
	{
		this.spinnerContainer.style.display = DisplayStyle.Flex;
		this.spinnerContainer.style.opacity = 0f;
		this.spinnerContainer.EnableInClassList("fade", fade);
		yield return new WaitForEndOfFrame();
		this.spinnerContainer.style.opacity = 1f;
		yield break;
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x0003B254 File Offset: 0x00039454
	private void HideSpinner(bool fade)
	{
		if (this.spinnerShowCoroutine != null)
		{
			base.StopCoroutine(this.spinnerShowCoroutine);
		}
		if (this.spinnerHideCoroutine != null)
		{
			base.StopCoroutine(this.spinnerHideCoroutine);
		}
		this.spinnerHideCoroutine = this.IHideSpinner(fade);
		base.StartCoroutine(this.spinnerHideCoroutine);
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x0000D5D4 File Offset: 0x0000B7D4
	private IEnumerator IHideSpinner(bool fade)
	{
		this.spinnerContainer.style.opacity = 1f;
		this.spinnerContainer.EnableInClassList("fade", fade);
		yield return new WaitForEndOfFrame();
		this.spinnerContainer.style.opacity = 0f;
		yield return new WaitForSeconds(0.2f);
		this.spinnerContainer.style.display = DisplayStyle.None;
		yield break;
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x0003B2A4 File Offset: 0x000394A4
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x0000D5FD File Offset: 0x0000B7FD
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x0000D607 File Offset: 0x0000B807
	protected internal override string __getTypeName()
	{
		return "UIOverlayManager";
	}

	// Token: 0x040005E3 RID: 1507
	[Header("Components")]
	public VisualTreeAsset overlayAsset;

	// Token: 0x040005E4 RID: 1508
	[HideInInspector]
	public Dictionary<string, Overlay> activeOverlays = new Dictionary<string, Overlay>();

	// Token: 0x040005E5 RID: 1509
	private VisualElement spinnerContainer;

	// Token: 0x040005E6 RID: 1510
	private IEnumerator spinnerShowCoroutine;

	// Token: 0x040005E7 RID: 1511
	private IEnumerator spinnerHideCoroutine;
}
