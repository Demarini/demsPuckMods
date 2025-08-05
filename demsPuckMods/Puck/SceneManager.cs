using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000093 RID: 147
public class SceneManager : NetworkBehaviourSingleton<global::SceneManager>
{
	// Token: 0x060003A4 RID: 932 RVA: 0x0000942F File Offset: 0x0000762F
	private void Start()
	{
		UnityEngine.SceneManagement.SceneManager.sceneLoaded += this.OnSceneLoaded;
		UnityEngine.SceneManagement.SceneManager.sceneUnloaded += this.OnSceneUnloaded;
		if (!Application.isBatchMode)
		{
			this.LoadChangingRoomScene();
		}
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x00009460 File Offset: 0x00007660
	public override void OnDestroy()
	{
		UnityEngine.SceneManagement.SceneManager.sceneLoaded -= this.OnSceneLoaded;
		UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= this.OnSceneUnloaded;
		base.OnDestroy();
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x0000948A File Offset: 0x0000768A
	public void LoadChangingRoomScene()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(1);
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x00009492 File Offset: 0x00007692
	public void LoadLevel1Scene()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(2);
	}

	// Token: 0x060003A8 RID: 936 RVA: 0x0000949A File Offset: 0x0000769A
	private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnSceneLoaded", new Dictionary<string, object>
		{
			{
				"scene",
				scene
			}
		});
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x000094C1 File Offset: 0x000076C1
	private void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnSceneUnloaded", new Dictionary<string, object>
		{
			{
				"scene",
				scene
			}
		});
	}

	// Token: 0x060003AB RID: 939 RVA: 0x0001AD84 File Offset: 0x00018F84
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060003AC RID: 940 RVA: 0x000094F0 File Offset: 0x000076F0
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060003AD RID: 941 RVA: 0x000094FA File Offset: 0x000076FA
	protected internal override string __getTypeName()
	{
		return "SceneManager";
	}
}
