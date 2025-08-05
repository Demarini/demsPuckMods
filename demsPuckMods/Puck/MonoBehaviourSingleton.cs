using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000013 RID: 19
public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	// Token: 0x17000008 RID: 8
	// (get) Token: 0x0600008F RID: 143 RVA: 0x000071E0 File Offset: 0x000053E0
	// (set) Token: 0x0600008E RID: 142 RVA: 0x000071D8 File Offset: 0x000053D8
	public static T Instance
	{
		get
		{
			return MonoBehaviourSingleton<T>.instance;
		}
		set
		{
			MonoBehaviourSingleton<T>.instance = value;
		}
	}

	// Token: 0x06000090 RID: 144 RVA: 0x00010C24 File Offset: 0x0000EE24
	public virtual void Awake()
	{
		if (MonoBehaviourSingleton<T>.instance != null && MonoBehaviourSingleton<T>.instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		MonoBehaviourSingleton<T>.instance = UnityEngine.Object.FindFirstObjectByType<T>();
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06000091 RID: 145 RVA: 0x000071E7 File Offset: 0x000053E7
	public void DestroyOnLoad()
	{
		UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(base.gameObject, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
	}

	// Token: 0x04000040 RID: 64
	private static T instance;
}
