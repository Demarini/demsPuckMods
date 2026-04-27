using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000159 RID: 345
[UxmlElement]
public class Friend : VisualElement
{
	// Token: 0x170000FC RID: 252
	// (get) Token: 0x06000A27 RID: 2599 RVA: 0x00030F86 File Offset: 0x0002F186
	// (set) Token: 0x06000A28 RID: 2600 RVA: 0x00030F8E File Offset: 0x0002F18E
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

	// Token: 0x170000FD RID: 253
	// (get) Token: 0x06000A29 RID: 2601 RVA: 0x00030FAC File Offset: 0x0002F1AC
	// (set) Token: 0x06000A2A RID: 2602 RVA: 0x00030FB4 File Offset: 0x0002F1B4
	[UxmlAttribute]
	public Texture2D Texture
	{
		get
		{
			return this.texture;
		}
		set
		{
			if (this.texture == value)
			{
				return;
			}
			this.texture = value;
			this.Update();
		}
	}

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x06000A2B RID: 2603 RVA: 0x00030FD2 File Offset: 0x0002F1D2
	// (set) Token: 0x06000A2C RID: 2604 RVA: 0x00030FDA File Offset: 0x0002F1DA
	public User User { get; private set; }

	// Token: 0x06000A2D RID: 2605 RVA: 0x00030FE3 File Offset: 0x0002F1E3
	public Friend()
	{
		base.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.TrickleDown);
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x00031000 File Offset: 0x0002F200
	private void OnAttachToPanel(AttachToPanelEvent e)
	{
		this.User = this.Query("User", null);
		this.inviteIconButton = this.Query("InviteIconButtonContainer", null).First().Query(null, null);
		this.inviteIconButton.clicked += this.OnClickInviteButton;
		this.Update();
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x00031067 File Offset: 0x0002F267
	private void OnClickInviteButton()
	{
		Action inviteButtonClicked = this.InviteButtonClicked;
		if (inviteButtonClicked == null)
		{
			return;
		}
		inviteButtonClicked();
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x00031079 File Offset: 0x0002F279
	private void Update()
	{
		if (this.User != null)
		{
			this.User.AvatarTexture = this.Texture;
			this.User.Username = this.Username;
		}
	}

	// Token: 0x040005D5 RID: 1493
	private string username;

	// Token: 0x040005D6 RID: 1494
	private Texture2D texture;

	// Token: 0x040005D8 RID: 1496
	public Action InviteButtonClicked;

	// Token: 0x040005D9 RID: 1497
	private IconButton inviteIconButton;

	// Token: 0x0200015A RID: 346
	[CompilerGenerated]
	[Serializable]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		// Token: 0x06000A31 RID: 2609 RVA: 0x000310A8 File Offset: 0x0002F2A8
		[RegisterUxmlCache]
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(Friend.UxmlSerializedData), new UxmlAttributeNames[]
			{
				new UxmlAttributeNames("Username", "username", null, Array.Empty<string>()),
				new UxmlAttributeNames("Texture", "texture", null, Array.Empty<string>())
			});
		}

		// Token: 0x06000A32 RID: 2610 RVA: 0x00031102 File Offset: 0x0002F302
		public override object CreateInstance()
		{
			return new Friend();
		}

		// Token: 0x06000A33 RID: 2611 RVA: 0x0003110C File Offset: 0x0002F30C
		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			Friend friend = (Friend)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.Username_UxmlAttributeFlags))
			{
				friend.Username = this.Username;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.Texture_UxmlAttributeFlags))
			{
				friend.Texture = this.Texture;
			}
		}

		// Token: 0x040005DA RID: 1498
		[SerializeField]
		private Texture2D Texture;

		// Token: 0x040005DB RID: 1499
		[SerializeField]
		private string Username;

		// Token: 0x040005DC RID: 1500
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags Texture_UxmlAttributeFlags;

		// Token: 0x040005DD RID: 1501
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags Username_UxmlAttributeFlags;
	}
}
