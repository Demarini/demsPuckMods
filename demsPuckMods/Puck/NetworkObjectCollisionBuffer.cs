using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000016 RID: 22
public class NetworkObjectCollisionBuffer : NetworkBehaviour
{
	// Token: 0x0600009C RID: 156 RVA: 0x0000722C File Offset: 0x0000542C
	private void Awake()
	{
		this.buffer = new NetworkList<NetworkObjectCollision>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
	}

	// Token: 0x0600009D RID: 157 RVA: 0x0000723C File Offset: 0x0000543C
	public override void OnNetworkSpawn()
	{
		this.buffer.Initialize(this);
		this.buffer.OnListChanged += this.OnBufferChanged;
		this.Client_InitializeNetworkVariables();
		base.OnNetworkSpawn();
	}

	// Token: 0x0600009E RID: 158 RVA: 0x0000726D File Offset: 0x0000546D
	protected override void OnNetworkSessionSynchronized()
	{
		this.Client_InitializeNetworkVariables();
		base.OnNetworkSessionSynchronized();
	}

	// Token: 0x0600009F RID: 159 RVA: 0x0000727B File Offset: 0x0000547B
	public override void OnNetworkDespawn()
	{
		this.buffer.OnListChanged -= this.OnBufferChanged;
		this.buffer.Dispose();
		base.OnNetworkDespawn();
	}

	// Token: 0x060000A0 RID: 160 RVA: 0x00010D4C File Offset: 0x0000EF4C
	private void OnBufferChanged(NetworkListEvent<NetworkObjectCollision> changeEvent)
	{
		this.Buffer.Clear();
		foreach (NetworkObjectCollision item in this.buffer)
		{
			this.Buffer.Add(item);
		}
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x00010DAC File Offset: 0x0000EFAC
	private void OnCollisionEnter(Collision collision)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if ((this.collisionLayers.value & 1 << collision.gameObject.layer) == 0)
		{
			return;
		}
		NetworkObject component = collision.gameObject.GetComponent<NetworkObject>();
		if (!component)
		{
			return;
		}
		NetworkObjectReference networkObjectReference = new NetworkObjectReference(component);
		NetworkObjectCollision item = default(NetworkObjectCollision);
		foreach (NetworkObjectCollision networkObjectCollision in this.buffer)
		{
			NetworkObjectReference networkObjectReference2 = networkObjectCollision.NetworkObjectReference;
			if (networkObjectReference2.Equals(networkObjectReference))
			{
				item = networkObjectCollision;
				break;
			}
		}
		if (this.buffer.Contains(item))
		{
			this.buffer.Remove(item);
		}
		if (this.buffer.Count >= this.bufferSize)
		{
			this.buffer.RemoveAt(0);
		}
		this.buffer.Add(new NetworkObjectCollision
		{
			NetworkObjectReference = networkObjectReference,
			Time = Time.time
		});
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x000072A5 File Offset: 0x000054A5
	public void Clear()
	{
		this.buffer.Clear();
		this.Buffer.Clear();
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x00010EC0 File Offset: 0x0000F0C0
	public void Client_InitializeNetworkVariables()
	{
		this.OnBufferChanged(new NetworkListEvent<NetworkObjectCollision>
		{
			Type = NetworkListEvent<NetworkObjectCollision>.EventType.Value
		});
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x00010EE4 File Offset: 0x0000F0E4
	protected override void __initializeVariables()
	{
		bool flag = this.buffer == null;
		if (flag)
		{
			throw new Exception("NetworkObjectCollisionBuffer.buffer cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.buffer.Initialize(this);
		base.__nameNetworkVariable(this.buffer, "buffer");
		this.NetworkVariableFields.Add(this.buffer);
		base.__initializeVariables();
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x000072D8 File Offset: 0x000054D8
	protected internal override string __getTypeName()
	{
		return "NetworkObjectCollisionBuffer";
	}

	// Token: 0x04000044 RID: 68
	[Header("Settings")]
	[SerializeField]
	private int bufferSize = 10;

	// Token: 0x04000045 RID: 69
	[SerializeField]
	private LayerMask collisionLayers;

	// Token: 0x04000046 RID: 70
	private NetworkList<NetworkObjectCollision> buffer;

	// Token: 0x04000047 RID: 71
	[HideInInspector]
	public readonly List<NetworkObjectCollision> Buffer = new List<NetworkObjectCollision>();
}
