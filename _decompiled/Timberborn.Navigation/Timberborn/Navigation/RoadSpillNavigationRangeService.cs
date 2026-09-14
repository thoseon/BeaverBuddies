using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Timberborn.Navigation;

internal class RoadSpillNavigationRangeService
{
	private readonly NodeIdService _nodeIdService;

	private readonly InstantDistrictMap _instantDistrictMap;

	private readonly PreviewDistrictMap _previewDistrictMap;

	public RoadSpillNavigationRangeService(NodeIdService nodeIdService, InstantDistrictMap instantDistrictMap, PreviewDistrictMap previewDistrictMap)
	{
		_nodeIdService = nodeIdService;
		_instantDistrictMap = instantDistrictMap;
		_previewDistrictMap = previewDistrictMap;
	}

	public IEnumerable<Vector3Int> GetNodesInRange(Vector3 position)
	{
		return GetNodesFromFlowFieldAt(_instantDistrictMap, position);
	}

	public IEnumerable<Vector3Int> GetPreviewNodesInRange(Vector3 position)
	{
		return GetNodesFromFlowFieldAt(_previewDistrictMap, position);
	}

	private IEnumerable<Vector3Int> GetNodesFromFlowFieldAt(DistrictMap districtMap, Vector3 position)
	{
		int nodeId = _nodeIdService.WorldToId(position);
		return from nodeId2 in districtMap.GetDistrictRoadSpillFlowFieldByRoadNodeId(nodeId).GetAllNodes()
			select _nodeIdService.IdToGrid(nodeId2);
	}
}
