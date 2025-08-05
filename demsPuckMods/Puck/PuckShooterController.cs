using System;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class PuckShooterController : MonoBehaviour
{
	// Token: 0x060000E5 RID: 229 RVA: 0x0000768D File Offset: 0x0000588D
	private void Awake()
	{
		this.puckShooter = base.GetComponent<PuckShooter>();
	}

	// Token: 0x04000068 RID: 104
	private PuckShooter puckShooter;
}
