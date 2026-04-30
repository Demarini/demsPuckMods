using System;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

// Token: 0x020001DF RID: 479
public static class CountryUtils
{
	// Token: 0x17000124 RID: 292
	// (get) Token: 0x06000E09 RID: 3593 RVA: 0x000423CC File Offset: 0x000405CC
	// (set) Token: 0x06000E0A RID: 3594 RVA: 0x000423D3 File Offset: 0x000405D3
	public static List<Country> Countries { get; private set; } = new List<Country>();

	// Token: 0x06000E0B RID: 3595 RVA: 0x000423DB File Offset: 0x000405DB
	static CountryUtils()
	{
		CountryUtils.LoadCountries();
	}

	// Token: 0x06000E0C RID: 3596 RVA: 0x000423EC File Offset: 0x000405EC
	public static Country GetCountryByCode(string code)
	{
		return CountryUtils.Countries.Find((Country country) => country.code.Equals(code, StringComparison.OrdinalIgnoreCase));
	}

	// Token: 0x06000E0D RID: 3597 RVA: 0x0004241C File Offset: 0x0004061C
	public static Country GetCountryByName(string name)
	{
		return CountryUtils.Countries.Find((Country country) => country.name.Equals(name, StringComparison.OrdinalIgnoreCase));
	}

	// Token: 0x06000E0E RID: 3598 RVA: 0x0004244C File Offset: 0x0004064C
	private static void LoadCountries()
	{
		try
		{
			CountryUtils.Countries = JsonSerializer.Deserialize<List<Country>>(Resources.Load<TextAsset>("countries").text, null);
			Debug.Log(string.Format("[CountryUtils] Loaded {0} countries", CountryUtils.Countries.Count));
		}
		catch (Exception ex)
		{
			Debug.LogError("[CountryUtils] Error loading countries asset: " + ex.Message);
		}
	}
}
