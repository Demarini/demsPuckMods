using System;
using UnityEngine;

// Token: 0x0200006F RID: 111
public class SynchronizedObjectController : MonoBehaviour
{
	// Token: 0x060003AF RID: 943 RVA: 0x00015611 File Offset: 0x00013811
	private void Awake()
	{
		this.synchronizedObject = base.GetComponent<SynchronizedObject>();
	}

	// Token: 0x0400028E RID: 654
	private SynchronizedObject synchronizedObject;
}
