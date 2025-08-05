using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class ScoreboardController : MonoBehaviour
{
	// Token: 0x0600012E RID: 302 RVA: 0x000079D7 File Offset: 0x00005BD7
	private void Awake()
	{
		this.scoreboard = base.GetComponent<Scoreboard>();
	}

	// Token: 0x0600012F RID: 303 RVA: 0x00011E24 File Offset: 0x00010024
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnGameStateChanged", new Action<Dictionary<string, object>>(this.Event_OnGameStateChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnGamePhaseChanged", new Action<Dictionary<string, object>>(this.Event_OnGamePhaseChanged));
		this.scoreboard.TurnOff();
	}

	// Token: 0x06000130 RID: 304 RVA: 0x000079E5 File Offset: 0x00005BE5
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnGameStateChanged", new Action<Dictionary<string, object>>(this.Event_OnGameStateChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnGamePhaseChanged", new Action<Dictionary<string, object>>(this.Event_OnGamePhaseChanged));
	}

	// Token: 0x06000131 RID: 305 RVA: 0x00007A1D File Offset: 0x00005C1D
	private void Event_OnGamePhaseChanged(Dictionary<string, object> message)
	{
		if ((GamePhase)message["newGamePhase"] == GamePhase.Warmup)
		{
			this.scoreboard.TurnOff();
			return;
		}
		this.scoreboard.TurnOn();
	}

	// Token: 0x06000132 RID: 306 RVA: 0x00011E74 File Offset: 0x00010074
	private void Event_OnGameStateChanged(Dictionary<string, object> message)
	{
		GameState gameState = (GameState)message["newGameState"];
		GamePhase phase = gameState.Phase;
		if (phase != GamePhase.FaceOff)
		{
			if (phase == GamePhase.Playing)
			{
				this.scoreboard.SetTime(gameState.Time);
				return;
			}
		}
		else
		{
			this.scoreboard.SetPeriod(gameState.Period);
			this.scoreboard.SetBlueScore(gameState.BlueScore);
			this.scoreboard.SetRedScore(gameState.RedScore);
		}
	}

	// Token: 0x0400009B RID: 155
	private Scoreboard scoreboard;
}
