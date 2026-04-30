using System;
using UnityEngine.UIElements;

// Token: 0x020001B6 RID: 438
public class PopupTextContent : IPopupContent
{
	// Token: 0x17000119 RID: 281
	// (get) Token: 0x06000C95 RID: 3221 RVA: 0x0003B2DE File Offset: 0x000394DE
	// (set) Token: 0x06000C96 RID: 3222 RVA: 0x0003B2E6 File Offset: 0x000394E6
	public object Data { get; set; }

	// Token: 0x06000C97 RID: 3223 RVA: 0x0003B2EF File Offset: 0x000394EF
	public PopupTextContent(VisualTreeAsset asset, string text, object data = null)
	{
		this.Asset = asset;
		this.Text = text;
		this.Data = data;
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x0003B30C File Offset: 0x0003950C
	public void Initialize(VisualElement parent)
	{
		this.assetInstance = this.Asset.Instantiate();
		this.assetInstance.Query("TextLabel", null).text = this.Text;
		parent.Add(this.assetInstance);
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x0003B34C File Offset: 0x0003954C
	public void Dispose()
	{
		this.assetInstance.RemoveFromHierarchy();
	}

	// Token: 0x0400076E RID: 1902
	public VisualTreeAsset Asset;

	// Token: 0x0400076F RID: 1903
	public string Text;

	// Token: 0x04000770 RID: 1904
	private VisualElement assetInstance;
}
