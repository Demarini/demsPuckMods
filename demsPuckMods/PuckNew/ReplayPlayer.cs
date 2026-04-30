using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000104 RID: 260
public class ReplayPlayer : MonoBehaviour
{
	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x06000713 RID: 1811 RVA: 0x00022790 File Offset: 0x00020990
	[HideInInspector]
	public float TickInterval
	{
		get
		{
			return 1f / (float)this.TickRate;
		}
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x000227A0 File Offset: 0x000209A0
	private void Update()
	{
		if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
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

	// Token: 0x06000715 RID: 1813 RVA: 0x00022834 File Offset: 0x00020A34
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

	// Token: 0x06000716 RID: 1814 RVA: 0x000228F4 File Offset: 0x00020AF4
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
		this.IsReplaying = false;
		this.Server_Dispose();
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x0002291C File Offset: 0x00020B1C
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

	// Token: 0x06000718 RID: 1816 RVA: 0x000229C8 File Offset: 0x00020BC8
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

	// Token: 0x06000719 RID: 1817 RVA: 0x00022B2C File Offset: 0x00020D2C
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

	// Token: 0x0600071A RID: 1818 RVA: 0x00022D3C File Offset: 0x00020F3C
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
						Player replayPlayerByClientId = MonoBehaviourSingleton<PlayerManager>.Instance.GetReplayPlayerByClientId(replayPlayerBodySpawned.OwnerClientId);
						if (!replayPlayerByClientId)
						{
							return;
						}
						replayPlayerByClientId.GameState.Value = replayPlayerBodySpawned.GameState;
						replayPlayerByClientId.CustomizationState.Value = replayPlayerBodySpawned.CustomizationState;
						replayPlayerByClientId.Username.Value = replayPlayerBodySpawned.Username;
						replayPlayerByClientId.Number.Value = replayPlayerBodySpawned.Number;
						replayPlayerByClientId.Server_SpawnPlayerBody(replayPlayerBodySpawned.Position, replayPlayerBodySpawned.Rotation, replayPlayerByClientId.Role);
						return;
					}
					else
					{
						if (!(eventName == "PlayerInput"))
						{
							return;
						}
						ReplayPlayerInput replayPlayerInput = (ReplayPlayerInput)eventData;
						Player replayPlayerByClientId2 = MonoBehaviourSingleton<PlayerManager>.Instance.GetReplayPlayerByClientId(replayPlayerInput.OwnerClientId);
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
					Puck puck = MonoBehaviourSingleton<PuckManager>.Instance.Server_SpawnPuck(replayPuckSpawned.Position, replayPuckSpawned.Rotation, true);
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
					Player replayPlayerByClientId3 = MonoBehaviourSingleton<PlayerManager>.Instance.GetReplayPlayerByClientId(replayStickSpawned.OwnerClientId);
					if (!replayPlayerByClientId3)
					{
						return;
					}
					replayPlayerByClientId3.Server_SpawnStick(replayStickSpawned.Position, replayStickSpawned.Rotation, replayPlayerByClientId3.Role);
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
					Puck replayPuckByNetworkObjectId = MonoBehaviourSingleton<PuckManager>.Instance.GetReplayPuckByNetworkObjectId(this.replayPuckNetworkObjectIdMap[replayPuckDespawned.NetworkObjectId]);
					if (!replayPuckByNetworkObjectId)
					{
						return;
					}
					MonoBehaviourSingleton<PuckManager>.Instance.Server_DespawnPuck(replayPuckByNetworkObjectId);
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
				Player replayPlayerByClientId4 = MonoBehaviourSingleton<PlayerManager>.Instance.GetReplayPlayerByClientId(replayStickMove.OwnerClientId);
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
					Player replayPlayerByClientId5 = MonoBehaviourSingleton<PlayerManager>.Instance.GetReplayPlayerByClientId(replayPlayerBodyMove.OwnerClientId);
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
					replayPlayerByClientId5.PlayerBody.Stamina.Value = replayPlayerBodyMove.Stamina;
					replayPlayerByClientId5.PlayerBody.Speed.Value = replayPlayerBodyMove.Speed;
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
					MonoBehaviourSingleton<PlayerManager>.Instance.Server_SpawnPlayer(replayPlayerSpawned.OwnerClientId, replayPlayerSpawned.GameState, replayPlayerSpawned.CustomizationState, replayPlayerSpawned.Handedness, replayPlayerSpawned.SteamId.ToString(), replayPlayerSpawned.Username.ToString(), replayPlayerSpawned.Number, replayPlayerSpawned.PatreonLevel, replayPlayerSpawned.AdminLevel, replayPlayerSpawned.IsMuted, true);
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
				Player replayPlayerByClientId6 = MonoBehaviourSingleton<PlayerManager>.Instance.GetReplayPlayerByClientId(replayStickDespawned.OwnerClientId);
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
				Puck replayPuckByNetworkObjectId2 = MonoBehaviourSingleton<PuckManager>.Instance.GetReplayPuckByNetworkObjectId(this.replayPuckNetworkObjectIdMap[replayPuckMove.NetworkObjectId]);
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
				Player replayPlayerByClientId7 = MonoBehaviourSingleton<PlayerManager>.Instance.GetReplayPlayerByClientId(replayPlayerBodyDespawned.OwnerClientId);
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
			Player replayPlayerByClientId8 = MonoBehaviourSingleton<PlayerManager>.Instance.GetReplayPlayerByClientId(replayPlayerDespawned.OwnerClientId);
			if (!replayPlayerByClientId8)
			{
				return;
			}
			replayPlayerByClientId8.NetworkObject.Despawn(true);
			Debug.Log(string.Format("[ReplayPlayer] Despawned replay player {0}", replayPlayerByClientId8.OwnerClientId));
			return;
		}
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x00023450 File Offset: 0x00021650
	private void Server_DisposeReplayObjects()
	{
		foreach (Player player in MonoBehaviourSingleton<PlayerManager>.Instance.GetReplayPlayers())
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
			Debug.Log(string.Format("[ReplayPlayer] Despawned replay player {0}", player.OwnerClientId));
		}
		foreach (Puck puck in MonoBehaviourSingleton<PuckManager>.Instance.GetReplayPucks())
		{
			puck.transform.DOKill(false);
			MonoBehaviourSingleton<PuckManager>.Instance.Server_DespawnPuck(puck);
			Debug.Log(string.Format("[ReplayPlayer] Despawned replay puck {0}", puck.OwnerClientId));
		}
	}

