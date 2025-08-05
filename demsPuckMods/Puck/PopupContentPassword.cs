using System;
using UnityEngine.UIElements;

// Token: 0x02000125 RID: 293
public class PopupContentPassword : IPopupContent
{
	// Token: 0x06000A4B RID: 2635 RVA: 0x0000D8DC File Offset: 0x0000BADC
	public PopupContentPassword(VisualTreeAsset asset)
	{
		this.Asset = asset;
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x0003C050 File Offset: 0x0003A250
	public void Initialize(VisualElement containerVisualElement)
	{
		this.templateContainer = Utils.InstantiateVisualTreeAsset(this.Asset, Position.Relative);
		VisualElement e = this.templateContainer;
		this.textField = e.Query("PasswordTextField", null).First().Query("TextField", null);
		this.textField.value = this.Password;
		this.textField.RegisterCallback<ChangeEvent<string>>(new EventCallback<ChangeEvent<string>>(this.OnPasswordChanged), TrickleDown.NoTrickleDown);
		containerVisualElement.Add(this.templateContainer);
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x0000D8EB File Offset: 0x0000BAEB
	public void Dispose(VisualElement containerVisualElement)
	{
		this.textField.UnregisterCallback<ChangeEvent<string>>(new EventCallback<ChangeEvent<string>>(this.OnPasswordChanged), TrickleDown.NoTrickleDown);
		containerVisualElement.Remove(this.templateContainer);
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x0000D911 File Offset: 0x0000BB11
	private void OnPasswordChanged(ChangeEvent<string> changeEvent)
	{
		this.Password = changeEvent.newValue;
	}

	// Token: 0x0400060D RID: 1549
	public VisualTreeAsset Asset;

	// Token: 0x0400060E RID: 1550
	public string Password;

	// Token: 0x0400060F RID: 1551
	private TemplateContainer templateContainer;

	// Token: 0x04000610 RID: 1552
	private TextField textField;
}
