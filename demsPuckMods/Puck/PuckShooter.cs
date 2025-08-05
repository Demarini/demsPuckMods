using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200001D RID: 29
public class PuckShooter : NetworkBehaviour
{
	// Token: 0x060000CB RID: 203 RVA: 0x0000755D File Offset: 0x0000575D
	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		if (NetworkManager.Singleton.IsServer && this.shootOnStart)
		{
			this.Server_StartShootingCoroutine();
		}
	}

	// Token: 0x060000CC RID: 204 RVA: 0x0000757F File Offset: 0x0000577F
	public override void OnNetworkDespawn()
	{
		base.OnNetworkDespawn();
		if (NetworkManager.Singleton.IsServer)
		{
			this.Server_StopShootingCoroutine();
		}
	}

	// Token: 0x060000CD RID: 205 RVA: 0x00011248 File Offset: 0x0000F448
	public void Server_Shoot()
	{
		if (!base.IsServer)
		{
			return;
		}
		Puck puck = NetworkBehaviourSingleton<PuckManager>.Instance.Server_SpawnPuck(base.transform.position, Quaternion.identity, base.transform.forward * this.force, false);
		this.shotPucks.Add(puck);
		base.StartCoroutine(this.IDestroyAfterTime(puck, this.destroyTime));
	}

	// Token: 0x060000CE RID: 206 RVA: 0x00007599 File Offset: 0x00005799
	public void Server_StartShootingCoroutine()
	{
		if (!base.IsServer)
		{
			return;
		}
		this.Server_StopShootingCoroutine();
		this.shootIntervalCoroutine = this.IShootInterval();
		base.StartCoroutine(this.shootIntervalCoroutine);
	}

	// Token: 0x060000CF RID: 207 RVA: 0x000075C3 File Offset: 0x000057C3
	public void Server_StopShootingCoroutine()
	{
		if (!base.IsServer)
		{
			return;
		}
		if (this.shootIntervalCoroutine == null)
		{
			return;
		}
		base.StopCoroutine(this.shootIntervalCoroutine);
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x000075E3 File Offset: 0x000057E3
	private IEnumerator IShootInterval()
	{
		yield return new WaitForSeconds(this.interval);
		this.Server_Shoot();
		this.Server_StartShootingCoroutine();
		yield break;
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x000075F2 File Offset: 0x000057F2
	private IEnumerator IDestroyAfterTime(Puck puck, float time)
	{
		yield return new WaitForSeconds(time);
		if (puck2 && this.shotPucks.Contains(puck2))
		{
			this.shotPucks.Remove(puck2);
			puck2.NetworkObject.Despawn(true);
		}
		else
		{
			this.shotPucks.RemoveAll((Puck puck) => puck == null);
		}
		yield break;
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x00007643 File Offset: 0x00005843
	protected internal override string __getTypeName()
	{
		return "PuckShooter";
	}

	// Token: 0x04000058 RID: 88
	[Header("Settings")]
	[SerializeField]
	private float force = 1000f;

	// Token: 0x04000059 RID: 89
	[SerializeField]
	private float interval = 1f;

	// Token: 0x0400005A RID: 90
	[SerializeField]
	private bool shootOnStart;

	// Token: 0x0400005B RID: 91
	[SerializeField]
	private float destroyTime = 2f;

	// Token: 0x0400005C RID: 92
	private List<Puck> shotPucks = new List<Puck>();

	// Token: 0x0400005D RID: 93
	private IEnumerator shootIntervalCoroutine;
}
