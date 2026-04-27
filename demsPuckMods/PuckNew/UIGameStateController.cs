using System;
using System.Collections.Generic;

// Token: 0x02000196 RID: 406
public class UIGameStateController : UIViewController<UIGameState>
{
	// Token: 0x06000B8A RID: 2954 RVA: 0x00036909 File Offset: 0x00034B09
	public override void Awake()
	{
		base.Awake();
		this.uiGameState = base.GetComponent<UIGameState>();
		EventManager.AddEventListener("Event_Everyone_OnGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnGameStateChanged));
		EventManager.AddEventListener("Event_OnShowGameUserInterfaceChanged", new Action<Dictionary<string, object>>(this.Event_OnShowGameUserInterfaceChanged));
	}

	// Token: 0x06000B8B RID: 2955 RVA: 0x00036949 File Offset: 0x00034B49
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnGameStateChanged));
		EventManager.RemoveEventListener("Event_OnShowGameUserInterfaceChanged", new Action<Dictionary<string, object>>(this.Event_OnShowGameUserInterfaceChanged));
		base.OnDestroy();
	}

	// Token: 0x06000B8C RID: 2956 RVA: 0x00036980 File Offset: 0x00034B80
	private void Event_Everyone_OnGameStateChanged(Dictionary<string, object> message)
	{
		GameState gameState = (GameState)message["newGameState"];
		this.uiGameState.SetPhase(Utils.GetHumanizedGamePhase(gameState.Phase, gameState.Period, gameState.IsOvertime));
		this.uiGameState.SetTick(gameState.Tick);
		this.uiGameState.SetScore(PlayerTeam.Blue, gameState.BlueScore);
		this.uiGameState.SetScore(PlayerTeam.Red, gameState.RedScore);
	}

	// Token: 0x06000B8D RID: 2957 RVA: 0x000369F5 File Offset: 0x00034BF5
	private void Event_OnShowGameUserInterfaceChanged(Dictionary<string, object> message)
	{
		if (GlobalStateManager.UIState.Phase == UIPhase.LockerRoom)
		{
			return;
		}
		if ((bool)message["value"])
		{
			this.uiGameState.Show();
			return;
		}
		this.uiGameState.Hide();
	}

	// Token: 0x040006D6 RID: 1750
	private UIGameState uiGameState;
}
