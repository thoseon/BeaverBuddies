using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Timberborn.Navigation;

internal class DistrictService : IDistrictService
{
	private readonly InstantDistrictMap _instantDistrictMap;

	private readonly NodeIdService _nodeIdService;

	private readonly DistrictUpdater _districtUpdater;

	private readonly PreviewDistrictMap _previewDistrictMap;

	private readonly DistrictConflictDetector _districtConflictDetector;

	private readonly PreviewRoadNavMeshGraph _previewRoadNavMeshGraph;

	private readonly PreviewDistrictObstacleService _previewDistrictObstacleService;

	private readonly DistrictMap _districtMap;

	private readonly GlobalReachabilityService _globalReachabilityService;

	private readonly DistrictRandomDestinationPicker _districtRandomDestinationPicker;

	public DistrictService(InstantDistrictMap instantDistrictMap, NodeIdService nodeIdService, DistrictUpdater districtUpdater, PreviewDistrictMap previewDistrictMap, DistrictConflictDetector districtConflictDetector, PreviewRoadNavMeshGraph previewRoadNavMeshGraph, PreviewDistrictObstacleService previewDistrictObstacleService, DistrictMap districtMap, GlobalReachabilityService globalReachabilityService, DistrictRandomDestinationPicker districtRandomDestinationPicker)
	{
		_instantDistrictMap = instantDistrictMap;
		_nodeIdService = nodeIdService;
		_districtUpdater = districtUpdater;
		_previewDistrictMap = previewDistrictMap;
		_districtConflictDetector = districtConflictDetector;
		_previewRoadNavMeshGraph = previewRoadNavMeshGraph;
		_previewDistrictObstacleService = previewDistrictObstacleService;
		_districtMap = districtMap;
		_globalReachabilityService = globalReachabilityService;
		_districtRandomDestinationPicker = districtRandomDestinationPicker;
	}

	public District AddDistrict(Vector3Int centerCoordinates)
	{
		if (_nodeIdService.Contains(centerCoordinates))
		{
			District district = new District(_nodeIdService.GridToId(centerCoordinates), centerCoordinates);
			_districtUpdater.EnqueueChange(DistrictChange.AddDistrict(district));
			return district;
		}
		throw new ArgumentException($"{centerCoordinates} is out of map");
	}

	public void RemoveDistrict(District district)
	{
		_districtUpdater.EnqueueChange(DistrictChange.RemoveDistrict(district));
	}

	public District AddPreviewDistrict(Vector3Int centerCoordinates)
	{
		if (_nodeIdService.Contains(centerCoordinates))
		{
			District district = new District(_nodeIdService.GridToId(centerCoordinates), centerCoordinates);
			_districtUpdater.ApplyPreviewChange(DistrictChange.AddDistrict(district));
			return district;
		}
		throw new ArgumentException($"{centerCoordinates} is out of map");
	}

	public void RemovePreviewDistrict(District district)
	{
		_districtUpdater.ApplyPreviewChange(DistrictChange.RemoveDistrict(district));
	}

	public void SetObstacle(Vector3Int coordinates)
	{
		if (_nodeIdService.Contains(coordinates))
		{
			int obstacle = _nodeIdService.GridToId(coordinates);
			_districtUpdater.EnqueueChange(DistrictChange.SetObstacle(obstacle));
		}
	}

	public void UnsetObstacle(Vector3Int coordinates)
	{
		if (_nodeIdService.Contains(coordinates))
		{
			int nodeId = _nodeIdService.GridToId(coordinates);
			_districtUpdater.EnqueueChange(DistrictChange.UnsetObstacle(nodeId));
		}
	}

	public void SetPreviewObstacle(Vector3Int coordinates)
	{
		if (_nodeIdService.Contains(coordinates))
		{
			int obstacle = _nodeIdService.GridToId(coordinates);
			_districtUpdater.ApplyPreviewChange(DistrictChange.SetObstacle(obstacle));
		}
	}

	public void UnsetPreviewObstacle(Vector3Int coordinates)
	{
		if (_nodeIdService.Contains(coordinates))
		{
			int nodeId = _nodeIdService.GridToId(coordinates);
			_districtUpdater.ApplyPreviewChange(DistrictChange.UnsetObstacle(nodeId));
		}
	}

	public bool IsPreviewDistrictInConflict(Vector3Int? previewDistrictCenter)
	{
		IReadOnlyCollection<int> readOnlyCollection = _previewDistrictMap.DistrictCenterNodeIds();
		IEnumerable<int> enumerable2;
		if (!previewDistrictCenter.HasValue)
		{
			IEnumerable<int> enumerable = readOnlyCollection;
			enumerable2 = enumerable;
		}
		else
		{
			enumerable2 = readOnlyCollection.Append(_nodeIdService.GridToId(previewDistrictCenter.Value));
		}
		IEnumerable<int> districtCenters = enumerable2;
		return _districtConflictDetector.AreDistrictsInConflict(_previewRoadNavMeshGraph, _previewDistrictObstacleService, districtCenters);
	}

	public bool DistrictIsGloballyReachable(District district, Vector3 start)
	{
		if (district != null && _instantDistrictMap.HasDistrictCenter(district))
		{
			return _globalReachabilityService.AreaReachable(start, district.CenterNodeId);
		}
		return false;
	}

	public bool IsOnDistrictRoad(District district, Vector3 road)
	{
		return IsOnDistrictRoad(_districtMap, district, road);
	}

	public bool IsOnInstantDistrictRoad(District district, Vector3 road)
	{
		return IsOnDistrictRoad(_instantDistrictMap, district, road);
	}

	public bool IsOnPreviewDistrictRoad(District district, Vector3 road)
	{
		return IsOnDistrictRoad(_previewDistrictMap, district, road);
	}

	public bool IsOnInstantDistrictRoadSpill(Accessible accessible)
	{
		for (int i = 0; i < accessible.Accesses.Count; i++)
		{
			if (IsOnInstantDistrictRoadSpill(accessible.Accesses[i]))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsOnInstantDistrictRoadSpill(Vector3 position)
	{
		if (_nodeIdService.Contains(position))
		{
			return _instantDistrictMap.NodeHasAnyDistrictRoadSpillFlowField(_nodeIdService.WorldToId(position));
		}
		return false;
	}

	public Vector3 GetRandomDestinationInDistrict(District district, Vector3 coordinates)
	{
		return _districtRandomDestinationPicker.GetRandomDestination(district, coordinates);
	}

	private bool IsOnDistrictRoad(DistrictMap districtMap, District district, Vector3 road)
	{
		if (_nodeIdService.Contains(road))
		{
			int nodeId = _nodeIdService.WorldToId(road);
			return districtMap.RoadNodeIsOccupiedByDistrict(district, nodeId);
		}
		return false;
	}
}
