using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.UndoSystem;

namespace Timberborn.StartingLocationSystem;

public class StartingLocationService : ILoadableSingleton
{
	private readonly EntityComponentRegistry _entityComponentRegistry;

	private readonly EntityService _entityService;

	private readonly EventBus _eventBus;

	private readonly IUndoRegistry _undoRegistry;

	public StartingLocationService(EntityComponentRegistry entityComponentRegistry, EntityService entityService, EventBus eventBus, IUndoRegistry undoRegistry)
	{
		_entityComponentRegistry = entityComponentRegistry;
		_entityService = entityService;
		_eventBus = eventBus;
		_undoRegistry = undoRegistry;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnBlockObjectSet(BlockObjectSetEvent blockObjectSetEvent)
	{
		if (blockObjectSetEvent.BlockObject.TryGetComponent<StartingLocation>(out var component))
		{
			DeleteOtherStartingLocations(component);
		}
	}

	public bool HasStartingLocation()
	{
		return _entityComponentRegistry.GetEnabled<StartingLocation>().Any();
	}

	public Placement GetPlacement()
	{
		return GetStartingLocation().GetComponent<BlockObject>().Placement;
	}

	public void DeleteStartingLocations()
	{
		foreach (StartingLocation item in _entityComponentRegistry.GetEnabled<StartingLocation>().ToList())
		{
			_entityService.Delete(item);
		}
	}

	public StartingLocation GetStartingLocation()
	{
		List<StartingLocation> list = _entityComponentRegistry.GetEnabled<StartingLocation>().ToList();
		if (list.IsEmpty())
		{
			throw new InvalidOperationException("No StartingLocationSpec exists.");
		}
		if (list.Count > 1)
		{
			throw new InvalidOperationException("There must be only one StartingLocationSpec.");
		}
		return list[0];
	}

	private void DeleteOtherStartingLocations(StartingLocation remainingStartingLocation)
	{
		if (_undoRegistry.IsProcessingStack)
		{
			return;
		}
		foreach (StartingLocation item in (from startingLocation in _entityComponentRegistry.GetEnabled<StartingLocation>()
			where startingLocation != remainingStartingLocation
			select startingLocation).ToList())
		{
			_entityService.Delete(item);
		}
	}
}
