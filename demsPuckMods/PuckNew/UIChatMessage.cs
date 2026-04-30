using System;
using DG.Tweening;
using UnityEngine.UIElements;

// Token: 0x02000185 RID: 389
public class UIChatMessage
{
	// Token: 0x17000111 RID: 273
	// (get) Token: 0x06000B09 RID: 2825 RVA: 0x00034C03 File Offset: 0x00032E03
	private double expiryTimestamp
	{
		get
		{
			return this.ChatMessage.Timestamp + (double)(this.ExpiryTime * 1000f);
		}
	}

	// Token: 0x17000112 RID: 274
	// (get) Token: 0x06000B0A RID: 2826 RVA: 0x00034C1E File Offset: 0x00032E1E
	private float expiresInTime
	{
		get
		{
			return (float)(this.expiryTimestamp - Utils.GetTimestamp()) / 1000f;
		}
	}

	// Token: 0x17000113 RID: 275
	// (get) Token: 0x06000B0B RID: 2827 RVA: 0x00034C33 File Offset: 0x00032E33
	private bool isExpired
	{
		get
		{
			return Utils.GetTimestamp() > this.expiryTimestamp;
		}
	}

	// Token: 0x06000B0C RID: 2828 RVA: 0x00034C44 File Offset: 0x00032E44
	public UIChatMessage(ChatMessage chatMessage, VisualElement visualElement, float expiryTime = 5f)
	{
		this.ChatMessage = chatMessage;
		this.VisualElement = visualElement;
		this.ExpiryTime = expiryTime;
		this.label = this.VisualElement.Query(null, null);
		this.Focus();
		this.StartExpiryTween();
	}

	// Token: 0x06000B0D RID: 2829 RVA: 0x00034C90 File Offset: 0x00032E90
	public void Focus()
	{
		Tween tween = this.blurTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.label.EnableInClassList("blurred", false);
	}

	// Token: 0x06000B0E RID: 2830 RVA: 0x00034CB5 File Offset: 0x00032EB5
	public void Blur()
	{
		if (!this.isExpired)
		{
			this.StartExpiryTween();
			return;
		}
		Tween tween = this.blurTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.label.EnableInClassList("blurred", true);
	}

	// Token: 0x06000B0F RID: 2831 RVA: 0x00034CE9 File Offset: 0x00032EE9
	public void Dispose()
	{
		Tween tween = this.blurTween;
		if (tween == null)
		{
			return;
		}
		tween.Kill(false);
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x00034CFC File Offset: 0x00032EFC
	private void StartExpiryTween()
	{
		Tween tween = this.blurTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.blurTween = DOVirtual.DelayedCall(this.expiresInTime, delegate
		{
			this.Blur();
		}, true);
	}

	// Token: 0x04000692 RID: 1682
	public ChatMessage ChatMessage;

	// Token: 0x04000693 RID: 1683
	public VisualElement VisualElement;

	// Token: 0x04000694 RID: 1684
	public float ExpiryTime;

	// Token: 0x04000695 RID: 1685
	private Label label;

	// Token: 0x04000696 RID: 1686
	private Tween blurTween;
}
