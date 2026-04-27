using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x020001B9 RID: 441
public class UIPopupManager : UIView
{
	// Token: 0x06000CA5 RID: 3237 RVA: 0x0003B67C File Offset: 0x0003987C
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("PopupsView", null);
		this.popups = base.View.Query("Popups", null);
		this.popups.Clear();
		this.UpdateFocus();
		this.UpdateVisibility();
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x0003B6D4 File Offset: 0x000398D4
	public void ShowPopup(string name, string title, object content, bool showOkButton, bool showCloseButton)
	{
		if (this.namePopupMap.ContainsKey(name))
		{
			return;
		}
		Popup popup = new Popup(this.popupAsset.Instantiate(), name, title, content, showOkButton, showCloseButton);
		this.popups.Add(popup.VisualElement);
		popup.VisualElement.BringToFront();
		this.namePopupMap.Add(name, popup);
		this.UpdateFocus();
		this.UpdateVisibility();
		EventManager.TriggerEvent("Event_OnPopupShow", new Dictionary<string, object>
		{
			{
				"name",
				name
			}
		});
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x0003B758 File Offset: 0x00039958
	public void HidePopup(string name)
	{
		if (!this.namePopupMap.ContainsKey(name))
		{
			return;
		}
		this.popups.Remove(this.namePopupMap[name].VisualElement);
		this.namePopupMap[name].Dispose();
		this.namePopupMap.Remove(name);
		this.UpdateFocus();
		this.UpdateVisibility();
		EventManager.TriggerEvent("Event_OnPopupHide", new Dictionary<string, object>
		{
			{
				"name",
				name
			}
		});
	}

	// Token: 0x06000CA8 RID: 3240 RVA: 0x0003B7D5 File Offset: 0x000399D5
	private void UpdateFocus()
	{
		base.IsFocused = (this.namePopupMap.Count > 0);
	}

	// Token: 0x06000CA9 RID: 3241 RVA: 0x0003B7EB File Offset: 0x000399EB
	private void UpdateVisibility()
	{
		this.popups.style.display = ((this.namePopupMap.Count > 0) ? DisplayStyle.Flex : DisplayStyle.None);
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x0003B814 File Offset: 0x00039A14
	public PopupTextContent CreateTextContent(string text, object data = null)
	{
		return new PopupTextContent(this.textContentAsset, text, data);
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x0003B823 File Offset: 0x00039A23
	public PopupPasswordContent CreatePasswordContent(object data = null)
	{
		return new PopupPasswordContent(this.passwordContentAsset, data);
	}

	// Token: 0x04000783 RID: 1923
	[Header("References")]
	[SerializeField]
	private VisualTreeAsset popupAsset;

	// Token: 0x04000784 RID: 1924
	[SerializeField]
	private VisualTreeAsset textContentAsset;

	// Token: 0x04000785 RID: 1925
	[SerializeField]
	private VisualTreeAsset passwordContentAsset;

	// Token: 0x04000786 RID: 1926
	private VisualElement popups;

	// Token: 0x04000787 RID: 1927
	private Dictionary<string, Popup> namePopupMap = new Dictionary<string, Popup>();
}
