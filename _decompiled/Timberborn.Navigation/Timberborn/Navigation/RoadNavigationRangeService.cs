using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Timberborn.Navigation;

internal class RoadNavigationRangeService : ISingletonPreviewNavMeshListener, ISingletonInstantNavMeshListener
{
	private readonly NodeIdService _nodeIdService;

	private readonly RoadFlowFieldGenerator _roadFlowFieldGenerator;

	private readonly InstantRoadNavMeshGraph _instantRoadNavMeshGraph;

	private readonly PreviewRoadNavMeshGraph _previewRoadNavMeshGraph;

	private readonly InstantDistrictMap _instantDistrictMap;

	private readonly PreviewDistrictMap _previewDistrictMap;

	private readonly AccessFlowField _flowField = new AccessFlowField();

	private Vector3 _lastUsedFlowFieldPosition;

	private DistrictMap _lastUsedDistrictMap;

	public RoadNavigationRangeService(NodeIdService nodeIdService, RoadFlowFieldGenerator roadFlowFieldGenerator, InstantRoadNavMeshGraph instantRoadNavMeshGraph, PreviewRoadNavMeshGraph previewRoadNavMeshGraph, InstantDistrictMap instantDistrictMap, PreviewDistrictMap previewDistrictMap)
	{
		_nodeIdService = nodeIdService;
		_roadFlowFieldGenerator = roadFlowFieldGenerator;
		_instantRoadNavMeshGraph = instantRoadNavMeshGraph;
		_previewRoadNavMeshGraph = previewRoadNavMeshGraph;
		_instantDistrictMap = instantDistrictMap;
		_previewDistrictMap = previewDistrictMap;
	}

	public IEnumerable<WeightedCoordinates> GetNodesInRange(Vector3 position)
	{
		AccessFlowField flowField = FilledReusableFlowField(_instantRoadNavMeshGraph, _instantDistrictMap, position);
		return GetInRangeNodes(flowField);
	}

	public IEnumerable<WeightedCoordinates> GetPreviewNodesInRange(Vector3 position)
	{
		AccessFlowField flowField = FilledReusableFlowField(_previewRoadNavMeshGraph, _previewDistrictMap, position);
		return GetInRangeNodes(flowField);
	}

	public void OnInstantNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		OnNavMeshUpdated(navMeshUpdate);
	}

	public void OnPreviewNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		OnNavMeshUpdated(navMeshUpdate);
	}

	private void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		_flowField.OnNodesChanged(navMeshUpdate.RoadNodeIds);
	}

	private AccessFlowField FilledReusableFlowField(RoadNavMeshGraph roadNavMeshGraph, DistrictMap districtMap, Vector3 position)
	{
		if (_lastUsedFlowFieldPosition != position || _lastUsedDistrictMap != districtMap)
		{
			_lastUsedFlowFieldPosition = position;
			_lastUsedDistrictMap = districtMap;
			_flowField.Clear();
		}
		int num = _nodeIdService.WorldToId(position);
		AccessFlowField districtRoadFlowFieldByRoadNodeId = districtMap.GetDistrictRoadFlowFieldByRoadNodeId(num);
		_roadFlowFieldGenerator.FillFlowField(roadNavMeshGraph, _flowField, districtRoadFlowFieldByRoadNodeId, num);
		return _flowField;
	}

	private IEnumerable<WeightedCoordinates> GetInRangeNodes(AccessFlowField flowField)
	{
		return from node in flowField.GetAllNodes()
			select new WeightedCoordinates(_nodeIdService.IdToGrid(node.Id), node.GScore);
	}
}
