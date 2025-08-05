using System;
using System.Collections.Generic;
using System.ComponentModel;

// Token: 0x0200015D RID: 349
internal class ObservableList<T> : List<T> where T : INotifyPropertyChanged
{
	// Token: 0x14000007 RID: 7
	// (add) Token: 0x06000C3B RID: 3131 RVA: 0x000417D8 File Offset: 0x0003F9D8
	// (remove) Token: 0x06000C3C RID: 3132 RVA: 0x00041810 File Offset: 0x0003FA10
	public event ObservableList<T>.OnAdd onAdd;

	// Token: 0x14000008 RID: 8
	// (add) Token: 0x06000C3D RID: 3133 RVA: 0x00041848 File Offset: 0x0003FA48
	// (remove) Token: 0x06000C3E RID: 3134 RVA: 0x00041880 File Offset: 0x0003FA80
	public event ObservableList<T>.OnRemove onRemove;

	// Token: 0x14000009 RID: 9
	// (add) Token: 0x06000C3F RID: 3135 RVA: 0x000418B8 File Offset: 0x0003FAB8
	// (remove) Token: 0x06000C40 RID: 3136 RVA: 0x000418F0 File Offset: 0x0003FAF0
	public event ObservableList<T>.OnClear onClear;

	// Token: 0x1400000A RID: 10
	// (add) Token: 0x06000C41 RID: 3137 RVA: 0x00041928 File Offset: 0x0003FB28
	// (remove) Token: 0x06000C42 RID: 3138 RVA: 0x00041960 File Offset: 0x0003FB60
	public event ObservableList<T>.OnModify onModify;

	// Token: 0x06000C43 RID: 3139 RVA: 0x00041998 File Offset: 0x0003FB98
	public new void Add(T item)
	{
		base.Add(item);
		ObservableList<T>.OnAdd onAdd = this.onAdd;
		if (onAdd != null)
		{
			onAdd(item);
		}
		item.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
		{
			ObservableList<T>.OnModify onModify = this.onModify;
			if (onModify == null)
			{
				return;
			}
			onModify(item, e);
		};
	}

	// Token: 0x06000C44 RID: 3140 RVA: 0x0000F095 File Offset: 0x0000D295
	public new void Remove(T item)
	{
		base.Remove(item);
		ObservableList<T>.OnRemove onRemove = this.onRemove;
		if (onRemove == null)
		{
			return;
		}
		onRemove(item);
	}

	// Token: 0x06000C45 RID: 3141 RVA: 0x0000F0B0 File Offset: 0x0000D2B0
	public new void Clear()
	{
		base.Clear();
		ObservableList<T>.OnClear onClear = this.onClear;
		if (onClear == null)
		{
			return;
		}
		onClear();
	}

	// Token: 0x0200015E RID: 350
	// (Invoke) Token: 0x06000C48 RID: 3144
	public delegate void OnAdd(T item);

	// Token: 0x0200015F RID: 351
	// (Invoke) Token: 0x06000C4C RID: 3148
	public delegate void OnRemove(T item);

	// Token: 0x02000160 RID: 352
	// (Invoke) Token: 0x06000C50 RID: 3152
	public delegate void OnClear();

	// Token: 0x02000161 RID: 353
	// (Invoke) Token: 0x06000C54 RID: 3156
	public delegate void OnModify(T item, PropertyChangedEventArgs e);
}
