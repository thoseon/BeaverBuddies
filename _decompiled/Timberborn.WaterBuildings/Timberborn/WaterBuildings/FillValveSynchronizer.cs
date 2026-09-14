using System.Collections.Generic;
using Timberborn.Automation;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.WaterBuildings;

internal class FillValveSynchronizer
{
	private static readonly Vector3Int[] Neighbors = new Vector3Int[2]
	{
		new Vector3Int(-1, 0, 0),
		new Vector3Int(1, 0, 0)
	};

	private readonly IBlockService _blockService;

	private readonly Queue<FillValve> _neighborsQueue = new Queue<FillValve>();

	private readonly HashSet<FillValve> _visitedNeighbors = new HashSet<FillValve>();

	public FillValveSynchronizer(IBlockService blockService)
	{
		_blockService = blockService;
	}

	public void SynchronizeAllNeighbors(FillValve fillValve)
	{
		SynchronizeNeighbors(fillValve, unfinishedOnly: false);
	}

	public void SynchronizeWithAllNeighbors(FillValve fillValve)
	{
		if (fillValve.IsSynchronized)
		{
			SynchronizeWithNeighbors(fillValve, unfinishedOnly: false);
		}
	}

	public void SynchronizeWithUnfinishedNeighbors(FillValve fillValve)
	{
		if (fillValve.IsSynchronized)
		{
			SynchronizeWithNeighbors(fillValve, unfinishedOnly: true);
		}
	}

	private void SynchronizeNeighbors(FillValve startingFillValve, bool unfinishedOnly)
	{
		if (!startingFillValve.IsSynchronized)
		{
			return;
		}
		EnqueueValve(startingFillValve);
		while (!_neighborsQueue.IsEmpty())
		{
			FillValve fillValve = _neighborsQueue.Dequeue();
			BlockObject component = fillValve.GetComponent<BlockObject>();
			Vector3Int[] neighbors = Neighbors;
			foreach (Vector3Int coordinates in neighbors)
			{
				SynchronizeNeighbor(fillValve, component.TransformCoordinates(coordinates), component.Orientation, unfinishedOnly);
			}
		}
		_visitedNeighbors.Clear();
	}

	private void SynchronizeWithNeighbors(FillValve fillValve, bool unfinishedOnly)
	{
		BlockObject component = fillValve.GetComponent<BlockObject>();
		Vector3Int[] neighbors = Neighbors;
		foreach (Vector3Int coordinates in neighbors)
		{
			Vector3Int coordinates2 = component.TransformCoordinates(coordinates);
			FillValve valve = GetValve(coordinates2, component.Orientation);
			if (valve != null)
			{
				SynchronizeNeighbors(valve, unfinishedOnly);
				break;
			}
		}
	}

	private void SynchronizeNeighbor(FillValve sourceFillValve, Vector3Int neighborCoords, Orientation orientation, bool unfinishedOnly)
	{
		FillValve valve = GetValve(neighborCoords, orientation);
		if ((bool)valve && !_visitedNeighbors.Contains(valve))
		{
			BlockObject component = valve.GetComponent<BlockObject>();
			if (!unfinishedOnly || !component.IsFinished)
			{
				valve.SetTargetHeightEnabled(sourceFillValve.TargetHeightEnabled);
				valve.SetTargetHeight(sourceFillValve.TargetHeight);
				valve.SetAutomationTargetHeightEnabled(sourceFillValve.AutomationTargetHeightEnabled);
				valve.SetAutomationTargetHeight(sourceFillValve.AutomationTargetHeight);
				Automatable component2 = sourceFillValve.GetComponent<Automatable>();
				valve.GetComponent<Automatable>().SetInput(component2.Input);
				EnqueueValve(valve);
			}
		}
	}

	private void EnqueueValve(FillValve fillValve)
	{
		_neighborsQueue.Enqueue(fillValve);
		_visitedNeighbors.Add(fillValve);
	}

	private FillValve GetValve(Vector3Int coordinates, Orientation orientation)
	{
		FillValve bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<FillValve>(coordinates);
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
