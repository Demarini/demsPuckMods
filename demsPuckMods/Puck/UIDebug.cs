using System;
using UnityEngine.UIElements;

// Token: 0x02000108 RID: 264
public class UIDebug : UIComponent<UIDebug>
{
	// Token: 0x0600093C RID: 2364 RVA: 0x0000CC24 File Offset: 0x0000AE24
	public override void Awake()
	{
		base.Awake();
		base.AlwaysVisible = true;
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x0000CC33 File Offset: 0x0000AE33
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("DebugContainer", null);
		this.buildLabel = this.container.Query("BuildLabel", null);
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x0000CC68 File Offset: 0x0000AE68
	public void SetBuildLabelText(string text)
	{
		this.buildLabel.text = text;
	}

	// Token: 0x06000940 RID: 2368 RVA: 0x0003904C File Offset: 0x0003724C
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000941 RID: 2369 RVA: 0x0000CC7E File Offset: 0x0000AE7E
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000942 RID: 2370 RVA: 0x0000CC88 File Offset: 0x0000AE88
	protected internal override string __getTypeName()
	{
		return "UIDebug";
	}

	// Token: 0x040005A0 RID: 1440
	private Label buildLabel;
}
