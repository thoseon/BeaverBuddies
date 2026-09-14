using System;
using UnityEngine;

namespace Timberborn.Navigation;

internal class DistanceCalculator
{
	private readonly NodeIdService _nodeIdService;

	public DistanceCalculator(NodeIdService nodeIdService)
	{
		_nodeIdService = nodeIdService;
	}

	public float Distance(int aNodeId, Vector3Int bNavMeshCoords)
	{
		return Distance(_nodeIdService.IdToGrid(aNodeId), bNavMeshCoords);
	}

	private static float Distance(Vector3Int a, Vector3Int b)
	{
		int num = Math.Abs(a.x - b.x);
		int num2 = Math.Abs(a.y - b.y);
		return 0.9f * (float)(num + num2) + -0.52722f * (float)Math.Min(num, num2);
	}
}
