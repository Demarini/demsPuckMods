using System;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000161 RID: 353
[UxmlElement]
public class Mmr : VisualElement
{
	// Token: 0x17000105 RID: 261
	// (get) Token: 0x06000A5B RID: 2651 RVA: 0x0003174A File Offset: 0x0002F94A
	// (set) Token: 0x06000A5C RID: 2652 RVA: 0x00031754 File Offset: 0x0002F954
	public int? CurrentValue
	{
		get
		{
			return this.currentValue;
		}
		set
		{
			int? num = this.currentValue;
			int? num2 = value;
			if (num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null))
			{
				return;
			}
			this.currentValue = value;
			this.Update();
		}
	}

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x06000A5D RID: 2653 RVA: 0x0003179B File Offset: 0x0002F99B
	// (set) Token: 0x06000A5E RID: 2654 RVA: 0x000317A4 File Offset: 0x0002F9A4
	public int? TargetValue
	{
		get
		{
			return this.targetValue;
		}
		set
		{
			int? num = this.targetValue;
			int? num2 = value;
			if (num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null))
			{
				return;
			}
			this.targetValue = value;
			this.StartCurrentValueTransition();
		}
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x000317EB File Offset: 0x0002F9EB
	public Mmr()
	{
		base.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.TrickleDown);
	}

	// Token: 0x06000A60 RID: 2656 RVA: 0x00031806 File Offset: 0x0002FA06
	private void OnAttachToPanel(AttachToPanelEvent e)
	{
		this.valueLabel = this.Query("ValueLabel", null);
		this.remainderLabel = this.Query("RemainderLabel", null);
		this.Update();
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x0003183C File Offset: 0x0002FA3C
	private static Color HexColor(string hex)
	{
		Color result;
		ColorUtility.TryParseHtmlString(hex, out result);
		return result;
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x00031854 File Offset: 0x0002FA54
	private void StartCurrentValueTransition()
	{
		if (this.TargetValue == null)
		{
			return;
		}
		if (this.CurrentValue == null)
		{
			this.CurrentValue = this.TargetValue;
			return;
		}
		Tween tween = this.transitionTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.transitionTween = DOVirtual.Int(this.CurrentValue.Value, this.TargetValue.Value, 3f, delegate(int value)
		{
			this.CurrentValue = new int?(value);
		}).SetEase(Ease.OutCubic);
	}

	// Token: 0x06000A63 RID: 2659 RVA: 0x000318E0 File Offset: 0x0002FAE0
	private void Update()
	{
		if (this.CurrentValue == null)
		{
			base.style.display = DisplayStyle.None;
			return;
		}
		base.style.display = DisplayStyle.Flex;
		IStyle style = base.style;
		int? num = this.CurrentValue;
		Color v;
		if (num != null)
		{
			int valueOrDefault = num.GetValueOrDefault();
			if (valueOrDefault < 1500)
			{
				if (valueOrDefault < 1000)
				{
					v = Mmr.HexColor("#444751");
					goto IL_E3;
				}
				if (valueOrDefault >= 1250)
				{
					v = Mmr.HexColor("#112061");
					goto IL_E3;
				}
				v = Mmr.HexColor("#2b425a");
				goto IL_E3;
			}
			else if (valueOrDefault < 2000)
			{
				if (valueOrDefault >= 1750)
				{
					v = Mmr.HexColor("#5a005e");
					goto IL_E3;
				}
				v = Mmr.HexColor("#481a60");
				goto IL_E3;
			}
			else if (valueOrDefault < 2250)
			{
				v = Mmr.HexColor("#610809");
				goto IL_E3;
			}
		}
		v = Mmr.HexColor("#625000");
		IL_E3:
		style.unityBackgroundImageTintColor = v;
		if (this.valueLabel != null)
		{
			this.valueLabel.text = this.CurrentValue.ToString();
			IStyle style2 = this.valueLabel.style;
			num = this.CurrentValue;
			if (num != null)
			{
				int valueOrDefault = num.GetValueOrDefault();
				if (valueOrDefault < 1500)
				{
					if (valueOrDefault < 1000)
					{
						v = Mmr.HexColor("#cbd2e2");
						goto IL_1CD;
					}
					if (valueOrDefault >= 1250)
					{
						v = Mmr.HexColor("#4364ff");
						goto IL_1CD;
					}
					v = Mmr.HexColor("#81b6eb");
					goto IL_1CD;
				}
				else if (valueOrDefault < 2000)
				{
					if (valueOrDefault >= 1750)
					{
						v = Mmr.HexColor("#ea02fb");
						goto IL_1CD;
					}
					v = Mmr.HexColor("#c16bff");
					goto IL_1CD;
				}
				else if (valueOrDefault < 2250)
				{
					v = Mmr.HexColor("#f93034");
					goto IL_1CD;
				}
			}
			v = Mmr.HexColor("#f5dc0c");
			IL_1CD:
			style2.color = v;
		}
		if (this.remainderLabel != null)
		{
			num = this.TargetValue;
			int? num2 = this.CurrentValue;
			bool flag = num.GetValueOrDefault() > num2.GetValueOrDefault() & (num != null & num2 != null);
			num2 = this.CurrentValue;
			num = this.TargetValue;
			bool flag2 = !(num2.GetValueOrDefault() == num.GetValueOrDefault() & num2 != null == (num != null));
			this.remainderLabel.style.display = (flag2 ? DisplayStyle.Flex : DisplayStyle.None);
			this.remainderLabel.text = (flag ? string.Format("+{0}", this.TargetValue - this.CurrentValue) : string.Format("{0}", this.TargetValue - this.CurrentValue));
		}
	}

	// Token: 0x040005F4 RID: 1524
	private int? currentValue;

	// Token: 0x040005F5 RID: 1525
	private int? targetValue;

	// Token: 0x040005F6 RID: 1526
	private Tween transitionTween;

	// Token: 0x040005F7 RID: 1527
	private Label valueLabel;

	// Token: 0x040005F8 RID: 1528
	private Label remainderLabel;

	// Token: 0x02000162 RID: 354
	[CompilerGenerated]
	[Serializable]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		// Token: 0x06000A65 RID: 2661 RVA: 0x00031C0E File Offset: 0x0002FE0E
		public override object CreateInstance()
		{
			return new Mmr();
		}
	}
}
