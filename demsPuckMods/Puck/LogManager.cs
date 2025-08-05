using System;
using System.IO;
using System.Text;
using UnityEngine;

// Token: 0x0200004A RID: 74
public class LogManager : MonoBehaviourSingleton<LogManager>
{
	// Token: 0x17000030 RID: 48
	// (get) Token: 0x0600021A RID: 538 RVA: 0x0000841F File Offset: 0x0000661F
	public string LogsPath
	{
		get
		{
			return Path.Combine(Path.GetFullPath("."), "Logs");
		}
	}

	// Token: 0x0600021B RID: 539 RVA: 0x00016270 File Offset: 0x00014470
	public override void Awake()
	{
		base.Awake();
		if (!Directory.Exists(this.LogsPath))
		{
			Directory.CreateDirectory(this.LogsPath);
		}
		string path = Path.Combine(this.LogsPath, "Puck.log");
		this.streamWriter = new StreamWriter(path, false, Encoding.UTF8);
		this.streamWriter.AutoFlush = true;
		Application.logMessageReceived += this.OnLogMessageReceived;
	}

	// Token: 0x0600021C RID: 540 RVA: 0x00008435 File Offset: 0x00006635
	private void OnDestroy()
	{
		Application.logMessageReceived -= this.OnLogMessageReceived;
		if (this.streamWriter != null)
		{
			this.streamWriter.Close();
			this.streamWriter = null;
		}
	}

	// Token: 0x0600021D RID: 541 RVA: 0x00008462 File Offset: 0x00006662
	private void OnLogMessageReceived(string message, string stackTrace, LogType type)
	{
		if (this.streamWriter != null)
		{
			this.streamWriter.WriteLine(string.Format("{0:yyyy-MM-dd HH:mm:ss} {1}", DateTime.Now, message));
		}
	}

	// Token: 0x04000146 RID: 326
	private StreamWriter streamWriter;
}
