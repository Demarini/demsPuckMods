using System;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Composites
{
	// Token: 0x02000239 RID: 569
	[DisplayStringFormat("{modifier}+{button}")]
	public class ButtonWithOneOrNoneModifier : InputBindingComposite<float>
	{
		// Token: 0x06000FE7 RID: 4071 RVA: 0x00045FC6 File Offset: 0x000441C6
		public override float ReadValue(ref InputBindingCompositeContext context)
		{
			if (this.ModifierIsPressed(ref context) || this.modifier == 0)
			{
				return context.ReadValue<float>(this.button);
			}
			return 0f;
		}

		// Token: 0x06000FE8 RID: 4072 RVA: 0x00045FEC File Offset: 0x000441EC
		private bool ModifierIsPressed(ref InputBindingCompositeContext context)
		{
			bool flag = context.ReadValueAsButton(this.modifier);
			if (flag && !this.ignoreOrder)
			{
				double pressTime = context.GetPressTime(this.button);
				return context.GetPressTime(this.modifier) <= pressTime;
			}
			return flag;
		}

		// Token: 0x06000FE9 RID: 4073 RVA: 0x00046032 File Offset: 0x00044232
		public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
		{
			return this.ReadValue(ref context);
		}

		// Token: 0x04000952 RID: 2386
		[InputControl]
		public int modifier;

		// Token: 0x04000953 RID: 2387
		[InputControl]
		public int button;

		// Token: 0x04000954 RID: 2388
		public bool ignoreOrder;
	}
}
