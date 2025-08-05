using System;
using UnityEngine;

// Token: 0x02000037 RID: 55
public class DependencyManagerController : MonoBehaviour
{
	// Token: 0x06000185 RID: 389 RVA: 0x00007E6C File Offset: 0x0000606C
	private void Awake()
	{
		this.dependencyManager = base.GetComponent<DependencyManager>();
	}

	// Token: 0x06000186 RID: 390 RVA: 0x00006C1B File Offset: 0x00004E1B
	private void Start()
	{
	}

	// Token: 0x06000187 RID: 391 RVA: 0x00006C1B File Offset: 0x00004E1B
	private void OnDestroy()
	{
	}

	// Token: 0x040000CF RID: 207
	private DependencyManager dependencyManager;
}
