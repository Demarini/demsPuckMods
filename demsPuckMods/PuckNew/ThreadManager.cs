using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x0200013E RID: 318
public class ThreadManager : MonoBehaviourSingleton<ThreadManager>
{
	// Token: 0x06000979 RID: 2425 RVA: 0x0002DD40 File Offset: 0x0002BF40
	private void Update()
	{
		Queue<Action> obj = this.executionQueue;
		lock (obj)
		{
			while (this.executionQueue.Count > 0)
			{
				this.executionQueue.Dequeue()();
			}
		}
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x0002DD9C File Offset: 0x0002BF9C
	public void Enqueue(IEnumerator action)
	{
		Queue<Action> obj = this.executionQueue;
		lock (obj)
		{
			this.executionQueue.Enqueue(delegate
			{
				this.StartCoroutine(action);
			});
		}
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x0002DE04 File Offset: 0x0002C004
	public void Enqueue(Action action)
	{
		this.Enqueue(this.ActionWrapper(action));
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x0002DE13 File Offset: 0x0002C013
	private IEnumerator ActionWrapper(Action action)
	{
		action();
		yield return null;
		yield break;
	}

	// Token: 0x04000571 RID: 1393
	private Queue<Action> executionQueue = new Queue<Action>();
}
