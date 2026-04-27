using System;
using UnityEngine.UIElements;

// Token: 0x020001B5 RID: 437
internal interface IPopupContent
{
	// Token: 0x17000118 RID: 280
	// (get) Token: 0x06000C91 RID: 3217
	// (set) Token: 0x06000C92 RID: 3218
	object Data { get; set; }

	// Token: 0x06000C93 RID: 3219
	void Initialize(VisualElement viewVisualElement);

	// Token: 0x06000C94 RID: 3220
	void Dispose();
}
