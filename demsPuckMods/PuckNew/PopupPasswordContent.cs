using System;
using UnityEngine.UIElements;

// Token: 0x020001B7 RID: 439
public class PopupPasswordContent : IPopupContent
{
	// Token: 0x1700011A RID: 282
	// (get) Token: 0x06000C9A RID: 3226 RVA: 0x0003B359 File Offset: 0x00039559
	// (set) Token: 0x06000C9B RID: 3227 RVA: 0x0003B361 File Offset: 0x00039561
	public object Data { get; set; }

	// Token: 0x06000C9C RID: 3228 RVA: 0x0003B36A File Offset: 0x0003956A
	public PopupPasswordContent(VisualTreeAsset asset, object data = null)
	{
		this.Asset = asset;
		this.Data = data;
	}

	// Token: 0x06000C9D RID: 3229 RVA: 0x0003B380 File Offset: 0x00039580
	public void Initialize(VisualElement parent)
	{
		this.assetInstance = this.Asset.Instantiate();
		this.textField = this.assetInstance.Query("PasswordTextField", null).First().Query(null, null);
		this.textField.value = this.Password;
		this.textField.RegisterCallback<ChangeEvent<string>>(new EventCallback<ChangeEvent<string>>(this.OnPasswordChanged), TrickleDown.NoTrickleDown);
		parent.Add(this.assetInstance);
	}

	// Token: 0x06000C9E RID: 3230 RVA: 0x0003B3FE File Offset: 0x000395FE
	public void Dispose()
	{
		this.textField.UnregisterCallback<ChangeEvent<string>>(new EventCallback<ChangeEvent<string>>(this.OnPasswordChanged), TrickleDown.NoTrickleDown);
		this.assetInstance.RemoveFromHierarchy();
	}

	// Token: 0x06000C9F RID: 3231 RVA: 0x0003B423 File Offset: 0x00039623
	private void OnPasswordChanged(ChangeEvent<string> changeEvent)
	{
		this.Password = changeEvent.newValue;
	}

	// Token: 0x04000772 RID: 1906
	public VisualTreeAsset Asset;

	// Token: 0x04000773 RID: 1907
	public string Password;

	// Token: 0x04000774 RID: 1908
	private VisualElement assetInstance;

	// Token: 0x04000775 RID: 1909
	private TextField textField;
}
