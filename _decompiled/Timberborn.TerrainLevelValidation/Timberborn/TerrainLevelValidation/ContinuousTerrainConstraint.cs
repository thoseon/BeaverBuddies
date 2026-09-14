using System;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.MapIndexSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.TerrainLevelValidation;

internal class ContinuousTerrainConstraint : BaseComponent, IAwakableComponent, IInfiniteUndergroundModel
{
	private readonly ITerrainService _terrainService;

	private readonly MapIndexService _mapIndexService;

	private BlockObject _blockObject;

	public ContinuousTerrainConstraint(ITerrainService terrainService, MapIndexService mapIndexService)
	{
		_terrainService = terrainService;
		_mapIndexService = mapIndexService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		if (_blockObject.Blocks.GetAllBlocks().Any((Block block) => block.MatterBelow == MatterBelow.Ground && !block.OccupyAllBelow))
		{
			throw new NotSupportedException($"Some of the ground blocks of {this} " + "are not set to occupy all below.");
		}
	}

	public bool IsNotOnFirstColumnOfTerrain()
	{
		foreach (Vector3Int foundationCoordinate in _blockObject.PositionedBlocks.GetFoundationCoordinates())
		{
			if (_terrainService.Contains(foundationCoordinate))
			{
				int index3D = _mapIndexService.CellToIndex(foundationCoordinate.XY());
				if (_terrainService.GetColumnCeiling(index3D) < foundationCoordinate.z)
				{
					return true;
				}
			}
		}
		return false;
	}
}
