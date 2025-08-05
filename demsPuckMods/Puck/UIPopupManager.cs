using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000127 RID: 295
public class UIPopupManager : UIComponent<UIPopupManager>
{
	// Token: 0x06000A54 RID: 2644 RVA: 0x0000D98F File Offset: 0x0000BB8F
	public override void Awake()
	{
		base.Awake();
		base.VisibilityRequiresMouse = true;
		base.AlwaysVisible = true;
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x0000D9A5 File Offset: 0x0000BBA5
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("PopupsContainer", null);
		this.UpdateVisibility();
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x0003C294 File Offset: 0x0003A494
	public void ShowPopup(string name, string title, object content, bool showOkButton, bool showCloseButton)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		if (this.activePopups.ContainsKey(name))
		{
			return;
		}
		TemplateContainer templateContainer = Utils.InstantiateVisualTreeAsset(this.popupAsset, Position.Absolute);
		VisualElement visualElement = templateContainer.Query("Popup", null);
		Popup popup = new Popup(templateContainer, visualElement, name, title, content, showOkButton, showCloseButton);
		this.container.Add(popup.TemplateContainer);
		popup.VisualElement.BringToFront();
		this.activePopups.Add(name, popup);
		this.UpdateVisibility();
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPopupShow", new Dictionary<string, object>
		{
			{
				"name",
				name
			}
		});
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x0003C334 File Offset: 0x0003A534
	public void HidePopup(string name)
	{
		if (!this.activePopups.ContainsKey(name))
		{
			return;
		}
		this.container.Remove(this.activePopups[name].TemplateContainer);
		this.activePopups[name].Dispose();
		this.activePopups.Remove(name);
		this.UpdateVisibility();
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPopupHide", new Dictionary<string, object>
		{
			{
				"name",
				name
			}
		});
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x0000D9C4 File Offset: 0x0000BBC4
	private void UpdateVisibility()
	{
		if (this.activePopups.Count > 0)
		{
			this.Show();
			return;
		}
		this.Hide(true);
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x0003C3B0 File Offset: 0x0003A5B0
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x0000D9F5 File Offset: 0x0000BBF5
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x0000D9FF File Offset: 0x0000BBFF
	protected internal override string __getTypeName()
	{
		return "UIPopupManager";
	}

	// Token: 0x0400061C RID: 1564
	[Header("Components")]
	[SerializeField]
	public VisualTreeAsset popupAsset;

	// Token: 0x0400061D RID: 1565
	[SerializeField]
	public VisualTreeAsset popupContentTextAsset;

	// Token: 0x0400061E RID: 1566
	[SerializeField]
	public VisualTreeAsset popupContentPasswordAsset;

	// Token: 0x0400061F RID: 1567
	private Dictionary<string, Popup> activePopups = new Dictionary<string, Popup>();
}
