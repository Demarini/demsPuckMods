using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x020000F8 RID: 248
public class UIAnnouncement : UIComponent<UIAnnouncement>
{
	// Token: 0x06000885 RID: 2181 RVA: 0x0003548C File Offset: 0x0003368C
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("AnnouncementContainer", null);
		this.blueTeamScoreAnnouncement = this.container.Query("BlueTeamScoreAnnouncement", null);
		this.blueTeamScorePlayersLabel = this.blueTeamScoreAnnouncement.Query("PlayersLabel", null);
		this.redTeamScoreAnnouncement = this.container.Query("RedTeamScoreAnnouncement", null);
		this.redTeamScorePlayersLabel = this.redTeamScoreAnnouncement.Query("PlayersLabel", null);
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x0000C387 File Offset: 0x0000A587
	public override void Show()
	{
		if (MonoBehaviourSingleton<SettingsManager>.Instance.ShowGameUserInterface == 0)
		{
			return;
		}
		base.Show();
	}

	// Token: 0x06000887 RID: 2183 RVA: 0x0000C39C File Offset: 0x0000A59C
	public override void Hide(bool ignoreAlwaysVisible = false)
	{
		base.Hide(ignoreAlwaysVisible);
		this.blueTeamScoreAnnouncement.style.visibility = Visibility.Hidden;
		this.redTeamScoreAnnouncement.style.visibility = Visibility.Hidden;
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x0000C3D1 File Offset: 0x0000A5D1
	public void ShowBlueTeamScoreAnnouncement(float time, Player goalPlayer, Player assistPlayer, Player secondAssistPlayer)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		base.StartCoroutine(this.IShowBlueTeamScoreAnnouncement(time, goalPlayer, assistPlayer, secondAssistPlayer));
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x0000C3ED File Offset: 0x0000A5ED
	public void ShowRedTeamScoreAnnouncement(float time, Player goalPlayer, Player assistPlayer, Player secondAssistPlayer)
	{
		if (Application.isBatchMode)
		{
			return;
		}
		base.StartCoroutine(this.IShowRedTeamScoreAnnouncement(time, goalPlayer, assistPlayer, secondAssistPlayer));
	}

	// Token: 0x0600088A RID: 2186 RVA: 0x0000C409 File Offset: 0x0000A609
	private IEnumerator IShowBlueTeamScoreAnnouncement(float time, Player goalPlayer, Player assistPlayer, Player secondAssistPlayer)
	{
		if (MonoBehaviourSingleton<SettingsManager>.Instance.ShowGameUserInterface == 0)
		{
			yield break;
		}
		string text = "";
		if (goalPlayer)
		{
			text += string.Format("{0}", goalPlayer.Username.Value);
			if (assistPlayer)
			{
				text += string.Format("<br><size=75%>{0}", assistPlayer.Username.Value);
			}
			if (secondAssistPlayer)
			{
				text += string.Format(" & {0}", secondAssistPlayer.Username.Value);
			}
		}
		this.blueTeamScorePlayersLabel.text = text;
		this.blueTeamScoreAnnouncement.style.visibility = Visibility.Visible;
		this.Show();
		yield return new WaitForSeconds(time);
		this.Hide(false);
		yield break;
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x0000C435 File Offset: 0x0000A635
	private IEnumerator IShowRedTeamScoreAnnouncement(float time, Player goalPlayer, Player assistPlayer, Player secondAssistPlayer)
	{
		if (MonoBehaviourSingleton<SettingsManager>.Instance.ShowGameUserInterface == 0)
		{
			yield break;
		}
		string text = "";
		if (goalPlayer)
		{
			text += string.Format("{0}", goalPlayer.Username.Value);
			if (assistPlayer)
			{
				text += string.Format(" + {0}", assistPlayer.Username.Value);
			}
			if (secondAssistPlayer)
			{
				text += string.Format(" & {0}", secondAssistPlayer.Username.Value);
			}
		}
		this.redTeamScorePlayersLabel.text = text;
		this.redTeamScoreAnnouncement.style.visibility = Visibility.Visible;
		this.Show();
		yield return new WaitForSeconds(time);
		this.Hide(false);
		yield break;
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x00035520 File Offset: 0x00033720
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x0000C469 File Offset: 0x0000A669
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x0000C473 File Offset: 0x0000A673
	protected internal override string __getTypeName()
	{
		return "UIAnnouncement";
	}

	// Token: 0x0400051F RID: 1311
	private VisualElement blueTeamScoreAnnouncement;

	// Token: 0x04000520 RID: 1312
	private VisualElement redTeamScoreAnnouncement;

	// Token: 0x04000521 RID: 1313
	private Label blueTeamScorePlayersLabel;

	// Token: 0x04000522 RID: 1314
	private Label redTeamScorePlayersLabel;
}
