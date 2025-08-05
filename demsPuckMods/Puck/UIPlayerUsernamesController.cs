using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000122 RID: 290
public class UIPlayerUsernamesController : MonoBehaviour
{
	// Token: 0x06000A3C RID: 2620 RVA: 0x0000D87E File Offset: 0x0000BA7E
	private void Awake()
	{
		this.uiPlayerUsernames = base.GetComponent<UIPlayerUsernames>();
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x0003BDE8 File Offset: 0x00039FE8
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodySpawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerBodyDespawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodyDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerUsernameChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerUsernameChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerNumberChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerNumberChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnShowPlayerUsernamesChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnShowPlayerUsernamesChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPlayerUsernamesFadeThresholdChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerUsernamesFadeThresholdChanged));
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x0003BE98 File Offset: 0x0003A098
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodySpawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerBodyDespawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodyDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerUsernameChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerUsernameChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerNumberChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerNumberChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnShowPlayerUsernamesChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnShowPlayerUsernamesChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPlayerUsernamesFadeThresholdChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerUsernamesFadeThresholdChanged));
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x0003BF48 File Offset: 0x0003A148
	private void Event_OnPlayerBodySpawned(Dictionary<string, object> message)
	{
		PlayerBodyV2 playerBody = (PlayerBodyV2)message["playerBody"];
		this.uiPlayerUsernames.AddPlayerBody(playerBody);
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x0003BF74 File Offset: 0x0003A174
	private void Event_OnPlayerBodyDespawned(Dictionary<string, object> message)
	{
		PlayerBodyV2 playerBody = (PlayerBodyV2)message["playerBody"];
		this.uiPlayerUsernames.RemovePlayerBody(playerBody);
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x0003BFA0 File Offset: 0x0003A1A0
	private void Event_OnPlayerUsernameChanged(Dictionary<string, object> message)
	{
		PlayerBodyV2 playerBody = ((Player)message["player"]).PlayerBody;
		if (playerBody)
		{
			this.uiPlayerUsernames.UpdatePlayerBody(playerBody);
		}
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x0003BFA0 File Offset: 0x0003A1A0
	private void Event_OnPlayerNumberChanged(Dictionary<string, object> message)
	{
		PlayerBodyV2 playerBody = ((Player)message["player"]).PlayerBody;
		if (playerBody)
		{
			this.uiPlayerUsernames.UpdatePlayerBody(playerBody);
		}
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x0000D88C File Offset: 0x0000BA8C
	private void Event_Client_OnShowPlayerUsernamesChanged(Dictionary<string, object> message)
	{
		if ((bool)message["value"])
		{
			this.uiPlayerUsernames.Show();
			return;
		}
		this.uiPlayerUsernames.Hide(true);
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x0003BFD8 File Offset: 0x0003A1D8
	private void Event_Client_OnPlayerUsernamesFadeThresholdChanged(Dictionary<string, object> message)
	{
		float fadeThreshold = (float)message["value"];
		this.uiPlayerUsernames.FadeThreshold = fadeThreshold;
	}

	// Token: 0x04000609 RID: 1545
	private UIPlayerUsernames uiPlayerUsernames;
}
