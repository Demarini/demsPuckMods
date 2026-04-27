using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

// Token: 0x020001D1 RID: 465
public class UITeamSelect : UIView
{
	// Token: 0x06000DB4 RID: 3508 RVA: 0x000410A4 File Offset: 0x0003F2A4
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("TeamSelectView", null);
		this.teamSelect = base.View.Query("TeamSelect", null);
		this.blueButton = this.teamSelect.Query("BlueButton", null);
		this.blueButton.clicked += this.OnClickTeamBlue;
		this.redButton = this.teamSelect.Query("RedButton", null);
		this.redButton.clicked += this.OnClickTeamRed;
		this.spectatorButton = this.teamSelect.Query("SpectatorButton", null);
		this.spectatorButton.clicked += this.OnClickTeamSpectator;
	}

	// Token: 0x06000DB5 RID: 3509 RVA: 0x0004117D File Offset: 0x0003F37D
	private void OnClickTeamBlue()
	{
		EventManager.TriggerEvent("Event_OnTeamSelectClickTeam", new Dictionary<string, object>
		{
			{
				"team",
				PlayerTeam.Blue
			}
		});
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x0004119F File Offset: 0x0003F39F
	private void OnClickTeamRed()
	{
		EventManager.TriggerEvent("Event_OnTeamSelectClickTeam", new Dictionary<string, object>
		{
			{
				"team",
				PlayerTeam.Red
			}
		});
	}

	// Token: 0x06000DB7 RID: 3511 RVA: 0x000411C1 File Offset: 0x0003F3C1
	private void OnClickTeamSpectator()
	{
		EventManager.TriggerEvent("Event_OnTeamSelectClickTeam", new Dictionary<string, object>
		{
			{
				"team",
				PlayerTeam.Spectator
			}
		});
	}

	// Token: 0x04000808 RID: 2056
	private VisualElement teamSelect;

	// Token: 0x04000809 RID: 2057
	private Button blueButton;

	// Token: 0x0400080A RID: 2058
	private Button redButton;

	// Token: 0x0400080B RID: 2059
	private Button spectatorButton;
}
