using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using Timberborn.Workshops;
using UnityEngine.UIElements;

namespace Timberborn.WorkshopsUI;

public class ManufactoryBatchControlRowItemFactory
{
	private static readonly string CurrentRecipeLocKey = "Manufactory.CurrentRecipe";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly ILoc _loc;

	public ManufactoryBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, DropdownItemsSetter dropdownItemsSetter, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_dropdownItemsSetter = dropdownItemsSetter;
		_loc = loc;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		Manufactory component = entity.GetComponent<Manufactory>();
		if (component != null && component.ProductionRecipes.Length > 1 && !entity.GetComponent<ManufactoryTogglableRecipes>())
		{
			string elementName = "Game/BatchControl/DropdownBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			Dropdown dropdown = visualElement.Q<Dropdown>("Dropdown");
			ManufactoryDropdownProvider manufactoryDropdownProvider = component.GetComponent<ManufactoryDropdownProvider>();
			_dropdownItemsSetter.SetLocalizableItems(dropdown, manufactoryDropdownProvider);
			_tooltipRegistrar.Register(dropdown, () => GetTooltipText(manufactoryDropdownProvider));
			return new ManufactoryBatchControlRowItem(visualElement, dropdown, component);
		}
		return null;
	}

	private string GetTooltipText(ManufactoryDropdownProvider manufactoryDropdownProvider)
	{
		string text = _loc.T(CurrentRecipeLocKey);
		string text2 = _loc.T(manufactoryDropdownProvider.GetValue());
		return text + " " + text2;
	}
}
