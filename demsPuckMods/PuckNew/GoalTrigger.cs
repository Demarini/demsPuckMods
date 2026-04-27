using System;
using UnityEngine;

// Token: 0x0200000C RID: 12
public class GoalTrigger : MonoBehaviour
{
	// Token: 0x06000038 RID: 56 RVA: 0x00002A0C File Offset: 0x00000C0C
	private void OnTriggerEnter(Collider collider)
	{
		Puck componentInParent = collider.GetComponentInParent<Puck>();
		if (!componentInParent)
		{
			return;
		}
		this.goal.Server_OnPuckEnterGoal(componentInParent);
	}

	// Token: 0x0400001E RID: 30
	[Header("References")]
	[SerializeField]
	private Goal goal;
}
