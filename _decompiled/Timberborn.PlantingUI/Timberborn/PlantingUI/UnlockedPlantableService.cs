using Timberborn.ScienceSystem;
using Timberborn.SingletonSystem;
using Timberborn.ToolButtonSystem;
using Timberborn.ToolSystem;

namespace Timberborn.PlantingUI;

internal class UnlockedPlantableService : IPostLoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly ToolButtonService _toolButtonService;

	private readonly ToolUnlockingService _toolUnlockingService;

	private readonly UnlockedPlantableGroupsRegistry _unlockedPlantableGroupsRegistry;

	public UnlockedPlantableService(EventBus eventBus, ToolButtonService toolButtonService, ToolUnlockingService toolUnlockingService, UnlockedPlantableGroupsRegistry unlockedPlantableGroupsRegistry)
	{
		_eventBus = eventBus;
		_toolButtonService = toolButtonService;
		_toolUnlockingService = toolUnlockingService;
		_unlockedPlantableGroupsRegistry = unlockedPlantableGroupsRegistry;
	}

	public void PostLoad()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnBuildingUnlocked(BuildingUnlockedEvent buildingUnlockedEvent)
	{
		_unlockedPlantableGroupsRegistry.AddUnlockedPlantableGroups(buildingUnlockedEvent.BuildingSpec);
		UnlockPlantables();
	}

	private void UnlockPlantables()
	{
		foreach (ToolButton toolButton in _toolButtonService.ToolButtons)
		{
			if (toolButton.Tool is PlantingTool plantingTool && _toolUnlockingService.IsLocked(toolButton.Tool) && !_unlockedPlantableGroupsRegistry.IsLocked(plantingTool.PlantableSpec))
			{
				_toolUnlockingService.Unlock(toolButton.Tool);
			}
		}
	}
}
