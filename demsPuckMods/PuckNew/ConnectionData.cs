using System;

// Token: 0x020000CD RID: 205
public class ConnectionData
{
	// Token: 0x170000AA RID: 170
	// (get) Token: 0x0600062E RID: 1582 RVA: 0x00020753 File Offset: 0x0001E953
	// (set) Token: 0x0600062F RID: 1583 RVA: 0x0002075B File Offset: 0x0001E95B
	public string SteamId { get; set; }

	// Token: 0x170000AB RID: 171
	// (get) Token: 0x06000630 RID: 1584 RVA: 0x00020764 File Offset: 0x0001E964
	// (set) Token: 0x06000631 RID: 1585 RVA: 0x0002076C File Offset: 0x0001E96C
	public string Key { get; set; }

	// Token: 0x170000AC RID: 172
	// (get) Token: 0x06000632 RID: 1586 RVA: 0x00020775 File Offset: 0x0001E975
	// (set) Token: 0x06000633 RID: 1587 RVA: 0x0002077D File Offset: 0x0001E97D
	public string Password { get; set; }

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x06000634 RID: 1588 RVA: 0x00020786 File Offset: 0x0001E986
	// (set) Token: 0x06000635 RID: 1589 RVA: 0x0002078E File Offset: 0x0001E98E
	public ulong[] EnabledModIds { get; set; }

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x06000636 RID: 1590 RVA: 0x00020797 File Offset: 0x0001E997
	// (set) Token: 0x06000637 RID: 1591 RVA: 0x0002079F File Offset: 0x0001E99F
	public PlayerHandedness Handedness { get; set; }

	// Token: 0x170000AF RID: 175
	// (get) Token: 0x06000638 RID: 1592 RVA: 0x000207A8 File Offset: 0x0001E9A8
	// (set) Token: 0x06000639 RID: 1593 RVA: 0x000207B0 File Offset: 0x0001E9B0
	public int FlagID { get; set; }

	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x0600063A RID: 1594 RVA: 0x000207B9 File Offset: 0x0001E9B9
	// (set) Token: 0x0600063B RID: 1595 RVA: 0x000207C1 File Offset: 0x0001E9C1
	public int HeadgearIDBlueAttacker { get; set; }

	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x0600063C RID: 1596 RVA: 0x000207CA File Offset: 0x0001E9CA
	// (set) Token: 0x0600063D RID: 1597 RVA: 0x000207D2 File Offset: 0x0001E9D2
	public int HeadgearIDRedAttacker { get; set; }

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x0600063E RID: 1598 RVA: 0x000207DB File Offset: 0x0001E9DB
	// (set) Token: 0x0600063F RID: 1599 RVA: 0x000207E3 File Offset: 0x0001E9E3
	public int HeadgearIDBlueGoalie { get; set; }

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x06000640 RID: 1600 RVA: 0x000207EC File Offset: 0x0001E9EC
	// (set) Token: 0x06000641 RID: 1601 RVA: 0x000207F4 File Offset: 0x0001E9F4
	public int HeadgearIDRedGoalie { get; set; }

	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x06000642 RID: 1602 RVA: 0x000207FD File Offset: 0x0001E9FD
	// (set) Token: 0x06000643 RID: 1603 RVA: 0x00020805 File Offset: 0x0001EA05
	public int MustacheID { get; set; }

	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x06000644 RID: 1604 RVA: 0x0002080E File Offset: 0x0001EA0E
	// (set) Token: 0x06000645 RID: 1605 RVA: 0x00020816 File Offset: 0x0001EA16
	public int BeardID { get; set; }

	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x06000646 RID: 1606 RVA: 0x0002081F File Offset: 0x0001EA1F
	// (set) Token: 0x06000647 RID: 1607 RVA: 0x00020827 File Offset: 0x0001EA27
	public int JerseyIDBlueAttacker { get; set; }

	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x06000648 RID: 1608 RVA: 0x00020830 File Offset: 0x0001EA30
	// (set) Token: 0x06000649 RID: 1609 RVA: 0x00020838 File Offset: 0x0001EA38
	public int JerseyIDRedAttacker { get; set; }

	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x0600064A RID: 1610 RVA: 0x00020841 File Offset: 0x0001EA41
	// (set) Token: 0x0600064B RID: 1611 RVA: 0x00020849 File Offset: 0x0001EA49
	public int JerseyIDBlueGoalie { get; set; }

	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x0600064C RID: 1612 RVA: 0x00020852 File Offset: 0x0001EA52
	// (set) Token: 0x0600064D RID: 1613 RVA: 0x0002085A File Offset: 0x0001EA5A
	public int JerseyIDRedGoalie { get; set; }

