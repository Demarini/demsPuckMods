using System;
using UnityEngine.InputSystem;

// Token: 0x020000AC RID: 172
public class DoublePressInteraction : IInputInteraction
{
	// Token: 0x0600056B RID: 1387 RVA: 0x0001D8E0 File Offset: 0x0001BAE0
	public void Process(ref InputInteractionContext context)
	{
		if (context.timerHasExpired)
		{
			context.Canceled();
			return;
		}
		InputActionPhase phase = context.phase;
		if (phase != InputActionPhase.Waiting)
		{
			if (phase != InputActionPhase.Started)
			{
				return;
			}
			if (this.released)
			{
				if (context.ReadValue<float>() > this.pressThreshold)
				{
					context.Performed();
					return;
				}
			}
			else if (context.ReadValue<float>() < this.releaseThreshold)
			{
				this.released = true;
			}
		}
		else if (context.ReadValue<float>() > this.pressThreshold)
		{
			context.Started();
			context.SetTimeout(this.maxTapDuration);
			return;
		}
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x0001D95F File Offset: 0x0001BB5F
	public void Reset()
	{
		this.released = false;
	}

	// Token: 0x0400034B RID: 843
	public float maxTapDuration = 0.2f;

	// Token: 0x0400034C RID: 844
	public float pressThreshold = 0.5f;

	// Token: 0x0400034D RID: 845
	public float releaseThreshold = 0.5f;

	// Token: 0x0400034E RID: 846
	private bool released;
}
