using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x0200015B RID: 347
[UxmlElement]
public class Icon : VisualElement
{
	// Token: 0x170000FF RID: 255
	// (get) Token: 0x06000A35 RID: 2613 RVA: 0x00031159 File Offset: 0x0002F359
	// (set) Token: 0x06000A36 RID: 2614 RVA: 0x00031161 File Offset: 0x0002F361
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

	// Token: 0x06000A37 RID: 2615 RVA: 0x0003117F File Offset: 0x0002F37F
	public Icon()
	{
		base.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.TrickleDown);
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x0003119A File Offset: 0x0002F39A
	private void OnAttachToPanel(AttachToPanelEvent e)
	{
		this.Update();
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x000311A2 File Offset: 0x0002F3A2
	private void Update()
	{
		base.style.backgroundImage = this.Texture;
	}

	// Token: 0x040005DE RID: 1502
	private Texture2D texture;

	// Token: 0x0200015C RID: 348
	[CompilerGenerated]
	[Serializable]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		// Token: 0x06000A3A RID: 2618 RVA: 0x000311BA File Offset: 0x0002F3BA
		[RegisterUxmlCache]
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(Icon.UxmlSerializedData), new UxmlAttributeNames[]
			{
				new UxmlAttributeNames("Texture", "texture", null, Array.Empty<string>())
			});
		}

		// Token: 0x06000A3B RID: 2619 RVA: 0x000311ED File Offset: 0x0002F3ED
		public override object CreateInstance()
		{
			return new Icon();
		}

		// Token: 0x06000A3C RID: 2620 RVA: 0x000311F4 File Offset: 0x0002F3F4
		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			Icon icon = (Icon)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.Texture_UxmlAttributeFlags))
			{
				icon.Texture = this.Texture;
			}
		}

		// Token: 0x040005DF RID: 1503
		[SerializeField]
		private Texture2D Texture;

		// Token: 0x040005E0 RID: 1504
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags Texture_UxmlAttributeFlags;
	}
}
