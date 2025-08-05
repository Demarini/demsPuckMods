using System;
using DG.Tweening;
using UnityEngine;

// Token: 0x02000004 RID: 4
public class ChangingRoomPlayer : MonoBehaviour
{
	// Token: 0x17000002 RID: 2
	// (get) Token: 0x0600001A RID: 26 RVA: 0x00006CC7 File Offset: 0x00004EC7
	// (set) Token: 0x0600001B RID: 27 RVA: 0x00006CCF File Offset: 0x00004ECF
	public PlayerTeam Team
	{
		get
		{
			return this.team;
		}
		set
		{
			if (this.team == value)
			{
				return;
			}
			this.team = value;
			this.OnTeamChanged();
		}
	}

	// Token: 0x17000003 RID: 3
	// (get) Token: 0x0600001C RID: 28 RVA: 0x00006CE8 File Offset: 0x00004EE8
	// (set) Token: 0x0600001D RID: 29 RVA: 0x00006CF0 File Offset: 0x00004EF0
	public PlayerRole Role
	{
		get
		{
			return this.role;
		}
		set
		{
			if (this.role == value)
			{
				return;
			}
			this.role = value;
			this.OnRoleChanged();
		}
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00006D09 File Offset: 0x00004F09
	private void Start()
	{
		this.lastMousePosition = MonoBehaviourSingleton<InputManager>.Instance.PointAction.ReadValue<Vector2>();
		this.initialPosition = base.transform.position;
		this.initialRotation = base.transform.eulerAngles;
	}

	// Token: 0x0600001F RID: 31 RVA: 0x00006D42 File Offset: 0x00004F42
	private void OnDestroy()
	{
		base.transform.DOKill(false);
	}

	// Token: 0x06000020 RID: 32 RVA: 0x00006D51 File Offset: 0x00004F51
	private void Update()
	{
		this.lastMousePosition = this.mousePosition;
		this.mousePosition = MonoBehaviourSingleton<InputManager>.Instance.PointAction.ReadValue<Vector2>();
		if (this.TrackMouse)
		{
			this.LookAtMouse();
		}
		if (this.RotateWithMouse)
		{
			this.Rotate();
		}
	}

	// Token: 0x06000021 RID: 33 RVA: 0x0000F758 File Offset: 0x0000D958
	private void LookAtMouse()
	{
		if (Camera.main == null)
		{
			return;
		}
		this.mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(this.mousePosition.x, this.mousePosition.y, 1f));
		this.mouseCameraLocalPosition = Camera.main.transform.InverseTransformPoint(this.mouseWorldPosition);
		this.mouseCameraLocalPosition.x = -this.mouseCameraLocalPosition.x;
		this.mouseCameraLocalPosition.y = this.mouseCameraLocalPosition.y + Camera.main.transform.position.y;
		this.PlayerMesh.LookAt(base.transform.TransformPoint(this.mouseCameraLocalPosition) + base.transform.forward * this.lookAtDistance, Time.deltaTime);
	}

	// Token: 0x06000022 RID: 34 RVA: 0x0000F834 File Offset: 0x0000DA34
	private void Rotate()
	{
		this.mouseRotationInertia = Vector2.Lerp(this.mouseRotationInertia, this.mouseRotationDelta, Time.deltaTime * 10f);
		base.transform.rotation *= Quaternion.AngleAxis(-this.mouseRotationInertia.x, Vector3.up);
		if (MonoBehaviourSingleton<InputManager>.Instance.ClickAction.IsPressed())
		{
			this.mouseRotationDelta = this.mousePosition - this.lastMousePosition;
			return;
		}
		this.mouseRotationDelta = Vector2.zero;
	}

	// Token: 0x06000023 RID: 35 RVA: 0x00006D90 File Offset: 0x00004F90
	public void Client_MovePlayerToDefaultPosition()
	{
		base.transform.DOKill(false);
		base.transform.DOMove(this.initialPosition, 0.5f, false);
		base.transform.DORotate(this.initialRotation, 0.5f, RotateMode.Fast);
	}

	// Token: 0x06000024 RID: 36 RVA: 0x0000F8C4 File Offset: 0x0000DAC4
	public void Client_MovePlayerToIdentityPosition()
	{
		base.transform.DOKill(false);
		base.transform.DOMove(this.identityPosition.position, 0.5f, false);
		base.transform.DORotate(this.identityPosition.eulerAngles, 0.5f, RotateMode.Fast);
	}

	// Token: 0x06000025 RID: 37 RVA: 0x0000F918 File Offset: 0x0000DB18
	public void UpdatePlayerMesh()
	{
		this.PlayerMesh.SetUsername(MonoBehaviourSingleton<StateManager>.Instance.PlayerData.username);
		this.PlayerMesh.SetNumber(MonoBehaviourSingleton<StateManager>.Instance.PlayerData.number.ToString());
		this.PlayerMesh.SetJersey(this.Team, MonoBehaviourSingleton<SettingsManager>.Instance.GetJerseySkin(this.Team, this.Role));
		this.PlayerMesh.SetRole(this.Role);
		this.PlayerMesh.PlayerHead.SetHelmetFlag(MonoBehaviourSingleton<SettingsManager>.Instance.Country);
		this.PlayerMesh.PlayerHead.SetHelmetVisor(MonoBehaviourSingleton<SettingsManager>.Instance.GetVisorSkin(this.Team, this.Role));
		this.PlayerMesh.PlayerHead.SetMustache(MonoBehaviourSingleton<SettingsManager>.Instance.Mustache);
		this.PlayerMesh.PlayerHead.SetBeard(MonoBehaviourSingleton<SettingsManager>.Instance.Beard);
	}

	// Token: 0x06000026 RID: 38 RVA: 0x00006DCF File Offset: 0x00004FCF
	private void OnTeamChanged()
	{
		this.UpdatePlayerMesh();
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00006DCF File Offset: 0x00004FCF
	private void OnRoleChanged()
	{
		this.UpdatePlayerMesh();
	}

	// Token: 0x04000005 RID: 5
	[Header("References")]
	[SerializeField]
	public PlayerMesh PlayerMesh;

	// Token: 0x04000006 RID: 6
	[SerializeField]
	public Transform identityPosition;

	// Token: 0x04000007 RID: 7
	[Header("Settings")]
	[SerializeField]
	private float lookAtDistance = 5f;

	// Token: 0x04000008 RID: 8
	[HideInInspector]
	public bool TrackMouse = true;

	// Token: 0x04000009 RID: 9
	public bool RotateWithMouse;

	// Token: 0x0400000A RID: 10
	private PlayerTeam team;

	// Token: 0x0400000B RID: 11
	private PlayerRole role;

	// Token: 0x0400000C RID: 12
	private Vector2 lastMousePosition;

	// Token: 0x0400000D RID: 13
	private Vector2 mousePosition;

	// Token: 0x0400000E RID: 14
	private Vector2 mouseRotationDelta;

	// Token: 0x0400000F RID: 15
	private Vector2 mouseRotationInertia;

	// Token: 0x04000010 RID: 16
	private Vector3 mouseWorldPosition;

	// Token: 0x04000011 RID: 17
	private Vector3 mouseCameraLocalPosition;

	// Token: 0x04000012 RID: 18
	private Vector3 initialPosition;

	// Token: 0x04000013 RID: 19
	private Vector3 initialRotation;
}
