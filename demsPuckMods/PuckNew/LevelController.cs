using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200000F RID: 15
public class LevelController : MonoBehaviour
{
	// Token: 0x06000049 RID: 73 RVA: 0x00002C60 File Offset: 0x00000E60
	public virtual void Awake()
	{
		this.level = base.GetComponent<Level>();
		EventManager.AddEventListener("Event_Everyone_OnGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnGameStateChanged));
	}

	// Token: 0x0600004A RID: 74 RVA: 0x00002C84 File Offset: 0x00000E84
	public virtual void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnGameStateChanged));
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00002C9C File Offset: 0x00000E9C
	private void Event_Everyone_OnGameStateChanged(Dictionary<string, object> eventParams)
	{
		ref GameState ptr = (GameState)eventParams["oldGameState"];
		GameState gameState = (GameState)eventParams["newGameState"];
		if (ptr.Phase == gameState.Phase)
		{
			return;
		}
		switch (gameState.Phase)
		{
		case GamePhase.Warmup:
		case GamePhase.PreGame:
		case GamePhase.FaceOff:
			this.level.SetBlueGoalLightEnabled(false);
			this.level.SetRedGoalLightEnabled(false);
			return;
		case GamePhase.Play:
			break;
		case GamePhase.BlueScore:
			this.level.SetRedGoalLightEnabled(true);
			return;
		case GamePhase.RedScore:
			this.level.SetBlueGoalLightEnabled(true);
			break;
		default:
			return;
		}
	}

	// Token: 0x04000028 RID: 40
	private Level level;
}
