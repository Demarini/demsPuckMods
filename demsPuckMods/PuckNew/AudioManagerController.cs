using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000085 RID: 133
public class AudioManagerController : MonoBehaviour
{
	// Token: 0x0600047D RID: 1149 RVA: 0x00018928 File Offset: 0x00016B28
	private void Awake()
	{
		this.audioManager = base.GetComponent<AudioManager>();
		EventManager.AddEventListener("Event_OnGlobalVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnGlobalVolumeChanged));
		EventManager.AddEventListener("Event_OnAmbientVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnAmbientVolumeChanged));
		EventManager.AddEventListener("Event_OnGameVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnGameVolumeChanged));
		EventManager.AddEventListener("Event_OnVoiceVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnVoiceVolumeChanged));
		EventManager.AddEventListener("Event_OnUIVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnUIVolumeChanged));
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x000189B0 File Offset: 0x00016BB0
	private void Start()
	{
		this.audioManager.SetGlobalVolume(SettingsManager.GlobalVolume);
		this.audioManager.SetAmbientVolume(SettingsManager.AmbientVolume);
		this.audioManager.SetGameVolume(SettingsManager.GameVolume);
		this.audioManager.SetVoiceVolume(SettingsManager.VoiceVolume);
		this.audioManager.SetUIVolume(SettingsManager.UIVolume);
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x00018A10 File Offset: 0x00016C10
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnGlobalVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnGlobalVolumeChanged));
		EventManager.RemoveEventListener("Event_OnAmbientVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnAmbientVolumeChanged));
		EventManager.RemoveEventListener("Event_OnGameVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnGameVolumeChanged));
		EventManager.RemoveEventListener("Event_OnVoiceVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnVoiceVolumeChanged));
		EventManager.RemoveEventListener("Event_OnUIVolumeChanged", new Action<Dictionary<string, object>>(this.Event_OnUIVolumeChanged));
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x00018A8C File Offset: 0x00016C8C
	private void Event_OnGlobalVolumeChanged(Dictionary<string, object> eventParams)
	{
		float globalVolume = (float)eventParams["value"];
		this.audioManager.SetGlobalVolume(globalVolume);
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x00018AB8 File Offset: 0x00016CB8
	private void Event_OnAmbientVolumeChanged(Dictionary<string, object> eventParams)
	{
		float ambientVolume = (float)eventParams["value"];
		this.audioManager.SetAmbientVolume(ambientVolume);
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x00018AE4 File Offset: 0x00016CE4
	private void Event_OnGameVolumeChanged(Dictionary<string, object> eventParams)
	{
		float gameVolume = (float)eventParams["value"];
		this.audioManager.SetGameVolume(gameVolume);
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x00018B10 File Offset: 0x00016D10
	private void Event_OnVoiceVolumeChanged(Dictionary<string, object> eventParams)
	{
		float voiceVolume = (float)eventParams["value"];
		this.audioManager.SetVoiceVolume(voiceVolume);
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x00018B3C File Offset: 0x00016D3C
	private void Event_OnUIVolumeChanged(Dictionary<string, object> eventParams)
	{
		float uivolume = (float)eventParams["value"];
		this.audioManager.SetUIVolume(uivolume);
	}

	// Token: 0x040002C2 RID: 706
	private AudioManager audioManager;
}
