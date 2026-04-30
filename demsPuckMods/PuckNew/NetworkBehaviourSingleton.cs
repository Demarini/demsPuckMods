using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020001E4 RID: 484
public class NetworkBehaviourSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
	// Token: 0x17000126 RID: 294
	// (get) Token: 0x06000E19 RID: 3609 RVA: 0x000425EC File Offset: 0x000407EC
	public static T Instance
	{
		get
		{
			return NetworkBehaviourSingleton<!0>.instance;
		}
	}

	// Token: 0x06000E1A RID: 3610 RVA: 0x000425F4 File Offset: 0x000407F4
	public virtual void Awake()
	{
		if (NetworkBehaviourSingleton<!0>.instance != null && NetworkBehaviourSingleton<!0>.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (NetworkBehaviourSingleton<!0>.instance == null)
		{
			NetworkBehaviourSingleton<!0>.instance = (this as !0);
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	// Token: 0x06000E1B RID: 3611 RVA: 0x00042556 File Offset: 0x00040756
	public void AllowSceneDestruction()
	{
		UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(base.gameObject, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
	}

	// Token: 0x06000E1D RID: 3613 RVA: 0x00042660 File Offset: 0x00040860
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000E1E RID: 3614 RVA: 0x000021BE File Offset: 0x000003BE
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000E1F RID: 3615 RVA: 0x00042676 File Offset: 0x00040876
	protected internal override string __getTypeName()
	{
		return "NetworkBehaviourSingleton`1";
	}

	// Token: 0x04000890 RID: 2192
	private static T instance;
}
