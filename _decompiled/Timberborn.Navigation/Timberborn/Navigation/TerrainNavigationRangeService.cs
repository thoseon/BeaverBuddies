using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Timberborn.Navigation;

internal class TerrainNavigationRangeService : ISingletonPreviewNavMeshListener, ISingletonInstantNavMeshListener
{
	private readonly NodeIdService _nodeIdService;

	private readonly TerrainFlowFieldGenerator _terrainFlowFieldGenerator;

	private readonly InstantTerrainNavMeshGraph _instantTerrainNavMeshGraph;

	private readonly PreviewTerrainNavMeshGraph _previewTerrainNavMeshGraph;

	private readonly AccessFlowField _flowField = new AccessFlowField();

	private Vector3 _lastUsedFlowFieldPosition;

	private TerrainNavMeshGraph _lastUsedNavMeshGraph;

	public TerrainNavigationRangeService(NodeIdService nodeIdService, TerrainFlowFieldGenerator terrainFlowFieldGenerator, InstantTerrainNavMeshGraph instantTerrainNavMeshGraph, PreviewTerrainNavMeshGraph previewTerrainNavMeshGraph)
	{
		_nodeIdService = nodeIdService;
		_terrainFlowFieldGenerator = terrainFlowFieldGenerator;
		_instantTerrainNavMeshGraph = instantTerrainNavMeshGraph;
		_previewTerrainNavMeshGraph = previewTerrainNavMeshGraph;
	}

	public IEnumerable<Vector3Int> GetNodesInRange(Vector3 position, float maxDistance)
	{
		AccessFlowField flowField = FilledReusableFlowField(_instantTerrainNavMeshGraph, position, maxDistance);
		return GetNodes(flowField);
	}

	public IEnumerable<Vector3Int> GetPreviewNodesInRange(Vector3 position, float maxDistance)
	{
		AccessFlowField flowField = FilledReusableFlowField(_previewTerrainNavMeshGraph, position, maxDistance);
		return GetNodes(flowField);
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
		_flowField.OnNodesChanged(navMeshUpdate.TerrainNodeIds);
	}

	private AccessFlowField FilledReusableFlowField(TerrainNavMeshGraph terrainNavMeshGraph, Vector3 position, float maxDistance)
	{
		if (_lastUsedFlowFieldPosition != position || _lastUsedNavMeshGraph != terrainNavMeshGraph)
		{
			_lastUsedFlowFieldPosition = position;
			_lastUsedNavMeshGraph = terrainNavMeshGraph;
			_flowField.Clear();
		}
		int startNodeId = _nodeIdService.WorldToId(position);
		_terrainFlowFieldGenerator.FillFlowFieldUpToDistance(terrainNavMeshGraph, _flowField, maxDistance, startNodeId);
		return _flowField;
	}

	private IEnumerable<Vector3Int> GetNodes(AccessFlowField flowField)
	{
		return from node in flowField.GetAllNodes()
			select _nodeIdService.IdToGrid(node.Id);
	}
}
