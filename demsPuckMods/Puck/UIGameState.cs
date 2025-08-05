using System;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x0200010A RID: 266
public class UIGameState : UIComponent<UIGameState>
{
	// Token: 0x0600094B RID: 2379 RVA: 0x00039064 File Offset: 0x00037264
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("GameStateContainer", null);
		this.gameTimeLabel = this.container.Query("GameTime", null);
		this.gamePhaseLabel = this.container.Query("GamePhase", null);
		this.blueScoreLabel = this.container.Query("BlueScore", null);
		this.redScoreLabel = this.container.Query("RedScore", null);
	}

	// Token: 0x0600094C RID: 2380 RVA: 0x0000CD2B File Offset: 0x0000AF2B
	public override void Show()
	{
		if (MonoBehaviourSingleton<SettingsManager>.Instance.ShowGameUserInterface == 0)
		{
			return;
		}
		base.Show();
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x0000CD40 File Offset: 0x0000AF40
	public void SetBlueTeamScore(int score)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		this.blueScoreLabel.text = string.Format("{0}", score);
	}

	// Token: 0x0600094E RID: 2382 RVA: 0x0000CD65 File Offset: 0x0000AF65
	public void SetRedTeamScore(int score)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		this.redScoreLabel.text = string.Format("{0}", score);
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x000390F8 File Offset: 0x000372F8
	public void SetGameTime(float time)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)time);
		this.gameTimeLabel.text = (timeSpan.ToString("mm':'ss") ?? "");
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x0000CD8A File Offset: 0x0000AF8A
	public void SetGamePhase(string text)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		this.gamePhaseLabel.text = text;
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x00039138 File Offset: 0x00037338
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x0000CDA8 File Offset: 0x0000AFA8
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x0000CDB2 File Offset: 0x0000AFB2
	protected internal override string __getTypeName()
	{
		return "UIGameState";
	}

	// Token: 0x040005A2 RID: 1442
	private Label gameTimeLabel;

	// Token: 0x040005A3 RID: 1443
	private Label gamePhaseLabel;

	// Token: 0x040005A4 RID: 1444
	private Label blueScoreLabel;

	// Token: 0x040005A5 RID: 1445
	private Label redScoreLabel;
}
