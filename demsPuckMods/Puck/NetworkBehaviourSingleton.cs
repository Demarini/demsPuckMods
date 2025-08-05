using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000014 RID: 20
public class NetworkBehaviourSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
	// Token: 0x17000009 RID: 9
	// (get) Token: 0x06000093 RID: 147 RVA: 0x000071F9 File Offset: 0x000053F9
	public static T Instance
	{
		get
		{
			return NetworkBehaviourSingleton<T>.instance;
		}
	}

	// Token: 0x06000094 RID: 148 RVA: 0x00010C78 File Offset: 0x0000EE78
	public virtual void Awake()
	{
		if (NetworkBehaviourSingleton<T>.instance != null && NetworkBehaviourSingleton<T>.instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		NetworkBehaviourSingleton<T>.instance = UnityEngine.Object.FindFirstObjectByType<T>();
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06000095 RID: 149 RVA: 0x000071E7 File Offset: 0x000053E7
	public void DestroyOnLoad()
	{
		UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(base.gameObject, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
	}

	// Token: 0x06000097 RID: 151 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000098 RID: 152 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000099 RID: 153 RVA: 0x00007200 File Offset: 0x00005400
	protected internal override string __getTypeName()
	{
		return "NetworkBehaviourSingleton`1";
	}

	// Token: 0x04000041 RID: 65
	private static T instance;
}
