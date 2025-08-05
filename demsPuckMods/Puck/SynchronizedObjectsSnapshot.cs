using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200016A RID: 362
public struct SynchronizedObjectsSnapshot : Snapshot
{
	// Token: 0x1700012E RID: 302
	// (get) Token: 0x06000C6E RID: 3182 RVA: 0x0000F183 File Offset: 0x0000D383
	// (set) Token: 0x06000C6F RID: 3183 RVA: 0x0000F18B File Offset: 0x0000D38B
	public double remoteTime { readonly get; set; }

	// Token: 0x1700012F RID: 303
	// (get) Token: 0x06000C70 RID: 3184 RVA: 0x0000F194 File Offset: 0x0000D394
	// (set) Token: 0x06000C71 RID: 3185 RVA: 0x0000F19C File Offset: 0x0000D39C
	public double localTime { readonly get; set; }

	// Token: 0x17000130 RID: 304
	// (get) Token: 0x06000C72 RID: 3186 RVA: 0x0000F1A5 File Offset: 0x0000D3A5
	// (set) Token: 0x06000C73 RID: 3187 RVA: 0x0000F1AD File Offset: 0x0000D3AD
	public List<SynchronizedObjectSnapshot> snapshots { readonly get; set; }

	// Token: 0x06000C74 RID: 3188 RVA: 0x0000F1B6 File Offset: 0x0000D3B6
	public SynchronizedObjectsSnapshot(double remoteTime, double localTime, List<SynchronizedObjectSnapshot> snapshots)
	{
		this.remoteTime = remoteTime;
		this.localTime = localTime;
		this.snapshots = snapshots;
	}

	// Token: 0x06000C75 RID: 3189 RVA: 0x00041F68 File Offset: 0x00040168
	public static void Interpolate(SynchronizedObjectsSnapshot from, SynchronizedObjectsSnapshot to, double t)
	{
		new List<SynchronizedObjectSnapshot>();
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
