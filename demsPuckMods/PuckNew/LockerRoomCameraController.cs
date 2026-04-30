using System;
using System.Collections.Generic;

// Token: 0x02000011 RID: 17
public class LockerRoomCameraController : BaseCameraController
{
	// Token: 0x06000053 RID: 83 RVA: 0x00002D88 File Offset: 0x00000F88
	public override void Awake()
	{
		base.Awake();
		this.lockerRoomCamera = base.GetComponent<LockerRoomCamera>();
		EventManager.AddEventListener("Event_OnMainMenuShow", new Action<Dictionary<string, object>>(this.Event_OnMainMenuShow));
		EventManager.AddEventListener("Event_OnPlayerMenuShow", new Action<Dictionary<string, object>>(this.Event_OnPlayerMenuShow));
		EventManager.AddEventListener("Event_OnAppearanceShow", new Action<Dictionary<string, object>>(this.Event_OnAppearanceShow));
		EventManager.AddEventListener("Event_OnAppearanceCategoryChanged", new Action<Dictionary<string, object>>(this.Event_OnAppearanceCategoryChanged));
	}

	// Token: 0x06000054 RID: 84 RVA: 0x00002E00 File Offset: 0x00001000
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnMainMenuShow", new Action<Dictionary<string, object>>(this.Event_OnMainMenuShow));
		EventManager.RemoveEventListener("Event_OnPlayerMenuShow", new Action<Dictionary<string, object>>(this.Event_OnPlayerMenuShow));
		EventManager.RemoveEventListener("Event_OnAppearanceShow", new Action<Dictionary<string, object>>(this.Event_OnAppearanceShow));
		EventManager.RemoveEventListener("Event_OnAppearanceCategoryChanged", new Action<Dictionary<string, object>>(this.Event_OnAppearanceCategoryChanged));
		base.OnDestroy();
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00002E6B File Offset: 0x0000106B
	private void SetAppearancePosition(AppearanceCategory category, AppearanceSubcategory subcategory)
	{
		if (category == AppearanceCategory.Head)
		{
			this.lockerRoomCamera.SetPosition("headCloseUp");
			return;
		}
		if (category != AppearanceCategory.Stick)
		{
			this.lockerRoomCamera.SetPosition("bodyCloseUp");
			return;
		}
		this.lockerRoomCamera.SetPosition("stickCloseUp");
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00002EA8 File Offset: 0x000010A8
	private void Event_OnMainMenuShow(Dictionary<string, object> message)
	{
		this.lockerRoomCamera.SetPosition("default");
	}

	// Token: 0x06000057 RID: 87 RVA: 0x00002EBA File Offset: 0x000010BA
	private void Event_OnPlayerMenuShow(Dictionary<string, object> message)
	{
		this.lockerRoomCamera.SetPosition("bodyCloseUp");
	}

	// Token: 0x06000058 RID: 88 RVA: 0x00002ECC File Offset: 0x000010CC
	private void Event_OnAppearanceShow(Dictionary<string, object> message)
	{
		AppearanceCategory category = (AppearanceCategory)message["category"];
		AppearanceSubcategory subcategory = (AppearanceSubcategory)message["subcategory"];
		this.SetAppearancePosition(category, subcategory);
	}

	// Token: 0x06000059 RID: 89 RVA: 0x00002F04 File Offset: 0x00001104
	private void Event_OnAppearanceCategoryChanged(Dictionary<string, object> message)
	{
		AppearanceCategory category = (AppearanceCategory)message["category"];
		AppearanceSubcategory subcategory = (AppearanceSubcategory)message["subcategory"];
		this.SetAppearancePosition(category, subcategory);
	}

	// Token: 0x0400002A RID: 42
	private LockerRoomCamera lockerRoomCamera;
}
