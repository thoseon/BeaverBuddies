using System.Collections.Generic;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.BlockObstacles;

public class BlockOccupationLayer
{
	private readonly List<BlockOccupier> _blockOccupiers = new List<BlockOccupier>();

	public int GridHeight { get; }

	public BlockOccupationLayer(int gridHeight)
	{
		GridHeight = gridHeight;
	}

	public void AddBlockOccupier(BlockOccupier blockOccupier)
	{
		_blockOccupiers.Add(blockOccupier);
	}

	public bool CanBeAddedToServices()
	{
		foreach (BlockOccupier blockOccupier in _blockOccupiers)
		{
			if (!blockOccupier.CanBeAddedToServices())
			{
				return false;
			}
		}
		return true;
	}

	public void AddToServices()
	{
		foreach (BlockOccupier blockOccupier in _blockOccupiers)
		{
			blockOccupier.AddToServices();
		}
	}

	public void RemoveFromServices()
	{
		foreach (BlockOccupier blockOccupier in _blockOccupiers)
		{
			blockOccupier.RemoveFromServices();
		}
	}

	public bool Contains(Vector2Int coordinates)
	{
		foreach (BlockOccupier blockOccupier in _blockOccupiers)
		{
			if (blockOccupier.BlockObject.Coordinates.XY() == coordinates)
			{
				return true;
			}
		}
		return false;
	}

	public void Remove()
	{
		RemoveFromServices();
		foreach (BlockOccupier blockOccupier in _blockOccupiers)
		{
			Object.Destroy(blockOccupier.GameObject);
		}
	}
}
