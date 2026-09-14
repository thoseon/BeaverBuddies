using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.LevelVisibilitySystem;
using Timberborn.Navigation;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.BuildingsNavigation;

internal class BoundsNavRangeDrawingService : ILoadableSingleton, ISingletonPreviewNavMeshListener
{
	private readonly INavigationRangeService _navigationRangeService;

	private readonly BoundsNavRangeDrawer _boundsNavRangeDrawer;

	private readonly EventBus _eventBus;

	private readonly HashSet<Vector3Int> _terrainNodes = new HashSet<Vector3Int>();

	private bool _fresh;

	private Vector3 _rangeAreaCenter;

	private bool _isPreview;

	private bool _drawTerrain;

	private bool _drawRoadSpill;

	public BoundsNavRangeDrawingService(INavigationRangeService navigationRangeService, BoundsNavRangeDrawer boundsNavRangeDrawer, EventBus eventBus)
	{
		_navigationRangeService = navigationRangeService;
		_boundsNavRangeDrawer = boundsNavRangeDrawer;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnMaxVisibleLevelChanged(MaxVisibleLevelChangedEvent maxVisibleLevelChangedEvent)
	{
		_fresh = false;
	}

	public void OnPreviewNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		_fresh = false;
	}

	public void DrawRange(Vector3 rangeAreaCenter, bool isPreview, bool drawTerrain, bool drawRoadSpill)
	{
		UpdateArea(rangeAreaCenter, isPreview, drawTerrain, drawRoadSpill);
		_boundsNavRangeDrawer.Draw();
	}

	private void UpdateArea(Vector3 rangeAreaCenter, bool isPreview, bool drawTerrain, bool drawRoadSpill)
	{
		if (!_fresh || _rangeAreaCenter != rangeAreaCenter || _isPreview != isPreview || _drawTerrain != drawTerrain || _drawRoadSpill != drawRoadSpill)
		{
			_fresh = true;
			_rangeAreaCenter = rangeAreaCenter;
			_isPreview = isPreview;
			_drawTerrain = drawTerrain;
			_drawRoadSpill = drawRoadSpill;
			UpdateAllNodes();
			UpdateNavRangeDrawer();
		}
	}

	private void UpdateAllNodes()
	{
		_terrainNodes.Clear();
		if (_isPreview)
		{
			UpdatePreviewNodes();
		}
		else
		{
			UpdateNodes();
		}
	}

	private void UpdatePreviewNodes()
	{
		if (_drawTerrain)
		{
			_terrainNodes.AddRange(_navigationRangeService.GetTerrainPreviewNodesInRange(_rangeAreaCenter));
		}
		else if (_drawRoadSpill)
		{
			_terrainNodes.AddRange(_navigationRangeService.GetRoadSpillPreviewNodesInRange(_rangeAreaCenter));
		}
	}

	private void UpdateNodes()
	{
		if (_drawTerrain)
		{
			_terrainNodes.AddRange(_navigationRangeService.GetTerrainNodesInRange(_rangeAreaCenter));
		}
		else if (_drawRoadSpill)
		{
			_terrainNodes.AddRange(_navigationRangeService.GetRoadSpillNodesInRange(_rangeAreaCenter));
		}
	}

	private void UpdateNavRangeDrawer()
	{
		if (_isPreview)
		{
			_boundsNavRangeDrawer.UpdateAreaPreview(_terrainNodes);
		}
		else
		{
			_boundsNavRangeDrawer.UpdateArea(_terrainNodes);
		}
	}
}
