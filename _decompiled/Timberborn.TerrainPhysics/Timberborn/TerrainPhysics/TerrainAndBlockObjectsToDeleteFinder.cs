using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.TerrainPhysics;

internal class TerrainAndBlockObjectsToDeleteFinder : ILoadableSingleton
{
	private readonly IBlockService _blockService;

	private readonly TerrainPhysicsValidatorFactory _terrainPhysicsValidatorFactory;

	private readonly StackableBlockService _stackableBlockService;

	private readonly SupportsToBeDeleted _supportsToBeDeleted;

	private readonly ITerrainService _terrainService;

	private readonly TerrainOnBlockObjectFinder _terrainOnBlockObjectFinder;

	private readonly Queue<Vector3Int> _terrainToCheck = new Queue<Vector3Int>();

	private readonly Queue<BlockObject> _blockObjectsToCheck = new Queue<BlockObject>();

	private readonly HashSet<Vector3Int> _checkedTerrain = new HashSet<Vector3Int>();

	private readonly Queue<Vector3Int> _blockObjectCoordinatesToCheck = new Queue<Vector3Int>();

	private TerrainPhysicsValidator _terrainPhysicsValidator;

	public TerrainAndBlockObjectsToDeleteFinder(IBlockService blockService, TerrainPhysicsValidatorFactory terrainPhysicsValidatorFactory, StackableBlockService stackableBlockService, SupportsToBeDeleted supportsToBeDeleted, ITerrainService terrainService, TerrainOnBlockObjectFinder terrainOnBlockObjectFinder)
	{
		_blockService = blockService;
		_terrainPhysicsValidatorFactory = terrainPhysicsValidatorFactory;
		_stackableBlockService = stackableBlockService;
		_supportsToBeDeleted = supportsToBeDeleted;
		_terrainService = terrainService;
		_terrainOnBlockObjectFinder = terrainOnBlockObjectFinder;
	}

	public void Load()
	{
		_terrainPhysicsValidator = _terrainPhysicsValidatorFactory.CreatePreviewValidator();
	}

	public void FindAll(IEnumerable<Vector3Int> inputTerrain, HashSet<Vector3Int> outputTerrain, HashSet<BlockObject> outputBlockObjects)
	{
		ProcessInputTerrain(inputTerrain, markAsDeleted: false);
		GetTerrainAndBlockObjectsToDelete(outputTerrain, outputBlockObjects);
	}

	public void FindAllMarkInputAsDeleted(IEnumerable<Vector3Int> inputTerrain, IEnumerable<BlockObject> inputBlockObjects, HashSet<Vector3Int> outputTerrain, HashSet<BlockObject> outputBlockObjects)
	{
		ProcessInputTerrain(inputTerrain, markAsDeleted: true);
		ProcessInputBlockObjects(inputBlockObjects);
		GetTerrainAndBlockObjectsToDelete(outputTerrain, outputBlockObjects);
	}

	public void FindAll(IEnumerable<BlockObject> inputBlockObjects, HashSet<Vector3Int> outputTerrain, HashSet<BlockObject> outputBlockObjects)
	{
		ProcessInputBlockObjects(inputBlockObjects);
		GetTerrainAndBlockObjectsToDelete(outputTerrain, outputBlockObjects);
	}

	private void ProcessInputTerrain(IEnumerable<Vector3Int> inputTerrain, bool markAsDeleted)
	{
		foreach (Vector3Int item in inputTerrain)
		{
			_terrainToCheck.Enqueue(item);
			Vector3Int vector3Int = item.Above();
			if (_terrainService.Underground(vector3Int))
			{
				_terrainToCheck.Enqueue(vector3Int);
			}
			if (markAsDeleted)
			{
				_supportsToBeDeleted.Mark(item);
			}
		}
	}

	private void ProcessInputBlockObjects(IEnumerable<BlockObject> inputBlockObjects)
	{
		foreach (BlockObject inputBlockObject in inputBlockObjects)
		{
			_terrainOnBlockObjectFinder.Find(inputBlockObject, _terrainToCheck);
			MarkBlockObjectBlocksForDeletion(inputBlockObject);
			AddNextBlockObjectToValidate(inputBlockObject);
		}
	}

	private void GetTerrainAndBlockObjectsToDelete(HashSet<Vector3Int> outputTerrain, HashSet<BlockObject> outputBlockObjects)
	{
		while (_terrainToCheck.Count > 0 || _blockObjectCoordinatesToCheck.Count > 0)
		{
			_terrainPhysicsValidator.CheckTerrainForDestruction(_terrainToCheck, _checkedTerrain);
			_terrainToCheck.Clear();
			AddTerrainAndTerrainBlockToDelete(outputTerrain, outputBlockObjects);
			AddBlockObjectStackToDelete(outputBlockObjects);
			foreach (BlockObject item in _blockObjectsToCheck)
			{
				_terrainOnBlockObjectFinder.Find(item, _terrainToCheck);
			}
			_blockObjectsToCheck.Clear();
		}
		_supportsToBeDeleted.Clear();
	}

