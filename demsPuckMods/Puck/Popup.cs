using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

// Token: 0x02000126 RID: 294
public class Popup
{
	// Token: 0x06000A4F RID: 2639 RVA: 0x0003C0D8 File Offset: 0x0003A2D8
	public Popup(TemplateContainer templateContainer, VisualElement visualElement, string name, string title, object content, bool showOkButton, bool showCloseButton)
	{
		this.TemplateContainer = templateContainer;
		this.VisualElement = visualElement;
		this.Name = name;
		this.Title = title;
		this.Content = content;
		this.ShowOkButton = showOkButton;
		this.ShowCloseButton = showCloseButton;
		this.Initialize();
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x0003C128 File Offset: 0x0003A328
	public void Initialize()
	{
		this.titleLabel = this.VisualElement.Query("TitleLabel", null);
		this.titleLabel.text = this.Title;
		this.contentContainerVisualElement = this.VisualElement.Query("ContentContainer", null);
		this.okButton = this.VisualElement.Query("OkButton", null);
		this.closeButton = this.VisualElement.Query("CloseButton", null);
		this.okButton.clicked += this.OnClickOk;
		this.closeButton.clicked += this.OnClickClose;
		if (!this.ShowOkButton)
		{
			this.okButton.style.display = DisplayStyle.None;
		}
		if (!this.ShowCloseButton)
		{
			this.closeButton.style.display = DisplayStyle.None;
		}
		IPopupContent popupContent = this.Content as IPopupContent;
		if (popupContent != null)
		{
			popupContent.Initialize(this.contentContainerVisualElement);
		}
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x0003C23C File Offset: 0x0003A43C
	public void Dispose()
	{
		this.okButton.clicked -= this.OnClickOk;
		this.closeButton.clicked -= this.OnClickClose;
		IPopupContent popupContent = this.Content as IPopupContent;
		if (popupContent != null)
		{
			popupContent.Dispose(this.contentContainerVisualElement);
		}
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x0000D91F File Offset: 0x0000BB1F
	private void OnClickOk()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPopupClickOk", new Dictionary<string, object>
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

	// Token: 0x06000A53 RID: 2643 RVA: 0x0000D957 File Offset: 0x0000BB57
	private void OnClickClose()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPopupClickClose", new Dictionary<string, object>
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

	// Token: 0x04000611 RID: 1553
	public TemplateContainer TemplateContainer;

	// Token: 0x04000612 RID: 1554
	public VisualElement VisualElement;

	// Token: 0x04000613 RID: 1555
	public string Name;

	// Token: 0x04000614 RID: 1556
	public string Title;

	// Token: 0x04000615 RID: 1557
	public object Content;

	// Token: 0x04000616 RID: 1558
	public bool ShowOkButton;

	// Token: 0x04000617 RID: 1559
	public bool ShowCloseButton;

	// Token: 0x04000618 RID: 1560
	private Label titleLabel;

	// Token: 0x04000619 RID: 1561
	private VisualElement contentContainerVisualElement;

	// Token: 0x0400061A RID: 1562
	private Button okButton;

	// Token: 0x0400061B RID: 1563
	private Button closeButton;
}
