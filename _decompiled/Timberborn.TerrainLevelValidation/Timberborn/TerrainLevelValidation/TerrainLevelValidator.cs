using Timberborn.BlockSystem;
using Timberborn.Localization;
using Timberborn.MapStateSystem;
using UnityEngine;

namespace Timberborn.TerrainLevelValidation;

internal class TerrainLevelValidator : IBlockObjectValidator
{
	private static readonly string BottomConstraintLocKey = "Buildings.BottomConstraint";

	private static readonly string TopConstraintLocKey = "Buildings.TopConstraint";

	private readonly ILoc _loc;

	private readonly MapSize _mapSize;

	public TerrainLevelValidator(ILoc loc, MapSize mapSize)
	{
		_loc = loc;
		_mapSize = mapSize;
	}

	public bool IsValid(BlockObject blockObject, out string errorMessage)
	{
		if (IsBottomConstraintDissatisfied(blockObject))
		{
			errorMessage = _loc.T(BottomConstraintLocKey);
			return false;
		}
		if (IsTopTerrainConstraintDissatisfied(blockObject) || IsTopBlockObjectConstraintDissatisfied(blockObject))
		{
			errorMessage = _loc.T(TopConstraintLocKey);
			return false;
		}
		errorMessage = null;
		return true;
	}

	private static bool IsBottomConstraintDissatisfied(BlockObject blockObject)
	{
		BottomTerrainLevelValidationConstraint component = blockObject.GetComponent<BottomTerrainLevelValidationConstraint>();
		if (component != null)
		{
			return component.BottomLevel <= 0;
		}
		return false;
	}

	private bool IsTopTerrainConstraintDissatisfied(BlockObject blockObject)
	{
		if (blockObject.GetComponent<TopTerrainLevelValidationConstraint>() != null)
		{
			return blockObject.Coordinates.z >= _mapSize.MaxGameTerrainHeight;
		}
		return false;
	}

	private bool IsTopBlockObjectConstraintDissatisfied(BlockObject blockObject)
	{
		int z = _mapSize.TotalSize.z;
		foreach (Vector3Int occupiedCoordinate in blockObject.PositionedBlocks.GetOccupiedCoordinates())
		{
			if (occupiedCoordinate.z >= z)
			{
				return true;
			}
		}
		return false;
	}
}
