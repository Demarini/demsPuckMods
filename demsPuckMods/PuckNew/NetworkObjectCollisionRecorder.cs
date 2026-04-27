using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200001B RID: 27
public class NetworkObjectCollisionRecorder : NetworkBehaviour
{
	// Token: 0x17000006 RID: 6
	// (get) Token: 0x06000094 RID: 148 RVA: 0x00003B7C File Offset: 0x00001D7C
	[HideInInspector]
	public List<NetworkObjectCollision> NetworkObjectCollisions
	{
		get
		{
			return this.Buffer.AsNativeArray().ToList<NetworkObjectCollision>();
		}
	}

	// Token: 0x06000095 RID: 149 RVA: 0x00003B93 File Offset: 0x00001D93
	protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
	{
		this.InitializeNetworkVariables(null);
		base.OnNetworkPreSpawn(ref networkManager);
	}

	// Token: 0x06000096 RID: 150 RVA: 0x00003BA3 File Offset: 0x00001DA3
	public override void OnNetworkSpawn()
	{
		this.Buffer.OnListChanged += this.OnBufferChanged;
		base.OnNetworkSpawn();
	}

	// Token: 0x06000097 RID: 151 RVA: 0x00003BC2 File Offset: 0x00001DC2
	protected override void OnNetworkPostSpawn()
	{
		if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsConnectedClient)
		{
			this.ProcessInitialNetworkVariableValues();
		}
		base.OnNetworkPostSpawn();
	}

	// Token: 0x06000098 RID: 152 RVA: 0x00003BE8 File Offset: 0x00001DE8
	protected override void OnNetworkSessionSynchronized()
	{
		this.ProcessInitialNetworkVariableValues();
		base.OnNetworkSessionSynchronized();
	}

	// Token: 0x06000099 RID: 153 RVA: 0x00003BF6 File Offset: 0x00001DF6
	public override void OnNetworkDespawn()
	{
		this.Buffer.OnListChanged -= this.OnBufferChanged;
		base.OnNetworkDespawn();
	}

	// Token: 0x0600009A RID: 154 RVA: 0x00003C15 File Offset: 0x00001E15
	public void InitializeNetworkVariables(List<NetworkObjectCollision> buffer = null)
	{
		if (this.isNetworkVariablesInitialized)
		{
			return;
		}
		this.isNetworkVariablesInitialized = true;
		this.Buffer = new NetworkList<NetworkObjectCollision>(buffer, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
	}

	// Token: 0x0600009B RID: 155 RVA: 0x00003C38 File Offset: 0x00001E38
	private void ProcessInitialNetworkVariableValues()
	{
		this.OnBufferChanged(new NetworkListEvent<NetworkObjectCollision>
		{
			Type = NetworkListEvent<NetworkObjectCollision>.EventType.Full
		});
	}

	// Token: 0x0600009C RID: 156 RVA: 0x000020D3 File Offset: 0x000002D3
	private void OnBufferChanged(NetworkListEvent<NetworkObjectCollision> changeEvent)
	{
	}

	// Token: 0x0600009D RID: 157 RVA: 0x00003C5C File Offset: 0x00001E5C
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
		NetworkObjectCollision? networkObjectCollision = null;
		foreach (NetworkObjectCollision networkObjectCollision2 in this.Buffer)
		{
			NetworkObjectReference networkObjectReference2 = networkObjectCollision2.NetworkObjectReference;
			if (networkObjectReference2.Equals(networkObjectReference))
			{
				networkObjectCollision = new NetworkObjectCollision?(networkObjectCollision2);
				break;
			}
		}
		if (networkObjectCollision != null && this.Buffer.Contains(networkObjectCollision.Value))
		{
			this.Buffer.Remove(networkObjectCollision.Value);
		}
		if (this.Buffer.Count >= this.bufferSize)
		{
			this.Buffer.RemoveAt(0);
		}
		this.Buffer.Add(new NetworkObjectCollision
		{
			NetworkObjectReference = networkObjectReference,
			Time = Time.time
		});
	}

	// Token: 0x0600009F RID: 159 RVA: 0x00003D9C File Offset: 0x00001F9C
	protected override void __initializeVariables()
	{
		bool flag = this.Buffer == null;
		if (flag)
		{
			throw new Exception("NetworkObjectCollisionRecorder.Buffer cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.Buffer.Initialize(this);
		base.__nameNetworkVariable(this.Buffer, "Buffer");
		this.NetworkVariableFields.Add(this.Buffer);
		base.__initializeVariables();
	}

	// Token: 0x060000A0 RID: 160 RVA: 0x000021BE File Offset: 0x000003BE
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x00003DFF File Offset: 0x00001FFF
	protected internal override string __getTypeName()
	{
		return "NetworkObjectCollisionRecorder";
	}

	// Token: 0x04000043 RID: 67
	[Header("Settings")]
	[SerializeField]
	private int bufferSize = 10;

	// Token: 0x04000044 RID: 68
	[SerializeField]
	private LayerMask collisionLayers;

	// Token: 0x04000045 RID: 69
	[HideInInspector]
	public NetworkList<NetworkObjectCollision> Buffer;

	// Token: 0x04000046 RID: 70
	private bool isNetworkVariablesInitialized;
}
