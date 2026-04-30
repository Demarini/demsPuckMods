using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

// Token: 0x020001E9 RID: 489
internal class ObservableList<T> : List<!0> where T : INotifyPropertyChanged
{
	// Token: 0x14000004 RID: 4
	// (add) Token: 0x06000E36 RID: 3638 RVA: 0x000428CC File Offset: 0x00040ACC
	// (remove) Token: 0x06000E37 RID: 3639 RVA: 0x00042904 File Offset: 0x00040B04
	public event ObservableList<!0>.OnAdd onAdd
	{
		[CompilerGenerated]
		add
		{
			ObservableList<T>.OnAdd onAdd = this.onAdd;
			ObservableList<T>.OnAdd onAdd2;
			do
			{
				onAdd2 = onAdd;
				ObservableList<T>.OnAdd value2 = (ObservableList<!0>.OnAdd)Delegate.Combine(onAdd2, value);
				onAdd = Interlocked.CompareExchange<ObservableList<T>.OnAdd>(ref this.onAdd, value2, onAdd2);
			}
			while (onAdd != onAdd2);
		}
		[CompilerGenerated]
		remove
		{
			ObservableList<T>.OnAdd onAdd = this.onAdd;
			ObservableList<T>.OnAdd onAdd2;
			do
			{
				onAdd2 = onAdd;
				ObservableList<T>.OnAdd value2 = (ObservableList<!0>.OnAdd)Delegate.Remove(onAdd2, value);
				onAdd = Interlocked.CompareExchange<ObservableList<T>.OnAdd>(ref this.onAdd, value2, onAdd2);
			}
			while (onAdd != onAdd2);
		}
	}

	// Token: 0x14000005 RID: 5
	// (add) Token: 0x06000E38 RID: 3640 RVA: 0x0004293C File Offset: 0x00040B3C
	// (remove) Token: 0x06000E39 RID: 3641 RVA: 0x00042974 File Offset: 0x00040B74
	public event ObservableList<!0>.OnRemove onRemove
	{
		[CompilerGenerated]
		add
		{
			ObservableList<T>.OnRemove onRemove = this.onRemove;
			ObservableList<T>.OnRemove onRemove2;
			do
			{
				onRemove2 = onRemove;
				ObservableList<T>.OnRemove value2 = (ObservableList<!0>.OnRemove)Delegate.Combine(onRemove2, value);
				onRemove = Interlocked.CompareExchange<ObservableList<T>.OnRemove>(ref this.onRemove, value2, onRemove2);
			}
			while (onRemove != onRemove2);
		}
		[CompilerGenerated]
		remove
		{
			ObservableList<T>.OnRemove onRemove = this.onRemove;
			ObservableList<T>.OnRemove onRemove2;
			do
			{
				onRemove2 = onRemove;
				ObservableList<T>.OnRemove value2 = (ObservableList<!0>.OnRemove)Delegate.Remove(onRemove2, value);
				onRemove = Interlocked.CompareExchange<ObservableList<T>.OnRemove>(ref this.onRemove, value2, onRemove2);
			}
			while (onRemove != onRemove2);
		}
	}

	// Token: 0x14000006 RID: 6
	// (add) Token: 0x06000E3A RID: 3642 RVA: 0x000429AC File Offset: 0x00040BAC
	// (remove) Token: 0x06000E3B RID: 3643 RVA: 0x000429E4 File Offset: 0x00040BE4
	public event ObservableList<!0>.OnClear onClear
	{
		[CompilerGenerated]
		add
		{
			ObservableList<T>.OnClear onClear = this.onClear;
			ObservableList<T>.OnClear onClear2;
			do
			{
				onClear2 = onClear;
				ObservableList<T>.OnClear value2 = (ObservableList<!0>.OnClear)Delegate.Combine(onClear2, value);
				onClear = Interlocked.CompareExchange<ObservableList<T>.OnClear>(ref this.onClear, value2, onClear2);
			}
			while (onClear != onClear2);
		}
		[CompilerGenerated]
		remove
		{
			ObservableList<T>.OnClear onClear = this.onClear;
			ObservableList<T>.OnClear onClear2;
			do
			{
				onClear2 = onClear;
				ObservableList<T>.OnClear value2 = (ObservableList<!0>.OnClear)Delegate.Remove(onClear2, value);
				onClear = Interlocked.CompareExchange<ObservableList<T>.OnClear>(ref this.onClear, value2, onClear2);
			}
			while (onClear != onClear2);
		}
	}

	// Token: 0x14000007 RID: 7
	// (add) Token: 0x06000E3C RID: 3644 RVA: 0x00042A1C File Offset: 0x00040C1C
	// (remove) Token: 0x06000E3D RID: 3645 RVA: 0x00042A54 File Offset: 0x00040C54
	public event ObservableList<!0>.OnModify onModify
	{
		[CompilerGenerated]
		add
		{
			ObservableList<T>.OnModify onModify = this.onModify;
			ObservableList<T>.OnModify onModify2;
			do
			{
				onModify2 = onModify;
				ObservableList<T>.OnModify value2 = (ObservableList<!0>.OnModify)Delegate.Combine(onModify2, value);
				onModify = Interlocked.CompareExchange<ObservableList<T>.OnModify>(ref this.onModify, value2, onModify2);
			}
			while (onModify != onModify2);
		}
		[CompilerGenerated]
		remove
		{
			ObservableList<T>.OnModify onModify = this.onModify;
			ObservableList<T>.OnModify onModify2;
			do
			{
				onModify2 = onModify;
				ObservableList<T>.OnModify value2 = (ObservableList<!0>.OnModify)Delegate.Remove(onModify2, value);
				onModify = Interlocked.CompareExchange<ObservableList<T>.OnModify>(ref this.onModify, value2, onModify2);
			}
			while (onModify != onModify2);
		}
	}

	// Token: 0x06000E3E RID: 3646 RVA: 0x00042A8C File Offset: 0x00040C8C
	public new void Add(T item)
	{
		base.Add(item);
		ObservableList<!0>.OnAdd onAdd = this.onAdd;
		if (onAdd != null)
		{
			onAdd(item);
		}
		item.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
		{
			ObservableList<!0>.OnModify onModify = this.onModify;
			if (onModify == null)
			{
				return;
			}
			onModify(item, e);
		};
	}

	// Token: 0x06000E3F RID: 3647 RVA: 0x00042AED File Offset: 0x00040CED
	public new void Remove(T item)
	{
		base.Remove(item);
		ObservableList<!0>.OnRemove onRemove = this.onRemove;
		if (onRemove == null)
		{
			return;
		}
		onRemove(item);
	}

	// Token: 0x06000E40 RID: 3648 RVA: 0x00042B08 File Offset: 0x00040D08
	public new void Clear()
	{
		base.Clear();
		ObservableList<!0>.OnClear onClear = this.onClear;
		if (onClear == null)
		{
			return;
		}
		onClear();
	}

	// Token: 0x020001EA RID: 490
	// (Invoke) Token: 0x06000E43 RID: 3651
	public delegate void OnAdd(T item);

	// Token: 0x020001EB RID: 491
	// (Invoke) Token: 0x06000E47 RID: 3655
	public delegate void OnRemove(T item);

	// Token: 0x020001EC RID: 492
	// (Invoke) Token: 0x06000E4B RID: 3659
	public delegate void OnClear();

	// Token: 0x020001ED RID: 493
	// (Invoke) Token: 0x06000E4F RID: 3663
	public delegate void OnModify(T item, PropertyChangedEventArgs e);
}
