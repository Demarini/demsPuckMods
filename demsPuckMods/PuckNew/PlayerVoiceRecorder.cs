using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Steamworks;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200003E RID: 62
public class PlayerVoiceRecorder : NetworkBehaviour
{
	// Token: 0x060001ED RID: 493 RVA: 0x0000CB5D File Offset: 0x0000AD5D
	private void Awake()
	{
		this.Player = base.GetComponent<Player>();
	}

	// Token: 0x060001EE RID: 494 RVA: 0x0000CB6B File Offset: 0x0000AD6B
	private void Update()
	{
		if (this.Player.IsLocalPlayer && this.IsRecording)
		{
			this.Client_SendVoiceData();
		}
	}

	// Token: 0x060001EF RID: 495 RVA: 0x0000CB88 File Offset: 0x0000AD88
	private AudioClip InitializeAudioClip(uint sampleRate)
	{
		this.audioQueue = new ConcurrentQueue<float>();
		return AudioClip.Create("VoiceData", (int)sampleRate, 1, (int)sampleRate, true, new AudioClip.PCMReaderCallback(this.OnAudioRead), null);
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x0000CBB0 File Offset: 0x0000ADB0
	private void OnAudioRead(float[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			float num;
			if (this.audioQueue != null && this.audioQueue.TryDequeue(out num))
			{
				data[i] = num;
			}
			else
			{
				data[i] = 0f;
			}
		}
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x0000CBF0 File Offset: 0x0000ADF0
	private void WriteToClip(byte[] decompressed, int bytesWritten)
	{
		int num = 0;
		while (num < bytesWritten - 1 && num + 1 < decompressed.Length)
		{
			float f = (float)((short)((int)decompressed[num] | (int)decompressed[num + 1] << 8)) / 32768f;
			this.WriteToClip(f);
			num += 2;
		}
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x0000CC2E File Offset: 0x0000AE2E
	private void WriteToClip(float f)
	{
		if (this.audioQueue != null)
		{
			this.audioQueue.Enqueue(f);
		}
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x0000CC44 File Offset: 0x0000AE44
	private void Client_SendVoiceData()
	{
		uint num;
		SteamUser.GetAvailableVoice(out num);
		if (num > 0U)
		{
			byte[] array = new byte[num];
			uint num2;
			SteamUser.GetVoice(true, array, num, out num2);
			if (num2 > 0U)
			{
				Debug.Log(string.Format("[PlayerVoiceRecorder] Sending voice data to server ({0}b)", array.Length));
				this.Client_VoiceDataRpc(array, default(RpcParams));
			}
		}
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x0000CC9C File Offset: 0x0000AE9C
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_RequestVoiceStartRpc(uint sampleRate, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 1666941424U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter writer = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			BytePacker.WriteValueBitPacked(writer, sampleRate);
			base.__endSendRpc(ref writer, 1666941424U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId)
		{
			return;
		}
		if (!this.IsEnabled)
		{
			return;
		}
		if (this.IsRecording)
		{
			return;
		}
		this.Server_VoiceStartRpc(sampleRate);
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x0000CDA0 File Offset: 0x0000AFA0
	[Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server, DeferLocal = true)]
	public void Server_VoiceStartRpc(uint sampleRate)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 236455107U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true
			};
			FastBufferWriter writer = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Everyone, RpcDelivery.Reliable);
			BytePacker.WriteValueBitPacked(writer, sampleRate);
			base.__endSendRpc(ref writer, 236455107U, rpcParams2, attributeParams, SendTo.Everyone, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (this.Player.IsMuted.Value)
		{
			Debug.Log(string.Format("[PlayerVoiceRecorder] Player is muted, not starting voice recording ({0})", base.OwnerClientId));
			return;
		}
		AudioClip value = this.InitializeAudioClip(sampleRate);
		if (this.Player.IsLocalPlayer)
		{
			Debug.Log(string.Format("[PlayerVoiceRecorder] Recording Steam voice at {0}Hz", sampleRate));
			SteamUser.StartVoiceRecording();
		}
		Debug.Log(string.Format("[PlayerVoiceRecorder] Player started voice recording ({0})", base.OwnerClientId));
		this.IsRecording = true;
		this.SampleRate = sampleRate;
		EventManager.TriggerEvent("Event_Everyone_OnPlayerVoiceStarted", new Dictionary<string, object>
		{
			{
				"player",
				this.Player
			},
			{
				"audioClip",
				value
			}
		});
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x0000CF38 File Offset: 0x0000B138
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_VoiceDataRpc(byte[] voiceData, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 198182109U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			bool flag = voiceData != null;
			fastBufferWriter.WriteValueSafe<bool>(flag, default(FastBufferWriter.ForPrimitives));
			if (flag)
			{
				fastBufferWriter.WriteValueSafe<byte>(voiceData, default(FastBufferWriter.ForPrimitives));
			}
			base.__endSendRpc(ref fastBufferWriter, 198182109U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (this.Player.IsMuted.Value)
		{
			return;
		}
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId)
		{
			return;
		}
		if (!this.IsRecording)
		{
			return;
		}
		this.Server_VoiceDataRpc(voiceData);
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x0000D084 File Offset: 0x0000B284
	[Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Server, DeferLocal = true)]
	public void Server_VoiceDataRpc(byte[] voiceData)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2054004997U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.ClientsAndHost, RpcDelivery.Reliable);
			bool flag = voiceData != null;
			fastBufferWriter.WriteValueSafe<bool>(flag, default(FastBufferWriter.ForPrimitives));
			if (flag)
			{
				fastBufferWriter.WriteValueSafe<byte>(voiceData, default(FastBufferWriter.ForPrimitives));
			}
			base.__endSendRpc(ref fastBufferWriter, 2054004997U, rpcParams2, attributeParams, SendTo.ClientsAndHost, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		byte[] array = new byte[this.SampleRate * 2U];
		uint num;
		SteamUser.DecompressVoice(voiceData, (uint)voiceData.Length, array, (uint)array.Length, out num, this.SampleRate);
		if (num > 0U)
		{
			this.WriteToClip(array, (int)num);
		}
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x0000D1D8 File Offset: 0x0000B3D8
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_RequestVoiceStopRpc(RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2728922553U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			base.__endSendRpc(ref fastBufferWriter, 2728922553U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId)
		{
			return;
		}
		if (!this.IsRecording)
		{
			return;
		}
		this.Server_VoiceStopRpc();
	}

	// Token: 0x060001F9 RID: 505 RVA: 0x0000D2C8 File Offset: 0x0000B4C8
	[Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server, DeferLocal = true)]
	public void Server_VoiceStopRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 824274507U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Everyone, RpcDelivery.Reliable);
			base.__endSendRpc(ref fastBufferWriter, 824274507U, rpcParams2, attributeParams, SendTo.Everyone, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (this.Player.IsLocalPlayer)
		{
			Debug.Log("[PlayerVoiceRecorder] Stopping Steam voice recording");
			SteamUser.StopVoiceRecording();
		}
		Debug.Log(string.Format("[PlayerVoiceRecorder] Player stopped voice recording ({0})", base.OwnerClientId));
		this.IsRecording = false;
		EventManager.TriggerEvent("Event_Everyone_OnPlayerVoiceStopped", new Dictionary<string, object>
		{
			{
				"player",
				this.Player
			}
		});
	}

	// Token: 0x060001FB RID: 507 RVA: 0x0000D400 File Offset: 0x0000B600
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060001FC RID: 508 RVA: 0x0000D418 File Offset: 0x0000B618
	protected override void __initializeRpcs()
	{
		base.__registerRpc(1666941424U, new NetworkBehaviour.RpcReceiveHandler(PlayerVoiceRecorder.__rpc_handler_1666941424), "Client_RequestVoiceStartRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(236455107U, new NetworkBehaviour.RpcReceiveHandler(PlayerVoiceRecorder.__rpc_handler_236455107), "Server_VoiceStartRpc", RpcInvokePermission.Server);
		base.__registerRpc(198182109U, new NetworkBehaviour.RpcReceiveHandler(PlayerVoiceRecorder.__rpc_handler_198182109), "Client_VoiceDataRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(2054004997U, new NetworkBehaviour.RpcReceiveHandler(PlayerVoiceRecorder.__rpc_handler_2054004997), "Server_VoiceDataRpc", RpcInvokePermission.Server);
		base.__registerRpc(2728922553U, new NetworkBehaviour.RpcReceiveHandler(PlayerVoiceRecorder.__rpc_handler_2728922553), "Client_RequestVoiceStopRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(824274507U, new NetworkBehaviour.RpcReceiveHandler(PlayerVoiceRecorder.__rpc_handler_824274507), "Server_VoiceStopRpc", RpcInvokePermission.Server);
		base.__initializeRpcs();
	}

	// Token: 0x060001FD RID: 509 RVA: 0x0000D4F4 File Offset: 0x0000B6F4
	private static void __rpc_handler_1666941424(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		uint sampleRate;
		ByteUnpacker.ReadValueBitPacked(reader, out sampleRate);
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((PlayerVoiceRecorder)target).Client_RequestVoiceStartRpc(sampleRate, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001FE RID: 510 RVA: 0x0000D564 File Offset: 0x0000B764
	private static void __rpc_handler_236455107(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		uint sampleRate;
		ByteUnpacker.ReadValueBitPacked(reader, out sampleRate);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((PlayerVoiceRecorder)target).Server_VoiceStartRpc(sampleRate);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001FF RID: 511 RVA: 0x0000D5C8 File Offset: 0x0000B7C8
	private static void __rpc_handler_198182109(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool flag;
		reader.ReadValueSafe<bool>(out flag, default(FastBufferWriter.ForPrimitives));
		byte[] voiceData = null;
		if (flag)
		{
			reader.ReadValueSafe<byte>(out voiceData, default(FastBufferWriter.ForPrimitives));
		}
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((PlayerVoiceRecorder)target).Client_VoiceDataRpc(voiceData, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000200 RID: 512 RVA: 0x0000D670 File Offset: 0x0000B870
	private static void __rpc_handler_2054004997(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool flag;
		reader.ReadValueSafe<bool>(out flag, default(FastBufferWriter.ForPrimitives));
		byte[] voiceData = null;
		if (flag)
		{
			reader.ReadValueSafe<byte>(out voiceData, default(FastBufferWriter.ForPrimitives));
		}
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((PlayerVoiceRecorder)target).Server_VoiceDataRpc(voiceData);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000201 RID: 513 RVA: 0x0000D70C File Offset: 0x0000B90C
	private static void __rpc_handler_2728922553(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((PlayerVoiceRecorder)target).Client_RequestVoiceStopRpc(ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000202 RID: 514 RVA: 0x0000D76C File Offset: 0x0000B96C
	private static void __rpc_handler_824274507(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((PlayerVoiceRecorder)target).Server_VoiceStopRpc();
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000203 RID: 515 RVA: 0x0000D7BD File Offset: 0x0000B9BD
	protected internal override string __getTypeName()
	{
		return "PlayerVoiceRecorder";
	}

	// Token: 0x04000148 RID: 328
	[HideInInspector]
	public Player Player;

	// Token: 0x04000149 RID: 329
	[HideInInspector]
	public bool IsEnabled;

	// Token: 0x0400014A RID: 330
	[HideInInspector]
	public bool IsRecording;

	// Token: 0x0400014B RID: 331
	[HideInInspector]
	public uint SampleRate;

	// Token: 0x0400014C RID: 332
	private ConcurrentQueue<float> audioQueue;
}
