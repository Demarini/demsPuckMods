using System;
using System.Collections.Generic;
using System.Text.Json;

// Token: 0x0200014A RID: 330
public class OutMessage
{
	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x060009DE RID: 2526 RVA: 0x0002FD0A File Offset: 0x0002DF0A
	public bool IsRequestMessage
	{
		get
		{
			return this.ResponseMessageName != null;
		}
	}

	// Token: 0x060009DF RID: 2527 RVA: 0x0002FD15 File Offset: 0x0002DF15
	public OutMessage(string messageName, Dictionary<string, object> data = null, string responseMessageName = null)
	{
		this.MessageName = messageName;
		this.Data = data;
		this.ResponseMessageName = responseMessageName;
	}

	// Token: 0x060009E0 RID: 2528 RVA: 0x0002FD34 File Offset: 0x0002DF34
	public override string ToString()
	{
		string result;
		try
		{
			result = JsonSerializer.Serialize<Dictionary<string, object>>(this.Data, WebSocketManager.JsonOptions);
		}
		catch
		{
			result = null;
		}
		return result;
	}

	// Token: 0x040005B1 RID: 1457
	public readonly string MessageName;

	// Token: 0x040005B2 RID: 1458
	public readonly Dictionary<string, object> Data;

	// Token: 0x040005B3 RID: 1459
	public readonly string ResponseMessageName;
}
