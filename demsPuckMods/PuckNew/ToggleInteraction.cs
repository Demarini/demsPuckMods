using System;
using UnityEngine.InputSystem;

// Token: 0x020000B7 RID: 183
public class ToggleInteraction : IInputInteraction
{
	// Token: 0x0600059A RID: 1434 RVA: 0x0001EC24 File Offset: 0x0001CE24
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

	// Token: 0x0600059B RID: 1435 RVA: 0x0001EC7A File Offset: 0x0001CE7A
	public void Reset()
	{
		this.isToggled = false;
	}

	// Token: 0x0400038F RID: 911
	private bool isToggled;
}
