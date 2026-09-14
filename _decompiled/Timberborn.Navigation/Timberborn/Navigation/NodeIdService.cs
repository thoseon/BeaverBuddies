using System;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Navigation;

internal class NodeIdService : ILoadableSingleton
{
	private static readonly Vector3Int Boundary = new Vector3Int(1, 1, 1);

	private readonly INavMeshSizeProvider _navMeshSizeProvider;

	private Vector3Int _size;

	private Vector3Int _minCoords;

	private Vector3Int _maxCoords;

	private Vector3Int[] _idToCoordinatesTable;

	public int NumberOfNodes { get; private set; }

	public NodeIdService(INavMeshSizeProvider navMeshSizeProvider)
	{
		_navMeshSizeProvider = navMeshSizeProvider;
	}

	public void Load()
	{
		Vector3Int size = _navMeshSizeProvider.Size;
		_size = size + 2 * Boundary;
		NumberOfNodes = _size.x * _size.y * _size.z;
		_minCoords = -Boundary;
		_maxCoords = size - new Vector3Int(1, 1, 1) + Boundary;
		InitializeIdToCoordinatesTable();
	}

	public Vector3Int IdToGrid(int nodeId)
	{
		try
		{
			return _idToCoordinatesTable[nodeId];
		}
		catch (Exception)
		{
			throw new ArgumentOutOfRangeException($"Coordinates {IdToGridSlow(nodeId)} of node {nodeId} are out of map");
		}
	}

	public Vector3 IdToWorld(int nodeId)
	{
		return NavigationCoordinateSystem.GridToWorld(IdToGrid(nodeId));
	}

	public int GridToId(Vector3Int coordinates)
	{
		Vector3Int vector3Int = coordinates + Boundary;
		int x = vector3Int.x;
		int y = vector3Int.y;
		int z = vector3Int.z;
		return x * _size.y * _size.z + y * _size.z + z;
	}

	public int WorldToId(Vector3 position)
	{
		return GridToId(NavigationCoordinateSystem.WorldToGridInt(position));
	}

	public float Distance(int fromNodeId, int toNodeId)
	{
		return Vector3.Distance(IdToGrid(fromNodeId), IdToGrid(toNodeId));
	}

	public bool Contains(Vector3 worldPosition)
	{
		return Contains(NavigationCoordinateSystem.WorldToGridInt(worldPosition));
	}

	public bool Contains(Vector3Int navMeshCoordinates)
	{
		int x = navMeshCoordinates.x;
		int y = navMeshCoordinates.y;
		int z = navMeshCoordinates.z;
		if (x >= _minCoords.x && x <= _maxCoords.x && y >= _minCoords.y && y <= _maxCoords.y && z >= _minCoords.z)
		{
			return z <= _maxCoords.z;
		}
		return false;
	}

	private void InitializeIdToCoordinatesTable()
	{
		_idToCoordinatesTable = new Vector3Int[NumberOfNodes];
		int num = 0;
		for (int i = _minCoords.x; i <= _maxCoords.x; i++)
		{
			for (int j = _minCoords.y; j <= _maxCoords.y; j++)
			{
				for (int k = _minCoords.z; k <= _maxCoords.z; k++)
				{
					_idToCoordinatesTable[num++] = new Vector3Int(i, j, k);
				}
			}
		}
	}

	private Vector3Int IdToGridSlow(int nodeId)
	{
		int z = nodeId % _size.z;
		int y = nodeId / _size.z % _size.y;
		return new Vector3Int(nodeId / (_size.y * _size.z), y, z) - Boundary;
	}
}
