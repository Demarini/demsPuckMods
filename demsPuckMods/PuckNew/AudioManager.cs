using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000084 RID: 132
public class AudioManager : MonoBehaviourSingleton<AudioManager>
{
	// Token: 0x06000477 RID: 1143 RVA: 0x00018866 File Offset: 0x00016A66
	public void SetGlobalVolume(float volume)
	{
		this.mixer.SetFloat("globalVolume", Mathf.Log(volume + 0.001f) * 20f);
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x0001888B File Offset: 0x00016A8B
	public void SetAmbientVolume(float volume)
	{
		this.mixer.SetFloat("ambientVolume", Mathf.Log(volume + 0.001f) * 20f);
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x000188B0 File Offset: 0x00016AB0
	public void SetGameVolume(float volume)
	{
		this.mixer.SetFloat("gameVolume", Mathf.Log(volume + 0.001f) * 20f);
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x000188D5 File Offset: 0x00016AD5
	public void SetVoiceVolume(float volume)
	{
		this.mixer.SetFloat("voiceVolume", Mathf.Log(volume + 0.001f) * 20f);
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x000188FA File Offset: 0x00016AFA
	public void SetUIVolume(float volume)
	{
		this.mixer.SetFloat("uiVolume", Mathf.Log(volume + 0.001f) * 20f);
	}

	// Token: 0x040002C1 RID: 705
	[Header("References")]
	[SerializeField]
	private AudioMixer mixer;
}
