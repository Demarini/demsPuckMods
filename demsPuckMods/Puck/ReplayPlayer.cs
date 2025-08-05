using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200008B RID: 139
public class ReplayPlayer : NetworkBehaviour
{
	// Token: 0x1700004E RID: 78
	// (get) Token: 0x0600036E RID: 878 RVA: 0x0000932F File Offset: 0x0000752F
	[HideInInspector]
	public float TickInterval
	{
		get
		{
			return 1f / (float)this.TickRate;
		}
	}

	// Token: 0x0600036F RID: 879 RVA: 0x00019024 File Offset: 0x00017224
	private void Update()
	{
		if (!base.IsSpawned)
		{
			return;
		}
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!this.IsReplaying)
		{
			return;
		}
		this.tickAccumulator += Time.deltaTime * (float)this.TickRate;
		if (this.tickAccumulator >= 1f)
		{
			while (this.tickAccumulator >= 1f)
			{
				this.tickAccumulator -= 1f;
			}
			this.Server_Tick(this.Tick, false);
			this.Tick++;
		}
	}

	// Token: 0x06000370 RID: 880 RVA: 0x000190B4 File Offset: 0x000172B4
	public void Server_StartReplay(SortedList<int, List<ValueTuple<string, object>>> tickEventMap, int tickRate, int fromTick = 0)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.IsReplaying)
		{
			return;
		}
		if (tickEventMap.Keys.Count == 0)
		{
			return;
		}
		int num = tickEventMap.Keys.Min();
		int max = tickEventMap.Keys.Max();
		fromTick = Mathf.Clamp(fromTick, num, max);
		this.EventMap = tickEventMap;
		this.TickRate = tickRate;
		int i;
		for (i = num; i < fromTick; i++)
		{
			this.Server_Tick(i, true);
			this.EventMap.Remove(i);
		}
		this.Tick = i;
		List<ValueTuple<string, object>> list = this.Server_GetSkippedEvents();
		if (list.Count > 0)
		{
			this.Tick--;
			this.EventMap.Add(this.Tick, list);
		}
		this.IsReplaying = true;
	}

	// Token: 0x06000371 RID: 881 RVA: 0x0000933E File Offset: 0x0000753E
	public void Server_StopReplay()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!this.IsReplaying)
		{
			return;
		}
		Debug.Log("[REPLAY PLAYER] Replay playback stopped");
		this.IsReplaying = false;
		this.Server_Dispose();
	}

	// Token: 0x06000372 RID: 882 RVA: 0x00019174 File Offset: 0x00017374
	private void Server_Tick(int tick, bool skip = false)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (tick > this.EventMap.Keys.Max())
		{
			this.Server_StopReplay();
			return;
		}
		if (!this.EventMap.ContainsKey(tick))
		{
			return;
		}
		foreach (ValueTuple<string, object> valueTuple in this.EventMap[tick])
		{
			string item = valueTuple.Item1;
			object item2 = valueTuple.Item2;
			if (skip)
			{
				this.Server_SkipEvent(item, item2);
			}
			else
			{
				this.Server_ReplayEvent(item, item2);
			}
		}
	}

	// Token: 0x06000373 RID: 883 RVA: 0x00019220 File Offset: 0x00017420
	private List<ValueTuple<string, object>> Server_GetSkippedEvents()
	{
		List<ValueTuple<string, object>> list = new List<ValueTuple<string, object>>();
		foreach (ReplayPlayerSpawned replayPlayerSpawned in this.replayPlayerSpawnedList)
		{
			list.Add(new ValueTuple<string, object>("PlayerSpawned", replayPlayerSpawned));
		}
		foreach (ReplayPlayerBodySpawned replayPlayerBodySpawned in this.replayPlayerBodySpawnedList)
		{
			list.Add(new ValueTuple<string, object>("PlayerBodySpawned", replayPlayerBodySpawned));
		}
		foreach (ReplayStickSpawned replayStickSpawned in this.replayStickSpawnedList)
		{
			list.Add(new ValueTuple<string, object>("StickSpawned", replayStickSpawned));
		}
		foreach (ReplayPuckSpawned replayPuckSpawned in this.replayPuckSpawnedList)
		{
			list.Add(new ValueTuple<string, object>("PuckSpawned", replayPuckSpawned));
		}
		return list;
	}

	// Token: 0x06000374 RID: 884 RVA: 0x00019384 File Offset: 0x00017584
	private void Server_SkipEvent(string eventName, object eventData)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(eventName);
		if (num <= 1238133331U)
		{
			if (num <= 541464130U)
			{
				if (num != 151785130U)
				{
					if (num != 541464130U)
					{
						return;
					}
					if (!(eventName == "PlayerBodySpawned"))
					{
						return;
					}
					ReplayPlayerBodySpawned item = (ReplayPlayerBodySpawned)eventData;
					this.replayPlayerBodySpawnedList.Add(item);
					return;
				}
				else
				{
					if (!(eventName == "PuckSpawned"))
					{
						return;
					}
					ReplayPuckSpawned item2 = (ReplayPuckSpawned)eventData;
					this.replayPuckSpawnedList.Add(item2);
					return;
				}
			}
			else if (num != 921486523U)
			{
				if (num != 1238133331U)
				{
					return;
				}
				if (!(eventName == "StickSpawned"))
				{
					return;
				}
				ReplayStickSpawned item3 = (ReplayStickSpawned)eventData;
				this.replayStickSpawnedList.Add(item3);
				return;
			}
			else
			{
				if (!(eventName == "PuckDespawned"))
				{
					return;
				}
				ReplayPuckDespawned puckDespawned = (ReplayPuckDespawned)eventData;
				this.replayPuckSpawnedList.RemoveAll((ReplayPuckSpawned x) => x.NetworkObjectId == puckDespawned.NetworkObjectId);
				return;
			}
		}
		else if (num <= 1946187694U)
		{
			if (num != 1769383370U)
			{
				if (num != 1946187694U)
				{
					return;
				}
				if (!(eventName == "PlayerSpawned"))
				{
					return;
				}
				ReplayPlayerSpawned item4 = (ReplayPlayerSpawned)eventData;
				this.replayPlayerSpawnedList.Add(item4);
				return;
			}
			else
			{
				if (!(eventName == "StickDespawned"))
				{
					return;
				}
				ReplayStickDespawned stickDespawned = (ReplayStickDespawned)eventData;
				this.replayStickSpawnedList.RemoveAll((ReplayStickSpawned x) => x.OwnerClientId == stickDespawned.OwnerClientId);
				return;
			}
		}
		else if (num != 3342882255U)
		{
			if (num != 3490707091U)
			{
				return;
			}
			if (!(eventName == "PlayerBodyDespawned"))
			{
				return;
			}
			ReplayPlayerBodyDespawned playerBodyDespawned = (ReplayPlayerBodyDespawned)eventData;
			this.replayPlayerBodySpawnedList.RemoveAll((ReplayPlayerBodySpawned x) => x.OwnerClientId == playerBodyDespawned.OwnerClientId);
			return;
		}
		else
		{
			if (!(eventName == "PlayerDespawned"))
			{
				return;
			}
			ReplayPlayerDespawned playerDespawned = (ReplayPlayerDespawned)eventData;
			this.replayPlayerSpawnedList.RemoveAll((ReplayPlayerSpawned x) => x.OwnerClientId == playerDespawned.OwnerClientId);
			return;
		}
	}

	// Token: 0x06000375 RID: 885 RVA: 0x00019594 File Offset: 0x00017794
	private void Server_ReplayEvent(string eventName, object eventData)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(eventName);
		if (num <= 1238133331U)
		{
			if (num <= 541464130U)
			{
				if (num != 151785130U)
				{
					if (num != 468072804U)
					{
						if (num != 541464130U)
						{
							return;
						}
						if (!(eventName == "PlayerBodySpawned"))
						{
							return;
						}
						ReplayPlayerBodySpawned replayPlayerBodySpawned = (ReplayPlayerBodySpawned)eventData;
						Player replayPlayerByClientId = NetworkBehaviourSingleton<PlayerManager>.Instance.GetReplayPlayerByClientId(replayPlayerBodySpawned.OwnerClientId);
						if (!replayPlayerByClientId)
						{
							return;
						}
						replayPlayerByClientId.Username.Value = replayPlayerBodySpawned.Username;
						replayPlayerByClientId.Number.Value = replayPlayerBodySpawned.Number;
						replayPlayerByClientId.Team.Value = replayPlayerBodySpawned.Team;
						replayPlayerByClientId.Role.Value = replayPlayerBodySpawned.Role;
						replayPlayerByClientId.Country.Value = replayPlayerBodySpawned.Country;
						replayPlayerByClientId.VisorAttackerBlueSkin.Value = replayPlayerBodySpawned.VisorAttackerBlueSkin;
						replayPlayerByClientId.VisorAttackerRedSkin.Value = replayPlayerBodySpawned.VisorAttackerRedSkin;
						replayPlayerByClientId.VisorGoalieBlueSkin.Value = replayPlayerBodySpawned.VisorGoalieBlueSkin;
						replayPlayerByClientId.VisorGoalieRedSkin.Value = replayPlayerBodySpawned.VisorGoalieRedSkin;
						replayPlayerByClientId.Mustache.Value = replayPlayerBodySpawned.Mustache;
						replayPlayerByClientId.Beard.Value = replayPlayerBodySpawned.Beard;
						replayPlayerByClientId.JerseyAttackerBlueSkin.Value = replayPlayerBodySpawned.JerseyAttackerBlueSkin;
						replayPlayerByClientId.JerseyAttackerRedSkin.Value = replayPlayerBodySpawned.JerseyAttackerRedSkin;
						replayPlayerByClientId.JerseyGoalieBlueSkin.Value = replayPlayerBodySpawned.JerseyGoalieBlueSkin;
						replayPlayerByClientId.JerseyGoalieRedSkin.Value = replayPlayerBodySpawned.JerseyGoalieRedSkin;
						replayPlayerByClientId.Server_SpawnPlayerBody(replayPlayerBodySpawned.Position, replayPlayerBodySpawned.Rotation, replayPlayerByClientId.Role.Value);
						return;
					}
					else
					{
						if (!(eventName == "PlayerInput"))
						{
							return;
						}
						ReplayPlayerInput replayPlayerInput = (ReplayPlayerInput)eventData;
						Player replayPlayerByClientId2 = NetworkBehaviourSingleton<PlayerManager>.Instance.GetReplayPlayerByClientId(replayPlayerInput.OwnerClientId);
						if (!replayPlayerByClientId2)
						{
							return;
						}
						replayPlayerByClientId2.PlayerInput.LookAngleInput.ClientValue = replayPlayerInput.LookAngleInput;
						replayPlayerByClientId2.PlayerInput.BladeAngleInput.ClientValue = replayPlayerInput.BladeAngleInput;
						replayPlayerByClientId2.PlayerInput.TrackInput.ClientValue = replayPlayerInput.TrackInput;
						replayPlayerByClientId2.PlayerInput.LookInput.ClientValue = replayPlayerInput.LookInput;
						return;
					}
				}
				else
				{
					if (!(eventName == "PuckSpawned"))
					{
						return;
					}
					ReplayPuckSpawned replayPuckSpawned = (ReplayPuckSpawned)eventData;
					Puck puck = NetworkBehaviourSingleton<PuckManager>.Instance.Server_SpawnPuck(replayPuckSpawned.Position, replayPuckSpawned.Rotation, Vector3.zero, true);
					this.replayPuckNetworkObjectIdMap.Add(replayPuckSpawned.NetworkObjectId, puck.NetworkObjectId);
					puck.transform.position = replayPuckSpawned.Position;
					puck.transform.rotation = replayPuckSpawned.Rotation;
					puck.Server_Freeze();
					return;
				}
			}
			else if (num != 551687134U)
			{
				if (num != 921486523U)
				{
					if (num != 1238133331U)
					{
						return;
					}
					if (!(eventName == "StickSpawned"))
					{
						return;
					}
					ReplayStickSpawned replayStickSpawned = (ReplayStickSpawned)eventData;
					Player replayPlayerByClientId3 = NetworkBehaviourSingleton<PlayerManager>.Instance.GetReplayPlayerByClientId(replayStickSpawned.OwnerClientId);
					if (!replayPlayerByClientId3)
					{
						return;
					}
					replayPlayerByClientId3.StickAttackerBlueSkin.Value = replayStickSpawned.StickAttackerBlueSkin;
					replayPlayerByClientId3.StickAttackerRedSkin.Value = replayStickSpawned.StickAttackerRedSkin;
					replayPlayerByClientId3.StickGoalieBlueSkin.Value = replayStickSpawned.StickGoalieBlueSkin;
					replayPlayerByClientId3.StickGoalieRedSkin.Value = replayStickSpawned.StickGoalieRedSkin;
					replayPlayerByClientId3.StickShaftAttackerBlueTapeSkin.Value = replayStickSpawned.StickShaftAttackerBlueTapeSkin;
					replayPlayerByClientId3.StickShaftAttackerRedTapeSkin.Value = replayStickSpawned.StickShaftAttackerRedTapeSkin;
					replayPlayerByClientId3.StickShaftGoalieBlueTapeSkin.Value = replayStickSpawned.StickShaftGoalieBlueTapeSkin;
					replayPlayerByClientId3.StickShaftGoalieRedTapeSkin.Value = replayStickSpawned.StickShaftGoalieRedTapeSkin;
					replayPlayerByClientId3.StickBladeAttackerBlueTapeSkin.Value = replayStickSpawned.StickBladeAttackerBlueTapeSkin;
					replayPlayerByClientId3.StickBladeAttackerRedTapeSkin.Value = replayStickSpawned.StickBladeAttackerRedTapeSkin;
					replayPlayerByClientId3.StickBladeGoalieBlueTapeSkin.Value = replayStickSpawned.StickBladeGoalieBlueTapeSkin;
					replayPlayerByClientId3.StickBladeGoalieRedTapeSkin.Value = replayStickSpawned.StickBladeGoalieRedTapeSkin;
					replayPlayerByClientId3.Server_SpawnStick(replayStickSpawned.Position, replayStickSpawned.Rotation, replayPlayerByClientId3.Role.Value);
					return;
				}
				else
				{
					if (!(eventName == "PuckDespawned"))
					{
						return;
					}
					ReplayPuckDespawned replayPuckDespawned = (ReplayPuckDespawned)eventData;
					if (!this.replayPuckNetworkObjectIdMap.ContainsKey(replayPuckDespawned.NetworkObjectId))
					{
						return;
					}
					Puck replayPuckByNetworkObjectId = NetworkBehaviourSingleton<PuckManager>.Instance.GetReplayPuckByNetworkObjectId(this.replayPuckNetworkObjectIdMap[replayPuckDespawned.NetworkObjectId]);
					if (!replayPuckByNetworkObjectId)
					{
						return;
					}
					NetworkBehaviourSingleton<PuckManager>.Instance.Server_DespawnPuck(replayPuckByNetworkObjectId);
					return;
				}
			}
			else
			{
				if (!(eventName == "StickMove"))
				{
					return;
				}
				ReplayStickMove replayStickMove = (ReplayStickMove)eventData;
				Player replayPlayerByClientId4 = NetworkBehaviourSingleton<PlayerManager>.Instance.GetReplayPlayerByClientId(replayStickMove.OwnerClientId);
				if (!replayPlayerByClientId4)
				{
					return;
				}
				if (!replayPlayerByClientId4.Stick)
				{
					return;
				}
				replayPlayerByClientId4.Stick.transform.DOKill(true);
				replayPlayerByClientId4.Stick.transform.DOMove(replayStickMove.Position, this.TickInterval, false).SetEase(Ease.Linear);
				replayPlayerByClientId4.Stick.transform.DORotateQuaternion(replayStickMove.Rotation, this.TickInterval).SetEase(Ease.Linear);
				return;
			}
		}
		else if (num <= 2284572537U)
		{
			if (num != 1769383370U)
			{
				if (num != 1946187694U)
				{
					if (num != 2284572537U)
					{
						return;
					}
					if (!(eventName == "PlayerBodyMove"))
					{
						return;
					}
					ReplayPlayerBodyMove replayPlayerBodyMove = (ReplayPlayerBodyMove)eventData;
					Player replayPlayerByClientId5 = NetworkBehaviourSingleton<PlayerManager>.Instance.GetReplayPlayerByClientId(replayPlayerBodyMove.OwnerClientId);
					if (!replayPlayerByClientId5)
					{
						return;
					}
					if (!replayPlayerByClientId5.PlayerBody)
					{
						return;
					}
					replayPlayerByClientId5.PlayerBody.transform.DOKill(true);
					replayPlayerByClientId5.PlayerBody.transform.DOMove(replayPlayerBodyMove.Position, this.TickInterval, false).SetEase(Ease.Linear);
					replayPlayerByClientId5.PlayerBody.transform.DORotateQuaternion(replayPlayerBodyMove.Rotation, this.TickInterval).SetEase(Ease.Linear);
					replayPlayerByClientId5.PlayerBody.StaminaCompressed.Value = replayPlayerBodyMove.Stamina;
					replayPlayerByClientId5.PlayerBody.SpeedCompressed.Value = replayPlayerBodyMove.Speed;
					replayPlayerByClientId5.PlayerBody.IsSprinting.Value = replayPlayerBodyMove.IsSprinting;
					replayPlayerByClientId5.PlayerBody.IsSliding.Value = replayPlayerBodyMove.IsSliding;
					replayPlayerByClientId5.PlayerBody.IsStopping.Value = replayPlayerBodyMove.IsStopping;
					replayPlayerByClientId5.PlayerBody.IsExtendedLeft.Value = replayPlayerBodyMove.IsExtendedLeft;
					replayPlayerByClientId5.PlayerBody.IsExtendedRight.Value = replayPlayerBodyMove.IsExtendedRight;
					return;
				}
				else
				{
					if (!(eventName == "PlayerSpawned"))
					{
						return;
					}
					ReplayPlayerSpawned replayPlayerSpawned = (ReplayPlayerSpawned)eventData;
					NetworkBehaviourSingleton<PlayerManager>.Instance.Server_SpawnPlayer(1337UL + replayPlayerSpawned.OwnerClientId, true);
					return;
				}
			}
			else
			{
				if (!(eventName == "StickDespawned"))
				{
					return;
				}
				ReplayStickDespawned replayStickDespawned = (ReplayStickDespawned)eventData;
				Player replayPlayerByClientId6 = NetworkBehaviourSingleton<PlayerManager>.Instance.GetReplayPlayerByClientId(replayStickDespawned.OwnerClientId);
				if (!replayPlayerByClientId6)
				{
					return;
				}
				if (!replayPlayerByClientId6.Stick)
				{
					return;
				}
				replayPlayerByClientId6.Server_DespawnStick();
				return;
			}
		}
		else if (num != 3342882255U)
		{
			if (num != 3490707091U)
			{
				if (num != 4197733569U)
				{
					return;
				}
				if (!(eventName == "PuckMove"))
				{
					return;
				}
				ReplayPuckMove replayPuckMove = (ReplayPuckMove)eventData;
				if (!this.replayPuckNetworkObjectIdMap.ContainsKey(replayPuckMove.NetworkObjectId))
				{
					return;
				}
				Puck replayPuckByNetworkObjectId2 = NetworkBehaviourSingleton<PuckManager>.Instance.GetReplayPuckByNetworkObjectId(this.replayPuckNetworkObjectIdMap[replayPuckMove.NetworkObjectId]);
				if (!replayPuckByNetworkObjectId2)
				{
					return;
				}
				replayPuckByNetworkObjectId2.transform.DOKill(true);
				replayPuckByNetworkObjectId2.transform.DOMove(replayPuckMove.Position, this.TickInterval, false).SetEase(Ease.Linear);
				replayPuckByNetworkObjectId2.transform.DORotateQuaternion(replayPuckMove.Rotation, this.TickInterval).SetEase(Ease.Linear);
				return;
			}
			else
			{
				if (!(eventName == "PlayerBodyDespawned"))
				{
					return;
				}
				ReplayPlayerBodyDespawned replayPlayerBodyDespawned = (ReplayPlayerBodyDespawned)eventData;
				Player replayPlayerByClientId7 = NetworkBehaviourSingleton<PlayerManager>.Instance.GetReplayPlayerByClientId(replayPlayerBodyDespawned.OwnerClientId);
				if (!replayPlayerByClientId7)
				{
					return;
				}
				if (!replayPlayerByClientId7.PlayerBody)
				{
					return;
				}
				replayPlayerByClientId7.Server_DespawnPlayerBody();
				return;
			}
		}
		else
		{
			if (!(eventName == "PlayerDespawned"))
			{
				return;
			}
			ReplayPlayerDespawned replayPlayerDespawned = (ReplayPlayerDespawned)eventData;
			Player replayPlayerByClientId8 = NetworkBehaviourSingleton<PlayerManager>.Instance.GetReplayPlayerByClientId(replayPlayerDespawned.OwnerClientId);
			if (!replayPlayerByClientId8)
			{
				return;
			}
			replayPlayerByClientId8.NetworkObject.Despawn(true);
			Debug.Log(string.Format("[ReplayPlayer] Despawned replay player {0}", replayPlayerByClientId8.OwnerClientId));
			return;
		}
	}

	// Token: 0x06000376 RID: 886 RVA: 0x00019E24 File Offset: 0x00018024
	private void Server_DisposeReplayObjects()
	{
		foreach (Player player in NetworkBehaviourSingleton<PlayerManager>.Instance.GetReplayPlayers())
		{
			if (player.PlayerBody)
			{
				player.PlayerBody.transform.DOKill(false);
				player.Server_DespawnPlayerBody();
			}
			if (player.Stick)
			{
				player.Stick.transform.DOKill(false);
				player.Server_DespawnStick();
			}
			player.NetworkObject.Despawn(true);
		}
		foreach (Puck puck in NetworkBehaviourSingleton<PuckManager>.Instance.GetReplayPucks())
		{
			puck.transform.DOKill(false);
			NetworkBehaviourSingleton<PuckManager>.Instance.Server_DespawnPuck(puck);
		}
	}

	// Token: 0x06000377 RID: 887 RVA: 0x00019F24 File Offset: 0x00018124
	private void Server_Dispose()
	{
		this.Server_DisposeReplayObjects();
		this.Tick = 0;
		this.EventMap.Clear();
		this.replayPuckNetworkObjectIdMap.Clear();
		this.replayPlayerSpawnedList.Clear();
		this.replayPlayerBodySpawnedList.Clear();
		this.replayStickSpawnedList.Clear();
		this.replayPuckSpawnedList.Clear();
	}

	// Token: 0x06000379 RID: 889 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x0600037A RID: 890 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x0600037B RID: 891 RVA: 0x0000936D File Offset: 0x0000756D
	protected internal override string __getTypeName()
	{
		return "ReplayPlayer";
	}

	// Token: 0x04000205 RID: 517
	[HideInInspector]
	public bool IsReplaying;

	// Token: 0x04000206 RID: 518
	[HideInInspector]
	public int TickRate = 15;

	// Token: 0x04000207 RID: 519
	[HideInInspector]
	public int Tick;

	// Token: 0x04000208 RID: 520
	[HideInInspector]
	public SortedList<int, List<ValueTuple<string, object>>> EventMap = new SortedList<int, List<ValueTuple<string, object>>>();

	// Token: 0x04000209 RID: 521
	private float tickAccumulator;

	// Token: 0x0400020A RID: 522
	private Dictionary<ulong, ulong> replayPuckNetworkObjectIdMap = new Dictionary<ulong, ulong>();

	// Token: 0x0400020B RID: 523
	private List<ReplayPlayerSpawned> replayPlayerSpawnedList = new List<ReplayPlayerSpawned>();

	// Token: 0x0400020C RID: 524
	private List<ReplayPlayerBodySpawned> replayPlayerBodySpawnedList = new List<ReplayPlayerBodySpawned>();

	// Token: 0x0400020D RID: 525
	private List<ReplayStickSpawned> replayStickSpawnedList = new List<ReplayStickSpawned>();

	// Token: 0x0400020E RID: 526
	private List<ReplayPuckSpawned> replayPuckSpawnedList = new List<ReplayPuckSpawned>();
}
