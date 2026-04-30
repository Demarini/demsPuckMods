using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000163 RID: 355
[UxmlElement]
public class PlayButton : Button
{
	// Token: 0x17000107 RID: 263
	// (get) Token: 0x06000A67 RID: 2663 RVA: 0x00031C15 File Offset: 0x0002FE15
	// (set) Token: 0x06000A68 RID: 2664 RVA: 0x00031C1D File Offset: 0x0002FE1D
	[UxmlAttribute]
	public string Text
	{
		get
		{
			return this.text;
		}
		set
		{
			if (this.text == value)
			{
				return;
			}
			base.text = null;
			this.text = value;
			this.Update();
		}
	}

	// Token: 0x17000108 RID: 264
	// (get) Token: 0x06000A69 RID: 2665 RVA: 0x00031C42 File Offset: 0x0002FE42
	// (set) Token: 0x06000A6A RID: 2666 RVA: 0x00031C4A File Offset: 0x0002FE4A
	[UxmlAttribute]
	public string Description
	{
		get
		{
			return this.description;
		}
		set
		{
			if (this.description == value)
			{
				return;
			}
			this.description = value;
			this.Update();
		}
	}

	// Token: 0x06000A6B RID: 2667 RVA: 0x00031C68 File Offset: 0x0002FE68
	public PlayButton()
	{
		base.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.TrickleDown);
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x00031C83 File Offset: 0x0002FE83
	private void OnAttachToPanel(AttachToPanelEvent e)
	{
		this.textLabel = this.Query("TextLabel", null);
		this.descriptionLabel = this.Query("DescriptionLabel", null);
		this.Update();
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x00031CB9 File Offset: 0x0002FEB9
	private void Update()
	{
		if (this.textLabel != null)
		{
			this.textLabel.text = this.Text;
		}
		if (this.descriptionLabel != null)
		{
			this.descriptionLabel.text = this.Description;
		}
	}

	// Token: 0x040005F9 RID: 1529
	private new string text;

	// Token: 0x040005FA RID: 1530
	private string description;

	// Token: 0x040005FB RID: 1531
	private Label textLabel;

	// Token: 0x040005FC RID: 1532
	private Label descriptionLabel;

	// Token: 0x02000164 RID: 356
	[CompilerGenerated]
	[Serializable]
	public new class UxmlSerializedData : Button.UxmlSerializedData
	{
		// Token: 0x06000A6E RID: 2670 RVA: 0x00031CF0 File Offset: 0x0002FEF0
		[RegisterUxmlCache]
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(PlayButton.UxmlSerializedData), new UxmlAttributeNames[]
			{
				new UxmlAttributeNames("Text", "text", null, Array.Empty<string>()),
				new UxmlAttributeNames("Description", "description", null, Array.Empty<string>())
			});
		}

		// Token: 0x06000A6F RID: 2671 RVA: 0x00031D4A File Offset: 0x0002FF4A
		public override object CreateInstance()
		{
			return new PlayButton();
		}

		// Token: 0x06000A70 RID: 2672 RVA: 0x00031D54 File Offset: 0x0002FF54
		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			PlayButton playButton = (PlayButton)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.Text_UxmlAttributeFlags))
			{
				playButton.Text = this.Text;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.Description_UxmlAttributeFlags))
			{
				playButton.Description = this.Description;
			}
		}

		// Token: 0x040005FD RID: 1533
		[SerializeField]
		private string Text;

		// Token: 0x040005FE RID: 1534
		[SerializeField]
		private string Description;

		// Token: 0x040005FF RID: 1535
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags Text_UxmlAttributeFlags;

		// Token: 0x04000600 RID: 1536
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags Description_UxmlAttributeFlags;
	}
}
