using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000008 RID: 8
public class Goal : MonoBehaviour
{
	// Token: 0x17000003 RID: 3
	// (get) Token: 0x0600002A RID: 42 RVA: 0x000026ED File Offset: 0x000008ED
	public Cloth NetCloth
	{
		get
		{
			return this.netCloth;
		}
	}

	// Token: 0x0600002B RID: 43 RVA: 0x000026F5 File Offset: 0x000008F5
	public void Server_OnPuckEnterGoal(Puck puck)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		EventManager.TriggerEvent("Event_Server_OnPuckEnterGoal", new Dictionary<string, object>
		{
			{
				"puck",
				puck
			},
			{
				"team",
				this.Team
			}
		});
	}

	// Token: 0x0600002C RID: 44 RVA: 0x00002738 File Offset: 0x00000938
	public void Client_AddNetClothSphereCollider(SphereCollider sphereCollider)
	{
		if (!NetworkManager.Singleton.IsClient)
		{
			return;
		}
		if (!sphereCollider)
		{
			return;
		}
		List<ClothSphereColliderPair> list = this.netCloth.sphereColliders.ToList<ClothSphereColliderPair>();
		list.Add(new ClothSphereColliderPair(sphereCollider));
		this.netCloth.sphereColliders = list.ToArray();
	}

	// Token: 0x0600002D RID: 45 RVA: 0x0000278C File Offset: 0x0000098C
	public void Client_RemoveNetClothSphereCollider(SphereCollider sphereCollider)
	{
		if (!NetworkManager.Singleton.IsClient)
		{
			return;
		}
		if (!sphereCollider)
		{
			return;
		}
		List<ClothSphereColliderPair> list = this.netCloth.sphereColliders.ToList<ClothSphereColliderPair>();
		list.RemoveAll((ClothSphereColliderPair pair) => pair.first == sphereCollider);
		this.netCloth.sphereColliders = list.ToArray();
	}

	// Token: 0x04000017 RID: 23
	[Header("Settings")]
	[SerializeField]
	private PlayerTeam Team;

	// Token: 0x04000018 RID: 24
	[Header("References")]
	[SerializeField]
	private Cloth netCloth;
}
