using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using UnityEngine;

// Token: 0x02000111 RID: 273
public class BanManager : MonoBehaviourSingleton<BanManager>
{
	// Token: 0x06000774 RID: 1908 RVA: 0x00024AC4 File Offset: 0x00022CC4
	public override void Awake()
	{
		base.Awake();
		this.bannedSteamIdsFilePath = (Utils.GetCommandLineArgument("--bannedSteamIdsPath", null) ?? "./banned_steam_ids.json");
		this.bannedSteamIdsFilePath = Path.GetFullPath(this.bannedSteamIdsFilePath);
		this.bannedIpAddressesFilePath = (Utils.GetCommandLineArgument("--bannedIpAddressesPath", null) ?? "./banned_ip_addresses.json");
		this.bannedIpAddressesFilePath = Path.GetFullPath(this.bannedIpAddressesFilePath);
	}

	// Token: 0x06000775 RID: 1909 RVA: 0x00024B30 File Offset: 0x00022D30
	public void Dispose()
	{
		this.bannedSteamIds.Clear();
		if (this.bannedSteamIdsWatcher != null)
		{
			this.bannedSteamIdsWatcher.Dispose();
			this.bannedSteamIdsWatcher = null;
		}
		this.bannedIpAddresses.Clear();
		if (this.bannedIpAddressesWatcher != null)
		{
			this.bannedIpAddressesWatcher.Dispose();
			this.bannedIpAddressesWatcher = null;
		}
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x00024B88 File Offset: 0x00022D88
	public void LoadBannedSteamIds()
	{
		if (!File.Exists(this.bannedSteamIdsFilePath))
		{
			Debug.LogWarning("[BanManager] Banned steam ids file not found at " + this.bannedSteamIdsFilePath + ", creating default...");
			File.AppendAllText(this.bannedSteamIdsFilePath, JsonSerializer.Serialize<List<string>>(new List<string>(), new JsonSerializerOptions
			{
				WriteIndented = true
			}));
		}
		this.ReadBannedSteamIds();
		this.WatchBannedSteamIds(this.bannedSteamIdsFilePath);
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x00024BEF File Offset: 0x00022DEF
	public void SaveBannedSteamIds()
	{
		File.WriteAllText(this.bannedSteamIdsFilePath, JsonSerializer.Serialize<List<string>>(this.bannedSteamIds, new JsonSerializerOptions
		{
			WriteIndented = true
		}));
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x00024C14 File Offset: 0x00022E14
	public void ReadBannedSteamIds()
	{
		string json = File.ReadAllText(this.bannedSteamIdsFilePath);
		this.bannedSteamIds = JsonSerializer.Deserialize<List<string>>(json, null);
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x00024C3C File Offset: 0x00022E3C
	public void WatchBannedSteamIds(string bannedSteamIdsFilePath)
	{
		if (this.bannedSteamIdsWatcher != null)
		{
			return;
		}
		string fileName = Path.GetFileName(bannedSteamIdsFilePath);
		string path = bannedSteamIdsFilePath.Replace(fileName, string.Empty);
		this.bannedSteamIdsWatcher = new FileSystemWatcher(path);
		this.bannedSteamIdsWatcher.NotifyFilter = NotifyFilters.LastWrite;
		this.bannedSteamIdsWatcher.Filter = fileName;
		this.bannedSteamIdsWatcher.EnableRaisingEvents = true;
		this.bannedSteamIdsWatcher.Changed += this.OnBannedSteamIdsFileChanged;
		Debug.Log("[BanManager] Watching banned Steam IDs file " + fileName);
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x00024CBE File Offset: 0x00022EBE
	public void AddBannedSteamId(string steamId)
	{
		if (this.bannedSteamIds.Contains(steamId))
		{
			return;
		}
		this.bannedSteamIds.Add(steamId);
		this.SaveBannedSteamIds();
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x00024CE1 File Offset: 0x00022EE1
	public void RemoveBannedSteamId(string steamId)
	{
		if (!this.bannedSteamIds.Contains(steamId))
		{
			return;
		}
		this.bannedSteamIds.Remove(steamId);
		this.SaveBannedSteamIds();
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x00024D05 File Offset: 0x00022F05
	public bool IsSteamIdBanned(string steamId)
	{
		return this.bannedSteamIds.Contains(steamId);
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x00024D14 File Offset: 0x00022F14
	public void LoadBannedIpAddresses()
	{
		if (!File.Exists(this.bannedIpAddressesFilePath))
		{
			Debug.LogWarning("[BanManager] Banned IP addresses file not found at " + this.bannedIpAddressesFilePath + ", creating default...");
			File.AppendAllText(this.bannedIpAddressesFilePath, JsonSerializer.Serialize<List<string>>(new List<string>(), new JsonSerializerOptions
			{
				WriteIndented = true
			}));
		}
		this.ReadBannedIpAddresses();
		this.WatchBannedIpAddresses(this.bannedIpAddressesFilePath);
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x00024D7B File Offset: 0x00022F7B
	public void SaveBannedIpAddresses()
	{
		File.WriteAllText(this.bannedIpAddressesFilePath, JsonSerializer.Serialize<List<string>>(this.bannedIpAddresses, new JsonSerializerOptions
		{
			WriteIndented = true
		}));
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x00024DA0 File Offset: 0x00022FA0
	public void ReadBannedIpAddresses()
	{
		string json = File.ReadAllText(this.bannedIpAddressesFilePath);
		this.bannedIpAddresses = JsonSerializer.Deserialize<List<string>>(json, null);
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x00024DC8 File Offset: 0x00022FC8
	public void WatchBannedIpAddresses(string bannedIpAddressesFilePath)
	{
		if (this.bannedIpAddressesWatcher != null)
		{
			return;
		}
		string fileName = Path.GetFileName(bannedIpAddressesFilePath);
		string path = bannedIpAddressesFilePath.Replace(fileName, string.Empty);
		this.bannedIpAddressesWatcher = new FileSystemWatcher(path);
		this.bannedIpAddressesWatcher.NotifyFilter = NotifyFilters.LastWrite;
		this.bannedIpAddressesWatcher.Filter = fileName;
		this.bannedIpAddressesWatcher.Changed += this.OnBannedIpAddressesFileChanged;
		this.bannedIpAddressesWatcher.EnableRaisingEvents = true;
		Debug.Log("[BanManager] Watching banned IP addresses file " + fileName);
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x00024E4A File Offset: 0x0002304A
	public void AddBannedIpAddress(string ipAddress)
	{
		if (this.bannedIpAddresses.Contains(ipAddress))
		{
			return;
		}
		this.bannedIpAddresses.Add(ipAddress);
		this.SaveBannedIpAddresses();
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x00024E6D File Offset: 0x0002306D
	public void RemoveBannedIpAddress(string ipAddress)
	{
		if (!this.bannedIpAddresses.Contains(ipAddress))
		{
			return;
		}
		this.bannedIpAddresses.Remove(ipAddress);
		this.SaveBannedIpAddresses();
	}

	// Token: 0x06000783 RID: 1923 RVA: 0x00024E91 File Offset: 0x00023091
	public bool IsIpAddressBanned(string ipAddress)
	{
		return this.bannedIpAddresses.Contains(ipAddress);
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x00024EA0 File Offset: 0x000230A0
	private void OnBannedSteamIdsFileChanged(object sender, FileSystemEventArgs e)
	{
		Debug.Log("[BanManager] Banned Steam IDs file changed: " + e.FullPath);
		string json = File.ReadAllText(e.FullPath);
		this.bannedSteamIds = JsonSerializer.Deserialize<List<string>>(json, null);
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x00024EDC File Offset: 0x000230DC
	private void OnBannedIpAddressesFileChanged(object sender, FileSystemEventArgs e)
	{
		Debug.Log("[BanManager] Banned IP addresses file changed: " + e.FullPath);
		string json = File.ReadAllText(e.FullPath);
		this.bannedIpAddresses = JsonSerializer.Deserialize<List<string>>(json, null);
	}

	// Token: 0x04000480 RID: 1152
	private List<string> bannedSteamIds = new List<string>();

	// Token: 0x04000481 RID: 1153
	private string bannedSteamIdsFilePath;

	// Token: 0x04000482 RID: 1154
	private FileSystemWatcher bannedSteamIdsWatcher;

	// Token: 0x04000483 RID: 1155
	private List<string> bannedIpAddresses = new List<string>();

	// Token: 0x04000484 RID: 1156
	private string bannedIpAddressesFilePath;

	// Token: 0x04000485 RID: 1157
	private FileSystemWatcher bannedIpAddressesWatcher;
}
