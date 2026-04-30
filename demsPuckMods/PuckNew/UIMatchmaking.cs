using System;
using UnityEngine.UIElements;

// Token: 0x0200019D RID: 413
public class UIMatchmaking : UIView
{
	// Token: 0x06000BC1 RID: 3009 RVA: 0x0003763C File Offset: 0x0003583C
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("MatchmakingView", null);
		this.matching = base.View.Query("Matching", null);
		this.matchingPhaseLabel = this.matching.Query("PhaseLabel", null);
		this.matchingTimeLabel = this.matching.Query("TimeLabel", null);
		this.matchingCloseIconButtonContainer = this.matching.Query("CloseIconButtonContainer", null);
		this.matchingCloseIconButton = this.matchingCloseIconButtonContainer.Query("IconButton", null);
		this.matchingCloseIconButton.clicked += this.OnClickMatchingClose;
		this.matchingConnectButton = this.matching.Query("ConnectButton", null);
		this.matchingConnectButton.clicked += this.OnClickMatchingConnect;
	}

	// Token: 0x06000BC2 RID: 3010 RVA: 0x00037736 File Offset: 0x00035936
	public void SetMatchingVisibility(bool isVisible)
	{
		this.matching.style.display = (isVisible ? DisplayStyle.Flex : DisplayStyle.None);
	}

	// Token: 0x06000BC3 RID: 3011 RVA: 0x00037754 File Offset: 0x00035954
	public void SetMatchingPhaseText(string text)
	{
		this.matchingPhaseLabel.text = text;
	}

	// Token: 0x06000BC4 RID: 3012 RVA: 0x00037762 File Offset: 0x00035962
	public void SetMatchingTimeVisibility(bool isVisible)
	{
		this.matchingTimeLabel.style.display = (isVisible ? DisplayStyle.Flex : DisplayStyle.None);
	}

	// Token: 0x06000BC5 RID: 3013 RVA: 0x00037780 File Offset: 0x00035980
	public void SetMatchingTimeText(int seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)seconds);
		if (timeSpan.TotalHours < 1.0)
		{
			this.matchingTimeLabel.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
			return;
		}
		this.matchingTimeLabel.text = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x0003780F File Offset: 0x00035A0F
	public void SetMatchingConnectButtonVisibility(bool isVisible)
	{
		this.matchingConnectButton.style.display = (isVisible ? DisplayStyle.Flex : DisplayStyle.None);
	}

	// Token: 0x06000BC7 RID: 3015 RVA: 0x0003782D File Offset: 0x00035A2D
	public void SetMatchingCloseButtonVisibility(bool isVisible)
	{
		this.matchingCloseIconButtonContainer.style.display = (isVisible ? DisplayStyle.Flex : DisplayStyle.None);
	}

	// Token: 0x06000BC8 RID: 3016 RVA: 0x0003784B File Offset: 0x00035A4B
	private void OnClickMatchingClose()
	{
		EventManager.TriggerEvent("Event_OnMatchmakingMatchingClickClose", null);
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x00037858 File Offset: 0x00035A58
	private void OnClickMatchingConnect()
	{
		EventManager.TriggerEvent("Event_OnMatchmakingMatchingClickConnect", null);
	}

	// Token: 0x040006F7 RID: 1783
	private VisualElement matching;

	// Token: 0x040006F8 RID: 1784
	private Label matchingPhaseLabel;

	// Token: 0x040006F9 RID: 1785
	private Label matchingTimeLabel;

	// Token: 0x040006FA RID: 1786
	private Button matchingConnectButton;

	// Token: 0x040006FB RID: 1787
	private TemplateContainer matchingCloseIconButtonContainer;

	// Token: 0x040006FC RID: 1788
	private IconButton matchingCloseIconButton;
}
