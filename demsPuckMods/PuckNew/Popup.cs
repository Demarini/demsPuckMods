using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

// Token: 0x020001B8 RID: 440
public class Popup
{
	// Token: 0x06000CA0 RID: 3232 RVA: 0x0003B431 File Offset: 0x00039631
	public Popup(VisualElement visualElement, string name, string title, object content, bool showOkButton, bool showCloseButton)
	{
		this.VisualElement = visualElement;
		this.Name = name;
		this.Title = title;
		this.Content = content;
		this.ShowOkButton = showOkButton;
		this.ShowCloseButton = showCloseButton;
		this.Initialize();
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x0003B46C File Offset: 0x0003966C
	public void Initialize()
	{
		this.header = this.VisualElement.Query("Header", null);
		this.content = this.VisualElement.Query("Content", null);
		this.footer = this.VisualElement.Query("Footer", null);
		this.titleLabel = this.header.Query(null, null);
		this.titleLabel.text = this.Title;
		this.closeIconButton = this.header.Query("CloseIconButtonContainer", null).First().Query(null, null);
		this.closeIconButton.clicked += this.OnClickClose;
		this.okButton = this.footer.Query("OkButton", null);
		this.okButton.clicked += this.OnClickOk;
		if (!this.ShowOkButton)
		{
			this.okButton.style.display = DisplayStyle.None;
		}
		if (!this.ShowCloseButton)
		{
			this.closeIconButton.style.display = DisplayStyle.None;
		}
		IPopupContent popupContent = this.Content as IPopupContent;
		if (popupContent != null)
		{
			popupContent.Initialize(this.content);
		}
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x0003B5C4 File Offset: 0x000397C4
	public void Dispose()
	{
		this.closeIconButton.clicked -= this.OnClickClose;
		this.okButton.clicked -= this.OnClickOk;
		IPopupContent popupContent = this.Content as IPopupContent;
		if (popupContent != null)
		{
			popupContent.Dispose();
		}
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x0003B614 File Offset: 0x00039814
	private void OnClickClose()
	{
		EventManager.TriggerEvent("Event_OnPopupClickClose", new Dictionary<string, object>
		{
			{
				"name",
				this.Name
			},
			{
				"content",
				this.Content
			}
		});
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x0003B647 File Offset: 0x00039847
	private void OnClickOk()
	{
		EventManager.TriggerEvent("Event_OnPopupClickOk", new Dictionary<string, object>
		{
			{
				"name",
				this.Name
			},
			{
				"content",
				this.Content
			}
		});
	}

	// Token: 0x04000776 RID: 1910
	public TemplateContainer TemplateContainer;

	// Token: 0x04000777 RID: 1911
	public VisualElement VisualElement;

	// Token: 0x04000778 RID: 1912
	public string Name;

	// Token: 0x04000779 RID: 1913
	public string Title;

	// Token: 0x0400077A RID: 1914
	public object Content;

	// Token: 0x0400077B RID: 1915
	public bool ShowOkButton;

	// Token: 0x0400077C RID: 1916
	public bool ShowCloseButton;

	// Token: 0x0400077D RID: 1917
	private VisualElement header;

	// Token: 0x0400077E RID: 1918
	private VisualElement content;

	// Token: 0x0400077F RID: 1919
	private VisualElement footer;

	// Token: 0x04000780 RID: 1920
	private Label titleLabel;

	// Token: 0x04000781 RID: 1921
	private Button okButton;

	// Token: 0x04000782 RID: 1922
	private IconButton closeIconButton;
}
