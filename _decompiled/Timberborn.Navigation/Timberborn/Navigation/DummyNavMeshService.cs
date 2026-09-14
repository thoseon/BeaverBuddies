using UnityEngine;

namespace Timberborn.Navigation;

public class DummyNavMeshService : INavMeshService
{
	public void AddEdge(NavMeshEdge navMeshEdge)
	{
	}

	public void RemoveEdge(NavMeshEdge navMeshEdge)
	{
	}

	public void BlockEdge(NavMeshEdge navMeshEdge)
	{
	}

	public void UnblockEdge(NavMeshEdge navMeshEdge)
	{
	}

	public void AddPreviewEdge(NavMeshEdge navMeshEdge)
	{
	}

	public void RemovePreviewEdge(NavMeshEdge navMeshEdge)
	{
	}

	public bool IsOnNavMesh(Vector3Int coordinates)
	{
		return false;
	}

	public bool AreConnected(Vector3Int coordinatesA, Vector3Int coordinatesB)
	{
		return false;
	}

	public bool AreConnectedInstant(Vector3Int coordinatesA, Vector3Int coordinatesB)
	{
		return false;
	}

	public bool AreConnectedRoadInstant(Vector3Int coordinatesA, Vector3Int coordinatesB)
	{
		return false;
	}

	public bool AreConnectedPreview(Vector3Int coordinatesA, Vector3Int coordinatesB)
	{
		return false;
	}

	public bool AreConnectedRoadPreview(Vector3Int coordinatesA, Vector3Int coordinatesB)
	{
		return false;
	}
}
