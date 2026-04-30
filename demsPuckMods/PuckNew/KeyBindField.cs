using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x0200015F RID: 351
[UxmlElement]
public class KeyBindField : VisualElement
{
	// Token: 0x17000101 RID: 257
	// (get) Token: 0x06000A47 RID: 2631 RVA: 0x00031318 File Offset: 0x0002F518
	// (set) Token: 0x06000A48 RID: 2632 RVA: 0x00031320 File Offset: 0x0002F520
	[UxmlAttribute]
	public KeyBindInteractionType InteractionType
	{
		get
		{
			return this.interactionType;
		}
		set
		{
			if (this.interactionType == value)
			{
				return;
			}
			this.interactionType = value;
			this.OnInteractionTypeChanged();
		}
	}

	// Token: 0x17000102 RID: 258
	// (get) Token: 0x06000A49 RID: 2633 RVA: 0x00031339 File Offset: 0x0002F539
	// (set) Token: 0x06000A4A RID: 2634 RVA: 0x00031341 File Offset: 0x0002F541
	[UxmlAttribute]
	public string Label
	{
		get
		{
			return this.label;
		}
		set
		{
			if (this.label == value)
			{
				return;
			}
			this.label = value;
			this.OnLabelChanged();
		}
	}

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x06000A4B RID: 2635 RVA: 0x0003135F File Offset: 0x0002F55F
	// (set) Token: 0x06000A4C RID: 2636 RVA: 0x00031367 File Offset: 0x0002F567
	[UxmlAttribute]
	public string Path
	{
		get
		{
			return this.path;
		}
		set
		{
			if (this.path == value)
			{
				return;
			}
			this.path = value;
			this.OnPathChanged();
		}
	}

