using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x0200015D RID: 349
[UxmlElement]
public class IconButton : Button
{
	// Token: 0x17000100 RID: 256
	// (get) Token: 0x06000A3E RID: 2622 RVA: 0x00031228 File Offset: 0x0002F428
	// (set) Token: 0x06000A3F RID: 2623 RVA: 0x00031230 File Offset: 0x0002F430
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

	// Token: 0x06000A40 RID: 2624 RVA: 0x0003124E File Offset: 0x0002F44E
	public IconButton()
	{
		base.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.TrickleDown);
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x00031269 File Offset: 0x0002F469
	private void OnAttachToPanel(AttachToPanelEvent e)
	{
		this.icon = this.Query(null, null);
		this.Update();
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x00031284 File Offset: 0x0002F484
	private void Update()
	{
		if (this.icon != null)
		{
			this.icon.Texture = this.Texture;
		}
	}

	// Token: 0x040005E1 RID: 1505
	private Texture2D texture;

	// Token: 0x040005E2 RID: 1506
	private Icon icon;

	// Token: 0x0200015E RID: 350
	[CompilerGenerated]
	[Serializable]
	public new class UxmlSerializedData : Button.UxmlSerializedData
	{
		// Token: 0x06000A43 RID: 2627 RVA: 0x0003129F File Offset: 0x0002F49F
		[RegisterUxmlCache]
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(IconButton.UxmlSerializedData), new UxmlAttributeNames[]
			{
				new UxmlAttributeNames("Texture", "texture", null, Array.Empty<string>())
			});
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x000312D2 File Offset: 0x0002F4D2
		public override object CreateInstance()
		{
			return new IconButton();
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x000312DC File Offset: 0x0002F4DC
		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			IconButton iconButton = (IconButton)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.Texture_UxmlAttributeFlags))
			{
				iconButton.Texture = this.Texture;
			}
		}

		// Token: 0x040005E3 RID: 1507
		[SerializeField]
		private Texture2D Texture;

		// Token: 0x040005E4 RID: 1508
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags Texture_UxmlAttributeFlags;
	}
}
