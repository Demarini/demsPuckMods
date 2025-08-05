using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200001A RID: 26
public class PuckElevationIndicatorController : MonoBehaviour
{
	// Token: 0x060000C1 RID: 193 RVA: 0x000074AB File Offset: 0x000056AB
	public void Awake()
	{
		this.puckElevationIndicator = base.GetComponent<PuckElevationIndicator>();
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x000074B9 File Offset: 0x000056B9
	public void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnShowPuckElevationChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnShowPuckElevationChanged));
		this.puckElevationIndicator.IsVisible = (MonoBehaviourSingleton<SettingsManager>.Instance.ShowPuckElevation > 0);
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x000074EE File Offset: 0x000056EE
	public void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnShowPuckElevationChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnShowPuckElevationChanged));
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x0001121C File Offset: 0x0000F41C
	private void Event_Client_OnShowPuckElevationChanged(Dictionary<string, object> message)
	{
		bool isVisible = (bool)message["value"];
		this.puckElevationIndicator.IsVisible = isVisible;
	}

	// Token: 0x04000055 RID: 85
	private PuckElevationIndicator puckElevationIndicator;
}
