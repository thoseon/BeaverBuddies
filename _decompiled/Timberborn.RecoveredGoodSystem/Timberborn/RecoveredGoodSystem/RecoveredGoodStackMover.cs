using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.RecoveredGoodSystem;

internal class RecoveredGoodStackMover : BaseComponent, IAwakableComponent, IInitializableEntity, IBlockObjectCustomOverriding, IDeletableEntity
{
	private readonly RecoveredGoodStackCoordinatesFinder _recoveredGoodStackCoordinatesFinder;

	private readonly ITerrainService _terrainService;

	private readonly IBlockService _blockService;

	private readonly EventBus _eventBus;

	private BlockObject _blockObject;

	private RecoveredGoodStack _recoveredGoodStack;

	private RecoveredGoodStackAccessible _recoveredGoodStackAccessible;

	private BlockObject _blockObjectBelow;

	public RecoveredGoodStackMover(RecoveredGoodStackCoordinatesFinder recoveredGoodStackCoordinatesFinder, ITerrainService terrainService, IBlockService blockService, EventBus eventBus)
	{
		_recoveredGoodStackCoordinatesFinder = recoveredGoodStackCoordinatesFinder;
		_terrainService = terrainService;
		_blockService = blockService;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_recoveredGoodStack = GetComponent<RecoveredGoodStack>();
		_recoveredGoodStackAccessible = GetComponent<RecoveredGoodStackAccessible>();
		_terrainService.TerrainHeightChanged += OnTerrainHeightChanged;
	}

	public void InitializeEntity()
	{
		_blockObjectBelow = GetBlockObjectBelow(_blockObject.CoordinatesAtBaseZ.Below());
		if ((bool)_blockObjectBelow)
		{
			_eventBus.Register(this);
		}
	}

	public void DeleteEntity()
	{
		_eventBus.Unregister(this);
		_terrainService.TerrainHeightChanged -= OnTerrainHeightChanged;
	}

	public void OverrideBy(BlockObject blockObject)
	{
		RecoveredGoodStack component = blockObject.GetComponent<RecoveredGoodStack>();
		if (component != null)
		{
			_recoveredGoodStack.MergeInto(component);
		}
		else
		{
			TryToReposition(blockObject);
		}
	}

	[OnEvent]
	public void OnEntityDeleted(EntityDeletedEvent entityDeletedEvent)
	{
		if ((bool)_blockObjectBelow && entityDeletedEvent.Entity.GetComponent<BlockObject>() == _blockObjectBelow)
		{
			TryToReposition();
		}
	}

	private void OnTerrainHeightChanged(object sender, TerrainHeightChangeEventArgs terrainHeightChangeEventArgs)
	{
		if (!_blockObjectBelow)
		{
			TerrainHeightChange change = terrainHeightChangeEventArgs.Change;
			if (!change.SetTerrain && _blockObject.Coordinates == change.Coordinates.ToVector3Int(change.To + 1))
			{
				TryToReposition();
			}
		}
	}

	private void TryToReposition(BlockObject overridingBlockObject = null)
	{
		if (_recoveredGoodStackCoordinatesFinder.FindValidCoordinates(_blockObject.Coordinates, overridingBlockObject, out var validCoordinates))
		{
			Placement placement = _blockObject.Placement;
			_blockObject.Reposition(new Placement(validCoordinates, placement.Orientation, placement.FlipMode));
			UpdateBlockObjectBelow(validCoordinates);
			_recoveredGoodStackAccessible.UpdateAccesses();
		}
		else
		{
			_recoveredGoodStack.Delete();
		}
	}

	private void UpdateBlockObjectBelow(Vector3Int validCoordinates)
	{
		BlockObject blockObjectBelow = GetBlockObjectBelow(validCoordinates.Below());
		if ((bool)blockObjectBelow)
		{
			if (!_blockObjectBelow)
			{
				_eventBus.Register(this);
			}
			_blockObjectBelow = blockObjectBelow;
		}
		else if ((bool)_blockObjectBelow)
		{
			_eventBus.Unregister(this);
			_blockObjectBelow = null;
		}
	}

	private BlockObject GetBlockObjectBelow(Vector3Int coordinates)
	{
		foreach (BlockObject item in _blockService.GetObjectsAt(coordinates))
		{
			if (item.PositionedBlocks.HasStackableBlockAt(coordinates))
			{
				return item;
			}
		}
		return null;
	}
}
