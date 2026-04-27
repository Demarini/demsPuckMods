using System;

// Token: 0x02000203 RID: 515
public class TCPServerPreviewResponse : TCPServerMessage
{
	// Token: 0x17000131 RID: 305
	// (get) Token: 0x06000EC3 RID: 3779 RVA: 0x0004467F File Offset: 0x0004287F
	// (set) Token: 0x06000EC4 RID: 3780 RVA: 0x00044687 File Offset: 0x00042887
	public string name { get; set; }

	// Token: 0x17000132 RID: 306
	// (get) Token: 0x06000EC5 RID: 3781 RVA: 0x00044690 File Offset: 0x00042890
	// (set) Token: 0x06000EC6 RID: 3782 RVA: 0x00044698 File Offset: 0x00042898
	public int players { get; set; }

	// Token: 0x17000133 RID: 307
	// (get) Token: 0x06000EC7 RID: 3783 RVA: 0x000446A1 File Offset: 0x000428A1
	// (set) Token: 0x06000EC8 RID: 3784 RVA: 0x000446A9 File Offset: 0x000428A9
	public int maxPlayers { get; set; }

	// Token: 0x17000134 RID: 308
	// (get) Token: 0x06000EC9 RID: 3785 RVA: 0x000446B2 File Offset: 0x000428B2
	// (set) Token: 0x06000ECA RID: 3786 RVA: 0x000446BA File Offset: 0x000428BA
	public bool isPasswordProtected { get; set; }

	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06000ECB RID: 3787 RVA: 0x000446C3 File Offset: 0x000428C3
	// (set) Token: 0x06000ECC RID: 3788 RVA: 0x000446CB File Offset: 0x000428CB
	public ulong[] clientRequiredModIds { get; set; }

	// Token: 0x06000ECD RID: 3789 RVA: 0x000446D4 File Offset: 0x000428D4
	public TCPServerPreviewResponse()
	{
		base.type = TCPServerMessageType.PreviewResponse;
	}
}
