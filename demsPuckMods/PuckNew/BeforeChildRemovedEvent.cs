using System;
using UnityEngine.UIElements;

// Token: 0x020000DB RID: 219
public class BeforeChildRemovedEvent : EventBase<BeforeChildRemovedEvent>
{
	// Token: 0x04000407 RID: 1031
	public int index;

	// Token: 0x04000408 RID: 1032
	public VisualElement child;
}
