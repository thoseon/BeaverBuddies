using UnityEngine;

namespace Timberborn.Navigation;

internal class NavMeshService : INavMeshService
{
	private readonly TerrainNavMeshGraph _terrainNavMeshGraph;

	private readonly PreviewTerrainNavMeshGraph _previewTerrainNavMeshGraph;

	private readonly NodeIdService _nodeIdService;

	private readonly NavMeshUpdater _navMeshUpdater;

	private readonly InstantRoadNavMeshGraph _instantRoadNavMeshGraph;

	private readonly InstantTerrainNavMeshGraph _instantTerrainNavMeshGraph;

	private readonly PreviewRoadNavMeshGraph _previewRoadNavMeshGraph;

	public NavMeshService(TerrainNavMeshGraph terrainNavMeshGraph, PreviewTerrainNavMeshGraph previewTerrainNavMeshGraph, NodeIdService nodeIdService, NavMeshUpdater navMeshUpdater, InstantRoadNavMeshGraph instantRoadNavMeshGraph, InstantTerrainNavMeshGraph instantTerrainNavMeshGraph, PreviewRoadNavMeshGraph previewRoadNavMeshGraph)
	{
		_terrainNavMeshGraph = terrainNavMeshGraph;
		_previewTerrainNavMeshGraph = previewTerrainNavMeshGraph;
		_nodeIdService = nodeIdService;
		_navMeshUpdater = navMeshUpdater;
		_instantRoadNavMeshGraph = instantRoadNavMeshGraph;
		_instantTerrainNavMeshGraph = instantTerrainNavMeshGraph;
		_previewRoadNavMeshGraph = previewRoadNavMeshGraph;
	}

	public void AddEdge(NavMeshEdge navMeshEdge)
	{
		_navMeshUpdater.EnqueueRegularChange(new NavMeshChangeSpecification(navMeshEdge, NavMeshChangeType.AddEdge));
	}

	public void RemoveEdge(NavMeshEdge navMeshEdge)
	{
		_navMeshUpdater.EnqueueRegularChange(new NavMeshChangeSpecification(navMeshEdge, NavMeshChangeType.RemoveEdge));
	}

	public void BlockEdge(NavMeshEdge navMeshEdge)
	{
		_navMeshUpdater.EnqueueRegularChange(new NavMeshChangeSpecification(navMeshEdge, NavMeshChangeType.BlockEdge));
	}

	public void UnblockEdge(NavMeshEdge navMeshEdge)
	{
		_navMeshUpdater.EnqueueRegularChange(new NavMeshChangeSpecification(navMeshEdge, NavMeshChangeType.UnblockEdge));
	}

	public void AddPreviewEdge(NavMeshEdge navMeshEdge)
	{
		_navMeshUpdater.EnqueuePreviewChange(new NavMeshChangeSpecification(navMeshEdge, NavMeshChangeType.AddEdge));
	}

	public void RemovePreviewEdge(NavMeshEdge navMeshEdge)
	{
		_navMeshUpdater.EnqueuePreviewChange(new NavMeshChangeSpecification(navMeshEdge, NavMeshChangeType.RemoveEdge));
	}

	public bool IsOnNavMesh(Vector3Int coordinates)
	{
		if (_nodeIdService.Contains(coordinates))
		{
			return _terrainNavMeshGraph.IsOnNavMesh(_nodeIdService.GridToId(coordinates));
		}
		return false;
	}

	public bool AreConnected(Vector3Int coordinatesA, Vector3Int coordinatesB)
	{
		return AreConnected(_terrainNavMeshGraph, coordinatesA, coordinatesB);
	}

	public bool AreConnectedPreview(Vector3Int coordinatesA, Vector3Int coordinatesB)
	{
		return AreConnected(_previewTerrainNavMeshGraph, coordinatesA, coordinatesB);
	}

	public bool AreConnectedRoadPreview(Vector3Int coordinatesA, Vector3Int coordinatesB)
	{
		return AreConnected(_previewRoadNavMeshGraph, coordinatesA, coordinatesB);
	}

	public bool AreConnectedInstant(Vector3Int coordinatesA, Vector3Int coordinatesB)
	{
		return AreConnected(_instantTerrainNavMeshGraph, coordinatesA, coordinatesB);
	}

	public bool AreConnectedRoadInstant(Vector3Int coordinatesA, Vector3Int coordinatesB)
	{
		return AreConnected(_instantRoadNavMeshGraph, coordinatesA, coordinatesB);
	}

	private bool AreConnected(TerrainNavMeshGraph terrainNavMeshGraph, Vector3Int coordinatesA, Vector3Int coordinatesB)
	{
		if (EdgeIsInMap(coordinatesA, coordinatesB))
		{
			return terrainNavMeshGraph.AreConnected(_nodeIdService.GridToId(coordinatesA), _nodeIdService.GridToId(coordinatesB));
		}
		return false;
	}

	private bool AreConnected(RoadNavMeshGraph roadNavMeshGraph, Vector3Int coordinatesA, Vector3Int coordinatesB)
	{
		if (EdgeIsInMap(coordinatesA, coordinatesB))
		{
			return roadNavMeshGraph.AreConnected(_nodeIdService.GridToId(coordinatesA), _nodeIdService.GridToId(coordinatesB));
		}
		return false;
	}

	private bool EdgeIsInMap(Vector3Int start, Vector3Int end)
	{
		if (_nodeIdService.Contains(start))
		{
			return _nodeIdService.Contains(end);
		}
		return false;
	}
}
