using System;

// Token: 0x020000A5 RID: 165
internal interface IGameMode
{
	// Token: 0x17000082 RID: 130
	// (get) Token: 0x0600053F RID: 1343
	// (set) Token: 0x06000540 RID: 1344
	bool IsInitialized { get; set; }

	// Token: 0x06000541 RID: 1345
	bool Initialize(Level level, ServerManager serverManager, GameManager gameManager, PlayerManager playerManager, PuckManager puckManager, ChatManager chatManager, ReplayManager replayManager);

	// Token: 0x06000542 RID: 1346
	bool Dispose();
}
