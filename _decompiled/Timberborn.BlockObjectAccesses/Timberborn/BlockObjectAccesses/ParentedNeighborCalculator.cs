using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using UnityEngine;

namespace Timberborn.BlockObjectAccesses;

public class ParentedNeighborCalculator : BaseComponent, IAwakableComponent
{
	private readonly NeighborCalculator _neighborCalculator;

	private BlockObject _blockObject;

	public ParentedNeighborCalculator(NeighborCalculator neighborCalculator)
	{
		_neighborCalculator = neighborCalculator;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public IEnumerable<ParentedNeighbor2D> GetNonInternalParentedNeighbors()
	{
		return _neighborCalculator.GetNonInternalParentedNeighborsWithDiagonal(GetBaseLevelOccupiedCoordinates()).Select(ParentedNeighbor2D.From3D).Distinct();
	}

	private IEnumerable<Vector3Int> GetBaseLevelOccupiedCoordinates()
	{
		return from coords in _blockObject.PositionedBlocks.GetOccupiedCoordinates()
			where coords.z == _blockObject.CoordinatesAtBaseZ.z
			select coords;
	}
}
