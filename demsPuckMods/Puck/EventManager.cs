using System;
using System.Collections.Generic;

// Token: 0x02000038 RID: 56
public class EventManager : MonoBehaviourSingleton<EventManager>
{
	// Token: 0x06000189 RID: 393 RVA: 0x00007E7A File Offset: 0x0000607A
	public void AddAnyEventListener(Action<Dictionary<string, object>> listener)
	{
		this.anyListeners.Add(listener);
	}

	// Token: 0x0600018A RID: 394 RVA: 0x00007E88 File Offset: 0x00006088
	public void RemoveAnyEventListener(Action<Dictionary<string, object>> listener)
	{
		this.anyListeners.Remove(listener);
	}

	// Token: 0x0600018B RID: 395 RVA: 0x00012E88 File Offset: 0x00011088
	public void AddEventListener(string eventName, Action<Dictionary<string, object>> listener)
	{
		if (this.events.ContainsKey(eventName))
		{
			Dictionary<string, Action<Dictionary<string, object>>> dictionary = this.events;
			dictionary[eventName] = (Action<Dictionary<string, object>>)Delegate.Combine(dictionary[eventName], listener);
			return;
		}
		Action<Dictionary<string, object>> action = null;
		action = (Action<Dictionary<string, object>>)Delegate.Combine(action, listener);
		this.events.Add(eventName, action);
	}

	// Token: 0x0600018C RID: 396 RVA: 0x00012EE4 File Offset: 0x000110E4
	public void RemoveEventListener(string eventName, Action<Dictionary<string, object>> listener)
	{
		if (this.events.ContainsKey(eventName))
		{
			Dictionary<string, Action<Dictionary<string, object>>> dictionary = this.events;
			dictionary[eventName] = (Action<Dictionary<string, object>>)Delegate.Remove(dictionary[eventName], listener);
		}
	}

	// Token: 0x0600018D RID: 397 RVA: 0x00012F24 File Offset: 0x00011124
	public void TriggerEvent(string eventName, Dictionary<string, object> message = null)
	{
		if (this.events.ContainsKey(eventName))
		{
			if (message == null)
			{
				message = new Dictionary<string, object>
				{
					{
						"eventName",
						eventName
					}
				};
			}
			else
			{
				message.Add("eventName", eventName);
			}
			Action<Dictionary<string, object>> action = this.events[eventName];
			if (action != null)
			{
				action(message);
			}
			foreach (Action<Dictionary<string, object>> action2 in this.anyListeners)
			{
				action2(message);
			}
		}
	}

	// Token: 0x040000D0 RID: 208
	private Dictionary<string, Action<Dictionary<string, object>>> events = new Dictionary<string, Action<Dictionary<string, object>>>();

	// Token: 0x040000D1 RID: 209
	private List<Action<Dictionary<string, object>>> anyListeners = new List<Action<Dictionary<string, object>>>();
}
