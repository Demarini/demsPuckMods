using System;
using UnityEngine;

// Token: 0x02000030 RID: 48
public class PlayerMesh : MonoBehaviour
{
	// Token: 0x17000014 RID: 20
	// (get) Token: 0x060000F7 RID: 247 RVA: 0x00005191 File Offset: 0x00003391
	// (set) Token: 0x060000F8 RID: 248 RVA: 0x00005199 File Offset: 0x00003399
	public float Stretch
	{
		get
		{
			return this.stretch;
		}
		set
		{
			if (this.stretch == value)
			{
				return;
			}
			this.stretch = value;
			this.OnStretchChanged();
		}
	}

	// Token: 0x060000F9 RID: 249 RVA: 0x000051B2 File Offset: 0x000033B2
	private void Awake()
	{
		this.initialGroinBonePosition = this.groinBone.localPosition;
		this.initialTorsoBonePosition = this.torsoBone.localPosition;
		this.initialHeadBonePosition = this.headBone.localPosition;
	}

	// Token: 0x060000FA RID: 250 RVA: 0x000051E8 File Offset: 0x000033E8
	public void LookAt(Vector3 targetPosition, float deltaTime, bool rotateTorso = true, bool rotateHead = true)
	{
		Quaternion b = Utils.GetLocalLookRotation(this.torsoBone, targetPosition);
		if (rotateTorso && rotateHead)
		{
			b = Utils.GetLocalLookRotation(this.torsoBone, targetPosition);
			b = Quaternion.Slerp(Quaternion.identity, b, 0.5f);
		}
		else if (rotateTorso)
		{
			b = Utils.GetLocalLookRotation(this.torsoBone, targetPosition);
		}
		else if (rotateHead)
		{
			b = Utils.GetLocalLookRotation(this.headBone, targetPosition);
		}
		Vector3 vector = Utils.WrapEulerAngles(b.eulerAngles);
		vector = Utils.Vector3Clamp(vector, new Vector3(-11.25f, -45f, 0f), new Vector3(45f, 45f, 0f));
		if (rotateTorso)
		{
			this.torsoBone.localRotation = Quaternion.Lerp(this.torsoBone.localRotation, Quaternion.Euler(vector), this.lookAtSpeed * deltaTime);
		}
		if (rotateHead)
		{
			this.headBone.localRotation = Quaternion.Lerp(this.headBone.localRotation, Quaternion.Euler(vector), this.lookAtSpeed * deltaTime);
		}
	}

	// Token: 0x060000FB RID: 251 RVA: 0x000052E0 File Offset: 0x000034E0
	public void SetUsername(string username)
	{
		this.PlayerTorso.SetUsername(username);
	}

	// Token: 0x060000FC RID: 252 RVA: 0x000052EE File Offset: 0x000034EE
	public void SetNumber(string number)
	{
		this.PlayerTorso.SetNumber(number);
	}

	// Token: 0x060000FD RID: 253 RVA: 0x000052FC File Offset: 0x000034FC
	public void SetLegsPadsActive(bool isActive)
	{
		this.PlayerLegPadLeft.gameObject.SetActive(isActive);
		this.PlayerLegPadRight.gameObject.SetActive(isActive);
	}

	// Token: 0x060000FE RID: 254 RVA: 0x00005320 File Offset: 0x00003520
	public void SetFlagID(int flagID)
	{
		this.PlayerHead.SetFlagID(flagID);
	}

	// Token: 0x060000FF RID: 255 RVA: 0x0000532E File Offset: 0x0000352E
	public void SetHeadgearID(int headgearID, PlayerRole role)
	{
		this.PlayerHead.SetHeadgearID(headgearID, role);
	}

	// Token: 0x06000100 RID: 256 RVA: 0x0000533D File Offset: 0x0000353D
	public void SetMustacheID(int mustacheID)
	{
		this.PlayerHead.SetMustacheID(mustacheID);
	}

	// Token: 0x06000101 RID: 257 RVA: 0x0000534B File Offset: 0x0000354B
	public void SetBeardID(int beardID)
	{
		this.PlayerHead.SetBeardID(beardID);
	}

	// Token: 0x06000102 RID: 258 RVA: 0x00005359 File Offset: 0x00003559
	public void SetJerseyID(int jerseyID, PlayerTeam team)
	{
		this.PlayerTorso.SetJerseyID(jerseyID, team);
		this.PlayerGroin.SetJerseyID(jerseyID, team);
	}

	// Token: 0x06000103 RID: 259 RVA: 0x00005378 File Offset: 0x00003578
	private void OnStretchChanged()
	{
		this.groinBone.localPosition = this.initialGroinBonePosition * this.Stretch;
		this.torsoBone.localPosition = this.initialTorsoBonePosition * this.Stretch;
		this.headBone.localPosition = this.initialHeadBonePosition * this.Stretch;
	}

	// Token: 0x040000AE RID: 174
	[Header("Settings")]
	[SerializeField]
	private float lookAtSpeed = 10f;

	// Token: 0x040000AF RID: 175
	[Header("References")]
	[SerializeField]
	private Transform groinBone;

	// Token: 0x040000B0 RID: 176
	[SerializeField]
	private Transform torsoBone;

	// Token: 0x040000B1 RID: 177
	[SerializeField]
	private Transform headBone;

	// Token: 0x040000B2 RID: 178
	[SerializeField]
	public PlayerHead PlayerHead;

	// Token: 0x040000B3 RID: 179
	[SerializeField]
	public PlayerTorso PlayerTorso;

	// Token: 0x040000B4 RID: 180
	[SerializeField]
	public PlayerGroin PlayerGroin;

	// Token: 0x040000B5 RID: 181
	[SerializeField]
	public PlayerLegPad PlayerLegPadLeft;

	// Token: 0x040000B6 RID: 182
	[SerializeField]
	public PlayerLegPad PlayerLegPadRight;

	// Token: 0x040000B7 RID: 183
	private float stretch = 1f;

	// Token: 0x040000B8 RID: 184
	private Vector3 initialGroinBonePosition;

	// Token: 0x040000B9 RID: 185
	private Vector3 initialTorsoBonePosition;

	// Token: 0x040000BA RID: 186
	private Vector3 initialHeadBonePosition;
}
