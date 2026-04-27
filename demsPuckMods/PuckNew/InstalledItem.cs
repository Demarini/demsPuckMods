using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// Token: 0x02000134 RID: 308
public class InstalledItem : INotifyPropertyChanged
{
	// Token: 0x14000002 RID: 2
	// (add) Token: 0x06000913 RID: 2323 RVA: 0x0002BE7C File Offset: 0x0002A07C
	// (remove) Token: 0x06000914 RID: 2324 RVA: 0x0002BEB4 File Offset: 0x0002A0B4
	public event PropertyChangedEventHandler PropertyChanged;

	// Token: 0x170000EC RID: 236
	// (get) Token: 0x06000915 RID: 2325 RVA: 0x0002BEE9 File Offset: 0x0002A0E9
	// (set) Token: 0x06000916 RID: 2326 RVA: 0x0002BEF1 File Offset: 0x0002A0F1
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

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x06000917 RID: 2327 RVA: 0x0002BF0F File Offset: 0x0002A10F
	// (set) Token: 0x06000918 RID: 2328 RVA: 0x0002BF17 File Offset: 0x0002A117
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

	// Token: 0x170000EE RID: 238
	// (get) Token: 0x06000919 RID: 2329 RVA: 0x0002BF3A File Offset: 0x0002A13A
	// (set) Token: 0x0600091A RID: 2330 RVA: 0x0002BF42 File Offset: 0x0002A142
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

	// Token: 0x0600091B RID: 2331 RVA: 0x0002BF60 File Offset: 0x0002A160
	public InstalledItem(ulong id, string path)
	{
		this.id = id;
		this.path = path;
		EventManager.AddEventListener("Event_OnItemDetails", new Action<Dictionary<string, object>>(this.Event_OnItemDetails));
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x0002BF8C File Offset: 0x0002A18C
	public void Dispose()
	{
		EventManager.RemoveEventListener("Event_OnItemDetails", new Action<Dictionary<string, object>>(this.Event_OnItemDetails));
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x0002BFA4 File Offset: 0x0002A1A4
	private void Event_OnItemDetails(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["id"];
		string title = (string)message["title"];
		string description = (string)message["description"];
		string previewUrl = (string)message["previewUrl"];
		string metadata = (string)message["metadata"];
		if (this.id != num)
		{
			return;
		}
		this.ItemDetails = new ItemDetails
		{
			Title = title,
			Description = description,
			PreviewUrl = previewUrl,
			Metadata = metadata
		};
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x0002C039 File Offset: 0x0002A239
	private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
		if (propertyChanged == null)
		{
			return;
		}
		propertyChanged(this, new PropertyChangedEventArgs(propertyName));
	}

	// Token: 0x04000545 RID: 1349
	private ulong id;

	// Token: 0x04000546 RID: 1350
	private string path;

	// Token: 0x04000547 RID: 1351
	private ItemDetails itemDetails;
}
