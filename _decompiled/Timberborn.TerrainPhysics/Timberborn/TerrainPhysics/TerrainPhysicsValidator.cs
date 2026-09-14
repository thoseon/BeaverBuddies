using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.TerrainPhysics;

internal class TerrainPhysicsValidator
{
	public static readonly int MaxSupportDistance = 3;

	private static readonly int MaxSupportDistanceDoubled = MaxSupportDistance * 2;

	private readonly ITerrainService _terrainService;

	private readonly StackableBlockService _stackableBlockService;

	private readonly PreviewBlockService _previewBlockService;

	private readonly SupportsToBeDeleted _supportsToBeDeleted;

	private readonly bool _validatePreviewBlocks;

	private readonly HashSet<Vector3Int> _forcedTerrain = new HashSet<Vector3Int>();

	private readonly HashSet<Vector3Int> _extendedSearchArea = new HashSet<Vector3Int>();

	private readonly Queue<Vector3Int> _coordinatesToVisit = new Queue<Vector3Int>();

	private readonly HashSet<Vector3Int> _checkedArea = new HashSet<Vector3Int>();

	private readonly HashSet<Vector3Int> _addedDataPoints = new HashSet<Vector3Int>();

	private readonly Dictionary<Vector3Int, int> _distanceField = new Dictionary<Vector3Int, int>();

	private readonly List<Vector3Int> _extendedSearchAreaOffsets = new List<Vector3Int>();

	private readonly HashSet<Vector3Int> _checkedAreaOffsets = new HashSet<Vector3Int>();

	public TerrainPhysicsValidator(ITerrainService terrainService, StackableBlockService stackableBlockService, PreviewBlockService previewBlockService, SupportsToBeDeleted supportsToBeDeleted, bool validatePreviewBlocks)
	{
		_terrainService = terrainService;
		_stackableBlockService = stackableBlockService;
		_previewBlockService = previewBlockService;
		_supportsToBeDeleted = supportsToBeDeleted;
		_validatePreviewBlocks = validatePreviewBlocks;
	}

	public void Initialize()
	{
		PrepareValidationOffsetData();
	}

	public void CheckTerrainForDestruction(Queue<Vector3Int> terrainToCheck, HashSet<Vector3Int> terrainQueuedForDestruction)
	{
		while (terrainToCheck.Count > 0)
		{
			while (terrainToCheck.Count > 0)
			{
				AddDataToSets(terrainToCheck.Dequeue());
			}
			BuildDistanceField();
			foreach (Vector3Int item in _checkedArea)
			{
				if (!AreCoordinatesInvalid(item) || !terrainQueuedForDestruction.Add(item))
				{
					continue;
				}
				_supportsToBeDeleted.Mark(item);
				Vector3Int vector3Int = item.Above();
				if (!terrainQueuedForDestruction.Contains(vector3Int) && IsUnderground(vector3Int))
				{
					terrainToCheck.Enqueue(vector3Int);
				}
				Vector3Int[] corners4Vector3Int = Deltas.Corners4Vector3Int;
				foreach (Vector3Int vector3Int2 in corners4Vector3Int)
				{
					Vector3Int vector3Int3 = item + vector3Int2;
					if (!terrainQueuedForDestruction.Contains(vector3Int3) && IsUnderground(vector3Int3))
					{
						terrainToCheck.Enqueue(vector3Int3);
					}
				}
			}
			ClearData();
		}
		ClearAllData();
	}

	public void GetValidTerrainToAdd(ICollection<Vector3Int> inputTerrain, HashSet<Vector3Int> terrainToAdd)
	{
		foreach (Vector3Int item in inputTerrain)
		{
			_forcedTerrain.Add(item);
			AddDataToSets(item);
		}
		BuildDistanceField();
		foreach (Vector3Int item2 in inputTerrain)
		{
			if (IsTerrainValid(item2))
			{
				terrainToAdd.Add(item2);
			}
		}
		ClearAllData();
	}

	public bool ValidateBlockObjectPreview(BlockObject blockObject)
	{
		foreach (Vector3Int allCoordinate in blockObject.PositionedBlocks.GetAllCoordinates())
		{
			_forcedTerrain.Add(allCoordinate);
			AddDataToSets(allCoordinate);
		}
		BuildDistanceField();
		bool result = ValidateCheckedArea();
		ClearAllData();
		return result;
	}

	public bool CanTerrainBeAdded(Vector3Int coordinates)
	{
		_forcedTerrain.Add(coordinates);
		AddDataToSets(coordinates);
		BuildDistanceField();
		bool result = IsTerrainValid(coordinates);
		ClearAllData();
		return result;
	}

	public bool CanBeDestroyed(BlockObject blockObject)
	{
		ImmutableArray<Block> allBlocks = blockObject.PositionedBlocks.GetAllBlocks();
		MarkStackableBlocksForDeletion(allBlocks);
		AddDataToSetsFromStackableBlocks(allBlocks);
		BuildDistanceField();
		bool result = ValidateCheckedArea();
		ClearAllData();
		_supportsToBeDeleted.Clear();
		return result;
	}

	private void PrepareValidationOffsetData()
	{
		for (int i = -MaxSupportDistanceDoubled; i <= MaxSupportDistanceDoubled; i++)
		{
			int num = Mathf.Abs(i);
			for (int j = -MaxSupportDistanceDoubled; j <= MaxSupportDistanceDoubled; j++)
			{
				int num2 = Mathf.Abs(j) + num;
				Vector3Int item = new Vector3Int(j, i, 0);
				if (num2 <= MaxSupportDistanceDoubled)
				{
					_extendedSearchAreaOffsets.Add(item);
					if (num2 <= MaxSupportDistance)
					{
						_checkedAreaOffsets.Add(item);
					}
				}
			}
		}
	}