	// Token: 0x17000104 RID: 260
	// (get) Token: 0x06000A4D RID: 2637 RVA: 0x00031385 File Offset: 0x0002F585
	// (set) Token: 0x06000A4E RID: 2638 RVA: 0x0003138D File Offset: 0x0002F58D
	public KeyBindInteraction Interaction
	{
		get
		{
			return this.interaction;
		}
		set
		{
			if (this.interaction == value)
			{
				return;
			}
			this.interaction = value;
			this.OnInteractionChanged();
		}
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x000313A6 File Offset: 0x0002F5A6
	public KeyBindField()
	{
		base.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.TrickleDown);
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x000313C4 File Offset: 0x0002F5C4
	private void OnAttachToPanel(AttachToPanelEvent e)
	{
		this.nameLabel = this.Query("NameLabel", null).First();
		this.pathTextField = this.Query("PathTextField", null).First();
		this.pathTextField.RegisterCallback<ClickEvent>(new EventCallback<ClickEvent>(this.OnPathTextFieldClicked), TrickleDown.NoTrickleDown);
		this.interactionDropdownField = this.Query("InteractionDropdownField", null).First();
		this.interactionDropdownField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnInteractionDropdownFieldValueChanged));
		this.OnLabelChanged();
		this.OnPathChanged();
		this.OnInteractionTypeChanged();
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x00031464 File Offset: 0x0002F664
	private void OnInteractionTypeChanged()
	{
		if (this.interactionDropdownField == null)
		{
			return;
		}
		KeyBindInteractionType keyBindInteractionType = this.InteractionType;
		if (keyBindInteractionType == KeyBindInteractionType.Press)
		{
			this.interactionDropdownField.choices = new List<string>
			{
				"PRESS",
				"RELEASE",
				"DOUBLE PRESS",
				"HOLD"
			};
			this.interactionDropdownField.index = 0;
			return;
		}
		if (keyBindInteractionType != KeyBindInteractionType.Hold)
		{
			return;
		}
		this.interactionDropdownField.choices = new List<string>
		{
			"CONTINUOUS",
			"TOGGLE"
		};
		this.interactionDropdownField.index = 0;
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x00031504 File Offset: 0x0002F704
	private void OnLabelChanged()
	{
		if (this.nameLabel == null)
		{
			return;
		}
		this.nameLabel.text = this.label;
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x00031520 File Offset: 0x0002F720
	private void OnPathChanged()
	{
		if (this.pathTextField == null)
		{
			return;
		}
		this.pathTextField.value = this.path;
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x0003153C File Offset: 0x0002F73C
	private void OnInteractionChanged()
	{
		if (this.interactionDropdownField == null)
		{
			return;
		}
		string value;
		switch (this.interaction)
		{
		case KeyBindInteraction.Release:
			value = "RELEASE";
			goto IL_63;
		case KeyBindInteraction.DoublePress:
			value = "DOUBLE PRESS";
			goto IL_63;
		case KeyBindInteraction.Hold:
			value = "HOLD";
			goto IL_63;
		case KeyBindInteraction.Toggle:
			value = "TOGGLE";
			goto IL_63;
		}
		value = ((this.InteractionType == KeyBindInteractionType.Press) ? "PRESS" : "CONTINUOUS");
		IL_63:
		this.interactionDropdownField.value = value;
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x000315B8 File Offset: 0x0002F7B8
	private void OnPathTextFieldClicked(ClickEvent clickEvent)
	{
		Action click = this.Click;
		if (click == null)
		{
			return;
		}
		click();
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x000315CC File Offset: 0x0002F7CC
	private void OnInteractionDropdownFieldValueChanged(ChangeEvent<string> changeEvent)
	{
		string newValue = changeEvent.newValue;
		if (!(newValue == "RELEASE"))
		{
			if (!(newValue == "DOUBLE PRESS"))
			{
				if (!(newValue == "HOLD"))
				{
					if (!(newValue == "TOGGLE"))
					{
						this.interaction = ((this.InteractionType == KeyBindInteractionType.Press) ? KeyBindInteraction.Press : KeyBindInteraction.Continuous);
					}
					else
					{
						this.interaction = KeyBindInteraction.Toggle;
					}
				}
				else
				{
					this.interaction = KeyBindInteraction.Hold;
				}
			}
			else
			{
				this.interaction = KeyBindInteraction.DoublePress;
			}
		}
		else
		{
			this.interaction = KeyBindInteraction.Release;
		}
		Action<KeyBindInteraction> interactionChange = this.InteractionChange;
		if (interactionChange == null)
		{
			return;
		}
		interactionChange(this.interaction);
	}

	// Token: 0x040005E5 RID: 1509
	private KeyBindInteractionType interactionType;

	// Token: 0x040005E6 RID: 1510
	private string label;

	// Token: 0x040005E7 RID: 1511
	private string path;

	// Token: 0x040005E8 RID: 1512
	private KeyBindInteraction interaction;

	// Token: 0x040005E9 RID: 1513
	public Action Click;

	// Token: 0x040005EA RID: 1514
	public Action<KeyBindInteraction> InteractionChange;

	// Token: 0x040005EB RID: 1515
	private Label nameLabel;

	// Token: 0x040005EC RID: 1516
	private TextField pathTextField;

	// Token: 0x040005ED RID: 1517
	private DropdownField interactionDropdownField;

	// Token: 0x02000160 RID: 352
	[CompilerGenerated]
	[Serializable]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		// Token: 0x06000A57 RID: 2647 RVA: 0x00031664 File Offset: 0x0002F864
		[RegisterUxmlCache]
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(KeyBindField.UxmlSerializedData), new UxmlAttributeNames[]
			{
				new UxmlAttributeNames("InteractionType", "interaction-type", null, Array.Empty<string>()),
				new UxmlAttributeNames("Label", "label", null, Array.Empty<string>()),
				new UxmlAttributeNames("Path", "path", null, Array.Empty<string>())
			});
		}

		// Token: 0x06000A58 RID: 2648 RVA: 0x000316DA File Offset: 0x0002F8DA
		public override object CreateInstance()
		{
			return new KeyBindField();
		}

		// Token: 0x06000A59 RID: 2649 RVA: 0x000316E4 File Offset: 0x0002F8E4
		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			KeyBindField keyBindField = (KeyBindField)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.InteractionType_UxmlAttributeFlags))
			{
				keyBindField.InteractionType = this.InteractionType;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.Label_UxmlAttributeFlags))
			{
				keyBindField.Label = this.Label;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.Path_UxmlAttributeFlags))
			{
				keyBindField.Path = this.Path;
			}
		}

		// Token: 0x040005EE RID: 1518
		[SerializeField]
		private KeyBindInteractionType InteractionType;

		// Token: 0x040005EF RID: 1519
		[SerializeField]
		private string Label;

		// Token: 0x040005F0 RID: 1520
		[SerializeField]
		private string Path;

		// Token: 0x040005F1 RID: 1521
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags InteractionType_UxmlAttributeFlags;

		// Token: 0x040005F2 RID: 1522
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags Label_UxmlAttributeFlags;

		// Token: 0x040005F3 RID: 1523
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags Path_UxmlAttributeFlags;
	}
}
