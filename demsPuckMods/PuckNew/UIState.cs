using System;
using System.Collections.Generic;

// Token: 0x020000A8 RID: 168
public struct UIState
{
	// Token: 0x17000083 RID: 131
	// (get) Token: 0x06000552 RID: 1362 RVA: 0x0001D2D6 File Offset: 0x0001B4D6
	public bool IsInteracting
	{
		get
		{
			return this.InteractingViews != null && this.InteractingViews.Count > 0;
		}
	}

	// Token: 0x06000553 RID: 1363 RVA: 0x0001D2F0 File Offset: 0x0001B4F0
	public bool IsViewInteracting<T>() where T : UIView
	{
		if (this.InteractingViews == null)
		{
			return false;
		}
		using (List<UIView>.Enumerator enumerator = this.InteractingViews.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current is !!0)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x0001D354 File Offset: 0x0001B554
	public bool IsViewTopmostInteracting<T>() where T : UIView
	{
		return this.InteractingViews != null && this.InteractingViews.Count != 0 && this.InteractingViews[this.InteractingViews.Count - 1] is !!0;
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x0001D38D File Offset: 0x0001B58D
	public UIView GetTopmostInteractingView()
	{
		if (this.InteractingViews == null || this.InteractingViews.Count == 0)
		{
			return null;
		}
		return this.InteractingViews[this.InteractingViews.Count - 1];
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x0001D3BE File Offset: 0x0001B5BE
	public bool Equals(UIState other)
	{
		return this.Phase == other.Phase && this.IsMouseRequired == other.IsMouseRequired && this.IsMouseOverUI == other.IsMouseOverUI && this.InteractingViews == other.InteractingViews;
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x0001D3FC File Offset: 0x0001B5FC
	public override bool Equals(object obj)
	{
		if (obj is UIState)
		{
			UIState other = (UIState)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000558 RID: 1368 RVA: 0x0001D421 File Offset: 0x0001B621
	public override int GetHashCode()
	{
		return HashCode.Combine<UIPhase, bool, bool, List<UIView>>(this.Phase, this.IsMouseRequired, this.IsMouseOverUI, this.InteractingViews);
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x0001D440 File Offset: 0x0001B640
	public override string ToString()
	{
		return string.Format("Phase: {0}, IsMouseRequired: {1}, IsMouseOverUI: {2}, IsInteracting: {3}", new object[]
		{
			this.Phase,
			this.IsMouseRequired,
			this.IsMouseOverUI,
			this.IsInteracting
		});
	}

	// Token: 0x04000340 RID: 832
	public UIPhase Phase;

	// Token: 0x04000341 RID: 833
	public bool IsMouseRequired;

	// Token: 0x04000342 RID: 834
	public bool IsMouseOverUI;

	// Token: 0x04000343 RID: 835
	public List<UIView> InteractingViews;
}
