using System;
using System.Collections.Generic;
using System.Text.Json;
using SocketIOClient;
using UnityEngine;

// Token: 0x0200014B RID: 331
public class InMessage
{
	// Token: 0x060009E1 RID: 2529 RVA: 0x0002FD6C File Offset: 0x0002DF6C
	public InMessage(string messageName, SocketIOResponse response)
	{
		this.MessageName = messageName;
		this.response = response;
	}

	// Token: 0x060009E2 RID: 2530 RVA: 0x0002FD82 File Offset: 0x0002DF82
	public T GetData<T>()
	{
		return this.response.GetValue<T>(0);
	}

	// Token: 0x060009E3 RID: 2531 RVA: 0x0002FD90 File Offset: 0x0002DF90
	public void Respond(Dictionary<string, object> data)
	{
		try
		{
			string text = JsonSerializer.Serialize<Dictionary<string, object>>(data, WebSocketManager.JsonOptions);
			Debug.Log(string.Concat(new string[]
			{
				"[WebsocketManager] WebSocket sending response to message ",
				this.MessageName,
				" (",
				text,
				")"
			}));
			this.response.CallbackAsync(new object[]
			{
				data
			});
		}
		catch (Exception ex)
		{
			Debug.LogError("[WebsocketManager] WebSocket failed to send response to message " + this.MessageName + ": " + ex.Message);
		}
	}

	// Token: 0x060009E4 RID: 2532 RVA: 0x0002FE2C File Offset: 0x0002E02C
	public override string ToString()
	{
		string result;
		try
		{
			result = JsonSerializer.Serialize<object>(this.response.GetValue<object>(0), WebSocketManager.JsonOptions);
		}
		catch
		{
			result = this.response.ToString();
		}
		return result;
	}

	// Token: 0x040005B4 RID: 1460
	public readonly string MessageName;

	// Token: 0x040005B5 RID: 1461
	private SocketIOResponse response;
}
