using System;
using UnityEngine.UIElements;

// Token: 0x0200018C RID: 396
public class UIDebug : UIView
{
	// Token: 0x06000B4B RID: 2891 RVA: 0x00035AD6 File Offset: 0x00033CD6
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("DebugView", null);
		this.buildLabel = base.View.Query("BuildLabel", null);
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x00035B0B File Offset: 0x00033D0B
	public override bool Show()
	{
		return SettingsManager.Debug && base.Show();
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x00035B21 File Offset: 0x00033D21
	public override bool Hide()
	{
		return !SettingsManager.Debug && base.Hide();
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x00035B37 File Offset: 0x00033D37
	public void SetBuild(string text)
	{
		this.buildLabel.text = text;
	}

	// Token: 0x040006B0 RID: 1712
	private Label buildLabel;
}
