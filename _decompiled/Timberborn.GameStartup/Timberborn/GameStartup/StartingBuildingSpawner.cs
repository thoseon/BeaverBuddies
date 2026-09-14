using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.ConstructionSites;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.GameFactionSystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;

namespace Timberborn.GameStartup;

internal class StartingBuildingSpawner : ILoadableSingleton
{
	private readonly FactionService _factionService;

	private readonly TemplateNameMapper _templateNameMapper;

	private readonly ConstructionFactory _constructionFactory;

	private readonly CameraTargeter _cameraTargeter;

	private readonly EntityService _entityService;

	private readonly StartingGoodsProvider _startingGoodsProvider;

	public Building StartingBuilding { get; private set; }

	public TemplateSpec StartingBuildingTemplateSpec { get; private set; }

	public StartingBuildingSpawner(FactionService factionService, TemplateNameMapper templateNameMapper, ConstructionFactory constructionFactory, CameraTargeter cameraTargeter, EntityService entityService, StartingGoodsProvider startingGoodsProvider)
	{
		_factionService = factionService;
		_templateNameMapper = templateNameMapper;
		_constructionFactory = constructionFactory;
		_cameraTargeter = cameraTargeter;
		_entityService = entityService;
		_startingGoodsProvider = startingGoodsProvider;
	}

	public void Load()
	{
		string startingBuildingId = _factionService.Current.StartingBuildingId;
		StartingBuildingTemplateSpec = _templateNameMapper.GetTemplate(startingBuildingId);
	}

	public void Place(Placement? placement)
	{
		if (placement.HasValue)
		{
			PlaceStartingBuilding(placement.Value);
		}
	}

	public void DeleteStartingBuilding()
	{
		if ((bool)StartingBuilding)
		{
			_startingGoodsProvider.RemoveStartingInventory(StartingBuilding);
			_entityService.Delete(StartingBuilding);
		}
		StartingBuilding = null;
	}

	private void PlaceStartingBuilding(Placement placement)
	{
		Building startingBuilding = CreateStartingBuilding(placement);
		_startingGoodsProvider.AddStartingInventory(startingBuilding);
		StartingBuilding = startingBuilding;
		_cameraTargeter.CenterCameraOn(StartingBuilding.GetComponent<SelectableObject>());
	}

	private Building CreateStartingBuilding(Placement placement)
	{
		EntitySetup.Builder entitySetupBuilder = new EntitySetup.Builder(StartingBuildingTemplateSpec.GetSpec<BlockObjectSpec>().Blueprint);
		return _constructionFactory.CreateAsFinished(entitySetupBuilder, placement).GetComponent<Building>();
	}
}
