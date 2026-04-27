using System;
using UnityEngine;

// Token: 0x02000014 RID: 20
public class LockerRoomStick : MonoBehaviour
{
	// Token: 0x06000076 RID: 118 RVA: 0x0000368D File Offset: 0x0000188D
	public void SetSkinID(int skinID, PlayerTeam team, PlayerRole role)
	{
		((role == PlayerRole.Goalie) ? this.goalieStickMesh : this.attackerStickMesh).SetSkinID(skinID, team);
	}

	// Token: 0x06000077 RID: 119 RVA: 0x000036A8 File Offset: 0x000018A8
	public void SetShaftTapeID(int shaftTapeID, PlayerRole role)
	{
		((role == PlayerRole.Goalie) ? this.goalieStickMesh : this.attackerStickMesh).SetShaftTapeID(shaftTapeID);
	}

	// Token: 0x06000078 RID: 120 RVA: 0x000036C2 File Offset: 0x000018C2
	public void SetBladeTapeID(int bladeTapeID, PlayerRole role)
	{
		((role == PlayerRole.Goalie) ? this.goalieStickMesh : this.attackerStickMesh).SetBladeTapeID(bladeTapeID);
	}

	// Token: 0x06000079 RID: 121 RVA: 0x000036DC File Offset: 0x000018DC
	public void ShowRoleStick(PlayerRole role)
	{
		this.attackerStickMesh.gameObject.SetActive(role == PlayerRole.Attacker);
		this.goalieStickMesh.gameObject.SetActive(role == PlayerRole.Goalie);
	}

	// Token: 0x04000037 RID: 55
	[Header("References")]
	[SerializeField]
	private StickMesh attackerStickMesh;

	// Token: 0x04000038 RID: 56
	[SerializeField]
	private StickMesh goalieStickMesh;
}
