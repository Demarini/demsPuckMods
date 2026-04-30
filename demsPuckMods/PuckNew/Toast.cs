using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x020001D3 RID: 467
public class Toast
{
	// Token: 0x06000DBB RID: 3515 RVA: 0x000411FF File Offset: 0x0003F3FF
	public Toast(UIToastManager uiToastManager, VisualElement visualElement, string name, string content, float hideDelay)
	{
		this.UIToastManager = uiToastManager;
		this.VisualElement = visualElement;
		this.Name = name;
		this.Content = content;
		this.HideDelay = hideDelay;
		this.Initialize();
	}

	// Token: 0x06000DBC RID: 3516 RVA: 0x00041234 File Offset: 0x0003F434
	public void Initialize()
	{
		this.VisualElement.RegisterCallback<ClickEvent>(new EventCallback<ClickEvent>(this.OnClick), TrickleDown.NoTrickleDown);
		this.contentLabel = this.VisualElement.Query("ContentLabel", null);
		this.contentLabel.text = this.Content;
		this.Hide();
	}

	// Token: 0x06000DBD RID: 3517 RVA: 0x0004128C File Offset: 0x0003F48C
	public void Hide()
	{
		this.hideCoroutine = this.IHide();
		this.UIToastManager.StartCoroutine(this.hideCoroutine);
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x000412AC File Offset: 0x0003F4AC
	private IEnumerator IHide()
	{
		yield return new WaitForSeconds(this.HideDelay);
		this.UIToastManager.HideToast(this.Name);
		this.hideCoroutine = null;
		yield break;
	}

	// Token: 0x06000DBF RID: 3519 RVA: 0x000412BB File Offset: 0x0003F4BB
	public void Dispose()
	{
		this.VisualElement.UnregisterCallback<ClickEvent>(new EventCallback<ClickEvent>(this.OnClick), TrickleDown.NoTrickleDown);
		if (this.hideCoroutine != null)
		{
			this.UIToastManager.StopCoroutine(this.hideCoroutine);
			this.hideCoroutine = null;
		}
	}

	// Token: 0x06000DC0 RID: 3520 RVA: 0x000412F5 File Offset: 0x0003F4F5
	private void OnClick(ClickEvent clickEvent)
	{
		this.UIToastManager.HideToast(this.Name);
	}

	// Token: 0x0400080D RID: 2061
	public UIToastManager UIToastManager;

	// Token: 0x0400080E RID: 2062
	public VisualElement VisualElement;

	// Token: 0x0400080F RID: 2063
	public string Name;

	// Token: 0x04000810 RID: 2064
	public string Content;

	// Token: 0x04000811 RID: 2065
	public float HideDelay;

	// Token: 0x04000812 RID: 2066
	private IEnumerator hideCoroutine;

	// Token: 0x04000813 RID: 2067
	private Label contentLabel;
}
