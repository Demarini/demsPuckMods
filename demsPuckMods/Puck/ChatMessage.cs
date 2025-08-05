using System;
using DG.Tweening;
using UnityEngine.UIElements;

// Token: 0x020000FF RID: 255
public class ChatMessage
{
	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x060008CC RID: 2252 RVA: 0x0000C692 File Offset: 0x0000A892
	public float RemainingFadeTime
	{
		get
		{
			return this.CreateTime + 15f - this.Time;
		}
	}

	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x060008CD RID: 2253 RVA: 0x0000C6A7 File Offset: 0x0000A8A7
	public bool IsNew
	{
		get
		{
			return this.RemainingFadeTime > 0f;
		}
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x00036EF8 File Offset: 0x000350F8
	public ChatMessage(Label messageLabel, float createTime, string message)
	{
		this.MessageLabel = messageLabel;
		this.Message = message;
		this.Time = createTime;
		this.CreateTime = createTime;
		this.Timestamp = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
		messageLabel.text = message;
		messageLabel.style.opacity = 0f;
		this.Show(0f, true);
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x00036F78 File Offset: 0x00035178
	public void Show(float delay = 0f, bool autoHide = true)
	{
		if (this.IsVisible)
		{
			return;
		}
		this.IsVisible = true;
		Tween tween = this.showTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		Tween tween2 = this.hideTween;
		if (tween2 != null)
		{
			tween2.Kill(false);
		}
		this.showTween = DOVirtual.DelayedCall(delay, delegate
		{
			this.IsReady = true;
			this.MessageLabel.style.opacity = 1f;
			if (autoHide)
			{
				this.Hide();
			}
		}, true);
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x00036FE8 File Offset: 0x000351E8
	public void Hide()
	{
		if (!this.IsVisible)
		{
			return;
		}
		if (!this.IsReady)
		{
			return;
		}
		this.IsVisible = false;
		Tween tween = this.showTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		Tween tween2 = this.hideTween;
		if (tween2 != null)
		{
			tween2.Kill(false);
		}
		this.hideTween = DOVirtual.DelayedCall(this.IsNew ? this.RemainingFadeTime : 0f, delegate
		{
			this.MessageLabel.style.opacity = 0f;
		}, true);
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x0000C6B6 File Offset: 0x0000A8B6
	public void Update(float time)
	{
		this.Time = time;
	}

	// Token: 0x060008D2 RID: 2258 RVA: 0x0000C6BF File Offset: 0x0000A8BF
	public void Dispose()
	{
		Tween tween = this.showTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		Tween tween2 = this.hideTween;
		if (tween2 == null)
		{
			return;
		}
		tween2.Kill(false);
	}

	// Token: 0x04000561 RID: 1377
	public Label MessageLabel;

	// Token: 0x04000562 RID: 1378
	public string Message;

	// Token: 0x04000563 RID: 1379
	public float Time;

	// Token: 0x04000564 RID: 1380
	public float CreateTime;

	// Token: 0x04000565 RID: 1381
	public double Timestamp;

	// Token: 0x04000566 RID: 1382
	public bool IsVisible;

	// Token: 0x04000567 RID: 1383
	public const float FadeOutTime = 15f;

	// Token: 0x04000568 RID: 1384
	public bool IsReady;

	// Token: 0x04000569 RID: 1385
	private Tween showTween;

	// Token: 0x0400056A RID: 1386
	private Tween hideTween;
}
