using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using UnityEngine;

// Token: 0x02000126 RID: 294
public class WhitelistManager : MonoBehaviourSingleton<WhitelistManager>
{
	// Token: 0x0600083A RID: 2106 RVA: 0x000279D2 File Offset: 0x00025BD2
	public override void Awake()
	{
		base.Awake();
		this.whitelistedSteamIdsFilePath = (Utils.GetCommandLineArgument("--whitelistedSteamIdsPath", null) ?? "./whitelisted_steam_ids.json");
		this.whitelistedSteamIdsFilePath = Path.GetFullPath(this.whitelistedSteamIdsFilePath);
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x00027A05 File Offset: 0x00025C05
	public void Dispose()
	{
		this.whitelistedSteamIds.Clear();
		if (this.whitelistedSteamIdsWatcher != null)
		{
			this.whitelistedSteamIdsWatcher.Dispose();
			this.whitelistedSteamIdsWatcher = null;
		}
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x00027A2C File Offset: 0x00025C2C
	public void LoadWhitelistedSteamIds()
	{
		if (!File.Exists(this.whitelistedSteamIdsFilePath))
		{
			Debug.LogWarning("[WhitelistManager] Whitelisted Steam IDs file not found at " + this.whitelistedSteamIdsFilePath + ", creating default...");
			File.AppendAllText(this.whitelistedSteamIdsFilePath, JsonSerializer.Serialize<List<string>>(new List<string>(), new JsonSerializerOptions
			{
				WriteIndented = true
			}));
		}
		this.ReadWhitelistedSteamIds();
		this.WatchWhitelistedSteamIds(this.whitelistedSteamIdsFilePath);
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x00027A93 File Offset: 0x00025C93
	public void SaveWhitelistedSteamIds()
	{
		File.WriteAllText(this.whitelistedSteamIdsFilePath, JsonSerializer.Serialize<List<string>>(this.whitelistedSteamIds, new JsonSerializerOptions
		{
			WriteIndented = true
		}));
	}

	// Token: 0x0600083E RID: 2110 RVA: 0x00027AB8 File Offset: 0x00025CB8
	public void ReadWhitelistedSteamIds()
	{
		string json = File.ReadAllText(this.whitelistedSteamIdsFilePath);
		this.whitelistedSteamIds = JsonSerializer.Deserialize<List<string>>(json, null);
	}

	// Token: 0x0600083F RID: 2111 RVA: 0x00027AE0 File Offset: 0x00025CE0
	public void WatchWhitelistedSteamIds(string whitelistedSteamIdsFilePath)
	{
		if (this.whitelistedSteamIdsWatcher != null)
		{
			return;
		}
		string fileName = Path.GetFileName(whitelistedSteamIdsFilePath);
		string path = whitelistedSteamIdsFilePath.Replace(fileName, string.Empty);
		this.whitelistedSteamIdsWatcher = new FileSystemWatcher(path);
		this.whitelistedSteamIdsWatcher.NotifyFilter = NotifyFilters.LastWrite;
		this.whitelistedSteamIdsWatcher.Filter = fileName;
		this.whitelistedSteamIdsWatcher.EnableRaisingEvents = true;
		this.whitelistedSteamIdsWatcher.Changed += this.OnWhitelistedSteamIdsFileChanged;
		Debug.Log("[WhitelistManager] Watching whitelisted Steam IDs file " + fileName);
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x00027B62 File Offset: 0x00025D62
	public void AddWhitelistedSteamId(string steamId)
	{
		if (this.whitelistedSteamIds.Contains(steamId))
		{
			return;
		}
		this.whitelistedSteamIds.Add(steamId);
		this.SaveWhitelistedSteamIds();
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x00027B88 File Offset: 0x00025D88
	public void AddWhitelistedSteamIds(List<string> steamIds)
	{
		bool flag = false;
		foreach (string item in steamIds)
		{
			if (!this.whitelistedSteamIds.Contains(item))
			{
				this.whitelistedSteamIds.Add(item);
				flag = true;
			}
		}
		if (flag)
		{
			this.SaveWhitelistedSteamIds();
		}
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x00027BF8 File Offset: 0x00025DF8
	public void RemoveWhitelistedSteamId(string steamId)
	{
		if (!this.whitelistedSteamIds.Contains(steamId))
		{
			return;
		}
		this.whitelistedSteamIds.Remove(steamId);
		this.SaveWhitelistedSteamIds();
	}

	// Token: 0x06000843 RID: 2115 RVA: 0x00027C1C File Offset: 0x00025E1C
	public void RemoveWhitelistedSteamIds(List<string> steamIds)
	{
		bool flag = false;
		foreach (string item in steamIds)
		{
			if (this.whitelistedSteamIds.Contains(item))
			{
				this.whitelistedSteamIds.Remove(item);
				flag = true;
			}
		}
		if (flag)
		{
			this.SaveWhitelistedSteamIds();
		}
	}

	// Token: 0x06000844 RID: 2116 RVA: 0x00027C8C File Offset: 0x00025E8C
	public bool IsSteamIdWhitelisted(string steamId)
	{
		return this.whitelistedSteamIds.Contains(steamId);
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x00027C9C File Offset: 0x00025E9C
	private void OnWhitelistedSteamIdsFileChanged(object sender, FileSystemEventArgs e)
	{
		Debug.Log("[WhitelistManager] Whitelisted Steam IDs file changed: " + e.FullPath);
		string json = File.ReadAllText(e.FullPath);
		this.whitelistedSteamIds = JsonSerializer.Deserialize<List<string>>(json, null);
	}

	// Token: 0x040004D7 RID: 1239
	private List<string> whitelistedSteamIds = new List<string>();

	// Token: 0x040004D8 RID: 1240
	private string whitelistedSteamIdsFilePath;

	// Token: 0x040004D9 RID: 1241
	private FileSystemWatcher whitelistedSteamIdsWatcher;
}
