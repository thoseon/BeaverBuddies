using System.Linq;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.Planting;
using Timberborn.SelectionToolSystem;
using Timberborn.TemplateSystem;
using Timberborn.ToolButtonSystem;
using Timberborn.ToolSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.PlantingUI;

public class PlantingToolButtonFactory
{
	private static readonly string CancelPlantingImageKey = "CancelToolIcon";

	private readonly ToolButtonFactory _toolButtonFactory;

	private readonly PlantableDescriber _plantableDescriber;

	private readonly PlantingSelectionService _plantingSelectionService;

	private readonly DevModePlantableSpawner _devModePlantableSpawner;

	private readonly ToolUnlockingService _toolUnlockingService;

	private readonly SelectionToolProcessorFactory _selectionToolProcessorFactory;

	private readonly ILoc _loc;

	private readonly TemplateService _templateService;

	public PlantingToolButtonFactory(ToolButtonFactory toolButtonFactory, PlantableDescriber plantableDescriber, PlantingSelectionService plantingSelectionService, DevModePlantableSpawner devModePlantableSpawner, ToolUnlockingService toolUnlockingService, SelectionToolProcessorFactory selectionToolProcessorFactory, ILoc loc, TemplateService templateService)
	{
		_toolButtonFactory = toolButtonFactory;
		_plantableDescriber = plantableDescriber;
		_plantingSelectionService = plantingSelectionService;
		_devModePlantableSpawner = devModePlantableSpawner;
		_toolUnlockingService = toolUnlockingService;
		_selectionToolProcessorFactory = selectionToolProcessorFactory;
		_loc = loc;
		_templateService = templateService;
	}

	public ToolButton CreatePlantingTool(PlantableSpec plantableSpec, VisualElement buttonParent)
	{
		PlantingTool tool = new PlantingTool(_plantableDescriber, _plantingSelectionService, _devModePlantableSpawner, _toolUnlockingService, _selectionToolProcessorFactory, plantableSpec, GetPlanterBuildingName(plantableSpec));
		Sprite asset = plantableSpec.GetSpec<LabeledEntitySpec>().Icon.Asset;
		return _toolButtonFactory.Create(tool, asset, buttonParent);
	}

	public ToolButton CreateCancelTool(VisualElement buttonParent)
	{
		CancelPlantingTool tool = new CancelPlantingTool(_plantingSelectionService, _loc, _selectionToolProcessorFactory);
		return _toolButtonFactory.Create(tool, CancelPlantingImageKey, buttonParent);
	}

	private string GetPlanterBuildingName(PlantableSpec plantableSpec)
	{
		string displayNameLocKey = _templateService.GetAll<PlanterBuildingSpec>().Single((PlanterBuildingSpec building) => building.PlantableResourceGroup == plantableSpec.ResourceGroup).GetSpec<LabeledEntitySpec>()
			.DisplayNameLocKey;
		return _loc.T(displayNameLocKey);
	}
}
