using UnityEngine;

namespace Timberborn.Navigation;

public interface INavMeshService
{
	void AddEdge(NavMeshEdge navMeshEdge);

	void RemoveEdge(NavMeshEdge navMeshEdge);

	void BlockEdge(NavMeshEdge navMeshEdge);

	void UnblockEdge(NavMeshEdge navMeshEdge);

	void AddPreviewEdge(NavMeshEdge navMeshEdge);

	void RemovePreviewEdge(NavMeshEdge navMeshEdge);

	bool IsOnNavMesh(Vector3Int coordinates);

	bool AreConnected(Vector3Int coordinatesA, Vector3Int coordinatesB);

	bool AreConnectedInstant(Vector3Int coordinatesA, Vector3Int coordinatesB);

	bool AreConnectedRoadInstant(Vector3Int coordinatesA, Vector3Int coordinatesB);

	bool AreConnectedPreview(Vector3Int coordinatesA, Vector3Int coordinatesB);

	bool AreConnectedRoadPreview(Vector3Int coordinatesA, Vector3Int coordinatesB);
}
