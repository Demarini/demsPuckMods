using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

// Token: 0x02000155 RID: 341
[UxmlElement]
public class ChildClassifier : VisualElement
{
	// Token: 0x06000A11 RID: 2577 RVA: 0x00030C70 File Offset: 0x0002EE70
	public ChildClassifier()
	{
		base.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.NoTrickleDown);
		base.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnDetachFromPanel), TrickleDown.NoTrickleDown);
		base.RegisterCallback<ChildAddedEvent>(new EventCallback<ChildAddedEvent>(this.OnChildAdded), TrickleDown.NoTrickleDown);
		base.RegisterCallback<BeforeChildRemovedEvent>(new EventCallback<BeforeChildRemovedEvent>(this.OnBeforeChildRemoved), TrickleDown.NoTrickleDown);
		base.RegisterCallback<HierarchyChangedEvent>(new EventCallback<HierarchyChangedEvent>(this.OnHierarchyChanged), TrickleDown.NoTrickleDown);
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x00030CE2 File Offset: 0x0002EEE2
	private List<VisualElement> GetChildren()
	{
		return base.Children().ToList<VisualElement>();
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x00030CEF File Offset: 0x0002EEEF
	private List<VisualElement> GetVisibleChildren()
	{
		return base.Children().ToList<VisualElement>().FindAll((VisualElement child) => child.resolvedStyle.display == DisplayStyle.Flex);
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x00030D20 File Offset: 0x0002EF20
	private void SetFirstChild(VisualElement child)
	{
		if (this.firstChild != null)
		{
			this.DisposeChild(this.firstChild);
		}
		this.firstChild = child;
		this.firstChild.EnableInClassList("firstChild", true);
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x00030D4E File Offset: 0x0002EF4E
	private void SetLastChild(VisualElement child)
	{
		if (this.lastChild != null)
		{
			this.DisposeChild(this.lastChild);
		}
		this.lastChild = child;
		this.lastChild.EnableInClassList("lastChild", true);
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x00030D7C File Offset: 0x0002EF7C
	private void SetChildClasses(List<VisualElement> children)
	{
		if (children.Count == 0)
		{
			return;
		}
		if (children.Count == 1)
		{
			this.SetLastChild(children[0]);
			return;
		}
		this.SetFirstChild(children[0]);
		this.SetLastChild(children[children.Count - 1]);
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x00030DCA File Offset: 0x0002EFCA
	private void DisposeChild(VisualElement child)
	{
		child.EnableInClassList("firstChild", false);
		child.EnableInClassList("lastChild", false);
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x00030DE4 File Offset: 0x0002EFE4
	private void OnAttachToPanel(AttachToPanelEvent e)
	{
		this.GetChildren().ForEach(delegate(VisualElement child)
		{
			child.RegisterCallback<RenderingToggledEvent>(new EventCallback<RenderingToggledEvent>(this.OnChildRenderingToggled), TrickleDown.NoTrickleDown);
		});
		List<VisualElement> visibleChildren = this.GetVisibleChildren();
		this.SetChildClasses(visibleChildren);
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x00030E16 File Offset: 0x0002F016
	private void OnDetachFromPanel(DetachFromPanelEvent e)
	{
		this.GetChildren().ForEach(delegate(VisualElement child)
		{
			child.UnregisterCallback<RenderingToggledEvent>(new EventCallback<RenderingToggledEvent>(this.OnChildRenderingToggled), TrickleDown.NoTrickleDown);
		});
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x00030E30 File Offset: 0x0002F030
	private void OnChildAdded(ChildAddedEvent e)
	{
		int index = e.index;
		e.child.RegisterCallback<RenderingToggledEvent>(new EventCallback<RenderingToggledEvent>(this.OnChildRenderingToggled), TrickleDown.NoTrickleDown);
		List<VisualElement> visibleChildren = this.GetVisibleChildren();
		if (index == 0 || index == visibleChildren.Count - 1)
		{
			this.SetChildClasses(visibleChildren);
		}
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x00030E78 File Offset: 0x0002F078
	private void OnBeforeChildRemoved(BeforeChildRemovedEvent e)
	{
		int index = e.index;
		VisualElement child = e.child;
		child.UnregisterCallback<RenderingToggledEvent>(new EventCallback<RenderingToggledEvent>(this.OnChildRenderingToggled), TrickleDown.NoTrickleDown);
		List<VisualElement> visibleChildren = this.GetVisibleChildren();
		if (index == 0 || index == visibleChildren.Count - 1)
		{
			List<VisualElement> childClasses = this.GetVisibleChildren().FindAll((VisualElement c) => c != child);
			this.SetChildClasses(childClasses);
		}
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x00030EEC File Offset: 0x0002F0EC
	private void OnHierarchyChanged(HierarchyChangedEvent e)
	{
		List<VisualElement> visibleChildren = this.GetVisibleChildren();
		this.SetChildClasses(visibleChildren);
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x00030F08 File Offset: 0x0002F108
	private void OnChildRenderingToggled(RenderingToggledEvent e)
	{
		List<VisualElement> visibleChildren = this.GetVisibleChildren();
		this.SetChildClasses(visibleChildren);
	}

	// Token: 0x040005D0 RID: 1488
	private VisualElement firstChild;

	// Token: 0x040005D1 RID: 1489
	private VisualElement lastChild;

	// Token: 0x02000156 RID: 342
	[CompilerGenerated]
	[Serializable]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		// Token: 0x06000A20 RID: 2592 RVA: 0x00030F4D File Offset: 0x0002F14D
		public override object CreateInstance()
		{
			return new ChildClassifier();
		}
	}
}
