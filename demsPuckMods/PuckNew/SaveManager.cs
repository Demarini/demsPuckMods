using System;
using DG.Tweening;
using UnityEngine;

// Token: 0x0200010B RID: 267
public static class SaveManager
{
	// Token: 0x06000740 RID: 1856 RVA: 0x000020D3 File Offset: 0x000002D3
	public static void Initialize()
	{
	}

	// Token: 0x06000741 RID: 1857 RVA: 0x000242C9 File Offset: 0x000224C9
	public static void Dispose()
	{
		Tween tween = SaveManager.saveDebounceTween;
		if (tween == null)
		{
			return;
		}
		tween.Kill(false);
	}

	// Token: 0x06000742 RID: 1858 RVA: 0x000242DB File Offset: 0x000224DB
	public static void SetBool(string key, bool value)
	{
		PlayerPrefs.SetInt(key, value ? 1 : 0);
		SaveManager.Save();
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x000242F0 File Offset: 0x000224F0
	public static bool GetBool(string key, bool defaultValue)
	{
		int defaultValue2 = defaultValue ? 1 : 0;
		return PlayerPrefs.GetInt(key, defaultValue2) == 1;
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x0002430F File Offset: 0x0002250F
	public static void SetInt(string key, int value)
	{
		PlayerPrefs.SetInt(key, value);
		SaveManager.Save();
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x0002431D File Offset: 0x0002251D
	public static int GetInt(string key, int defaultValue)
	{
		return PlayerPrefs.GetInt(key, defaultValue);
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x00024326 File Offset: 0x00022526
	public static void SetFloat(string key, float value)
	{
		PlayerPrefs.SetFloat(key, value);
		SaveManager.Save();
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x00024334 File Offset: 0x00022534
	public static float GetFloat(string key, float defaultValue)
	{
		return PlayerPrefs.GetFloat(key, defaultValue);
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x0002433D File Offset: 0x0002253D
	public static void SetString(string key, string value)
	{
		PlayerPrefs.SetString(key, value);
		SaveManager.Save();
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x0002434B File Offset: 0x0002254B
	public static string GetString(string key, string defaultValue)
	{
		return PlayerPrefs.GetString(key, defaultValue);
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x00024354 File Offset: 0x00022554
	public static void SetEnum<T>(string key, T value) where T : Enum
	{
		PlayerPrefs.SetInt(key, Convert.ToInt32(value));
		SaveManager.Save();
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x0002436C File Offset: 0x0002256C
	public static T GetEnum<T>(string key, T defaultValue) where T : Enum
	{
		int defaultValue2 = Convert.ToInt32(defaultValue);
		int @int = PlayerPrefs.GetInt(key, defaultValue2);
		return (!!0)((object)Enum.ToObject(typeof(!!0), @int));
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x000243A4 File Offset: 0x000225A4
	private static void Save()
	{
		Tween tween = SaveManager.saveDebounceTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		SaveManager.saveDebounceTween = DOVirtual.DelayedCall(0f, delegate
		{
			PlayerPrefs.Save();
		}, true);
	}

	// Token: 0x04000477 RID: 1143
	private static Tween saveDebounceTween;
}
