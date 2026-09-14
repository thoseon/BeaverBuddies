using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.Planting;

internal class PlantingCoordinatesUnsetter : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly TemplateNameRetriever _templateNameRetriever;

	private readonly TemplateNameMapper _templateNameMapper;

	private readonly PlantingService _plantingService;

	private readonly ITerrainService _terrainService;

	public PlantingCoordinatesUnsetter(EventBus eventBus, TemplateNameRetriever templateNameRetriever, TemplateNameMapper templateNameMapper, PlantingService plantingService, ITerrainService terrainService)
	{
		_eventBus = eventBus;
		_templateNameRetriever = templateNameRetriever;
		_templateNameMapper = templateNameMapper;
		_plantingService = plantingService;
		_terrainService = terrainService;
	}

	[OnEvent]
	public void OnBlockObjectSet(BlockObjectSetEvent blockObjectSetEvent)
	{
		BlockObject blockObject = blockObjectSetEvent.BlockObject;
		if (!blockObject.Overridable && blockObject.HasComponent<TemplateSpec>())
		{
			UnsetCoordinatesIntersectingBlockObject(blockObject);
		}
	}

	public void Load()
	{
		_eventBus.Register(this);
		_terrainService.TerrainHeightChanged += OnTerrainHeightChanged;
	}

	private void UnsetCoordinatesIntersectingBlockObject(BlockObject blockObject)
	{
		string templateName = _templateNameRetriever.GetTemplateName(blockObject);
		foreach (Block occupiedBlock in blockObject.PositionedBlocks.GetOccupiedBlocks())
		{
			UnsetCoordinatesIfBlockIsIntersectingPlantable(occupiedBlock, templateName);
		}
	}

	private void OnTerrainHeightChanged(object sender, TerrainHeightChangeEventArgs terrainHeightChangeEventArgs)
	{
		TerrainHeightChange change = terrainHeightChangeEventArgs.Change;
		if (!change.SetTerrain)
		{
			Vector3Int coordinates = change.Coordinates.ToVector3Int(change.To + 1);
			_plantingService.UnsetPlantingCoordinates(coordinates);
		}
	}

	private void UnsetCoordinatesIfBlockIsIntersectingPlantable(Block block, string templateName)
	{
		Vector3Int coordinates = block.Coordinates;
		string resourceAt = _plantingService.GetResourceAt(coordinates);
		if (resourceAt != null && resourceAt != templateName && IsBlockIntersectingPlantable(block, coordinates, resourceAt))
		{
			_plantingService.UnsetPlantingCoordinates(block.Coordinates);
		}
	}

	private bool IsBlockIntersectingPlantable(Block block, Vector3Int plantableCoordinates, string plantableName)
	{
		return _templateNameMapper.GetTemplate(plantableName).GetSpec<BlockObjectSpec>().GetBlocks(new Placement(plantableCoordinates))
			.Any(((Block)block).IsIntersecting);
	}
}
