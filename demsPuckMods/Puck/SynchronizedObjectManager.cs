using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000B7 RID: 183
public class SynchronizedObjectManager : NetworkBehaviourSingleton<SynchronizedObjectManager>
{
	// Token: 0x17000082 RID: 130
	// (get) Token: 0x0600056D RID: 1389 RVA: 0x0000A673 File Offset: 0x00008873
	// (set) Token: 0x0600056E RID: 1390 RVA: 0x0002244C File Offset: 0x0002064C
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

	// Token: 0x17000083 RID: 131
	// (get) Token: 0x0600056F RID: 1391 RVA: 0x0000A67B File Offset: 0x0000887B
	// (set) Token: 0x06000570 RID: 1392 RVA: 0x0000A683 File Offset: 0x00008883
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

	// Token: 0x17000084 RID: 132
	// (get) Token: 0x06000571 RID: 1393 RVA: 0x0000A6A0 File Offset: 0x000088A0
	[HideInInspector]
	public float TickInterval
	{
		get
		{
			return 1f / (float)this.TickRate;
		}
	}

	// Token: 0x17000085 RID: 133
	// (get) Token: 0x06000572 RID: 1394 RVA: 0x0000A6AF File Offset: 0x000088AF
	private double clientBufferTime
	{
		get
		{
			return (double)this.TickInterval * this.snapshotInterpolationSettings.bufferTimeMultiplier;
		}
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x0002249C File Offset: 0x0002069C
	public override void Awake()
	{
		base.Awake();
		this.driftEma = new ExponentialMovingAverage(this.TickRate * this.snapshotInterpolationSettings.driftEmaDuration);
		this.deliveryTimeEma = new ExponentialMovingAverage(this.TickRate * this.snapshotInterpolationSettings.deliveryTimeEmaDuration);
	}

	// Token: 0x06000574 RID: 1396 RVA: 0x000224EC File Offset: 0x000206EC
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

	// Token: 0x06000575 RID: 1397 RVA: 0x000225C0 File Offset: 0x000207C0
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

	// Token: 0x06000576 RID: 1398 RVA: 0x0000A6C4 File Offset: 0x000088C4
	public void AddSynchronizedObject(SynchronizedObject synchronizedObject)
	{
		this.synchronizedObjects.Add(synchronizedObject);
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x0000A6D2 File Offset: 0x000088D2
	public void RemoveSynchronizedObject(SynchronizedObject synchronizedObject)
	{
		this.synchronizedObjects.Remove(synchronizedObject);
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x0000A6E1 File Offset: 0x000088E1
	private void ClearSynchronizedObjects()
	{
		this.synchronizedObjects.Clear();
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x0000A6EE File Offset: 0x000088EE
	public void Server_AddSynchronizedClientId(ulong clientId)
	{
		this.synchronizedClientIds.Add(clientId);
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x0000A6FC File Offset: 0x000088FC
	public void Server_RemoveSynchronizedClientId(ulong clientId)
	{
		this.synchronizedClientIds.Remove(clientId);
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x0000A70B File Offset: 0x0000890B
	private void ClearSynchronizedClientIds()
	{
		this.synchronizedClientIds.Clear();
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x00022610 File Offset: 0x00020810
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

	// Token: 0x0600057D RID: 1405 RVA: 0x000226A8 File Offset: 0x000208A8
	private ValueTuple<ushort, Vector3, Quaternion> DecodeSynchronizedObjectData(SynchronizedObjectData synchronizedObjectData)
	{
		float x = (float)synchronizedObjectData.Rx / 32767f;
		float y = (float)synchronizedObjectData.Ry / 32767f;
		float z = (float)synchronizedObjectData.Rz / 32767f;
		float w = (float)synchronizedObjectData.Rw / 32767f;
		Quaternion item = new Quaternion(x, y, z, w);
		return new ValueTuple<ushort, Vector3, Quaternion>(synchronizedObjectData.NetworkObjectId, new Vector3((float)synchronizedObjectData.X / 655f, (float)synchronizedObjectData.Y / 655f, (float)synchronizedObjectData.Z / 655f), item);
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x00022734 File Offset: 0x00020934
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

	// Token: 0x0600057F RID: 1407 RVA: 0x000227B0 File Offset: 0x000209B0
	public void Server_ForceSynchronizeClientId(ulong clientId)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (clientId == 0UL)
		{
			return;
		}
		SynchronizedObjectData[] synchronizedObjectsData = this.Server_GatherSynchronizedObjectData(true);
		this.Server_SynchronizeObjectsRpc(this.serverLastSentTickId, this.serverLastSentServerTime, synchronizedObjectsData, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x000227FC File Offset: 0x000209FC
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

	// Token: 0x06000581 RID: 1409 RVA: 0x00022954 File Offset: 0x00020B54
	[Rpc(SendTo.SpecifiedInParams, Delivery = RpcDelivery.Unreliable)]
	private void Server_SynchronizeObjectsRpc(ushort tickId, double serverTime, SynchronizedObjectData[] synchronizedObjectsData, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 1738927239U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				Delivery = RpcDelivery.Unreliable
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
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSynchronizeObjects", new Dictionary<string, object>
		{
			{
				"serverDeltaTime",
				num
			}
		});
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x00022B2C File Offset: 0x00020D2C
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

	// Token: 0x06000583 RID: 1411 RVA: 0x00022D1C File Offset: 0x00020F1C
	private void OnValidate()
	{
		this.snapshotInterpolationSettings.catchupNegativeThreshold = Mathf.Min(this.snapshotInterpolationSettings.catchupNegativeThreshold, 0f);
		this.snapshotInterpolationSettings.catchupPositiveThreshold = Mathf.Max(this.snapshotInterpolationSettings.catchupPositiveThreshold, 0f);
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x00022DCC File Offset: 0x00020FCC
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x0000A718 File Offset: 0x00008918
	protected override void __initializeRpcs()
	{
		base.__registerRpc(1738927239U, new NetworkBehaviour.RpcReceiveHandler(SynchronizedObjectManager.__rpc_handler_1738927239), "Server_SynchronizeObjectsRpc");
		base.__initializeRpcs();
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x00022DE4 File Offset: 0x00020FE4
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

	// Token: 0x06000588 RID: 1416 RVA: 0x0000A73E File Offset: 0x0000893E
	protected internal override string __getTypeName()
	{
		return "SynchronizedObjectManager";
	}

	// Token: 0x040002F4 RID: 756
	[Header("Settings")]
	[SerializeField]
	private SnapshotInterpolationSettings snapshotInterpolationSettings;

	// Token: 0x040002F5 RID: 757
	[SerializeField]
	private bool skipLateTicks = true;

	// Token: 0x040002F6 RID: 758
	private int tickRate = 100;

	// Token: 0x040002F7 RID: 759
	private bool useNetworkSmoothing;

	// Token: 0x040002F8 RID: 760
	[HideInInspector]
	public float NetworkSmoothingStrength = 1f;

	// Token: 0x040002F9 RID: 761
	private float serverTickAccumulator;

	// Token: 0x040002FA RID: 762
	private ushort serverLastSentTickId;

	// Token: 0x040002FB RID: 763
	private double serverLastSentServerTime;

	// Token: 0x040002FC RID: 764
	private ushort clientLastReceivedTickId;

	// Token: 0x040002FD RID: 765
	private double clientLastReceivedServerTime;

	// Token: 0x040002FE RID: 766
	private bool clientHasReceivedFirstTick;

	// Token: 0x040002FF RID: 767
	private float clientAccumulatedDeltaTime;

	// Token: 0x04000300 RID: 768
	private double clientLocalTimeline;

	// Token: 0x04000301 RID: 769
	private double clientLocalTimescale = 1.0;

	// Token: 0x04000302 RID: 770
	private List<SynchronizedObject> synchronizedObjects = new List<SynchronizedObject>();

	// Token: 0x04000303 RID: 771
	private List<ulong> synchronizedClientIds = new List<ulong>();

	// Token: 0x04000304 RID: 772
	private SortedList<double, SynchronizedObjectsSnapshot> snapshots = new SortedList<double, SynchronizedObjectsSnapshot>();

	// Token: 0x04000305 RID: 773
	private ExponentialMovingAverage driftEma;

	// Token: 0x04000306 RID: 774
	private ExponentialMovingAverage deliveryTimeEma;
}
