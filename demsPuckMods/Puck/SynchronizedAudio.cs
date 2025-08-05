using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000D1 RID: 209
[RequireComponent(typeof(AudioSource))]
public class SynchronizedAudio : NetworkBehaviour
{
	// Token: 0x0600063F RID: 1599 RVA: 0x0000AF1C File Offset: 0x0000911C
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.initialVolume = this.audioSource.volume;
		this.initialPitch = this.audioSource.pitch;
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x0002579C File Offset: 0x0002399C
	public override void OnNetworkSpawn()
	{
		this.Volume.Initialize(this);
		NetworkVariable<byte> volume = this.Volume;
		volume.OnValueChanged = (NetworkVariable<byte>.OnValueChangedDelegate)Delegate.Combine(volume.OnValueChanged, new NetworkVariable<byte>.OnValueChangedDelegate(this.OnVolumeChanged));
		this.Pitch.Initialize(this);
		NetworkVariable<byte> pitch = this.Pitch;
		pitch.OnValueChanged = (NetworkVariable<byte>.OnValueChangedDelegate)Delegate.Combine(pitch.OnValueChanged, new NetworkVariable<byte>.OnValueChangedDelegate(this.OnPitchChanged));
		if (NetworkManager.Singleton.IsServer)
		{
			this.Volume.Value = (byte)(this.initialVolume * 25f);
			this.Pitch.Value = (byte)(this.initialPitch * 25f);
		}
		this.Client_InitializeNetworkVariables();
		base.OnNetworkSpawn();
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x0000AF4C File Offset: 0x0000914C
	protected override void OnNetworkSessionSynchronized()
	{
		this.Client_InitializeNetworkVariables();
		base.OnNetworkSessionSynchronized();
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x00025858 File Offset: 0x00023A58
	public override void OnNetworkDespawn()
	{
		NetworkVariable<byte> volume = this.Volume;
		volume.OnValueChanged = (NetworkVariable<byte>.OnValueChangedDelegate)Delegate.Remove(volume.OnValueChanged, new NetworkVariable<byte>.OnValueChangedDelegate(this.OnVolumeChanged));
		this.Volume.Dispose();
		NetworkVariable<byte> pitch = this.Pitch;
		pitch.OnValueChanged = (NetworkVariable<byte>.OnValueChangedDelegate)Delegate.Remove(pitch.OnValueChanged, new NetworkVariable<byte>.OnValueChangedDelegate(this.OnPitchChanged));
		this.Pitch.Dispose();
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

	// Token: 0x06000643 RID: 1603 RVA: 0x0000AF5A File Offset: 0x0000915A
	public override void OnDestroy()
	{
		this.DOKill(false);
		base.OnDestroy();
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x00025904 File Offset: 0x00023B04
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

	// Token: 0x06000645 RID: 1605 RVA: 0x0000AF6A File Offset: 0x0000916A
	private void OnVolumeChanged(byte oldVolume, byte newVolume)
	{
		this.audioSource.volume = (float)newVolume / 25f;
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x0000AF7F File Offset: 0x0000917F
	private void OnPitchChanged(byte oldPitch, byte newPitch)
	{
		this.audioSource.pitch = (float)newPitch / 25f;
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x0000AF94 File Offset: 0x00009194
	public void Server_SetVolume(float volume)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Volume.Value = (byte)(volume * 25f);
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x0000AFB6 File Offset: 0x000091B6
	public void Server_SetPitch(float pitch)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Pitch.Value = (byte)(pitch * 25f);
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x00025960 File Offset: 0x00023B60
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
			clipIndex = UnityEngine.Random.Range(0, this.audioClips.Count);
		}
		float num = (clipIndex == -1) ? this.audioSource.clip.length : this.audioClips[clipIndex].length;
		if (randomTime)
		{
			time = UnityEngine.Random.Range(0f, Mathf.Max(num - duration - fadeInDuration - fadeOutDuration, 0f));
		}
		this.duration = ((duration == -1f) ? num : duration);
		if (!isOneShot)
		{
			this.Volume.Value = (byte)(volume * 25f);
			this.Pitch.Value = (byte)(pitch * 25f);
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

	// Token: 0x0600064A RID: 1610 RVA: 0x00025AB0 File Offset: 0x00023CB0
	[Rpc(SendTo.SpecifiedInParams)]
	private void Server_PlayRpc(float volume, float pitch, bool isOneShot, int clipIndex, float time, bool fadeIn, float fadeInDuration, bool fadeOut, float fadeOutDuration, float duration, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 408477299U;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
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

	// Token: 0x0600064B RID: 1611 RVA: 0x00025D90 File Offset: 0x00023F90
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
		this.Server_PlayRpc((float)this.Volume.Value / 25f, (float)this.Pitch.Value / 25f, false, this.clipIndex, this.time, false, this.fadeInDuration, this.fadeOut, this.fadeOutDuration, this.duration, base.RpcTarget.Single(clientId, RpcTargetUse.Temp));
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x0000AFD8 File Offset: 0x000091D8
	public void Client_InitializeNetworkVariables()
	{
		this.OnVolumeChanged(this.Volume.Value, this.Volume.Value);
		this.OnPitchChanged(this.Pitch.Value, this.Pitch.Value);
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x00025E18 File Offset: 0x00024018
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

	// Token: 0x06000654 RID: 1620 RVA: 0x0000B070 File Offset: 0x00009270
	protected override void __initializeRpcs()
	{
		base.__registerRpc(408477299U, new NetworkBehaviour.RpcReceiveHandler(SynchronizedAudio.__rpc_handler_408477299), "Server_PlayRpc");
		base.__initializeRpcs();
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x00025EC8 File Offset: 0x000240C8
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

	// Token: 0x06000656 RID: 1622 RVA: 0x0000B096 File Offset: 0x00009296
	protected internal override string __getTypeName()
	{
		return "SynchronizedAudio";
	}

	// Token: 0x04000366 RID: 870
	[Header("References")]
	[SerializeField]
	private List<AudioClip> audioClips = new List<AudioClip>();

	// Token: 0x04000367 RID: 871
	[Header("Settings")]
	[SerializeField]
	private bool stopOnDespawn = true;

	// Token: 0x04000368 RID: 872
	private NetworkVariable<byte> Volume = new NetworkVariable<byte>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x04000369 RID: 873
	private NetworkVariable<byte> Pitch = new NetworkVariable<byte>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x0400036A RID: 874
	private AudioSource audioSource;

	// Token: 0x0400036B RID: 875
	private int clipIndex;

	// Token: 0x0400036C RID: 876
	private float time;

	// Token: 0x0400036D RID: 877
	private bool fadeIn;

	// Token: 0x0400036E RID: 878
	private float fadeInDuration;

	// Token: 0x0400036F RID: 879
	private bool fadeOut;

	// Token: 0x04000370 RID: 880
	private float fadeOutDuration;

	// Token: 0x04000371 RID: 881
	private float duration;

	// Token: 0x04000372 RID: 882
	private bool isPlaying;

	// Token: 0x04000373 RID: 883
	private float initialVolume;

	// Token: 0x04000374 RID: 884
	private float initialPitch;

	// Token: 0x04000375 RID: 885
	private Sequence audioSourceSequence;
}
