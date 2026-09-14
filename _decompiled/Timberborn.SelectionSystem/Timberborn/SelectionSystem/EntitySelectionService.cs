using Timberborn.BaseComponentSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.EntityUndoSystem;
using Timberborn.LevelVisibilitySystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.SelectionSystem;

public class EntitySelectionService : ILoadableSingleton, IUpdatableSingleton
{
	private readonly EventBus _eventBus;

	private readonly Highlighter _highlighter;

	private readonly SelectableObjectRetriever _selectableObjectRetriever;

	private readonly CameraTargeter _cameraTargeter;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly ISpecService _specService;

	private Color _entitySelectionColor;

	public bool IsAnythingSelected { get; private set; }

	public SelectableObject SelectedObject { get; private set; }

	private bool SelectedObjectIsDestroyed => !SelectedObject;

	public EntitySelectionService(EventBus eventBus, Highlighter highlighter, SelectableObjectRetriever selectableObjectRetriever, CameraTargeter cameraTargeter, ILevelVisibilityService levelVisibilityService, ISpecService specService)
	{
		_eventBus = eventBus;
		_highlighter = highlighter;
		_selectableObjectRetriever = selectableObjectRetriever;
		_cameraTargeter = cameraTargeter;
		_levelVisibilityService = levelVisibilityService;
		_specService = specService;
	}

	public void Load()
	{
		_eventBus.Register(this);
		_entitySelectionColor = _specService.GetSingleSpec<SelectionColorsSpec>().EntitySelection;
	}

	public void UpdateSingleton()
	{
		if (IsAnythingSelected && !SelectedObjectIsDestroyed)
		{
			HighlightSelectedObject();
		}
	}

	[OnEvent]
	public void OnEntityDeleted(EntityDeletedEvent entityDeletedEvent)
	{
		Unselect(entityDeletedEvent.Entity.GetComponent<SelectableObject>());
	}

	[OnEvent]
	public void OnUndoableEntityChanged(UndoableEntityChangedEvent undoableEntityChangedEvent)
	{
		Select(undoableEntityChangedEvent.Entity);
	}

	public void Select(BaseComponent target)
	{
		if (IsSelectable(target))
		{
			SelectableObject selectableObject = _selectableObjectRetriever.GetSelectableObject(target);
			SelectSelectable(selectableObject);
		}
	}

	public void SelectAndFollow(BaseComponent target)
	{
		if (IsSelectable(target))
		{
			SelectableObject selectableObject = _selectableObjectRetriever.GetSelectableObject(target);
			UpdateVisibleLayer(selectableObject);
			SelectSelectable(selectableObject);
			FollowSelectable(selectableObject);
		}
	}

	public void SelectAndFocusOn(BaseComponent target)
	{
		if (IsSelectable(target))
		{
			SelectableObject selectableObject = _selectableObjectRetriever.GetSelectableObject(target);
			UpdateVisibleLayer(selectableObject);
			SelectSelectable(selectableObject);
			FocusOnSelectable(selectableObject);
		}
	}

	public void UnselectAndFollow(BaseComponent target)
	{
		Unselect();
		if (IsSelectable(target))
		{
			SelectableObject selectableObject = _selectableObjectRetriever.GetSelectableObject(target);
			FollowSelectable(selectableObject);
		}
	}

	public void Unselect()
	{
		if (IsAnythingSelected)
		{
			if (!SelectedObjectIsDestroyed)
			{
				_highlighter.UnhighlightAllPrimary();
				SelectedObject.OnUnselect();
			}
			SelectableObject selectedObject = SelectedObject;
			SelectedObject = null;
			IsAnythingSelected = false;
			_eventBus.Post(new SelectableObjectUnselectedEvent(selectedObject));
		}
	}

	public void Replace(SelectableObject oldTarget, SelectableObject newTarget)
	{
		if (IsSelected(oldTarget))
		{
			if (IsFollowed(oldTarget))
			{
				FollowSelectable(newTarget);
			}
			SelectSelectable(newTarget);
		}
	}

	public bool IsSelected(SelectableObject target)
	{
		if (IsAnythingSelected)
		{
			return SelectedObject == target;
		}
		return false;
	}

	public void UnhighlightUntilNextUpdate()
	{
		if (IsAnythingSelected)
		{
			_highlighter.UnhighlightAllPrimary();
		}
	}

	private static bool IsSelectable(BaseComponent target)
	{
		if ((bool)target && target.HasComponent<EntityComponent>())
		{
			return !target.GetComponent<EntityComponent>().Deleted;
		}
		return false;
	}

	private void Unselect(SelectableObject target)
	{
		if (IsSelected(target))
		{
			Unselect();
		}
	}

	private void SelectSelectable(SelectableObject target)
	{
		if (SelectedObject != target)
		{
			Unselect();
			SelectedObject = target;
			IsAnythingSelected = true;
			HighlightSelectedObject();
			_eventBus.Post(new SelectableObjectSelectedEvent(SelectedObject));
			target.OnSelect();
		}
	}

	private void UpdateVisibleLayer(SelectableObject target)
	{
		int z = CoordinateSystem.WorldToGridInt(target.Transform.position).z;
		if (_levelVisibilityService.MaxVisibleLevel < z)
		{
			_levelVisibilityService.SetMaxVisibleLevel(z);
		}
	}

	private void FollowSelectable(SelectableObject selectableObject)
	{
		_cameraTargeter.Follow(selectableObject);
	}

	private void FocusOnSelectable(SelectableObject selectableObject)
	{
		_cameraTargeter.CenterCameraOn(selectableObject);
	}

	private bool IsFollowed(SelectableObject target)
	{
		SelectableObject followedTarget = _cameraTargeter.FollowedTarget;
		if ((bool)followedTarget)
		{
			return followedTarget == target;
		}
		return false;
	}

	private void HighlightSelectedObject()
	{
		_highlighter.HighlightPrimary(SelectedObject, _entitySelectionColor);
	}
}
