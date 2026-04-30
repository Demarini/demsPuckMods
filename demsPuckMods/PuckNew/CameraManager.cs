using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000095 RID: 149
public static class CameraManager
{
	// Token: 0x060004D7 RID: 1239 RVA: 0x0001A935 File Offset: 0x00018B35
	public static void Initialize()
	{
		CameraManagerController.Initialize();
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x0001A93C File Offset: 0x00018B3C
	public static void Dispose()
	{
		CameraManagerController.Dispose();
		CameraManager.DisableAllCameras();
		CameraManager.cameras.Clear();
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x0001A954 File Offset: 0x00018B54
	public static void RegisterCamera(BaseCamera camera)
	{
		if (CameraManager.cameras.Contains(camera))
		{
			return;
		}
		CameraManager.cameras.Add(camera);
		EventManager.TriggerEvent("Event_OnCameraRegistered", new Dictionary<string, object>
		{
			{
				"camera",
				camera
			}
		});
		if (CameraManager.IsActiveCamera(camera))
		{
			CameraManager.EnableCamera(camera);
		}
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x0001A9A4 File Offset: 0x00018BA4
	public static void UnregisterCamera(BaseCamera camera)
	{
		if (CameraManager.cameras.Contains(camera))
		{
			CameraManager.cameras.Remove(camera);
		}
		if (CameraManager.activeCamera == camera)
		{
			CameraManager.activeCameraType = global::CameraType.None;
			CameraManager.activeCameraOwnerClientId = null;
			CameraManager.activeCamera = null;
		}
		EventManager.TriggerEvent("Event_OnCameraUnregistered", new Dictionary<string, object>
		{
			{
				"camera",
				camera
			}
		});
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x0001AA0C File Offset: 0x00018C0C
	public static BaseCamera GetCameraByType(global::CameraType cameraType)
	{
		return CameraManager.cameras.Find((BaseCamera camera) => camera.Type == cameraType);
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x0001AA3C File Offset: 0x00018C3C
	public static BaseCamera GetCameraByOwnerClientId(ulong ownerClientId)
	{
		return CameraManager.cameras.Find((BaseCamera camera) => camera.OwnerClientId == ownerClientId);
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x0001AA6C File Offset: 0x00018C6C
	public static BaseCamera GetActiveCamera()
	{
		return CameraManager.cameras.Find((BaseCamera camera) => CameraManager.IsActiveCamera(camera));
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x0001AA98 File Offset: 0x00018C98
	public static void SetActiveCamera(global::CameraType type, ulong? ownerClientId = null)
	{
		Debug.Log(string.Format("[CameraManager] Setting active camera to type {0}", type));
		CameraManager.activeCameraType = type;
		CameraManager.activeCameraOwnerClientId = ownerClientId;
		BaseCamera baseCamera = CameraManager.GetActiveCamera();
		if (baseCamera != null)
		{
			CameraManager.EnableCamera(baseCamera);
		}
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x0001AADC File Offset: 0x00018CDC
	public static bool IsActiveCamera(BaseCamera camera)
	{
		if (CameraManager.activeCameraType != camera.Type)
		{
			return false;
		}
		if (CameraManager.activeCameraOwnerClientId != null)
		{
			ulong? num = CameraManager.activeCameraOwnerClientId;
			ulong ownerClientId = camera.OwnerClientId;
			return num.GetValueOrDefault() == ownerClientId & num != null;
		}
		return true;
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x0001AB25 File Offset: 0x00018D25
	public static void EnableCamera(BaseCamera camera)
	{
		if (camera.IsEnabled)
		{
			return;
		}
		CameraManager.DisableAllCameras();
		Debug.Log(string.Format("[CameraManager] Enabling camera of type {0}", camera.Type));
		camera.Enable();
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x0001AB56 File Offset: 0x00018D56
	public static void DisableCamera(BaseCamera camera)
	{
		if (!camera.IsEnabled)
		{
			return;
		}
		Debug.Log(string.Format("[CameraManager] Disabling camera of type {0}", camera.Type));
		camera.Disable();
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x0001AB84 File Offset: 0x00018D84
	public static void DisableAllCameras()
	{
		foreach (BaseCamera camera in CameraManager.cameras)
		{
			CameraManager.DisableCamera(camera);
		}
	}

	// Token: 0x040002FD RID: 765
	private static List<BaseCamera> cameras = new List<BaseCamera>();

	// Token: 0x040002FE RID: 766
	private static BaseCamera activeCamera = null;

	// Token: 0x040002FF RID: 767
	private static global::CameraType activeCameraType = global::CameraType.None;

	// Token: 0x04000300 RID: 768
	private static ulong? activeCameraOwnerClientId = null;
}
