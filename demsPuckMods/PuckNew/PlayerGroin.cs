using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000021 RID: 33
[RequireComponent(typeof(MeshRendererTexturer))]
[ExecuteInEditMode]
public class PlayerGroin : MonoBehaviour
{
	// Token: 0x060000CB RID: 203 RVA: 0x00004A05 File Offset: 0x00002C05
	private void Awake()
	{
		this.meshRendererTexturer = base.GetComponent<MeshRendererTexturer>();
	}

	// Token: 0x060000CC RID: 204 RVA: 0x00004A14 File Offset: 0x00002C14
	public void SetJerseyID(int jerseyID, PlayerTeam team)
	{
		Jersey jersey = this.jerseys.Find((Jersey j) => j.ID == jerseyID && j.IsForTeam(team));
		if (jersey == null)
		{
			Debug.LogWarning(string.Format("[PlayerGroin] Tried to set invalid jerseyID {0}", jerseyID));
			return;
		}
		this.meshRendererTexturer.SetTexture(jersey.Texture);
	}

	// Token: 0x0400007F RID: 127
	[Header("References")]
	[SerializeField]
	private List<Jersey> jerseys = new List<Jersey>();

	// Token: 0x04000080 RID: 128
	private MeshRendererTexturer meshRendererTexturer;
}
