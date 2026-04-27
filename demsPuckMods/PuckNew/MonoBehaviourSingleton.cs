using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020001E2 RID: 482
public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	// Token: 0x17000125 RID: 293
	// (get) Token: 0x06000E13 RID: 3603 RVA: 0x000424E4 File Offset: 0x000406E4
	public static T Instance
	{
		get
		{
			return MonoBehaviourSingleton<!0>.instance;
		}
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x000424EC File Offset: 0x000406EC
	public virtual void Awake()
	{
		if (MonoBehaviourSingleton<!0>.instance != null && MonoBehaviourSingleton<!0>.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (MonoBehaviourSingleton<!0>.instance == null)
		{
			MonoBehaviourSingleton<!0>.instance = (this as !0);
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	// Token: 0x06000E15 RID: 3605 RVA: 0x00042556 File Offset: 0x00040756
	public void AllowSceneDestruction()
	{
		UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(base.gameObject, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
	}

	// Token: 0x0400088C RID: 2188
	private static T instance;
}
