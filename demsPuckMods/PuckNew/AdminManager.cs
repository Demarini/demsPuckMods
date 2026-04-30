using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using UnityEngine;

// Token: 0x0200010F RID: 271
public class AdminManager : MonoBehaviourSingleton<AdminManager>
{
	// Token: 0x06000763 RID: 1891 RVA: 0x00024808 File Offset: 0x00022A08
	public override void Awake()
	{
		base.Awake();
		this.adminSteamIdsFilePath = (Utils.GetCommandLineArgument("--adminSteamIdsPath", null) ?? "./admin_steam_ids.json");
		this.adminSteamIdsFilePath = Path.GetFullPath(this.adminSteamIdsFilePath);
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x0002483B File Offset: 0x00022A3B
	public void Dispose()
	{
		this.adminSteamIds.Clear();
		if (this.adminSteamIdsWatcher != null)
		{
			this.adminSteamIdsWatcher.Dispose();
			this.adminSteamIdsWatcher = null;
		}
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x00024864 File Offset: 0x00022A64
	public void LoadAdminSteamIds()
	{
		if (!File.Exists(this.adminSteamIdsFilePath))
		{
			Debug.LogWarning("[AdminManager] Admin steam ids file not found at " + this.adminSteamIdsFilePath + ", creating default...");
			File.AppendAllText(this.adminSteamIdsFilePath, JsonSerializer.Serialize<List<string>>(new List<string>(), new JsonSerializerOptions
			{
				WriteIndented = true
			}));
		}
		this.ReadAdminSteamIds();
		this.WatchAdminSteamIds(this.adminSteamIdsFilePath);
	}

	// Token: 0x06000766 RID: 1894 RVA: 0x000248CB File Offset: 0x00022ACB
	public void SaveAdminSteamIds()
	{
		File.WriteAllText(this.adminSteamIdsFilePath, JsonSerializer.Serialize<List<string>>(this.adminSteamIds, new JsonSerializerOptions
		{
			WriteIndented = true
		}));
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x000248F0 File Offset: 0x00022AF0
	public void ReadAdminSteamIds()
	{
		string json = File.ReadAllText(this.adminSteamIdsFilePath);
		this.adminSteamIds = JsonSerializer.Deserialize<List<string>>(json, null);
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x00024918 File Offset: 0x00022B18
	public void WatchAdminSteamIds(string adminSteamIdsFilePath)
	{
		if (this.adminSteamIdsWatcher != null)
		{
			return;
		}
		string fileName = Path.GetFileName(adminSteamIdsFilePath);
		string path = adminSteamIdsFilePath.Replace(fileName, string.Empty);
		this.adminSteamIdsWatcher = new FileSystemWatcher(path);
		this.adminSteamIdsWatcher.NotifyFilter = NotifyFilters.LastWrite;
		this.adminSteamIdsWatcher.Filter = fileName;
		this.adminSteamIdsWatcher.EnableRaisingEvents = true;
		this.adminSteamIdsWatcher.Changed += this.OnAdminSteamIdsFileChanged;
		Debug.Log("[AdminManager] Watching admin Steam IDs file " + fileName);
	}

	// Token: 0x06000769 RID: 1897 RVA: 0x0002499C File Offset: 0x00022B9C
	private void OnAdminSteamIdsFileChanged(object sender, FileSystemEventArgs e)
	{
		Debug.Log("[AdminManager] Admin Steam IDs file changed: " + e.FullPath);
		string json = File.ReadAllText(e.FullPath);
		this.adminSteamIds = JsonSerializer.Deserialize<List<string>>(json, null);
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x000249D7 File Offset: 0x00022BD7
	public void AddAdminSteamId(string steamId)
	{
		if (this.adminSteamIds.Contains(steamId))
		{
			return;
		}
		this.adminSteamIds.Add(steamId);
		this.SaveAdminSteamIds();
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x000249FA File Offset: 0x00022BFA
	public void RemoveAdminSteamId(string steamId)
	{
		if (!this.adminSteamIds.Contains(steamId))
		{
			return;
		}
		this.adminSteamIds.Remove(steamId);
		this.SaveAdminSteamIds();
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x00024A1E File Offset: 0x00022C1E
	public bool IsSteamIdAdmin(string steamId)
	{
		return this.adminSteamIds.Contains(steamId);
	}

	// Token: 0x0400047C RID: 1148
	private List<string> adminSteamIds = new List<string>();

	// Token: 0x0400047D RID: 1149
	private string adminSteamIdsFilePath;

	// Token: 0x0400047E RID: 1150
	private FileSystemWatcher adminSteamIdsWatcher;
}
