using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// Token: 0x02000099 RID: 153
public static class CameraManagerController
{
	// Token: 0x060004EB RID: 1259 RVA: 0x0001AC2C File Offset: 0x00018E2C
	public static void Initialize()
	{
		EventManager.AddEventListener("Event_OnBaseCameraStarted", new Action<Dictionary<string, object>>(CameraManagerController.Event_OnBaseCameraStarted));
		EventManager.AddEventListener("Event_OnBaseCameraDestroyed", new Action<Dictionary<string, object>>(CameraManagerController.Event_OnBaseCameraDestroyed));
		EventManager.AddEventListener("Event_OnSceneLoaded", new Action<Dictionary<string, object>>(CameraManagerController.Event_OnSceneLoaded));
		EventManager.AddEventListener("Event_Everyone_OnPlayerGameStateChanged", new Action<Dictionary<string, object>>(CameraManagerController.Event_Everyone_OnPlayerGameStateChanged));
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x0001AC94 File Offset: 0x00018E94
	public static void Dispose()
	{
		EventManager.RemoveEventListener("Event_OnBaseCameraStarted", new Action<Dictionary<string, object>>(CameraManagerController.Event_OnBaseCameraStarted));
		EventManager.RemoveEventListener("Event_OnBaseCameraDestroyed", new Action<Dictionary<string, object>>(CameraManagerController.Event_OnBaseCameraDestroyed));
		EventManager.RemoveEventListener("Event_OnSceneLoaded", new Action<Dictionary<string, object>>(CameraManagerController.Event_OnSceneLoaded));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerGameStateChanged", new Action<Dictionary<string, object>>(CameraManagerController.Event_Everyone_OnPlayerGameStateChanged));
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x0001ACF9 File Offset: 0x00018EF9
	private static void Event_OnBaseCameraStarted(Dictionary<string, object> eventParams)
	{
		CameraManager.RegisterCamera((BaseCamera)eventParams["baseCamera"]);
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x0001AD10 File Offset: 0x00018F10
	private static void Event_OnBaseCameraDestroyed(Dictionary<string, object> eventParams)
	{
		CameraManager.UnregisterCamera((BaseCamera)eventParams["baseCamera"]);
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x0001AD28 File Offset: 0x00018F28
	private static void Event_OnSceneLoaded(Dictionary<string, object> eventParams)
	{
		if (((Scene)eventParams["scene"]).name == "locker_room")
		{
			CameraManager.SetActiveCamera(CameraType.LockerRoom, null);
		}
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x0001AD68 File Offset: 0x00018F68
	private static void Event_Everyone_OnPlayerGameStateChanged(Dictionary<string, object> eventParams)
	{
		Player player = (Player)eventParams["player"];
		PlayerGameState playerGameState = (PlayerGameState)eventParams["oldGameState"];
		PlayerGameState playerGameState2 = (PlayerGameState)eventParams["newGameState"];
		if (!player.IsLocalPlayer)
		{
			return;
		}
		if (playerGameState.Phase == playerGameState2.Phase && playerGameState.Team == playerGameState2.Team)
		{
			return;
		}
		switch (playerGameState2.Phase)
		{
		case PlayerPhase.TeamSelect:
			CameraManager.SetActiveCamera(CameraType.Cinematic, null);
			return;
		case PlayerPhase.PositionSelect:
			if (playerGameState2.Team == PlayerTeam.Blue)
			{
				CameraManager.SetActiveCamera(CameraType.BluePositionSelection, null);
				return;
			}
			if (playerGameState2.Team == PlayerTeam.Red)
			{
				CameraManager.SetActiveCamera(CameraType.RedPositionSelection, null);
				return;
			}
			CameraManager.SetActiveCamera(CameraType.Cinematic, null);
			return;
		case PlayerPhase.Play:
			CameraManager.SetActiveCamera(CameraType.Player, new ulong?(player.OwnerClientId));
			return;
		case PlayerPhase.Replay:
			CameraManager.SetActiveCamera(CameraType.Replay, null);
			return;
		case PlayerPhase.Spectate:
			CameraManager.SetActiveCamera(CameraType.Spectator, new ulong?(player.OwnerClientId));
			return;
		default:
			CameraManager.SetActiveCamera(CameraType.Cinematic, null);
			return;
		}
	}
}
