using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000104 RID: 260
[UxmlElement]
public class KeyBindControl : VisualElement
{
	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x06000919 RID: 2329 RVA: 0x0000C9D3 File Offset: 0x0000ABD3
	// (set) Token: 0x0600091A RID: 2330 RVA: 0x0000C9DB File Offset: 0x0000ABDB
	[UxmlAttribute]
	public string NameLabel
	{
		get
		{
			return this.nameLabel;
		}
		set
		{
			if (this.nameLabel == value)
			{
				return;
			}
			this.nameLabel = value;
			this.OnNameLabelChanged();
		}
	}

	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x0600091B RID: 2331 RVA: 0x0000C9F9 File Offset: 0x0000ABF9
	// (set) Token: 0x0600091C RID: 2332 RVA: 0x0000CA01 File Offset: 0x0000AC01
	[UxmlAttribute]
	public string PathLabel
	{
		get
		{
			return this.pathLabel;
		}
		set
		{
			if (this.pathLabel == value)
			{
				return;
			}
			this.pathLabel = value;
			this.OnPathLabelChanged();
		}
	}

	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x0600091D RID: 2333 RVA: 0x0000CA1F File Offset: 0x0000AC1F
	// (set) Token: 0x0600091E RID: 2334 RVA: 0x0000CA27 File Offset: 0x0000AC27
	[UxmlAttribute]
	public string TypeDropdownValue
	{
		get
		{
			return this.typeDropdownValue;
		}
		set
		{
			if (this.typeDropdownValue == value)
			{
				return;
			}
			this.typeDropdownValue = value;
			if (this.DropdownField == null)
			{
				return;
			}
			this.DropdownField.value = value;
		}
	}

	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x0600091F RID: 2335 RVA: 0x0000CA54 File Offset: 0x0000AC54
	// (set) Token: 0x06000920 RID: 2336 RVA: 0x0000CA5C File Offset: 0x0000AC5C
	[UxmlAttribute]
	public bool IsTypeDropdownVisible
	{
		get
		{
			return this.isTypeDropdownVisible;
		}
		set
		{
			if (this.isTypeDropdownVisible == value)
			{
				return;
			}
			this.isTypeDropdownVisible = value;
			this.OnTypeDropdownVisibilityChanged();
		}
	}

	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x06000921 RID: 2337 RVA: 0x0000CA75 File Offset: 0x0000AC75
	// (set) Token: 0x06000922 RID: 2338 RVA: 0x0000CA7D File Offset: 0x0000AC7D
	[UxmlAttribute]
	public bool IsPressable
	{
		get
		{
			return this.isPressable;
		}
		set
		{
			if (this.isPressable == value)
			{
				return;
			}
			this.isPressable = value;
			this.OnControlTypeChanged();
		}
	}

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x06000923 RID: 2339 RVA: 0x0000CA96 File Offset: 0x0000AC96
	// (set) Token: 0x06000924 RID: 2340 RVA: 0x0000CA9E File Offset: 0x0000AC9E
	[UxmlAttribute]
	public bool IsHoldable
	{
		get
		{
			return this.isHoldable;
		}
		set
		{
			if (this.isHoldable == value)
			{
				return;
			}
			this.isHoldable = value;
			this.OnControlTypeChanged();
		}
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x0000CAB7 File Offset: 0x0000ACB7
	public KeyBindControl()
	{
		base.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.NoTrickleDown);
		base.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnDetachFromPanel), TrickleDown.NoTrickleDown);
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x00038CD0 File Offset: 0x00036ED0
	private void OnAttachToPanel(AttachToPanelEvent e)
	{
		this.Label = this.Query("Label", null).First();
		this.TextField = this.Query("TextField", null).First();
		if (this.TextField != null)
		{
			this.TextField.RegisterCallback<ClickEvent>(new EventCallback<ClickEvent>(this.OnTextFieldClickEvent), TrickleDown.NoTrickleDown);
		}
		this.DropdownField = this.Query("DropdownField", null).First();
		if (this.DropdownField != null)
		{
			this.DropdownField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnDropdownFieldValueChanged));
		}
		this.OnNameLabelChanged();
		this.OnPathLabelChanged();
		this.OnControlTypeChanged();
		this.OnTypeDropdownVisibilityChanged();
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x00006C1B File Offset: 0x00004E1B
	private void OnDetachFromPanel(DetachFromPanelEvent e)
	{
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x0000CAEC File Offset: 0x0000ACEC
	private void OnNameLabelChanged()
	{
		if (this.Label == null)
		{
			return;
		}
		this.Label.text = this.nameLabel;
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x0000CB08 File Offset: 0x0000AD08
	private void OnPathLabelChanged()
	{
		if (this.TextField == null)
		{
			return;
		}
		this.TextField.value = this.PathLabel;
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x0000CB24 File Offset: 0x0000AD24
	private void OnTypeDropdownVisibilityChanged()
	{
		if (this.DropdownField == null)
		{
			return;
		}
		this.DropdownField.style.display = (this.IsTypeDropdownVisible ? DisplayStyle.Flex : DisplayStyle.None);
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x0000CB50 File Offset: 0x0000AD50
	private void OnTextFieldClickEvent(ClickEvent clickEvent)
	{
		Action onClicked = this.OnClicked;
		if (onClicked == null)
		{
			return;
		}
		onClicked();
	}

	// Token: 0x0600092C RID: 2348 RVA: 0x0000CB62 File Offset: 0x0000AD62
	private void OnDropdownFieldValueChanged(ChangeEvent<string> changeEvent)
	{
		Action<string> onTypeDropdownValueChanged = this.OnTypeDropdownValueChanged;
		if (onTypeDropdownValueChanged == null)
		{
			return;
		}
		onTypeDropdownValueChanged(changeEvent.newValue);
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x00038D84 File Offset: 0x00036F84
	private void OnControlTypeChanged()
	{
		if (this.DropdownField == null)
		{
			return;
		}
		if (this.IsPressable)
		{
			this.DropdownField.choices = new List<string>
			{
				"PRESS",
				"RELEASE",
				"DOUBLE PRESS",
				"HOLD"
			};
			this.DropdownField.index = 0;
			return;
		}
		if (this.IsHoldable)
		{
			this.DropdownField.choices = new List<string>
			{
				"CONTINUOUS",
				"TOGGLE"
			};
			this.DropdownField.index = 0;
			return;
		}
		this.DropdownField.choices = new List<string>();
		this.DropdownField.index = -1;
	}

	// Token: 0x04000586 RID: 1414
	private string nameLabel;

	// Token: 0x04000587 RID: 1415
	private string pathLabel;

	// Token: 0x04000588 RID: 1416
	private string typeDropdownValue;

	// Token: 0x04000589 RID: 1417
	private bool isTypeDropdownVisible;

	// Token: 0x0400058A RID: 1418
	private bool isPressable = true;

	// Token: 0x0400058B RID: 1419
	private bool isHoldable;

	// Token: 0x0400058C RID: 1420
	public Action OnClicked;

	// Token: 0x0400058D RID: 1421
	public Action<string> OnTypeDropdownValueChanged;

	// Token: 0x0400058E RID: 1422
	public Label Label;

	// Token: 0x0400058F RID: 1423
	public TextField TextField;

	// Token: 0x04000590 RID: 1424
	public DropdownField DropdownField;

	// Token: 0x02000105 RID: 261
	[CompilerGenerated]
	[Serializable]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		// Token: 0x0600092E RID: 2350 RVA: 0x00038E44 File Offset: 0x00037044
		[RegisterUxmlCache]
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(KeyBindControl.UxmlSerializedData), new UxmlAttributeNames[]
			{
				new UxmlAttributeNames("NameLabel", "name-label", null, Array.Empty<string>()),
				new UxmlAttributeNames("PathLabel", "path-label", null, Array.Empty<string>()),
				new UxmlAttributeNames("TypeDropdownValue", "type-dropdown-value", null, Array.Empty<string>()),
				new UxmlAttributeNames("IsTypeDropdownVisible", "is-type-dropdown-visible", null, Array.Empty<string>()),
				new UxmlAttributeNames("IsPressable", "is-pressable", null, Array.Empty<string>()),
				new UxmlAttributeNames("IsHoldable", "is-holdable", null, Array.Empty<string>())
			});
		}

		// Token: 0x0600092F RID: 2351 RVA: 0x0000CB7A File Offset: 0x0000AD7A
		public override object CreateInstance()
		{
			return new KeyBindControl();
		}

		// Token: 0x06000930 RID: 2352 RVA: 0x00038F10 File Offset: 0x00037110
		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			KeyBindControl keyBindControl = (KeyBindControl)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.NameLabel_UxmlAttributeFlags))
			{
				keyBindControl.NameLabel = this.NameLabel;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.PathLabel_UxmlAttributeFlags))
			{
				keyBindControl.PathLabel = this.PathLabel;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.TypeDropdownValue_UxmlAttributeFlags))
			{
				keyBindControl.TypeDropdownValue = this.TypeDropdownValue;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.IsTypeDropdownVisible_UxmlAttributeFlags))
			{
				keyBindControl.IsTypeDropdownVisible = this.IsTypeDropdownVisible;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.IsPressable_UxmlAttributeFlags))
			{
				keyBindControl.IsPressable = this.IsPressable;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.IsHoldable_UxmlAttributeFlags))
			{
				keyBindControl.IsHoldable = this.IsHoldable;
			}
		}

		// Token: 0x04000591 RID: 1425
		[SerializeField]
		private string NameLabel;

		// Token: 0x04000592 RID: 1426
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags NameLabel_UxmlAttributeFlags;

		// Token: 0x04000593 RID: 1427
		[SerializeField]
		private string PathLabel;

		// Token: 0x04000594 RID: 1428
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags PathLabel_UxmlAttributeFlags;

		// Token: 0x04000595 RID: 1429
		[SerializeField]
		private string TypeDropdownValue;

		// Token: 0x04000596 RID: 1430
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags TypeDropdownValue_UxmlAttributeFlags;

		// Token: 0x04000597 RID: 1431
		[SerializeField]
		private bool IsTypeDropdownVisible;

		// Token: 0x04000598 RID: 1432
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags IsTypeDropdownVisible_UxmlAttributeFlags;

		// Token: 0x04000599 RID: 1433
		[SerializeField]
		private bool IsPressable;

		// Token: 0x0400059A RID: 1434
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags IsPressable_UxmlAttributeFlags;

		// Token: 0x0400059B RID: 1435
		[SerializeField]
		private bool IsHoldable;

		// Token: 0x0400059C RID: 1436
		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags IsHoldable_UxmlAttributeFlags;
	}
}
