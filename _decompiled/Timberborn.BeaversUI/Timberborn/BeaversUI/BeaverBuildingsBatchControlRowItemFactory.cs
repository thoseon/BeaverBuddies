using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.DwellingSystem;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using Timberborn.TooltipSystem;
using Timberborn.WorkSystem;
using UnityEngine.UIElements;

namespace Timberborn.BeaversUI;

public class BeaverBuildingsBatchControlRowItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly EntitySelectionService _entitySelectionService;

	public BeaverBuildingsBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ILoc loc, ITooltipRegistrar tooltipRegistrar, EntitySelectionService entitySelectionService)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_tooltipRegistrar = tooltipRegistrar;
		_entitySelectionService = entitySelectionService;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		Dweller component = entity.GetComponent<Dweller>();
		Worker component2 = entity.GetComponent<Worker>();
		if ((bool)component || (bool)(BaseComponent)(object)component2)
		{
			string elementName = "Game/BatchControl/BeaverBuildingsBatchControlRowItem";
			VisualElement root = _visualElementLoader.LoadVisualElement(elementName);
			return CreateRow(root, component, component2);
		}
		return null;
	}

	private BeaverBuildingsBatchControlRowItem CreateRow(VisualElement root, Dweller dweller, Worker worker)
	{
		BeaverBuildingsBatchControlRowItem beaverBuildingsBatchControlRowItem = new BeaverBuildingsBatchControlRowItem(root, _tooltipRegistrar, _loc, _entitySelectionService, dweller, root.Q<Button>("HomeButton"), root.Q<Image>("HomeIcon"), worker, root.Q<Button>("WorkplaceButton"), root.Q<Image>("WorkplaceIcon"));
		beaverBuildingsBatchControlRowItem.Initialize();
		return beaverBuildingsBatchControlRowItem;
	}
}
