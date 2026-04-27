using System;
using System.Collections.Generic;

// Token: 0x020001B4 RID: 436
public class UIPlayerUsernamesController : UIViewController<UIPlayerUsernames>
{
	// Token: 0x06000C86 RID: 3206 RVA: 0x0003AFF8 File Offset: 0x000391F8
	public override void Awake()
	{
		base.Awake();
		this.uiPlayerUsernames = base.GetComponent<UIPlayerUsernames>();
		EventManager.AddEventListener("Event_Everyone_OnLevelSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnLevelSpawned));
		EventManager.AddEventListener("Event_Everyone_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodySpawned));
		EventManager.AddEventListener("Event_Everyone_OnPlayerBodyDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodyDespawned));
		EventManager.AddEventListener("Event_Everyone_OnPlayerUsernameChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerUsernameChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerNumberChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerNumberChanged));
		EventManager.AddEventListener("Event_OnShowPlayerUsernamesChanged", new Action<Dictionary<string, object>>(this.Event_OnShowPlayerUsernamesChanged));
		EventManager.AddEventListener("Event_OnPlayerUsernamesFadeThresholdChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerUsernamesFadeThresholdChanged));
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x0003B0B1 File Offset: 0x000392B1
	private void Start()
	{
		if (!SettingsManager.ShowPlayerUsernames)
		{
			this.uiPlayerUsernames.Hide();
		}
		this.uiPlayerUsernames.FadeThreshold = SettingsManager.PlayerUsernamesFadeThreshold;
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x0003B0D8 File Offset: 0x000392D8
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnLevelSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnLevelSpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodySpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerBodyDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodyDespawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerUsernameChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerUsernameChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerNumberChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerNumberChanged));
		EventManager.RemoveEventListener("Event_OnShowPlayerUsernamesChanged", new Action<Dictionary<string, object>>(this.Event_OnShowPlayerUsernamesChanged));
		EventManager.RemoveEventListener("Event_OnPlayerUsernamesFadeThresholdChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerUsernamesFadeThresholdChanged));
		base.OnDestroy();
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x0003B188 File Offset: 0x00039388
	private void Event_Everyone_OnLevelSpawned(Dictionary<string, object> message)
	{
		Level level = (Level)message["level"];
		this.uiPlayerUsernames.Bounds = level.Bounds;
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x0003B1B8 File Offset: 0x000393B8
	private void Event_Everyone_OnPlayerBodySpawned(Dictionary<string, object> message)
	{
		PlayerBody playerBody = (PlayerBody)message["playerBody"];
		this.uiPlayerUsernames.AddPlayerBody(playerBody);
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x0003B1E4 File Offset: 0x000393E4
	private void Event_Everyone_OnPlayerBodyDespawned(Dictionary<string, object> message)
	{
		PlayerBody playerBody = (PlayerBody)message["playerBody"];
		this.uiPlayerUsernames.RemovePlayerBody(playerBody);
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x0003B210 File Offset: 0x00039410
	private void Event_Everyone_OnPlayerUsernameChanged(Dictionary<string, object> message)
	{
		PlayerBody playerBody = ((Player)message["player"]).PlayerBody;
		if (playerBody)
		{
			this.uiPlayerUsernames.StyleUsername(playerBody);
		}
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x0003B248 File Offset: 0x00039448
	private void Event_Everyone_OnPlayerNumberChanged(Dictionary<string, object> message)
	{
		PlayerBody playerBody = ((Player)message["player"]).PlayerBody;
		if (playerBody)
		{
			this.uiPlayerUsernames.StyleUsername(playerBody);
		}
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x0003B27F File Offset: 0x0003947F
	private void Event_OnShowPlayerUsernamesChanged(Dictionary<string, object> message)
	{
		if ((bool)message["value"])
		{
			this.uiPlayerUsernames.Show();
			return;
		}
		this.uiPlayerUsernames.Hide();
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x0003B2AC File Offset: 0x000394AC
	private void Event_OnPlayerUsernamesFadeThresholdChanged(Dictionary<string, object> message)
	{
		float fadeThreshold = (float)message["value"];
		this.uiPlayerUsernames.FadeThreshold = fadeThreshold;
	}

	// Token: 0x0400076C RID: 1900
	private UIPlayerUsernames uiPlayerUsernames;
}
