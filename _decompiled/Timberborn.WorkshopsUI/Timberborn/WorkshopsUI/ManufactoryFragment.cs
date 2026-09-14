using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.TooltipSystem;
using Timberborn.Workshops;
using UnityEngine.UIElements;

namespace Timberborn.WorkshopsUI;

internal class ManufactoryFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private Manufactory _manufactory;

	private ManufactoryTogglableRecipes _manufactoryTogglableRecipes;

	private ManufactoryDropdownProvider _manufactoryDropdownProvider;

	private Dropdown _dropdown;

	private VisualElement _root;

	private bool _isAutomaticRecipeManufactory;

	private bool Visible
	{
		get
		{
			if (!_manufactoryTogglableRecipes && (bool)_manufactory)
			{
				return _manufactory.ProductionRecipes.Length > 1;
			}
			return false;
		}
	}

	public ManufactoryFragment(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, DropdownItemsSetter dropdownItemsSetter)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_dropdownItemsSetter = dropdownItemsSetter;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/ManufactoryFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_dropdown = _root.Q<Dropdown>("Recipes");
		_tooltipRegistrar.RegisterLocalizable(_dropdown, () => _manufactoryDropdownProvider.GetValue());
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_manufactory = entity.GetComponent<Manufactory>();
		_manufactoryTogglableRecipes = entity.GetComponent<ManufactoryTogglableRecipes>();
		_isAutomaticRecipeManufactory = entity.GetComponent<AutomaticRecipeManufactory>();
		if (Visible && !_isAutomaticRecipeManufactory)
		{
			_manufactory.RecipeChanged += OnProductionRecipeChanged;
			_manufactoryDropdownProvider = _manufactory.GetComponent<ManufactoryDropdownProvider>();
			_root.ToggleDisplayStyle(visible: true);
			_dropdownItemsSetter.SetLocalizableItems(_dropdown, _manufactoryDropdownProvider);
		}
	}

	public void UpdateFragment()
	{
		_root.ToggleDisplayStyle(Visible && !_isAutomaticRecipeManufactory);
	}

	public void ClearFragment()
	{
		if (Visible)
		{
			_manufactory.RecipeChanged -= OnProductionRecipeChanged;
		}
		_dropdown.ClearItems();
		_manufactory = null;
		_manufactoryTogglableRecipes = null;
		_manufactoryDropdownProvider = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	private void OnProductionRecipeChanged(object sender, EventArgs e)
	{
		_dropdown.UpdateSelectedValue();
	}
}
