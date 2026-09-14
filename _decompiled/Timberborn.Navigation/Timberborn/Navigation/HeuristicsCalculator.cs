using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Navigation;

internal class HeuristicsCalculator
{
	private readonly DistanceCalculator _distanceCalculator;

	private readonly NodeIdService _nodeIdService;

	private Vector3Int _destinationNodeCoords;

	public HeuristicsCalculator(DistanceCalculator distanceCalculator, NodeIdService nodeIdService)
	{
		_distanceCalculator = distanceCalculator;
		_nodeIdService = nodeIdService;
	}

	public void SetDestinationNode(int nodeId)
	{
		_destinationNodeCoords = _nodeIdService.IdToGrid(nodeId);
	}

	public void SetDestinationNodes(IReadOnlyList<int> nodeIds)
	{
		_destinationNodeCoords = CalculateAverageCoordinates(nodeIds);
	}

	public float H(int nodeId)
	{
		return _distanceCalculator.Distance(nodeId, _destinationNodeCoords);
	}

	private Vector3Int CalculateAverageCoordinates(IReadOnlyList<int> nodeIds)
	{
		Vector3Int zero = Vector3Int.zero;
		int count = nodeIds.Count;
		for (int i = 0; i < count; i++)
		{
			zero += _nodeIdService.IdToGrid(nodeIds[i]);
		}
		return new Vector3Int(Average(zero.x, count), Average(zero.y, count), Average(zero.z, count));
	}

	private static int Average(int vectorComponent, int numberOfNodes)
	{
		return Mathf.RoundToInt((float)vectorComponent / (float)numberOfNodes);
	}
}
