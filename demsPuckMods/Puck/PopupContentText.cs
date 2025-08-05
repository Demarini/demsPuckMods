using System;
using UnityEngine.UIElements;

// Token: 0x02000124 RID: 292
public class PopupContentText : IPopupContent
{
	// Token: 0x06000A48 RID: 2632 RVA: 0x0000D8B8 File Offset: 0x0000BAB8
	public PopupContentText(VisualTreeAsset asset, string text)
	{
		this.Asset = asset;
		this.Text = text;
	}

	// Token: 0x06000A49 RID: 2633 RVA: 0x0003C004 File Offset: 0x0003A204
	public void Initialize(VisualElement containerVisualElement)
	{
		this.templateContainer = Utils.InstantiateVisualTreeAsset(this.Asset, Position.Relative);
		this.templateContainer.Query("TextLabel", null).text = this.Text;
		containerVisualElement.Add(this.templateContainer);
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x0000D8CE File Offset: 0x0000BACE
	public void Dispose(VisualElement containerVisualElement)
	{
		containerVisualElement.Remove(this.templateContainer);
	}

	// Token: 0x0400060A RID: 1546
	public VisualTreeAsset Asset;

	// Token: 0x0400060B RID: 1547
	public string Text;

	// Token: 0x0400060C RID: 1548
	private TemplateContainer templateContainer;
}
