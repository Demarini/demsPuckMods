using System;
using System.Collections.Generic;

// Token: 0x02000166 RID: 358
public static class SortedListExtensions
{
	// Token: 0x06000C62 RID: 3170 RVA: 0x00041C44 File Offset: 0x0003FE44
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
