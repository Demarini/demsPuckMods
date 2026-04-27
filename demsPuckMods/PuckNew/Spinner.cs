using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000166 RID: 358
[UxmlElement]
public class Spinner : VisualElement
{
	// Token: 0x17000109 RID: 265
	// (get) Token: 0x06000A72 RID: 2674 RVA: 0x00031DA1 File Offset: 0x0002FFA1
	// (set) Token: 0x06000A73 RID: 2675 RVA: 0x00031DA9 File Offset: 0x0002FFA9
	[UxmlAttribute]
	public float speed { get; set; }

	// Token: 0x1700010A RID: 266
	// (get) Token: 0x06000A74 RID: 2676 RVA: 0x00031DB2 File Offset: 0x0002FFB2
	// (set) Token: 0x06000A75 RID: 2677 RVA: 0x00031DBA File Offset: 0x0002FFBA
	[UxmlAttribute]
	public SpinnerDirection direction { get; set; }

	// Token: 0x06000A76 RID: 2678 RVA: 0x00031DC3 File Offset: 0x0002FFC3
	public Spinner()
	{
		base.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.NoTrickleDown);
		base.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnDetachFromPanel), TrickleDown.NoTrickleDown);
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x00031DF1 File Offset: 0x0002FFF1
	private void OnAttachToPanel(AttachToPanelEvent e)
	{
		this.scheduledItem = base.schedule.Execute(new Action(this.OnScheduleUpdate)).Every(16L);
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x00031E18 File Offset: 0x00030018
	private void OnDetachFromPanel(DetachFromPanelEvent e)
	{
		this.scheduledItem.Pause();
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x00031E28 File Offset: 0x00030028
	private void OnScheduleUpdate()
	{
		base.style.rotate = new Rotate(base.style.rotate.value.angle.value + this.speed * (float)((this.direction == SpinnerDirection.Clockwise) ? 1 : -1));
	}

	// Token: 0x04000606 RID: 1542
	private IVisualElementScheduledItem scheduledItem;

	// Token: 0x02000167 RID: 359
	[CompilerGenerated]
	[Serializable]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		// Token: 0x06000A7A RID: 2682 RVA: 0x00031E88 File Offset: 0x00030088
		[RegisterUxmlCache]
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(Spinner.UxmlSerializedData), new UxmlAttributeNames[]
			{
				new UxmlAttributeNames("speed", "speed", null, Array.Empty<string>()),
				new UxmlAttributeNames("direction", "direction", null, Array.Empty<string>())
			});
		}

		// Token: 0x06000A7B RID: 2683 RVA: 0x00031EE2 File Offset: 0x000300E2
		public override object CreateInstance()
		{
			return new Spinner();
		}

		// Token: 0x06000A7C RID: 2684 RVA: 0x00031EEC File Offset: 0x000300EC
		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			Spinner spinner = (Spinner)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.speed_UxmlAttributeFlags))
			{
				spinner.speed = this.speed;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.direction_UxmlAttributeFlags))
			{
				spinner.direction = this.direction;
			}
		}

		// Token: 0x04000607 RID: 1543
		[SerializeField]
		private float speed;

		// Token: 0x04000608 RID: 1544
		[SerializeField]
		private SpinnerDirection direction;

		// Token: 0x04000609 RID: 1545
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags speed_UxmlAttributeFlags;

		// Token: 0x0400060A RID: 1546
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags direction_UxmlAttributeFlags;
	}
}
