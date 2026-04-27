using System;
using System.Collections.Generic;

// Token: 0x020001F3 RID: 499
public static class SortedListExtensions
{
	// Token: 0x06000E60 RID: 3680 RVA: 0x00042EDC File Offset: 0x000410DC
	public static void RemoveRange<T, U>(this SortedList<T, U> list, int amount)
	{
		int num = 0;
		while (num < amount && num < list.Count)
		{
			list.RemoveAt(0);
			num++;
		}
	}
}
