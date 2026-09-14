using System;
using System.Collections.Generic;
using Timberborn.AreaSelectionSystemUI;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.GameSound;
using Timberborn.Planting;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainQueryingSystem;
using UnityEngine;

namespace Timberborn.PlantingUI;

public class PlantingSelectionService : ILoadableSingleton
{
	private readonly TerrainAreaService _terrainAreaService;

	private readonly PlantingAreaValidator _plantingAreaValidator;

	private readonly PlantingService _plantingService;

	private readonly ISpecService _specService;

	private readonly GameUISoundController _gameUISoundController;

	private readonly AreaHighlightingService _areaHighlightingService;

	private readonly IBlockService _blockService;

	private readonly PlantablePreviewService _plantablePreviewService;

	private readonly EventBus _eventBus;

	private readonly MeasurableAreaDrawer _measurableAreaDrawer;

	private PlantingSelectionServiceSpec _plantingSelectionServiceSpec;

	public PlantingSelectionService(TerrainAreaService terrainAreaService, PlantingAreaValidator plantingAreaValidator, PlantingService plantingService, ISpecService specService, GameUISoundController gameUISoundController, AreaHighlightingService areaHighlightingService, IBlockService blockService, PlantablePreviewService plantablePreviewService, EventBus eventBus, MeasurableAreaDrawer measurableAreaDrawer)
	{
		_terrainAreaService = terrainAreaService;
		_plantingAreaValidator = plantingAreaValidator;
		_plantingService = plantingService;
		_specService = specService;
		_gameUISoundController = gameUISoundController;
		_areaHighlightingService = areaHighlightingService;
		_blockService = blockService;
		_plantablePreviewService = plantablePreviewService;
		_eventBus = eventBus;
		_measurableAreaDrawer = measurableAreaDrawer;
	}

	public void Load()
	{
		_plantingSelectionServiceSpec = _specService.GetSingleSpec<PlantingSelectionServiceSpec>();
	}

	public void HighlightMarkableArea(IEnumerable<Vector3Int> inputBlocks, Ray ray, string templateName)
	{
		foreach (Vector3Int item in _terrainAreaService.InMapLeveledCoordinates(inputBlocks, ray))
		{
			if (_plantingAreaValidator.CanPlant(item, templateName))
			{
				_areaHighlightingService.DrawTile(item, _plantingSelectionServiceSpec.PlantingToolTile);
				_measurableAreaDrawer.AddMeasurableCoordinates(item);
			}
		}
	}

	public void MarkArea(IEnumerable<Vector3Int> inputBlocks, Ray ray, string templateName)
	{
		if (ActInArea(inputBlocks, ray, (Vector3Int coords) => _plantingAreaValidator.CanPlant(coords, templateName), delegate(Vector3Int coords)
		{
			_plantingService.SetPlantingCoordinates(coords, templateName);
		}))
		{
			_eventBus.Post(new PlantingAreaMarkedEvent());
		}
	}

	public void HighlightUnmarkableArea(IEnumerable<Vector3Int> inputBlocks, Ray ray)
	{
		foreach (Vector3Int item in _terrainAreaService.InMapLeveledCoordinates(inputBlocks, ray))
		{
			_measurableAreaDrawer.AddMeasurableCoordinates(item);
			if (_plantingService.GetResourceAt(item) != null)
			{
				_areaHighlightingService.DrawTile(item, _plantingSelectionServiceSpec.ToolActionTile);
				Highlight(item);
			}
			else
			{
				_areaHighlightingService.DrawTile(item, _plantingSelectionServiceSpec.ToolNoActionTile);
			}
		}
		_areaHighlightingService.Highlight();
	}

	public void UnmarkArea(IEnumerable<Vector3Int> inputBlocks, Ray ray)
	{
		UnhighlightAll();
		ActInArea(inputBlocks, ray, (Vector3Int coords) => _plantingService.IsResourceAt(coords), delegate(Vector3Int coords)
		{
			_plantingService.UnsetPlantingCoordinates(coords);
		});
	}

	public void UnhighlightAll()
	{
		_areaHighlightingService.UnhighlightAll();
	}

	private bool ActInArea(IEnumerable<Vector3Int> inputBlocks, Ray ray, Predicate<Vector3Int> predicate, Action<Vector3Int> action)
	{
		bool flag = false;
		foreach (Vector3Int item in _terrainAreaService.InMapLeveledCoordinates(inputBlocks, ray))
		{
			if (predicate(item))
			{
				action(item);
				flag = true;
			}
		}
		if (flag)
		{
			_gameUISoundController.PlayFieldPlacedSound();
		}
		return flag;
	}

	private void Highlight(Vector3Int coords)
	{
		PlantablePreview preview = _plantablePreviewService.GetPreview(coords);
		if (preview != null && preview.IsShown)
		{
			_areaHighlightingService.AddForHighlight(preview);
			return;
		}
		Plantable bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<Plantable>(coords);
		if (bottomObjectComponentAt != null)
		{
			_areaHighlightingService.AddForHighlight(bottomObjectComponentAt);
		}
	}
}
