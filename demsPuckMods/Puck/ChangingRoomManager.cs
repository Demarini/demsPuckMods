using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// Token: 0x02000034 RID: 52
public class ChangingRoomManager : MonoBehaviourSingleton<ChangingRoomManager>
{
	// Token: 0x17000021 RID: 33
	// (get) Token: 0x06000166 RID: 358 RVA: 0x00007CB8 File Offset: 0x00005EB8
	// (set) Token: 0x06000167 RID: 359 RVA: 0x00007CC0 File Offset: 0x00005EC0
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

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x06000168 RID: 360 RVA: 0x00007CD9 File Offset: 0x00005ED9
	// (set) Token: 0x06000169 RID: 361 RVA: 0x00007CE1 File Offset: 0x00005EE1
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

	// Token: 0x0600016A RID: 362 RVA: 0x00007CFA File Offset: 0x00005EFA
	public override void Awake()
	{
		base.Awake();
		base.DestroyOnLoad();
		this.initialMainCameraPosition = this.mainCamera.transform.position;
		this.inititalMainCameraRotation = this.mainCamera.transform.eulerAngles;
	}

	// Token: 0x0600016B RID: 363 RVA: 0x00007D34 File Offset: 0x00005F34
	private void OnDestroy()
	{
		this.mainCamera.transform.DOKill(false);
	}

	// Token: 0x0600016C RID: 364 RVA: 0x00007D48 File Offset: 0x00005F48
	public void Client_DisableAllCameras()
	{
		this.mainCamera.Disable();
	}

	// Token: 0x0600016D RID: 365 RVA: 0x00007D55 File Offset: 0x00005F55
	public void Client_EnableMainCamera()
	{
		this.Client_DisableAllCameras();
		this.mainCamera.Enable();
	}

	// Token: 0x0600016E RID: 366 RVA: 0x00012A10 File Offset: 0x00010C10
	public void Client_MoveCameraToDefaultPosition()
	{
		this.mainCamera.transform.DOKill(false);
		this.mainCamera.transform.DOMove(this.initialMainCameraPosition, 0.5f, false);
		this.mainCamera.transform.DORotate(this.inititalMainCameraRotation, 0.5f, RotateMode.Fast);
	}

	// Token: 0x0600016F RID: 367 RVA: 0x00012A6C File Offset: 0x00010C6C
	public void Client_MoveCameraToPlayerPosition()
	{
		this.mainCamera.transform.DOKill(false);
		this.mainCamera.transform.DOMove(this.playerCameraPosition.position, 0.5f, false);
		this.mainCamera.transform.DORotate(this.playerCameraPosition.eulerAngles, 0.5f, RotateMode.Fast);
	}

	// Token: 0x06000170 RID: 368 RVA: 0x00012AD0 File Offset: 0x00010CD0
	public void Client_MoveCameraToIdentityPosition()
	{
		this.mainCamera.transform.DOKill(false);
		this.mainCamera.transform.DOMove(this.identityCameraPosition.position, 0.5f, false);
		this.mainCamera.transform.DORotate(this.identityCameraPosition.eulerAngles, 0.5f, RotateMode.Fast);
	}

	// Token: 0x06000171 RID: 369 RVA: 0x00012B34 File Offset: 0x00010D34
	public void Client_MoveCameraToAppearanceDefaultPosition()
	{
		this.mainCamera.transform.DOKill(false);
		this.mainCamera.transform.DOMove(this.appearanceDefaultCameraPosition.position, 0.5f, false);
		this.mainCamera.transform.DORotate(this.appearanceDefaultCameraPosition.eulerAngles, 0.5f, RotateMode.Fast);
	}

	// Token: 0x06000172 RID: 370 RVA: 0x00012B98 File Offset: 0x00010D98
	public void Client_MoveCameraToAppearanceHeadPosition()
	{
		this.mainCamera.transform.DOKill(false);
		this.mainCamera.transform.DOMove(this.appearanceHeadCameraPosition.position, 0.5f, false);
		this.mainCamera.transform.DORotate(this.appearanceHeadCameraPosition.eulerAngles, 0.5f, RotateMode.Fast);
	}

	// Token: 0x06000173 RID: 371 RVA: 0x00012BFC File Offset: 0x00010DFC
	public void Client_MoveCameraToAppearanceJerseyPosition()
	{
		this.mainCamera.transform.DOKill(false);
		this.mainCamera.transform.DOMove(this.appearanceJerseyCameraPosition.position, 0.5f, false);
		this.mainCamera.transform.DORotate(this.appearanceJerseyCameraPosition.eulerAngles, 0.5f, RotateMode.Fast);
	}

	// Token: 0x06000174 RID: 372 RVA: 0x00007D68 File Offset: 0x00005F68
	private void OnTeamChanged()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnChangingRoomTeamChanged", new Dictionary<string, object>
		{
			{
				"team",
				this.Team
			}
		});
	}

	// Token: 0x06000175 RID: 373 RVA: 0x00007D94 File Offset: 0x00005F94
	private void OnRoleChanged()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnChangingRoomRoleChanged", new Dictionary<string, object>
		{
			{
				"role",
				this.Role
			}
		});
	}

	// Token: 0x040000C4 RID: 196
	[Header("References")]
	[SerializeField]
	private BaseCamera mainCamera;

	// Token: 0x040000C5 RID: 197
	[SerializeField]
	private Transform playerCameraPosition;

	// Token: 0x040000C6 RID: 198
	[SerializeField]
	private Transform identityCameraPosition;

	// Token: 0x040000C7 RID: 199
	[SerializeField]
	private Transform appearanceDefaultCameraPosition;

	// Token: 0x040000C8 RID: 200
	[SerializeField]
	private Transform appearanceHeadCameraPosition;

	// Token: 0x040000C9 RID: 201
	[SerializeField]
	private Transform appearanceJerseyCameraPosition;

	// Token: 0x040000CA RID: 202
	private PlayerTeam team;

	// Token: 0x040000CB RID: 203
	private PlayerRole role;

	// Token: 0x040000CC RID: 204
	private Vector3 initialMainCameraPosition;

	// Token: 0x040000CD RID: 205
	private Vector3 inititalMainCameraRotation;
}
