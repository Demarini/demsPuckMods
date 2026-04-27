using System;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class LockerRoomController : MonoBehaviour
{
	// Token: 0x06000085 RID: 133 RVA: 0x00003924 File Offset: 0x00001B24
	private void Awake()
	{
		this.lockerRoom = base.GetComponent<LockerRoom>();
	}

	// Token: 0x06000086 RID: 134 RVA: 0x000020D3 File Offset: 0x000002D3
	private void OnDestroy()
	{
	}

	// Token: 0x0400003A RID: 58
	private LockerRoom lockerRoom;
}
