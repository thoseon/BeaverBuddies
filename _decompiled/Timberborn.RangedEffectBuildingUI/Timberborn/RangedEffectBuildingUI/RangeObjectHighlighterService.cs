using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlueprintSystem;
using Timberborn.BuildingRange;
using Timberborn.Common;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.RangedEffectBuildingUI;

public class RangeObjectHighlighterService : ILoadableSingleton
{
	private readonly ISpecService _specService;

	private readonly Highlighter _highlighter;

	private IBuildingWithRange _objectsPreview;

	private readonly HashSet<BaseComponent> _currentObjects = new HashSet<BaseComponent>();

	private readonly Dictionary<string, HashSet<IBuildingWithRange>> _buildingsWithRanges = new Dictionary<string, HashSet<IBuildingWithRange>>();

	private Color _buildingRangeObjectColor;

	public RangeObjectHighlighterService(ISpecService specService, Highlighter highlighter)
	{
		_specService = specService;
		_highlighter = highlighter;
	}

	public void Load()
	{
		_buildingRangeObjectColor = _specService.GetSingleSpec<RangedEffectBuildingColorsSpec>().BuildingRangeObject;
	}

	public void AddBuildingWithObjectRange(IBuildingWithRange buildingWithRange)
	{
		_buildingsWithRanges.GetOrAdd(buildingWithRange.RangeName).Add(buildingWithRange);
	}

	public void RemoveBuildingWithObjectRange(IBuildingWithRange buildingWithRange)
	{
		string rangeName = buildingWithRange.RangeName;
		_buildingsWithRanges[rangeName].Remove(buildingWithRange);
		RecalculateHighlightArea(rangeName);
	}

	public void AddPreviewBuildingWithObjectRange(IBuildingWithRange buildingWithRange)
	{
		_objectsPreview = buildingWithRange;
		HighlightObjects();
	}

	public void RemovePreviewBuildingWithObjectRange()
	{
		_objectsPreview = null;
		ClearHighlights();
	}

	public void RecalculateAreaAndHighlightObjects(string rangeName)
	{
		RecalculateHighlightArea(rangeName);
		HighlightObjects();
	}

	public void HighlightObjects()
	{
		ClearHighlights();
		foreach (BaseComponent currentObject in _currentObjects)
		{
			_highlighter.HighlightSecondary(currentObject, _buildingRangeObjectColor);
		}
		if (_objectsPreview == null)
		{
			return;
		}
		foreach (BaseComponent item in _objectsPreview.GetObjectsInRange())
		{
			_highlighter.HighlightSecondary(item, _buildingRangeObjectColor);
		}
	}

	public void ClearHighlights()
	{
		_highlighter.UnhighlightAllSecondary();
	}

	private void RecalculateHighlightArea(string rangeName)
	{
		_currentObjects.Clear();
		foreach (IBuildingWithRange item in _buildingsWithRanges.GetOrAdd(rangeName))
		{
			_currentObjects.UnionWith(item.GetObjectsInRange());
		}
	}
}
