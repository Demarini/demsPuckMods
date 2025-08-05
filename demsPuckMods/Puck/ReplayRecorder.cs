using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000090 RID: 144
public class ReplayRecorder : NetworkBehaviour
{
	// Token: 0x06000384 RID: 900 RVA: 0x00019FE0 File Offset: 0x000181E0
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

	// Token: 0x06000385 RID: 901 RVA: 0x0001A068 File Offset: 0x00018268
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
		foreach (Player player in NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false))
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
		foreach (Puck puck in NetworkBehaviourSingleton<PuckManager>.Instance.GetPucks(false))
		{
			this.Server_AddPuckSpawnedEvent(puck);
		}
	}

	// Token: 0x06000386 RID: 902 RVA: 0x000093C8 File Offset: 0x000075C8
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

	// Token: 0x06000387 RID: 903 RVA: 0x0001A18C File Offset: 0x0001838C
	private void Server_Tick()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		foreach (Player player in NetworkBehaviourSingleton<PlayerManager>.Instance.GetSpawnedPlayers(false))
		{
			this.Server_AddReplayEvent("PlayerBodyMove", new ReplayPlayerBodyMove
			{
				OwnerClientId = player.OwnerClientId,
				Position = player.PlayerBody.transform.position,
				Rotation = player.PlayerBody.transform.rotation,
				Stamina = player.PlayerBody.StaminaCompressed.Value,
				Speed = player.PlayerBody.StaminaCompressed.Value,
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
		foreach (Puck puck in NetworkBehaviourSingleton<PuckManager>.Instance.GetPucks(false))
		{
			this.Server_AddReplayEvent("PuckMove", new ReplayPuckMove
			{
				NetworkObjectId = puck.NetworkObjectId,
				Position = puck.transform.position,
				Rotation = puck.transform.rotation
			});
		}
	}

	// Token: 0x06000388 RID: 904 RVA: 0x0001A464 File Offset: 0x00018664
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

	// Token: 0x06000389 RID: 905 RVA: 0x0001A4E0 File Offset: 0x000186E0
	public void Server_AddPlayerSpawnedEvent(Player player)
	{
		this.Server_AddReplayEvent("PlayerSpawned", new ReplayPlayerSpawned
		{
			OwnerClientId = player.OwnerClientId
		});
	}

	// Token: 0x0600038A RID: 906 RVA: 0x0001A514 File Offset: 0x00018714
	public void Server_AddPlayerDespawnedEvent(Player player)
	{
		this.Server_AddReplayEvent("PlayerDespawned", new ReplayPlayerDespawned
		{
			OwnerClientId = player.OwnerClientId
		});
	}

	// Token: 0x0600038B RID: 907 RVA: 0x0001A548 File Offset: 0x00018748
	public void Server_AddPlayerBodySpawnedEvent(PlayerBodyV2 playerBody)
	{
		this.Server_AddReplayEvent("PlayerBodySpawned", new ReplayPlayerBodySpawned
		{
			OwnerClientId = playerBody.OwnerClientId,
			Position = playerBody.transform.position,
			Rotation = playerBody.transform.rotation,
			Username = playerBody.Player.Username.Value,
			Number = playerBody.Player.Number.Value,
			Team = playerBody.Player.Team.Value,
			Role = playerBody.Player.Role.Value,
			Country = playerBody.Player.Country.Value,
			VisorAttackerBlueSkin = playerBody.Player.VisorAttackerBlueSkin.Value,
			VisorAttackerRedSkin = playerBody.Player.VisorAttackerRedSkin.Value,
			VisorGoalieBlueSkin = playerBody.Player.VisorGoalieBlueSkin.Value,
			VisorGoalieRedSkin = playerBody.Player.VisorGoalieRedSkin.Value,
			Mustache = playerBody.Player.Mustache.Value,
			Beard = playerBody.Player.Beard.Value,
			JerseyAttackerBlueSkin = playerBody.Player.JerseyAttackerBlueSkin.Value,
			JerseyAttackerRedSkin = playerBody.Player.JerseyAttackerRedSkin.Value,
			JerseyGoalieBlueSkin = playerBody.Player.JerseyGoalieBlueSkin.Value,
			JerseyGoalieRedSkin = playerBody.Player.JerseyGoalieRedSkin.Value
		});
	}

	// Token: 0x0600038C RID: 908 RVA: 0x0001A6F8 File Offset: 0x000188F8
	public void Server_AddPlayerBodyDespawnedEvent(PlayerBodyV2 playerBody)
	{
		this.Server_AddReplayEvent("PlayerBodyDespawned", new ReplayPlayerBodyDespawned
		{
			OwnerClientId = playerBody.OwnerClientId
		});
	}

	// Token: 0x0600038D RID: 909 RVA: 0x0001A72C File Offset: 0x0001892C
	public void Server_AddStickSpawnedEvent(Stick stick)
	{
		this.Server_AddReplayEvent("StickSpawned", new ReplayStickSpawned
		{
			OwnerClientId = stick.OwnerClientId,
			Position = stick.transform.position,
			Rotation = stick.transform.rotation,
			StickAttackerBlueSkin = stick.Player.StickAttackerBlueSkin.Value,
			StickAttackerRedSkin = stick.Player.StickAttackerRedSkin.Value,
			StickGoalieBlueSkin = stick.Player.StickGoalieBlueSkin.Value,
			StickGoalieRedSkin = stick.Player.StickGoalieRedSkin.Value,
			StickShaftAttackerBlueTapeSkin = stick.Player.StickShaftAttackerBlueTapeSkin.Value,
			StickShaftAttackerRedTapeSkin = stick.Player.StickShaftAttackerRedTapeSkin.Value,
			StickShaftGoalieBlueTapeSkin = stick.Player.StickShaftGoalieBlueTapeSkin.Value,
			StickShaftGoalieRedTapeSkin = stick.Player.StickShaftGoalieRedTapeSkin.Value,
			StickBladeAttackerBlueTapeSkin = stick.Player.StickBladeAttackerBlueTapeSkin.Value,
			StickBladeAttackerRedTapeSkin = stick.Player.StickBladeAttackerRedTapeSkin.Value,
			StickBladeGoalieBlueTapeSkin = stick.Player.StickBladeGoalieBlueTapeSkin.Value,
			StickBladeGoalieRedTapeSkin = stick.Player.StickBladeGoalieRedTapeSkin.Value
		});
	}

	// Token: 0x0600038E RID: 910 RVA: 0x0001A898 File Offset: 0x00018A98
	public void Server_AddStickDespawnedEvent(Stick stick)
	{
		this.Server_AddReplayEvent("StickDespawned", new ReplayStickDespawned
		{
			OwnerClientId = stick.OwnerClientId
		});
	}

	// Token: 0x0600038F RID: 911 RVA: 0x0001A8CC File Offset: 0x00018ACC
	public void Server_AddPuckSpawnedEvent(Puck puck)
	{
		this.Server_AddReplayEvent("PuckSpawned", new ReplayPuckSpawned
		{
			NetworkObjectId = puck.NetworkObjectId,
			Position = puck.transform.position,
			Rotation = puck.transform.rotation
		});
	}

	// Token: 0x06000390 RID: 912 RVA: 0x0001A924 File Offset: 0x00018B24
	public void Server_AddPuckDespawnedEvent(Puck puck)
	{
		this.Server_AddReplayEvent("PuckDespawned", new ReplayPuckDespawned
		{
			NetworkObjectId = puck.NetworkObjectId
		});
	}

	// Token: 0x06000392 RID: 914 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000393 RID: 915 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000394 RID: 916 RVA: 0x00009413 File Offset: 0x00007613
	protected internal override string __getTypeName()
	{
		return "ReplayRecorder";
	}

	// Token: 0x04000213 RID: 531
	[HideInInspector]
	public bool IsRecording;

	// Token: 0x04000214 RID: 532
	[HideInInspector]
	public int TickRate = 15;

	// Token: 0x04000215 RID: 533
	[HideInInspector]
	public int Tick;

	// Token: 0x04000216 RID: 534
	[HideInInspector]
	public SortedList<int, List<ValueTuple<string, object>>> EventMap = new SortedList<int, List<ValueTuple<string, object>>>();

	// Token: 0x04000217 RID: 535
	private float tickAccumulator;
}
