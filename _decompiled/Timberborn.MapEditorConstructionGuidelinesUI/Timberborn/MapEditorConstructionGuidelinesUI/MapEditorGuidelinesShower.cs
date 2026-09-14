using Timberborn.ConstructionGuidelines;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapEditorConstructionGuidelinesUI;

internal class MapEditorGuidelinesShower : ILoadableSingleton
{
	private readonly ConstructionGuidelinesRenderingService _constructionGuidelinesRenderingService;

	private readonly EventBus _eventBus;

	private ConstructionGuidelinesToggle _constructionGuidelinesToggle;

	private VisualElement _root;

	public MapEditorGuidelinesShower(ConstructionGuidelinesRenderingService constructionGuidelinesRenderingService, EventBus eventBus)
	{
		_constructionGuidelinesRenderingService = constructionGuidelinesRenderingService;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
		_constructionGuidelinesToggle = _constructionGuidelinesRenderingService.GetConstructionGuidelinesToggle();
	}

	[OnEvent]
	public void OnToolEntered(ToolEnteredEvent toolEnteredEvent)
	{
		if (toolEnteredEvent.Tool is IBrushWithGuidelines)
		{
			ChangeGuidelinesVisibility(show: true);
		}
	}

	[OnEvent]
	public void OnToolExited(ToolExitedEvent toolExitedEvent)
	{
		ChangeGuidelinesVisibility(show: false);
	}

	private void ChangeGuidelinesVisibility(bool show)
	{
		if (show)
		{
			_constructionGuidelinesToggle.ShowGuidelines();
		}
		else
		{
			_constructionGuidelinesToggle.HideGuidelines();
		}
	}
}
