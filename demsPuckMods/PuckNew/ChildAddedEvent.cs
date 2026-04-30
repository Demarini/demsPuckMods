using System;
using UnityEngine.UIElements;

// Token: 0x020000DA RID: 218
public class ChildAddedEvent : EventBase<ChildAddedEvent>
{
	// Token: 0x04000405 RID: 1029
	public int index;

	// Token: 0x04000406 RID: 1030
	public VisualElement child;
}
