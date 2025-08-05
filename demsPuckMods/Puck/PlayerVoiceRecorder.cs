using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000E8 RID: 232
public class PlayerVoiceRecorder : NetworkBehaviour
{
	// Token: 0x060007AC RID: 1964 RVA: 0x0000BC08 File Offset: 0x00009E08
	private void Awake()
	{
		this.Player = base.GetComponent<Player>();
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x0000BC16 File Offset: 0x00009E16
	private void Update()
	{
		if (!this.IsEnabled)
		{
			return;
		}
		if (!this.Player)
		{
			return;
		}
		if (!base.IsOwner)
		{
			return;
		}
		if (!this.IsRecording)
		{
			return;
		}
		if (MonoBehaviourSingleton<StateManager>.Instance.IsMuted)
		{
			return;
		}
		this.SendVoiceData();
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x0000BC54 File Offset: 0x00009E54
	public override void OnDestroy()
	{
		this.StopDelayedStopRecordingCoroutine();
		if (this.IsRecording)
		{
			SteamUser.StopVoiceRecording();
		}
		base.OnDestroy();
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x0002E204 File Offset: 0x0002C404
	private void SendVoiceData()
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
				this.Client_VoiceDataRpc(array);
			}
		}
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x0002E23C File Offset: 0x0002C43C
	private AudioClip InitializeAudioClip(int sampleRate)
	{
		this.playbackBuffer = 0;
		this.dataPosition = 0;
		this.dataReceived = 0;
		this.bufferSize = sampleRate * 2;
		this.buffer = new float[this.bufferSize];
		return AudioClip.Create("VoiceData", sampleRate, 1, sampleRate, true, new AudioClip.PCMReaderCallback(this.OnAudioRead), null);
	}

	// Token: 0x060007B1 RID: 1969 RVA: 0x0002E294 File Offset: 0x0002C494
	private void OnAudioRead(float[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			data[i] = 0f;
			if (this.playbackBuffer > 0)
			{
				this.dataPosition++;
				this.playbackBuffer--;
				data[i] = this.buffer[this.dataPosition % this.bufferSize];
			}
		}
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x0002E2F4 File Offset: 0x0002C4F4
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

	// Token: 0x060007B3 RID: 1971 RVA: 0x0000BC6F File Offset: 0x00009E6F
	private void WriteToClip(float f)
	{
		this.buffer[this.dataReceived % this.bufferSize] = f;
		this.dataReceived++;
		this.playbackBuffer++;
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x0002E334 File Offset: 0x0002C534
	public void StartRecording()
	{
		if (!this.IsEnabled)
		{
			return;
		}
		if (MonoBehaviourSingleton<StateManager>.Instance.IsMuted)
		{
			return;
		}
		if (this.IsRecording)
		{
			this.StopDelayedStopRecordingCoroutine();
			return;
		}
		this.IsRecording = true;
		this.sampleRate = SteamUser.GetVoiceOptimalSampleRate();
		SteamUser.StartVoiceRecording();
		this.Client_VoiceStartRpc(this.sampleRate);
		Debug.Log(string.Format("[PlayerVoiceRecorder] Starting Steam voice recording at {0}Hz", this.sampleRate));
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x0000BCA2 File Offset: 0x00009EA2
	public void StopRecording()
	{
		if (!this.IsEnabled)
		{
			return;
		}
		if (MonoBehaviourSingleton<StateManager>.Instance.IsMuted)
		{
			return;
		}
		if (!this.IsRecording)
		{
			return;
		}
		this.StartDelayedStopRecordingCoroutine();
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x0000BCC9 File Offset: 0x00009EC9
	private void StartDelayedStopRecordingCoroutine()
	{
		this.StopDelayedStopRecordingCoroutine();
		this.delayedStopRecordingCoroutine = this.IDelayedStopRecording(this.recordingStopDelay);
		base.StartCoroutine(this.delayedStopRecordingCoroutine);
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x0000BCF0 File Offset: 0x00009EF0
	private void StopDelayedStopRecordingCoroutine()
	{
		if (this.delayedStopRecordingCoroutine != null)
		{
			base.StopCoroutine(this.delayedStopRecordingCoroutine);
		}
	}

	// Token: 0x060007B8 RID: 1976 RVA: 0x0000BD06 File Offset: 0x00009F06
	private IEnumerator IDelayedStopRecording(float delay)
	{
		Debug.Log(string.Format("[PlayerVoiceRecorder] Stopping Steam voice recording in {0} seconds", delay));
		yield return new WaitForSeconds(delay);
		this.IsRecording = false;
		SteamUser.StopVoiceRecording();
		this.Client_VoiceStopRpc();
		Debug.Log("[PlayerVoiceRecorder] Steam voice recording stopped");
		yield break;
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x0002E3A4 File Offset: 0x0002C5A4
	[Rpc(SendTo.Server)]
	public void Client_VoiceStartRpc(uint sampleRate)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2598611224U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter writer = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			BytePacker.WriteValueBitPacked(writer, sampleRate);
			base.__endSendRpc(ref writer, 2598611224U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (!this.IsEnabled)
		{
			return;
		}
		this.Server_VoiceStartRpc(sampleRate);
	}

	// Token: 0x060007BA RID: 1978 RVA: 0x0002E478 File Offset: 0x0002C678
	[Rpc(SendTo.ClientsAndHost)]
	public void Server_VoiceStartRpc(uint sampleRate)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 236455107U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter writer = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.ClientsAndHost, RpcDelivery.Reliable);
			BytePacker.WriteValueBitPacked(writer, sampleRate);
			base.__endSendRpc(ref writer, 236455107U, rpcParams2, attributeParams, SendTo.ClientsAndHost, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.sampleRate = sampleRate;
		AudioClip value = this.InitializeAudioClip((int)sampleRate);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerVoiceStarted", new Dictionary<string, object>
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

	// Token: 0x060007BB RID: 1979 RVA: 0x0002E57C File Offset: 0x0002C77C
	[Rpc(SendTo.Server)]
	public void Client_VoiceDataRpc(byte[] voiceData)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3881473849U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			bool flag = voiceData != null;
			fastBufferWriter.WriteValueSafe<bool>(flag, default(FastBufferWriter.ForPrimitives));
			if (flag)
			{
				fastBufferWriter.WriteValueSafe<byte>(voiceData, default(FastBufferWriter.ForPrimitives));
			}
			base.__endSendRpc(ref fastBufferWriter, 3881473849U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (!this.IsEnabled)
		{
			return;
		}
		this.Server_VoiceDataRpc(voiceData);
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x0002E690 File Offset: 0x0002C890
	[Rpc(SendTo.ClientsAndHost)]
	public void Server_VoiceDataRpc(byte[] voiceData)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2054004997U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
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
		byte[] array = new byte[this.sampleRate * 5U];
		uint num;
		SteamUser.DecompressVoice(voiceData, (uint)voiceData.Length, array, (uint)array.Length, out num, this.sampleRate);
		if (num > 0U)
		{
			this.WriteToClip(array, (int)num);
		}
	}

	// Token: 0x060007BD RID: 1981 RVA: 0x0002E7C4 File Offset: 0x0002C9C4
	[Rpc(SendTo.Server)]
	public void Client_VoiceStopRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 1645882530U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			base.__endSendRpc(ref fastBufferWriter, 1645882530U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (!this.IsEnabled)
		{
			return;
		}
		this.Server_VoiceStopRpc();
	}

	// Token: 0x060007BE RID: 1982 RVA: 0x0002E88C File Offset: 0x0002CA8C
	[Rpc(SendTo.ClientsAndHost)]
	public void Server_VoiceStopRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 824274507U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.ClientsAndHost, RpcDelivery.Reliable);
			base.__endSendRpc(ref fastBufferWriter, 824274507U, rpcParams2, attributeParams, SendTo.ClientsAndHost, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerVoiceStopped", new Dictionary<string, object>
		{
			{
				"player",
				this.Player
			}
		});
	}

	// Token: 0x060007C0 RID: 1984 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x0002E968 File Offset: 0x0002CB68
	protected override void __initializeRpcs()
	{
		base.__registerRpc(2598611224U, new NetworkBehaviour.RpcReceiveHandler(PlayerVoiceRecorder.__rpc_handler_2598611224), "Client_VoiceStartRpc");
		base.__registerRpc(236455107U, new NetworkBehaviour.RpcReceiveHandler(PlayerVoiceRecorder.__rpc_handler_236455107), "Server_VoiceStartRpc");
		base.__registerRpc(3881473849U, new NetworkBehaviour.RpcReceiveHandler(PlayerVoiceRecorder.__rpc_handler_3881473849), "Client_VoiceDataRpc");
		base.__registerRpc(2054004997U, new NetworkBehaviour.RpcReceiveHandler(PlayerVoiceRecorder.__rpc_handler_2054004997), "Server_VoiceDataRpc");
		base.__registerRpc(1645882530U, new NetworkBehaviour.RpcReceiveHandler(PlayerVoiceRecorder.__rpc_handler_1645882530), "Client_VoiceStopRpc");
		base.__registerRpc(824274507U, new NetworkBehaviour.RpcReceiveHandler(PlayerVoiceRecorder.__rpc_handler_824274507), "Server_VoiceStopRpc");
		base.__initializeRpcs();
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x0002EA28 File Offset: 0x0002CC28
	private static void __rpc_handler_2598611224(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		uint num;
		ByteUnpacker.ReadValueBitPacked(reader, out num);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((PlayerVoiceRecorder)target).Client_VoiceStartRpc(num);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060007C3 RID: 1987 RVA: 0x0002EA8C File Offset: 0x0002CC8C
	private static void __rpc_handler_236455107(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		uint num;
		ByteUnpacker.ReadValueBitPacked(reader, out num);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((PlayerVoiceRecorder)target).Server_VoiceStartRpc(num);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060007C4 RID: 1988 RVA: 0x0002EAF0 File Offset: 0x0002CCF0
	private static void __rpc_handler_3881473849(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((PlayerVoiceRecorder)target).Client_VoiceDataRpc(voiceData);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060007C5 RID: 1989 RVA: 0x0002EB8C File Offset: 0x0002CD8C
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

	// Token: 0x060007C6 RID: 1990 RVA: 0x0002EC28 File Offset: 0x0002CE28
	private static void __rpc_handler_1645882530(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((PlayerVoiceRecorder)target).Client_VoiceStopRpc();
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x0002EC7C File Offset: 0x0002CE7C
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

	// Token: 0x060007C8 RID: 1992 RVA: 0x0000BD2F File Offset: 0x00009F2F
	protected internal override string __getTypeName()
	{
		return "PlayerVoiceRecorder";
	}

	// Token: 0x0400046D RID: 1133
	[Header("Settings")]
	[SerializeField]
	private float recordingStopDelay = 1f;

	// Token: 0x0400046E RID: 1134
	[HideInInspector]
	public Player Player;

	// Token: 0x0400046F RID: 1135
	[HideInInspector]
	public bool IsEnabled;

	// Token: 0x04000470 RID: 1136
	[HideInInspector]
	public bool IsRecording;

	// Token: 0x04000471 RID: 1137
	private int bufferSize;

	// Token: 0x04000472 RID: 1138
	private float[] buffer;

	// Token: 0x04000473 RID: 1139
	private int playbackBuffer;

	// Token: 0x04000474 RID: 1140
	private int dataPosition;

	// Token: 0x04000475 RID: 1141
	private int dataReceived;

	// Token: 0x04000476 RID: 1142
	private uint sampleRate;

	// Token: 0x04000477 RID: 1143
	private IEnumerator delayedStopRecordingCoroutine;
}
