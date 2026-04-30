using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// Token: 0x02000234 RID: 564
public static class Utils
{
	// Token: 0x06000FA9 RID: 4009 RVA: 0x00044F30 File Offset: 0x00043130
	public static float WrapEulerAngle(float angle)
	{
		angle %= 360f;
		if (angle > 180f)
		{
			angle -= 360f;
		}
		if (angle < -180f)
		{
			angle += 360f;
		}
		return angle;
	}

	// Token: 0x06000FAA RID: 4010 RVA: 0x00044F60 File Offset: 0x00043160
	public static Vector3 WrapEulerAngles(Vector3 eulerAngles)
	{
		eulerAngles.x %= 360f;
		if (eulerAngles.x > 180f)
		{
			eulerAngles.x -= 360f;
		}
		if (eulerAngles.x < -180f)
		{
			eulerAngles.x += 360f;
		}
		eulerAngles.y %= 360f;
		if (eulerAngles.y > 180f)
		{
			eulerAngles.y -= 360f;
		}
		if (eulerAngles.y < -180f)
		{
			eulerAngles.y += 360f;
		}
		eulerAngles.z %= 360f;
		if (eulerAngles.z > 180f)
		{
			eulerAngles.z -= 360f;
		}
		if (eulerAngles.z < -180f)
		{
			eulerAngles.z += 360f;
		}
		return eulerAngles;
	}

	// Token: 0x06000FAB RID: 4011 RVA: 0x0004504C File Offset: 0x0004324C
	public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
	{
		Vector3 vector = point - pivot;
		vector = Quaternion.Euler(angles) * vector;
		point = vector + pivot;
		return point;
	}

