using System.Collections.Generic;
using Timberborn.Automation;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.WaterBuildings;

internal class ThrottlingValveSynchronizer
{
	private static readonly Vector3Int[] Neighbors = new Vector3Int[2]
	{
		new Vector3Int(-1, 0, 0),
		new Vector3Int(1, 0, 0)
	};

	private readonly IBlockService _blockService;

	private readonly Queue<ThrottlingValve> _neighborsQueue = new Queue<ThrottlingValve>();

	private readonly HashSet<ThrottlingValve> _visitedNeighbors = new HashSet<ThrottlingValve>();

	public ThrottlingValveSynchronizer(IBlockService blockService)
	{
		_blockService = blockService;
	}

	public void SynchronizeAllNeighbors(ThrottlingValve throttlingValve)
	{
		SynchronizeNeighbors(throttlingValve, unfinishedOnly: false);
	}

	public void SynchronizeWithAllNeighbors(ThrottlingValve throttlingValve)
	{
		if (throttlingValve.IsSynchronized)
		{
			SynchronizeWithNeighbors(throttlingValve, unfinishedOnly: false);
		}
	}

	public void SynchronizeWithUnfinishedNeighbors(ThrottlingValve throttlingValve)
	{
		if (throttlingValve.IsSynchronized)
		{
			SynchronizeWithNeighbors(throttlingValve, unfinishedOnly: true);
		}
	}

	private void SynchronizeNeighbors(ThrottlingValve startingValve, bool unfinishedOnly)
	{
		if (!startingValve.IsSynchronized)
		{
			return;
		}
		EnqueueValve(startingValve);
		while (!_neighborsQueue.IsEmpty())
		{
			ThrottlingValve throttlingValve = _neighborsQueue.Dequeue();
			BlockObject component = throttlingValve.GetComponent<BlockObject>();
			Vector3Int[] neighbors = Neighbors;
			foreach (Vector3Int coordinates in neighbors)
			{
				SynchronizeNeighbor(throttlingValve, component.TransformCoordinates(coordinates), component.Orientation, unfinishedOnly);
			}
		}
		_visitedNeighbors.Clear();
	}

	private void SynchronizeWithNeighbors(ThrottlingValve throttlingValve, bool unfinishedOnly)
	{
		BlockObject component = throttlingValve.GetComponent<BlockObject>();
		Vector3Int[] neighbors = Neighbors;
		foreach (Vector3Int coordinates in neighbors)
		{
			Vector3Int coordinates2 = component.TransformCoordinates(coordinates);
			ThrottlingValve throttlingValve2 = GetThrottlingValve(coordinates2, component.Orientation);
			if (throttlingValve2 != null)
			{
				SynchronizeNeighbors(throttlingValve2, unfinishedOnly);
				break;
			}
		}
	}

	private void SynchronizeNeighbor(ThrottlingValve sourceValve, Vector3Int neighborCoords, Orientation orientation, bool unfinishedOnly)
	{
		ThrottlingValve throttlingValve = GetThrottlingValve(neighborCoords, orientation);
		if ((bool)throttlingValve && !_visitedNeighbors.Contains(throttlingValve))
		{
			BlockObject component = throttlingValve.GetComponent<BlockObject>();
			if (!unfinishedOnly || !component.IsFinished)
			{
				throttlingValve.SetOutflowLimit(sourceValve.OutflowLimit);
				throttlingValve.SetOutflowLimitEnabled(sourceValve.OutflowLimitEnabled);
				throttlingValve.SetAutomationOutflowLimit(sourceValve.AutomationOutflowLimit);
				throttlingValve.SetAutomationOutflowLimitEnabled(sourceValve.AutomationOutflowLimitEnabled);
				throttlingValve.SetReactionSpeed(sourceValve.ReactionSpeed);
				Automatable component2 = sourceValve.GetComponent<Automatable>();
				throttlingValve.GetComponent<Automatable>().SetInput(component2.Input);
				EnqueueValve(throttlingValve);
			}
		}
	}

	private void EnqueueValve(ThrottlingValve throttlingValve)
	{
		_neighborsQueue.Enqueue(throttlingValve);
		_visitedNeighbors.Add(throttlingValve);
	}

	private ThrottlingValve GetThrottlingValve(Vector3Int coordinates, Orientation orientation)
	{
		ThrottlingValve bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<ThrottlingValve>(coordinates);
		if ((bool)bottomObjectComponentAt && bottomObjectComponentAt.IsSynchronized)
		{
			BlockObject component = bottomObjectComponentAt.GetComponent<BlockObject>();
			if (component.Orientation == orientation && component.Coordinates == coordinates)
			{
				return bottomObjectComponentAt;
			}
		}
		return null;
	}
}
