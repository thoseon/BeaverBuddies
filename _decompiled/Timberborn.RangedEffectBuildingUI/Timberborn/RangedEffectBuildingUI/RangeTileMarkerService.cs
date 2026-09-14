using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.BuildingRange;
using Timberborn.Common;
using Timberborn.Rendering;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.RangedEffectBuildingUI;

public class RangeTileMarkerService : ILoadableSingleton
{
	private readonly AreaTileDrawerFactory _areaTileDrawerFactory;

	private readonly ISpecService _specService;

	private readonly RootObjectProvider _rootObjectProvider;

	private GameObject _parent;

	private AreaTileDrawer _areaTileDrawer;

	private IBuildingWithRange _previewBuildingWithRange;

	private Preview _preview;

	private readonly HashSet<Vector3Int> _currentRange = new HashSet<Vector3Int>();

	private readonly HashSet<Vector3Int> _showableRange = new HashSet<Vector3Int>();

	private readonly Dictionary<string, HashSet<IBuildingWithRange>> _buildingsWithRanges = new Dictionary<string, HashSet<IBuildingWithRange>>();

	public RangeTileMarkerService(AreaTileDrawerFactory areaTileDrawerFactory, ISpecService specService, RootObjectProvider rootObjectProvider)
	{
		_areaTileDrawerFactory = areaTileDrawerFactory;
		_specService = specService;
		_rootObjectProvider = rootObjectProvider;
	}

	public void Load()
	{
		_parent = _rootObjectProvider.CreateRootObject("RangeTileMarkerService");
		RangedEffectBuildingColorsSpec singleSpec = _specService.GetSingleSpec<RangedEffectBuildingColorsSpec>();
		_areaTileDrawer = _areaTileDrawerFactory.Create(singleSpec.BuildingRangeTile, _parent);
	}

	public void AddBuildingWithRange(IBuildingWithRange buildingWithRange)
	{
		_buildingsWithRanges.GetOrAdd(buildingWithRange.RangeName).Add(buildingWithRange);
	}

	public void RemoveBuildingWithRange(IBuildingWithRange buildingWithRange)
	{
		_buildingsWithRanges[buildingWithRange.RangeName].Remove(buildingWithRange);
	}

	public void AddPreviewBuildingWithRange(IBuildingWithRange buildingWithRange, Preview preview)
	{
		_previewBuildingWithRange = buildingWithRange;
		_preview = preview;
		DrawArea();
	}

	public void RemovePreviewBuildingWithRange()
	{
		_previewBuildingWithRange = null;
		_preview = null;
	}

	public void DrawArea()
	{
		_showableRange.Clear();
		_showableRange.UnionWith(_currentRange);
		if (_previewBuildingWithRange != null && _preview.GameObject.activeInHierarchy)
		{
			_showableRange.UnionWith(_previewBuildingWithRange.GetBlocksInRange());
		}
		_areaTileDrawer.UpdateArea(_showableRange);
	}

	public void RecalculateArea(string rangeName)
	{
		_currentRange.Clear();
		foreach (IBuildingWithRange item in _buildingsWithRanges.GetOrAdd(rangeName))
		{
			_currentRange.UnionWith(item.GetBlocksInRange());
		}
		DrawArea();
	}

	public void ShowArea()
	{
		_areaTileDrawer.ShowAllTiles();
	}

	public void HideArea()
	{
		_areaTileDrawer.HideAllTiles();
	}
}
