using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.BlockObjectAccesses;

public class NeighborCalculator
{
	private readonly HashSet<Vector3Int> _checkedNeighbors = new HashSet<Vector3Int>();

	private readonly HashSet<Vector3Int> _blocks = new HashSet<Vector3Int>();

	public IEnumerable<Vector3Int> GetNonInternalNeighborsWithoutDiagonal(IEnumerable<Vector3Int> blocks)
	{
		return from neighbor in GetNeighbors(blocks, Deltas.Neighbors4Vector3Int)
			select neighbor.Neighbor;
	}

	public IEnumerable<ParentedNeighbor> GetNonInternalParentedNeighborsWithDiagonal(IEnumerable<Vector3Int> blocks)
	{
		return GetNeighbors(blocks, Deltas.Neighbors8Vector3IntOrdered, allowNeighboursDuplicate: true);
	}

	private IEnumerable<ParentedNeighbor> GetNeighbors(IEnumerable<Vector3Int> blocks, IEnumerable<Vector3Int> neighborDeltas, bool allowNeighboursDuplicate = false)
	{
		_blocks.AddRange(blocks);
		_checkedNeighbors.AddRange(_blocks);
		foreach (Vector3Int delta in neighborDeltas)
		{
			foreach (Vector3Int block in _blocks)
			{
				Vector3Int vector3Int = block + delta;
				if (!_blocks.Contains(vector3Int) && (allowNeighboursDuplicate || _checkedNeighbors.Add(vector3Int)))
				{
					yield return new ParentedNeighbor(vector3Int, block);
				}
			}
		}
		_blocks.Clear();
		_checkedNeighbors.Clear();
	}
}
