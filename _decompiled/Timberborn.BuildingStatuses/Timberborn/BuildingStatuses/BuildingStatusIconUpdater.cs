using Timberborn.ConstructionMode;
using Timberborn.LevelVisibilitySystem;
using Timberborn.SingletonSystem;
using Timberborn.StatusSystem;

namespace Timberborn.BuildingStatuses;

internal class BuildingStatusIconUpdater : ILoadableSingleton, IUpdatableSingleton
{
	private readonly EventBus _eventBus;

	private readonly IStatusIconOffsetService _statusIconOffsetService;

	private bool _updateNextFrame;

	public BuildingStatusIconUpdater(EventBus eventBus, IStatusIconOffsetService statusIconOffsetService)
	{
		_eventBus = eventBus;
		_statusIconOffsetService = statusIconOffsetService;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnConstructionModeChanged(ConstructionModeChangedEvent constructionModeChangedEvent)
	{
		if (constructionModeChangedEvent.InConstructionMode)
		{
			_statusIconOffsetService.EnablePreviewMode();
		}
		else
		{
			_statusIconOffsetService.DisablePreviewMode();
		}
		_updateNextFrame = true;
	}

	[OnEvent]
	public void OnMaxVisibleLevelChanged(MaxVisibleLevelChangedEvent maxVisibleLevelChangedEvent)
	{
		_updateNextFrame = true;
	}

	public void UpdateSingleton()
	{
		if (_updateNextFrame)
		{
			_statusIconOffsetService.RepositionAllIcons();
			_updateNextFrame = false;
		}
	}
}