	// Token: 0x0600071C RID: 1820 RVA: 0x00023584 File Offset: 0x00021784
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

	// Token: 0x04000463 RID: 1123
	[HideInInspector]
	public bool IsReplaying;

	// Token: 0x04000464 RID: 1124
	[HideInInspector]
	public int TickRate = 15;

	// Token: 0x04000465 RID: 1125
	[HideInInspector]
	public int Tick;

	// Token: 0x04000466 RID: 1126
	[HideInInspector]
	public SortedList<int, List<ValueTuple<string, object>>> EventMap = new SortedList<int, List<ValueTuple<string, object>>>();

	// Token: 0x04000467 RID: 1127
	private float tickAccumulator;

	// Token: 0x04000468 RID: 1128
	private Dictionary<ulong, ulong> replayPuckNetworkObjectIdMap = new Dictionary<ulong, ulong>();

	// Token: 0x04000469 RID: 1129
	private List<ReplayPlayerSpawned> replayPlayerSpawnedList = new List<ReplayPlayerSpawned>();

	// Token: 0x0400046A RID: 1130
	private List<ReplayPlayerBodySpawned> replayPlayerBodySpawnedList = new List<ReplayPlayerBodySpawned>();

	// Token: 0x0400046B RID: 1131
	private List<ReplayStickSpawned> replayStickSpawnedList = new List<ReplayStickSpawned>();

	// Token: 0x0400046C RID: 1132
	private List<ReplayPuckSpawned> replayPuckSpawnedList = new List<ReplayPuckSpawned>();
}
