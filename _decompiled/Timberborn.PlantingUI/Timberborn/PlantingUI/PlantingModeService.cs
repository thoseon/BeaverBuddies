using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.Growing;
using Timberborn.LevelVisibilitySystem;
using Timberborn.Planting;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using UnityEngine;

namespace Timberborn.PlantingUI;

public class PlantingModeService : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly PlantingService _plantingService;

	private readonly PlantablePreviewService _plantablePreviewService;

	private readonly ToolGroupService _toolGroupService;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private bool _inPlantingMode;

	public PlantingModeService(EventBus eventBus, PlantingService plantingService, PlantablePreviewService plantablePreviewService, ToolGroupService toolGroupService, ILevelVisibilityService levelVisibilityService)
	{
		_eventBus = eventBus;
		_plantingService = plantingService;
		_plantablePreviewService = plantablePreviewService;
		_toolGroupService = toolGroupService;
		_levelVisibilityService = levelVisibilityService;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnToolGroupEntered(ToolGroupEnteredEvent toolGroupEnteredEvent)
	{
		if (IsPlantingToolGroup(toolGroupEnteredEvent.ToolGroup))
		{
			EnterPlantingMode();
		}
	}

	[OnEvent]
	public void OnToolGroupExited(ToolGroupExitedEvent toolGroupExitedEvent)
	{
		if (IsPlantingToolGroup(toolGroupExitedEvent.ToolGroup))
		{
			ExitPlantingMode();
		}
	}

	[OnEvent]
	public void OnEntityInitialized(EntityInitializedEvent entityInitializedEvent)
	{
		BlockObject component = entityInitializedEvent.Entity.GetComponent<BlockObject>();
		if (component == null || component.Overridable || !component.GetComponent<Growable>())
		{
			return;
		}
		foreach (Vector3Int foundationCoordinate in component.PositionedBlocks.GetFoundationCoordinates())
		{
			_plantablePreviewService.HidePreview(foundationCoordinate);
		}
	}

	[OnEvent]
	public void OnToolEntered(ToolEnteredEvent toolEnteredEvent)
	{
		if (toolEnteredEvent.Tool is PlantingTool)
		{
			EnterPlantingMode();
		}
	}

	[OnEvent]
	public void OnToolExited(ToolExitedEvent toolExitedEvent)
	{
		if (toolExitedEvent.Tool is PlantingTool && !IsPlantingToolGroup(_toolGroupService.ActiveToolGroup))
		{
			ExitPlantingMode();
		}
	}

	[OnEvent]
	public void OnMaxVisibleLevelChanged(MaxVisibleLevelChangedEvent maxVisibleLevelChangedEvent)
	{
		if (!_inPlantingMode)
		{
			return;
		}
		foreach (Vector3Int plantingCoordinate in _plantingService.PlantingCoordinates)
		{
			if (_plantablePreviewService.ShouldShowPreview(plantingCoordinate) && plantingCoordinate.z <= _levelVisibilityService.MaxVisibleLevel)
			{
				_plantablePreviewService.ShowPreview(plantingCoordinate);
			}
			else
			{
				_plantablePreviewService.HidePreview(plantingCoordinate);
			}
		}
	}

	private void EnterPlantingMode()
	{
		if (_inPlantingMode)
		{
			return;
		}
		foreach (Vector3Int plantingCoordinate in _plantingService.PlantingCoordinates)
		{
			if (_plantablePreviewService.ShouldShowPreview(plantingCoordinate) && plantingCoordinate.z <= _levelVisibilityService.MaxVisibleLevel)
			{
				_plantablePreviewService.ShowPreview(plantingCoordinate);
			}
		}
		_inPlantingMode = true;
	}

	private void ExitPlantingMode()
	{
		if (_inPlantingMode)
		{
			_plantablePreviewService.HidePreviews();
			_inPlantingMode = false;
		}
	}

	private static bool IsPlantingToolGroup(ToolGroupSpec toolGroupSpec)
	{
		return toolGroupSpec?.HasSpec<PlantingToolGroupSpec>() ?? false;
	}
}
