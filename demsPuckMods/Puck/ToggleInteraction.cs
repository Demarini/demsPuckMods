using System;
using UnityEngine.InputSystem;

// Token: 0x02000044 RID: 68
public class ToggleInteraction : IInputInteraction
{
	// Token: 0x060001EC RID: 492 RVA: 0x0001599C File Offset: 0x00013B9C
	public void Process(ref InputInteractionContext context)
	{
		if (!context.action.IsPressed())
		{
			return;
		}
		InputActionPhase phase = context.phase;
		if (phase != InputActionPhase.Waiting)
		{
			if (phase != InputActionPhase.Started)
			{
				return;
			}
			if (this.isToggled)
			{
				this.isToggled = false;
				context.Canceled();
			}
		}
		else if (!this.isToggled)
		{
			this.isToggled = true;
			context.Started();
			return;
		}
	}

	// Token: 0x060001ED RID: 493 RVA: 0x000081B1 File Offset: 0x000063B1
	public void Reset()
	{
		this.isToggled = false;
	}

	// Token: 0x04000127 RID: 295
	private bool isToggled;
}
