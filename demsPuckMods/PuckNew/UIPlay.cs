using System;
using UnityEngine.UIElements;

// Token: 0x020001AE RID: 430
public class UIPlay : UIView
{
	// Token: 0x06000C59 RID: 3161 RVA: 0x0003A598 File Offset: 0x00038798
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("PlayView", null);
		this.play = base.View.Query("Play", null);
		this.closeIconButton = base.View.Query("CloseIconButtonContainer", null).First().Query(null, null);
		this.closeIconButton.clicked += this.OnClickClose;
		this.threeVsThreeButton = this.play.Query("ThreeVsThreePlayButtonContainer", null).First().Query(null, null);
		this.threeVsThreeButton.clicked += this.OnClickThreeVersusThree;
		this.fiveVsFiveButton = this.play.Query("FiveVsFivePlayButtonContainer", null).First().Query(null, null);
		this.fiveVsFiveButton.clicked += this.OnClickFiveVersusFive;
		this.practiceButton = this.play.Query("PracticePlayButtonContainer", null).First().Query(null, null);
		this.practiceButton.clicked += this.OnClickPractice;
		this.serverBrowserButton = this.play.Query("ServerBrowserPlayButtonContainer", null).First().Query(null, null);
		this.serverBrowserButton.clicked += this.OnClickServerBrowser;
		this.statistics = this.play.Query("Statistics", null);
		this.playersLabel = this.statistics.Query("PlayersLabel", null);
	}

	// Token: 0x06000C5A RID: 3162 RVA: 0x0003A75A File Offset: 0x0003895A
	public void SetThreeVsThreeButtonEnabled(bool enabled)
	{
		this.threeVsThreeButton.SetEnabled(enabled);
	}

	// Token: 0x06000C5B RID: 3163 RVA: 0x0003A768 File Offset: 0x00038968
	public void SetThreeVsThreeButtonDescription(string description)
	{
		this.threeVsThreeButton.Description = description;
	}

	// Token: 0x06000C5C RID: 3164 RVA: 0x0003A776 File Offset: 0x00038976
	public void SetFiveVsFiveButtonEnabled(bool enabled)
	{
		this.fiveVsFiveButton.SetEnabled(enabled);
	}

	// Token: 0x06000C5D RID: 3165 RVA: 0x0003A784 File Offset: 0x00038984
	public void SetFiveVsFiveButtonDescription(string description)
	{
		this.fiveVsFiveButton.Description = description;
	}

	// Token: 0x06000C5E RID: 3166 RVA: 0x0003A792 File Offset: 0x00038992
	public void SetStatistics(int players)
	{
		this.playersLabel.text = string.Format("PLAYERS ONLINE: {0}", players);
	}

	// Token: 0x06000C5F RID: 3167 RVA: 0x0003A7AF File Offset: 0x000389AF
	private void OnClickClose()
	{
		EventManager.TriggerEvent("Event_OnPlayClickClose", null);
	}

	// Token: 0x06000C60 RID: 3168 RVA: 0x0003A7BC File Offset: 0x000389BC
	private void OnClickThreeVersusThree()
	{
		EventManager.TriggerEvent("Event_OnPlayClickThreeVsThree", null);
	}

	// Token: 0x06000C61 RID: 3169 RVA: 0x0003A7C9 File Offset: 0x000389C9
	private void OnClickFiveVersusFive()
	{
		EventManager.TriggerEvent("Event_OnPlayClickFiveVsFive", null);
	}

	// Token: 0x06000C62 RID: 3170 RVA: 0x0003A7D6 File Offset: 0x000389D6
	private void OnClickPractice()
	{
		EventManager.TriggerEvent("Event_OnPlayClickPractice", null);
	}

	// Token: 0x06000C63 RID: 3171 RVA: 0x0003A7E3 File Offset: 0x000389E3
	private void OnClickServerBrowser()
	{
		EventManager.TriggerEvent("Event_OnPlayClickServerBrowser", null);
	}

	// Token: 0x04000755 RID: 1877
	private VisualElement play;

	// Token: 0x04000756 RID: 1878
	private IconButton closeIconButton;

	// Token: 0x04000757 RID: 1879
	private PlayButton threeVsThreeButton;

	// Token: 0x04000758 RID: 1880
	private PlayButton fiveVsFiveButton;

	// Token: 0x04000759 RID: 1881
	private PlayButton practiceButton;

	// Token: 0x0400075A RID: 1882
	private PlayButton serverBrowserButton;

	// Token: 0x0400075B RID: 1883
	private VisualElement statistics;

	// Token: 0x0400075C RID: 1884
	private Label playersLabel;
}