	// Token: 0x170000BA RID: 186
	// (get) Token: 0x0600064E RID: 1614 RVA: 0x00020863 File Offset: 0x0001EA63
	// (set) Token: 0x0600064F RID: 1615 RVA: 0x0002086B File Offset: 0x0001EA6B
	public int StickSkinIDBlueAttacker { get; set; }

	// Token: 0x170000BB RID: 187
	// (get) Token: 0x06000650 RID: 1616 RVA: 0x00020874 File Offset: 0x0001EA74
	// (set) Token: 0x06000651 RID: 1617 RVA: 0x0002087C File Offset: 0x0001EA7C
	public int StickSkinIDRedAttacker { get; set; }

	// Token: 0x170000BC RID: 188
	// (get) Token: 0x06000652 RID: 1618 RVA: 0x00020885 File Offset: 0x0001EA85
	// (set) Token: 0x06000653 RID: 1619 RVA: 0x0002088D File Offset: 0x0001EA8D
	public int StickSkinIDBlueGoalie { get; set; }

	// Token: 0x170000BD RID: 189
	// (get) Token: 0x06000654 RID: 1620 RVA: 0x00020896 File Offset: 0x0001EA96
	// (set) Token: 0x06000655 RID: 1621 RVA: 0x0002089E File Offset: 0x0001EA9E
	public int StickSkinIDRedGoalie { get; set; }

	// Token: 0x170000BE RID: 190
	// (get) Token: 0x06000656 RID: 1622 RVA: 0x000208A7 File Offset: 0x0001EAA7
	// (set) Token: 0x06000657 RID: 1623 RVA: 0x000208AF File Offset: 0x0001EAAF
	public int StickShaftTapeIDBlueAttacker { get; set; }

	// Token: 0x170000BF RID: 191
	// (get) Token: 0x06000658 RID: 1624 RVA: 0x000208B8 File Offset: 0x0001EAB8
	// (set) Token: 0x06000659 RID: 1625 RVA: 0x000208C0 File Offset: 0x0001EAC0
	public int StickShaftTapeIDRedAttacker { get; set; }

	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x0600065A RID: 1626 RVA: 0x000208C9 File Offset: 0x0001EAC9
	// (set) Token: 0x0600065B RID: 1627 RVA: 0x000208D1 File Offset: 0x0001EAD1
	public int StickShaftTapeIDBlueGoalie { get; set; }

	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x0600065C RID: 1628 RVA: 0x000208DA File Offset: 0x0001EADA
	// (set) Token: 0x0600065D RID: 1629 RVA: 0x000208E2 File Offset: 0x0001EAE2
	public int StickShaftTapeIDRedGoalie { get; set; }

	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x0600065E RID: 1630 RVA: 0x000208EB File Offset: 0x0001EAEB
	// (set) Token: 0x0600065F RID: 1631 RVA: 0x000208F3 File Offset: 0x0001EAF3
	public int StickBladeTapeIDBlueAttacker { get; set; }

	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x06000660 RID: 1632 RVA: 0x000208FC File Offset: 0x0001EAFC
	// (set) Token: 0x06000661 RID: 1633 RVA: 0x00020904 File Offset: 0x0001EB04
	public int StickBladeTapeIDRedAttacker { get; set; }

	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x06000662 RID: 1634 RVA: 0x0002090D File Offset: 0x0001EB0D
	// (set) Token: 0x06000663 RID: 1635 RVA: 0x00020915 File Offset: 0x0001EB15
	public int StickBladeTapeIDBlueGoalie { get; set; }

	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x06000664 RID: 1636 RVA: 0x0002091E File Offset: 0x0001EB1E
	// (set) Token: 0x06000665 RID: 1637 RVA: 0x00020926 File Offset: 0x0001EB26
	public int StickBladeTapeIDRedGoalie { get; set; }
}
