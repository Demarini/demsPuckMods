using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000A9 RID: 169
public class SpectatorManagerController : NetworkBehaviour
{
	// Token: 0x060004E4 RID: 1252 RVA: 0x0000A090 File Offset: 0x00008290
	private void Awake()
	{
		this.spectatorManager = base.GetComponent<SpectatorManager>();
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x0000A09E File Offset: 0x0000829E
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnLevelStarted", new Action<Dictionary<string, object>>(this.Event_OnLevelStarted));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnLevelDestroyed", new Action<Dictionary<string, object>>(this.Event_OnLevelDestroyed));
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x0000A0D6 File Offset: 0x000082D6
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnLevelStarted", new Action<Dictionary<string, object>>(this.Event_OnLevelStarted));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnLevelDestroyed", new Action<Dictionary<string, object>>(this.Event_OnLevelDestroyed));
		base.OnDestroy();
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x0002077C File Offset: 0x0001E97C
	private void Event_OnLevelStarted(Dictionary<string, object> message)
	{
		List<Transform> spectatorPositions = (List<Transform>)message["spectatorPositions"];
		this.spectatorManager.SetSpectatorPositions(spectatorPositions);
		this.spectatorManager.SpawnSpectators();
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x0000A114 File Offset: 0x00008314
	private void Event_OnLevelDestroyed(Dictionary<string, object> message)
	{
		this.spectatorManager.ClearSpectators();
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x0000A121 File Offset: 0x00008321
	protected internal override string __getTypeName()
	{
		return "SpectatorManagerController";
	}

	// Token: 0x040002C8 RID: 712
	private SpectatorManager spectatorManager;
}
