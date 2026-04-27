using System;
using System.Collections.Generic;

// Token: 0x0200019A RID: 410
public class UIIdentityController : UIViewController<UIIdentity>
{
	// Token: 0x06000BA6 RID: 2982 RVA: 0x00037046 File Offset: 0x00035246
	public override void Awake()
	{
		base.Awake();
		this.uiIdentity = base.GetComponent<UIIdentity>();
		EventManager.AddEventListener("Event_OnPlayerDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerDataChanged));
	}

	// Token: 0x06000BA7 RID: 2983 RVA: 0x00037070 File Offset: 0x00035270
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnPlayerDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerDataChanged));
		base.OnDestroy();
	}

	// Token: 0x06000BA8 RID: 2984 RVA: 0x00037090 File Offset: 0x00035290
	private void Event_OnPlayerDataChanged(Dictionary<string, object> message)
	{
		PlayerData playerData = (PlayerData)message["newPlayerData"];
		if (playerData == null)
		{
			return;
		}
		this.uiIdentity.SetIdentity(playerData.username, playerData.number);
	}

	// Token: 0x040006E3 RID: 1763
	private UIIdentity uiIdentity;
}
