using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000106 RID: 262
[UxmlElement]
public class RotatingImage : VisualElement
{
	// Token: 0x170000DA RID: 218
	// (get) Token: 0x06000932 RID: 2354 RVA: 0x0000CB89 File Offset: 0x0000AD89
	// (set) Token: 0x06000933 RID: 2355 RVA: 0x0000CB91 File Offset: 0x0000AD91
	[UxmlAttribute]
	public float rotationSpeed { get; set; }

	// Token: 0x06000934 RID: 2356 RVA: 0x0000CB9A File Offset: 0x0000AD9A
	public RotatingImage()
	{
		base.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.NoTrickleDown);
		base.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnDetachFromPanel), TrickleDown.NoTrickleDown);
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x0000CBC8 File Offset: 0x0000ADC8
	private void OnAttachToPanel(AttachToPanelEvent e)
	{
		base.schedule.Execute(new Action(this.OnScheduleUpdate)).Every(16L);
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x00006C1B File Offset: 0x00004E1B
	private void OnDetachFromPanel(DetachFromPanelEvent e)
	{
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x00038FC4 File Offset: 0x000371C4
	private void OnScheduleUpdate()
	{
		base.style.rotate = new Rotate(base.style.rotate.value.angle.value + this.rotationSpeed);
	}

	// Token: 0x02000107 RID: 263
	[CompilerGenerated]
	[Serializable]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		// Token: 0x06000938 RID: 2360 RVA: 0x0000CBEA File Offset: 0x0000ADEA
		[RegisterUxmlCache]
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(RotatingImage.UxmlSerializedData), new UxmlAttributeNames[]
			{
				new UxmlAttributeNames("rotationSpeed", "rotation-speed", null, Array.Empty<string>())
			});
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x0000CC1D File Offset: 0x0000AE1D
		public override object CreateInstance()
		{
			return new RotatingImage();
		}

		// Token: 0x0600093A RID: 2362 RVA: 0x00039018 File Offset: 0x00037218
		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			RotatingImage rotatingImage = (RotatingImage)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.rotationSpeed_UxmlAttributeFlags))
			{
				rotatingImage.rotationSpeed = this.rotationSpeed;
			}
		}

		// Token: 0x0400059E RID: 1438
		[SerializeField]
		private float rotationSpeed;

		// Token: 0x0400059F RID: 1439
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags rotationSpeed_UxmlAttributeFlags;
	}
}
