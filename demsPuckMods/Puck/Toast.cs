using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x0200013E RID: 318
public class Toast
{
	// Token: 0x06000B46 RID: 2886 RVA: 0x0000E88D File Offset: 0x0000CA8D
	public Toast(UIToastManager uiToastManager, TemplateContainer templateContainer, VisualElement visualElement, string name, string content, float hideDelay)
	{
		this.UIToastManager = uiToastManager;
		this.TemplateContainer = templateContainer;
		this.VisualElement = visualElement;
		this.Name = name;
		this.Content = content;
		this.HideDelay = hideDelay;
		this.Initialize();
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x000408D0 File Offset: 0x0003EAD0
	public void Initialize()
	{
		this.VisualElement.RegisterCallback<ClickEvent>(new EventCallback<ClickEvent>(this.OnClick), TrickleDown.NoTrickleDown);
		this.contentLabel = this.VisualElement.Query("ContentLabel", null);
		this.contentLabel.text = this.Content;
		this.Hide();
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x0000E8C8 File Offset: 0x0000CAC8
	public void Hide()
	{
		this.hideCoroutine = this.IHide();
		this.UIToastManager.StartCoroutine(this.hideCoroutine);
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x0000E8E8 File Offset: 0x0000CAE8
	private IEnumerator IHide()
	{
		yield return new WaitForSeconds(this.HideDelay);
		this.UIToastManager.HideToast(this.Name);
		this.hideCoroutine = null;
		yield break;
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x0000E8F7 File Offset: 0x0000CAF7
	public void Dispose()
	{
		this.VisualElement.UnregisterCallback<ClickEvent>(new EventCallback<ClickEvent>(this.OnClick), TrickleDown.NoTrickleDown);
		if (this.hideCoroutine != null)
		{
			this.UIToastManager.StopCoroutine(this.hideCoroutine);
			this.hideCoroutine = null;
		}
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x0000E931 File Offset: 0x0000CB31
	private void OnClick(ClickEvent clickEvent)
	{
		this.UIToastManager.HideToast(this.Name);
	}

	// Token: 0x040006A5 RID: 1701
	public UIToastManager UIToastManager;

	// Token: 0x040006A6 RID: 1702
	public TemplateContainer TemplateContainer;

	// Token: 0x040006A7 RID: 1703
	public VisualElement VisualElement;

	// Token: 0x040006A8 RID: 1704
	public string Name;

	// Token: 0x040006A9 RID: 1705
	public string Content;

	// Token: 0x040006AA RID: 1706
	public float HideDelay;

	// Token: 0x040006AB RID: 1707
	private IEnumerator hideCoroutine;

	// Token: 0x040006AC RID: 1708
	private Label contentLabel;
}
