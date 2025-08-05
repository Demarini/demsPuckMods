using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x0200002E RID: 46
public class Spectator : MonoBehaviour
{
	// Token: 0x1700001B RID: 27
	// (get) Token: 0x06000142 RID: 322 RVA: 0x00007AB8 File Offset: 0x00005CB8
	[HideInInspector]
	private float AnimationUpdateInterval
	{
		get
		{
			return 1f / this.animationUpdateRate;
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x06000143 RID: 323 RVA: 0x00007AC6 File Offset: 0x00005CC6
	[HideInInspector]
	private float LookAtUpdateInterval
	{
		get
		{
			return 1f / this.lookAtUpdateRate;
		}
	}

	// Token: 0x06000144 RID: 324 RVA: 0x00007AD4 File Offset: 0x00005CD4
	private void Awake()
	{
		this.animationUpdateAccumulator = UnityEngine.Random.Range(0f, this.AnimationUpdateInterval);
		this.lookAtUpdateAccumulator = UnityEngine.Random.Range(0f, this.LookAtUpdateInterval);
	}

	// Token: 0x06000145 RID: 325 RVA: 0x00012468 File Offset: 0x00010668
	private void Start()
	{
		this.Randomize();
		this.animator.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
		this.PlayAnimation("Seated", 0f);
	}

	// Token: 0x06000146 RID: 326 RVA: 0x00007B02 File Offset: 0x00005D02
	private void OnDestroy()
	{
		if (this.animationCoroutine != null)
		{
			base.StopCoroutine(this.animationCoroutine);
		}
	}

	// Token: 0x06000147 RID: 327 RVA: 0x000124A0 File Offset: 0x000106A0
	private void Update()
	{
		if (!this.playerMesh)
		{
			return;
		}
		this.animationUpdateAccumulator += Time.deltaTime;
		if (this.animationUpdateAccumulator >= this.AnimationUpdateInterval)
		{
			this.animator.Update(this.animationUpdateAccumulator);
			while (this.animationUpdateAccumulator >= this.AnimationUpdateInterval)
			{
				this.animationUpdateAccumulator -= this.AnimationUpdateInterval;
			}
		}
		this.lookAtUpdateAccumulator += Time.deltaTime;
		if (this.lookAtUpdateAccumulator >= this.LookAtUpdateInterval)
		{
			if (this.LookTarget)
			{
				this.playerMesh.LookAt(this.LookTarget.position, this.lookAtUpdateAccumulator);
			}
			else
			{
				this.playerMesh.LookAt(new Vector3(0f, 0f, 0f), this.lookAtUpdateAccumulator);
			}
			while (this.lookAtUpdateAccumulator >= this.LookAtUpdateInterval)
			{
				this.lookAtUpdateAccumulator -= this.LookAtUpdateInterval;
			}
		}
	}

	// Token: 0x06000148 RID: 328 RVA: 0x00007B18 File Offset: 0x00005D18
	private IEnumerator IAnimate()
	{
		this.PlayAnimation("Seated", UnityEngine.Random.Range(0f, 0.25f));
		yield return new WaitForSeconds(3f);
		if (UnityEngine.Random.Range(0, 3) == 0)
		{
			this.PlayAnimation("Standing", UnityEngine.Random.Range(0f, 0.25f));
		}
		else
		{
			this.PlayAnimation("Cheering", UnityEngine.Random.Range(0f, 0.25f));
		}
		yield return new WaitForSeconds(3f);
		base.StartCoroutine(this.IAnimate());
		yield break;
	}

	// Token: 0x06000149 RID: 329 RVA: 0x00007B27 File Offset: 0x00005D27
	public void PlayAnimation(string animationName, float delay = 0f)
	{
		if (this.animationCoroutine != null)
		{
			base.StopCoroutine(this.animationCoroutine);
		}
		this.animationCoroutine = this.IPlayAnimation(animationName, delay);
		base.StartCoroutine(this.animationCoroutine);
	}

	// Token: 0x0600014A RID: 330 RVA: 0x00007B58 File Offset: 0x00005D58
	private IEnumerator IPlayAnimation(string animationName, float delay)
	{
		yield return new WaitForSeconds(delay);
		this.StopAnimations();
		this.animator.SetBool(animationName, true);
		yield break;
	}

	// Token: 0x0600014B RID: 331 RVA: 0x00007B75 File Offset: 0x00005D75
	public void StopAnimations()
	{
		this.animator.SetBool("Seated", false);
		this.animator.SetBool("Cheering", false);
		this.animator.SetBool("Standing", false);
	}

	// Token: 0x0600014C RID: 332 RVA: 0x000125A4 File Offset: 0x000107A4
	private void Randomize()
	{
		this.playerMesh.IsUsernameActive = false;
		this.playerMesh.IsNumberActive = false;
		this.playerMesh.IsLegPadsActive = false;
		string[] array = this.playerMesh.PlayerGroin.TextureNames.ToList<string>().FindAll((string texture) => texture.Contains("spectator")).ToArray();
		string texture3 = array[UnityEngine.Random.Range(0, array.Length)];
		string texture2 = this.playerMesh.PlayerTorso.TextureNames[UnityEngine.Random.Range(0, this.playerMesh.PlayerTorso.TextureNames.Length)];
		this.playerMesh.PlayerGroin.SetTexture(texture3);
		this.playerMesh.PlayerTorso.SetTexture(texture2);
		string text = this.playerMesh.PlayerHead.HairTypes[UnityEngine.Random.Range(0, this.playerMesh.PlayerHead.HairTypes.Length)];
		if (text != "hair_pony_tail")
		{
			string mustache = this.playerMesh.PlayerHead.MustacheTypes[UnityEngine.Random.Range(0, this.playerMesh.PlayerHead.MustacheTypes.Length)];
			string beard = this.playerMesh.PlayerHead.BeardTypes[UnityEngine.Random.Range(0, this.playerMesh.PlayerHead.BeardTypes.Length)];
			this.playerMesh.PlayerHead.SetBeard(beard);
			this.playerMesh.PlayerHead.SetMustache(mustache);
		}
		this.playerMesh.PlayerHead.HeadType = PlayerHeadType.Spectator;
		this.playerMesh.PlayerHead.SetHair(text);
	}

	// Token: 0x040000AC RID: 172
	[Header("References")]
	[SerializeField]
	private PlayerMesh playerMesh;

	// Token: 0x040000AD RID: 173
	[SerializeField]
	private Animator animator;

	// Token: 0x040000AE RID: 174
	[Header("Settings")]
	[SerializeField]
	private float animationUpdateRate = 30f;

	// Token: 0x040000AF RID: 175
	[SerializeField]
	private float lookAtUpdateRate = 15f;

	// Token: 0x040000B0 RID: 176
	[HideInInspector]
	public Transform LookTarget;

	// Token: 0x040000B1 RID: 177
	private float animationUpdateAccumulator;

	// Token: 0x040000B2 RID: 178
	private float lookAtUpdateAccumulator;

	// Token: 0x040000B3 RID: 179
	private IEnumerator animationCoroutine;
}
