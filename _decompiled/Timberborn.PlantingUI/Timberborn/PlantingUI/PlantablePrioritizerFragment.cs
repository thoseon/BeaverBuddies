using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.Planting;
using UnityEngine.UIElements;

namespace Timberborn.PlantingUI;

internal class PlantablePrioritizerFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private PlantablePrioritizer _plantablePrioritizer;

	private Dropdown _dropdown;

	private VisualElement _root;

	public PlantablePrioritizerFragment(VisualElementLoader visualElementLoader, DropdownItemsSetter dropdownItemsSetter)
	{
		_visualElementLoader = visualElementLoader;
		_dropdownItemsSetter = dropdownItemsSetter;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/PlantablePrioritizerFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_dropdown = _root.Q<Dropdown>("Priorities");
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_plantablePrioritizer = entity.GetComponent<PlantablePrioritizer>();
		if ((bool)_plantablePrioritizer)
		{
			PlantablePrioritizerDropdownProvider component = entity.GetComponent<PlantablePrioritizerDropdownProvider>();
			if (component != null && component.HasMultipleOptions)
			{
				_dropdownItemsSetter.SetLocalizableItems(_dropdown, component);
				_plantablePrioritizer.PrioritizedPlantableChanged += OnPrioritizedPlantableChanged;
				_root.ToggleDisplayStyle(visible: true);
			}
		}
	}

	public void ClearFragment()
	{
		if ((bool)_plantablePrioritizer)
		{
			_plantablePrioritizer.PrioritizedPlantableChanged -= OnPrioritizedPlantableChanged;
		}
		_dropdown.ClearItems();
		_plantablePrioritizer = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
	}

	private void OnPrioritizedPlantableChanged(object sender, EventArgs e)
	{
		_dropdown.UpdateSelectedValue();
	}
}
