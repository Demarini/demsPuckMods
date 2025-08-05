using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using UnityEngine;

// Token: 0x0200009A RID: 154
public class ServerConfigurationManager : MonoBehaviour
{
	// Token: 0x17000070 RID: 112
	// (get) Token: 0x06000403 RID: 1027 RVA: 0x000097D9 File Offset: 0x000079D9
	// (set) Token: 0x06000404 RID: 1028 RVA: 0x000097E1 File Offset: 0x000079E1
	public ServerConfiguration ServerConfiguration { get; set; }

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x06000405 RID: 1029 RVA: 0x0001B27C File Offset: 0x0001947C
	public ulong[] ClientRequiredModIds
	{
		get
		{
			return (from mod in this.ServerConfiguration.mods
			where mod.clientRequired
			select mod.id).ToArray<ulong>();
		}
	}

	// Token: 0x17000072 RID: 114
	// (get) Token: 0x06000406 RID: 1030 RVA: 0x0001B2E4 File Offset: 0x000194E4
	public ulong[] EnabledModIds
	{
		get
		{
			return (from mod in this.ServerConfiguration.mods
			where mod.enabled
			select mod.id).ToArray<ulong>();
		}
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x0001B34C File Offset: 0x0001954C
	private void Awake()
	{
		if (!Application.isBatchMode)
		{
			return;
		}
		string path = "./server_configuration.json";
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i] == "--serverConfigurationPath")
			{
				path = commandLineArgs[i + 1];
			}
		}
		string text = Uri.UnescapeDataString(new Uri(Path.GetFullPath(path)).AbsolutePath);
		string environmentVariable = Environment.GetEnvironmentVariable("PUCK_SERVER_CONFIGURATION");
		if (!string.IsNullOrEmpty(environmentVariable))
		{
			Debug.Log("[ServerConfigurationManager] Reading server configuration from environment variable PUCK_SERVER_CONFIGURATION...");
			string text2 = environmentVariable;
			Debug.Log("[ServerConfigurationManager] PUCK_SERVER_CONFIGURATION: " + text2);
			Debug.Log("[ServerConfigurationManager] Parsing server configuration...");
			this.ServerConfiguration = JsonSerializer.Deserialize<ServerConfiguration>(text2, null);
			return;
		}
		if (File.Exists(text))
		{
			Debug.Log("[ServerConfigurationManager] Reading server configuration file from " + text + "...");
		}
		else
		{
			Debug.Log("[ServerConfigurationManager] Server configuration file not found at " + text + ", creating...");
			File.AppendAllText(text, JsonSerializer.Serialize<ServerConfiguration>(new ServerConfiguration(), new JsonSerializerOptions
			{
				WriteIndented = true
			}));
		}
		string text3 = File.ReadAllText(text);
		Debug.Log("[ServerConfigurationManager] " + text + ": " + text3);
		Debug.Log("[ServerConfigurationManager] Parsing server configuration...");
		this.ServerConfiguration = JsonSerializer.Deserialize<ServerConfiguration>(text3, null);
	}
}
