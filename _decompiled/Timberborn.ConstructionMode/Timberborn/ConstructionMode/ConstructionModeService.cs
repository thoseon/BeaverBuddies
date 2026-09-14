using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using Timberborn.WaterSystemRendering;

namespace Timberborn.ConstructionMode;

public class ConstructionModeService : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly EntityComponentRegistry _entityComponentRegistry;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly ToolGroupService _toolGroupService;

	private readonly WaterOpacityService _waterOpacityService;

	private WaterOpacityToggle _waterOpacityToggle;

	public bool InConstructionMode { get; private set; }

	private IEnumerable<ConstructionModeModel> ConstructionModeModels => _entityComponentRegistry.GetEnabled<ConstructionModeModel>();

	public ConstructionModeService(EventBus eventBus, EntityComponentRegistry entityComponentRegistry, EntitySelectionService entitySelectionService, ToolGroupService toolGroupService, WaterOpacityService waterOpacityService)
	{
		_eventBus = eventBus;
		_entityComponentRegistry = entityComponentRegistry;
		_entitySelectionService = entitySelectionService;
		_toolGroupService = toolGroupService;
		_waterOpacityService = waterOpacityService;
	}

	public void Load()
	{
		_eventBus.Register(this);
		_waterOpacityToggle = _waterOpacityService.GetWaterOpacityToggle();
	}

	[OnEvent]
	public void OnToolGroupEntered(ToolGroupEnteredEvent toolGroupEnteredEvent)
	{
		ToolGroupSpec toolGroup = toolGroupEnteredEvent.ToolGroup;
		if ((object)toolGroup != null && toolGroup.HasSpec<ConstructionModeToolGroupSpec>())
		{
			EnterConstructionMode();
		}
	}

	[OnEvent]
	public void OnToolGroupExited(ToolGroupExitedEvent toolGroupExitedEvent)
	{
		ToolGroupSpec toolGroup = toolGroupExitedEvent.ToolGroup;
		if ((object)toolGroup != null && toolGroup.HasSpec<ConstructionModeToolGroupSpec>())
		{
			ExitConstructionMode();
		}
	}

	[OnEvent]
	public void OnToolEntered(ToolEnteredEvent toolEnteredEvent)
	{
		if (toolEnteredEvent.Tool is IConstructionModeEnabler)
		{
			EnterConstructionMode();
		}
	}

	[OnEvent]
	public void OnToolExited(ToolExitedEvent toolExitedEvent)
	{
		if (toolExitedEvent.Tool is IConstructionModeEnabler)
		{
			ExitConstructionMode();
		}
	}

	[OnEvent]
	public void OnSelectableObjectSelected(SelectableObjectSelectedEvent selectableObjectSelectedEvent)
	{
		if (IsUnfinished(selectableObjectSelectedEvent.SelectableObject))
		{
			EnterConstructionMode();
		}
	}

	[OnEvent]
	public void OnSelectableObjectUnselected(SelectableObjectUnselectedEvent selectableObjectUnselectedEvent)
	{
		ExitConstructionMode();
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		BlockObject blockObject = enteredFinishedStateEvent.BlockObject;
		if (_entitySelectionService.IsSelected(blockObject.GetComponent<SelectableObject>()))
		{
			ExitConstructionMode();
		}
	}

	[OnEvent]
	public void OnEnteredUnfinishedState(EnteredUnfinishedStateEvent enteredUnfinishedState)
	{
		if (InConstructionMode)
		{
			ConstructionModeModel component = enteredUnfinishedState.BlockObject.GetComponent<ConstructionModeModel>();
			if ((bool)component)
			{
				component.EnterConstructionMode();
			}
		}
	}

	private void EnterConstructionMode()
	{
		if (InConstructionMode)
		{
			return;
		}
		foreach (ConstructionModeModel constructionModeModel in ConstructionModeModels)
		{
			constructionModeModel.EnterConstructionMode();
		}
		ToggleConstructionMode(inConstructionMode: true);
		_waterOpacityToggle.HideWater();
	}

	private void ExitConstructionMode()
	{
		if (!CanExitConstructionMode())
		{
			return;
		}
		foreach (ConstructionModeModel constructionModeModel in ConstructionModeModels)
		{
			constructionModeModel.ExitConstructionMode();
		}
		ToggleConstructionMode(inConstructionMode: false);
		_waterOpacityToggle.ShowWater();
	}

	private bool CanExitConstructionMode()
	{
		SelectableObject selectedObject = _entitySelectionService.SelectedObject;
		bool flag = (bool)selectedObject && IsUnfinished(selectedObject);
		bool flag2 = _toolGroupService.ActiveToolGroup?.HasSpec<ConstructionModeToolGroupSpec>() ?? false;
		if (InConstructionMode && !flag2)
		{
			return !flag;
		}
		return false;
	}

	private static bool IsUnfinished(BaseComponent baseComponent)
	{
		BlockObject component = baseComponent.GetComponent<BlockObject>();
		if ((bool)component)
		{
			return component.IsUnfinished;
		}
		return false;
	}

	private void ToggleConstructionMode(bool inConstructionMode)
	{
		InConstructionMode = inConstructionMode;
		_eventBus.Post(new ConstructionModeChangedEvent(InConstructionMode));
	}
}
