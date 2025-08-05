using System;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000D7 RID: 215
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerBodyV2))]
[RequireComponent(typeof(Hover))]
public class Movement : MonoBehaviour
{
	// Token: 0x1700008A RID: 138
	// (get) Token: 0x06000678 RID: 1656 RVA: 0x000265BC File Offset: 0x000247BC
	[HideInInspector]
	public float Speed
	{
		get
		{
			return new Vector3(this.Rigidbody.linearVelocity.x, 0f, this.Rigidbody.linearVelocity.z).magnitude;
		}
	}

	// Token: 0x1700008B RID: 139
	// (get) Token: 0x06000679 RID: 1657 RVA: 0x0000B16E File Offset: 0x0000936E
	[HideInInspector]
	public float NormalizedMaximumSpeed
	{
		get
		{
			return this.Speed / this.MaximumSpeed;
		}
	}

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x0600067A RID: 1658 RVA: 0x0000B17D File Offset: 0x0000937D
	[HideInInspector]
	public float NormalizedMinimumSpeed
	{
		get
		{
			return this.Speed / this.MinimumSpeed;
		}
	}

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x0600067B RID: 1659 RVA: 0x0000B18C File Offset: 0x0000938C
	[HideInInspector]
	public float TurnSpeed
	{
		get
		{
			return Math.Abs(base.transform.InverseTransformVector(this.Rigidbody.angularVelocity).y);
		}
	}

	// Token: 0x1700008E RID: 142
	// (get) Token: 0x0600067C RID: 1660 RVA: 0x0000B1AE File Offset: 0x000093AE
	[HideInInspector]
	public float MaximumSpeed
	{
		get
		{
			return Mathf.Max(new float[]
			{
				this.maxForwardsSpeed,
				this.maxForwardsSprintSpeed,
				this.maxBackwardsSpeed,
				this.maxBackwardsSprintSpeed
			});
		}
	}

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x0600067D RID: 1661 RVA: 0x0000B1DF File Offset: 0x000093DF
	[HideInInspector]
	public float MinimumSpeed
	{
		get
		{
			return Mathf.Min(new float[]
			{
				this.maxForwardsSpeed,
				this.maxForwardsSprintSpeed,
				this.maxBackwardsSpeed,
				this.maxBackwardsSprintSpeed
			});
		}
	}

	// Token: 0x17000090 RID: 144
	// (get) Token: 0x0600067E RID: 1662 RVA: 0x0000B210 File Offset: 0x00009410
	[HideInInspector]
	public bool IsMovingForwards
	{
		get
		{
			return this.MovementDirection.InverseTransformVector(this.Rigidbody.linearVelocity).z > 0f;
		}
	}

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x0600067F RID: 1663 RVA: 0x0000B234 File Offset: 0x00009434
	[HideInInspector]
	public bool IsMovingBackwards
	{
		get
		{
			return this.MovementDirection.InverseTransformVector(this.Rigidbody.linearVelocity).z < 0f;
		}
	}

