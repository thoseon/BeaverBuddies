using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockObjectPickingSystem;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.CameraSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.GridTraversing;
using Timberborn.LevelVisibilitySystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainQueryingSystem;
using UnityEngine;

namespace Timberborn.AreaSelectionSystem;

public class SculptingTerrainPicker : ILoadableSingleton
{
	private readonly TerrainPicker _terrainPicker;

	private readonly AreaSelectionController _areaSelectionController;

	private readonly AreaIterator _areaIterator;

	private readonly CameraService _cameraService;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly BlockObjectRaycaster _blockObjectRaycaster;

	private readonly ISpecService _specService;

	private int _maxBlocks;

	public SculptingTerrainPicker(TerrainPicker terrainPicker, AreaSelectionController areaSelectionController, AreaIterator areaIterator, CameraService cameraService, ILevelVisibilityService levelVisibilityService, BlockObjectRaycaster blockObjectRaycaster, ISpecService specService)
	{
		_terrainPicker = terrainPicker;
		_areaSelectionController = areaSelectionController;
		_areaIterator = areaIterator;
		_cameraService = cameraService;
		_levelVisibilityService = levelVisibilityService;
		_blockObjectRaycaster = blockObjectRaycaster;
		_specService = specService;
	}

	public void Load()
	{
		_maxBlocks = _specService.GetSingleSpec<AreaPickersSpec>().SculptingMaxBlocks;
	}

	public bool PickTerrainAreaToAdd(AreaPicker.IntAreaCallback previewCallback, AreaPicker.IntAreaCallback actionCallback)
	{
		return _areaSelectionController.ProcessInput(delegate(Ray start, Ray end, bool _)
		{
			previewCallback(GetBlocksToAdd(start, end), start);
		}, delegate(Ray start, Ray end, bool _)
		{
			actionCallback(GetBlocksToAdd(start, end), start);
		}, delegate
		{
		});
	}

	public bool PickTerrainAreaToRemove(AreaPicker.IntAreaCallback previewCallback, AreaPicker.IntAreaCallback actionCallback)
	{
		return _areaSelectionController.ProcessInput(delegate(Ray start, Ray end, bool _)
		{
			previewCallback(GetBlocksToRemove(start, end), start);
		}, delegate(Ray start, Ray end, bool _)
		{
			actionCallback(GetBlocksToRemove(start, end), start);
		}, delegate
		{
		});
	}

	public void Reset()
	{
		_areaSelectionController.Reset();
	}

	private IEnumerable<Vector3Int> GetBlocksToAdd(Ray startRay, Ray endRay)
	{
		if (TryGetAddingStartBlock(startRay, out var block))
		{
			Vector3Int addingRectangleEndBlock = GetAddingRectangleEndBlock(block, endRay);
			return _areaIterator.GetRectangle(block, addingRectangleEndBlock, _maxBlocks);
		}
		return Enumerable.Empty<Vector3Int>();
	}

	private bool TryGetAddingStartBlock(Ray startRay, out Vector3Int block)
	{
		if (TryGetStackableBlockObjectCoordinates(startRay, out block))
		{
			return true;
		}
		TraversedCoordinates? traversedCoordinates = _terrainPicker.PickTerrainCoordinatesWithStump(startRay);
		if (traversedCoordinates.HasValue)
		{
			TraversedCoordinates valueOrDefault = traversedCoordinates.GetValueOrDefault();
			if (valueOrDefault.CoordinatesWithFaceOffset.z <= _levelVisibilityService.MaxVisibleLevel)
			{
				block = valueOrDefault.CoordinatesWithFaceOffset;
				return true;
			}
		}
		return false;
	}

	private Vector3Int GetAddingRectangleEndBlock(Vector3Int startBlock, Ray endRay)
	{
		if (TryGetStackableBlockObjectCoordinates(endRay, out var coordinates))
		{
			return coordinates;
		}
		TraversedCoordinates? traversedCoordinates = _terrainPicker.FindCoordinatesOnLevelInMap(endRay, startBlock.z);
		Vector3 a = CoordinateSystem.WorldToGrid(_cameraService.Transform.position);
		TraversedCoordinates? traversedCoordinates2 = _terrainPicker.PickTerrainCoordinatesWithStump(endRay);
		if (traversedCoordinates2.HasValue)
		{
			TraversedCoordinates valueOrDefault = traversedCoordinates2.GetValueOrDefault();
			if (valueOrDefault.CoordinatesWithFaceOffset.z <= _levelVisibilityService.MaxVisibleLevel)
			{
				if (a.z > (float)startBlock.z && traversedCoordinates.HasValue && Vector3.Distance(a, valueOrDefault.Intersection) > Vector3.Distance(a, traversedCoordinates.Value.Intersection))
				{
					return traversedCoordinates.Value.Coordinates;
				}
				return valueOrDefault.Coordinates;
			}
		}
		if (traversedCoordinates.HasValue && a.z > (float)startBlock.z)
		{
			return traversedCoordinates.Value.Coordinates;
		}
		return startBlock;
	}

	private IEnumerable<Vector3Int> GetBlocksToRemove(Ray startRay, Ray endRay)
	{
		if (TryGetStackableBlockObjectCoordinates(startRay, out var _))
		{
			return Enumerable.Empty<Vector3Int>();
		}
		TraversedCoordinates? traversedCoordinates = _terrainPicker.PickTerrainCoordinatesWithStump(startRay);
		if (traversedCoordinates.HasValue)
		{
			Vector3Int coordinates2 = traversedCoordinates.GetValueOrDefault().Coordinates;
			Vector3Int removingRectangleEndBlock = GetRemovingRectangleEndBlock(coordinates2, endRay);
			return _areaIterator.GetRectangle(coordinates2, removingRectangleEndBlock, _maxBlocks);
		}
		return Enumerable.Empty<Vector3Int>();
	}

	private Vector3Int GetRemovingRectangleEndBlock(Vector3Int startBlock, Ray endRay)
	{
		TraversedCoordinates? traversedCoordinates = _terrainPicker.PickTerrainCoordinatesWithStump(endRay);
		if (traversedCoordinates.HasValue)
		{
			return traversedCoordinates.Value.Coordinates;
		}
		return _terrainPicker.FindCoordinatesOnLevelInMap(endRay, startBlock.z)?.Coordinates ?? startBlock;
	}

	private bool TryGetStackableBlockObjectCoordinates(Ray ray, out Vector3Int coordinates)
	{
		if (_blockObjectRaycaster.TryHitBlockObject<BlockObject>(ray, out var blockObjectHit) && blockObjectHit.HitBlock.Stackable == BlockStackable.BlockObject)
		{
			coordinates = blockObjectHit.HitBlock.Coordinates.Above();
			return true;
		}
		coordinates = default(Vector3Int);
		return false;
	}
}
