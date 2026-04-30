using System;
using System.Globalization;
using UnityEngine.UIElements;

// Token: 0x02000197 RID: 407
public class UIHUD : UIView
{
	// Token: 0x06000B8F RID: 2959 RVA: 0x00036A38 File Offset: 0x00034C38
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("HUDView", null);
		this.staminaProgressBar = base.View.Query("StaminaProgressBar", null);
		this.speed = base.View.Query("Speed", null);
		this.speedLabel = this.speed.Query("SpeedLabel", null);
		this.unitsLabel = this.speed.Query("UnitsLabel", null);
	}

	// Token: 0x06000B90 RID: 2960 RVA: 0x00036ACC File Offset: 0x00034CCC
	public void SetStamina(float value)
	{
		this.staminaProgressBar.EnableInClassList("warning", value < 0.25f);
		this.staminaProgressBar.value = value;
	}

	// Token: 0x06000B91 RID: 2961 RVA: 0x00036AF4 File Offset: 0x00034CF4
	public void SetSpeed(float value)
	{
		float num = (float)Math.Round((double)((SettingsManager.Units == Units.Metric) ? Utils.GameUnitsToMetric(value) : Utils.GameUnitsToImperial(value)), 1);
		this.speedLabel.text = num.ToString("F1", CultureInfo.InvariantCulture);
	}

	// Token: 0x06000B92 RID: 2962 RVA: 0x00036B3B File Offset: 0x00034D3B
	public void SetUnits(string units)
	{
		this.unitsLabel.text = units;
	}

	// Token: 0x040006D7 RID: 1751
	private ProgressBar staminaProgressBar;

	// Token: 0x040006D8 RID: 1752
	private VisualElement speed;

	// Token: 0x040006D9 RID: 1753
	private Label speedLabel;

	// Token: 0x040006DA RID: 1754
	private Label unitsLabel;
}
