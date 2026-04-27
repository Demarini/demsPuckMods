using System;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x0200006A RID: 106
public class Spectator : MonoBehaviour
{
	// Token: 0x06000377 RID: 887 RVA: 0x00014570 File Offset: 0x00012770
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
	}

	// Token: 0x06000378 RID: 888 RVA: 0x0001459D File Offset: 0x0001279D
	public void PlayAnimation(string animationName)
	{
		this.StopAnimations();
		this.animator.SetBool(animationName, true);
		this.animationRequested = true;
	}

	// Token: 0x06000379 RID: 889 RVA: 0x000145B9 File Offset: 0x000127B9
	public void StopAnimations()
	{
		this.animator.SetBool("Seated", false);
		this.animator.SetBool("Cheering", false);
		this.animator.SetBool("Standing", false);
		this.animationRequested = true;
	}

	// Token: 0x0600037A RID: 890 RVA: 0x000145F8 File Offset: 0x000127F8
	public void UpdateAnimation()
	{
		if (!this.playerMesh)
		{
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
		bool flag = currentAnimatorStateInfo.loop || currentAnimatorStateInfo.normalizedTime < 1f || this.animator.IsInTransition(0) || this.animationRequested;
		double num = Time.timeAsDouble - this.lastUpdateTime;
		if (flag)
		{
			this.animator.Update((float)num);
		}
		else
		{
			this.playerMesh.LookAt(this.LookTarget ? this.LookTarget.position : Vector3.zero, (float)num, false, true);
		}
		if (this.animationRequested)
		{
			this.animationRequested = false;
		}
		this.lastUpdateTime = Time.timeAsDouble;
	}

	// Token: 0x0600037B RID: 891 RVA: 0x000146B4 File Offset: 0x000128B4
	public void RandomizeAppearance()
	{
		this.playerMesh.SetUsername(null);
		this.playerMesh.SetNumber(null);
		this.playerMesh.SetLegsPadsActive(false);
		int headgearID = this.headgearOptions[Random.Range(0, this.headgearOptions.Length)];
		this.playerMesh.SetHeadgearID(headgearID, PlayerRole.None);
		int jerseyID = this.jerseyOptions[Random.Range(0, this.jerseyOptions.Length)];
		this.playerMesh.SetJerseyID(jerseyID, PlayerTeam.None);
		int mustacheID = this.mustacheOptions[Random.Range(0, this.mustacheOptions.Length)];
		this.playerMesh.SetMustacheID(mustacheID);
		int beardID = this.beardOptions[Random.Range(0, this.beardOptions.Length)];
		this.playerMesh.SetBeardID(beardID);
	}

	// Token: 0x04000269 RID: 617
	[Header("References")]
	[SerializeField]
	private PlayerMesh playerMesh;

	// Token: 0x0400026A RID: 618
	[SerializeField]
	private Animator animator;

	// Token: 0x0400026B RID: 619
	[HideInInspector]
	public Transform LookTarget;

	// Token: 0x0400026C RID: 620
	private bool animationRequested;

	// Token: 0x0400026D RID: 621
	private double lastUpdateTime;

	// Token: 0x0400026E RID: 622
	private int[] headgearOptions = new int[]
	{
		-1,
		537,
		538,
		539
	};

	// Token: 0x0400026F RID: 623
	private int[] jerseyOptions = new int[]
	{
		2118,
		2119,
		2120,
		2121,
		2122
	};

	// Token: 0x04000270 RID: 624
	private int[] mustacheOptions = new int[]
	{
		-1,
		1024,
		1025,
		1026,
		1027,
		1028,
		1029,
		1030
	};

	// Token: 0x04000271 RID: 625
	private int[] beardOptions = new int[]
	{
		-1,
		1536,
		1537,
		1538,
		1539,
		1540
	};
}
