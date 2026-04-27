using System;

// Token: 0x02000010 RID: 16
public class LockerRoomCamera : BaseCamera
{
	// Token: 0x0600004D RID: 77 RVA: 0x00002D32 File Offset: 0x00000F32
	public override void Awake()
	{
		base.Awake();
		this.smoothPositioner = base.GetComponent<SmoothPositioner>();
	}

	// Token: 0x0600004E RID: 78 RVA: 0x00002D46 File Offset: 0x00000F46
	public void SetPosition(string positionName)
	{
		this.smoothPositioner.SetPosition(positionName, false);
	}

	// Token: 0x06000050 RID: 80 RVA: 0x00002D60 File Offset: 0x00000F60
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000051 RID: 81 RVA: 0x00002D76 File Offset: 0x00000F76
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000052 RID: 82 RVA: 0x00002D80 File Offset: 0x00000F80
	protected internal override string __getTypeName()
	{
		return "LockerRoomCamera";
	}

	// Token: 0x04000029 RID: 41
	private SmoothPositioner smoothPositioner;
}
