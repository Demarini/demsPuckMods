using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200000D RID: 13
public class Level : NetworkBehaviour
{
	// Token: 0x0600003A RID: 58 RVA: 0x00002A38 File Offset: 0x00000C38
	private void Awake()
	{
		MeshRenderer component = this.boundsGameObject.GetComponent<MeshRenderer>();
		if (component == null)
		{
			Debug.LogWarning("[Level] boundsGameObject does not have a MeshRenderer component");
			return;
		}
		this.Bounds = component.bounds;
	}

	// Token: 0x0600003B RID: 59 RVA: 0x00002A71 File Offset: 0x00000C71
	protected override void OnNetworkPostSpawn()
	{
		EventManager.TriggerEvent("Event_Everyone_OnLevelSpawned", new Dictionary<string, object>
		{
			{
				"level",
				this
			}
		});
		base.OnNetworkPostSpawn();
	}

	// Token: 0x0600003C RID: 60 RVA: 0x00002A94 File Offset: 0x00000C94
	public override void OnNetworkDespawn()
	{
		EventManager.TriggerEvent("Event_Everyone_OnLevelDespawned", new Dictionary<string, object>
		{
			{
				"level",
				this
			}
		});
		base.OnNetworkDespawn();
	}

	// Token: 0x0600003D RID: 61 RVA: 0x00002AB7 File Offset: 0x00000CB7
	public void SetBlueGoalLightEnabled(bool isEnabled)
	{
		this.blueGoalLight.enabled = isEnabled;
	}

	// Token: 0x0600003E RID: 62 RVA: 0x00002AC5 File Offset: 0x00000CC5
	public void SetRedGoalLightEnabled(bool isEnabled)
	{
		this.redGoalLight.enabled = isEnabled;
	}

	// Token: 0x0600003F RID: 63 RVA: 0x00002AD4 File Offset: 0x00000CD4
	public void Server_PlayBlueGoalSound()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.blueGoalSound.Server_Play(-1f, -1f, false, -1, 0f, false, false, false, 0f, false, 0f, -1f);
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00002B20 File Offset: 0x00000D20
	public void Server_PlayRedGoalSound()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.redGoalSound.Server_Play(-1f, -1f, false, -1, 0f, false, false, false, 0f, false, 0f, -1f);
	}

	// Token: 0x06000041 RID: 65 RVA: 0x00002B6C File Offset: 0x00000D6C
	public void Server_PlayerCheerSound(float duration)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.cheerSounds.ForEach(delegate(SynchronizedAudio cheerSound)
		{
			cheerSound.Server_Play(-1f, -1f, false, -1, 0f, false, false, true, 3f, true, 3f, duration);
		});
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00002BAC File Offset: 0x00000DAC
	public void Server_PlayHornSound()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.hornSound.Server_Play(-1f, -1f, false, -1, 0f, false, false, false, 0f, false, 0f, -1f);
	}

	// Token: 0x06000044 RID: 68 RVA: 0x00002C08 File Offset: 0x00000E08
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000045 RID: 69 RVA: 0x000021BE File Offset: 0x000003BE
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000046 RID: 70 RVA: 0x00002C1E File Offset: 0x00000E1E
	protected internal override string __getTypeName()
	{
		return "Level";
	}

	// Token: 0x0400001F RID: 31
	[Header("References")]
	[SerializeField]
	private GameObject boundsGameObject;

	// Token: 0x04000020 RID: 32
	[SerializeField]
	private Light blueGoalLight;

	// Token: 0x04000021 RID: 33
	[SerializeField]
	private Light redGoalLight;

	// Token: 0x04000022 RID: 34
	[SerializeField]
	private SynchronizedAudio blueGoalSound;

	// Token: 0x04000023 RID: 35
	[SerializeField]
	private SynchronizedAudio redGoalSound;

	// Token: 0x04000024 RID: 36
	[SerializeField]
	private List<SynchronizedAudio> cheerSounds = new List<SynchronizedAudio>();

	// Token: 0x04000025 RID: 37
	[SerializeField]
	private SynchronizedAudio hornSound;

	// Token: 0x04000026 RID: 38
	[HideInInspector]
	public Bounds Bounds;
}
