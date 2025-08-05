using System;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000143 RID: 323
public class UIComponent<T> : NetworkBehaviourSingleton<T>, UIComponent where T : NetworkBehaviourSingleton<T>
{
	// Token: 0x14000005 RID: 5
	// (add) Token: 0x06000B7D RID: 2941 RVA: 0x00041274 File Offset: 0x0003F474
	// (remove) Token: 0x06000B7E RID: 2942 RVA: 0x000412AC File Offset: 0x0003F4AC
	[HideInInspector]
	public event EventHandler OnVisibilityChanged;

	// Token: 0x14000006 RID: 6
	// (add) Token: 0x06000B7F RID: 2943 RVA: 0x000412E4 File Offset: 0x0003F4E4
	// (remove) Token: 0x06000B80 RID: 2944 RVA: 0x0004131C File Offset: 0x0003F51C
	[HideInInspector]
	public event EventHandler OnFocusChanged;

	// Token: 0x170000EC RID: 236
	// (get) Token: 0x06000B81 RID: 2945 RVA: 0x0000EA1C File Offset: 0x0000CC1C
	// (set) Token: 0x06000B82 RID: 2946 RVA: 0x0000EA24 File Offset: 0x0000CC24
	[HideInInspector]
	public bool FocusRequiresMouse { get; set; }

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x06000B83 RID: 2947 RVA: 0x0000EA2D File Offset: 0x0000CC2D
	// (set) Token: 0x06000B84 RID: 2948 RVA: 0x0000EA35 File Offset: 0x0000CC35
	[HideInInspector]
	public bool VisibilityRequiresMouse { get; set; }

	// Token: 0x170000EE RID: 238
	// (get) Token: 0x06000B85 RID: 2949 RVA: 0x0000EA3E File Offset: 0x0000CC3E
	// (set) Token: 0x06000B86 RID: 2950 RVA: 0x0000EA46 File Offset: 0x0000CC46
	[HideInInspector]
	public bool AlwaysVisible { get; set; }

	// Token: 0x170000EF RID: 239
	// (get) Token: 0x06000B87 RID: 2951 RVA: 0x0000EA4F File Offset: 0x0000CC4F
	public bool IsVisible
	{
		get
		{
			return this.container != null && this.container.style.display == DisplayStyle.Flex;
		}
	}

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x06000B88 RID: 2952 RVA: 0x0000EA76 File Offset: 0x0000CC76
	// (set) Token: 0x06000B89 RID: 2953 RVA: 0x0000EA7E File Offset: 0x0000CC7E
	public bool IsFocused
	{
		get
		{
			return this.isFocused;
		}
		set
		{
			this.isFocused = value;
			EventHandler onFocusChanged = this.OnFocusChanged;
			if (onFocusChanged == null)
			{
				return;
			}
			onFocusChanged(this, EventArgs.Empty);
		}
	}

	// Token: 0x06000B8A RID: 2954 RVA: 0x0000EA9D File Offset: 0x0000CC9D
	public virtual void Show()
	{
		if (Application.isBatchMode)
		{
			return;
		}
		if (this.container == null)
		{
			return;
		}
		this.container.style.display = DisplayStyle.Flex;
		EventHandler onVisibilityChanged = this.OnVisibilityChanged;
		if (onVisibilityChanged == null)
		{
			return;
		}
		onVisibilityChanged(this, EventArgs.Empty);
	}

	// Token: 0x06000B8B RID: 2955 RVA: 0x00041354 File Offset: 0x0003F554
	public virtual void Hide(bool ignoreAlwaysVisible = false)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		if (this.container == null)
		{
			return;
		}
		if (!ignoreAlwaysVisible && this.AlwaysVisible)
		{
			return;
		}
		this.container.style.display = DisplayStyle.None;
		EventHandler onVisibilityChanged = this.OnVisibilityChanged;
		if (onVisibilityChanged == null)
		{
			return;
		}
		onVisibilityChanged(this, EventArgs.Empty);
	}

	// Token: 0x06000B8C RID: 2956 RVA: 0x0000EADC File Offset: 0x0000CCDC
	public virtual void Toggle()
	{
		if (Application.isBatchMode)
		{
			return;
		}
		if (this.IsVisible)
		{
			this.Hide(false);
			return;
		}
		this.Show();
	}

	// Token: 0x06000B8E RID: 2958 RVA: 0x000413AC File Offset: 0x0003F5AC
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x0000EB04 File Offset: 0x0000CD04
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000B90 RID: 2960 RVA: 0x0000EB0E File Offset: 0x0000CD0E
	protected internal override string __getTypeName()
	{
		return "UIComponent`1";
	}

	// Token: 0x040006B8 RID: 1720
	private bool isFocused;

	// Token: 0x040006B9 RID: 1721
	protected VisualElement rootVisualElement;

	// Token: 0x040006BA RID: 1722
	protected VisualElement container;
}
