using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.DropdownSystem;

[UxmlElement]
public class Dropdown : VisualElement, ILocalizableElement
{
	[Serializable]
	[CompilerGenerated]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		[UxmlAttribute("label-loc-key")]
		[SerializeField]
		private string _labelLocKey;

		[UxmlAttribute("force-label")]
		[SerializeField]
		private bool _forceLabel;

		[UxmlAttribute("buttons-only-selection")]
		[SerializeField]
		private bool _buttonsOnlySelection;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags _labelLocKey_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags _forceLabel_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags _buttonsOnlySelection_UxmlAttributeFlags;

		[RegisterUxmlCache]
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[3]
			{
				new UxmlAttributeNames("_labelLocKey", "label-loc-key", null),
				new UxmlAttributeNames("_forceLabel", "force-label", null),
				new UxmlAttributeNames("_buttonsOnlySelection", "buttons-only-selection", null)
			});
		}

		public override object CreateInstance()
		{
			return new Dropdown();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			Dropdown dropdown = (Dropdown)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(_labelLocKey_UxmlAttributeFlags))
			{
				dropdown._labelLocKey = _labelLocKey;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(_forceLabel_UxmlAttributeFlags))
			{
				dropdown._forceLabel = _forceLabel;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(_buttonsOnlySelection_UxmlAttributeFlags))
			{
				dropdown._buttonsOnlySelection = _buttonsOnlySelection;
			}
		}
	}

	private static readonly string SelectableClass = "dropdown__selectable";

	[UxmlAttribute("label-loc-key")]
	private string _labelLocKey;

	[UxmlAttribute("force-label")]
	private bool _forceLabel;

	[UxmlAttribute("buttons-only-selection")]
	private bool _buttonsOnlySelection;

	private DropdownListDrawer _dropdownListDrawer;

	private ITooltipRegistrar _tooltipRegistrar;

	private Button _selection;

	private VisualElement _selectedItem;

	private readonly List<string> _items = new List<string>();

	private readonly List<VisualElement> _elements = new List<VisualElement>();

	private IDropdownProvider _dropdownProvider;

	private Func<string, bool, DropdownElement> _elementGetter;

	public string HoveredItem { get; private set; }

	public bool IsSet => true;

	public event EventHandler ValueChanged;

	public event EventHandler Showed;

	public Dropdown()
	{
		Resources.Load<VisualTreeAsset>("UI/Views/Core/Dropdown").CloneTree(this);
		this.Q<Label>("Label").text = _labelLocKey;
	}

	public void Initialize(DropdownListDrawer dropdownListDrawer)
	{
		_dropdownListDrawer = dropdownListDrawer;
		_selectedItem = this.Q<VisualElement>("SelectedItemContent");
		_selection = this.Q<Button>("Selection");
		_selection.EnableInClassList(SelectableClass, !_buttonsOnlySelection);
		_selection.RegisterCallback<DetachFromPanelEvent>(delegate
		{
			_dropdownListDrawer.HideDropdown();
		});
		if (_buttonsOnlySelection)
		{
			this.Q<Button>("ArrowDown").RegisterCallback<ClickEvent>(ToggleSelectionListDisplayStyle);
			this.Q<Button>("ArrowLeft").RegisterCallback<ClickEvent>(SelectPrevious);
			this.Q<Button>("ArrowRight").RegisterCallback<ClickEvent>(SelectNext);
		}
		else
		{
			this.Q<Button>("ArrowLeft").ToggleDisplayStyle(visible: false);
			this.Q<Button>("ArrowRight").ToggleDisplayStyle(visible: false);
			_selection.RegisterCallback<ClickEvent>(ToggleSelectionListDisplayStyle);
		}
	}

	public void OverrideLabelLocKey(string newLocKey)
	{
		_labelLocKey = newLocKey;
	}

	public void Localize(ILoc loc)
	{
		Label label = this.Q<Label>("Label");
		if (!string.IsNullOrEmpty(_labelLocKey))
		{
			label.text = loc.T(_labelLocKey);
		}
		else
		{
			label.ToggleDisplayStyle(_forceLabel);
		}
	}

	public void SetItems(IDropdownProvider dropdownProvider, ITooltipRegistrar tooltipRegistrar, Func<string, bool, DropdownElement> elementGetter)
	{
		_tooltipRegistrar = tooltipRegistrar;
		_dropdownProvider = dropdownProvider;
		_elementGetter = elementGetter;
		UpdateSelectedValue();
	}

	public void ClearItems()
	{
		_dropdownProvider = null;
		_elementGetter = null;
		HoveredItem = null;
		_selectedItem.Clear();
		_items.Clear();
		_elements.Clear();
		_dropdownListDrawer.HideDropdown();
	}

	public void UpdateSelectedValue()
	{
		UpdateSelectedValue(_dropdownProvider.GetValue());
	}

	public void Hide()
	{
		_dropdownListDrawer.HideDropdown();
		HoveredItem = null;
	}

	private void ToggleSelectionListDisplayStyle(ClickEvent evt)
	{
		if (_dropdownListDrawer.DropdownVisible)
		{
			Hide();
			return;
		}
		AddItemsIfNeeded();
		_dropdownListDrawer.ShowDropdown(_selection, _elements);
		Showed?.Invoke(this, EventArgs.Empty);
	}

	private void SelectPrevious(ClickEvent evt)
	{
		AddItemsIfNeeded();
		if (_items.Count != 0)
		{
			int num = _items.IndexOf(_dropdownProvider.GetValue()) - 1;
			if (num < 0)
			{
				num = _items.Count - 1;
			}
			SetAndUpdate(_items[num]);
		}
	}

	private void SelectNext(ClickEvent evt)
	{
		AddItemsIfNeeded();
		if (_items.Count != 0)
		{
			int num = _items.IndexOf(_dropdownProvider.GetValue()) + 1;
			if (num >= _items.Count)
			{
				num = 0;
			}
			SetAndUpdate(_items[num]);
		}
	}

	private void AddItemsIfNeeded()
	{
		if (_elements.Count != 0)
		{
			return;
		}
		for (int i = 0; i < _dropdownProvider.Items.Count; i++)
		{
			string item = _dropdownProvider.Items[i];
			DropdownElement dropdownElement = _elementGetter(item, arg2: false);
			dropdownElement.Content.RegisterCallback<ClickEvent>(delegate
			{
				SetAndUpdate(item);
			});
			if (!string.IsNullOrWhiteSpace(dropdownElement.Tooltip))
			{
				_tooltipRegistrar.Register(dropdownElement.Content, dropdownElement.Tooltip);
			}
			_items.Add(item);
			_elements.Add(dropdownElement.Content);
			dropdownElement.Content.RegisterCallback<MouseEnterEvent>(delegate
			{
				HoveredItem = item;
			});
			dropdownElement.Content.RegisterCallback<MouseLeaveEvent>(delegate
			{
				HoveredItem = null;
			});
		}
	}

	private void SetAndUpdate(string newValue)
	{
		if (_dropdownProvider.GetValue() != newValue)
		{
			_dropdownProvider.SetValue(newValue);
			UpdateSelectedValue(newValue);
			ValueChanged?.Invoke(this, EventArgs.Empty);
		}
		_dropdownListDrawer.HideDropdown();
	}

	private void UpdateSelectedValue(string newValue)
	{
		_selectedItem.Clear();
		VisualElement content = _elementGetter(newValue, arg2: true).Content;
		content.SetEnabled(value: false);
		content.AddToClassList("dropdown-item--selected");
		_selectedItem.Add(content);
	}
}
