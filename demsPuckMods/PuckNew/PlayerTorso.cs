using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000033 RID: 51
[RequireComponent(typeof(MeshRendererTexturer))]
[ExecuteInEditMode]
public class PlayerTorso : MonoBehaviour
{
	// Token: 0x06000107 RID: 263 RVA: 0x00005438 File Offset: 0x00003638
	private void Awake()
	{
		this.meshRendererTexturer = base.GetComponent<MeshRendererTexturer>();
	}

	// Token: 0x06000108 RID: 264 RVA: 0x00005446 File Offset: 0x00003646
	public void SetUsername(string username)
	{
		if (string.IsNullOrEmpty(username))
		{
			this.usernameText.gameObject.SetActive(false);
			return;
		}
		this.usernameText.text = username;
		this.usernameText.gameObject.SetActive(true);
	}

	// Token: 0x06000109 RID: 265 RVA: 0x0000547F File Offset: 0x0000367F
	public void SetNumber(string number)
	{
		if (string.IsNullOrEmpty(number))
		{
			this.numberText.gameObject.SetActive(false);
			return;
		}
		this.numberText.text = number;
		this.numberText.gameObject.SetActive(true);
	}

	// Token: 0x0600010A RID: 266 RVA: 0x000054B8 File Offset: 0x000036B8
	public void SetJerseyID(int jerseyID, PlayerTeam team)
	{
		Jersey jersey = this.jerseys.Find((Jersey j) => j.ID == jerseyID && j.IsForTeam(team));
		if (jersey == null)
		{
			Debug.LogWarning(string.Format("[PlayerTorso] Tried to set invalid jerseyID {0} for team {1}", jerseyID, team));
			return;
		}
		this.meshRendererTexturer.SetTexture(jersey.Texture);
	}

	// Token: 0x040000C2 RID: 194
	[Header("Settings")]
	[SerializeField]
	private List<Jersey> jerseys = new List<Jersey>();

	// Token: 0x040000C3 RID: 195
	[Header("References")]
	[SerializeField]
	private TMP_Text usernameText;

	// Token: 0x040000C4 RID: 196
	[SerializeField]
	private TMP_Text numberText;

	// Token: 0x040000C5 RID: 197
	private MeshRendererTexturer meshRendererTexturer;
}
