using System;

// Token: 0x02000142 RID: 322
public interface UIComponent
{
	// Token: 0x14000003 RID: 3
	// (add) Token: 0x06000B6D RID: 2925
	// (remove) Token: 0x06000B6E RID: 2926
	event EventHandler OnVisibilityChanged;

	// Token: 0x14000004 RID: 4
	// (add) Token: 0x06000B6F RID: 2927
	// (remove) Token: 0x06000B70 RID: 2928
	event EventHandler OnFocusChanged;

	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x06000B71 RID: 2929
	// (set) Token: 0x06000B72 RID: 2930
	bool FocusRequiresMouse { get; set; }

	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x06000B73 RID: 2931
	// (set) Token: 0x06000B74 RID: 2932
	bool VisibilityRequiresMouse { get; set; }

	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x06000B75 RID: 2933
	// (set) Token: 0x06000B76 RID: 2934
	bool AlwaysVisible { get; set; }

	// Token: 0x170000EA RID: 234
	// (get) Token: 0x06000B77 RID: 2935
	bool IsVisible { get; }

	// Token: 0x170000EB RID: 235
	// (get) Token: 0x06000B78 RID: 2936
	// (set) Token: 0x06000B79 RID: 2937
	bool IsFocused { get; set; }

	// Token: 0x06000B7A RID: 2938
	void Show();

	// Token: 0x06000B7B RID: 2939
	void Hide(bool ignoreAlwaysVisible = false);

	// Token: 0x06000B7C RID: 2940
	void Toggle();
}
