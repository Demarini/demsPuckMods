using System;
using UnityEngine;

// Token: 0x020000E2 RID: 226
public class PhysicsManagerController : MonoBehaviour
{
	// Token: 0x060006AF RID: 1711 RVA: 0x00021856 File Offset: 0x0001FA56
	private void Awake()
	{
		this.physicsManager = base.GetComponent<PhysicsManager>();
	}

	// Token: 0x060006B0 RID: 1712 RVA: 0x000020D3 File Offset: 0x000002D3
	private void Start()
	{
	}

	// Token: 0x060006B1 RID: 1713 RVA: 0x000020D3 File Offset: 0x000002D3
	private void OnDestroy()
	{
	}

	// Token: 0x0400040E RID: 1038
	private PhysicsManager physicsManager;
}
