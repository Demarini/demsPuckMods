using System;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000020 RID: 32
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerBody))]
[RequireComponent(typeof(Hover))]
public class Movement : MonoBehaviour
{
	// Token: 0x17000008 RID: 8
	// (get) Token: 0x060000BB RID: 187 RVA: 0x00004344 File Offset: 0x00002544
	[HideInInspector]
	public float Speed
	{
		get
		{
			return new Vector3(this.Rigidbody.linearVelocity.x, 0f, this.Rigidbody.linearVelocity.z).magnitude;
		}
	}

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x060000BC RID: 188 RVA: 0x00004383 File Offset: 0x00002583
	[HideInInspector]
	public float NormalizedMaximumSpeed
	{
		get
		{
			return this.Speed / this.MaximumSpeed;
		}
	}

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x060000BD RID: 189 RVA: 0x00004392 File Offset: 0x00002592
	[HideInInspector]
	public float NormalizedMinimumSpeed
	{
		get
		{
			return this.Speed / this.MinimumSpeed;
		}
	}

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x060000BE RID: 190 RVA: 0x000043A1 File Offset: 0x000025A1
	[HideInInspector]
	public float TurnSpeed
	{
		get
		{
			return Math.Abs(base.transform.InverseTransformVector(this.Rigidbody.angularVelocity).y);
		}
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x060000BF RID: 191 RVA: 0x000043C3 File Offset: 0x000025C3
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

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x060000C0 RID: 192 RVA: 0x000043F4 File Offset: 0x000025F4
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

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x060000C1 RID: 193 RVA: 0x00004425 File Offset: 0x00002625
	[HideInInspector]
	public bool IsMovingForwards
	{
		get
		{
			return this.MovementDirection.InverseTransformVector(this.Rigidbody.linearVelocity).z > 0f;
		}
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x060000C2 RID: 194 RVA: 0x00004449 File Offset: 0x00002649
	[HideInInspector]
	public bool IsMovingBackwards
	{
		get
		{
			return this.MovementDirection.InverseTransformVector(this.Rigidbody.linearVelocity).z < 0f;
		}
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x060000C3 RID: 195 RVA: 0x0000446D File Offset: 0x0000266D
	[HideInInspector]
	public bool IsTurningLeft
	{
		get
		{
			return base.transform.InverseTransformVector(this.Rigidbody.angularVelocity).y < 0f;
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x060000C4 RID: 196 RVA: 0x00004491 File Offset: 0x00002691
	[HideInInspector]
	public bool IsTurningRight
	{
		get
		{
			return base.transform.InverseTransformVector(this.Rigidbody.angularVelocity).y > 0f;
		}
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x000044B5 File Offset: 0x000026B5
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
		this.PlayerBody = base.GetComponent<PlayerBody>();
		this.Hover = base.GetComponent<Hover>();
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x000044DB File Offset: 0x000026DB
	private void Start()
	{
		this.currentMaxSpeed = this.maxForwardsSpeed;
		this.currentAcceleration = this.forwardsAcceleration;
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x000044F5 File Offset: 0x000026F5
	private void FixedUpdate()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Move();
		this.Turn();
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x00004510 File Offset: 0x00002710
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

	// Token: 0x060000C9 RID: 201 RVA: 0x00004768 File Offset: 0x00002968
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

	// Token: 0x04000060 RID: 96
	[Header("Settings")]
	[SerializeField]
	private float forwardsAcceleration = 2f;

	// Token: 0x04000061 RID: 97
	[SerializeField]
	private float forwardsSprintAcceleration = 4.75f;

	// Token: 0x04000062 RID: 98
	[SerializeField]
	private float forwardsSprintOverspeedAcceleration = 1f;

	// Token: 0x04000063 RID: 99
	[SerializeField]
	private float backwardsAcceleration = 1.8f;

	// Token: 0x04000064 RID: 100
	[SerializeField]
	private float backwardsSprintAcceleration = 2f;

	// Token: 0x04000065 RID: 101
	[SerializeField]
	private float backwardsSprintOverspeedAcceleration = 1f;

	// Token: 0x04000066 RID: 102
	[SerializeField]
	private float brakeAcceleration = 5f;

	// Token: 0x04000067 RID: 103
	[SerializeField]
	private float drag = 0.025f;

	// Token: 0x04000068 RID: 104
	[SerializeField]
	private float overspeedDrag = 0.025f;

	// Token: 0x04000069 RID: 105
	[Space(20f)]
	[SerializeField]
	private float maxForwardsSpeed = 7.5f;

	// Token: 0x0400006A RID: 106
	[SerializeField]
	private float maxForwardsSprintSpeed = 8.75f;

	// Token: 0x0400006B RID: 107
	[SerializeField]
	private float maxBackwardsSpeed = 7.25f;

	// Token: 0x0400006C RID: 108
	[SerializeField]
	private float maxBackwardsSprintSpeed = 7.25f;

	// Token: 0x0400006D RID: 109
	[Space(20f)]
	[SerializeField]
	private float turnAcceleration = 1.625f;

	// Token: 0x0400006E RID: 110
	[SerializeField]
	private float turnBrakeAcceleration = 3.25f;

	// Token: 0x0400006F RID: 111
	[SerializeField]
	private float turnMaxSpeed = 1.375f;

	// Token: 0x04000070 RID: 112
	[SerializeField]
	private float turnDrag = 3f;

	// Token: 0x04000071 RID: 113
	[SerializeField]
	private float turnOverspeedDrag = 2.25f;

	// Token: 0x04000072 RID: 114
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x04000073 RID: 115
	[HideInInspector]
	public PlayerBody PlayerBody;

	// Token: 0x04000074 RID: 116
	[HideInInspector]
	public Hover Hover;

	// Token: 0x04000075 RID: 117
	[HideInInspector]
	public bool MoveForwards;

	// Token: 0x04000076 RID: 118
	[HideInInspector]
	public bool MoveBackwards;

	// Token: 0x04000077 RID: 119
	[HideInInspector]
	public bool TurnLeft;

	// Token: 0x04000078 RID: 120
	[HideInInspector]
	public bool TurnRight;

	// Token: 0x04000079 RID: 121
	[HideInInspector]
	public float TurnMultiplier;

	// Token: 0x0400007A RID: 122
	[HideInInspector]
	public bool Sprint;

	// Token: 0x0400007B RID: 123
	[HideInInspector]
	public float AmbientDrag;

	// Token: 0x0400007C RID: 124
	[HideInInspector]
	public Transform MovementDirection;

	// Token: 0x0400007D RID: 125
	private float currentMaxSpeed;

	// Token: 0x0400007E RID: 126
	private float currentAcceleration;
}
