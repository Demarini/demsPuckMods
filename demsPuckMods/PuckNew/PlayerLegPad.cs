using System;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using DG.Tweening.CustomPlugins;
using DG.Tweening.Plugins.Options;
using UnityEngine;

// Token: 0x0200002F RID: 47
public class PlayerLegPad : MonoBehaviour
{
	// Token: 0x17000013 RID: 19
	// (get) Token: 0x060000EA RID: 234 RVA: 0x00004E8C File Offset: 0x0000308C
	// (set) Token: 0x060000EB RID: 235 RVA: 0x00004E94 File Offset: 0x00003094
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

	// Token: 0x060000EC RID: 236 RVA: 0x00004EAA File Offset: 0x000030AA
	private void Awake()
	{
		this.localPosition = base.transform.localPosition;
		this.localRotation = base.transform.localRotation;
	}

	// Token: 0x060000ED RID: 237 RVA: 0x00004ED0 File Offset: 0x000030D0
	private void Update()
	{
		this.ShootLegPadRaycast();
		base.transform.localPosition = new Vector3(this.localPosition.x, this.localYPosition, this.localPosition.z);
		base.transform.localRotation = this.localRotation;
	}

	// Token: 0x060000EE RID: 238 RVA: 0x000020D3 File Offset: 0x000002D3
	private void FixedUpdate()
	{
	}

	// Token: 0x060000EF RID: 239 RVA: 0x00004F20 File Offset: 0x00003120
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

	// Token: 0x060000F0 RID: 240 RVA: 0x00004F48 File Offset: 0x00003148
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

	// Token: 0x060000F1 RID: 241 RVA: 0x00005050 File Offset: 0x00003250
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

	// Token: 0x040000A4 RID: 164
	[Header("Settings")]
	[SerializeField]
	private float raycastDistance = 1f;

	// Token: 0x040000A5 RID: 165
	[SerializeField]
	private float transitionDuration = 0.15f;

	// Token: 0x040000A6 RID: 166
	[Space(20f)]
	[SerializeField]
	private LayerMask raycastLayerMask;

	// Token: 0x040000A7 RID: 167
	[Header("References")]
	[SerializeField]
	private SerializedDictionary<PlayerLegPadState, Transform> positions = new SerializedDictionary<PlayerLegPadState, Transform>();

	// Token: 0x040000A8 RID: 168
	private PlayerLegPadState state;

	// Token: 0x040000A9 RID: 169
	private Vector3 localPosition = Vector3.zero;

	// Token: 0x040000AA RID: 170
	private float localYPosition;

	// Token: 0x040000AB RID: 171
	private Quaternion localRotation = Quaternion.identity;

	// Token: 0x040000AC RID: 172
	private Tween localPositionTween;

	// Token: 0x040000AD RID: 173
	private Tween localRotationTween;
}
