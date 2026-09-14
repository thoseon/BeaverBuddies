using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.Localization;
using Timberborn.Planting;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.PlantingUI;

public class PlantablePrioritizerBatchControlRowItemFactory
{
	private static readonly string PlantingPrioritizeLocKey = "Planting.Prioritize";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly ILoc _loc;

	public PlantablePrioritizerBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, DropdownItemsSetter dropdownItemsSetter, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_dropdownItemsSetter = dropdownItemsSetter;
		_loc = loc;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		PlantablePrioritizerDropdownProvider dropdownProvider = entity.GetComponent<PlantablePrioritizerDropdownProvider>();
		if (dropdownProvider != null && dropdownProvider.HasMultipleOptions)
		{
			string elementName = "Game/BatchControl/DropdownBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			Dropdown dropdown = visualElement.Q<Dropdown>("Dropdown");
			_dropdownItemsSetter.SetLocalizableItems(dropdown, dropdownProvider);
			_tooltipRegistrar.Register(dropdown, () => GetTooltipText(dropdownProvider));
			PlantablePrioritizer component = entity.GetComponent<PlantablePrioritizer>();
			return new PlantablePrioritizerBatchControlRowItem(visualElement, dropdown, component);
		}
		return null;
	}

	private string GetTooltipText(PlantablePrioritizerDropdownProvider dropdownProvider)
	{
		string text = _loc.T(PlantingPrioritizeLocKey);
		string text2 = _loc.T(dropdownProvider.GetValue());
		return text + " " + text2;
	}
}