	// Token: 0x06000FAC RID: 4012 RVA: 0x00045078 File Offset: 0x00043278
	public static Vector3 Vector2Clamp(Vector2 value, Vector2 min, Vector2 max)
	{
		return new Vector3(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y));
	}

	// Token: 0x06000FAD RID: 4013 RVA: 0x000450B0 File Offset: 0x000432B0
	public static Vector3 Vector3Clamp(Vector3 value, Vector3 min, Vector3 max)
	{
		return new Vector3(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y), Mathf.Clamp(value.z, min.z, max.z));
	}

	// Token: 0x06000FAE RID: 4014 RVA: 0x00045107 File Offset: 0x00043307
	public static Vector3 Vector3Abs(Vector3 value)
	{
		return new Vector3(Mathf.Abs(value.x), Mathf.Abs(value.y), Mathf.Abs(value.z));
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x0004512F File Offset: 0x0004332F
	public static Vector3 Vector3Slerp3(Vector3 a, Vector3 b, Vector3 c, float t)
	{
		if (t <= 0f)
		{
			return Vector3.Slerp(a, b, t + 1f);
		}
		return Vector3.Slerp(b, c, t);
	}

	// Token: 0x06000FB0 RID: 4016 RVA: 0x00045150 File Offset: 0x00043350
	public static float Map(float value, float from1, float to1, float from2, float to2)
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	// Token: 0x06000FB1 RID: 4017 RVA: 0x00045160 File Offset: 0x00043360
	public static Quaternion GetLocalLookRotation(Transform transform, Vector3 target)
	{
		if (transform.parent == null)
		{
			return Quaternion.LookRotation(target - transform.position);
		}
		Quaternion rhs = Quaternion.LookRotation(target - transform.position);
		return Quaternion.Inverse(transform.parent.rotation) * rhs;
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x000451B5 File Offset: 0x000433B5
	public static float GameUnitsToMetric(float value)
	{
		return value * 3.6f;
	}

	// Token: 0x06000FB3 RID: 4019 RVA: 0x000451BE File Offset: 0x000433BE
	public static float GameUnitsToImperial(float value)
	{
		return value * 2.2369363f;
	}

	// Token: 0x06000FB4 RID: 4020 RVA: 0x000451C8 File Offset: 0x000433C8
	public static float GetCollisionForce(Collision collision)
	{
		if (collision == null)
		{
			return 0f;
		}
		float result = 0f;
		if (collision.contacts.Length != 0)
		{
			result = Vector3.Dot(collision.contacts[0].normal, collision.relativeVelocity.normalized) * collision.relativeVelocity.magnitude;
		}
		return result;
	}

	// Token: 0x06000FB5 RID: 4021 RVA: 0x00045224 File Offset: 0x00043424
	public static void SetRigidbodyCollisionDetectionMode(Rigidbody rigidbody, CollisionDetectionMode mode)
	{
		if (rigidbody == null)
		{
			return;
		}
		if (rigidbody.collisionDetectionMode == mode)
		{
			return;
		}
		bool isKinematic = rigidbody.isKinematic;
		Vector3 linearVelocity = rigidbody.linearVelocity;
		Vector3 angularVelocity = rigidbody.angularVelocity;
		rigidbody.collisionDetectionMode = mode;
		rigidbody.isKinematic = true;
		rigidbody.isKinematic = false;
		rigidbody.isKinematic = isKinematic;
		rigidbody.linearVelocity = linearVelocity;
		rigidbody.angularVelocity = angularVelocity;
	}

	// Token: 0x06000FB6 RID: 4022 RVA: 0x00045284 File Offset: 0x00043484
	public static List<string> GetTeamNames()
	{
		return new List<string>
		{
			"BLUE",
			"RED"
		};
	}

	// Token: 0x06000FB7 RID: 4023 RVA: 0x000452A4 File Offset: 0x000434A4
	public static PlayerTeam GetTeamFromName(string name)
	{
		PlayerTeam result;
		if (!(name == "BLUE"))
		{
			if (!(name == "RED"))
			{
				result = PlayerTeam.Blue;
			}
			else
			{
				result = PlayerTeam.Red;
			}
		}
		else
		{
			result = PlayerTeam.Blue;
		}
		return result;
	}

	// Token: 0x06000FB8 RID: 4024 RVA: 0x000452D8 File Offset: 0x000434D8
	public static string GetNameFromTeam(PlayerTeam team)
	{
		string result;
		if (team != PlayerTeam.Blue)
		{
			if (team != PlayerTeam.Red)
			{
				result = "UNKNOWN";
			}
			else
			{
				result = "RED";
			}
		}
		else
		{
			result = "BLUE";
		}
		return result;
	}

	// Token: 0x06000FB9 RID: 4025 RVA: 0x00045306 File Offset: 0x00043506
	public static List<string> GetRoleNames()
	{
		return new List<string>
		{
			"SKATER",
			"GOALIE"
		};
	}

	// Token: 0x06000FBA RID: 4026 RVA: 0x00045324 File Offset: 0x00043524
	public static PlayerRole GetRoleFromName(string name)
	{
		PlayerRole result;
		if (!(name == "SKATER"))
		{
			if (!(name == "GOALIE"))
			{
				result = PlayerRole.Attacker;
			}
			else
			{
				result = PlayerRole.Goalie;
			}
		}
		else
		{
			result = PlayerRole.Attacker;
		}
		return result;
	}

	// Token: 0x06000FBB RID: 4027 RVA: 0x00045358 File Offset: 0x00043558
	public static string GetNameFromRole(PlayerRole role)
	{
		string result;
		if (role != PlayerRole.Attacker)
		{
			if (role != PlayerRole.Goalie)
			{
				result = "UNKNOWN";
			}
			else
			{
				result = "GOALIE";
			}
		}
		else
		{
			result = "SKATER";
		}
		return result;
	}

	// Token: 0x06000FBC RID: 4028 RVA: 0x00045386 File Offset: 0x00043586
	public static List<string> GetHandednessNames()
	{
		return new List<string>
		{
			"LEFT",
			"RIGHT"
		};
	}

	// Token: 0x06000FBD RID: 4029 RVA: 0x000453A4 File Offset: 0x000435A4
	public static PlayerHandedness GetHandednessFromName(string name)
	{
		PlayerHandedness result;
		if (!(name == "LEFT"))
		{
			if (!(name == "RIGHT"))
			{
				result = PlayerHandedness.Right;
			}
			else
			{
				result = PlayerHandedness.Right;
			}
		}
		else
		{
			result = PlayerHandedness.Left;
		}
		return result;
	}

	// Token: 0x06000FBE RID: 4030 RVA: 0x000453D8 File Offset: 0x000435D8
	public static string GetNameFromHandedness(PlayerHandedness handedness)
	{
		string result;
		if (handedness != PlayerHandedness.Left)
		{
			if (handedness != PlayerHandedness.Right)
			{
				result = "UNKNOWN";
			}
			else
			{
				result = "RIGHT";
			}
		}
		else
		{
			result = "LEFT";
		}
		return result;
	}

	// Token: 0x06000FBF RID: 4031 RVA: 0x00045406 File Offset: 0x00043606
	public static List<string> GetUnitsNames()
	{
		return new List<string>
		{
			"METRIC",
			"IMPERIAL"
		};
	}

	// Token: 0x06000FC0 RID: 4032 RVA: 0x00045424 File Offset: 0x00043624
	public static Units GetUnitsFromName(string name)
	{
		Units result;
		if (!(name == "METRIC"))
		{
			if (!(name == "IMPERIAL"))
			{
				result = Units.Metric;
			}
			else
			{
				result = Units.Imperial;
			}
		}
		else
		{
			result = Units.Metric;
		}
		return result;
	}

	// Token: 0x06000FC1 RID: 4033 RVA: 0x00045458 File Offset: 0x00043658
	public static string GetNameFromUnits(Units units)
	{
		string result;
		if (units != Units.Metric)
		{
			if (units != Units.Imperial)
			{
				result = "UNKNOWN";
			}
			else
			{
				result = "IMPERIAL";
			}
		}
		else
		{
			result = "METRIC";
		}
		return result;
	}

	// Token: 0x06000FC2 RID: 4034 RVA: 0x00045485 File Offset: 0x00043685
	public static List<string> GetFullScreenModeNames()
	{
		return new List<string>
		{
			"FULLSCREEN",
			"BORDERLESS",
			"WINDOWED"
		};
	}

	// Token: 0x06000FC3 RID: 4035 RVA: 0x000454B0 File Offset: 0x000436B0
	public static FullScreenMode GetFullScreenModeFromName(string name)
	{
		FullScreenMode result;
		if (!(name == "FULLSCREEN"))
		{
			if (!(name == "BORDERLESS"))
			{
				if (!(name == "WINDOWED"))
				{
					result = FullScreenMode.FullScreenWindow;
				}
				else
				{
					result = FullScreenMode.Windowed;
				}
			}
			else
			{
				result = FullScreenMode.FullScreenWindow;
			}
		}
		else
		{
			result = FullScreenMode.ExclusiveFullScreen;
		}
		return result;
	}

	// Token: 0x06000FC4 RID: 4036 RVA: 0x000454F8 File Offset: 0x000436F8
	public static string GetNameFromFullScreenMode(FullScreenMode mode)
	{
		switch (mode)
		{
		case FullScreenMode.ExclusiveFullScreen:
			return "FULLSCREEN";
		case FullScreenMode.FullScreenWindow:
			return "BORDERLESS";
		case FullScreenMode.Windowed:
			return "WINDOWED";
		}
		return "UNKNOWN";
	}

	// Token: 0x06000FC5 RID: 4037 RVA: 0x0004553C File Offset: 0x0004373C
	public static List<DisplayInfo> GetDisplayLayout()
	{
		List<DisplayInfo> list = new List<DisplayInfo>();
		Screen.GetDisplayLayout(list);
		return list;
	}

	// Token: 0x06000FC6 RID: 4038 RVA: 0x00045549 File Offset: 0x00043749
	public static List<string> GetDisplayNames()
	{
		return Utils.GetDisplayLayout().Select((DisplayInfo displayInfo, int index) => Utils.FormatDisplay(index, displayInfo)).ToList<string>();
	}

	// Token: 0x06000FC7 RID: 4039 RVA: 0x0004557C File Offset: 0x0004377C
	public static string GetDisplayNameFromIndex(int index)
	{
		List<DisplayInfo> displayLayout = Utils.GetDisplayLayout();
		if (index < 0 || index > displayLayout.Count - 1)
		{
			return "UNKNOWN";
		}
		return Utils.FormatDisplay(index, displayLayout[index]);
	}

	// Token: 0x06000FC8 RID: 4040 RVA: 0x000455B4 File Offset: 0x000437B4
	public static int GetDisplayIndexFromName(string name)
	{
		List<DisplayInfo> displayLayout = Utils.GetDisplayLayout();
		for (int i = 0; i < displayLayout.Count; i++)
		{
			if (Utils.FormatDisplay(i, displayLayout[i]) == name)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000FC9 RID: 4041 RVA: 0x000455F0 File Offset: 0x000437F0
	public static string FormatDisplay(int index, DisplayInfo displayInfo)
	{
		return string.Format("{0} ({1})", displayInfo.name, index);
	}

	// Token: 0x06000FCA RID: 4042 RVA: 0x00045608 File Offset: 0x00043808
	public static List<Resolution> GetResolutions()
	{
		return Screen.resolutions.ToList<Resolution>();
	}

	// Token: 0x06000FCB RID: 4043 RVA: 0x00045614 File Offset: 0x00043814
	public static List<string> GetResolutionNames()
	{
		return (from resolution in Utils.GetResolutions()
		select Utils.FormatResolution(resolution)).ToList<string>();
	}

	// Token: 0x06000FCC RID: 4044 RVA: 0x00045644 File Offset: 0x00043844
	public static string GetResolutionNameFromIndex(int index)
	{
		List<Resolution> resolutions = Utils.GetResolutions();
		if (index < 0 || index > resolutions.Count - 1)
		{
			return "UNKNOWN";
		}
		return Utils.FormatResolution(resolutions[index]);
	}

	// Token: 0x06000FCD RID: 4045 RVA: 0x00045678 File Offset: 0x00043878
	public static int GetResolutionIndexFromName(string name)
	{
		List<Resolution> resolutions = Utils.GetResolutions();
		for (int i = 0; i < resolutions.Count; i++)
		{
			if (Utils.FormatResolution(resolutions[i]) == name)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x000456B4 File Offset: 0x000438B4
	public static string FormatResolution(Resolution resolution)
	{
		return string.Format("{0}x{1} @ {2}Hz", resolution.width, resolution.height, resolution.refreshRateRatio.value.ToString("F0"));
	}

	// Token: 0x06000FCF RID: 4047 RVA: 0x000456FF File Offset: 0x000438FF
	public static List<string> GetApplicationQualityNames()
	{
		return new List<string>
		{
			"LOW",
			"MEDIUM",
			"HIGH",
			"ULTRA"
		};
	}

	// Token: 0x06000FD0 RID: 4048 RVA: 0x00045734 File Offset: 0x00043934
	public static ApplicationQuality GetApplicationQualityFromName(string name)
	{
		ApplicationQuality result;
		if (!(name == "LOW"))
		{
			if (!(name == "MEDIUM"))
			{
				if (!(name == "HIGH"))
				{
					if (!(name == "ULTRA"))
					{
						result = ApplicationQuality.High;
					}
					else
					{
						result = ApplicationQuality.Ultra;
					}
				}
				else
				{
					result = ApplicationQuality.High;
				}
			}
			else
			{
				result = ApplicationQuality.Medium;
			}
		}
		else
		{
			result = ApplicationQuality.Low;
		}
		return result;
	}

	// Token: 0x06000FD1 RID: 4049 RVA: 0x0004578C File Offset: 0x0004398C
	public static string GetNameFromApplicationQuality(ApplicationQuality quality)
	{
		string result;
		switch (quality)
		{
		case ApplicationQuality.Low:
			result = "LOW";
			break;
		case ApplicationQuality.Medium:
			result = "MEDIUM";
			break;
		case ApplicationQuality.High:
			result = "HIGH";
			break;
		case ApplicationQuality.Ultra:
			result = "ULTRA";
			break;
		default:
			result = "UNKNOWN";
			break;
		}
		return result;
	}

	// Token: 0x06000FD2 RID: 4050 RVA: 0x000457D8 File Offset: 0x000439D8
	public static KeyBindInteraction GetKeyBindInteractionFromInteraction(string interaction, KeyBindInteractionType interactionType)
	{
		if (interaction == "Press(behavior=1)")
		{
			return KeyBindInteraction.Release;
		}
		if (interaction == "DoublePress")
		{
			return KeyBindInteraction.DoublePress;
		}
		if (interaction == "Hold")
		{
			return KeyBindInteraction.Hold;
		}
		if (interaction == "Toggle")
		{
			return KeyBindInteraction.Toggle;
		}
		if (interactionType != KeyBindInteractionType.Press)
		{
			return KeyBindInteraction.Continuous;
		}
		return KeyBindInteraction.Press;
	}

	// Token: 0x06000FD3 RID: 4051 RVA: 0x00045829 File Offset: 0x00043A29
	public static string GetInteractionFromKeyBindInteraction(KeyBindInteraction keyBindInteraction)
	{
		switch (keyBindInteraction)
		{
		case KeyBindInteraction.Release:
			return "Press(behavior=1)";
		case KeyBindInteraction.DoublePress:
			return "DoublePress";
		case KeyBindInteraction.Hold:
			return "Hold";
		case KeyBindInteraction.Toggle:
			return "Toggle";
		}
		return string.Empty;
	}

	// Token: 0x06000FD4 RID: 4052 RVA: 0x00045868 File Offset: 0x00043A68
	public static string GetHumanizedGamePhase(GamePhase phase, int period, bool isOvertime)
	{
		string result;
		switch (phase)
		{
		case GamePhase.None:
			result = "";
			break;
		case GamePhase.Warmup:
			result = "WARMUP";
			break;
		case GamePhase.PreGame:
			result = "PRE-GAME";
			break;
		case GamePhase.FaceOff:
			result = "FACE-OFF";
			break;
		case GamePhase.Play:
			result = (isOvertime ? "OVERTIME" : string.Format("PERIOD {0}", period));
			break;
		case GamePhase.BlueScore:
			result = "SCORE!";
			break;
		case GamePhase.RedScore:
			result = "SCORE!";
			break;
		case GamePhase.Replay:
			result = "REPLAY";
			break;
		case GamePhase.Intermission:
			result = "INTERMISSION";
			break;
		case GamePhase.GameOver:
			result = "GAME OVER";
			break;
		case GamePhase.PostGame:
			result = "POST-GAME";
			break;
		default:
			result = phase.ToString();
			break;
		}
		return result;
	}

	// Token: 0x06000FD5 RID: 4053 RVA: 0x00045928 File Offset: 0x00043B28
	public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(sourceDir);
		if (!directoryInfo.Exists)
		{
			throw new DirectoryNotFoundException("Source directory not found: " + directoryInfo.FullName);
		}
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		Directory.CreateDirectory(destinationDir);
		foreach (FileInfo fileInfo in directoryInfo.GetFiles())
		{
			string destFileName = Path.Combine(destinationDir, fileInfo.Name);
			fileInfo.CopyTo(destFileName, true);
		}
		if (recursive)
		{
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				string destinationDir2 = Path.Combine(destinationDir, directoryInfo2.Name);
				Utils.CopyDirectory(directoryInfo2.FullName, destinationDir2, true);
			}
		}
	}

	// Token: 0x06000FD6 RID: 4054 RVA: 0x000459D8 File Offset: 0x00043BD8
	public static string GetConnectionRejectionMessage(ConnectionRejectionCode code, string message = null)
	{
		if (!string.IsNullOrEmpty(message))
		{
			return message;
		}
		switch (code)
		{
		default:
			return "Server unreachable";
		case ConnectionRejectionCode.ServerFull:
			return "Server full";
		case ConnectionRejectionCode.TimedOut:
			return "Timed out";
		case ConnectionRejectionCode.Banned:
			return "Banned";
		case ConnectionRejectionCode.NotWhitelisted:
			return "Not whitelisted";
		case ConnectionRejectionCode.MissingPassword:
			return "Missing password";
		case ConnectionRejectionCode.InvalidPassword:
			return "Invalid password";
		case ConnectionRejectionCode.MissingMods:
			return "Missing mods";
		}
	}

	// Token: 0x06000FD7 RID: 4055 RVA: 0x00045A44 File Offset: 0x00043C44
	public static string GetDisconnectionMessage(DisconnectionCode code, string message = null)
	{
		if (!string.IsNullOrEmpty(message))
		{
			return message;
		}
		switch (code)
		{
		default:
			return "Connection lost";
		case DisconnectionCode.Disconnected:
			return "Disconnected";
		case DisconnectionCode.Kicked:
			return "Kicked";
		case DisconnectionCode.Banned:
			return "Banned";
		}
	}

	// Token: 0x06000FD8 RID: 4056 RVA: 0x00045A80 File Offset: 0x00043C80
	public static string GetCommandLineArgument(string name, string[] args = null)
	{
		if (args == null)
		{
			args = Environment.GetCommandLineArgs();
		}
		int i = 0;
		while (i < args.Length)
		{
			if (args[i] == (name ?? ""))
			{
				if (i + 1 >= args.Length)
				{
					return null;
				}
				return args[i + 1];
			}
			else
			{
				i++;
			}
		}
		return null;
	}

	// Token: 0x06000FD9 RID: 4057 RVA: 0x00045ACC File Offset: 0x00043CCC
	public static void PrintUPnPLogs()
	{
		if (uPnPHelper.DebugMode)
		{
			List<string> debugMessageArray = uPnPHelper.GetDebugMessageArray();
			foreach (string str in debugMessageArray.ToList<string>())
			{
				Debug.Log("[uPnPHelper] " + str);
			}
			debugMessageArray.Clear();
		}
		List<string> errorMessageArray = uPnPHelper.GetErrorMessageArray();
		foreach (string str2 in errorMessageArray.ToList<string>())
		{
			Debug.LogError("[uPnPHelper] " + str2);
		}
		errorMessageArray.Clear();
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x00045B94 File Offset: 0x00043D94
	public static double GetTimestamp()
	{
		return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
	}
}
