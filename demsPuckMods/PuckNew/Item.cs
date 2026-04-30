using System;
using System.Linq;
using System.Text.Json.Serialization;

// Token: 0x020000B8 RID: 184
public class Item
{
	// Token: 0x1700008A RID: 138
	// (get) Token: 0x0600059D RID: 1437 RVA: 0x0001EC83 File Offset: 0x0001CE83
	// (set) Token: 0x0600059E RID: 1438 RVA: 0x0001EC8B File Offset: 0x0001CE8B
	public int id { get; set; }

	// Token: 0x1700008B RID: 139
	// (get) Token: 0x0600059F RID: 1439 RVA: 0x0001EC94 File Offset: 0x0001CE94
	// (set) Token: 0x060005A0 RID: 1440 RVA: 0x0001EC9C File Offset: 0x0001CE9C
	public string name { get; set; }

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x060005A1 RID: 1441 RVA: 0x0001ECA5 File Offset: 0x0001CEA5
	// (set) Token: 0x060005A2 RID: 1442 RVA: 0x0001ECAD File Offset: 0x0001CEAD
	public string description { get; set; }

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x060005A3 RID: 1443 RVA: 0x0001ECB6 File Offset: 0x0001CEB6
	// (set) Token: 0x060005A4 RID: 1444 RVA: 0x0001ECBE File Offset: 0x0001CEBE
	public string[] categories { get; set; } = new string[0];

	// Token: 0x1700008E RID: 142
	// (get) Token: 0x060005A5 RID: 1445 RVA: 0x0001ECC7 File Offset: 0x0001CEC7
	// (set) Token: 0x060005A6 RID: 1446 RVA: 0x0001ECCF File Offset: 0x0001CECF
	public int price { get; set; }

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x060005A7 RID: 1447 RVA: 0x0001ECD8 File Offset: 0x0001CED8
	[JsonIgnore]
	public bool IsFlag
	{
		get
		{
			return this.categories.Contains("flag");
		}
	}

	// Token: 0x17000090 RID: 144
	// (get) Token: 0x060005A8 RID: 1448 RVA: 0x0001ECEA File Offset: 0x0001CEEA
	[JsonIgnore]
	public bool IsHeadgear
	{
		get
		{
			return this.categories.Contains("headgear");
		}
	}

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x060005A9 RID: 1449 RVA: 0x0001ECFC File Offset: 0x0001CEFC
	[JsonIgnore]
	public bool IsMustache
	{
		get
		{
			return this.categories.Contains("mustache");
		}
	}

	// Token: 0x17000092 RID: 146
	// (get) Token: 0x060005AA RID: 1450 RVA: 0x0001ED0E File Offset: 0x0001CF0E
	[JsonIgnore]
	public bool IsBeard
	{
		get
		{
			return this.categories.Contains("beard");
		}
	}

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x060005AB RID: 1451 RVA: 0x0001ED20 File Offset: 0x0001CF20
	[JsonIgnore]
	public bool IsJersey
	{
		get
		{
			return this.categories.Contains("jersey");
		}
	}

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x060005AC RID: 1452 RVA: 0x0001ED32 File Offset: 0x0001CF32
	[JsonIgnore]
	public bool IsStickSkin
	{
		get
		{
			return this.categories.Contains("stickSkin");
		}
	}

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x060005AD RID: 1453 RVA: 0x0001ED44 File Offset: 0x0001CF44
	[JsonIgnore]
	public bool IsStickShaftTape
	{
		get
		{
			return this.categories.Contains("stickShaftTape");
		}
	}

	// Token: 0x17000096 RID: 150
	// (get) Token: 0x060005AE RID: 1454 RVA: 0x0001ED56 File Offset: 0x0001CF56
	[JsonIgnore]
	public bool IsStickBladeTape
	{
		get
		{
			return this.categories.Contains("stickBladeTape");
		}
	}

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x060005AF RID: 1455 RVA: 0x0001ED68 File Offset: 0x0001CF68
	[JsonIgnore]
	public bool HasRolePostfix
	{
		get
		{
			return this.categories.Contains("attacker") || this.categories.Contains("goalie");
		}
	}

	// Token: 0x17000098 RID: 152
	// (get) Token: 0x060005B0 RID: 1456 RVA: 0x0001ED8E File Offset: 0x0001CF8E
	[JsonIgnore]
	public bool IsAttackerItem
	{
		get
		{
			return !this.HasRolePostfix || this.categories.Contains("attacker");
		}
	}

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x060005B1 RID: 1457 RVA: 0x0001EDAA File Offset: 0x0001CFAA
	[JsonIgnore]
	public bool IsGoalieItem
	{
		get
		{
			return !this.HasRolePostfix || this.categories.Contains("goalie");
		}
	}

	// Token: 0x1700009A RID: 154
	// (get) Token: 0x060005B2 RID: 1458 RVA: 0x0001EDC6 File Offset: 0x0001CFC6
	[JsonIgnore]
	public bool IsPurchased
	{
		get
		{
			return BackendManager.PlayerState.PlayerData != null && BackendManager.PlayerState.PlayerData.items.Any((PlayerItem item) => item.itemId == this.id);
		}
	}

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x060005B3 RID: 1459 RVA: 0x0001EDF6 File Offset: 0x0001CFF6
	[JsonIgnore]
	public bool IsOwned
	{
		get
		{
			return this.price == 0 || this.IsPurchased;
		}
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x060005B4 RID: 1460 RVA: 0x0001EE08 File Offset: 0x0001D008
	[JsonIgnore]
	public bool IsUnlisted
	{
		get
		{
			return this.categories.Contains("unlisted");
		}
	}

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x060005B5 RID: 1461 RVA: 0x0001EE1A File Offset: 0x0001D01A
	[JsonIgnore]
	public string EditorDisplayName
	{
		get
		{
			return string.Format("{0} ({1})", this.name, this.id);
		}
	}
}
