using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.Gathering;
using UnityEngine.UIElements;

namespace Timberborn.GatheringUI;

internal class GatherablePrioritizerFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private GatherablePrioritizer _gatherablePrioritizer;

	private Dropdown _dropdown;

	private VisualElement _root;

	public GatherablePrioritizerFragment(VisualElementLoader visualElementLoader, DropdownItemsSetter dropdownItemsSetter)
	{
		_visualElementLoader = visualElementLoader;
		_dropdownItemsSetter = dropdownItemsSetter;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/GatherablePrioritizerFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_dropdown = _root.Q<Dropdown>("Priorities");
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_gatherablePrioritizer = entity.GetComponent<GatherablePrioritizer>();
		if ((bool)_gatherablePrioritizer)
		{
			GatherablePrioritizerDropdownProvider component = entity.GetComponent<GatherablePrioritizerDropdownProvider>();
			if (component != null && component.HasMultipleOptions)
			{
				_dropdownItemsSetter.SetItems(_dropdown, component);
				_gatherablePrioritizer.PrioritizedGatherableChanged += OnPrioritizedGatherableChanged;
				_root.ToggleDisplayStyle(visible: true);
			}
		}
	}

	public void ClearFragment()
	{
		if ((bool)_gatherablePrioritizer)
		{
			_gatherablePrioritizer.PrioritizedGatherableChanged -= OnPrioritizedGatherableChanged;
		}
		_dropdown.ClearItems();
		_gatherablePrioritizer = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
	}

	private void OnPrioritizedGatherableChanged(object sender, EventArgs e)
	{
		_dropdown.UpdateSelectedValue();
	}
}
