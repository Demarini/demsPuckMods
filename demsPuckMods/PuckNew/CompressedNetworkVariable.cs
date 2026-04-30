using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Netcode;

// Token: 0x020001D9 RID: 473
public class CompressedNetworkVariable<TRaw, TNetwork> : NetworkVariable<!1> where TRaw : struct where TNetwork : struct
{
	// Token: 0x14000003 RID: 3
	// (add) Token: 0x06000DEE RID: 3566 RVA: 0x00041EB8 File Offset: 0x000400B8
	// (remove) Token: 0x06000DEF RID: 3567 RVA: 0x00041EF0 File Offset: 0x000400F0
	public event Action<!0, !0> OnRawValueChanged
	{
		[CompilerGenerated]
		add
		{
			Action<TRaw, TRaw> action = this.OnRawValueChanged;
			Action<TRaw, TRaw> action2;
			do
			{
				action2 = action;
				Action<TRaw, TRaw> value2 = (Action<!0, !0>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action<TRaw, TRaw>>(ref this.OnRawValueChanged, value2, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action<TRaw, TRaw> action = this.OnRawValueChanged;
			Action<TRaw, TRaw> action2;
			do
			{
				action2 = action;
				Action<TRaw, TRaw> value2 = (Action<!0, !0>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action<TRaw, TRaw>>(ref this.OnRawValueChanged, value2, action2);
			}
			while (action != action2);
		}
	}

	// Token: 0x06000DF0 RID: 3568 RVA: 0x00041F28 File Offset: 0x00040128
	public CompressedNetworkVariable(Func<TRaw, TNetwork> compressor, Func<TNetwork, TRaw> decompressor, TRaw initialValue = default(TRaw), NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) : base(compressor(initialValue), readPerm, writePerm)
	{
		this.compressor = compressor;
		this.decompressor = decompressor;
		this.cachedValue = initialValue;
		this.OnValueChanged = (NetworkVariable<!1>.OnValueChangedDelegate)Delegate.Combine(this.OnValueChanged, new NetworkVariable<!1>.OnValueChangedDelegate(this.OnCompressedValueChanged));
	}

	// Token: 0x1700011F RID: 287
	// (get) Token: 0x06000DF1 RID: 3569 RVA: 0x00041F7D File Offset: 0x0004017D
	// (set) Token: 0x06000DF2 RID: 3570 RVA: 0x00041F88 File Offset: 0x00040188
	public new TRaw Value
	{
		get
		{
			return this.cachedValue;
		}
		set
		{
			TRaw traw = this.cachedValue;
			this.cachedValue = value;
			base.Value = this.compressor(value);
			if (!this.cachedValue.Equals(traw))
			{
				Action<!0, !0> onRawValueChanged = this.OnRawValueChanged;
				if (onRawValueChanged == null)
				{
					return;
				}
				onRawValueChanged(traw, this.cachedValue);
			}
		}
	}

	// Token: 0x06000DF3 RID: 3571 RVA: 0x00041FE8 File Offset: 0x000401E8
	private void OnCompressedValueChanged(TNetwork previousCompressed, TNetwork newCompressed)
	{
		TRaw arg = this.cachedValue;
		this.cachedValue = this.decompressor(newCompressed);
		Action<!0, !0> onRawValueChanged = this.OnRawValueChanged;
		if (onRawValueChanged == null)
		{
			return;
		}
		onRawValueChanged(arg, this.cachedValue);
	}

	// Token: 0x06000DF4 RID: 3572 RVA: 0x00042028 File Offset: 0x00040228
	public static CompressedNetworkVariable<float, short> CreateFloatToShort(float minValue, float maxValue, float initialValue = 0f, NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server)
	{
		return new CompressedNetworkVariable<float, short>(delegate(float value)
		{
			float val = (value - minValue) / (maxValue - minValue);
			return (short)(Math.Max(0f, Math.Min(1f, val)) * 32767f);
		}, delegate(short compressed)
		{
			float num = (float)compressed / 32767f;
			return minValue + num * (maxValue - minValue);
		}, initialValue, readPerm, writePerm);
	}

	// Token: 0x06000DF5 RID: 3573 RVA: 0x0004206C File Offset: 0x0004026C
	public static CompressedNetworkVariable<float, byte> CreateFloatToByte(float minValue, float maxValue, float initialValue = 0f, NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server)
	{
		return new CompressedNetworkVariable<float, byte>(delegate(float value)
		{
			float val = (value - minValue) / (maxValue - minValue);
			return (byte)(Math.Max(0f, Math.Min(1f, val)) * 255f);
		}, delegate(byte compressed)
		{
			float num = (float)compressed / 255f;
			return minValue + num * (maxValue - minValue);
		}, initialValue, readPerm, writePerm);
	}

	// Token: 0x0400081F RID: 2079
	private readonly Func<TRaw, TNetwork> compressor;

	// Token: 0x04000820 RID: 2080
	private readonly Func<TNetwork, TRaw> decompressor;

	// Token: 0x04000821 RID: 2081
	private TRaw cachedValue;
}
