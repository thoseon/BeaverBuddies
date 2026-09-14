using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.GridTraversing;
using Timberborn.LevelVisibilitySystem;
using Timberborn.SelectionSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.BlockObjectPickingSystem;

public class BlockObjectPreviewPicker
{
	private readonly ITerrainService _terrainService;

	private readonly GridTraversal _gridTraversal;

	private readonly StackableBlockService _stackableBlockService;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly SelectableObjectRaycaster _selectableObjectRaycaster;

	public BlockObjectPreviewPicker(ITerrainService terrainService, GridTraversal gridTraversal, StackableBlockService stackableBlockService, ILevelVisibilityService levelVisibilityService, SelectableObjectRaycaster selectableObjectRaycaster)
	{
		_terrainService = terrainService;
		_gridTraversal = gridTraversal;
		_stackableBlockService = stackableBlockService;
		_levelVisibilityService = levelVisibilityService;
		_selectableObjectRaycaster = selectableObjectRaycaster;
	}

	public PickedCoordinates? CenteredPreviewCoordinates(PlaceableBlockObjectSpec placeableBlockObjectSpec, Orientation orientation, Ray ray)
	{
		BlockObjectSpec spec = placeableBlockObjectSpec.GetSpec<BlockObjectSpec>();
		CustomPivotSpec customPivot = placeableBlockObjectSpec.CustomPivot;
		BlockObject objectHitByRaycast = GetObjectHitByRaycast();
		bool canBeAttachedToTerrainSide = placeableBlockObjectSpec.CanBeAttachedToTerrainSide;
		bool flag = spec.Blocks.FastAll((BlockSpec block) => block.Underground);
		foreach (TraversedCoordinates item in _gridTraversal.TraverseRay(ray))
		{
			Vector3Int coordinates = item.Coordinates;
			bool flag2 = item.Face.z == 1;
			if (_terrainService.Contains(coordinates.XY()))
			{
				if (flag2)
				{
					if ((flag && IsTerrainWithStump(coordinates)) || (!flag && IsTerrainOrStackable(coordinates)))
					{
						Vector3Int vector3Int = ComposeCoordinates(orientation, customPivot, spec, item);
						return flag ? new PickedCoordinates?(new PickedCoordinates(vector3Int.Below(), vector3Int.z, -1, canBeAttachedToTerrainSide)) : new PickedCoordinates?(new PickedCoordinates(vector3Int, vector3Int.z, 0, canBeAttachedToTerrainSide));
					}
				}
				else if ((flag && IsTerrainWithStump(coordinates)) || (canBeAttachedToTerrainSide && IsTerrainOrUnfinishedTerrain(coordinates)))
				{
					Vector3Int coordinates2 = ComposeCoordinates(orientation, customPivot, spec, item);
					if (canBeAttachedToTerrainSide)
					{
						coordinates2 += item.Face;
					}
					bool filterOverhangingCoordinates = HasStackableBelow(coordinates2);
					return new PickedCoordinates(coordinates2, item.Intersection.z, 0, filterOverhangingCoordinates);
				}
			}
			if (ShouldObjectBlockCoordinates(objectHitByRaycast, coordinates))
			{
				return null;
			}
		}
		return null;
	}

	private BlockObject GetObjectHitByRaycast()
	{
		if (_selectableObjectRaycaster.TryHitSelectableObject(out var hitObject))
		{
			Preview component = hitObject.GetComponent<Preview>();
			if ((bool)component && !component.PreviewState.IsLast)
			{
				return hitObject.GetComponent<BlockObject>();
			}
		}
		return null;
	}

	private bool IsTerrainOrUnfinishedTerrain(Vector3Int coords)
	{
		if (_terrainService.Underground(coords) || _stackableBlockService.IsUnfinishedGroundBlockAt(coords))
		{
			return coords.z <= _levelVisibilityService.MaxVisibleLevel;
		}
		return false;
	}

	private bool IsTerrainOrStackable(Vector3Int coords)
	{
		if (!IsTerrain(coords))
		{
			return IsStackable(coords);
		}
		return true;
	}

	private bool IsTerrainWithStump(Vector3Int coords)
	{
		if (_terrainService.Underground(coords))
		{
			return coords.z <= _levelVisibilityService.MaxVisibleLevel;
		}
		return false;
	}

	private bool HasStackableBelow(Vector3Int coordinates)
	{
		Vector3Int coords = coordinates.Below();
		if (!_terrainService.Underground(coords))
		{
			return _stackableBlockService.IsStackableBlockAt(coords);
		}
		return true;
	}

	private bool IsTerrain(Vector3Int coords)
	{
		if (_terrainService.Underground(coords))
		{
			return coords.z < _levelVisibilityService.MaxVisibleLevel;
		}
		return false;
	}

	private bool IsStackable(Vector3Int coords)
	{
		if (_levelVisibilityService.BlockIsVisible(coords + new Vector3Int(0, 0, 1)))
		{
			return _stackableBlockService.IsStackableBlockAt(coords);
		}
		return false;
	}

	private static Vector3Int ComposeCoordinates(Orientation orientation, CustomPivotSpec customPivot, BlockObjectSpec blockObjectSpec, TraversedCoordinates candidate)
	{
		Vector3 centerOffset = (customPivot.HasCustomPivot ? orientation.Transform(customPivot.Coordinates) : blockObjectSpec.CalculateCenterOffset(orientation));
		return CenterCoordinates(candidate, centerOffset) - new Vector3Int(0, 0, blockObjectSpec.BaseZ);
	}

	private static Vector3Int CenterCoordinates(TraversedCoordinates traversedCoordinates, Vector3 centerOffset)
	{
		Vector3 vector = FaceAdjustedIntersection(traversedCoordinates);
		Vector3 vector2 = new Vector3(vector.x - centerOffset.x + 0.5f * Mathf.Sign(centerOffset.x), vector.y - centerOffset.y + 0.5f * Mathf.Sign(centerOffset.y), vector.z);
		return new Vector3Int(Mathf.FloorToInt(vector2.x), Mathf.FloorToInt(vector2.y), Mathf.FloorToInt(vector2.z));
	}

	private static Vector3 FaceAdjustedIntersection(TraversedCoordinates traversedCoordinates)
	{
		Vector3Int face = traversedCoordinates.Face;
		Vector3 vector = new Vector3((float)face.x * -0.001f, (float)face.y * -0.001f, 0.001f);
		return traversedCoordinates.Intersection + vector;
	}

	private static bool ShouldObjectBlockCoordinates(BlockObject blockObject, Vector3Int coordinates)
	{
		if ((bool)blockObject && !blockObject.IsPreview && !blockObject.Overridable && blockObject.PositionedBlocks.TryGetBlock(coordinates, out var result) && result.Occupation.IsFull())
		{
			return DoesBlockAboveHasFullOccupation(blockObject, result);
		}
		return false;
	}

	private static bool DoesBlockAboveHasFullOccupation(BlockObject blockObject, Block block)
	{
		Vector3Int coordinates = new Vector3Int(block.Coordinates.x, block.Coordinates.y, block.Coordinates.z + 1);
		if (blockObject.PositionedBlocks.TryGetBlock(coordinates, out var result))
		{
			return result.Occupation.IsFull();
		}
		return false;
	}
}
