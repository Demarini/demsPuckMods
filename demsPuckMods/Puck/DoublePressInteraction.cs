using System;
using UnityEngine.InputSystem;

// Token: 0x0200003F RID: 63
public class DoublePressInteraction : IInputInteraction
{
	// Token: 0x060001C9 RID: 457 RVA: 0x00014574 File Offset: 0x00012774
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

	// Token: 0x060001CA RID: 458 RVA: 0x000080FB File Offset: 0x000062FB
	public void Reset()
	{
		this.released = false;
	}

	// Token: 0x040000EE RID: 238
	public float maxTapDuration = 0.2f;

	// Token: 0x040000EF RID: 239
	public float pressThreshold = 0.5f;

	// Token: 0x040000F0 RID: 240
	public float releaseThreshold = 0.5f;

	// Token: 0x040000F1 RID: 241
	private bool released;
}
