using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

// Token: 0x020000BB RID: 187
public class ThreadManager : MonoBehaviourSingleton<ThreadManager>
{
	// Token: 0x06000599 RID: 1433 RVA: 0x00023204 File Offset: 0x00021404
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

	// Token: 0x0600059A RID: 1434 RVA: 0x00023260 File Offset: 0x00021460
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

	// Token: 0x0600059B RID: 1435 RVA: 0x0000A782 File Offset: 0x00008982
	public void Enqueue(Action action)
	{
		this.Enqueue(this.ActionWrapper(action));
	}

	// Token: 0x0600059C RID: 1436 RVA: 0x000232C8 File Offset: 0x000214C8
	public Task EnqueueAsync(Action action)
	{
		ThreadManager.<>c__DisplayClass4_0 CS$<>8__locals1 = new ThreadManager.<>c__DisplayClass4_0();
		CS$<>8__locals1.action = action;
		CS$<>8__locals1.tcs = new TaskCompletionSource<bool>();
		this.Enqueue(this.ActionWrapper(new Action(CS$<>8__locals1.<EnqueueAsync>g__WrappedAction|0)));
		return CS$<>8__locals1.tcs.Task;
	}

	// Token: 0x0600059D RID: 1437 RVA: 0x0000A791 File Offset: 0x00008991
	private IEnumerator ActionWrapper(Action a)
	{
		a();
		yield return null;
		yield break;
	}

	// Token: 0x0400030A RID: 778
	private Queue<Action> executionQueue = new Queue<Action>();
}
