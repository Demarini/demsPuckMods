using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

// Token: 0x020000D3 RID: 211
public class ConnectionManager : MonoBehaviourSingleton<ConnectionManager>
{
	// Token: 0x06000678 RID: 1656 RVA: 0x000209A8 File Offset: 0x0001EBA8
	private void Start()
	{
		this.UnityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
		NetworkManager.Singleton.OnClientStarted += this.Client_OnClientStarted;
		NetworkManager.Singleton.OnClientStopped += this.Client_OnClientStopped;
		NetworkManager.Singleton.NetworkConfig.ProtocolVersion = ApplicationManager.Version;
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x00020A05 File Offset: 0x0001EC05
	private void OnDestroy()
	{
		if (NetworkManager.Singleton != null)
		{
			NetworkManager.Singleton.OnClientStarted -= this.Client_OnClientStarted;
			NetworkManager.Singleton.OnClientStopped -= this.Client_OnClientStopped;
		}
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x00020A40 File Offset: 0x0001EC40
	public void Client_StartClient(string ipAddress, ushort port, string password = null)
	{
		Debug.Log(string.Format("[ConnectionManager] Starting client {0}:{1}", ipAddress, port));
		if (NetworkManager.Singleton.IsClient)
		{
			Connection value = new Connection
			{
				EndPoint = new EndPoint(ipAddress, port),
				Password = password
			};
			GlobalStateManager.SetConnectionState(new Dictionary<string, object>
			{
				{
					"pendingConnection",
					value
				}
			});
			this.Client_Disconnect();
			return;
		}
		string s = JsonSerializer.Serialize<ConnectionData>(new ConnectionData
		{
			SteamId = BackendManager.PlayerState.PlayerData.steamId,
			Key = BackendManager.PlayerState.Key,
			Password = password,
			EnabledModIds = MonoBehaviourSingleton<ModManager>.Instance.EnabledModIds,
			Handedness = SettingsManager.Handedness,
			FlagID = SettingsManager.FlagID,
			HeadgearIDBlueAttacker = SettingsManager.HeadgearIDBlueAttacker,
			HeadgearIDRedAttacker = SettingsManager.HeadgearIDRedAttacker,
			HeadgearIDBlueGoalie = SettingsManager.HeadgearIDBlueGoalie,
			HeadgearIDRedGoalie = SettingsManager.HeadgearIDRedGoalie,
			MustacheID = SettingsManager.MustacheID,
			BeardID = SettingsManager.BeardID,
			JerseyIDBlueAttacker = SettingsManager.JerseyIDBlueAttacker,
			JerseyIDRedAttacker = SettingsManager.JerseyIDRedAttacker,
			JerseyIDBlueGoalie = SettingsManager.JerseyIDBlueGoalie,
			JerseyIDRedGoalie = SettingsManager.JerseyIDRedGoalie,
			StickSkinIDBlueAttacker = SettingsManager.StickSkinIDBlueAttacker,
			StickSkinIDRedAttacker = SettingsManager.StickSkinIDRedAttacker,
			StickSkinIDBlueGoalie = SettingsManager.StickSkinIDBlueGoalie,
			StickSkinIDRedGoalie = SettingsManager.StickSkinIDRedGoalie,
			StickShaftTapeIDBlueAttacker = SettingsManager.StickShaftTapeIDBlueAttacker,
			StickShaftTapeIDRedAttacker = SettingsManager.StickShaftTapeIDRedAttacker,
			StickShaftTapeIDBlueGoalie = SettingsManager.StickShaftTapeIDBlueGoalie,
			StickShaftTapeIDRedGoalie = SettingsManager.StickShaftTapeIDRedGoalie,
			StickBladeTapeIDBlueAttacker = SettingsManager.StickBladeTapeIDBlueAttacker,
			StickBladeTapeIDRedAttacker = SettingsManager.StickBladeTapeIDRedAttacker,
			StickBladeTapeIDBlueGoalie = SettingsManager.StickBladeTapeIDBlueGoalie,
			StickBladeTapeIDRedGoalie = SettingsManager.StickBladeTapeIDRedGoalie
		}, null);
		NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(s);
		this.UnityTransport.SetConnectionData(ipAddress, port, null);
		Connection value2 = new Connection
		{
			EndPoint = new EndPoint(ipAddress, port),
			Password = password
		};
		GlobalStateManager.SetConnectionState(new Dictionary<string, object>
		{
			{
				"connection",
				value2
			},
			{
				"pendingConnection",
				null
			}
		});
		NetworkManager.Singleton.StartClient();
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x00020C65 File Offset: 0x0001EE65
	private void Client_OnClientStarted()
	{
		EventManager.TriggerEvent("Event_OnClientStarted", null);
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x00020C72 File Offset: 0x0001EE72
	private void Client_OnClientStopped(bool wasHost)
	{
		base.StartCoroutine(this.DelayedOnClientStopped(wasHost));
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x00020C82 File Offset: 0x0001EE82
	private IEnumerator DelayedOnClientStopped(bool wasHost)
	{
		yield return new WaitForEndOfFrame();
		EventManager.TriggerEvent("Event_OnClientStopped", new Dictionary<string, object>
		{
			{
				"wasHost",
				wasHost
			}
		});
		yield break;
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x00020C91 File Offset: 0x0001EE91
	public void Client_Disconnect()
	{
		if (!NetworkManager.Singleton.IsClient)
		{
			return;
		}
		Debug.Log(string.Format("[ConnectionManager] Puck ({0}) network shutdown", ApplicationManager.Version));
		NetworkManager.Singleton.Shutdown(true);
	}

	// Token: 0x040003F9 RID: 1017
	[HideInInspector]
	public UnityTransport UnityTransport;
}
