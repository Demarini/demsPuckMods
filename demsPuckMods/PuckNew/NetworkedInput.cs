using System;
using UnityEngine;

// Token: 0x020001E5 RID: 485
public class NetworkedInput<T>
{
	// Token: 0x17000127 RID: 295
	// (get) Token: 0x06000E20 RID: 3616 RVA: 0x0004267D File Offset: 0x0004087D
	public bool HasChanged
	{
		get
		{
			return this.HasChangedValidator(this.LastSentValue, this.ClientValue);
		}
	}

	// Token: 0x17000128 RID: 296
	// (get) Token: 0x06000E21 RID: 3617 RVA: 0x00042696 File Offset: 0x00040896
	public bool ShouldChange
	{
		get
		{
			return this.ShouldChangeValidator(this.LastReceivedValue, this.LastReceivedTime, this.ServerValue);
		}
	}

	// Token: 0x06000E22 RID: 3618 RVA: 0x000426B8 File Offset: 0x000408B8
	public NetworkedInput(T initialValue = default(T), NetworkedInput<T>.HasChangedDelegate hasChangedValidator = null, NetworkedInput<T>.ShouldChangeDelegate shouldChangeValidator = null)
	{
		this.ClientValue = initialValue;
		this.LastSentValue = default(!0);
		this.ServerValue = default(!0);
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

	// Token: 0x06000E23 RID: 3619 RVA: 0x00042725 File Offset: 0x00040925
	public void ClientTick()
	{
		this.LastSentValue = this.ClientValue;
		this.LastSentTime = Time.timeAsDouble;
	}

	// Token: 0x06000E24 RID: 3620 RVA: 0x0004273E File Offset: 0x0004093E
	public void ServerTick()
	{
		this.LastReceivedValue = this.ServerValue;
		this.LastReceivedTime = Time.timeAsDouble;
	}

	// Token: 0x04000891 RID: 2193
	public T ClientValue;

	// Token: 0x04000892 RID: 2194
	public T ServerValue;

	// Token: 0x04000893 RID: 2195
	public T LastSentValue;

	// Token: 0x04000894 RID: 2196
	public double LastSentTime;

	// Token: 0x04000895 RID: 2197
	public T LastReceivedValue;

	// Token: 0x04000896 RID: 2198
	public double LastReceivedTime;

	// Token: 0x04000897 RID: 2199
	private NetworkedInput<T>.HasChangedDelegate HasChangedValidator;

	// Token: 0x04000898 RID: 2200
	private NetworkedInput<T>.ShouldChangeDelegate ShouldChangeValidator;

	// Token: 0x020001E6 RID: 486
	// (Invoke) Token: 0x06000E28 RID: 3624
	public delegate bool HasChangedDelegate(T LastSentValue, T ClientValue);

	// Token: 0x020001E7 RID: 487
	// (Invoke) Token: 0x06000E2C RID: 3628
	public delegate bool ShouldChangeDelegate(T LastSentValue, double lastReceivedTime, T ClientValue);
}