	private void AddDataToSets(Vector3Int coordinates)
	{
		if (!_addedDataPoints.Add(coordinates))
		{
			return;
		}
		foreach (Vector3Int extendedSearchAreaOffset in _extendedSearchAreaOffsets)
		{
			Vector3Int vector3Int = coordinates + extendedSearchAreaOffset;
			if (IsUnderground(vector3Int))
			{
				if (_checkedAreaOffsets.Contains(extendedSearchAreaOffset))
				{
					_checkedArea.Add(vector3Int);
				}
				if (_extendedSearchArea.Add(vector3Int) && HasSupport(vector3Int))
				{
					_coordinatesToVisit.Enqueue(vector3Int);
					_distanceField[vector3Int] = 0;
				}
			}
		}
	}

	private void BuildDistanceField()
	{
		while (_coordinatesToVisit.Count > 0)
		{
			Vector3Int vector3Int = _coordinatesToVisit.Dequeue();
			int num = _distanceField[vector3Int] + 1;
			bool flag = _terrainService.Underground(vector3Int);
			Vector3Int[] neighbors4Vector3Int = Deltas.Neighbors4Vector3Int;
			foreach (Vector3Int vector3Int2 in neighbors4Vector3Int)
			{
				Vector3Int vector3Int3 = vector3Int + vector3Int2;
				bool flag2 = _terrainService.Underground(vector3Int3);
				if ((flag || !flag2) && _extendedSearchArea.Contains(vector3Int3) && IsUnderground(vector3Int3) && !_supportsToBeDeleted.IsMarked(vector3Int3) && (!_distanceField.TryGetValue(vector3Int3, out var value) || value > num))
				{
					_distanceField[vector3Int3] = num;
					_coordinatesToVisit.Enqueue(vector3Int3);
				}
			}
		}
	}

	private bool AreCoordinatesInvalid(Vector3Int coordinates)
	{
		if (IsUnderground(coordinates))
		{
			if (_distanceField.TryGetValue(coordinates, out var value))
			{
				return value > MaxSupportDistance;
			}
			return true;
		}
		return false;
	}

	private void ClearAllData()
	{
		_forcedTerrain.Clear();
		_addedDataPoints.Clear();
		ClearData();
	}

	private void ClearData()
	{
		_extendedSearchArea.Clear();
		_checkedArea.Clear();
		_distanceField.Clear();
		_forcedTerrain.Clear();
	}

	private bool IsTerrainValid(Vector3Int coordinates)
	{
		if (_distanceField.TryGetValue(coordinates, out var value))
		{
			return value <= MaxSupportDistance;
		}
		return false;
	}

	private bool ValidateCheckedArea()
	{
		foreach (Vector3Int item in _checkedArea)
		{
			if (!_supportsToBeDeleted.IsMarked(item) && (!_distanceField.TryGetValue(item, out var value) || value > MaxSupportDistance))
			{
				return false;
			}
		}
		return true;
	}

	private void MarkStackableBlocksForDeletion(ImmutableArray<Block> allBlocks)
	{
		foreach (Block item in allBlocks)
		{
			if (item.Stackable.IsStackable())
			{
				_supportsToBeDeleted.Mark(item.Coordinates);
			}
		}
	}

	private void AddDataToSetsFromStackableBlocks(ImmutableArray<Block> allBlocks)
	{
		foreach (Block item in allBlocks)
		{
			if (item.Stackable.IsStackable())
			{
				Vector3Int coordinates = item.Coordinates;
				AddDataToSets(coordinates.Above());
				AddDataToSets(coordinates);
			}
		}
	}

	private bool IsUnderground(Vector3Int coordinates)
	{
		if (!_supportsToBeDeleted.IsMarked(coordinates))
		{
			if (!_forcedTerrain.Contains(coordinates) && !_terrainService.Underground(coordinates))
			{
				if (_validatePreviewBlocks)
				{
					if (!_stackableBlockService.IsUnfinishedGroundBlockAt(coordinates))
					{
						return _previewBlockService.IsUnfinishedGroundBlockAt(coordinates);
					}
					return true;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	private bool HasSupport(Vector3Int coordinates)
	{
		Vector3Int vector3Int = coordinates.Below();
		if (!_supportsToBeDeleted.IsMarked(vector3Int))
		{
			if (!PreviewHasSupport(coordinates, vector3Int))
			{
				return TerrainHasSupport(coordinates, vector3Int);
			}
			return true;
		}
		return false;
	}

	private bool PreviewHasSupport(Vector3Int coordinates, Vector3Int coordinatesBelow)
	{
		if (_validatePreviewBlocks && (_forcedTerrain.Contains(coordinates) || _stackableBlockService.IsUnfinishedGroundBlockAt(coordinates) || _previewBlockService.IsUnfinishedGroundBlockAt(coordinates)))
		{
			if (!_forcedTerrain.Contains(coordinatesBelow) && !_terrainService.Underground(coordinatesBelow) && !_stackableBlockService.IsStackableBlockAt(coordinatesBelow))
			{
				return _previewBlockService.IsUnfinishedGroundBlockAt(coordinatesBelow);
			}
			return true;
		}
		return false;
	}

	private bool TerrainHasSupport(Vector3Int coordinates, Vector3Int coordinatesBelow)
	{
		if (_forcedTerrain.Contains(coordinates) || _terrainService.Underground(coordinates))
		{
			if (!_forcedTerrain.Contains(coordinatesBelow) && !_terrainService.Underground(coordinatesBelow))
			{
				return _stackableBlockService.IsFinishedStackableBlockAt(coordinatesBelow);
			}
			return true;
		}
		return false;
	}
}
