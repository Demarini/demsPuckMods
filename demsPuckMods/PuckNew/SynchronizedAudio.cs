using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200006C RID: 108
[RequireComponent(typeof(AudioSource))]
public class SynchronizedAudio : NetworkBehaviour
{
	// Token: 0x06000380 RID: 896 RVA: 0x000147ED File Offset: 0x000129ED
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.initialVolume = this.audioSource.volume;
		this.initialPitch = this.audioSource.pitch;
	}

	// Token: 0x06000381 RID: 897 RVA: 0x0001481D File Offset: 0x00012A1D
	protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
	{
		this.InitializeNetworkVariables(NetworkingUtils.CompressFloatToByte(this.initialVolume, 0f, 1f), NetworkingUtils.CompressFloatToByte(this.initialPitch, 0f, 1f));
		base.OnNetworkPreSpawn(ref networkManager);
	}

	// Token: 0x06000382 RID: 898 RVA: 0x00014858 File Offset: 0x00012A58
	public override void OnNetworkSpawn()
	{
		NetworkVariable<byte> volume = this.Volume;
		volume.OnValueChanged = (NetworkVariable<byte>.OnValueChangedDelegate)Delegate.Combine(volume.OnValueChanged, new NetworkVariable<byte>.OnValueChangedDelegate(this.OnVolumeChanged));
		NetworkVariable<byte> pitch = this.Pitch;
		pitch.OnValueChanged = (NetworkVariable<byte>.OnValueChangedDelegate)Delegate.Combine(pitch.OnValueChanged, new NetworkVariable<byte>.OnValueChangedDelegate(this.OnPitchChanged));
		base.OnNetworkSpawn();
	}

	// Token: 0x06000383 RID: 899 RVA: 0x000148B9 File Offset: 0x00012AB9
	protected override void OnNetworkPostSpawn()
	{
		if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsConnectedClient)
		{
			this.ProcessInitialNetworkVariableValues();
		}
		base.OnNetworkPostSpawn();
	}

	// Token: 0x06000384 RID: 900 RVA: 0x000148DF File Offset: 0x00012ADF
	protected override void OnNetworkSessionSynchronized()
	{
		this.ProcessInitialNetworkVariableValues();
		base.OnNetworkSessionSynchronized();
	}

	// Token: 0x06000385 RID: 901 RVA: 0x000148F0 File Offset: 0x00012AF0
	public override void OnNetworkDespawn()
	{
		NetworkVariable<byte> volume = this.Volume;
		volume.OnValueChanged = (NetworkVariable<byte>.OnValueChangedDelegate)Delegate.Remove(volume.OnValueChanged, new NetworkVariable<byte>.OnValueChangedDelegate(this.OnVolumeChanged));
		NetworkVariable<byte> pitch = this.Pitch;
		pitch.OnValueChanged = (NetworkVariable<byte>.OnValueChangedDelegate)Delegate.Remove(pitch.OnValueChanged, new NetworkVariable<byte>.OnValueChangedDelegate(this.OnPitchChanged));
		if (this.stopOnDespawn && this.audioSource != null)
		{
			this.audioSource.Stop();
			if (this.audioSourceSequence != null)
			{
				this.audioSourceSequence.Kill(false);
			}
		}
		base.OnNetworkDespawn();
	}

	// Token: 0x06000386 RID: 902 RVA: 0x00014986 File Offset: 0x00012B86
	public override void OnDestroy()
	{
		this.DOKill(false);
	}

	// Token: 0x06000387 RID: 903 RVA: 0x00014990 File Offset: 0x00012B90
	public void InitializeNetworkVariables(byte volume = 0, byte pitch = 0)
	{
		if (this.isNetworkVariablesInitialized)
		{
			return;
		}
		this.isNetworkVariablesInitialized = true;
		this.Volume = new NetworkVariable<byte>(volume, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
		this.Pitch = new NetworkVariable<byte>(pitch, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
	}

	// Token: 0x06000388 RID: 904 RVA: 0x000149BE File Offset: 0x00012BBE
	private void ProcessInitialNetworkVariableValues()
	{
		this.OnVolumeChanged(0, this.Volume.Value);
		this.OnPitchChanged(0, this.Pitch.Value);
	}

	// Token: 0x06000389 RID: 905 RVA: 0x000149E4 File Offset: 0x00012BE4
	private void Update()
	{
		if (!base.IsSpawned)
		{
			return;
		}
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.isPlaying && !this.audioSource.isPlaying)
		{
			this.isPlaying = false;
		}
		if (this.isPlaying)
		{
			this.time += Time.deltaTime;
		}
	}

	// Token: 0x0600038A RID: 906 RVA: 0x00014A3D File Offset: 0x00012C3D
	private void OnVolumeChanged(byte oldVolume, byte newVolume)
	{
		this.audioSource.volume = NetworkingUtils.DecompressByteToFloat(newVolume, 0f, 1f);
	}

	// Token: 0x0600038B RID: 907 RVA: 0x00014A5A File Offset: 0x00012C5A
	private void OnPitchChanged(byte oldPitch, byte newPitch)
	{
		this.audioSource.pitch = NetworkingUtils.DecompressByteToFloat(newPitch, 0f, 1f);
	}

	// Token: 0x0600038C RID: 908 RVA: 0x00014A77 File Offset: 0x00012C77
	public void Server_SetVolume(float volume)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Volume.Value = NetworkingUtils.CompressFloatToByte(volume, 0f, 1f);
	}

	// Token: 0x0600038D RID: 909 RVA: 0x00014AA1 File Offset: 0x00012CA1
	public void Server_SetPitch(float pitch)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Pitch.Value = NetworkingUtils.CompressFloatToByte(pitch, 0f, 1f);
	}

	// Token: 0x0600038E RID: 910 RVA: 0x00014ACC File Offset: 0x00012CCC
	public void Server_Play(float volume = -1f, float pitch = -1f, bool isOneShot = false, int clipIndex = -1, float time = 0f, bool randomClip = false, bool randomTime = false, bool fadeIn = false, float fadeInDuration = 0f, bool fadeOut = false, float fadeOutDuration = 0f, float duration = -1f)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		volume = ((volume == -1f) ? this.initialVolume : volume);
		pitch = ((pitch == -1f) ? this.initialPitch : pitch);
		if (randomClip && this.audioClips.Count > 0)
		{
			clipIndex = Random.Range(0, this.audioClips.Count);
		}
		float num = (clipIndex == -1) ? this.audioSource.clip.length : this.audioClips[clipIndex].length;
		if (randomTime)
		{
			time = Random.Range(0f, Mathf.Max(num - duration - fadeInDuration - fadeOutDuration, 0f));
		}
		this.duration = ((duration == -1f) ? num : duration);
		if (!isOneShot)
		{
			this.Volume.Value = NetworkingUtils.CompressFloatToByte(volume, 0f, 1f);
			this.Pitch.Value = NetworkingUtils.CompressFloatToByte(pitch, 0f, 1f);
			this.clipIndex = clipIndex;
			this.time = time;
			this.fadeIn = fadeIn;
			this.fadeInDuration = fadeInDuration;
			this.fadeOut = fadeOut;
			this.fadeOutDuration = fadeOutDuration;
			this.isPlaying = true;
		}
		this.Server_PlayRpc(volume, pitch, isOneShot, clipIndex, time, fadeIn, fadeInDuration, fadeOut, fadeOutDuration, duration, base.RpcTarget.Everyone);
	}

	// Token: 0x0600038F RID: 911 RVA: 0x00014C2C File Offset: 0x00012E2C
	[Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server, DeferLocal = true)]
	private void Server_PlayRpc(float volume, float pitch, bool isOneShot, int clipIndex, float time, bool fadeIn, float fadeInDuration, bool fadeOut, float fadeOutDuration, float duration, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 408477299U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true
			};
			FastBufferWriter writer = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
			writer.WriteValueSafe<float>(volume, default(FastBufferWriter.ForPrimitives));
			writer.WriteValueSafe<float>(pitch, default(FastBufferWriter.ForPrimitives));
			writer.WriteValueSafe<bool>(isOneShot, default(FastBufferWriter.ForPrimitives));
			BytePacker.WriteValueBitPacked(writer, clipIndex);
			writer.WriteValueSafe<float>(time, default(FastBufferWriter.ForPrimitives));
			writer.WriteValueSafe<bool>(fadeIn, default(FastBufferWriter.ForPrimitives));
			writer.WriteValueSafe<float>(fadeInDuration, default(FastBufferWriter.ForPrimitives));
			writer.WriteValueSafe<bool>(fadeOut, default(FastBufferWriter.ForPrimitives));
			writer.WriteValueSafe<float>(fadeOutDuration, default(FastBufferWriter.ForPrimitives));
			writer.WriteValueSafe<float>(duration, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref writer, 408477299U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.audioSource.volume = volume;
		this.audioSource.pitch = pitch;
		if (clipIndex != -1 && this.audioClips.Count > 0)
		{
			this.audioSource.clip = this.audioClips[clipIndex];
		}
		if (isOneShot)
		{
			this.audioSource.PlayOneShot(this.audioSource.clip);
			return;
		}
		this.audioSourceSequence = DOTween.Sequence(this);
		if (fadeIn)
		{
			this.audioSource.volume = 0f;
			this.audioSourceSequence.Append(DOTween.To(() => this.audioSource.volume, delegate(float x)
			{
				this.audioSource.volume = x;
			}, volume, fadeInDuration)).SetEase(Ease.Linear);
		}
		this.audioSourceSequence.AppendInterval(duration - fadeInDuration - fadeOutDuration - time);
		if (fadeOut)
		{
			this.audioSourceSequence.Append(DOTween.To(() => this.audioSource.volume, delegate(float x)
			{
				this.audioSource.volume = x;
			}, 0f, fadeOutDuration).OnComplete(delegate
			{
				this.audioSource.Stop();
			})).SetEase(Ease.Linear);
		}
		this.audioSource.Play();
		this.audioSource.time = time;
	}

	// Token: 0x06000390 RID: 912 RVA: 0x00014F30 File Offset: 0x00013130
	public void Server_ForceSynchronizeClientId(ulong clientId)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!this.isPlaying)
		{
			return;
		}
		this.Server_PlayRpc(NetworkingUtils.DecompressByteToFloat(this.Volume.Value, 0f, 1f), NetworkingUtils.DecompressByteToFloat(this.Pitch.Value, 0f, 1f), false, this.clipIndex, this.time, false, this.fadeInDuration, this.fadeOut, this.fadeOutDuration, this.duration, base.RpcTarget.Single(clientId, RpcTargetUse.Temp));
	}

	// Token: 0x06000397 RID: 919 RVA: 0x00015008 File Offset: 0x00013208
	protected override void __initializeVariables()
	{
		bool flag = this.Volume == null;
		if (flag)
		{
			throw new Exception("SynchronizedAudio.Volume cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.Volume.Initialize(this);
		base.__nameNetworkVariable(this.Volume, "Volume");
		this.NetworkVariableFields.Add(this.Volume);
		flag = (this.Pitch == null);
		if (flag)
		{
			throw new Exception("SynchronizedAudio.Pitch cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.Pitch.Initialize(this);
		base.__nameNetworkVariable(this.Pitch, "Pitch");
		this.NetworkVariableFields.Add(this.Pitch);
		base.__initializeVariables();
	}

	// Token: 0x06000398 RID: 920 RVA: 0x000150B8 File Offset: 0x000132B8
	protected override void __initializeRpcs()
	{
		base.__registerRpc(408477299U, new NetworkBehaviour.RpcReceiveHandler(SynchronizedAudio.__rpc_handler_408477299), "Server_PlayRpc", RpcInvokePermission.Server);
		base.__initializeRpcs();
	}

	// Token: 0x06000399 RID: 921 RVA: 0x000150E4 File Offset: 0x000132E4
	private static void __rpc_handler_408477299(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		float volume;
		reader.ReadValueSafe<float>(out volume, default(FastBufferWriter.ForPrimitives));
		float pitch;
		reader.ReadValueSafe<float>(out pitch, default(FastBufferWriter.ForPrimitives));
		bool isOneShot;
		reader.ReadValueSafe<bool>(out isOneShot, default(FastBufferWriter.ForPrimitives));
		int num;
		ByteUnpacker.ReadValueBitPacked(reader, out num);
		float num2;
		reader.ReadValueSafe<float>(out num2, default(FastBufferWriter.ForPrimitives));
		bool flag;
		reader.ReadValueSafe<bool>(out flag, default(FastBufferWriter.ForPrimitives));
		float num3;
		reader.ReadValueSafe<float>(out num3, default(FastBufferWriter.ForPrimitives));
		bool flag2;
		reader.ReadValueSafe<bool>(out flag2, default(FastBufferWriter.ForPrimitives));
		float num4;
		reader.ReadValueSafe<float>(out num4, default(FastBufferWriter.ForPrimitives));
		float num5;
		reader.ReadValueSafe<float>(out num5, default(FastBufferWriter.ForPrimitives));
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((SynchronizedAudio)target).Server_PlayRpc(volume, pitch, isOneShot, num, num2, flag, num3, flag2, num4, num5, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600039A RID: 922 RVA: 0x0001526B File Offset: 0x0001346B
	protected internal override string __getTypeName()
	{
		return "SynchronizedAudio";
	}

	// Token: 0x04000273 RID: 627
	[Header("Settings")]
	[SerializeField]
	private bool stopOnDespawn = true;

	// Token: 0x04000274 RID: 628
	[Header("References")]
	[SerializeField]
	private List<AudioClip> audioClips = new List<AudioClip>();

	// Token: 0x04000275 RID: 629
	private NetworkVariable<byte> Volume;

	// Token: 0x04000276 RID: 630
	private NetworkVariable<byte> Pitch;

	// Token: 0x04000277 RID: 631
	private bool isNetworkVariablesInitialized;

	// Token: 0x04000278 RID: 632
	private AudioSource audioSource;

	// Token: 0x04000279 RID: 633
	private int clipIndex;

	// Token: 0x0400027A RID: 634
	private float time;

	// Token: 0x0400027B RID: 635
	private bool fadeIn;

	// Token: 0x0400027C RID: 636
	private float fadeInDuration;

	// Token: 0x0400027D RID: 637
	private bool fadeOut;

	// Token: 0x0400027E RID: 638
	private float fadeOutDuration;

	// Token: 0x0400027F RID: 639
	private float duration;

	// Token: 0x04000280 RID: 640
	private bool isPlaying;

	// Token: 0x04000281 RID: 641
	private float initialVolume;

	// Token: 0x04000282 RID: 642
	private float initialPitch;

	// Token: 0x04000283 RID: 643
	private Sequence audioSourceSequence;
}
