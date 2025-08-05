using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

// Token: 0x020000DA RID: 218
public class PlayerHead : MonoBehaviour
{
	// Token: 0x17000095 RID: 149
	// (get) Token: 0x0600068E RID: 1678 RVA: 0x0000B391 File Offset: 0x00009591
	// (set) Token: 0x0600068D RID: 1677 RVA: 0x0000B378 File Offset: 0x00009578
	public PlayerHeadType HeadType
	{
		get
		{
			return this.headType;
		}
		set
		{
			if (this.headType == value)
			{
				return;
			}
			this.headType = value;
			this.OnHeadTypeChanged();
		}
	}

	// Token: 0x17000096 RID: 150
	// (get) Token: 0x0600068F RID: 1679 RVA: 0x0000B399 File Offset: 0x00009599
	public string[] HairTypes
	{
		get
		{
			return this.hairGameObjectMap.Keys.ToArray<string>();
		}
	}

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x06000690 RID: 1680 RVA: 0x0000B3AB File Offset: 0x000095AB
	public string[] MustacheTypes
	{
		get
		{
			return this.mustacheGameObjectMap.Keys.ToArray<string>();
		}
	}

	// Token: 0x17000098 RID: 152
	// (get) Token: 0x06000691 RID: 1681 RVA: 0x0000B3BD File Offset: 0x000095BD
	public string[] BeardTypes
	{
		get
		{
			return this.beardGameObjectMap.Keys.ToArray<string>();
		}
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x00026AF4 File Offset: 0x00024CF4
	public void HideHair()
	{
		foreach (GameObject gameObject in this.hairGameObjectMap.Values)
		{
			if (gameObject)
			{
				gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x00026B54 File Offset: 0x00024D54
	public void HideMustache()
	{
		foreach (GameObject gameObject in this.mustacheGameObjectMap.Values)
		{
			if (gameObject)
			{
				gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x00026BB4 File Offset: 0x00024DB4
	public void HideBeard()
	{
		foreach (GameObject gameObject in this.beardGameObjectMap.Values)
		{
			if (gameObject)
			{
				gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x00026C14 File Offset: 0x00024E14
	public void HideGear()
	{
		this.helmet.SetActive(false);
		this.helmetStrapLeft.SetActive(false);
		this.helmetStrapRight.SetActive(false);
		this.helmetFlag.SetActive(false);
		this.helmetVisor.SetActive(false);
		this.cage.SetActive(false);
		this.neckShield.SetActive(false);
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x00026C78 File Offset: 0x00024E78
	public void SetHair(string name)
	{
		if (this.HeadType != PlayerHeadType.Spectator)
		{
			return;
		}
		if (!this.hairGameObjectMap.ContainsKey(name))
		{
			return;
		}
		if (this.hairGameObjectMap[name] == null)
		{
			this.HideHair();
			return;
		}
		this.HideHair();
		this.hairGameObjectMap[name].SetActive(true);
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x00026CD4 File Offset: 0x00024ED4
	public void SetMustache(string name)
	{
		if (!this.mustacheGameObjectMap.ContainsKey(name))
		{
			return;
		}
		if (this.mustacheGameObjectMap[name] == null)
		{
			this.HideMustache();
			return;
		}
		this.HideMustache();
		this.mustacheGameObjectMap[name].SetActive(true);
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x00026D24 File Offset: 0x00024F24
	public void SetBeard(string name)
	{
		if (!this.beardGameObjectMap.ContainsKey(name))
		{
			return;
		}
		if (this.beardGameObjectMap[name] == null)
		{
			this.HideBeard();
			return;
		}
		this.HideBeard();
		this.beardGameObjectMap[name].SetActive(true);
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x00026D74 File Offset: 0x00024F74
	public void SetHelmetFlag(string name)
	{
		if (!this.flagTextureMap.ContainsKey(name))
		{
			return;
		}
		if (this.flagTextureMap[name] == null)
		{
			this.helmetFlag.SetActive(false);
			return;
		}
		this.helmetFlag.SetActive(true);
		MeshRendererTexturer component = this.helmetFlag.GetComponent<MeshRendererTexturer>();
		if (component == null)
		{
			this.helmetFlag.SetActive(false);
			return;
		}
		component.SetTexture(this.flagTextureMap[name]);
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x00026DF4 File Offset: 0x00024FF4
	public void SetHelmetVisor(string name)
	{
		if (this.HeadType != PlayerHeadType.Attacker)
		{
			return;
		}
		if (this.visorMaterialMap == null || !this.visorMaterialMap.ContainsKey(name))
		{
			return;
		}
		if (!this.helmetVisor)
		{
			return;
		}
		MeshRenderer component = this.helmetVisor.GetComponent<MeshRenderer>();
		if (component == null)
		{
			return;
		}
		if (this.visorMaterialMap[name] == null)
		{
			this.helmetVisor.SetActive(false);
			return;
		}
		this.helmetVisor.SetActive(true);
		UnityEngine.Object.Destroy(component.material);
		component.material = this.visorMaterialMap[name];
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x00026E90 File Offset: 0x00025090
	private void OnHeadTypeChanged()
	{
		switch (this.HeadType)
		{
		case PlayerHeadType.Attacker:
			this.HideGear();
			this.helmet.SetActive(true);
			this.helmetStrapLeft.SetActive(true);
			this.helmetStrapRight.SetActive(true);
			this.helmetFlag.SetActive(true);
			this.helmetVisor.SetActive(true);
			return;
		case PlayerHeadType.Goalie:
			this.HideGear();
			this.helmet.SetActive(true);
			this.cage.SetActive(true);
			this.neckShield.SetActive(true);
			this.helmetFlag.SetActive(true);
			return;
		case PlayerHeadType.Spectator:
			this.HideGear();
			return;
		default:
			return;
		}
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x00026F38 File Offset: 0x00025138
	private void OnDestroy()
	{
		this.flagTextureMap.Clear();
		this.hairGameObjectMap.Clear();
		this.mustacheGameObjectMap.Clear();
		this.beardGameObjectMap.Clear();
		this.visorMaterialMap.Clear();
		this.flagTextureMap = null;
		this.hairGameObjectMap = null;
		this.mustacheGameObjectMap = null;
		this.beardGameObjectMap = null;
		this.visorMaterialMap = null;
	}

	// Token: 0x040003B8 RID: 952
	[Header("References")]
	[SerializeField]
	private GameObject head;

	// Token: 0x040003B9 RID: 953
	[SerializeField]
	private SerializedDictionary<string, Texture> flagTextureMap = new SerializedDictionary<string, Texture>();

	// Token: 0x040003BA RID: 954
	[SerializeField]
	private SerializedDictionary<string, GameObject> hairGameObjectMap = new SerializedDictionary<string, GameObject>();

	// Token: 0x040003BB RID: 955
	[SerializeField]
	private SerializedDictionary<string, GameObject> mustacheGameObjectMap = new SerializedDictionary<string, GameObject>();

	// Token: 0x040003BC RID: 956
	[SerializeField]
	private SerializedDictionary<string, GameObject> beardGameObjectMap = new SerializedDictionary<string, GameObject>();

	// Token: 0x040003BD RID: 957
	[SerializeField]
	private GameObject helmet;

	// Token: 0x040003BE RID: 958
	[SerializeField]
	private GameObject helmetFlag;

	// Token: 0x040003BF RID: 959
	[SerializeField]
	private GameObject helmetStrapLeft;

	// Token: 0x040003C0 RID: 960
	[SerializeField]
	private GameObject helmetStrapRight;

	// Token: 0x040003C1 RID: 961
	[SerializeField]
	private GameObject helmetVisor;

	// Token: 0x040003C2 RID: 962
	[SerializeField]
	private SerializedDictionary<string, Material> visorMaterialMap = new SerializedDictionary<string, Material>();

	// Token: 0x040003C3 RID: 963
	[SerializeField]
	private GameObject cage;

	// Token: 0x040003C4 RID: 964
	[SerializeField]
	private GameObject neckShield;

	// Token: 0x040003C5 RID: 965
	private PlayerHeadType headType;
}
