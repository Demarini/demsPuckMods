using System;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x0200016A RID: 362
public class UIView : MonoBehaviour
{
	// Token: 0x1700010D RID: 269
	// (get) Token: 0x06000A89 RID: 2697 RVA: 0x000320D5 File Offset: 0x000302D5
	// (set) Token: 0x06000A8A RID: 2698 RVA: 0x000320E0 File Offset: 0x000302E0
	public bool IsVisible
	{
		get
		{
			return this.isVisible;
		}
		set
		{
			if (this.isVisible == value)
			{
				return;
			}
			bool oldIsVisible = this.isVisible;
			this.isVisible = value;
			this.OnIsVisibileChanged(oldIsVisible, this.isVisible);
		}
	}

	// Token: 0x1700010E RID: 270
	// (get) Token: 0x06000A8B RID: 2699 RVA: 0x00032112 File Offset: 0x00030312
	// (set) Token: 0x06000A8C RID: 2700 RVA: 0x0003211C File Offset: 0x0003031C
	public bool IsFocused
	{
		get
		{
			return this.isFocused;
		}
		set
		{
			if (this.isFocused == value)
			{
				return;
			}
			bool oldIsFocused = this.isFocused;
			this.isFocused = value;
			this.OnIsFocusedChanged(oldIsFocused, this.isFocused);
		}
	}

	// Token: 0x1700010F RID: 271
	// (get) Token: 0x06000A8D RID: 2701 RVA: 0x00032150 File Offset: 0x00030350
	[HideInInspector]
	public int Order
	{
		get
		{
			if (!(this.View.parent is TemplateContainer))
			{
				return this.View.parent.IndexOf(this.View);
			}
			return this.View.parent.parent.IndexOf(this.View.parent);
		}
	}

	// Token: 0x17000110 RID: 272
	// (get) Token: 0x06000A8E RID: 2702 RVA: 0x000321A6 File Offset: 0x000303A6
	// (set) Token: 0x06000A8F RID: 2703 RVA: 0x000321B0 File Offset: 0x000303B0
	public VisualElement View
	{
		get
		{
			return this.view;
		}
		set
		{
			if (this.view == value)
			{
				return;
			}
			VisualElement oldView = this.view;
			this.view = value;
			this.OnViewChanged(oldView, this.view);
		}
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x000321E2 File Offset: 0x000303E2
	public virtual bool Show()
	{
		if (this.IsVisible)
		{
			return false;
		}
		this.IsVisible = true;
		return true;
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x000321F6 File Offset: 0x000303F6
	public virtual bool Hide()
	{
		if (!this.IsVisible || this.AlwaysVisible)
		{
			return false;
		}
		this.IsVisible = false;
		return true;
	}

	// Token: 0x06000A92 RID: 2706 RVA: 0x00032212 File Offset: 0x00030412
	public virtual bool Toggle()
	{
		if (this.IsVisible)
		{
			return this.Hide();
		}
		return this.Show();
	}

	// Token: 0x06000A93 RID: 2707 RVA: 0x00032229 File Offset: 0x00030429
	private void OnIsVisibileChanged(bool oldIsVisible, bool newIsVisible)
	{
		this.View.style.display = (newIsVisible ? DisplayStyle.Flex : DisplayStyle.None);
		Action<UIView> onVisibility = this.OnVisibility;
		if (onVisibility == null)
		{
			return;
		}
		onVisibility(this);
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x00032258 File Offset: 0x00030458
	private void OnIsFocusedChanged(bool oldIsFocused, bool newIsFocused)
	{
		Action<UIView> onFocus = this.OnFocus;
		if (onFocus == null)
		{
			return;
		}
		onFocus(this);
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x000020D3 File Offset: 0x000002D3
	private void OnViewChanged(VisualElement oldView, VisualElement newView)
	{
	}

	// Token: 0x04000613 RID: 1555
	[Header("UI Settings")]
	public bool FocusRequiresMouse;

	// Token: 0x04000614 RID: 1556
	public bool FocusIsInteractive;

	// Token: 0x04000615 RID: 1557
	public bool VisibilityRequiresMouse;

	// Token: 0x04000616 RID: 1558
	public bool VisibilityIsInteractive;

	// Token: 0x04000617 RID: 1559
	public bool AlwaysVisible;

	// Token: 0x04000618 RID: 1560
	private bool isVisible = true;

	// Token: 0x04000619 RID: 1561
	private bool isFocused;

	// Token: 0x0400061A RID: 1562
	[HideInInspector]
	public Action<UIView> OnVisibility;

	// Token: 0x0400061B RID: 1563
	[HideInInspector]
	public Action<UIView> OnFocus;

	// Token: 0x0400061C RID: 1564
	[HideInInspector]
	public VisualElement RootVisualElement;

	// Token: 0x0400061D RID: 1565
	private VisualElement view;
}
