using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x020000C2 RID: 194
public class Mod : INotifyPropertyChanged
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x060005D5 RID: 1493 RVA: 0x0001F2D4 File Offset: 0x0001D4D4
	// (remove) Token: 0x060005D6 RID: 1494 RVA: 0x0001F30C File Offset: 0x0001D50C
	public event PropertyChangedEventHandler PropertyChanged;

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x060005D7 RID: 1495 RVA: 0x0001F341 File Offset: 0x0001D541
	// (set) Token: 0x060005D8 RID: 1496 RVA: 0x0001F349 File Offset: 0x0001D549
	public bool IsEnabled
	{
		get
		{
			return this.isEnabled;
		}
		set
		{
			if (this.isEnabled == value)
			{
				return;
			}
			this.isEnabled = value;
			this.NotifyPropertyChanged("IsEnabled");
		}
	}

	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x060005D9 RID: 1497 RVA: 0x0001F367 File Offset: 0x0001D567
	// (set) Token: 0x060005DA RID: 1498 RVA: 0x0001F36F File Offset: 0x0001D56F
	public Texture2D PreviewTexture
	{
		get
		{
			return this.previewTexture;
		}
		set
		{
			if (this.previewTexture == value)
			{
				return;
			}
			this.previewTexture = value;
			this.NotifyPropertyChanged("PreviewTexture");
		}
	}

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x060005DB RID: 1499 RVA: 0x0001F392 File Offset: 0x0001D592
	// (set) Token: 0x060005DC RID: 1500 RVA: 0x0001F39A File Offset: 0x0001D59A
	public bool IsAssemblyMod
	{
		get
		{
			return this.isAssemblyMod;
		}
		set
		{
			if (this.isAssemblyMod == value)
			{
				return;
			}
			this.isAssemblyMod = value;
			this.NotifyPropertyChanged("IsAssemblyMod");
		}
	}

	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x060005DD RID: 1501 RVA: 0x0001F3B8 File Offset: 0x0001D5B8
	// (set) Token: 0x060005DE RID: 1502 RVA: 0x0001F3C0 File Offset: 0x0001D5C0
	public bool IsPlugin
	{
		get
		{
			return this.isPlugin;
		}
		set
		{
			if (this.isPlugin == value)
			{
				return;
			}
			this.isPlugin = value;
			this.NotifyPropertyChanged("IsPlugin");
		}
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x0001F3E0 File Offset: 0x0001D5E0
	public Mod(ModManager modManager, InstalledItem installedItem)
	{
		this.modManager = modManager;
		this.InstalledItem = installedItem;
		this.InstalledItem.PropertyChanged += this.OnInstalledItemPropertyChanged;
		this.PropertyChanged += this.OnModPropertyChanged;
		this.assemblyPath = (from path in Directory.GetFiles(this.InstalledItem.Path, "*.dll", SearchOption.TopDirectoryOnly)
		orderby path
		select path).FirstOrDefault<string>();
		this.IsAssemblyMod = (this.assemblyPath != null);
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x0001F480 File Offset: 0x0001D680
	private void LoadAssembly()
	{
		if (this.instance != null)
		{
			return;
		}
		this.assembly = Assembly.LoadFile(this.assemblyPath);
		Type type2 = this.assembly.GetTypes().FirstOrDefault((Type type) => type.IsClass && !type.IsAbstract && typeof(IPuckMod).IsAssignableFrom(type));
		if (type2 == null)
		{
			throw new Exception("IPuckMod missing from assembly");
		}
		this.instance = Activator.CreateInstance(type2);
		this.onEnableMethod = type2.GetMethod("OnEnable");
		this.onDisableMethod = type2.GetMethod("OnDisable");
		Debug.Log(string.Format("[Mod] Loaded assembly for mod {0}", this.InstalledItem.Id));
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x0001F538 File Offset: 0x0001D738
	public void Enable(bool isManual = false)
	{
		if (this.IsEnabled)
		{
			return;
		}
		try
		{
			if (this.IsAssemblyMod)
			{
				this.LoadAssembly();
				if (!(bool)this.onEnableMethod.Invoke(this.instance, null))
				{
					throw new Exception("OnEnable returned false");
				}
			}
			this.IsEnabled = true;
			Debug.Log(string.Format("[Mod] Enabled mod {0}", this.InstalledItem.Id));
			EventManager.TriggerEvent("Event_OnModEnableSucceeded", new Dictionary<string, object>
			{
				{
					"mod",
					this
				},
				{
					"isManual",
					isManual
				}
			});
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("[Mod] Failed to enable mod {0}: {1}", this.InstalledItem.Id, ex.Message));
			EventManager.TriggerEvent("Event_OnModEnableFailed", new Dictionary<string, object>
			{
				{
					"mod",
					this
				},
				{
					"isManual",
					isManual
				}
			});
		}
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x0001F63C File Offset: 0x0001D83C
	public void Disable(bool isManual = false)
	{
		if (!this.IsEnabled)
		{
			return;
		}
		Debug.Log(string.Format("[Mod] Disabling mod {0}...", this.InstalledItem.Id));
		try
		{
			if (this.IsAssemblyMod && !(bool)this.onDisableMethod.Invoke(this.instance, null))
			{
				throw new Exception("OnDisable returned false");
			}
			this.IsEnabled = false;
			Debug.Log(string.Format("[Mod] Disabled mod {0}", this.InstalledItem.Id));
			EventManager.TriggerEvent("Event_OnModDisableSucceeded", new Dictionary<string, object>
			{
				{
					"mod",
					this
				},
				{
					"isManual",
					isManual
				}
			});
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("[Mod] Failed to disable mod {0}: {1}", this.InstalledItem.Id, ex.Message));
			EventManager.TriggerEvent("Event_OnModDisableFailed", new Dictionary<string, object>
			{
				{
					"mod",
					this
				},
				{
					"isManual",
					isManual
				}
			});
		}
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x0001F758 File Offset: 0x0001D958
	public void Dispose()
	{
		this.Disable(false);
		this.InstalledItem.PropertyChanged -= this.OnInstalledItemPropertyChanged;
		this.PropertyChanged -= this.OnModPropertyChanged;
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x0001F78A File Offset: 0x0001D98A
	private IEnumerator DownloadPreviewTexture()
	{
		UnityWebRequest www = UnityWebRequestTexture.GetTexture(this.InstalledItem.ItemDetails.PreviewUrl);
		yield return www.SendWebRequest();
		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError("[Mod] Failed to download preview texture: " + www.error);
		}
		else
		{
			this.PreviewTexture = DownloadHandlerTexture.GetContent(www);
		}
		this.isDownloadingPreviewTexture = false;
		yield break;
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x0001F79C File Offset: 0x0001D99C
	public void OnInstalledItemPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		EventManager.TriggerEvent("Event_OnModChanged", new Dictionary<string, object>
		{
			{
				"mod",
				this
			}
		});
		if (!this.isDownloadingPreviewTexture && this.InstalledItem.ItemDetails != null && this.InstalledItem.ItemDetails.PreviewUrl != null && this.PreviewTexture == null)
		{
			this.isDownloadingPreviewTexture = true;
			this.modManager.StartCoroutine(this.DownloadPreviewTexture());
		}
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x0001F812 File Offset: 0x0001DA12
	public void OnModPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		EventManager.TriggerEvent("Event_OnModChanged", new Dictionary<string, object>
		{
			{
				"mod",
				this
			}
		});
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x0001F82F File Offset: 0x0001DA2F
	private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
		if (propertyChanged == null)
		{
			return;
		}
		propertyChanged(this, new PropertyChangedEventArgs(propertyName));
	}

	// Token: 0x040003A2 RID: 930
	public InstalledItem InstalledItem;

	// Token: 0x040003A3 RID: 931
	private bool isEnabled;

	// Token: 0x040003A4 RID: 932
	private Texture2D previewTexture;

	// Token: 0x040003A5 RID: 933
	private bool isAssemblyMod;

	// Token: 0x040003A6 RID: 934
	private bool isPlugin;

	// Token: 0x040003A7 RID: 935
	private ModManager modManager;

	// Token: 0x040003A8 RID: 936
	private string assemblyPath;

	// Token: 0x040003A9 RID: 937
	private Assembly assembly;

	// Token: 0x040003AA RID: 938
	private object instance;

	// Token: 0x040003AB RID: 939
	private MethodInfo onEnableMethod;

	// Token: 0x040003AC RID: 940
	private MethodInfo onDisableMethod;

	// Token: 0x040003AD RID: 941
	private bool isDownloadingPreviewTexture;
}