	private void AddTerrainAndTerrainBlockToDelete(HashSet<Vector3Int> outputTerrain, HashSet<BlockObject> outputBlockObjects)
	{
		foreach (Vector3Int item in _checkedTerrain)
		{
			if (_stackableBlockService.IsUnfinishedGroundBlockAt(item))
			{
				BlockObject bottomObjectAt = _blockService.GetBottomObjectAt(item);
				if (!outputBlockObjects.Contains(bottomObjectAt))
				{
					_blockObjectsToCheck.Enqueue(bottomObjectAt);
				}
			}
			else if (!outputTerrain.Contains(item))
			{
				_terrainToCheck.Enqueue(item);
				_supportsToBeDeleted.Mark(item);
				foreach (BlockObject item2 in _blockService.GetObjectsAt(item))
				{
					if (item2.PositionedBlocks.GetBlock(item).Underground)
					{
						outputBlockObjects.Add(item2);
					}
				}
			}
			_blockObjectCoordinatesToCheck.Enqueue(item.Above());
		}
		_checkedTerrain.Clear();
		outputTerrain.AddRange(_terrainToCheck);
	}

	private void AddBlockObjectStackToDelete(HashSet<BlockObject> outputBlockObjects)
	{
		while (_blockObjectCoordinatesToCheck.Count > 0)
		{
			CheckBlockObjectStackToDelete(_blockObjectCoordinatesToCheck.Dequeue(), outputBlockObjects);
		}
		outputBlockObjects.AddRange(_blockObjectsToCheck);
	}

	private void CheckBlockObjectStackToDelete(Vector3Int coordinates, HashSet<BlockObject> outputBlockObjects)
	{
		foreach (BlockObject item in _blockService.GetObjectsAt(coordinates))
		{
			if (IsBlockValidForDeletion(coordinates, item, out var block) && !outputBlockObjects.Contains(item) && item.GetComponent<INonStackPickable>() == null)
			{
				_blockObjectsToCheck.Enqueue(item);
				MarkBlockObjectBlocksForDeletion(item);
				AddNextBlockObjectToValidate(item);
			}
			else if (block.Stackable.IsUnfinishedGround())
			{
				AddUnfinishedGroundBlockToCheck(item);
			}
		}
	}

	private bool IsBlockValidForDeletion(Vector3Int coordinates, BlockObject blockObject, out Block block)
	{
		block = blockObject.PositionedBlocks.GetBlock(coordinates);
		Block block2 = block;
		if (!block2.Underground || block2.MatterBelow != MatterBelow.Ground)
		{
			if (block.IsFoundationBlock)
			{
				return !block.Stackable.IsUnfinishedGround();
			}
			return false;
		}
		return true;
	}

	private void MarkBlockObjectBlocksForDeletion(BlockObject blockObject)
	{
		foreach (Block occupiedBlock in blockObject.PositionedBlocks.GetOccupiedBlocks())
		{
			if (occupiedBlock.Stackable.IsStackable() || occupiedBlock.Stackable.IsUnfinishedGround())
			{
				_supportsToBeDeleted.Mark(occupiedBlock.Coordinates);
			}
		}
	}

	private void AddUnfinishedGroundBlockToCheck(BlockObject blockObject)
	{
		foreach (Block occupiedBlock in blockObject.PositionedBlocks.GetOccupiedBlocks())
		{
			if (occupiedBlock.Stackable.IsUnfinishedGround())
			{
				_terrainToCheck.Enqueue(occupiedBlock.Coordinates);
			}
		}
	}

	private void AddNextBlockObjectToValidate(BlockObject blockObject)
	{
		foreach (Block occupiedBlock in blockObject.PositionedBlocks.GetOccupiedBlocks())
		{
			if (!occupiedBlock.Stackable.IsStackable())
			{
				continue;
			}
			Vector3Int vector3Int = occupiedBlock.Coordinates.Above();
			bool flag = IsUnderground(vector3Int);
			if (!flag && !_blockObjectCoordinatesToCheck.Contains(vector3Int))
			{
				_blockObjectCoordinatesToCheck.Enqueue(vector3Int);
			}
			else if (flag)
			{
				BlockObject undergroundObjectAt = _blockService.GetUndergroundObjectAt(vector3Int);
				if ((bool)undergroundObjectAt && undergroundObjectAt.PositionedBlocks.GetBlock(vector3Int).MatterBelow == MatterBelow.GroundOrStackable)
				{
					_blockObjectCoordinatesToCheck.Enqueue(vector3Int);
				}
				_terrainToCheck.Enqueue(vector3Int);
			}
		}
	}

	private bool IsUnderground(Vector3Int coordinates)
	{
		if (!_terrainService.Underground(coordinates))
		{
			return _stackableBlockService.IsUnfinishedGroundBlockAt(coordinates);
		}
		return true;
	}
}