	// Token: 0x17000092 RID: 146
	// (get) Token: 0x06000680 RID: 1664 RVA: 0x0000B258 File Offset: 0x00009458
	[HideInInspector]
	public bool IsTurningLeft
	{
		get
		{
			return base.transform.InverseTransformVector(this.Rigidbody.angularVelocity).y < 0f;
		}
	}

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x06000681 RID: 1665 RVA: 0x0000B27C File Offset: 0x0000947C
	[HideInInspector]
	public bool IsTurningRight
	{
		get
		{
			return base.transform.InverseTransformVector(this.Rigidbody.angularVelocity).y > 0f;
		}
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x0000B2A0 File Offset: 0x000094A0
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
		this.PlayerBody = base.GetComponent<PlayerBodyV2>();
		this.Hover = base.GetComponent<Hover>();
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x0000B2C6 File Offset: 0x000094C6
	private void Start()
	{
		this.currentMaxSpeed = this.maxForwardsSpeed;
		this.currentAcceleration = this.forwardsAcceleration;
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x0000B2E0 File Offset: 0x000094E0
	private void FixedUpdate()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Move();
		this.Turn();
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x000265FC File Offset: 0x000247FC
	private void Move()
	{
		if (!this.Hover.IsGrounded)
		{
			return;
		}
		if (this.IsMovingForwards)
		{
			if (this.Sprint)
			{
				this.currentMaxSpeed = this.maxForwardsSprintSpeed;
				this.currentAcceleration = ((this.Speed < this.maxForwardsSpeed) ? this.forwardsSprintAcceleration : this.forwardsSprintOverspeedAcceleration);
			}
			else
			{
				this.currentMaxSpeed = this.maxForwardsSpeed;
				this.currentAcceleration = this.forwardsAcceleration;
			}
		}
		else if (this.IsMovingBackwards)
		{
			if (this.Sprint)
			{
				this.currentMaxSpeed = this.maxBackwardsSprintSpeed;
				this.currentAcceleration = ((this.Speed < this.maxForwardsSpeed) ? this.backwardsSprintAcceleration : this.backwardsSprintOverspeedAcceleration);
			}
			else
			{
				this.currentMaxSpeed = this.maxBackwardsSpeed;
				this.currentAcceleration = this.backwardsAcceleration;
			}
		}
		if (this.MoveForwards)
		{
			if (this.IsMovingForwards)
			{
				float d = (this.Speed < this.currentMaxSpeed) ? this.currentAcceleration : 0f;
				this.Rigidbody.AddForce(this.MovementDirection.forward * d, ForceMode.Acceleration);
			}
			else if (this.IsMovingBackwards)
			{
				float d2 = this.brakeAcceleration;
				this.Rigidbody.AddForce(this.MovementDirection.forward * d2, ForceMode.Acceleration);
			}
		}
		else if (this.MoveBackwards)
		{
			if (this.IsMovingBackwards)
			{
				float d3 = (this.Speed < this.currentMaxSpeed) ? this.currentAcceleration : 0f;
				this.Rigidbody.AddForce(-this.MovementDirection.forward * d3, ForceMode.Acceleration);
			}
			else if (this.IsMovingForwards)
			{
				float d4 = this.brakeAcceleration;
				this.Rigidbody.AddForce(-this.MovementDirection.forward * d4, ForceMode.Acceleration);
			}
		}
		if (this.Speed > this.MaximumSpeed)
		{
			this.Rigidbody.linearVelocity *= 1f - this.overspeedDrag * Time.fixedDeltaTime;
		}
		else
		{
			this.Rigidbody.linearVelocity *= 1f - this.drag * Time.fixedDeltaTime;
		}
		this.Rigidbody.linearVelocity *= 1f - this.AmbientDrag * Time.fixedDeltaTime;
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x00026854 File Offset: 0x00024A54
	private void Turn()
	{
		if (this.TurnLeft)
		{
			if (this.IsTurningLeft)
			{
				float num = (this.TurnSpeed < this.turnMaxSpeed * this.TurnMultiplier) ? this.turnAcceleration : 0f;
				this.Rigidbody.AddTorque(base.transform.up * -num * this.TurnMultiplier, ForceMode.Acceleration);
			}
			else if (this.IsTurningRight)
			{
				float num2 = this.turnBrakeAcceleration;
				this.Rigidbody.AddTorque(base.transform.up * -num2 * this.TurnMultiplier, ForceMode.Acceleration);
			}
		}
		else if (this.TurnRight)
		{
			if (this.IsTurningRight)
			{
				float d = (this.TurnSpeed < this.turnMaxSpeed * this.TurnMultiplier) ? this.turnAcceleration : 0f;
				this.Rigidbody.AddTorque(base.transform.up * d * this.TurnMultiplier, ForceMode.Acceleration);
			}
			else if (this.IsTurningLeft)
			{
				float d2 = this.turnBrakeAcceleration;
				this.Rigidbody.AddTorque(base.transform.up * d2 * this.TurnMultiplier, ForceMode.Acceleration);
			}
		}
		else if (this.TurnSpeed < this.turnMaxSpeed * this.TurnMultiplier)
		{
			this.Rigidbody.angularVelocity *= 1f - this.turnDrag * Time.fixedDeltaTime;
		}
		if (this.TurnSpeed > this.turnMaxSpeed * this.TurnMultiplier)
		{
			this.Rigidbody.angularVelocity *= 1f - this.turnOverspeedDrag * Time.fixedDeltaTime;
		}
	}

	// Token: 0x04000393 RID: 915
	[Header("Settings")]
	[SerializeField]
	private float forwardsAcceleration = 2f;

	// Token: 0x04000394 RID: 916
	[SerializeField]
	private float forwardsSprintAcceleration = 4.75f;

	// Token: 0x04000395 RID: 917
	[SerializeField]
	private float forwardsSprintOverspeedAcceleration = 1f;

	// Token: 0x04000396 RID: 918
	[SerializeField]
	private float backwardsAcceleration = 1.8f;

	// Token: 0x04000397 RID: 919
	[SerializeField]
	private float backwardsSprintAcceleration = 2f;

	// Token: 0x04000398 RID: 920
	[SerializeField]
	private float backwardsSprintOverspeedAcceleration = 1f;

	// Token: 0x04000399 RID: 921
	[SerializeField]
	private float brakeAcceleration = 5f;

	// Token: 0x0400039A RID: 922
	[SerializeField]
	private float drag = 0.025f;

	// Token: 0x0400039B RID: 923
	[SerializeField]
	private float overspeedDrag = 0.025f;

	// Token: 0x0400039C RID: 924
	[Space(20f)]
	[SerializeField]
	private float maxForwardsSpeed = 7.5f;

	// Token: 0x0400039D RID: 925
	[SerializeField]
	private float maxForwardsSprintSpeed = 8.75f;

	// Token: 0x0400039E RID: 926
	[SerializeField]
	private float maxBackwardsSpeed = 7.25f;

	// Token: 0x0400039F RID: 927
	[SerializeField]
	private float maxBackwardsSprintSpeed = 7.25f;

	// Token: 0x040003A0 RID: 928
	[Space(20f)]
	[SerializeField]
	private float turnAcceleration = 1.625f;

	// Token: 0x040003A1 RID: 929
	[SerializeField]
	private float turnBrakeAcceleration = 3.25f;

	// Token: 0x040003A2 RID: 930
	[SerializeField]
	private float turnMaxSpeed = 1.375f;

	// Token: 0x040003A3 RID: 931
	[SerializeField]
	private float turnDrag = 3f;

	// Token: 0x040003A4 RID: 932
	[SerializeField]
	private float turnOverspeedDrag = 2.25f;

	// Token: 0x040003A5 RID: 933
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x040003A6 RID: 934
	[HideInInspector]
	public PlayerBodyV2 PlayerBody;

	// Token: 0x040003A7 RID: 935
	[HideInInspector]
	public Hover Hover;

	// Token: 0x040003A8 RID: 936
	[HideInInspector]
	public bool MoveForwards;

	// Token: 0x040003A9 RID: 937
	[HideInInspector]
	public bool MoveBackwards;

	// Token: 0x040003AA RID: 938
	[HideInInspector]
	public bool TurnLeft;

	// Token: 0x040003AB RID: 939
	[HideInInspector]
	public bool TurnRight;

	// Token: 0x040003AC RID: 940
	[HideInInspector]
	public float TurnMultiplier;

	// Token: 0x040003AD RID: 941
	[HideInInspector]
	public bool Sprint;

	// Token: 0x040003AE RID: 942
	[HideInInspector]
	public float AmbientDrag;

	// Token: 0x040003AF RID: 943
	[HideInInspector]
	public Transform MovementDirection;

	// Token: 0x040003B0 RID: 944
	private float currentMaxSpeed;

	// Token: 0x040003B1 RID: 945
	private float currentAcceleration;
}
