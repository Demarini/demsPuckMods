using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000095 RID: 149
public class EdgegapManager : MonoBehaviour
{
	// Token: 0x1700004F RID: 79
	// (get) Token: 0x060003B5 RID: 949 RVA: 0x00009529 File Offset: 0x00007729
	// (set) Token: 0x060003B6 RID: 950 RVA: 0x00009531 File Offset: 0x00007731
	public string RequestId { get; private set; }

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x060003B7 RID: 951 RVA: 0x0000953A File Offset: 0x0000773A
	// (set) Token: 0x060003B8 RID: 952 RVA: 0x00009542 File Offset: 0x00007742
	public string ArbitriumPublicIp { get; private set; }

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x060003B9 RID: 953 RVA: 0x0000954B File Offset: 0x0000774B
	// (set) Token: 0x060003BA RID: 954 RVA: 0x00009553 File Offset: 0x00007753
	public ushort ArbitriumPortGamePortExternal { get; private set; }

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x060003BB RID: 955 RVA: 0x0000955C File Offset: 0x0000775C
	// (set) Token: 0x060003BC RID: 956 RVA: 0x00009564 File Offset: 0x00007764
	public ushort ArbitriumPortPingPortExternal { get; private set; }

	// Token: 0x17000053 RID: 83
	// (get) Token: 0x060003BD RID: 957 RVA: 0x0000956D File Offset: 0x0000776D
	// (set) Token: 0x060003BE RID: 958 RVA: 0x00009575 File Offset: 0x00007775
	public string ArbitriumDeleteUrl { get; private set; }

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x060003BF RID: 959 RVA: 0x0000957E File Offset: 0x0000777E
	// (set) Token: 0x060003C0 RID: 960 RVA: 0x00009586 File Offset: 0x00007786
	public string ArbitriumDeleteToken { get; private set; }

	// Token: 0x17000055 RID: 85
	// (get) Token: 0x060003C1 RID: 961 RVA: 0x0000958F File Offset: 0x0000778F
	public bool IsEdgegap
	{
		get
		{
			return !string.IsNullOrEmpty(this.RequestId) && !string.IsNullOrEmpty(this.ArbitriumPublicIp) && this.ArbitriumPortGamePortExternal != 0 && this.ArbitriumPortPingPortExternal > 0;
		}
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x0001AE5C File Offset: 0x0001905C
	private void Awake()
	{
		string environmentVariable = Environment.GetEnvironmentVariable("ARBITRIUM_REQUEST_ID");
		string environmentVariable2 = Environment.GetEnvironmentVariable("ARBITRIUM_PUBLIC_IP");
		string environmentVariable3 = Environment.GetEnvironmentVariable("ARBITRIUM_PORT_GAME_PORT_EXTERNAL");
		string environmentVariable4 = Environment.GetEnvironmentVariable("ARBITRIUM_PORT_PING_PORT_EXTERNAL");
		string environmentVariable5 = Environment.GetEnvironmentVariable("ARBITRIUM_DELETE_URL");
		string environmentVariable6 = Environment.GetEnvironmentVariable("ARBITRIUM_DELETE_TOKEN");
		if (!string.IsNullOrEmpty(environmentVariable) && !string.IsNullOrEmpty(environmentVariable2) && !string.IsNullOrEmpty(environmentVariable3) && !string.IsNullOrEmpty(environmentVariable4) && !string.IsNullOrEmpty(environmentVariable5) && !string.IsNullOrEmpty(environmentVariable6))
		{
			this.RequestId = environmentVariable;
			this.ArbitriumPublicIp = environmentVariable2;
			this.ArbitriumPortGamePortExternal = ushort.Parse(environmentVariable3);
			this.ArbitriumPortPingPortExternal = ushort.Parse(environmentVariable4);
			this.ArbitriumDeleteUrl = environmentVariable5;
			this.ArbitriumDeleteToken = environmentVariable6;
		}
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x000095BE File Offset: 0x000077BE
	private void Start()
	{
		if (this.IsEdgegap)
		{
			this.StartDeleteDeploymentCoroutine();
		}
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x0001AF18 File Offset: 0x00019118
	public void StartDeleteDeploymentCoroutine()
	{
		if (!this.IsEdgegap)
		{
			return;
		}
		this.StopDeleteDeploymentCoroutine();
		Debug.Log(string.Format("[EdgegapManager] Starting delete deployment coroutine with delay: {0}", 60));
		this.deleteDeploymentCoroutine = this.IDeleteDeployment(60f);
		base.StartCoroutine(this.deleteDeploymentCoroutine);
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x000095CE File Offset: 0x000077CE
	public void StopDeleteDeploymentCoroutine()
	{
		if (!this.IsEdgegap)
		{
			return;
		}
		Debug.Log("[EdgegapManager] Stopping delete deployment coroutine");
		if (this.deleteDeploymentCoroutine != null)
		{
			base.StopCoroutine(this.deleteDeploymentCoroutine);
		}
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x000095F7 File Offset: 0x000077F7
	private IEnumerator IDeleteDeployment(float delay)
	{
		yield return new WaitForSeconds(delay);
		this.DeleteDeployment();
		yield break;
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x0001AF68 File Offset: 0x00019168
	public void DeleteDeployment()
	{
		EdgegapManager.<DeleteDeployment>d__33 <DeleteDeployment>d__;
		<DeleteDeployment>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<DeleteDeployment>d__.<>4__this = this;
		<DeleteDeployment>d__.<>1__state = -1;
		<DeleteDeployment>d__.<>t__builder.Start<EdgegapManager.<DeleteDeployment>d__33>(ref <DeleteDeployment>d__);
	}

	// Token: 0x0400021E RID: 542
	private const int EDGEGAP_DEPLOYMENT_DELETE_TIMEOUT = 60;

	// Token: 0x04000225 RID: 549
	private IEnumerator deleteDeploymentCoroutine;
}
