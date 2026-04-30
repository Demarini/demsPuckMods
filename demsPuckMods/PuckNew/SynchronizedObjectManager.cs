using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200013A RID: 314
public class SynchronizedObjectManager : NetworkBehaviourSingleton<SynchronizedObjectManager>
{
	// Token: 0x170000EF RID: 239
	// (get) Token: 0x0600094C RID: 2380 RVA: 0x0002CE3B File Offset: 0x0002B03B
	// (set) Token: 0x0600094D RID: 2381 RVA: 0x0002CE44 File Offset: 0x0002B044
	[HideInInspector]
	public int TickRate
	{
		get
		{
			return this.tickRate;
		}
		set
		{
			if (this.tickRate == value)
			{
				return;
			}
			this.driftEma = new ExponentialMovingAverage(value * this.snapshotInterpolationSettings.driftEmaDuration);
			this.deliveryTimeEma = new ExponentialMovingAverage(value * this.snapshotInterpolationSettings.deliveryTimeEmaDuration);
			this.tickRate = value;
		}
	}

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x0600094E RID: 2382 RVA: 0x0002CE92 File Offset: 0x0002B092
	// (set) Token: 0x0600094F RID: 2383 RVA: 0x0002CE9A File Offset: 0x0002B09A
	[HideInInspector]
	public bool UseNetworkSmoothing
	{
		get
		{
			return this.useNetworkSmoothing;
		}
		set
		{
			if (this.useNetworkSmoothing != value)
			{
				this.snapshots.Clear();
			}
			this.useNetworkSmoothing = value;
		}
	}

	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x06000950 RID: 2384 RVA: 0x0002CEB7 File Offset: 0x0002B0B7
	[HideInInspector]
	public float TickInterval
	{
		get
		{
			return 1f / (float)this.TickRate;
		}
	}

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x06000951 RID: 2385 RVA: 0x0002CEC6 File Offset: 0x0002B0C6
	private double clientBufferTime
	{
		get
		{
			return (double)this.TickInterval * this.snapshotInterpolationSettings.bufferTimeMultiplier;
		}
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x0002CEDC File Offset: 0x0002B0DC
	public override void Awake()
	{
		base.Awake();
		this.driftEma = new ExponentialMovingAverage(this.TickRate * this.snapshotInterpolationSettings.driftEmaDuration);
		this.deliveryTimeEma = new ExponentialMovingAverage(this.TickRate * this.snapshotInterpolationSettings.deliveryTimeEmaDuration);
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x0002CF2C File Offset: 0x0002B12C
	private void Update()
	{
		if (!base.IsSpawned)
		{
			return;
		}
		if (NetworkManager.Singleton.IsServer)
		{
			this.serverTickAccumulator += Time.deltaTime * (float)this.TickRate;
			if (this.serverTickAccumulator >= 1f)
			{
				while (this.serverTickAccumulator >= 1f)
				{
					this.serverTickAccumulator -= 1f;
				}
				this.Server_ServerTick();
				return;
			}
		}
		else if (this.UseNetworkSmoothing)
		{
			this.clientAccumulatedDeltaTime += Time.unscaledDeltaTime;
			if (this.snapshots.Count > 0)
			{
				SynchronizedObjectsSnapshot from;
				SynchronizedObjectsSnapshot to;
				double t;
				SnapshotInterpolation.Step<SynchronizedObjectsSnapshot>(this.snapshots, (double)this.clientAccumulatedDeltaTime, ref this.clientLocalTimeline, this.clientLocalTimescale, out from, out to, out t);
				SynchronizedObjectsSnapshot.Interpolate(from, to, t);
				this.clientAccumulatedDeltaTime = 0f;
			}
		}
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x0002D000 File Offset: 0x0002B200
	public void Dispose()
	{
		this.serverTickAccumulator = 0f;
		this.serverLastSentTickId = 0;
		this.clientLastReceivedTickId = 0;
		this.clientHasReceivedFirstTick = false;
		this.clientAccumulatedDeltaTime = 0f;
		this.snapshots.Clear();
		this.ClearSynchronizedObjects();
		this.ClearSynchronizedClientIds();
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x0002D04F File Offset: 0x0002B24F
	public void AddSynchronizedObject(SynchronizedObject synchronizedObject)
	{
		this.synchronizedObjects.Add(synchronizedObject);
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x0002D05D File Offset: 0x0002B25D
	public void RemoveSynchronizedObject(SynchronizedObject synchronizedObject)
	{
		this.synchronizedObjects.Remove(synchronizedObject);
	}

	// Token: 0x06000957 RID: 2391 RVA: 0x0002D06C File Offset: 0x0002B26C
	private void ClearSynchronizedObjects()
	{
		this.synchronizedObjects.Clear();
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x0002D079 File Offset: 0x0002B279
	public void Server_AddSynchronizedClientId(ulong clientId)
	{
		this.synchronizedClientIds.Add(clientId);
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x0002D087 File Offset: 0x0002B287
	public void Server_RemoveSynchronizedClientId(ulong clientId)
	{
		this.synchronizedClientIds.Remove(clientId);
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x0002D096 File Offset: 0x0002B296
	private void ClearSynchronizedClientIds()
	{
		this.synchronizedClientIds.Clear();
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x0002D0A4 File Offset: 0x0002B2A4
	private ValueTuple<ushort, short[], short[]> EncodeSynchronizedObject(ulong networkObjectId, Vector3 position, Quaternion rotation)
	{
		short num = (short)(rotation.x * 32767f);
		short num2 = (short)(rotation.y * 32767f);
		short num3 = (short)(rotation.z * 32767f);
		short num4 = (short)(rotation.w * 32767f);
		return new ValueTuple<ushort, short[], short[]>((ushort)networkObjectId, new short[]
		{
			(short)(position.x * 655f),
			(short)(position.y * 655f),
			(short)(position.z * 655f)
		}, new short[]
		{
			num,
			num2,
			num3,
			num4
		});
	}

	// Token: 0x0600095C RID: 2396 RVA: 0x0002D13C File Offset: 0x0002B33C
	private ValueTuple<ushort, Vector3, Quaternion> DecodeSynchronizedObjectData(SynchronizedObjectData synchronizedObjectData)
	{
		float x = (float)synchronizedObjectData.Rx / 32767f;
		float y = (float)synchronizedObjectData.Ry / 32767f;
		float z = (float)synchronizedObjectData.Rz / 32767f;
		float w = (float)synchronizedObjectData.Rw / 32767f;
		Quaternion item = new Quaternion(x, y, z, w);
		return new ValueTuple<ushort, Vector3, Quaternion>(synchronizedObjectData.NetworkObjectId, new Vector3((float)synchronizedObjectData.X / 655f, (float)synchronizedObjectData.Y / 655f, (float)synchronizedObjectData.Z / 655f), item);
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x0002D1C8 File Offset: 0x0002B3C8
	private void Server_ServerTick()
	{
		this.serverLastSentTickId += 1;
		if (this.serverLastSentTickId >= 65535)
		{
			this.serverLastSentTickId = 0;
		}
		SynchronizedObjectData[] synchronizedObjectsData = this.Server_GatherSynchronizedObjectData(false);
		this.serverLastSentServerTime = NetworkManager.Singleton.ServerTime.Time;
		this.Server_SynchronizeObjectsRpc(this.serverLastSentTickId, this.serverLastSentServerTime, synchronizedObjectsData, base.RpcTarget.Group<List<ulong>>(this.synchronizedClientIds, RpcTargetUse.Persistent));
	}

	// Token: 0x0600095E RID: 2398 RVA: 0x0002D244 File Offset: 0x0002B444
	public void Server_ForceSynchronizeClientId(ulong clientId)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		SynchronizedObjectData[] synchronizedObjectsData = this.Server_GatherSynchronizedObjectData(true);
		this.Server_SynchronizeObjectsRpc(this.serverLastSentTickId, this.serverLastSentServerTime, synchronizedObjectsData, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
	}

	// Token: 0x0600095F RID: 2399 RVA: 0x0002D28C File Offset: 0x0002B48C
	private SynchronizedObjectData[] Server_GatherSynchronizedObjectData(bool forceAllObjects = false)
	{
		List<SynchronizedObjectData> list = new List<SynchronizedObjectData>();
		foreach (SynchronizedObject synchronizedObject in this.synchronizedObjects)
		{
			if (synchronizedObject && (forceAllObjects || synchronizedObject.ShouldSendPosition(this.TickRate) || synchronizedObject.ShouldSendRotation(this.TickRate)))
			{
				ValueTuple<Vector3, Quaternion, ulong> valueTuple = synchronizedObject.OnServerTick((float)(NetworkManager.Singleton.ServerTime.Time - this.serverLastSentServerTime));
				Vector3 item = valueTuple.Item1;
				Quaternion item2 = valueTuple.Item2;
				ulong item3 = valueTuple.Item3;
				ValueTuple<ushort, short[], short[]> valueTuple2 = this.EncodeSynchronizedObject(item3, item, item2);
				ushort item4 = valueTuple2.Item1;
				short[] item5 = valueTuple2.Item2;
				short[] item6 = valueTuple2.Item3;
				list.Add(new SynchronizedObjectData
				{
					NetworkObjectId = item4,
					X = item5[0],
					Y = item5[1],
					Z = item5[2],
					Rx = item6[0],
					Ry = item6[1],
					Rz = item6[2],
					Rw = item6[3]
				});
			}
		}
		return list.ToArray();
	}

	// Token: 0x06000960 RID: 2400 RVA: 0x0002D3E4 File Offset: 0x0002B5E4
	[Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server, Delivery = RpcDelivery.Unreliable, DeferLocal = true)]
	private void Server_SynchronizeObjectsRpc(ushort tickId, double serverTime, SynchronizedObjectData[] synchronizedObjectsData, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 1738927239U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				Delivery = RpcDelivery.Unreliable,
				DeferLocal = true
			};
			FastBufferWriter writer = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Unreliable);
			BytePacker.WriteValueBitPacked(writer, tickId);
			writer.WriteValueSafe<double>(serverTime, default(FastBufferWriter.ForPrimitives));
			bool flag = synchronizedObjectsData != null;
			writer.WriteValueSafe<bool>(flag, default(FastBufferWriter.ForPrimitives));
			if (flag)
			{
				writer.WriteValueSafe<SynchronizedObjectData>(synchronizedObjectsData, default(FastBufferWriter.ForNetworkSerializable));
			}
			base.__endSendRpc(ref writer, 1738927239U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Unreliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (this.skipLateTicks && this.clientHasReceivedFirstTick && this.clientLastReceivedTickId - tickId < 32767 && tickId <= this.clientLastReceivedTickId)
		{
			Debug.Log(string.Format("[SynchronizedObjectManager] Dropped tick {0} because it's older than the last received tick {1}", tickId, this.clientLastReceivedTickId));
			return;
		}
		float num = (float)(serverTime - this.clientLastReceivedServerTime);
		this.Client_SynchronizeObjects(synchronizedObjectsData, num, serverTime);
		if (!this.clientHasReceivedFirstTick)
		{
			this.clientHasReceivedFirstTick = true;
		}
		this.clientLastReceivedTickId = tickId;
		this.clientLastReceivedServerTime = serverTime;
		EventManager.TriggerEvent("Event_OnSynchronizeObjects", new Dictionary<string, object>
		{
			{
				"serverDeltaTime",
				num
			}
		});
	}

	// Token: 0x06000961 RID: 2401 RVA: 0x0002D5D8 File Offset: 0x0002B7D8
	private void Client_SynchronizeObjects(SynchronizedObjectData[] synchronizedObjectsData, float serverDeltaTime, double serverTime)
	{
		if (this.UseNetworkSmoothing && this.clientHasReceivedFirstTick)
		{
			List<SynchronizedObjectSnapshot> list = new List<SynchronizedObjectSnapshot>();
			for (int i = 0; i < synchronizedObjectsData.Length; i++)
			{
				SynchronizedObjectData synchronizedObjectData = synchronizedObjectsData[i];
				ValueTuple<ushort, Vector3, Quaternion> valueTuple = this.DecodeSynchronizedObjectData(synchronizedObjectData);
				ushort networkObjectId = valueTuple.Item1;
				Vector3 item = valueTuple.Item2;
				Quaternion item2 = valueTuple.Item3;
				SynchronizedObject synchronizedObject3 = this.synchronizedObjects.Find((SynchronizedObject synchronizedObject) => synchronizedObject.NetworkObjectId == (ulong)networkObjectId);
				if (!(synchronizedObject3 == null))
				{
					SynchronizedObjectSnapshot item3 = synchronizedObject3.OnClientSmoothTick(item, item2, synchronizedObject3, serverDeltaTime);
					list.Add(item3);
				}
			}
			SynchronizedObjectsSnapshot snapshot = new SynchronizedObjectsSnapshot(serverTime, NetworkManager.Singleton.LocalTime.Time, list);
			if (this.snapshotInterpolationSettings.dynamicAdjustment)
			{
				this.snapshotInterpolationSettings.bufferTimeMultiplier = SnapshotInterpolation.DynamicAdjustment((double)this.TickInterval, this.deliveryTimeEma.StandardDeviation, (double)this.snapshotInterpolationSettings.dynamicAdjustmentTolerance) * (double)this.NetworkSmoothingStrength;
			}
			SnapshotInterpolation.InsertAndAdjust<SynchronizedObjectsSnapshot>(this.snapshots, this.snapshotInterpolationSettings.bufferLimit, snapshot, ref this.clientLocalTimeline, ref this.clientLocalTimescale, this.TickInterval, this.clientBufferTime, this.snapshotInterpolationSettings.catchupSpeed, this.snapshotInterpolationSettings.slowdownSpeed, ref this.driftEma, this.snapshotInterpolationSettings.catchupNegativeThreshold, this.snapshotInterpolationSettings.catchupPositiveThreshold, ref this.deliveryTimeEma);
			return;
		}
		for (int i = 0; i < synchronizedObjectsData.Length; i++)
		{
			SynchronizedObjectData synchronizedObjectData2 = synchronizedObjectsData[i];
			ValueTuple<ushort, Vector3, Quaternion> valueTuple = this.DecodeSynchronizedObjectData(synchronizedObjectData2);
			ushort networkObjectId = valueTuple.Item1;
			Vector3 item4 = valueTuple.Item2;
			Quaternion item5 = valueTuple.Item3;
			SynchronizedObject synchronizedObject2 = this.synchronizedObjects.Find((SynchronizedObject synchronizedObject) => synchronizedObject.NetworkObjectId == (ulong)networkObjectId);
			if (!(synchronizedObject2 == null))
			{
				synchronizedObject2.OnClientTick(item4, item5, serverDeltaTime);
			}
		}
	}

	// Token: 0x06000962 RID: 2402 RVA: 0x0002D7C8 File Offset: 0x0002B9C8
	private void OnValidate()
	{
		this.snapshotInterpolationSettings.catchupNegativeThreshold = Mathf.Min(this.snapshotInterpolationSettings.catchupNegativeThreshold, 0f);
		this.snapshotInterpolationSettings.catchupPositiveThreshold = Mathf.Max(this.snapshotInterpolationSettings.catchupPositiveThreshold, 0f);
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x0002D874 File Offset: 0x0002BA74
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x0002D88A File Offset: 0x0002BA8A
	protected override void __initializeRpcs()
	{
		base.__registerRpc(1738927239U, new NetworkBehaviour.RpcReceiveHandler(SynchronizedObjectManager.__rpc_handler_1738927239), "Server_SynchronizeObjectsRpc", RpcInvokePermission.Server);
		base.__initializeRpcs();
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x0002D8B8 File Offset: 0x0002BAB8
	private static void __rpc_handler_1738927239(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		ushort tickId;
		ByteUnpacker.ReadValueBitPacked(reader, out tickId);
		double serverTime;
		reader.ReadValueSafe<double>(out serverTime, default(FastBufferWriter.ForPrimitives));
		bool flag;
		reader.ReadValueSafe<bool>(out flag, default(FastBufferWriter.ForPrimitives));
		SynchronizedObjectData[] synchronizedObjectsData = null;
		if (flag)
		{
			reader.ReadValueSafe<SynchronizedObjectData>(out synchronizedObjectsData, default(FastBufferWriter.ForNetworkSerializable));
		}
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((SynchronizedObjectManager)target).Server_SynchronizeObjectsRpc(tickId, serverTime, synchronizedObjectsData, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x0002D990 File Offset: 0x0002BB90
	protected internal override string __getTypeName()
	{
		return "SynchronizedObjectManager";
	}

	// Token: 0x0400055B RID: 1371
	[Header("Settings")]
	[SerializeField]
	private SnapshotInterpolationSettings snapshotInterpolationSettings;

	// Token: 0x0400055C RID: 1372
	[SerializeField]
	private bool skipLateTicks = true;

	// Token: 0x0400055D RID: 1373
	private int tickRate = 100;

	// Token: 0x0400055E RID: 1374
	private bool useNetworkSmoothing;

	// Token: 0x0400055F RID: 1375
	[HideInInspector]
	public int NetworkSmoothingStrength = 1;

	// Token: 0x04000560 RID: 1376
	private float serverTickAccumulator;

	// Token: 0x04000561 RID: 1377
	private ushort serverLastSentTickId;

	// Token: 0x04000562 RID: 1378
	private double serverLastSentServerTime;

	// Token: 0x04000563 RID: 1379
	private ushort clientLastReceivedTickId;

	// Token: 0x04000564 RID: 1380
	private double clientLastReceivedServerTime;

	// Token: 0x04000565 RID: 1381
	private bool clientHasReceivedFirstTick;

	// Token: 0x04000566 RID: 1382
	private float clientAccumulatedDeltaTime;

	// Token: 0x04000567 RID: 1383
	private double clientLocalTimeline;

	// Token: 0x04000568 RID: 1384
	private double clientLocalTimescale = 1.0;

	// Token: 0x04000569 RID: 1385
	private List<SynchronizedObject> synchronizedObjects = new List<SynchronizedObject>();

	// Token: 0x0400056A RID: 1386
	private List<ulong> synchronizedClientIds = new List<ulong>();

	// Token: 0x0400056B RID: 1387
	private SortedList<double, SynchronizedObjectsSnapshot> snapshots = new SortedList<double, SynchronizedObjectsSnapshot>();

	// Token: 0x0400056C RID: 1388
	private ExponentialMovingAverage driftEma;

	// Token: 0x0400056D RID: 1389
	private ExponentialMovingAverage deliveryTimeEma;
}
