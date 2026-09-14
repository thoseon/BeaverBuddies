using System.Collections.Generic;
using Timberborn.Automation;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.WaterBuildings;

internal class FloodgateSynchronizer
{
	private readonly IBlockService _blockService;

	private readonly Queue<Floodgate> _neighborsQueue = new Queue<Floodgate>();

	private readonly HashSet<Floodgate> _visitedNeighbors = new HashSet<Floodgate>();

	public FloodgateSynchronizer(IBlockService blockService)
	{
		_blockService = blockService;
	}

	public void SynchronizeAllNeighbors(Floodgate floodgate)
	{
		SynchronizeNeighbors(floodgate, unfinishedOnly: false);
	}

	public void SynchronizeWithAllNeighbors(Floodgate floodgate)
	{
		if (floodgate.IsSynchronized)
		{
			SynchronizeWithNeighbors(floodgate, unfinishedOnly: false);
		}
	}

	public void SynchronizeWithUnfinishedNeighbors(Floodgate floodgate)
	{
		if (floodgate.IsSynchronized)
		{
			SynchronizeWithNeighbors(floodgate, unfinishedOnly: true);
		}
	}

	private void SynchronizeNeighbors(Floodgate startingFloodgate, bool unfinishedOnly)
	{
		if (!startingFloodgate.IsSynchronized)
		{
			return;
		}
		EnqueueFloodgate(startingFloodgate);
		while (!_neighborsQueue.IsEmpty())
		{
			Floodgate floodgate = _neighborsQueue.Dequeue();
			BlockObject component = floodgate.GetComponent<BlockObject>();
			int maxHeight = floodgate.MaxHeight;
			Vector3Int[] neighbors4Vector3Int = Deltas.Neighbors4Vector3Int;
			foreach (Vector3Int vector3Int in neighbors4Vector3Int)
			{
				Vector3Int vector3Int2 = component.Coordinates + vector3Int;
				for (int j = 0; j < maxHeight; j++)
				{
					Vector3Int neighborCoords = vector3Int2 + new Vector3Int(0, 0, j);
					SynchronizeNeighbor(startingFloodgate, neighborCoords, unfinishedOnly);
				}
			}
		}
		_visitedNeighbors.Clear();
	}

	private void SynchronizeWithNeighbors(Floodgate floodgate, bool unfinishedOnly)
	{
		BlockObject component = floodgate.GetComponent<BlockObject>();
		Vector3Int[] neighbors4Vector3Int = Deltas.Neighbors4Vector3Int;
		foreach (Vector3Int vector3Int in neighbors4Vector3Int)
		{
			Vector3Int vector3Int2 = component.Coordinates + vector3Int;
			for (int j = 0; j < floodgate.MaxHeight; j++)
			{
				Vector3Int coordinates = vector3Int2 + new Vector3Int(0, 0, j);
				Floodgate bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<Floodgate>(coordinates);
				if (bottomObjectComponentAt != null && bottomObjectComponentAt.IsSynchronized)
				{
					SynchronizeNeighbors(bottomObjectComponentAt, unfinishedOnly);
					break;
				}
			}
		}
	}

	private void SynchronizeNeighbor(Floodgate sourceFloodgate, Vector3Int neighborCoords, bool unfinishedOnly)
	{
		Floodgate bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<Floodgate>(neighborCoords);
		if ((bool)bottomObjectComponentAt && bottomObjectComponentAt.IsSynchronized && !_visitedNeighbors.Contains(bottomObjectComponentAt))
		{
			BlockObject component = bottomObjectComponentAt.GetComponent<BlockObject>();
			if (!unfinishedOnly || !component.IsFinished)
			{
				int z = component.Coordinates.z;
				bottomObjectComponentAt.SetHeight(sourceFloodgate.PositionedHeight - (float)z);
				bottomObjectComponentAt.SetAutomationHeight(sourceFloodgate.PositionedAutomationHeight - (float)z);
				Automatable component2 = sourceFloodgate.GetComponent<Automatable>();
				bottomObjectComponentAt.GetComponent<Automatable>().SetInput(component2.Input);
				EnqueueFloodgate(bottomObjectComponentAt);
			}
		}
	}

	private void EnqueueFloodgate(Floodgate floodgate)
	{
		_neighborsQueue.Enqueue(floodgate);
		_visitedNeighbors.Add(floodgate);
	}
}
