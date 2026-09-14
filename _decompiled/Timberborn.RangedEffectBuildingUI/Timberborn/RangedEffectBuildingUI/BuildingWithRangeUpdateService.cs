using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BuildingRange;
using Timberborn.ConstructionMode;
using Timberborn.EntitySystem;
using Timberborn.LevelVisibilitySystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.RangedEffectBuildingUI;

public class BuildingWithRangeUpdateService : ILoadableSingleton
{
	private readonly RangeTileMarkerService _rangeTileMarkerService;

	private readonly RangeObjectHighlighterService _rangeObjectHighlighterService;

	private readonly EventBus _eventBus;

	private IBuildingWithRange _selectedBuilding;

	private IBuildingWithRange _previewBuilding;

	public BuildingWithRangeUpdateService(RangeTileMarkerService rangeTileMarkerService, RangeObjectHighlighterService rangeObjectHighlighterService, EventBus eventBus)
	{
		_rangeTileMarkerService = rangeTileMarkerService;
		_rangeObjectHighlighterService = rangeObjectHighlighterService;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnEntityInitialized(EntityInitializedEvent entityInitializedEvent)
	{
		if (TryGetBuildingWithRange(entityInitializedEvent.Entity, out var buildingWithRange))
		{
			_rangeTileMarkerService.AddBuildingWithRange(buildingWithRange);
			_rangeObjectHighlighterService.AddBuildingWithObjectRange(buildingWithRange);
			if (IsSameRangeActive(buildingWithRange))
			{
				RecalculateArea(buildingWithRange);
			}
		}
	}

	[OnEvent]
	public void OnEntityDeleted(EntityDeletedEvent entityDeletedEvent)
	{
		if (TryGetBuildingWithRange(entityDeletedEvent.Entity, out var buildingWithRange))
		{
			_rangeTileMarkerService.RemoveBuildingWithRange(buildingWithRange);
			_rangeObjectHighlighterService.RemoveBuildingWithObjectRange(buildingWithRange);
		}
	}

	[OnEvent]
	public void OnSelectableObjectSelected(SelectableObjectSelectedEvent selectableObjectSelectedEvent)
	{
		if (TryGetBuildingWithRange(selectableObjectSelectedEvent.SelectableObject, out var buildingWithRange))
		{
			_selectedBuilding = buildingWithRange;
			Show(buildingWithRange);
		}
	}

	[OnEvent]
	public void OnSelectableObjectUnselected(SelectableObjectUnselectedEvent selectableObjectUnselectedEvent)
	{
		if (_selectedBuilding != null)
		{
			_rangeObjectHighlighterService.ClearHighlights();
			_rangeTileMarkerService.HideArea();
			_selectedBuilding = null;
		}
	}

	[OnEvent]
	public void OnMaxVisibleLevelChanged(MaxVisibleLevelChangedEvent maxVisibleLevelChangedEvent)
	{
		RecalculateArea();
	}

	[OnEvent]
	public void OnConstructionModeChanged(ConstructionModeChangedEvent constructionModeChangedEvent)
	{
		RecalculateArea();
	}

	public void AddPreview(IBuildingWithRange buildingWithRange, Preview preview)
	{
		_rangeTileMarkerService.AddPreviewBuildingWithRange(buildingWithRange, preview);
		_rangeObjectHighlighterService.AddPreviewBuildingWithObjectRange(buildingWithRange);
		_previewBuilding = buildingWithRange;
		Show(buildingWithRange);
	}

	public void RemovePreview()
	{
		if (_previewBuilding != null)
		{
			_rangeObjectHighlighterService.RemovePreviewBuildingWithObjectRange();
			_rangeObjectHighlighterService.ClearHighlights();
			_rangeTileMarkerService.RemovePreviewBuildingWithRange();
			_rangeTileMarkerService.HideArea();
			_previewBuilding = null;
		}
	}

	public void DrawArea()
	{
		_rangeTileMarkerService.DrawArea();
		_rangeObjectHighlighterService.HighlightObjects();
	}

	private static bool TryGetBuildingWithRange(BaseComponent component, out IBuildingWithRange buildingWithRange)
	{
		buildingWithRange = component.GetComponent<IBuildingWithRange>();
		return buildingWithRange != null;
	}

	private void Show(IBuildingWithRange buildingWithRange)
	{
		_rangeTileMarkerService.ShowArea();
		RecalculateArea(buildingWithRange);
		DrawArea();
	}

	private bool IsSameRangeActive(IBuildingWithRange buildingWithRange)
	{
		if (_previewBuilding == null || !(_previewBuilding.RangeName == buildingWithRange.RangeName))
		{
			if (_selectedBuilding != null)
			{
				return _selectedBuilding.RangeName == buildingWithRange.RangeName;
			}
			return false;
		}
		return true;
	}

	private void RecalculateArea()
	{
		if (_previewBuilding != null || _selectedBuilding != null)
		{
			RecalculateArea(_previewBuilding ?? _selectedBuilding);
		}
	}

	private void RecalculateArea(IBuildingWithRange buildingWithRange)
	{
		string rangeName = buildingWithRange.RangeName;
		_rangeTileMarkerService.RecalculateArea(rangeName);
		_rangeObjectHighlighterService.RecalculateAreaAndHighlightObjects(rangeName);
	}
}
