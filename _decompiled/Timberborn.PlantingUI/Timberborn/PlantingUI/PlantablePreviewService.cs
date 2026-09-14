using Timberborn.BlockSystem;
using Timberborn.Planting;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.PlantingUI;

public class PlantablePreviewService : ILoadableSingleton, IPostLoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly ITerrainService _terrainService;

	private readonly IBlockService _blockService;

	private readonly PlantablePreviewFactory _plantablePreviewFactory;

	private readonly PlantingService _plantingService;

	private PlantablePreview[,,] _previews;

	public PlantablePreviewService(EventBus eventBus, ITerrainService terrainService, IBlockService blockService, PlantablePreviewFactory plantablePreviewFactory, PlantingService plantingService)
	{
		_eventBus = eventBus;
		_terrainService = terrainService;
		_blockService = blockService;
		_plantablePreviewFactory = plantablePreviewFactory;
		_plantingService = plantingService;
	}

	public void Load()
	{
		_eventBus.Register(this);
		_previews = new PlantablePreview[_terrainService.Size.x, _terrainService.Size.y, _terrainService.Size.z];
	}

	public void PostLoad()
	{
		foreach (Vector3Int plantingCoordinate in _plantingService.PlantingCoordinates)
		{
			string resourceAt = _plantingService.GetResourceAt(plantingCoordinate);
			CreatePreview(resourceAt, plantingCoordinate).Hide();
		}
	}

	public PlantablePreview GetPreview(Vector3Int coordinates)
	{
		return _previews[coordinates.x, coordinates.y, coordinates.z];
	}

	public void ShowPreview(Vector3Int coordinates)
	{
		_previews[coordinates.x, coordinates.y, coordinates.z].Show();
	}

	public void HidePreviews()
	{
		PlantablePreview[,,] previews = _previews;
		int upperBound = previews.GetUpperBound(0);
		int upperBound2 = previews.GetUpperBound(1);
		int upperBound3 = previews.GetUpperBound(2);
		for (int i = previews.GetLowerBound(0); i <= upperBound; i++)
		{
			for (int j = previews.GetLowerBound(1); j <= upperBound2; j++)
			{
				for (int k = previews.GetLowerBound(2); k <= upperBound3; k++)
				{
					HidePreview(previews[i, j, k]);
				}
			}
		}
	}

	public void HidePreview(Vector3Int coordinates)
	{
		if (_terrainService.Contains(coordinates))
		{
			HidePreview(_previews[coordinates.x, coordinates.y, coordinates.z]);
		}
	}

	[OnEvent]
	public void OnPlantingCoordinatesSet(PlantingCoordinatesSetEvent plantingCoordinatesSetEvent)
	{
		CreatePreview(plantingCoordinatesSetEvent.Resource, plantingCoordinatesSetEvent.Coordinates);
	}

	[OnEvent]
	public void OnPlantingCoordinatesUnset(PlantingCoordinatesUnsetEvent plantingCoordinatesUnsetEvent)
	{
		Vector3Int coordinates = plantingCoordinatesUnsetEvent.Coordinates;
		Object.Destroy(_previews[coordinates.x, coordinates.y, coordinates.z]?.GameObject);
		_previews[coordinates.x, coordinates.y, coordinates.z] = null;
	}

	public bool ShouldShowPreview(Vector3Int coordinates)
	{
		Plantable bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<Plantable>(coordinates);
		if (bottomObjectComponentAt != null && !bottomObjectComponentAt.GetComponent<BlockObject>().Overridable)
		{
			string resourceAt = _plantingService.GetResourceAt(coordinates);
			return bottomObjectComponentAt.GetComponent<TemplateSpec>().TemplateName != resourceAt;
		}
		return true;
	}

	private static void HidePreview(PlantablePreview preview)
	{
		if ((bool)preview)
		{
			preview.Hide();
		}
	}

	private PlantablePreview CreatePreview(string resource, Vector3Int coords)
	{
		PlantablePreview plantablePreview = _plantablePreviewFactory.CreatePreview(resource, coords);
		_previews[coords.x, coords.y, coords.z] = plantablePreview;
		if (ShouldShowPreview(coords))
		{
			plantablePreview.Show();
		}
		else
		{
			plantablePreview.Hide();
		}
		return plantablePreview;
	}
}
