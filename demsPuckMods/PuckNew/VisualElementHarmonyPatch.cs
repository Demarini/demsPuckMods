using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x020000DD RID: 221
public static class VisualElementHarmonyPatch
{
	// Token: 0x060006A2 RID: 1698 RVA: 0x000215A4 File Offset: 0x0001F7A4
	private static bool IsEditor(VisualElement element)
	{
		return element != null && element.panel != null && element.panel.contextType == ContextType.Editor;
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x000215C3 File Offset: 0x0001F7C3
	public static void Patch()
	{
		Debug.Log("[VisualElementHarmonyPatch] Applying patches");
		VisualElementHarmonyPatch.harmony.PatchAll();
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x000215D9 File Offset: 0x0001F7D9
	public static void Unpatch()
	{
		Debug.Log("[VisualElementHarmonyPatch] Removing patches");
		VisualElementHarmonyPatch.harmony.UnpatchSelf();
	}

	// Token: 0x0400040A RID: 1034
	private static readonly Harmony harmony = new Harmony("Puck.VisualElement");

	// Token: 0x020000DE RID: 222
	[HarmonyPatch(typeof(VisualElement), "IncrementVersion")]
	private static class IncrementVersionPatch
	{
		// Token: 0x060006A6 RID: 1702 RVA: 0x00021600 File Offset: 0x0001F800
		[HarmonyPostfix]
		public static void Postfix(VisualElement __instance, VersionChangeType changeType)
		{
			if (VisualElementHarmonyPatch.IsEditor(__instance))
			{
				return;
			}
			if (changeType == VersionChangeType.Hierarchy)
			{
				HierarchyChangedEvent pooled = EventBase<HierarchyChangedEvent>.GetPooled();
				pooled.target = __instance;
				__instance.SendEvent(pooled);
				return;
			}
			if (changeType == VersionChangeType.DisableRendering)
			{
				RenderingToggledEvent pooled2 = EventBase<RenderingToggledEvent>.GetPooled();
				pooled2.target = __instance;
				__instance.SendEvent(pooled2);
				return;
			}
		}
	}

	// Token: 0x020000DF RID: 223
	[HarmonyPatch(typeof(VisualElement.Hierarchy), "PutChildAtIndex")]
	private static class PutChildAtIndexPatch
	{
		// Token: 0x060006A7 RID: 1703 RVA: 0x0002164C File Offset: 0x0001F84C
		[HarmonyPostfix]
		public static void Postfix(VisualElement.Hierarchy __instance, VisualElement child, int index)
		{
			FieldInfo fieldInfo = AccessTools.Field(typeof(VisualElement.Hierarchy), "m_Owner");
			VisualElement visualElement = ((fieldInfo != null) ? fieldInfo.GetValue(__instance) : null) as VisualElement;
			if (VisualElementHarmonyPatch.IsEditor(visualElement))
			{
				return;
			}
			ChildAddedEvent pooled = EventBase<ChildAddedEvent>.GetPooled();
			pooled.index = index;
			pooled.child = child;
			pooled.target = visualElement;
			visualElement.SendEvent(pooled);
		}
	}

	// Token: 0x020000E0 RID: 224
	[HarmonyPatch(typeof(VisualElement.Hierarchy), "RemoveChildAtIndex")]
	private static class RemoveChildAtIndexPatch
	{
		// Token: 0x060006A8 RID: 1704 RVA: 0x000216B0 File Offset: 0x0001F8B0
		[HarmonyPrefix]
		public static bool Prefix(VisualElement.Hierarchy __instance, int index)
		{
			FieldInfo fieldInfo = AccessTools.Field(typeof(VisualElement.Hierarchy), "m_Owner");
			VisualElement visualElement = ((fieldInfo != null) ? fieldInfo.GetValue(__instance) : null) as VisualElement;
			if (VisualElementHarmonyPatch.IsEditor(visualElement))
			{
				return true;
			}
			try
			{
				BeforeChildRemovedEvent pooled = EventBase<BeforeChildRemovedEvent>.GetPooled();
				pooled.index = index;
				pooled.child = visualElement.ElementAt(index);
				pooled.target = visualElement;
				visualElement.SendEvent(pooled);
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("[VisualElementHarmonyPatch] Exception in BeforeChildRemovedEvent on {0}: {1}", ((visualElement != null) ? visualElement.name : null) ?? "null", arg));
			}
			return true;
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x00021758 File Offset: 0x0001F958
		[HarmonyPostfix]
		public static void Postfix(VisualElement.Hierarchy __instance, int index)
		{
			FieldInfo fieldInfo = AccessTools.Field(typeof(VisualElement.Hierarchy), "m_Owner");
			VisualElement visualElement = ((fieldInfo != null) ? fieldInfo.GetValue(__instance) : null) as VisualElement;
			if (VisualElementHarmonyPatch.IsEditor(visualElement))
			{
				return;
			}
			ChildRemovedEvent pooled = EventBase<ChildRemovedEvent>.GetPooled();
			pooled.index = index;
			pooled.target = visualElement;
			visualElement.SendEvent(pooled);
		}
	}
}
