using Timberborn.BlockObjectTools;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.Coordinates;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;

namespace Timberborn.GameStartup;

internal class StartingBuildingToolShower : IInputProcessor, ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly InputService _inputService;

	private readonly StartingBuildingInitializer _startingBuildingInitializer;

	private readonly ToolService _toolService;

	private readonly StartingBuildingSpawner _startingBuildingSpawner;

	private readonly StartingBuildingToolFactory _startingBuildingToolFactory;

	private readonly ISettlementNamePromptShower _settlementNamePromptShower;

	private BlockObjectTool _buildingPlacementTool;

	private Placement? _previousBuildingPlacement;

	public StartingBuildingToolShower(EventBus eventBus, InputService inputService, StartingBuildingInitializer startingBuildingInitializer, ToolService toolService, StartingBuildingSpawner startingBuildingSpawner, StartingBuildingToolFactory startingBuildingToolFactory, ISettlementNamePromptShower settlementNamePromptShower)
	{
		_eventBus = eventBus;
		_inputService = inputService;
		_startingBuildingInitializer = startingBuildingInitializer;
		_toolService = toolService;
		_startingBuildingSpawner = startingBuildingSpawner;
		_startingBuildingToolFactory = startingBuildingToolFactory;
		_settlementNamePromptShower = settlementNamePromptShower;
	}

	public void Load()
	{
		PlaceableBlockObjectSpec spec = _startingBuildingSpawner.StartingBuildingTemplateSpec.GetSpec<PlaceableBlockObjectSpec>();
		_buildingPlacementTool = _startingBuildingToolFactory.Create(spec);
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnRelocateSettlement(RelocateSettlementEvent relocateSettlementEvent)
	{
		Building startingBuilding = _startingBuildingSpawner.StartingBuilding;
		if ((bool)startingBuilding)
		{
			BlockObject component = startingBuilding.GetComponent<BlockObject>();
			_previousBuildingPlacement = component.Placement;
			_startingBuildingSpawner.DeleteStartingBuilding();
		}
		_toolService.SwitchTool(_buildingPlacementTool);
		_inputService.AddInputProcessor(this);
	}

	[OnEvent]
	public void OnStartingBuildingPlacedEvent(StartingBuildingPlacedEvent startingBuildingPlacedEvent)
	{
		_inputService.RemoveInputProcessor(this);
		Place(startingBuildingPlacedEvent.Placement);
	}

	[OnEvent]
	public void OnResetStartingLocation(ResetStartingLocationEvent resetStartingLocationEvent)
	{
		_previousBuildingPlacement = null;
		_startingBuildingSpawner.DeleteStartingBuilding();
		Place(_startingBuildingInitializer.InitialPlacement);
		_inputService.RemoveInputProcessor(this);
	}

	public bool ProcessInput()
	{
		if (_inputService.Cancel)
		{
			PlaceStartingBuildingOnPreviousCoordinates();
			_inputService.RemoveInputProcessor(this);
			return true;
		}
		return false;
	}

	private void PlaceStartingBuildingOnPreviousCoordinates()
	{
		Place(_previousBuildingPlacement);
	}

	private void Place(Placement? placement)
	{
		_startingBuildingSpawner.Place(placement);
		bool includeResetStartLocationLink = placement != _startingBuildingInitializer.InitialPlacement;
		_settlementNamePromptShower.PromptDisallowingCancelling(includeResetStartLocationLink);
	}
}
