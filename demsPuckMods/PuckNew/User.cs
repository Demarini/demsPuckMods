using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000168 RID: 360
[UxmlElement]
public class User : VisualElement
{
	// Token: 0x1700010B RID: 267
	// (get) Token: 0x06000A7E RID: 2686 RVA: 0x00031F39 File Offset: 0x00030139
	// (set) Token: 0x06000A7F RID: 2687 RVA: 0x00031F41 File Offset: 0x00030141
	[UxmlAttribute]
	public string Username
	{
		get
		{
			return this.username;
		}
		set
		{
			if (this.username == value)
			{
				return;
			}
			this.username = value;
			this.Update();
		}
	}

	// Token: 0x1700010C RID: 268
	// (get) Token: 0x06000A80 RID: 2688 RVA: 0x00031F5F File Offset: 0x0003015F
	// (set) Token: 0x06000A81 RID: 2689 RVA: 0x00031F67 File Offset: 0x00030167
	[UxmlAttribute]
	public Texture2D AvatarTexture
	{
		get
		{
			return this.avatarTexture;
		}
		set
		{
			if (this.avatarTexture == value)
			{
				return;
			}
			this.avatarTexture = value;
			this.Update();
		}
	}

	// Token: 0x06000A82 RID: 2690 RVA: 0x00031F85 File Offset: 0x00030185
	public User()
	{
		base.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.TrickleDown);
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x00031FA0 File Offset: 0x000301A0
	private void OnAttachToPanel(AttachToPanelEvent e)
	{
		this.avatarIcon = this.Query("AvatarIconContainer", null).First().Query(null, null);
		this.usernameLabel = this.Query("UsernameLabel", null);
		this.Update();
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x00031FF0 File Offset: 0x000301F0
	private void Update()
	{
		if (this.usernameLabel != null)
		{
			this.usernameLabel.text = this.Username;
		}
		if (this.avatarIcon != null)
		{
			this.avatarIcon.Texture = this.AvatarTexture;
		}
	}

	// Token: 0x0400060B RID: 1547
	private string username;

	// Token: 0x0400060C RID: 1548
	private Texture2D avatarTexture;

	// Token: 0x0400060D RID: 1549
	private Icon avatarIcon;

	// Token: 0x0400060E RID: 1550
	private Label usernameLabel;

	// Token: 0x02000169 RID: 361
	[CompilerGenerated]
	[Serializable]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		// Token: 0x06000A85 RID: 2693 RVA: 0x00032024 File Offset: 0x00030224
		[RegisterUxmlCache]
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(User.UxmlSerializedData), new UxmlAttributeNames[]
			{
				new UxmlAttributeNames("Username", "username", null, Array.Empty<string>()),
				new UxmlAttributeNames("AvatarTexture", "avatar-texture", null, Array.Empty<string>())
			});
		}

		// Token: 0x06000A86 RID: 2694 RVA: 0x0003207E File Offset: 0x0003027E
		public override object CreateInstance()
		{
			return new User();
		}

		// Token: 0x06000A87 RID: 2695 RVA: 0x00032088 File Offset: 0x00030288
		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			User user = (User)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.Username_UxmlAttributeFlags))
			{
				user.Username = this.Username;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.AvatarTexture_UxmlAttributeFlags))
			{
				user.AvatarTexture = this.AvatarTexture;
			}
		}

		// Token: 0x0400060F RID: 1551
		[SerializeField]
		private Texture2D AvatarTexture;

		// Token: 0x04000610 RID: 1552
		[SerializeField]
		private string Username;

		// Token: 0x04000611 RID: 1553
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags AvatarTexture_UxmlAttributeFlags;

		// Token: 0x04000612 RID: 1554
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags Username_UxmlAttributeFlags;
	}
}
