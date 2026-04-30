using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020001FA RID: 506
public struct SynchronizedObjectsSnapshot : Snapshot
{
	// Token: 0x1700012B RID: 299
	// (get) Token: 0x06000E7B RID: 3707 RVA: 0x00043752 File Offset: 0x00041952
	// (set) Token: 0x06000E7C RID: 3708 RVA: 0x0004375A File Offset: 0x0004195A
	public double remoteTime { readonly get; set; }

	// Token: 0x1700012C RID: 300
	// (get) Token: 0x06000E7D RID: 3709 RVA: 0x00043763 File Offset: 0x00041963
	// (set) Token: 0x06000E7E RID: 3710 RVA: 0x0004376B File Offset: 0x0004196B
	public double localTime { readonly get; set; }

	// Token: 0x1700012D RID: 301
	// (get) Token: 0x06000E7F RID: 3711 RVA: 0x00043774 File Offset: 0x00041974
	// (set) Token: 0x06000E80 RID: 3712 RVA: 0x0004377C File Offset: 0x0004197C
	public List<SynchronizedObjectSnapshot> snapshots { readonly get; set; }

	// Token: 0x06000E81 RID: 3713 RVA: 0x00043785 File Offset: 0x00041985
	public SynchronizedObjectsSnapshot(double remoteTime, double localTime, List<SynchronizedObjectSnapshot> snapshots)
	{
		this.remoteTime = remoteTime;
		this.localTime = localTime;
		this.snapshots = snapshots;
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x0004379C File Offset: 0x0004199C
	public static void Interpolate(SynchronizedObjectsSnapshot from, SynchronizedObjectsSnapshot to, double t)
	{
		using (List<SynchronizedObjectSnapshot>.Enumerator enumerator = to.snapshots.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				SynchronizedObjectSnapshot toSnapshot = enumerator.Current;
				SynchronizedObjectSnapshot synchronizedObjectSnapshot = from.snapshots.FirstOrDefault((SynchronizedObjectSnapshot snapshot) => snapshot.SynchronizedObject == toSnapshot.SynchronizedObject);
				if ((synchronizedObjectSnapshot != null && !(synchronizedObjectSnapshot.SynchronizedObject == null)) || (toSnapshot != null && !(toSnapshot.SynchronizedObject == null)))
				{
					if (synchronizedObjectSnapshot == null || synchronizedObjectSnapshot.SynchronizedObject == null)
					{
						toSnapshot.SynchronizedObject.transform.position = toSnapshot.Position;
						toSnapshot.SynchronizedObject.transform.rotation = toSnapshot.Rotation;
						toSnapshot.SynchronizedObject.PredictedLinearVelocity = toSnapshot.LinearVelocity;
						toSnapshot.SynchronizedObject.PredictedAngularVelocity = toSnapshot.AngularVelocity;
					}
					else if (toSnapshot == null || toSnapshot.SynchronizedObject == null)
					{
						synchronizedObjectSnapshot.SynchronizedObject.transform.position = toSnapshot.Position;
						synchronizedObjectSnapshot.SynchronizedObject.transform.rotation = toSnapshot.Rotation;
						synchronizedObjectSnapshot.SynchronizedObject.PredictedLinearVelocity = toSnapshot.LinearVelocity;
						synchronizedObjectSnapshot.SynchronizedObject.PredictedAngularVelocity = toSnapshot.AngularVelocity;
					}
					else
					{
						toSnapshot.SynchronizedObject.transform.position = Vector3.LerpUnclamped(synchronizedObjectSnapshot.Position, toSnapshot.Position, (float)t);
						toSnapshot.SynchronizedObject.transform.rotation = Quaternion.SlerpUnclamped(synchronizedObjectSnapshot.Rotation, toSnapshot.Rotation, (float)t);
						toSnapshot.SynchronizedObject.PredictedLinearVelocity = Vector3.LerpUnclamped(synchronizedObjectSnapshot.LinearVelocity, toSnapshot.LinearVelocity, (float)t);
						toSnapshot.SynchronizedObject.PredictedAngularVelocity = Vector3.LerpUnclamped(synchronizedObjectSnapshot.AngularVelocity, toSnapshot.AngularVelocity, (float)t);
					}
				}
			}
		}
	}
}
