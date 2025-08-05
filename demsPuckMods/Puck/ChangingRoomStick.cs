using System;
using DG.Tweening;
using UnityEngine;

// Token: 0x02000006 RID: 6
public class ChangingRoomStick : MonoBehaviour
{
	// Token: 0x17000004 RID: 4
	// (get) Token: 0x0600003F RID: 63 RVA: 0x00006E54 File Offset: 0x00005054
	// (set) Token: 0x06000040 RID: 64 RVA: 0x00006E5C File Offset: 0x0000505C
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

	// Token: 0x06000041 RID: 65 RVA: 0x00006E75 File Offset: 0x00005075
	private void Start()
	{
		this.lastMousePosition = MonoBehaviourSingleton<InputManager>.Instance.PointAction.ReadValue<Vector2>();
		this.initialPosition = base.transform.position;
		this.initialRotation = base.transform.eulerAngles;
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00006EAE File Offset: 0x000050AE
	private void Update()
	{
		if (!this.RotateWithMouse)
		{
			return;
		}
		this.lastMousePosition = this.mousePosition;
		this.mousePosition = MonoBehaviourSingleton<InputManager>.Instance.PointAction.ReadValue<Vector2>();
		this.Rotate();
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00006D42 File Offset: 0x00004F42
	private void OnDestroy()
	{
		base.transform.DOKill(false);
	}

	// Token: 0x06000044 RID: 68 RVA: 0x0000FFC0 File Offset: 0x0000E1C0
	private void Rotate()
	{
		this.mouseRotationInertia = Vector2.Lerp(this.mouseRotationInertia, this.mouseRotationDelta, Time.deltaTime * 10f);
		base.transform.rotation *= Quaternion.AngleAxis(-this.mouseRotationInertia.x, Vector3.forward);
		if (MonoBehaviourSingleton<InputManager>.Instance.ClickAction.IsPressed())
		{
			this.mouseRotationDelta = this.mousePosition - this.lastMousePosition;
			return;
		}
		this.mouseRotationDelta = Vector2.zero;
	}

	// Token: 0x06000045 RID: 69 RVA: 0x00006EE0 File Offset: 0x000050E0
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000046 RID: 70 RVA: 0x00006EEE File Offset: 0x000050EE
	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x06000047 RID: 71 RVA: 0x00006EFC File Offset: 0x000050FC
	private void OnTeamChanged()
	{
		this.UpdateStickMesh();
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00010050 File Offset: 0x0000E250
	public void UpdateStickMesh()
	{
		this.StickMesh.SetSkin(this.Team, MonoBehaviourSingleton<SettingsManager>.Instance.GetStickSkin(this.Team, this.Role));
		this.StickMesh.SetShaftTape(MonoBehaviourSingleton<SettingsManager>.Instance.GetStickShaftSkin(this.Team, this.Role));
		this.StickMesh.SetBladeTape(MonoBehaviourSingleton<SettingsManager>.Instance.GetStickBladeSkin(this.Team, this.Role));
	}

	// Token: 0x06000049 RID: 73 RVA: 0x00006F04 File Offset: 0x00005104
	public void Client_MoveStickToDefaultPosition()
	{
		base.transform.DOKill(false);
		base.transform.DOMove(this.initialPosition, 0.5f, false);
		base.transform.DORotate(this.initialRotation, 0.5f, RotateMode.Fast);
	}

	// Token: 0x0600004A RID: 74 RVA: 0x000100C8 File Offset: 0x0000E2C8
	public void Client_MoveStickToAppearanceStickPosition()
	{
		base.transform.DOKill(false);
		base.transform.DOMove(this.appearanceStickPosition.position, 0.5f, false);
		base.transform.DORotate(this.appearanceStickPosition.eulerAngles, 0.5f, RotateMode.Fast);
	}

	// Token: 0x04000015 RID: 21
	[Header("References")]
	[SerializeField]
	public StickMesh StickMesh;

	// Token: 0x04000016 RID: 22
	[SerializeField]
	public Transform appearanceStickPosition;

	// Token: 0x04000017 RID: 23
	[Header("Settings")]
	public PlayerRole Role = PlayerRole.Attacker;

	// Token: 0x04000018 RID: 24
	private PlayerTeam team;

	// Token: 0x04000019 RID: 25
	public bool RotateWithMouse;

	// Token: 0x0400001A RID: 26
	private Vector2 lastMousePosition;

	// Token: 0x0400001B RID: 27
	private Vector2 mousePosition;

	// Token: 0x0400001C RID: 28
	private Vector2 mouseRotationDelta;

	// Token: 0x0400001D RID: 29
	private Vector2 mouseRotationInertia;

	// Token: 0x0400001E RID: 30
	private Vector3 initialPosition;

	// Token: 0x0400001F RID: 31
	private Vector3 initialRotation;
}
