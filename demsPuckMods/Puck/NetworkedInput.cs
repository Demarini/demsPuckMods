using System;
using UnityEngine;

// Token: 0x02000159 RID: 345
public class NetworkedInput<T>
{
	// Token: 0x1700012A RID: 298
	// (get) Token: 0x06000C2B RID: 3115 RVA: 0x0000EFE9 File Offset: 0x0000D1E9
	public bool HasChanged
	{
		get
		{
			return this.HasChangedValidator(this.LastSentValue, this.ClientValue);
		}
	}

	// Token: 0x1700012B RID: 299
	// (get) Token: 0x06000C2C RID: 3116 RVA: 0x0000F002 File Offset: 0x0000D202
	public bool ShouldChange
	{
		get
		{
			return this.ShouldChangeValidator(this.LastReceivedValue, this.LastReceivedTime, this.ServerValue);
		}
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x00041744 File Offset: 0x0003F944
	public NetworkedInput(T initialValue = default(T), NetworkedInput<T>.HasChangedDelegate hasChangedValidator = null, NetworkedInput<T>.ShouldChangeDelegate shouldChangeValidator = null)
	{
		this.ClientValue = initialValue;
		this.LastSentValue = default(T);
		this.ServerValue = default(T);
		if (hasChangedValidator != null)
		{
			this.HasChangedValidator = hasChangedValidator;
		}
		else
		{
			this.HasChangedValidator = ((T lastSentValue, T clientValue) => !this.ClientValue.Equals(this.LastSentValue));
		}
		if (shouldChangeValidator != null)
		{
			this.ShouldChangeValidator = shouldChangeValidator;
			return;
		}
		this.ShouldChangeValidator = ((T lastReceivedValue, double lastReceivedTime, T serverValue) => !this.ServerValue.Equals(this.LastReceivedValue));
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x0000F021 File Offset: 0x0000D221
	public void ClientTick()
	{
		this.LastSentValue = this.ClientValue;
		this.LastSentTime = Time.timeAsDouble;
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x0000F03A File Offset: 0x0000D23A
	public void ServerTick()
	{
		this.LastReceivedValue = this.ServerValue;
		this.LastReceivedTime = Time.timeAsDouble;
	}

	// Token: 0x04000704 RID: 1796
	public T ClientValue;

	// Token: 0x04000705 RID: 1797
	public T ServerValue;

	// Token: 0x04000706 RID: 1798
	public T LastSentValue;

	// Token: 0x04000707 RID: 1799
	public double LastSentTime;

	// Token: 0x04000708 RID: 1800
	public T LastReceivedValue;

	// Token: 0x04000709 RID: 1801
	public double LastReceivedTime;

	// Token: 0x0400070A RID: 1802
	private NetworkedInput<T>.HasChangedDelegate HasChangedValidator;

	// Token: 0x0400070B RID: 1803
	private NetworkedInput<T>.ShouldChangeDelegate ShouldChangeValidator;

	// Token: 0x0200015A RID: 346
	// (Invoke) Token: 0x06000C33 RID: 3123
	public delegate bool HasChangedDelegate(T LastSentValue, T ClientValue);

	// Token: 0x0200015B RID: 347
	// (Invoke) Token: 0x06000C37 RID: 3127
	public delegate bool ShouldChangeDelegate(T LastSentValue, double lastReceivedTime, T ClientValue);
}
