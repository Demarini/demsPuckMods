using System;
using UnityEngine.UIElements;

// Token: 0x02000195 RID: 405
public class UIGameState : UIView
{
	// Token: 0x06000B84 RID: 2948 RVA: 0x00036770 File Offset: 0x00034970
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("GameStateView", null);
		this.gameState = base.View.Query("GameState", null);
		this.blueScoreLabel = this.gameState.Query("BlueScoreLabel", null);
		this.redScoreLabel = this.gameState.Query("RedScoreLabel", null);
		this.timeLabel = this.gameState.Query("TimeLabel", null);
		this.phaseLabel = this.gameState.Query("PhaseLabel", null);
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x0003233E File Offset: 0x0003053E
	public override bool Show()
	{
		return SettingsManager.ShowGameUserInterface && base.Show();
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x00036820 File Offset: 0x00034A20
	public void SetScore(PlayerTeam team, int score)
	{
		if (team == PlayerTeam.Blue)
		{
			this.blueScoreLabel.text = string.Format("{0}", score);
			return;
		}
		if (team == PlayerTeam.Red)
		{
			this.redScoreLabel.text = string.Format("{0}", score);
		}
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x0003686C File Offset: 0x00034A6C
	public void SetTick(int tick)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)tick);
		if (timeSpan.TotalHours < 1.0)
		{
			this.timeLabel.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
			return;
		}
		this.timeLabel.text = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
	}

	// Token: 0x06000B88 RID: 2952 RVA: 0x000368FB File Offset: 0x00034AFB
	public void SetPhase(string text)
	{
		this.phaseLabel.text = text;
	}

	// Token: 0x040006D1 RID: 1745
	private VisualElement gameState;

	// Token: 0x040006D2 RID: 1746
	private Label blueScoreLabel;

	// Token: 0x040006D3 RID: 1747
	private Label redScoreLabel;

	// Token: 0x040006D4 RID: 1748
	private Label timeLabel;

	// Token: 0x040006D5 RID: 1749
	private Label phaseLabel;
}
