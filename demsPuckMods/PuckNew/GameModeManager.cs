using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A6 RID: 166
public class GameModeManager : MonoBehaviourSingleton<GameModeManager>
{
	// Token: 0x06000543 RID: 1347 RVA: 0x0001CF79 File Offset: 0x0001B179
	public override void Awake()
	{
		base.Awake();
		this.RegisterGameModes();
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x0001CF88 File Offset: 0x0001B188
	private void RegisterGameModes()
	{
		PublicGameMode<PublicGameModeConfig> value = new PublicGameMode<PublicGameModeConfig>("./public_game_mode_config.json", "--publicGameModeConfigPath", "--publicGameModeConfig", "PUCK_PUBLIC_GAME_MODE_CONFIG");
		CompetitiveGameMode<CompetitiveGameModeConfig> value2 = new CompetitiveGameMode<CompetitiveGameModeConfig>("./competitive_game_mode_config.json", "--competitiveGameModeConfigPath", "--competitiveGameModeConfig", "PUCK_COMPETITIVE_GAME_MODE_CONFIG");
		this.gameModeMap.Add("public", value);
		this.gameModeMap.Add("competitive", value2);
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x0001CFEC File Offset: 0x0001B1EC
	public void SelectGameMode(string name)
	{
		if (!this.gameModeMap.ContainsKey(name))
		{
			return;
		}
		this.selectedGameMode = this.gameModeMap[name];
		Debug.Log("[GameModeManager] Selected game mode " + this.selectedGameMode.GetType().Name);
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x0001D039 File Offset: 0x0001B239
	public void DeselectGameMode()
	{
		this.selectedGameMode = null;
		Debug.Log("[GameModeManager] Deselected game mode");
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x0001D04C File Offset: 0x0001B24C
	public void EnableSelectedGameMode()
	{
		if (this.selectedGameMode == null)
		{
			return;
		}
		if (this.selectedGameMode.IsInitialized)
		{
			return;
		}
		Debug.Log("[GameModeManager] Enabling game mode " + this.selectedGameMode.GetType().Name);
		this.selectedGameMode.Initialize(this.Level, this.serverManager, this.gameManager, this.playerManager, this.puckManager, this.chatManager, this.replayManager);
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x0001D0C8 File Offset: 0x0001B2C8
	public void DisableSelectedGameMode()
	{
		if (this.selectedGameMode == null)
		{
			return;
		}
		if (!this.selectedGameMode.IsInitialized)
		{
			return;
		}
		Debug.Log("[GameModeManager] Disabling game mode " + this.selectedGameMode.GetType().Name);
		this.selectedGameMode.Dispose();
	}

	// Token: 0x04000336 RID: 822
	[Header("References")]
	[SerializeField]
	private ServerManager serverManager;

	// Token: 0x04000337 RID: 823
	[SerializeField]
	private GameManager gameManager;

	// Token: 0x04000338 RID: 824
	[SerializeField]
	private PlayerManager playerManager;

	// Token: 0x04000339 RID: 825
	[SerializeField]
	private PuckManager puckManager;

	// Token: 0x0400033A RID: 826
	[SerializeField]
	private ChatManager chatManager;

	// Token: 0x0400033B RID: 827
	[SerializeField]
	private ReplayManager replayManager;

	// Token: 0x0400033C RID: 828
	[HideInInspector]
	public Level Level;

	// Token: 0x0400033D RID: 829
	private Dictionary<string, IGameMode> gameModeMap = new Dictionary<string, IGameMode>();

	// Token: 0x0400033E RID: 830
	private IGameMode selectedGameMode;
}
