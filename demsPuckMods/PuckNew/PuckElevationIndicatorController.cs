using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000058 RID: 88
public class PuckElevationIndicatorController : MonoBehaviour
{
	// Token: 0x060002FD RID: 765 RVA: 0x00012AE4 File Offset: 0x00010CE4
	private void Awake()
	{
		this.puckElevationIndicator = base.GetComponent<PuckElevationIndicator>();
	}

	// Token: 0x060002FE RID: 766 RVA: 0x00012AF2 File Offset: 0x00010CF2
	private void Start()
	{
		EventManager.AddEventListener("Event_OnShowPuckElevationChanged", new Action<Dictionary<string, object>>(this.Event_OnShowPuckElevationChanged));
		this.puckElevationIndicator.IsVisible = SettingsManager.ShowPuckElevation;
	}

	// Token: 0x060002FF RID: 767 RVA: 0x00012B1A File Offset: 0x00010D1A
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnShowPuckElevationChanged", new Action<Dictionary<string, object>>(this.Event_OnShowPuckElevationChanged));
	}

	// Token: 0x06000300 RID: 768 RVA: 0x00012B34 File Offset: 0x00010D34
	private void Event_OnShowPuckElevationChanged(Dictionary<string, object> message)
	{
		bool isVisible = (bool)message["value"];
		this.puckElevationIndicator.IsVisible = isVisible;
	}

	// Token: 0x04000211 RID: 529
	private PuckElevationIndicator puckElevationIndicator;
}
