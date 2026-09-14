using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.StatusSystem;
using UnityEngine;

namespace Timberborn.Terraforming;

internal class TerraformingDirectionalBlocker : BaseComponent, IAwakableComponent, IInitializableEntity, IUnfinishedStateListener
{
	private class BlockerData
	{
		public Vector3Int BlockedCoordinates { get; private set; }

		public Vector3Int BlockerCoordinates { get; private set; }

		public TerraformingDirectionalBlocker Blocking { get; set; }

		public TerraformingDirectionalBlocker BlockedBy { get; set; }

		public BlockObject StackableBlocker { get; set; }

		public Vector3Int Axis { get; }

		public StatusToggle StatusToggle { get; }

		public BlockerData(Vector3Int axis, string status)
		{
			Axis = axis;
			StatusToggle = StatusToggle.CreateNormalStatus("DirectionalBlocking", status);
		}

		public void SetCoordinates(BlockObject blockObject)
		{
			BlockedCoordinates = blockObject.TransformCoordinates(-Axis);
			BlockerCoordinates = blockObject.TransformCoordinates(Axis);
		}

		public void UpdateBlockingState(BlockableObject blockableObject)
		{
			if ((BlockedBy != null || StackableBlocker != null) && !StatusToggle.IsActive)
			{
				StatusToggle.Activate();
				blockableObject.Block(this);
			}
			else if (BlockedBy == null && StackableBlocker == null && StatusToggle.IsActive)
			{
				StatusToggle.Deactivate();
				blockableObject.Unblock(this);
			}
		}
	}

	private static readonly string DirectionalBlockingLocKey = "Status.Buildings.DirectionalBlocking";

	private static readonly string VerticalBlockingLocKey = "Status.Buildings.VerticalBlocking";

	private readonly IBlockService _blockService;

	private readonly ILoc _loc;

	private readonly EventBus _eventBus;

	private readonly Dictionary<Vector3Int, BlockerData> _perAxisBlockerData = new Dictionary<Vector3Int, BlockerData>();

	private BlockerData _blockerDataWithStackable;

	private BlockableObject _blockableObject;

	private BlockObject _blockObject;

	public TerraformingDirectionalBlocker(IBlockService blockService, ILoc loc, EventBus eventBus)
	{
		_blockService = blockService;
		_loc = loc;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
		_blockObject = GetComponent<BlockObject>();
		_perAxisBlockerData[Vector3Int.up] = new BlockerData(Vector3Int.up, _loc.T(DirectionalBlockingLocKey));
		_blockerDataWithStackable = new BlockerData(Vector3Int.back, _loc.T(VerticalBlockingLocKey));
		_perAxisBlockerData[Vector3Int.back] = _blockerDataWithStackable;
	}

	public void InitializeEntity()
	{
		foreach (BlockerData value in _perAxisBlockerData.Values)
		{
			GetComponent<StatusSubject>().RegisterStatus(value.StatusToggle);
		}
	}

	[OnEvent]
	public void OnEnteredUnfinishedState(EnteredUnfinishedStateEvent enteredUnfinishedStateEvent)
	{
		BlockObject blockObject = enteredUnfinishedStateEvent.BlockObject;
		if (IsValidBlockerBlockObject(blockObject, _blockObject.Coordinates.Below()))
		{
			_blockerDataWithStackable.StackableBlocker = blockObject;
			_blockerDataWithStackable.UpdateBlockingState(_blockableObject);
		}
	}

	[OnEvent]
	public void OnExitedUnfinishedState(ExitedUnfinishedStateEvent exitedUnfinishedStateEvent)
	{
		ClearStackableIfWasBlocking(exitedUnfinishedStateEvent.BlockObject);
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		ClearStackableIfWasBlocking(enteredFinishedStateEvent.BlockObject);
	}

	public void OnEnterUnfinishedState()
	{
		_eventBus.Register(this);
		foreach (BlockerData value in _perAxisBlockerData.Values)
		{
			value.SetCoordinates(_blockObject);
			CheckBlockBlockingThisObject(value);
			CheckBlockBlockedByThisObject(value);
			CheckForStackableBlocker(value);
		}
	}

