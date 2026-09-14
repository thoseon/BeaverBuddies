using Timberborn.CoreUI;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.TooltipSystem;

internal class TooltipContainer : ILoadableSingleton, IUpdatableSingleton
{
	private readonly EventBus _eventBus;

	private readonly MouseTooltipPositioner _mouseTooltipPositioner;

	private readonly RootVisualElementProvider _rootVisualElementProvider;

	private VisualElement _root;

	private VisualElement _tooltip;

	private bool _showingPriority;

	public TooltipContainer(EventBus eventBus, MouseTooltipPositioner mouseTooltipPositioner, RootVisualElementProvider rootVisualElementProvider)
	{
		_eventBus = eventBus;
		_mouseTooltipPositioner = mouseTooltipPositioner;
		_rootVisualElementProvider = rootVisualElementProvider;
	}

	public void Load()
	{
		_root = _rootVisualElementProvider.Create("TooltipContainer", "Core/TooltipContainer", 3);
		_tooltip = _root.Q<VisualElement>("TooltipWrapper");
		_tooltip.ToggleDisplayStyle(visible: false);
		_eventBus.Register(this);
	}

	public void UpdateSingleton()
	{
		_mouseTooltipPositioner.UpdatePosition(_tooltip);
		_tooltip.style.visibility = Visibility.Visible;
	}

	[OnEvent]
	public void OnUIVisibilityChanged(UIVisibilityChangedEvent uiVisibilityChangedEvent)
	{
		_root.ToggleDisplayStyle(uiVisibilityChangedEvent.UIVisible);
	}

	public void Show(VisualElement content)
	{
		if (!_showingPriority)
		{
			ShowInternal(content);
		}
	}

	public void ShowPriority(VisualElement content)
	{
		if (!_showingPriority)
		{
			ClearInternal();
			ShowInternal(content);
			_showingPriority = true;
		}
	}

	public void HidePriority()
	{
		if (_showingPriority)
		{
			ClearInternal();
			_showingPriority = false;
		}
	}

	public void Clear()
	{
		if (!_showingPriority)
		{
			ClearInternal();
		}
	}

	private void ShowInternal(VisualElement content)
	{
		_tooltip.Add(content);
		_tooltip.ToggleDisplayStyle(visible: true);
		_tooltip.style.visibility = Visibility.Hidden;
	}

	private void ClearInternal()
	{
		_tooltip.Clear();
		_tooltip.ToggleDisplayStyle(visible: false);
	}
}
