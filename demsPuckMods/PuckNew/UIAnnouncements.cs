using System;
using DG.Tweening;
using UnityEngine.UIElements;

// Token: 0x0200016C RID: 364
public class UIAnnouncements : UIView
{
	// Token: 0x06000A9A RID: 2714 RVA: 0x00032288 File Offset: 0x00030488
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("AnnouncementsView", null);
		this.announcements = base.View.Query("Announcements", null);
		this.score = this.announcements.Query("Score", null);
		this.headerLabel = this.score.Query("HeaderLabel", null);
		this.goalLabel = this.score.Query("GoalLabel", null);
		this.assistLabel = this.score.Query("AssistLabel", null);
		this.HideScore();
	}

	// Token: 0x06000A9B RID: 2715 RVA: 0x0003233E File Offset: 0x0003053E
	public override bool Show()
	{
		return SettingsManager.ShowGameUserInterface && base.Show();
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x00032350 File Offset: 0x00030550
	public void ShowScore(PlayerTeam team, Player goalPlayer, Player assistPlayer, Player secondAssistPlayer)
	{
		this.headerLabel.text = string.Empty;
		this.goalLabel.text = string.Empty;
		this.assistLabel.text = string.Empty;
		this.score.style.display = DisplayStyle.Flex;
		Tween tween = this.autoHideTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.autoHideTween = DOVirtual.DelayedCall(5f, delegate
		{
			this.HideScore();
		}, true);
		UIUtils.SetTeamClass(this.score, team);
		if (team != PlayerTeam.Blue)
		{
			if (team == PlayerTeam.Red)
			{
				this.headerLabel.text = "RED SCORES!";
			}
		}
		else
		{
			this.headerLabel.text = "BLUE SCORES!";
		}
		if (goalPlayer)
		{
			this.goalLabel.text = string.Format("#{0} {1}", goalPlayer.Number.Value, goalPlayer.Username.Value);
		}
		if (assistPlayer)
		{
			this.assistLabel.text = string.Format("#{0} {1}", assistPlayer.Number.Value, assistPlayer.Username.Value);
		}
		if (secondAssistPlayer)
		{
			Label label = this.assistLabel;
			label.text += string.Format(" & #{0} {1}", secondAssistPlayer.Number.Value, secondAssistPlayer.Username.Value);
		}
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x000324D0 File Offset: 0x000306D0
	public void HideScore()
	{
		this.headerLabel.text = string.Empty;
		this.goalLabel.text = string.Empty;
		this.assistLabel.text = string.Empty;
		this.score.style.display = DisplayStyle.None;
		Tween tween = this.autoHideTween;
		if (tween == null)
		{
			return;
		}
		tween.Kill(false);
	}

	// Token: 0x0400061F RID: 1567
	private VisualElement announcements;

	// Token: 0x04000620 RID: 1568
	private VisualElement score;

	// Token: 0x04000621 RID: 1569
	private Label headerLabel;

	// Token: 0x04000622 RID: 1570
	private Label goalLabel;

	// Token: 0x04000623 RID: 1571
	private Label assistLabel;

	// Token: 0x04000624 RID: 1572
	private Tween autoHideTween;
}
