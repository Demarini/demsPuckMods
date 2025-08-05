using System;
using TMPro;
using UnityEngine;

// Token: 0x020000DD RID: 221
public class PlayerMesh : MonoBehaviour
{
	// Token: 0x1700009A RID: 154
	// (get) Token: 0x060006AB RID: 1707 RVA: 0x0000B4D6 File Offset: 0x000096D6
	// (set) Token: 0x060006AC RID: 1708 RVA: 0x0000B4E8 File Offset: 0x000096E8
	public bool IsUsernameActive
	{
		get
		{
			return this.usernameText.gameObject.activeSelf;
		}
		set
		{
			this.usernameText.gameObject.SetActive(value);
		}
	}

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x060006AD RID: 1709 RVA: 0x0000B4FB File Offset: 0x000096FB
	// (set) Token: 0x060006AE RID: 1710 RVA: 0x0000B50D File Offset: 0x0000970D
	public bool IsNumberActive
	{
		get
		{
			return this.numberText.gameObject.activeSelf;
		}
		set
		{
			this.numberText.gameObject.SetActive(value);
		}
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x060006AF RID: 1711 RVA: 0x0000B520 File Offset: 0x00009720
	// (set) Token: 0x060006B0 RID: 1712 RVA: 0x0000B546 File Offset: 0x00009746
	public bool IsLegPadsActive
	{
		get
		{
			return this.PlayerLegPadLeft.gameObject.activeSelf && this.PlayerLegPadRight.gameObject.activeSelf;
		}
		set
		{
			this.PlayerLegPadLeft.gameObject.SetActive(value);
			this.PlayerLegPadRight.gameObject.SetActive(value);
		}
	}

	// Token: 0x060006B1 RID: 1713 RVA: 0x0000B56A File Offset: 0x0000976A
	private void Awake()
	{
		this.initialGroinBonePosition = this.groinBone.localPosition;
		this.initialTorsoBonePosition = this.torsoBone.localPosition;
		this.initialHeadBonePosition = this.headBone.localPosition;
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x000271D8 File Offset: 0x000253D8
	private void Update()
	{
		this.groinBone.localPosition = this.initialGroinBonePosition * this.Stretch;
		this.torsoBone.localPosition = this.initialTorsoBonePosition * this.Stretch;
		this.headBone.localPosition = this.initialHeadBonePosition * this.Stretch;
	}

	// Token: 0x060006B3 RID: 1715 RVA: 0x0002723C File Offset: 0x0002543C
	public void LookAt(Vector3 targetPosition, float deltaTime)
	{
		Quaternion b = Quaternion.LookRotation(targetPosition - this.torsoBone.transform.position);
		Quaternion b2 = Quaternion.Lerp(base.transform.rotation, b, 0.5f);
		Quaternion b3 = Quaternion.Lerp(base.transform.rotation, b, 1f);
		this.torsoBone.transform.rotation = Quaternion.Slerp(this.torsoBone.transform.rotation, b2, deltaTime * this.lookAtSpeed);
		this.headBone.transform.rotation = Quaternion.Slerp(this.headBone.transform.rotation, b3, deltaTime * this.lookAtSpeed);
	}

	// Token: 0x060006B4 RID: 1716 RVA: 0x0000B59F File Offset: 0x0000979F
	public void SetUsername(string username)
	{
		this.usernameText.text = username;
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x0000B5AD File Offset: 0x000097AD
	public void SetNumber(string number)
	{
		this.numberText.text = number;
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x000272F0 File Offset: 0x000254F0
	public void SetJersey(PlayerTeam team, string jersey)
	{
		string str = (team == PlayerTeam.Blue) ? "blue_" : "red_";
		this.PlayerTorso.SetTexture(str + jersey);
		this.PlayerGroin.SetTexture(str + jersey);
	}

	// Token: 0x060006B7 RID: 1719 RVA: 0x0000B5BB File Offset: 0x000097BB
	public void SetRole(PlayerRole role)
	{
		this.IsLegPadsActive = (role == PlayerRole.Goalie);
		this.PlayerHead.HeadType = ((role == PlayerRole.Goalie) ? PlayerHeadType.Goalie : PlayerHeadType.Attacker);
	}

	// Token: 0x040003D4 RID: 980
	[Header("References")]
	[SerializeField]
	private Transform groinBone;

	// Token: 0x040003D5 RID: 981
	[SerializeField]
	private Transform torsoBone;

	// Token: 0x040003D6 RID: 982
	[SerializeField]
	private Transform headBone;

	// Token: 0x040003D7 RID: 983
	[SerializeField]
	public PlayerGroin PlayerGroin;

	// Token: 0x040003D8 RID: 984
	[SerializeField]
	public PlayerTorso PlayerTorso;

	// Token: 0x040003D9 RID: 985
	[SerializeField]
	public PlayerHead PlayerHead;

	// Token: 0x040003DA RID: 986
	[SerializeField]
	public PlayerLegPad PlayerLegPadLeft;

	// Token: 0x040003DB RID: 987
	[SerializeField]
	public PlayerLegPad PlayerLegPadRight;

	// Token: 0x040003DC RID: 988
	[SerializeField]
	private TMP_Text usernameText;

	// Token: 0x040003DD RID: 989
	[SerializeField]
	private TMP_Text numberText;

	// Token: 0x040003DE RID: 990
	[Header("Settings")]
	[SerializeField]
	private float lookAtSpeed = 10f;

	// Token: 0x040003DF RID: 991
	[HideInInspector]
	public float Stretch = 1f;

	// Token: 0x040003E0 RID: 992
	private Vector3 initialGroinBonePosition;

	// Token: 0x040003E1 RID: 993
	private Vector3 initialTorsoBonePosition;

	// Token: 0x040003E2 RID: 994
	private Vector3 initialHeadBonePosition;
}
