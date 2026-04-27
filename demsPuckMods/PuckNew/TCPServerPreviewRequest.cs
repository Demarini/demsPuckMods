using System;

// Token: 0x02000202 RID: 514
public class TCPServerPreviewRequest : TCPServerMessage
{
	// Token: 0x06000EC2 RID: 3778 RVA: 0x00044670 File Offset: 0x00042870
	public TCPServerPreviewRequest()
	{
		base.type = TCPServerMessageType.PreviewRequest;
	}
}
