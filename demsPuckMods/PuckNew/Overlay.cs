using System;
using DG.Tweening;
using UnityEngine.UIElements;

// Token: 0x020001A8 RID: 424
public class Overlay
{
	// Token: 0x06000C31 RID: 3121 RVA: 0x00039AFC File Offset: 0x00037CFC
	public Overlay(VisualElement visualElement, string identifier, bool requiresSpinner = false)
	{
		this.VisualElement = visualElement;
		this.Identifier = identifier;
		this.VisualElement.style.display = DisplayStyle.None;
		this.VisualElement.style.opacity = 0f;
		this.spinner = this.VisualElement.Query(null, null);
		this.spinner.style.display = (requiresSpinner ? DisplayStyle.Flex : DisplayStyle.None);
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x00039B84 File Offset: 0x00037D84
	public void Show()
	{
		Tween tween = this.fadeOutTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		if (this.fadeInTween != null)
		{
			this.fadeInTween.Kill(false);
		}
		if (this.autoHideTween != null)
		{
			this.autoHideTween.Kill(false);
		}
		Action showing = this.Showing;
		if (showing != null)
		{
			showing();
		}
		if (this.FadeIn)
		{
			this.fadeInTween = DOVirtual.Float(this.VisualElement.style.opacity.value, 1f, this.FadeTime, delegate(float value)
			{
				this.VisualElement.style.opacity = value;
			}).OnStart(delegate
			{
				this.VisualElement.style.display = DisplayStyle.Flex;
				Action shown2 = this.Shown;
				if (shown2 == null)
				{
					return;
				}
				shown2();
			});
			if (this.AutoHide)
			{
				this.autoHideTween = DOVirtual.DelayedCall(this.FadeTime + this.HideTimeout, delegate
				{
					this.Hide();
				}, true);
				return;
			}
		}
		else
		{
			this.VisualElement.style.display = DisplayStyle.Flex;
			this.VisualElement.style.opacity = 1f;
			Action shown = this.Shown;
			if (shown != null)
			{
				shown();
			}
			if (this.AutoHide)
			{
				this.autoHideTween = DOVirtual.DelayedCall(this.HideTimeout, delegate
				{
					this.Hide();
				}, true);
			}
		}
	}

	// Token: 0x06000C33 RID: 3123 RVA: 0x00039CC8 File Offset: 0x00037EC8
	public void Hide()
	{
		Tween tween = this.fadeInTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		if (this.fadeOutTween != null)
		{
			this.fadeOutTween.Kill(false);
		}
		if (this.autoHideTween != null)
		{
			this.autoHideTween.Kill(false);
		}
		Action hiding = this.Hiding;
		if (hiding != null)
		{
			hiding();
		}
		if (this.FadeOut)
		{
			this.fadeOutTween = DOVirtual.Float(this.VisualElement.style.opacity.value, 0f, this.FadeTime, delegate(float value)
			{
				this.VisualElement.style.opacity = value;
			}).OnComplete(delegate
			{
				this.VisualElement.style.display = DisplayStyle.None;
				Action hidden2 = this.Hidden;
				if (hidden2 == null)
				{
					return;
				}
				hidden2();
			});
			return;
		}
		this.VisualElement.style.display = DisplayStyle.None;
		this.VisualElement.style.opacity = 0f;
		Action hidden = this.Hidden;
		if (hidden == null)
		{
			return;
		}
		hidden();
	}

	// Token: 0x06000C34 RID: 3124 RVA: 0x00039DB4 File Offset: 0x00037FB4
	public void Dispose()
	{
		Tween tween = this.fadeInTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		Tween tween2 = this.fadeOutTween;
		if (tween2 != null)
		{
			tween2.Kill(false);
		}
		Tween tween3 = this.autoHideTween;
		if (tween3 == null)
		{
			return;
		}
		tween3.Kill(false);
	}

	// Token: 0x04000736 RID: 1846
	public VisualElement VisualElement;

	// Token: 0x04000737 RID: 1847
	public string Identifier;

	// Token: 0x04000738 RID: 1848
	public bool RequiresSpinner;

	// Token: 0x04000739 RID: 1849
	public bool FadeIn;

	// Token: 0x0400073A RID: 1850
	public bool FadeOut;

	// Token: 0x0400073B RID: 1851
	public float FadeTime;

	// Token: 0x0400073C RID: 1852
	public bool AutoHide;

	// Token: 0x0400073D RID: 1853
	public float HideTimeout;

	// Token: 0x0400073E RID: 1854
	public Action Showing;

	// Token: 0x0400073F RID: 1855
	public Action Shown;

	// Token: 0x04000740 RID: 1856
	public Action Hiding;

	// Token: 0x04000741 RID: 1857
	public Action Hidden;

	// Token: 0x04000742 RID: 1858
	private Tween fadeInTween;

	// Token: 0x04000743 RID: 1859
	private Tween fadeOutTween;

	// Token: 0x04000744 RID: 1860
	private Tween autoHideTween;

	// Token: 0x04000745 RID: 1861
	private Spinner spinner;
}
