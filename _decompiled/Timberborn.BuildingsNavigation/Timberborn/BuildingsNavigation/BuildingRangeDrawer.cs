using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BuildingRange;
using Timberborn.Buildings;
using Timberborn.ConstructionMode;
using Timberborn.GameDistricts;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.BuildingsNavigation;

internal class BuildingRangeDrawer : BaseComponent, IAwakableComponent, IUpdatableComponent, ISelectionListener, IPreviewSelectionListener
{
	private readonly BoundsNavRangeDrawingService _boundsNavRangeDrawingService;

	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private readonly ConstructionModeService _constructionModeService;

	private BlockObject _blockObject;

	private BuildingAccessible _buildingAccessible;

	private DistrictBuilding _districtBuilding;

	private DistrictCenter _districtCenter;

	private Preview _preview;

	private bool _drawTerrainRange;

	private bool _drawRoadSpilledRange;

	public BuildingRangeDrawer(BoundsNavRangeDrawingService boundsNavRangeDrawingService, DistrictCenterRegistry districtCenterRegistry, ConstructionModeService constructionModeService)
	{
		_boundsNavRangeDrawingService = boundsNavRangeDrawingService;
		_districtCenterRegistry = districtCenterRegistry;
		_constructionModeService = constructionModeService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_buildingAccessible = GetComponent<BuildingAccessible>();
		_districtBuilding = GetComponent<DistrictBuilding>();
		_districtCenter = GetComponent<DistrictCenter>();
		_preview = GetComponent<Preview>();
		_drawTerrainRange = GetComponent<BuildingWithTerrainRange>();
		_drawRoadSpilledRange = GetComponent<BuildingWithRoadSpillRange>();
		DisableComponent();
	}

	public void Update()
	{
		DrawRange();
	}

	public void OnSelect()
	{
		DrawRange();
		EnableComponent();
	}

	public void OnUnselect()
	{
		DisableComponent();
	}

	public void OnPreviewSelect()
	{
		EnableComponent();
	}

	public void OnPreviewUnselect()
	{
		DisableComponent();
	}

	private void DrawRange()
	{
		bool isPreview = _blockObject.IsPreview;
		bool isFinished = _blockObject.IsFinished;
		bool flag = isPreview || !isFinished;
		Vector3? unblockedSingleAccess = GetUnblockedSingleAccess(_buildingAccessible, flag);
		if (unblockedSingleAccess.HasValue)
		{
			Vector3 valueOrDefault = unblockedSingleAccess.GetValueOrDefault();
			if (_drawTerrainRange || _drawRoadSpilledRange)
			{
				bool inConstructionMode = _constructionModeService.InConstructionMode;
				_boundsNavRangeDrawingService.DrawRange(valueOrDefault, flag | inConstructionMode, _drawTerrainRange, _drawRoadSpilledRange);
			}
			DistrictCenter districtCenter = GetDistrictCenter(isPreview, isFinished, valueOrDefault);
			if ((bool)districtCenter)
			{
				DistrictPathNavRangeDrawer component = districtCenter.GetComponent<DistrictPathNavRangeDrawer>();
				DrawingParameters drawingParameters = new DrawingParameters(flag, valueOrDefault, _blockObject.Orientation, isSingle: true);
				component.DrawRange(drawingParameters);
			}
		}
	}

	private DistrictCenter GetDistrictCenter(bool isPreview, bool isFinished, Vector3 buildingAccess)
	{
		if (isFinished)
		{
			return _districtBuilding.InstantDistrict;
		}
		if (isPreview && !_preview.PreviewState.IsBuildable)
		{
			return null;
		}
		foreach (DistrictCenter allDistrictCenter in _districtCenterRegistry.AllDistrictCenters)
		{
			if (allDistrictCenter.IsOnPreviewDistrictRoad(buildingAccess))
			{
				return allDistrictCenter;
			}
		}
		return _districtCenter;
	}

	private static Vector3? GetUnblockedSingleAccess(BuildingAccessible buildingAccessible, bool isPreview)
	{
		if (!isPreview)
		{
			return buildingAccessible.Accessible.UnblockedSingleAccessInstant;
		}
		return buildingAccessible.CalculateAccess();
	}
}
