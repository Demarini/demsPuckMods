using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

// Token: 0x0200007E RID: 126
public static class ApplicationManager
{
	// Token: 0x17000076 RID: 118
	// (get) Token: 0x06000455 RID: 1109 RVA: 0x00017ED9 File Offset: 0x000160D9
	public static bool IsDedicatedGameServer
	{
		get
		{
			return Application.isBatchMode;
		}
	}

	// Token: 0x17000077 RID: 119
	// (get) Token: 0x06000456 RID: 1110 RVA: 0x00017EE0 File Offset: 0x000160E0
	public static ushort Version
	{
		get
		{
			ushort result;
			if (!ushort.TryParse(Application.version, out result))
			{
				return 0;
			}
			return result;
		}
	}

	// Token: 0x17000078 RID: 120
	// (get) Token: 0x06000457 RID: 1111 RVA: 0x00017EFE File Offset: 0x000160FE
	// (set) Token: 0x06000458 RID: 1112 RVA: 0x00017F05 File Offset: 0x00016105
	private static bool IsDisplayChangeInProgress
	{
		get
		{
			return ApplicationManager.isDisplayChangeInProgress;
		}
		set
		{
			if (ApplicationManager.isDisplayChangeInProgress == value)
			{
				return;
			}
			ApplicationManager.isDisplayChangeInProgress = value;
			ApplicationManager.OnIsDisplayChangeInProgressChanged();
		}
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x00017F1B File Offset: 0x0001611B
	public static void Initialize()
	{
		ApplicationManagerController.Initialize();
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x00017F23 File Offset: 0x00016123
	public static void Dispose()
	{
		ApplicationManagerController.Dispose();
		Tween tween = ApplicationManager.mouseVisibilityDebounceTween;
		if (tween == null)
		{
			return;
		}
		tween.Kill(false);
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x00017F3A File Offset: 0x0001613A
	public static void SetFullScreenMode(FullScreenMode mode)
	{
		Debug.Log(string.Format("[ApplicationManager] Setting full screen mode to {0}", mode));
		Screen.fullScreenMode = mode;
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x00017F58 File Offset: 0x00016158
	public static Task SetDisplay(int index)
	{
		ApplicationManager.<SetDisplay>d__12 <SetDisplay>d__;
		<SetDisplay>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SetDisplay>d__.index = index;
		<SetDisplay>d__.<>1__state = -1;
		<SetDisplay>d__.<>t__builder.Start<ApplicationManager.<SetDisplay>d__12>(ref <SetDisplay>d__);
		return <SetDisplay>d__.<>t__builder.Task;
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x00017F9C File Offset: 0x0001619C
	public static void SetResolution(int index)
	{
		Debug.Log(string.Format("[ApplicationManager] Setting resolution to {0}", index));
		if (ApplicationManager.IsDisplayChangeInProgress)
		{
			return;
		}
		List<Resolution> resolutions = Utils.GetResolutions();
		if (index < 0 || index >= resolutions.Count)
		{
			return;
		}
		Resolution resolution = resolutions[index];
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x00018002 File Offset: 0x00016202
	public static void SetVSync(bool isEnabled)
	{
		Debug.Log(string.Format("[ApplicationManager] Setting vSync to {0}", isEnabled));
		QualitySettings.vSyncCount = (isEnabled ? 1 : 0);
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x00018025 File Offset: 0x00016225
	public static void SetTargetFrameRate(int targetFrameRate)
	{
		Debug.Log(string.Format("[ApplicationManager] Setting target frame rate to {0}", targetFrameRate));
		Application.targetFrameRate = targetFrameRate;
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x00018044 File Offset: 0x00016244
	public static void SetQuality(ApplicationQuality quality)
	{
		Debug.Log(string.Format("[ApplicationManager] Setting quality to {0}", quality));
		int vSyncCount = QualitySettings.vSyncCount;
		switch (quality)
		{
		case ApplicationQuality.Low:
			QualitySettings.SetQualityLevel(0, true);
			break;
		case ApplicationQuality.Medium:
			QualitySettings.SetQualityLevel(2, true);
			break;
		case ApplicationQuality.High:
			QualitySettings.SetQualityLevel(4, true);
			break;
		case ApplicationQuality.Ultra:
			QualitySettings.SetQualityLevel(5, true);
			break;
		default:
			QualitySettings.SetQualityLevel(4, true);
			break;
		}
		QualitySettings.vSyncCount = vSyncCount;
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x000180B8 File Offset: 0x000162B8
	public static void SetMouseVisibility(bool isVisible)
	{
		Tween tween = ApplicationManager.mouseVisibilityDebounceTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		ApplicationManager.mouseVisibilityDebounceTween = DOVirtual.DelayedCall(0f, delegate
		{
			Debug.Log(string.Format("[ApplicationManager] Setting mouse visibility to {0}", isVisible));
			if (isVisible)
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				return;
			}
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}, true);
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x000180FF File Offset: 0x000162FF
	private static void OnIsDisplayChangeInProgressChanged()
	{
		Debug.Log(string.Format("[ApplicationManager] Display change in progress: {0}", ApplicationManager.isDisplayChangeInProgress));
		EventManager.TriggerEvent("Event_OnIsDisplayChangeInProgressChanged", new Dictionary<string, object>
		{
			{
				"isDisplayChangeInProgress",
				ApplicationManager.isDisplayChangeInProgress
			}
		});
	}

	// Token: 0x040002B3 RID: 691
	private static bool isDisplayChangeInProgress;

	// Token: 0x040002B4 RID: 692
	private static Tween mouseVisibilityDebounceTween;
}
