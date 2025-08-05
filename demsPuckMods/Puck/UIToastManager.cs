using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000140 RID: 320
public class UIToastManager : UIComponent<UIToastManager>
{
	// Token: 0x06000B52 RID: 2898 RVA: 0x0000E95B File Offset: 0x0000CB5B
	public override void Awake()
	{
		base.Awake();
		base.AlwaysVisible = true;
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x0000E96A File Offset: 0x0000CB6A
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("ToastsContainer", null);
		this.container.Clear();
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x00040990 File Offset: 0x0003EB90
	public void ShowToast(string name, string content, float hideDelay = 3f)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		if (this.activeToasts.ContainsKey(name))
		{
			this.HideToast(name);
		}
		TemplateContainer templateContainer = Utils.InstantiateVisualTreeAsset(this.toastAsset, Position.Relative);
		VisualElement visualElement = templateContainer.Query("Toast", null);
		Toast toast = new Toast(this, templateContainer, visualElement, name, content, hideDelay);
		this.container.Add(toast.TemplateContainer);
		this.activeToasts.Add(name, toast);
	}

	// Token: 0x06000B55 RID: 2901 RVA: 0x00040A04 File Offset: 0x0003EC04
	public void HideToast(string name)
	{
		if (!this.activeToasts.ContainsKey(name))
		{
			return;
		}
		this.container.Remove(this.activeToasts[name].TemplateContainer);
		this.activeToasts[name].Dispose();
		this.activeToasts.Remove(name);
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x00040A5C File Offset: 0x0003EC5C
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000B58 RID: 2904 RVA: 0x0000E9A1 File Offset: 0x0000CBA1
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x0000E9AB File Offset: 0x0000CBAB
	protected internal override string __getTypeName()
	{
		return "UIToastManager";
	}

	// Token: 0x040006B0 RID: 1712
	[Header("Components")]
	[SerializeField]
	public VisualTreeAsset toastAsset;

	// Token: 0x040006B1 RID: 1713
	private Dictionary<string, Toast> activeToasts = new Dictionary<string, Toast>();
}
