using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

// Token: 0x02000233 RID: 563
public static class UIUtils
{
	// Token: 0x06000FA6 RID: 4006 RVA: 0x00044E1C File Offset: 0x0004301C
	public static void SetTeamClass(VisualElement element, PlayerTeam team)
	{
		foreach (object obj in Enum.GetValues(typeof(PlayerTeam)))
		{
			PlayerTeam team2 = (PlayerTeam)obj;
			element.EnableInClassList(UIUtils.GetClassFromTeam(team2), false);
		}
		element.EnableInClassList(UIUtils.GetClassFromTeam(team), true);
	}

	// Token: 0x06000FA7 RID: 4007 RVA: 0x00044E94 File Offset: 0x00043094
	public static string GetClassFromTeam(PlayerTeam team)
	{
		return "team" + team.ToString();
	}

	// Token: 0x06000FA8 RID: 4008 RVA: 0x00044EB0 File Offset: 0x000430B0
	public static List<VisualElement> GetVisualElementChildren(VisualElement element, bool recursive = false)
	{
		if (recursive)
		{
			List<VisualElement> list = new List<VisualElement>();
			foreach (VisualElement visualElement in element.hierarchy.Children())
			{
				list.Add(visualElement);
				list.AddRange(UIUtils.GetVisualElementChildren(visualElement, true));
			}
			return list;
		}
		return new List<VisualElement>(element.hierarchy.Children());
	}
}
