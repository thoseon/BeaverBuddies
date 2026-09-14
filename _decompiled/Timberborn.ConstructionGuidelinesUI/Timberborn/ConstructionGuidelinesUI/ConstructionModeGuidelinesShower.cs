using Timberborn.ConstructionGuidelines;
using Timberborn.ConstructionMode;
using Timberborn.SingletonSystem;

namespace Timberborn.ConstructionGuidelinesUI;

public class ConstructionModeGuidelinesShower : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly ConstructionModeService _constructionModeService;

	private readonly ConstructionGuidelinesRenderingService _constructionGuidelinesRenderingService;

	private ConstructionGuidelinesToggle _constructionGuidelinesToggle;

	public ConstructionModeGuidelinesShower(EventBus eventBus, ConstructionModeService constructionModeService, ConstructionGuidelinesRenderingService constructionGuidelinesRenderingService)
	{
		_eventBus = eventBus;
		_constructionModeService = constructionModeService;
		_constructionGuidelinesRenderingService = constructionGuidelinesRenderingService;
	}

	public void Load()
	{
		_eventBus.Register(this);
		_constructionGuidelinesToggle = _constructionGuidelinesRenderingService.GetConstructionGuidelinesToggle();
	}

	[OnEvent]
	public void OnConstructionModeChanged(ConstructionModeChangedEvent constructionModeChangedEvent)
	{
		if (_constructionModeService.InConstructionMode)
		{
			_constructionGuidelinesToggle.ShowGuidelines();
		}
		else
		{
			_constructionGuidelinesToggle.HideGuidelines();
		}
	}
}
