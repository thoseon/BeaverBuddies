using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.StatusSystem;

internal class StatusSlotsUpdater : ILoadableSingleton, IUpdatableSingleton
{
	private readonly IStatusIconOffsetService _statusIconOffsetService;

	private readonly EventBus _eventBus;

	private readonly HashSet<Vector2Int> _dirtyCoordinates = new HashSet<Vector2Int>();

	public StatusSlotsUpdater(IStatusIconOffsetService statusIconOffsetService, EventBus eventBus)
	{
		_statusIconOffsetService = statusIconOffsetService;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public void UpdateSingleton()
	{
		foreach (Vector2Int dirtyCoordinate in _dirtyCoordinates)
		{
			_statusIconOffsetService.UpdateAffectedStatusSlot(dirtyCoordinate);
		}
		_dirtyCoordinates.Clear();
	}

	[OnEvent]
	public void OnEntityDeletedEvent(EntityDeletedEvent entityDeletedEvent)
	{
		BlockObject component = entityDeletedEvent.Entity.GetComponent<BlockObject>();
		if ((bool)component)
		{
			MarkCoordinatesDirty(component);
		}
	}

	[OnEvent]
	public void OnEnteredUnfinishedStateEvent(EnteredUnfinishedStateEvent enteredUnfinishedStateEvent)
	{
		MarkCoordinatesDirty(enteredUnfinishedStateEvent.BlockObject);
	}

	[OnEvent]
	public void OnEnteredFinishedStateEvent(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		MarkCoordinatesDirty(enteredFinishedStateEvent.BlockObject);
	}

	private void MarkCoordinatesDirty(BlockObject blockObject)
	{
		foreach (Vector3Int occupiedCoordinate in blockObject.PositionedBlocks.GetOccupiedCoordinates())
		{
			_dirtyCoordinates.Add(occupiedCoordinate.XY());
		}
	}
}
