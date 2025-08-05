using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// Token: 0x020000B0 RID: 176
public class InstalledItem : INotifyPropertyChanged
{
	// Token: 0x14000002 RID: 2
	// (add) Token: 0x0600052E RID: 1326 RVA: 0x00021664 File Offset: 0x0001F864
	// (remove) Token: 0x0600052F RID: 1327 RVA: 0x0002169C File Offset: 0x0001F89C
	public event PropertyChangedEventHandler PropertyChanged;

	// Token: 0x1700007D RID: 125
	// (get) Token: 0x06000530 RID: 1328 RVA: 0x0000A39E File Offset: 0x0000859E
	// (set) Token: 0x06000531 RID: 1329 RVA: 0x0000A3A6 File Offset: 0x000085A6
	public ulong Id
	{
		get
		{
			return this.id;
		}
		set
		{
			if (this.id == value)
			{
				return;
			}
			this.id = value;
			this.NotifyPropertyChanged("Id");
		}
	}

	// Token: 0x1700007E RID: 126
	// (get) Token: 0x06000532 RID: 1330 RVA: 0x0000A3C4 File Offset: 0x000085C4
	// (set) Token: 0x06000533 RID: 1331 RVA: 0x0000A3CC File Offset: 0x000085CC
	public string Path
	{
		get
		{
			return this.path;
		}
		set
		{
			if (this.path == value)
			{
				return;
			}
			this.path = value;
			this.NotifyPropertyChanged("Path");
		}
	}

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x06000534 RID: 1332 RVA: 0x0000A3EF File Offset: 0x000085EF
	// (set) Token: 0x06000535 RID: 1333 RVA: 0x0000A3F7 File Offset: 0x000085F7
	public ItemDetails ItemDetails
	{
		get
		{
			return this.itemDetails;
		}
		set
		{
			if (this.itemDetails == value)
			{
				return;
			}
			this.itemDetails = value;
			this.NotifyPropertyChanged("ItemDetails");
		}
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x0000A415 File Offset: 0x00008615
	public InstalledItem(ulong id, string path)
	{
		this.id = id;
		this.path = path;
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnItemDetails", new Action<Dictionary<string, object>>(this.Event_Client_OnItemDetails));
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x0000A446 File Offset: 0x00008646
	public void Dispose()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnItemDetails", new Action<Dictionary<string, object>>(this.Event_Client_OnItemDetails));
	}

	// Token: 0x06000538 RID: 1336 RVA: 0x000216D4 File Offset: 0x0001F8D4
	private void Event_Client_OnItemDetails(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["id"];
		string title = (string)message["title"];
		string description = (string)message["description"];
		string previewUrl = (string)message["previewUrl"];
		if (this.id != num)
		{
			return;
		}
		this.ItemDetails = new ItemDetails
		{
			Title = title,
			Description = description,
			PreviewUrl = previewUrl
		};
	}

	// Token: 0x06000539 RID: 1337 RVA: 0x0000A463 File Offset: 0x00008663
	private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
	{
		PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
		if (propertyChanged == null)
		{
			return;
		}
		propertyChanged(this, new PropertyChangedEventArgs(propertyName));
	}

	// Token: 0x040002DA RID: 730
	private ulong id;

	// Token: 0x040002DB RID: 731
	private string path;

	// Token: 0x040002DC RID: 732
	private ItemDetails itemDetails;
}
