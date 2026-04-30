using System;
using System.Text.Json.Serialization;
using UnityEngine.InputSystem;

// Token: 0x020000AD RID: 173
public class KeyBind
{
	// Token: 0x17000086 RID: 134
	// (get) Token: 0x0600056E RID: 1390 RVA: 0x0001D994 File Offset: 0x0001BB94
	[JsonIgnore]
	public bool IsComposite
	{
		get
		{
			return this.InputAction.bindings[0].isComposite;
		}
	}

	// Token: 0x17000087 RID: 135
	// (get) Token: 0x0600056F RID: 1391 RVA: 0x0001D9BD File Offset: 0x0001BBBD
	// (set) Token: 0x06000570 RID: 1392 RVA: 0x0001D9C5 File Offset: 0x0001BBC5
	public string ModifierPath { get; set; }

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x06000571 RID: 1393 RVA: 0x0001D9CE File Offset: 0x0001BBCE
	// (set) Token: 0x06000572 RID: 1394 RVA: 0x0001D9D6 File Offset: 0x0001BBD6
	public string Path { get; set; }

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x06000573 RID: 1395 RVA: 0x0001D9DF File Offset: 0x0001BBDF
	// (set) Token: 0x06000574 RID: 1396 RVA: 0x0001D9E7 File Offset: 0x0001BBE7
	public string Interactions { get; set; }

	// Token: 0x06000575 RID: 1397 RVA: 0x000023EE File Offset: 0x000005EE
	[JsonConstructor]
	public KeyBind()
	{
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x0001D9F0 File Offset: 0x0001BBF0
	public KeyBind(InputAction inputAction)
	{
		this.InputAction = inputAction;
		this.Update(this.InputAction);
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x0001DA0C File Offset: 0x0001BC0C
	public void Update(InputAction inputAction)
	{
		this.InputAction = inputAction;
		this.ModifierPath = (this.IsComposite ? inputAction.bindings[1].effectivePath : null);
		this.Path = (this.IsComposite ? inputAction.bindings[2].effectivePath : inputAction.bindings[0].effectivePath);
		this.Interactions = inputAction.bindings[0].effectiveInteractions;
	}

	// Token: 0x0400034F RID: 847
	[JsonIgnore]
	public InputAction InputAction;
}
