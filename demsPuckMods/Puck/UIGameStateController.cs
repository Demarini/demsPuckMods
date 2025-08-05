using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x0200010B RID: 267
public class UIGameStateController : NetworkBehaviour
{
	// Token: 0x06000955 RID: 2389 RVA: 0x0000CDB9 File Offset: 0x0000AFB9
	private void Awake()
	{
		this.uiGameState = base.GetComponent<UIGameState>();
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x0000CDC7 File Offset: 0x0000AFC7
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnGameStateChanged", new Action<Dictionary<string, object>>(this.Event_OnGameStateChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnShowGameUserInterfaceChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnShowGameUserInterfaceChanged));
	}

	// Token: 0x06000957 RID: 2391 RVA: 0x0000CDFF File Offset: 0x0000AFFF
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnGameStateChanged", new Action<Dictionary<string, object>>(this.Event_OnGameStateChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnShowGameUserInterfaceChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnShowGameUserInterfaceChanged));
		base.OnDestroy();
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x00039150 File Offset: 0x00037350
	private void Event_OnGameStateChanged(Dictionary<string, object> message)
	{
		GameState gameState = (GameState)message["newGameState"];
		switch (gameState.Phase)
		{
		case GamePhase.Warmup:
			this.uiGameState.SetGameTime((float)gameState.Time);
			this.uiGameState.SetGamePhase("WARMUP");
			goto IL_176;
		case GamePhase.FaceOff:
			this.uiGameState.SetGameTime((float)gameState.Time);
			this.uiGameState.SetGamePhase("FACE OFF");
			goto IL_176;
		case GamePhase.BlueScore:
		case GamePhase.RedScore:
			this.uiGameState.SetGameTime((float)gameState.Time);
			this.uiGameState.SetGamePhase("GOAL");
			goto IL_176;
		case GamePhase.Replay:
			this.uiGameState.SetGameTime((float)gameState.Time);
			this.uiGameState.SetGamePhase("REPLAY");
			goto IL_176;
		case GamePhase.PeriodOver:
			this.uiGameState.SetGameTime((float)gameState.Time);
			this.uiGameState.SetGamePhase("INTERMISSION");
			goto IL_176;
		case GamePhase.GameOver:
			this.uiGameState.SetGameTime((float)gameState.Time);
			this.uiGameState.SetGamePhase("GAME OVER");
			goto IL_176;
		}
		this.uiGameState.SetGameTime((float)gameState.Time);
		if (gameState.Period <= 3)
		{
			this.uiGameState.SetGamePhase(string.Format("PERIOD {0}", gameState.Period));
		}
		else
		{
			this.uiGameState.SetGamePhase("OVERTIME");
		}
		IL_176:
		this.uiGameState.SetBlueTeamScore(gameState.BlueScore);
		this.uiGameState.SetRedTeamScore(gameState.RedScore);
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x0000CE3D File Offset: 0x0000B03D
	private void Event_Client_OnShowGameUserInterfaceChanged(Dictionary<string, object> message)
	{
		if (NetworkBehaviourSingleton<UIManager>.Instance.UIState == UIState.MainMenu)
		{
			return;
		}
		if ((bool)message["value"])
		{
			this.uiGameState.Show();
			return;
		}
		this.uiGameState.Hide(false);
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x0600095C RID: 2396 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x0000CE76 File Offset: 0x0000B076
	protected internal override string __getTypeName()
	{
		return "UIGameStateController";
	}

	// Token: 0x040005A6 RID: 1446
	private UIGameState uiGameState;
}
