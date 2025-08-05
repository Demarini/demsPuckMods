using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000048 RID: 72
public class LevelManager : NetworkBehaviourSingleton<LevelManager>
{
	// Token: 0x1700002E RID: 46
	// (get) Token: 0x060001FA RID: 506 RVA: 0x00008269 File Offset: 0x00006469
	public PuckShooter PuckShooter
	{
		get
		{
			return this.puckShooter;
		}
	}

	// Token: 0x1700002F RID: 47
	// (get) Token: 0x060001FB RID: 507 RVA: 0x00008271 File Offset: 0x00006471
	public Bounds IceBounds
	{
		get
		{
			return this.iceMeshRenderer.bounds;
		}
	}

	// Token: 0x060001FC RID: 508 RVA: 0x0000827E File Offset: 0x0000647E
	public override void Awake()
	{
		base.Awake();
		base.DestroyOnLoad();
		this.initialBlueGoalLightIntensity = this.blueGoalLight.intensity;
		this.initialRedGoalLightIntensity = this.redGoalLight.intensity;
	}

	// Token: 0x060001FD RID: 509 RVA: 0x00015AF4 File Offset: 0x00013CF4
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnLevelStarted", new Dictionary<string, object>
		{
			{
				"puckPositions",
				this.puckPositions
			},
			{
				"playerBluePositions",
				this.playerBluePositions
			},
			{
				"playerRedPositions",
				this.playerRedPositions
			},
			{
				"spectatorPositions",
				this.spectatorPositions
			}
		});
	}

	// Token: 0x060001FE RID: 510 RVA: 0x000082AE File Offset: 0x000064AE
	protected override void OnNetworkPostSpawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnLevelSpawned", null);
		base.OnNetworkPostSpawn();
	}

	// Token: 0x060001FF RID: 511 RVA: 0x000082C6 File Offset: 0x000064C6
	public override void OnNetworkDespawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnLevelDespawned", null);
		base.OnNetworkDespawn();
	}

	// Token: 0x06000200 RID: 512 RVA: 0x000082DE File Offset: 0x000064DE
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnLevelDestroyed", null);
		base.OnDestroy();
	}

	// Token: 0x06000201 RID: 513 RVA: 0x000082F6 File Offset: 0x000064F6
	public void Client_DisableAllCameras()
	{
		this.observerCamera.Disable();
		this.bluePositionSelectionCamera.Disable();
		this.redPositionSelectionCamera.Disable();
		this.replayCamera.Disable();
	}

	// Token: 0x06000202 RID: 514 RVA: 0x00008324 File Offset: 0x00006524
	public void Client_EnableObserverCamera()
	{
		this.Client_DisableAllCameras();
		this.observerCamera.Enable();
	}

	// Token: 0x06000203 RID: 515 RVA: 0x00008337 File Offset: 0x00006537
	public void Client_EnableBluePositionSelectionCamera()
	{
		this.Client_DisableAllCameras();
		this.bluePositionSelectionCamera.Enable();
	}

	// Token: 0x06000204 RID: 516 RVA: 0x0000834A File Offset: 0x0000654A
	public void Client_EnableRedPositionSelectionCamera()
	{
		this.Client_DisableAllCameras();
		this.redPositionSelectionCamera.Enable();
	}

	// Token: 0x06000205 RID: 517 RVA: 0x0000835D File Offset: 0x0000655D
	public void Client_EnableReplayCamera()
	{
		this.Client_DisableAllCameras();
		this.replayCamera.Enable();
	}

	// Token: 0x06000206 RID: 518 RVA: 0x00015B5C File Offset: 0x00013D5C
	public void Server_PlayPeriodHornSound()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.periodHornAudioSource.Server_Play(-1f, -1f, false, -1, 0f, false, false, false, 0f, false, 0f, -1f);
	}

	// Token: 0x06000207 RID: 519 RVA: 0x00015BA8 File Offset: 0x00013DA8
	public void Server_PlayTeamBlueGoalSound()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.blueGoalAudioSource.Server_Play(-1f, -1f, false, -1, 0f, false, false, false, 0f, false, 0f, -1f);
	}

	// Token: 0x06000208 RID: 520 RVA: 0x00015BF4 File Offset: 0x00013DF4
	public void Server_PlayTeamRedGoalSound()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.redGoalAudioSource.Server_Play(-1f, -1f, false, -1, 0f, false, false, false, 0f, false, 0f, -1f);
	}

	// Token: 0x06000209 RID: 521 RVA: 0x00015C40 File Offset: 0x00013E40
	public void Server_PlayCheerSound()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		foreach (SynchronizedAudio synchronizedAudio in this.crowdCheerAudioSources)
		{
			synchronizedAudio.Server_Play(-1f, -1f, false, -1, 0f, true, true, true, 0.5f, true, 5f, 15f);
		}
	}

	// Token: 0x0600020A RID: 522 RVA: 0x00008370 File Offset: 0x00006570
	public void Client_ActivateBlueGoalLight()
	{
		this.Client_DeactivateGoalLights();
		this.blueGoalLight.intensity = this.initialBlueGoalLightIntensity;
	}

	// Token: 0x0600020B RID: 523 RVA: 0x00008389 File Offset: 0x00006589
	public void Client_ActivateRedGoalLight()
	{
		this.Client_DeactivateGoalLights();
		this.redGoalLight.intensity = this.initialRedGoalLightIntensity;
	}

	// Token: 0x0600020C RID: 524 RVA: 0x000083A2 File Offset: 0x000065A2
	public void Client_DeactivateGoalLights()
	{
		this.blueGoalLight.intensity = 0f;
		this.redGoalLight.intensity = 0f;
	}

	// Token: 0x0600020E RID: 526 RVA: 0x00015CC4 File Offset: 0x00013EC4
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x0600020F RID: 527 RVA: 0x000083E2 File Offset: 0x000065E2
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000210 RID: 528 RVA: 0x000083EC File Offset: 0x000065EC
	protected internal override string __getTypeName()
	{
		return "LevelManager";
	}

	// Token: 0x0400012F RID: 303
	[Header("References")]
	[Space(20f)]
	[SerializeField]
	private BaseCamera observerCamera;

	// Token: 0x04000130 RID: 304
	[SerializeField]
	private BaseCamera bluePositionSelectionCamera;

	// Token: 0x04000131 RID: 305
	[SerializeField]
	private BaseCamera redPositionSelectionCamera;

	// Token: 0x04000132 RID: 306
	[SerializeField]
	private BaseCamera replayCamera;

	// Token: 0x04000133 RID: 307
	[Space(20f)]
	[SerializeField]
	private MeshRenderer iceMeshRenderer;

	// Token: 0x04000134 RID: 308
	[Space(20f)]
	[SerializeField]
	private List<SynchronizedAudio> crowdCheerAudioSources;

	// Token: 0x04000135 RID: 309
	[SerializeField]
	private SynchronizedAudio periodHornAudioSource;

	// Token: 0x04000136 RID: 310
	[SerializeField]
	private SynchronizedAudio blueGoalAudioSource;

	// Token: 0x04000137 RID: 311
	[SerializeField]
	private SynchronizedAudio redGoalAudioSource;

	// Token: 0x04000138 RID: 312
	[Space(20f)]
	[SerializeField]
	private Light blueGoalLight;

	// Token: 0x04000139 RID: 313
	[SerializeField]
	private Light redGoalLight;

	// Token: 0x0400013A RID: 314
	[Space(20f)]
	[SerializeField]
	private List<PuckPosition> puckPositions = new List<PuckPosition>();

	// Token: 0x0400013B RID: 315
	[Space(20f)]
	[SerializeField]
	private List<PlayerPosition> playerBluePositions;

	// Token: 0x0400013C RID: 316
	[SerializeField]
	private List<PlayerPosition> playerRedPositions;

	// Token: 0x0400013D RID: 317
	[SerializeField]
	private List<Transform> spectatorPositions = new List<Transform>();

	// Token: 0x0400013E RID: 318
	[SerializeField]
	private PuckShooter puckShooter;

	// Token: 0x0400013F RID: 319
	private float initialBlueGoalLightIntensity;

	// Token: 0x04000140 RID: 320
	private float initialRedGoalLightIntensity;
}
