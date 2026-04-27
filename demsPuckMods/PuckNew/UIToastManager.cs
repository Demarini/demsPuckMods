using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x020001D5 RID: 469
public class UIToastManager : UIView
{
	// Token: 0x06000DC7 RID: 3527 RVA: 0x00041385 File Offset: 0x0003F585
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("ToastsView", null);
		this.toasts = base.View.Query("Toasts", null);
		this.toasts.Clear();
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x000413C8 File Offset: 0x0003F5C8
	public void ShowToast(string name, string content, float hideDelay = 3f)
	{
		if (this.nameToastMap.ContainsKey(name))
		{
			this.HideToast(name);
		}
		VisualElement visualElement = this.toastAsset.Instantiate();
		Toast toast = new Toast(this, visualElement, name, content.ToUpper(), hideDelay);
		this.toasts.Add(toast.VisualElement);
		this.nameToastMap.Add(name, toast);
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x00041424 File Offset: 0x0003F624
	public void HideToast(string name)
	{
		if (!this.nameToastMap.ContainsKey(name))
		{
			return;
		}
		this.toasts.Remove(this.nameToastMap[name].VisualElement);
		this.nameToastMap[name].Dispose();
		this.nameToastMap.Remove(name);
	}

	// Token: 0x04000817 RID: 2071
	[Header("References")]
	[SerializeField]
	public VisualTreeAsset toastAsset;

	// Token: 0x04000818 RID: 2072
	private Dictionary<string, Toast> nameToastMap = new Dictionary<string, Toast>();

	// Token: 0x04000819 RID: 2073
	private VisualElement toasts;
}
