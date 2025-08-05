using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200000C RID: 12
public class Goal : MonoBehaviour
{
	// Token: 0x17000007 RID: 7
	// (get) Token: 0x0600006F RID: 111 RVA: 0x0000706E File Offset: 0x0000526E
	public Cloth NetCloth
	{
		get
		{
			return this.netCloth;
		}
	}

	// Token: 0x06000070 RID: 112 RVA: 0x00010870 File Offset: 0x0000EA70
	public void Server_OnPuckEnterGoal(Puck puck)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnPuckEnterTeamGoal", new Dictionary<string, object>
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

	// Token: 0x06000071 RID: 113 RVA: 0x000108C0 File Offset: 0x0000EAC0
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

	// Token: 0x06000072 RID: 114 RVA: 0x00010914 File Offset: 0x0000EB14
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

	// Token: 0x04000033 RID: 51
	[Header("References")]
	[SerializeField]
	private Cloth netCloth;

	// Token: 0x04000034 RID: 52
	[Header("Settings")]
	[SerializeField]
	private PlayerTeam Team;
}
