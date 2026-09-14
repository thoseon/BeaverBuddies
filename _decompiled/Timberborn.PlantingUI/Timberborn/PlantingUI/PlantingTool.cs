using System.Collections.Generic;
using System.Linq;
using Timberborn.Planting;
using Timberborn.SelectionToolSystem;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;
using UnityEngine;

namespace Timberborn.PlantingUI;

public class PlantingTool : ITool, IToolDescriptor
{
	private static readonly string CursorKey = "PlantingCursor";

	private readonly PlantableDescriber _plantableDescriber;

	private readonly PlantingSelectionService _plantingSelectionService;

	private readonly DevModePlantableSpawner _devModePlantableSpawner;

	private readonly ToolUnlockingService _toolUnlockingService;

	private readonly SelectionToolProcessor _selectionToolProcessor;

	public PlantableSpec PlantableSpec { get; }

	public string BuildingName { get; }

	public PlantingTool(PlantableDescriber plantableDescriber, PlantingSelectionService plantingSelectionService, DevModePlantableSpawner devModePlantableSpawner, ToolUnlockingService toolUnlockingService, SelectionToolProcessorFactory selectionToolProcessorFactory, PlantableSpec plantableSpec, string buildingName)
	{
		_plantableDescriber = plantableDescriber;
		_plantingSelectionService = plantingSelectionService;
		_devModePlantableSpawner = devModePlantableSpawner;
		_toolUnlockingService = toolUnlockingService;
		PlantableSpec = plantableSpec;
		BuildingName = buildingName;
		_selectionToolProcessor = selectionToolProcessorFactory.Create(PreviewCallback, ActionCallback, ShowNoneCallback, CursorKey);
	}

	public void Enter()
	{
		_selectionToolProcessor.Enter();
	}

	public void Exit()
	{
		_selectionToolProcessor.Exit();
	}

	public ToolDescription DescribeTool()
	{
		return _plantableDescriber.Describe(PlantableSpec, BuildingName);
	}

	private void PreviewCallback(IEnumerable<Vector3Int> inputBlocks, Ray ray)
	{
		_plantingSelectionService.HighlightMarkableArea(inputBlocks, ray, PlantableSpec.TemplateName);
	}

	private void ActionCallback(IEnumerable<Vector3Int> inputBlocks, Ray ray)
	{
		if (_toolUnlockingService.IsLocked(this))
		{
			_toolUnlockingService.TryToUnlock(this, delegate
			{
				Plant(inputBlocks, ray);
			}, delegate
			{
			});
		}
		else
		{
			Plant(inputBlocks, ray);
		}
	}

	private static void ShowNoneCallback()
	{
	}

	private void Plant(IEnumerable<Vector3Int> inputBlocks, Ray ray)
	{
		List<Vector3Int> list = inputBlocks.ToList();
		string templateName = PlantableSpec.TemplateName;
		_plantingSelectionService.MarkArea(list, ray, templateName);
		_devModePlantableSpawner.SpawnPlantables(list, templateName);
	}
}
