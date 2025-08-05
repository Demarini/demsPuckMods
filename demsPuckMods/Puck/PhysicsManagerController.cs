using System;
using UnityEngine;

// Token: 0x02000063 RID: 99
public class PhysicsManagerController : MonoBehaviour
{
	// Token: 0x060002BF RID: 703 RVA: 0x00008AD9 File Offset: 0x00006CD9
	private void Awake()
	{
		this.physicsManager = base.GetComponent<PhysicsManager>();
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x00006C1B File Offset: 0x00004E1B
	private void Start()
	{
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x00006C1B File Offset: 0x00004E1B
	private void OnDestroy()
	{
	}

	// Token: 0x04000195 RID: 405
	private PhysicsManager physicsManager;
}
