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

// Token: 0x02000050 RID: 80
public class Mod : INotifyPropertyChanged
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x0600023F RID: 575 RVA: 0x00016A28 File Offset: 0x00014C28
	// (remove) Token: 0x06000240 RID: 576 RVA: 0x00016A60 File Offset: 0x00014C60
	public event PropertyChangedEventHandler PropertyChanged;

	// Token: 0x17000033 RID: 51
	// (get) Token: 0x06000241 RID: 577 RVA: 0x0000855E File Offset: 0x0000675E
	// (set) Token: 0x06000242 RID: 578 RVA: 0x00008566 File Offset: 0x00006766
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

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x06000243 RID: 579 RVA: 0x00008584 File Offset: 0x00006784
	// (set) Token: 0x06000244 RID: 580 RVA: 0x0000858C File Offset: 0x0000678C
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

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x06000245 RID: 581 RVA: 0x000085AF File Offset: 0x000067AF
	// (set) Token: 0x06000246 RID: 582 RVA: 0x000085B7 File Offset: 0x000067B7
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

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x06000247 RID: 583 RVA: 0x000085D5 File Offset: 0x000067D5
	// (set) Token: 0x06000248 RID: 584 RVA: 0x000085DD File Offset: 0x000067DD
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

	// Token: 0x06000249 RID: 585 RVA: 0x00016A98 File Offset: 0x00014C98
	public Mod(ModManagerV2 modManager, InstalledItem installedItem)
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

	// Token: 0x0600024A RID: 586 RVA: 0x00016B38 File Offset: 0x00014D38
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

	// Token: 0x0600024B RID: 587 RVA: 0x00016BF0 File Offset: 0x00014DF0
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
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnModEnableSucceeded", new Dictionary<string, object>
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
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnModEnableFailed", new Dictionary<string, object>
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

	// Token: 0x0600024C RID: 588 RVA: 0x00016CFC File Offset: 0x00014EFC
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
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnModDisableSucceeded", new Dictionary<string, object>
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
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnModDisableFailed", new Dictionary<string, object>
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

	// Token: 0x0600024D RID: 589 RVA: 0x000085FB File Offset: 0x000067FB
	public void Dispose()
	{
		this.Disable(false);
		this.InstalledItem.PropertyChanged -= this.OnInstalledItemPropertyChanged;
		this.PropertyChanged -= this.OnModPropertyChanged;
	}

	// Token: 0x0600024E RID: 590 RVA: 0x0000862D File Offset: 0x0000682D
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

	// Token: 0x0600024F RID: 591 RVA: 0x00016E20 File Offset: 0x00015020
	public void OnInstalledItemPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnModChanged", new Dictionary<string, object>
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

	// Token: 0x06000250 RID: 592 RVA: 0x0000863C File Offset: 0x0000683C
	public void OnModPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnModChanged", new Dictionary<string, object>
		{
			{
				"mod",
				this
			}
		});
	}

	// Token: 0x06000251 RID: 593 RVA: 0x0000865E File Offset: 0x0000685E
	private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
	{
		PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
		if (propertyChanged == null)
		{
			return;
		}
		propertyChanged(this, new PropertyChangedEventArgs(propertyName));
	}

	// Token: 0x04000150 RID: 336
	public InstalledItem InstalledItem;

	// Token: 0x04000151 RID: 337
	private bool isEnabled;

	// Token: 0x04000152 RID: 338
	private Texture2D previewTexture;

	// Token: 0x04000153 RID: 339
	private bool isAssemblyMod;

	// Token: 0x04000154 RID: 340
	private bool isPlugin;

	// Token: 0x04000155 RID: 341
	private ModManagerV2 modManager;

	// Token: 0x04000156 RID: 342
	private string assemblyPath;

	// Token: 0x04000157 RID: 343
	private Assembly assembly;

	// Token: 0x04000158 RID: 344
	private object instance;

	// Token: 0x04000159 RID: 345
	private MethodInfo onEnableMethod;

	// Token: 0x0400015A RID: 346
	private MethodInfo onDisableMethod;

	// Token: 0x0400015B RID: 347
	private bool isDownloadingPreviewTexture;
}
