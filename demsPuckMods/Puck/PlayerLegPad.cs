using System;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using DG.Tweening.CustomPlugins;
using DG.Tweening.Plugins.Options;
using UnityEngine;

// Token: 0x020000DC RID: 220
public class PlayerLegPad : MonoBehaviour
{
	// Token: 0x17000099 RID: 153
	// (get) Token: 0x0600069E RID: 1694 RVA: 0x0000B40E File Offset: 0x0000960E
	// (set) Token: 0x0600069F RID: 1695 RVA: 0x0000B416 File Offset: 0x00009616
	[HideInInspector]
	public PlayerLegPadState State
	{
		get
		{
			return this.state;
		}
		set
		{
			this.OnStateChanged(this.state, value);
			this.state = value;
		}
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x0000B42C File Offset: 0x0000962C
	private void Awake()
	{
		this.localPosition = base.transform.localPosition;
		this.localRotation = base.transform.localRotation;
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x00026FA0 File Offset: 0x000251A0
	private void Update()
	{
		this.ShootLegPadRaycast();
		base.transform.localPosition = new Vector3(this.localPosition.x, this.localYPosition, this.localPosition.z);
		base.transform.localRotation = this.localRotation;
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x00006C1B File Offset: 0x00004E1B
	private void FixedUpdate()
	{
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x0000B450 File Offset: 0x00009650
	private void OnDestroy()
	{
		Tween tween = this.localPositionTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		Tween tween2 = this.localRotationTween;
		if (tween2 == null)
		{
			return;
		}
		tween2.Kill(false);
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x00026FF0 File Offset: 0x000251F0
	public void ShootLegPadRaycast()
	{
		Vector3 vector = base.transform.parent.TransformPoint(this.localPosition);
		vector.y = base.transform.parent.position.y;
		vector += base.transform.parent.up;
		Vector3 vector2 = -base.transform.parent.up;
		Debug.DrawRay(vector, vector2 * this.raycastDistance, Color.red);
		RaycastHit raycastHit;
		if (Physics.Raycast(vector, vector2, out raycastHit, this.raycastDistance, this.raycastLayerMask))
		{
			this.localYPosition = base.transform.parent.InverseTransformPoint(raycastHit.point).y + this.localPosition.y;
			return;
		}
		this.localYPosition = base.transform.parent.InverseTransformPoint(vector + vector2 * this.raycastDistance).y + this.localPosition.y;
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x000270F8 File Offset: 0x000252F8
	private void OnStateChanged(PlayerLegPadState oldState, PlayerLegPadState newState)
	{
		Tween tween = this.localPositionTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		Tween tween2 = this.localRotationTween;
		if (tween2 != null)
		{
			tween2.Kill(false);
		}
		if (oldState == PlayerLegPadState.Butterfly && newState == PlayerLegPadState.ButterflyExtended)
		{
			this.localPosition = this.positions[oldState].localPosition;
			this.localRotation = this.positions[oldState].localRotation;
		}
		this.localPositionTween = DOTween.To(() => this.localPosition, delegate(Vector3 value)
		{
			this.localPosition = value;
		}, this.positions[newState].localPosition, this.transitionDuration);
		this.localRotationTween = DOTween.To<Quaternion, Quaternion, NoOptions>(PureQuaternionPlugin.Plug(), () => this.localRotation, delegate(Quaternion value)
		{
			this.localRotation = value;
		}, this.positions[newState].localRotation, this.transitionDuration);
	}

	// Token: 0x040003CA RID: 970
	[Header("References")]
	[SerializeField]
	private SerializedDictionary<PlayerLegPadState, Transform> positions = new SerializedDictionary<PlayerLegPadState, Transform>();

	// Token: 0x040003CB RID: 971
	[Header("Settings")]
	[SerializeField]
	private float raycastDistance = 1f;

	// Token: 0x040003CC RID: 972
	[SerializeField]
	private float transitionDuration = 0.15f;

	// Token: 0x040003CD RID: 973
	[Space(20f)]
	[SerializeField]
	private LayerMask raycastLayerMask;

	// Token: 0x040003CE RID: 974
	private PlayerLegPadState state;

	// Token: 0x040003CF RID: 975
	private Vector3 localPosition = Vector3.zero;

	// Token: 0x040003D0 RID: 976
	private float localYPosition;

	// Token: 0x040003D1 RID: 977
	private Quaternion localRotation = Quaternion.identity;

	// Token: 0x040003D2 RID: 978
	private Tween localPositionTween;

	// Token: 0x040003D3 RID: 979
	private Tween localRotationTween;
}
