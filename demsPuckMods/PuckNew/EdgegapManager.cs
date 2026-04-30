using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;

// Token: 0x02000119 RID: 281
public class EdgegapManager : MonoBehaviour
{
	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x060007B2 RID: 1970 RVA: 0x00025AAF File Offset: 0x00023CAF
	// (set) Token: 0x060007B3 RID: 1971 RVA: 0x00025AB7 File Offset: 0x00023CB7
	public string RequestId { get; private set; }

	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x060007B4 RID: 1972 RVA: 0x00025AC0 File Offset: 0x00023CC0
	// (set) Token: 0x060007B5 RID: 1973 RVA: 0x00025AC8 File Offset: 0x00023CC8
	public string ArbitriumPublicIp { get; private set; }

	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x060007B6 RID: 1974 RVA: 0x00025AD1 File Offset: 0x00023CD1
	// (set) Token: 0x060007B7 RID: 1975 RVA: 0x00025AD9 File Offset: 0x00023CD9
	public ushort ArbitriumPortPuckExternal { get; private set; }

	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x060007B8 RID: 1976 RVA: 0x00025AE2 File Offset: 0x00023CE2
	// (set) Token: 0x060007B9 RID: 1977 RVA: 0x00025AEA File Offset: 0x00023CEA
	public string ArbitriumDeleteUrl { get; private set; }

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x060007BA RID: 1978 RVA: 0x00025AF3 File Offset: 0x00023CF3
	// (set) Token: 0x060007BB RID: 1979 RVA: 0x00025AFB File Offset: 0x00023CFB
	public string ArbitriumDeleteToken { get; private set; }

	// Token: 0x170000DA RID: 218
	// (get) Token: 0x060007BC RID: 1980 RVA: 0x00025B04 File Offset: 0x00023D04
	// (set) Token: 0x060007BD RID: 1981 RVA: 0x00025B0C File Offset: 0x00023D0C
	public bool IsEdgegap { get; private set; }

	// Token: 0x060007BE RID: 1982 RVA: 0x00025B18 File Offset: 0x00023D18
	private void Awake()
	{
		string environmentVariable = Environment.GetEnvironmentVariable("ARBITRIUM_REQUEST_ID");
		string environmentVariable2 = Environment.GetEnvironmentVariable("ARBITRIUM_PUBLIC_IP");
		string environmentVariable3 = Environment.GetEnvironmentVariable("ARBITRIUM_PORT_PUCK_EXTERNAL");
		string environmentVariable4 = Environment.GetEnvironmentVariable("ARBITRIUM_DELETE_URL");
		string environmentVariable5 = Environment.GetEnvironmentVariable("ARBITRIUM_DELETE_TOKEN");
		if (!string.IsNullOrEmpty(environmentVariable) && !string.IsNullOrEmpty(environmentVariable2) && !string.IsNullOrEmpty(environmentVariable3) && !string.IsNullOrEmpty(environmentVariable4) && !string.IsNullOrEmpty(environmentVariable5))
		{
			this.RequestId = environmentVariable;
			this.ArbitriumPublicIp = environmentVariable2;
			this.ArbitriumPortPuckExternal = ushort.Parse(environmentVariable3);
			this.ArbitriumDeleteUrl = environmentVariable4;
			this.ArbitriumDeleteToken = environmentVariable5;
			this.IsEdgegap = true;
			Debug.Log(string.Format("[EdgegapManager] Running in Edgegap (RequestId: {0}, ArbitriumPublicIp: {1}, ArbitriumPortPuckExternal: {2}, ArbitriumDeleteUrl: {3}, ArbitriumDeleteToken: {4})", new object[]
			{
				this.RequestId,
				this.ArbitriumPublicIp,
				this.ArbitriumPortPuckExternal,
				this.ArbitriumDeleteUrl,
				this.ArbitriumDeleteToken
			}));
		}
	}

