using System;
using UnityEngine;

// Token: 0x0200005A RID: 90
public class PuckPositionController : MonoBehaviour
{
	// Token: 0x06000308 RID: 776 RVA: 0x00012BC1 File Offset: 0x00010DC1
	private void Awake()
	{
		this.puckPosition = base.GetComponent<PuckPosition>();
	}

	// Token: 0x04000213 RID: 531
	private PuckPosition puckPosition;
}
