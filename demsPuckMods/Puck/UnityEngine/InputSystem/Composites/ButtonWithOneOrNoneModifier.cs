using System;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Composites
{
	// Token: 0x02000176 RID: 374
	[DisplayStringFormat("{modifier}+{button}")]
	public class ButtonWithOneOrNoneModifier : InputBindingComposite<float>
	{
		// Token: 0x06000CB4 RID: 3252 RVA: 0x0000F4E1 File Offset: 0x0000D6E1
		public override float ReadValue(ref InputBindingCompositeContext context)
		{
			if (this.ModifierIsPressed(ref context) || this.modifier == 0)
			{
				return context.ReadValue<float>(this.button);
			}
			return 0f;
		}

		// Token: 0x06000CB5 RID: 3253 RVA: 0x0004C010 File Offset: 0x0004A210
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

		// Token: 0x06000CB6 RID: 3254 RVA: 0x0000F506 File Offset: 0x0000D706
		public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
		{
			return this.ReadValue(ref context);
		}

		// Token: 0x04000761 RID: 1889
		[InputControl]
		public int modifier;

		// Token: 0x04000762 RID: 1890
		[InputControl]
		public int button;

		// Token: 0x04000763 RID: 1891
		public bool ignoreOrder;
	}
}
