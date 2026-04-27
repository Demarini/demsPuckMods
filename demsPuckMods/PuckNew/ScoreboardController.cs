using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000063 RID: 99
public class ScoreboardController : MonoBehaviour
{
	// Token: 0x06000352 RID: 850 RVA: 0x00013A8B File Offset: 0x00011C8B
	private void Awake()
	{
		this.scoreboard = base.GetComponent<Scoreboard>();
		EventManager.AddEventListener("Event_Everyone_OnGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnGameStateChanged));
	}

	// Token: 0x06000353 RID: 851 RVA: 0x00013AAF File Offset: 0x00011CAF
	private void Start()
	{
		this.scoreboard.TurnOff();
	}

	// Token: 0x06000354 RID: 852 RVA: 0x00013ABC File Offset: 0x00011CBC
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnGameStateChanged));
	}

	// Token: 0x06000355 RID: 853 RVA: 0x00013AD4 File Offset: 0x00011CD4
	private void Event_Everyone_OnGameStateChanged(Dictionary<string, object> message)
	{
		GameState gameState = (GameState)message["newGameState"];
		GamePhase phase = gameState.Phase;
		if (phase - GamePhase.PreGame <= 8)
		{
			this.scoreboard.TurnOn();
		}
		else
		{
			this.scoreboard.TurnOff();
		}
		this.scoreboard.SetTick(gameState.Tick);
		this.scoreboard.SetPeriod(gameState.Period);
		this.scoreboard.SetBlueScore(gameState.BlueScore);
		this.scoreboard.SetRedScore(gameState.RedScore);
	}

	// Token: 0x0400024B RID: 587
	private Scoreboard scoreboard;
}
