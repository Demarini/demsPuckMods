using System;
using UnityEngine;

// Token: 0x02000010 RID: 16
public class GoalTrigger : MonoBehaviour
{
	// Token: 0x06000083 RID: 131 RVA: 0x00010B50 File Offset: 0x0000ED50
	private void OnTriggerEnter(Collider collider)
	{
		Puck componentInParent = collider.GetComponentInParent<Puck>();
		if (!componentInParent)
		{
			return;
		}
		this.goal.Server_OnPuckEnterGoal(componentInParent);
	}

	// Token: 0x0400003A RID: 58
	[Header("References")]
	[SerializeField]
	private Goal goal;
}
