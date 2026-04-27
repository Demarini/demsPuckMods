using System;
using System.Collections.Generic;

// Token: 0x0200016D RID: 365
internal class UIAnnouncementsController : UIViewController<UIAnnouncements>
{
	// Token: 0x06000AA0 RID: 2720 RVA: 0x00032544 File Offset: 0x00030744
	public override void Awake()
	{
		base.Awake();
		this.uiAnnouncements = base.GetComponent<UIAnnouncements>();
		EventManager.AddEventListener("Event_Everyone_OnGoalScored", new Action<Dictionary<string, object>>(this.Event_Everyone_OnGoalScored));
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x0003256E File Offset: 0x0003076E
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnGoalScored", new Action<Dictionary<string, object>>(this.Event_Everyone_OnGoalScored));
		base.OnDestroy();
	}

	// Token: 0x06000AA2 RID: 2722 RVA: 0x0003258C File Offset: 0x0003078C
	public void Event_Everyone_OnGoalScored(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["byTeam"];
		Player goalPlayer = (Player)message["goalPlayer"];
		Player assistPlayer = (Player)message["assistPlayer"];
		Player secondAssistPlayer = (Player)message["secondAssistPlayer"];
		this.uiAnnouncements.ShowScore(team, goalPlayer, assistPlayer, secondAssistPlayer);
	}

	// Token: 0x04000625 RID: 1573
	private UIAnnouncements uiAnnouncements;
}