	// Token: 0x060007BF RID: 1983 RVA: 0x00025C0C File Offset: 0x00023E0C
	public void StartDependencyTimeout(EdgegapDependency dependency)
	{
		if (!this.IsEdgegap)
		{
			return;
		}
		Debug.Log(string.Format("[EdgegapManager] Starting timeout for dependency {0}", dependency));
		if (this.dependencyTweenMap.ContainsKey(dependency))
		{
			Tween tween = this.dependencyTweenMap[dependency];
			if (tween != null)
			{
				tween.Kill(false);
			}
		}
		this.dependencyTweenMap[dependency] = DOVirtual.DelayedCall(Constants.EDGEGAP_DEPENDENCY_TIMEOUTS[dependency], delegate
		{
			this.OnDependencyFailed(dependency);
		}, true);
	}

	// Token: 0x060007C0 RID: 1984 RVA: 0x00025CB4 File Offset: 0x00023EB4
	public void StopDependencyTimeout(EdgegapDependency dependency)
	{
		if (!this.IsEdgegap)
		{
			return;
		}
		Debug.Log(string.Format("[EdgegapManager] Stopping timeout for dependency {0}", dependency));
		if (this.dependencyTweenMap.ContainsKey(dependency))
		{
			Tween tween = this.dependencyTweenMap[dependency];
			if (tween == null)
			{
				return;
			}
			tween.Kill(false);
		}
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x00025D04 File Offset: 0x00023F04
	public void SetDependency(EdgegapDependency dependency, bool value)
	{
		if (!this.IsEdgegap)
		{
			return;
		}
		this.dependencyStatusMap[dependency] = value;
		if (value)
		{
			this.OnDependencyMet(dependency);
			return;
		}
		this.OnDependencyFailed(dependency);
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x00025D2E File Offset: 0x00023F2E
	private void StartDeploymentDeletion(float repeatInterval = 1f)
	{
		if (!this.IsEdgegap)
		{
			return;
		}
		if (this.deploymentDeletionTween != null)
		{
			return;
		}
		this.DeleteDeployment();
		this.deploymentDeletionTween = DOVirtual.DelayedCall(repeatInterval, delegate
		{
			this.DeleteDeployment();
		}, true).SetLoops(-1);
	}

	// Token: 0x060007C3 RID: 1987 RVA: 0x00025D68 File Offset: 0x00023F68
	private void DeleteDeployment()
	{
		EdgegapManager.<DeleteDeployment>d__32 <DeleteDeployment>d__;
		<DeleteDeployment>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<DeleteDeployment>d__.<>4__this = this;
		<DeleteDeployment>d__.<>1__state = -1;
		<DeleteDeployment>d__.<>t__builder.Start<EdgegapManager.<DeleteDeployment>d__32>(ref <DeleteDeployment>d__);
	}

	// Token: 0x060007C4 RID: 1988 RVA: 0x00025D9F File Offset: 0x00023F9F
	private void OnDependencyMet(EdgegapDependency dependency)
	{
		if (!this.IsEdgegap)
		{
			return;
		}
		Debug.Log(string.Format("[EdgegapManager] Dependency {0} met", dependency));
		this.StopDependencyTimeout(dependency);
	}

	// Token: 0x060007C5 RID: 1989 RVA: 0x00025DC6 File Offset: 0x00023FC6
	private void OnDependencyFailed(EdgegapDependency dependency)
	{
		if (!this.IsEdgegap)
		{
			return;
		}
		Debug.Log(string.Format("[EdgegapManager] Dependency {0} failed", dependency));
		this.StopDependencyTimeout(dependency);
		this.StartDeploymentDeletion(1f);
	}

	// Token: 0x040004A1 RID: 1185
	private Dictionary<EdgegapDependency, Tween> dependencyTweenMap = new Dictionary<EdgegapDependency, Tween>();

	// Token: 0x040004A2 RID: 1186
	private Dictionary<EdgegapDependency, bool> dependencyStatusMap = new Dictionary<EdgegapDependency, bool>();

	// Token: 0x040004A3 RID: 1187
	private Tween deploymentDeletionTween;
}
