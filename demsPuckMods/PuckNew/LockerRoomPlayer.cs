using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;

// Token: 0x02000012 RID: 18
public class LockerRoomPlayer : MonoBehaviour
{
	// Token: 0x0600005B RID: 91 RVA: 0x00002F44 File Offset: 0x00001144
	private void Awake()
	{
		this.initialRotation = base.transform.rotation.eulerAngles;
		this.targetRotation = this.initialRotation;
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00002F76 File Offset: 0x00001176
	private void Start()
	{
		this.SetRotationFromPreset(this.defaultRotationPreset);
	}

	// Token: 0x0600005D RID: 93 RVA: 0x00002F84 File Offset: 0x00001184
	private void Update()
	{
		Vector2 vector = InputManager.PointAction.ReadValue<Vector2>();
		if (this.AllowRotation)
		{
			if (InputManager.ClickAction.WasPressedThisFrame() && !GlobalStateManager.UIState.IsMouseOverUI)
			{
				this.IsRotating = true;
				this.lastPointerPosition = vector;
			}
			else if (InputManager.ClickAction.WasReleasedThisFrame())
			{
				this.IsRotating = false;
			}
			if (this.IsRotating)
			{
				Vector2 vector2 = vector - this.lastPointerPosition;
				this.lastPointerPosition = vector;
				if (this.IsRotating)
				{
					this.targetRotation.y = this.targetRotation.y + vector2.x * this.rotationSpeed * Time.deltaTime;
				}
			}
		}
		Quaternion b = Quaternion.Euler(this.targetRotation);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime / this.rotationSmoothing);
		BaseCamera activeCamera = CameraManager.GetActiveCamera();
		if (activeCamera)
		{
			Plane plane = new Plane(activeCamera.transform.forward, activeCamera.transform.position + activeCamera.transform.forward);
			Ray ray = activeCamera.UnityCamera.ScreenPointToRay(vector);
			float distance;
			if (plane.Raycast(ray, out distance))
			{
				Vector3 point = ray.GetPoint(distance);
				Vector3 vector3 = activeCamera.transform.InverseTransformPoint(point);
				vector3.x *= Vector3.Dot(base.transform.forward, activeCamera.transform.forward);
				vector3.y += 0.5f;
				vector3.z *= 2f;
				Vector3 vector4 = base.transform.position + base.transform.right * vector3.x + base.transform.up * vector3.y + base.transform.forward * vector3.z;
				this.lookAtPosition = vector4;
			}
		}
		this.playerMesh.LookAt(this.lookAtPosition, Time.deltaTime, true, true);
	}

	// Token: 0x0600005E RID: 94 RVA: 0x0000319E File Offset: 0x0000139E
	public void SetRotationFromPreset(string name)
	{
		if (!this.rotationPresets.ContainsKey(name))
		{
			Debug.LogError("[LockerRoomPlayer] Rotation preset " + name + " does not exist");
			return;
		}
		this.targetRotation = this.rotationPresets[name];
	}

	// Token: 0x0600005F RID: 95 RVA: 0x000031D6 File Offset: 0x000013D6
	public void SetUsername(string username)
	{
		this.playerMesh.SetUsername(username);
	}

	// Token: 0x06000060 RID: 96 RVA: 0x000031E4 File Offset: 0x000013E4
	public void SetNumber(string number)
	{
		this.playerMesh.SetNumber(number);
	}

	// Token: 0x06000061 RID: 97 RVA: 0x000031F2 File Offset: 0x000013F2
	public void SetLegsPadsActive(bool isActive)
	{
		this.playerMesh.SetLegsPadsActive(isActive);
	}

	// Token: 0x06000062 RID: 98 RVA: 0x00003200 File Offset: 0x00001400
	public void SetFlagID(int flagID)
	{
		this.playerMesh.SetFlagID(flagID);
	}

	// Token: 0x06000063 RID: 99 RVA: 0x0000320E File Offset: 0x0000140E
	public void SetHeadgearID(int headgearID, PlayerRole role)
	{
		this.playerMesh.SetHeadgearID(headgearID, role);
	}

	// Token: 0x06000064 RID: 100 RVA: 0x0000321D File Offset: 0x0000141D
	public void SetMustacheID(int mustacheID)
	{
		this.playerMesh.SetMustacheID(mustacheID);
	}

	// Token: 0x06000065 RID: 101 RVA: 0x0000322B File Offset: 0x0000142B
	public void SetBeardID(int beardID)
	{
		this.playerMesh.SetBeardID(beardID);
	}

	// Token: 0x06000066 RID: 102 RVA: 0x00003239 File Offset: 0x00001439
	public void SetJerseyID(int jerseyID, PlayerTeam team)
	{
		this.playerMesh.SetJerseyID(jerseyID, team);
	}

	// Token: 0x06000067 RID: 103 RVA: 0x00003248 File Offset: 0x00001448
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.black;
		Gizmos.DrawSphere(this.lookAtPosition, 0.05f);
	}

	// Token: 0x0400002B RID: 43
	[Header("Settings")]
	[SerializeField]
	private float rotationSpeed = 10f;

	// Token: 0x0400002C RID: 44
	[SerializeField]
	private float rotationSmoothing = 0.1f;

	// Token: 0x0400002D RID: 45
	[SerializeField]
	private SerializedDictionary<string, Vector3> rotationPresets = new SerializedDictionary<string, Vector3>();

	// Token: 0x0400002E RID: 46
	[SerializeField]
	private string defaultRotationPreset = "front";

	// Token: 0x0400002F RID: 47
	[Header("References")]
	[SerializeField]
	private PlayerMesh playerMesh;

	// Token: 0x04000030 RID: 48
	[HideInInspector]
	public bool AllowRotation;

	// Token: 0x04000031 RID: 49
	[HideInInspector]
	public bool IsRotating;

	// Token: 0x04000032 RID: 50
	private Vector2 lastPointerPosition = Vector2.zero;

	// Token: 0x04000033 RID: 51
	private Vector3 initialRotation;

	// Token: 0x04000034 RID: 52
	private Vector3 targetRotation;

	// Token: 0x04000035 RID: 53
	private Vector3 lookAtPosition;
}
