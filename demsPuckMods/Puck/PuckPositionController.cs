using System;
using UnityEngine;

// Token: 0x0200001C RID: 28
public class PuckPositionController : MonoBehaviour
{
	// Token: 0x060000C9 RID: 201 RVA: 0x0000754F File Offset: 0x0000574F
	private void Awake()
	{
		this.puckPosition = base.GetComponent<PuckPosition>();
	}

	// Token: 0x04000057 RID: 87
	private PuckPosition puckPosition;
}
