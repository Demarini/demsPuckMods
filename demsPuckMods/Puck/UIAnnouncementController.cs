using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x020000FB RID: 251
internal class UIAnnouncementController : NetworkBehaviour
{
	// Token: 0x0600089C RID: 2204 RVA: 0x0000C4A8 File Offset: 0x0000A6A8
	private void Awake()
	{
		this.uiAnnouncement = base.GetComponent<UIAnnouncement>();
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x0000C4B6 File Offset: 0x0000A6B6
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnGoalScored", new Action<Dictionary<string, object>>(this.Event_OnGoalScored));
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x0000C4D3 File Offset: 0x0000A6D3
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnGoalScored", new Action<Dictionary<string, object>>(this.Event_OnGoalScored));
		base.OnDestroy();
	}

	// Token: 0x0600089F RID: 2207 RVA: 0x00035798 File Offset: 0x00033998
	public void Event_OnGoalScored(Dictionary<string, object> message)
	{
		PlayerTeam playerTeam = (PlayerTeam)message["team"];
		bool flag = (bool)message["hasGoalPlayer"];
		ulong clientId = (ulong)message["goalPlayerClientId"];
		bool flag2 = (bool)message["hasAssistPlayer"];
		ulong clientId2 = (ulong)message["assistPlayerClientId"];
		bool flag3 = (bool)message["hasSecondAssistPlayer"];
		ulong clientId3 = (ulong)message["secondAssistPlayerClientId"];
		if (playerTeam == PlayerTeam.Blue)
		{
			this.uiAnnouncement.ShowBlueTeamScoreAnnouncement(3f, flag ? NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId) : null, flag2 ? NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId2) : null, flag3 ? NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId3) : null);
			return;
		}
		if (playerTeam != PlayerTeam.Red)
		{
			return;
		}
		this.uiAnnouncement.ShowRedTeamScoreAnnouncement(3f, flag ? NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId) : null, flag2 ? NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId2) : null, flag3 ? NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId3) : null);
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x0000C4F6 File Offset: 0x0000A6F6
	protected internal override string __getTypeName()
	{
		return "UIAnnouncementController";
	}

	// Token: 0x04000531 RID: 1329
	private UIAnnouncement uiAnnouncement;
}
