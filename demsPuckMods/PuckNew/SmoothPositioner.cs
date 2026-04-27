using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

// Token: 0x02000064 RID: 100
public class SmoothPositioner : MonoBehaviour
{
	// Token: 0x06000357 RID: 855 RVA: 0x00013B5B File Offset: 0x00011D5B
	private void Start()
	{
		this.SetPosition(this.initialPosition, true);
	}

	// Token: 0x06000358 RID: 856 RVA: 0x00013B6A File Offset: 0x00011D6A
	private void OnDestroy()
	{
		Tween tween = this.positionTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		Tween tween2 = this.rotationTween;
		if (tween2 == null)
		{
			return;
		}
		tween2.Kill(false);
	}

	// Token: 0x06000359 RID: 857 RVA: 0x00013B90 File Offset: 0x00011D90
	public void SetPosition(string positionName, bool instant = false)
	{
		if (!this.positions.ContainsKey(positionName))
		{
			Debug.LogError("[SmoothPositioner] Target position " + positionName + " does not exist");
			return;
		}
		if (instant)
		{
			Tween tween = this.positionTween;
			if (tween != null)
			{
				tween.Kill(false);
			}
			Tween tween2 = this.rotationTween;
			if (tween2 != null)
			{
				tween2.Kill(false);
			}
			base.transform.position = this.positions[positionName].position;
			base.transform.rotation = this.positions[positionName].rotation;
		}
		else
		{
			if (this.currentPosition == positionName)
			{
				return;
			}
			Tween tween3 = this.positionTween;
			if (tween3 != null)
			{
				tween3.Kill(false);
			}
			Tween tween4 = this.rotationTween;
			if (tween4 != null)
			{
				tween4.Kill(false);
			}
			this.positionTween = base.transform.DOMove(this.positions[positionName].position, this.transitionDuration, false).SetEase(this.transitionEase);
			this.rotationTween = base.transform.DORotateQuaternion(this.positions[positionName].rotation, this.transitionDuration).SetEase(this.transitionEase);
		}
		this.currentPosition = positionName;
	}

	// Token: 0x0600035A RID: 858 RVA: 0x00013CC8 File Offset: 0x00011EC8
	private void OnDrawGizmos()
	{
		if (!Application.isEditor)
		{
			return;
		}
		foreach (KeyValuePair<string, Transform> keyValuePair in this.positions)
		{
			string key = keyValuePair.Key;
			Transform value = keyValuePair.Value;
			Gizmos.color = Color.black;
			Gizmos.DrawSphere(value.position, 0.05f);
			Gizmos.matrix = value.localToWorldMatrix;
			Gizmos.DrawFrustum(Vector3.zero, 60f, 1f, 0f, 1f);
			Gizmos.matrix = Matrix4x4.identity;
			Gizmos.color = Color.green;
			Gizmos.DrawLine(value.position, value.position + value.forward * 1f);
		}
	}

	// Token: 0x0400024C RID: 588
	[Header("Settings")]
	[SerializeField]
	private SerializedDictionary<string, Transform> positions = new SerializedDictionary<string, Transform>();

	// Token: 0x0400024D RID: 589
	[SerializeField]
	private string initialPosition;

	// Token: 0x0400024E RID: 590
	[SerializeField]
	private float transitionDuration = 0.5f;

	// Token: 0x0400024F RID: 591
	[SerializeField]
	private Ease transitionEase = Ease.Linear;

	// Token: 0x04000250 RID: 592
	private string currentPosition;

	// Token: 0x04000251 RID: 593
	private Tween positionTween;

	// Token: 0x04000252 RID: 594
	private Tween rotationTween;
}
