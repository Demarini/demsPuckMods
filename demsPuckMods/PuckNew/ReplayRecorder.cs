using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000109 RID: 265
public class ReplayRecorder : MonoBehaviour
{
	// Token: 0x06000726 RID: 1830 RVA: 0x00023694 File Offset: 0x00021894
	private void Update()
	{
		if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!this.IsRecording)
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
			this.Server_Tick();
			this.Tick++;
		}
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x00023720 File Offset: 0x00021920
	public void Server_StartRecording(int tickRate)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.IsRecording)
		{
			return;
		}
		Debug.Log(string.Format("[ReplayRecorder] Replay recording started at {0} ticks per second", this.TickRate));
		this.EventMap.Clear();
		this.TickRate = tickRate;
		this.Tick = 0;
		this.IsRecording = true;
		foreach (Player player in MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false))
		{
			this.Server_AddPlayerSpawnedEvent(player);
			if (player.PlayerBody)
			{
				this.Server_AddPlayerBodySpawnedEvent(player.PlayerBody);
			}
			if (player.Stick)
			{
				this.Server_AddStickSpawnedEvent(player.Stick);
			}
		}
		foreach (Puck puck in MonoBehaviourSingleton<PuckManager>.Instance.GetPucks(false))
		{
			this.Server_AddPuckSpawnedEvent(puck);
		}
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x00023844 File Offset: 0x00021A44
	public void Server_StopRecording()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!this.IsRecording)
		{
			return;
		}
		Debug.Log("[ReplayRecorder] Replay recording stopped");
		this.Tick = 0;
		this.IsRecording = false;
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x00023874 File Offset: 0x00021A74
	private void Server_Tick()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		foreach (Player player in MonoBehaviourSingleton<PlayerManager>.Instance.GetSpawnedPlayers(false))
		{
			this.Server_AddReplayEvent("PlayerBodyMove", new ReplayPlayerBodyMove
			{
				OwnerClientId = player.OwnerClientId,
				Position = player.PlayerBody.transform.position,
				Rotation = player.PlayerBody.transform.rotation,
				Stamina = player.PlayerBody.Stamina.Value,
				Speed = player.PlayerBody.Speed.Value,
				IsSprinting = player.PlayerBody.IsSprinting.Value,
				IsSliding = player.PlayerBody.IsSliding.Value,
				IsStopping = player.PlayerBody.IsStopping.Value,
				IsExtendedLeft = player.PlayerBody.IsExtendedLeft.Value,
				IsExtendedRight = player.PlayerBody.IsExtendedRight.Value
			});
			this.Server_AddReplayEvent("StickMove", new ReplayStickMove
			{
				OwnerClientId = player.OwnerClientId,
				Position = player.Stick.transform.position,
				Rotation = player.Stick.transform.rotation
			});
			this.Server_AddReplayEvent("PlayerInput", new ReplayPlayerInput
			{
				OwnerClientId = player.OwnerClientId,
				LookAngleInput = player.PlayerInput.LookAngleInput.ServerValue,
				BladeAngleInput = player.PlayerInput.BladeAngleInput.ServerValue,
				TrackInput = player.PlayerInput.TrackInput.ServerValue,
				LookInput = player.PlayerInput.LookInput.ServerValue
			});
		}
		foreach (Puck puck in MonoBehaviourSingleton<PuckManager>.Instance.GetPucks(false))
		{
			this.Server_AddReplayEvent("PuckMove", new ReplayPuckMove
			{
				NetworkObjectId = puck.NetworkObjectId,
				Position = puck.transform.position,
				Rotation = puck.transform.rotation
			});
		}
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x00023B4C File Offset: 0x00021D4C
	public void Server_AddReplayEvent(string eventName, object eventData)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!this.IsRecording)
		{
			return;
		}
		if (this.EventMap.ContainsKey(this.Tick))
		{
			this.EventMap[this.Tick].Add(new ValueTuple<string, object>(eventName, eventData));
			return;
		}
		List<ValueTuple<string, object>> value = new List<ValueTuple<string, object>>
		{
			new ValueTuple<string, object>(eventName, eventData)
		};
		this.EventMap.Add(this.Tick, value);
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x00023BC8 File Offset: 0x00021DC8
	public void Server_AddPlayerSpawnedEvent(Player player)
	{
		this.Server_AddReplayEvent("PlayerSpawned", new ReplayPlayerSpawned
		{
			OwnerClientId = player.OwnerClientId,
			GameState = player.GameState.Value,
			CustomizationState = player.CustomizationState.Value,
			Handedness = player.Handedness.Value,
			SteamId = player.SteamId.Value,
			Username = player.Username.Value,
			Number = player.Number.Value,
			PatreonLevel = player.PatreonLevel.Value,
			AdminLevel = player.AdminLevel.Value,
			IsMuted = player.IsMuted.Value
		});
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x00023CA0 File Offset: 0x00021EA0
	public void Server_AddPlayerDespawnedEvent(Player player)
	{
		this.Server_AddReplayEvent("PlayerDespawned", new ReplayPlayerDespawned
		{
			OwnerClientId = player.OwnerClientId
		});
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x00023CD4 File Offset: 0x00021ED4
	public void Server_AddPlayerBodySpawnedEvent(PlayerBody playerBody)
	{
		this.Server_AddReplayEvent("PlayerBodySpawned", new ReplayPlayerBodySpawned
		{
			OwnerClientId = playerBody.OwnerClientId,
			Position = playerBody.transform.position,
			Rotation = playerBody.transform.rotation,
			GameState = playerBody.Player.GameState.Value,
			CustomizationState = playerBody.Player.CustomizationState.Value,
			Username = playerBody.Player.Username.Value,
			Number = playerBody.Player.Number.Value
		});
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x00023D88 File Offset: 0x00021F88
	public void Server_AddPlayerBodyDespawnedEvent(PlayerBody playerBody)
	{
		this.Server_AddReplayEvent("PlayerBodyDespawned", new ReplayPlayerBodyDespawned
		{
			OwnerClientId = playerBody.OwnerClientId
		});
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x00023DBC File Offset: 0x00021FBC
	public void Server_AddStickSpawnedEvent(Stick stick)
	{
		this.Server_AddReplayEvent("StickSpawned", new ReplayStickSpawned
		{
			OwnerClientId = stick.OwnerClientId,
			Position = stick.transform.position,
			Rotation = stick.transform.rotation
		});
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x00023E14 File Offset: 0x00022014
	public void Server_AddStickDespawnedEvent(Stick stick)
	{
		this.Server_AddReplayEvent("StickDespawned", new ReplayStickDespawned
		{
			OwnerClientId = stick.OwnerClientId
		});
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x00023E48 File Offset: 0x00022048
	public void Server_AddPuckSpawnedEvent(Puck puck)
	{
		this.Server_AddReplayEvent("PuckSpawned", new ReplayPuckSpawned
		{
			NetworkObjectId = puck.NetworkObjectId,
			Position = puck.transform.position,
			Rotation = puck.transform.rotation
		});
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x00023EA0 File Offset: 0x000220A0
	public void Server_AddPuckDespawnedEvent(Puck puck)
	{
		this.Server_AddReplayEvent("PuckDespawned", new ReplayPuckDespawned
		{
			NetworkObjectId = puck.NetworkObjectId
		});
	}

	// Token: 0x04000471 RID: 1137
	[HideInInspector]
	public bool IsRecording;

	// Token: 0x04000472 RID: 1138
	[HideInInspector]
	public int TickRate = 15;

	// Token: 0x04000473 RID: 1139
	[HideInInspector]
	public int Tick;

	// Token: 0x04000474 RID: 1140
	[HideInInspector]
	public SortedList<int, List<ValueTuple<string, object>>> EventMap = new SortedList<int, List<ValueTuple<string, object>>>();

	// Token: 0x04000475 RID: 1141
	private float tickAccumulator;
}
