using System;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

// Token: 0x020000B9 RID: 185
public static class ItemManager
{
	// Token: 0x1700009E RID: 158
	// (get) Token: 0x060005B8 RID: 1464 RVA: 0x0001EE5B File Offset: 0x0001D05B
	// (set) Token: 0x060005B9 RID: 1465 RVA: 0x0001EE62 File Offset: 0x0001D062
	public static List<Item> Items { get; private set; } = new List<Item>();

	// Token: 0x060005BA RID: 1466 RVA: 0x0001EE6A File Offset: 0x0001D06A
	static ItemManager()
	{
		ItemManager.LoadItems();
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x0001EE7B File Offset: 0x0001D07B
	public static void Initialize()
	{
		ItemManagerController.Initialize();
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x0001EE82 File Offset: 0x0001D082
	public static void Dispose()
	{
		ItemManagerController.Dispose();
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x0001EE8C File Offset: 0x0001D08C
	public static Item GetItemById(int id)
	{
		return ItemManager.Items.Find((Item item) => item.id == id);
	}

	// Token: 0x060005BE RID: 1470 RVA: 0x0001EEBC File Offset: 0x0001D0BC
	public static List<Item> GetItemsByCategories(string[] categories)
	{
		Predicate<string> <>9__1;
		return ItemManager.Items.FindAll(delegate(Item item)
		{
			string[] categories2 = item.categories;
			Predicate<string> match;
			if ((match = <>9__1) == null)
			{
				match = (<>9__1 = ((string itemCategory) => Array.IndexOf<string>(categories, itemCategory) >= 0));
			}
			return Array.Exists<string>(categories2, match);
		});
	}

	// Token: 0x060005BF RID: 1471 RVA: 0x0001EEEC File Offset: 0x0001D0EC
	private static void LoadItems()
	{
		try
		{
			ItemManager.Items = JsonSerializer.Deserialize<List<Item>>(Resources.Load<TextAsset>("items").text, null);
			Debug.Log(string.Format("[ItemManager] Loaded {0} items", ItemManager.Items.Count));
		}
		catch (Exception ex)
		{
			Debug.LogError("[ItemManager] Error loading items asset: " + ex.Message);
		}
	}
}
