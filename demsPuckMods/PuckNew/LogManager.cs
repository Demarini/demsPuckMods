using System;
using System.IO;
using System.Text;
using UnityEngine;

// Token: 0x020000BE RID: 190
public static class LogManager
{
	// Token: 0x060005CB RID: 1483 RVA: 0x0001F068 File Offset: 0x0001D268
	public static void Initialize()
	{
		Debug.unityLogger.logHandler = new LogManager.LogHandler(Debug.unityLogger.logHandler);
		if (!Directory.Exists(LogManager.logDirectoryPath))
		{
			Directory.CreateDirectory(LogManager.logDirectoryPath);
		}
		try
		{
			LogManager.streamWriter = new StreamWriter(LogManager.logFilePath, false, Encoding.UTF8);
			LogManager.streamWriter.AutoFlush = true;
		}
		catch (Exception ex)
		{
			Debug.LogError("[LogManager] Failed to initialize StreamWriter: " + ex.Message);
			LogManager.streamWriter = null;
		}
		Application.logMessageReceivedThreaded += LogManager.OnLogMessageReceivedThreaded;
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x0001F108 File Offset: 0x0001D308
	public static void Dispose()
	{
		Application.logMessageReceivedThreaded -= LogManager.OnLogMessageReceivedThreaded;
		object obj = LogManager.streamWriterLock;
		lock (obj)
		{
			if (LogManager.streamWriter != null)
			{
				LogManager.streamWriter.Flush();
				LogManager.streamWriter.Close();
				LogManager.streamWriter = null;
			}
		}
	}

	// Token: 0x060005CD RID: 1485 RVA: 0x0001F178 File Offset: 0x0001D378
	private static void OnLogMessageReceivedThreaded(string message, string stackTrace, LogType type)
	{
		object obj = LogManager.streamWriterLock;
		lock (obj)
		{
			if (LogManager.streamWriter != null)
			{
				LogManager.streamWriter.WriteLine(message ?? "");
			}
		}
	}

	// Token: 0x04000399 RID: 921
	private static string logFileRawPath = Utils.GetCommandLineArgument("--logPath", null) ?? "./Logs/Puck.log";

	// Token: 0x0400039A RID: 922
	private static string logFilePath = Path.GetFullPath(LogManager.logFileRawPath);

	// Token: 0x0400039B RID: 923
	private static string logDirectoryPath = Path.GetDirectoryName(LogManager.logFilePath);

	// Token: 0x0400039C RID: 924
	private static StreamWriter streamWriter;

	// Token: 0x0400039D RID: 925
	private static readonly object streamWriterLock = new object();

	// Token: 0x020000BF RID: 191
	private class LogHandler : ILogHandler
	{
		// Token: 0x060005CF RID: 1487 RVA: 0x0001F21E File Offset: 0x0001D41E
		public LogHandler(ILogHandler blh)
		{
			this.baseLogHandler = blh;
		}

		// Token: 0x060005D0 RID: 1488 RVA: 0x0001F230 File Offset: 0x0001D430
		public void LogFormat(LogType type, Object context, string format, params object[] args)
		{
			string arg;
			switch (type)
			{
			case LogType.Error:
				arg = "ERROR";
				break;
			case LogType.Assert:
				arg = "ASSERT";
				break;
			case LogType.Warning:
				arg = "WARNING";
				break;
			case LogType.Log:
				arg = "INFO";
				break;
			case LogType.Exception:
				arg = "EXCEPTION";
				break;
			default:
				arg = "UNKNOWN";
				break;
			}
			this.baseLogHandler.LogFormat(type, context, string.Format("[{0:yyyy-MM-dd HH:mm:ss}] [{1}] {2}", DateTime.UtcNow, arg, format), args);
		}

		// Token: 0x060005D1 RID: 1489 RVA: 0x0001F2AC File Offset: 0x0001D4AC
		public void LogException(Exception exception, Object context)
		{
			this.baseLogHandler.LogException(exception, context);
		}

		// Token: 0x0400039E RID: 926
		private ILogHandler baseLogHandler;
	}
}
