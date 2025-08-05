using System;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x0200010C RID: 268
public class UIHUD : UIComponent<UIHUD>
{
	// Token: 0x0600095E RID: 2398 RVA: 0x000392F8 File Offset: 0x000374F8
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("PlayerContainer", null);
		this.staminaProgressBar = this.container.Query("StaminaProgressBar", null);
		this.speedLabel = this.container.Query("SpeedLabel", null);
		this.unitsLabel = this.container.Query("UnitsLabel", null);
	}

	// Token: 0x0600095F RID: 2399 RVA: 0x0000CE7D File Offset: 0x0000B07D
	public void SetStamina(float value)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		this.staminaProgressBar.value = value;
	}

	// Token: 0x06000960 RID: 2400 RVA: 0x00039370 File Offset: 0x00037570
	public void SetSpeed(float value)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		float num = (float)Math.Round((double)((MonoBehaviourSingleton<SettingsManager>.Instance.Units == "METRIC") ? Utils.GameUnitsToMetric(value) : Utils.GameUnitsToImperial(value)), 1);
		this.speedLabel.text = num.ToString("F1");
	}

	// Token: 0x06000961 RID: 2401 RVA: 0x0000CE93 File Offset: 0x0000B093
	public void SetUnits(string units)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		this.unitsLabel.text = units;
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x000393CC File Offset: 0x000375CC
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x0000CEB1 File Offset: 0x0000B0B1
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x0000CEBB File Offset: 0x0000B0BB
	protected internal override string __getTypeName()
	{
		return "UIHUD";
	}

	// Token: 0x040005A7 RID: 1447
	private ProgressBar staminaProgressBar;

	// Token: 0x040005A8 RID: 1448
	private Label speedLabel;

	// Token: 0x040005A9 RID: 1449
	private Label unitsLabel;
}
