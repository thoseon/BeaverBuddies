using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.Gathering;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.GatheringUI;

public class GatherablePrioritizerBatchControlRowItemFactory
{
	private static readonly string GatheringPrioritizeLocKey = "Gathering.Prioritize";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly ILoc _loc;

	public GatherablePrioritizerBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, DropdownItemsSetter dropdownItemsSetter, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_dropdownItemsSetter = dropdownItemsSetter;
		_loc = loc;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		GatherablePrioritizerDropdownProvider dropdownProvider = entity.GetComponent<GatherablePrioritizerDropdownProvider>();
		if (dropdownProvider != null && dropdownProvider.HasMultipleOptions)
		{
			string elementName = "Game/BatchControl/DropdownBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			Dropdown dropdown = visualElement.Q<Dropdown>("Dropdown");
			_dropdownItemsSetter.SetItems(dropdown, dropdownProvider);
			_tooltipRegistrar.Register(dropdown, () => GetTooltipText(dropdownProvider));
			GatherablePrioritizer component = entity.GetComponent<GatherablePrioritizer>();
			return new GatherablePrioritizerBatchControlRowItem(visualElement, dropdown, component);
		}
		return null;
	}

	private string GetTooltipText(GatherablePrioritizerDropdownProvider dropdownProvider)
	{
		string text = _loc.T(GatheringPrioritizeLocKey);
		string value = dropdownProvider.GetValue();
		return text + " " + value;
	}
}
