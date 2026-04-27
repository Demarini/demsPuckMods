using System;
using System.Collections.Generic;

// Token: 0x0200018D RID: 397
public class UIDebugController : UIViewController<UIDebug>
{
	// Token: 0x06000B50 RID: 2896 RVA: 0x00035B45 File Offset: 0x00033D45
	public override void Awake()
	{
		base.Awake();
		this.uiDebug = base.GetComponent<UIDebug>();
		EventManager.AddEventListener("Event_OnDebugChanged", new Action<Dictionary<string, object>>(this.Event_OnDebugChanged));
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x00035B6F File Offset: 0x00033D6F
	private void Start()
	{
		this.uiDebug.SetBuild(string.Format("PUCK B{0} {1:yyyy-MM-dd HH:mm:ss}", ApplicationManager.Version, DateTime.UtcNow));
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x00035B9A File Offset: 0x00033D9A
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnDebugChanged", new Action<Dictionary<string, object>>(this.Event_OnDebugChanged));
		base.OnDestroy();
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x00035BB8 File Offset: 0x00033DB8
	private void Event_OnDebugChanged(Dictionary<string, object> message)
	{
		if ((bool)message["value"])
		{
			this.uiDebug.Show();
			return;
		}
		this.uiDebug.Hide();
	}

	// Token: 0x040006B1 RID: 1713
	private UIDebug uiDebug;
}