	public void OnExitUnfinishedState()
	{
		_eventBus.Unregister(this);
		foreach (BlockerData value in _perAxisBlockerData.Values)
		{
			if ((bool)value.BlockedBy)
			{
				Unblock(value.BlockedBy, value.Axis);
			}
			if ((bool)value.Blocking)
			{
				value.Blocking.Unblock(this, value.Axis);
			}
		}
	}

	private void ClearStackableIfWasBlocking(BlockObject blockObject)
	{
		if (blockObject == _blockerDataWithStackable.StackableBlocker)
		{
			_blockerDataWithStackable.StackableBlocker = null;
			_blockerDataWithStackable.UpdateBlockingState(_blockableObject);
		}
	}

	private void CheckBlockBlockingThisObject(BlockerData blockerData)
	{
		TerraformingDirectionalBlocker terraformingDirectionalBlocker = _blockService.GetObjectsWithComponentAt<TerraformingDirectionalBlocker>(blockerData.BlockerCoordinates).FirstOrDefault();
		Vector3Int axis = blockerData.Axis;
		if ((bool)terraformingDirectionalBlocker && terraformingDirectionalBlocker.IsBlockingCoordinates(_blockObject.Coordinates, axis))
		{
			Block(terraformingDirectionalBlocker, axis);
		}
	}

	private void CheckBlockBlockedByThisObject(BlockerData blockerData)
	{
		TerraformingDirectionalBlocker terraformingDirectionalBlocker = _blockService.GetObjectsWithComponentAt<TerraformingDirectionalBlocker>(blockerData.BlockedCoordinates).FirstOrDefault();
		Vector3Int axis = blockerData.Axis;
		if ((bool)terraformingDirectionalBlocker && terraformingDirectionalBlocker.IsBlockerAtCoordinates(_blockObject.Coordinates, axis))
		{
			terraformingDirectionalBlocker.Block(this, axis);
		}
	}

	private void CheckForStackableBlocker(BlockerData blockerData)
	{
		if (_blockerDataWithStackable == blockerData && TryGetStackableBlocker(out var stackableBlockObject))
		{
			blockerData.StackableBlocker = stackableBlockObject;
			blockerData.UpdateBlockingState(_blockableObject);
		}
	}

	private bool IsBlockingCoordinates(Vector3Int coordinates, Vector3Int axis)
	{
		return _perAxisBlockerData[axis].BlockedCoordinates == coordinates;
	}

	private bool IsBlockerAtCoordinates(Vector3Int coordinates, Vector3Int axis)
	{
		return _perAxisBlockerData[axis].BlockerCoordinates == coordinates;
	}

	private void Block(TerraformingDirectionalBlocker other, Vector3Int axis)
	{
		BlockerData blockerData = _perAxisBlockerData[axis];
		blockerData.BlockedBy = other;
		other._perAxisBlockerData[axis].Blocking = this;
		blockerData.UpdateBlockingState(_blockableObject);
	}

	private void Unblock(TerraformingDirectionalBlocker other, Vector3Int axis)
	{
		BlockerData blockerData = _perAxisBlockerData[axis];
		blockerData.BlockedBy = null;
		other._perAxisBlockerData[axis].Blocking = null;
		blockerData.UpdateBlockingState(_blockableObject);
	}

	private bool TryGetStackableBlocker(out BlockObject stackableBlockObject)
	{
		Vector3Int coordinates = _blockObject.Coordinates.Below();
		foreach (BlockObject item in _blockService.GetObjectsAt(coordinates))
		{
			if (IsValidBlockerBlockObject(item, coordinates))
			{
				stackableBlockObject = item;
				return true;
			}
		}
		stackableBlockObject = null;
		return false;
	}

	private bool IsValidBlockerBlockObject(BlockObject blockObject, Vector3Int coordinates)
	{
		if (blockObject != _blockObject && !blockObject.IsFinished && blockObject.PositionedBlocks.HasBlockAt(coordinates))
		{
			return blockObject.PositionedBlocks.GetBlock(coordinates).Stackable == BlockStackable.BlockObject;
		}
		return false;
	}
}
