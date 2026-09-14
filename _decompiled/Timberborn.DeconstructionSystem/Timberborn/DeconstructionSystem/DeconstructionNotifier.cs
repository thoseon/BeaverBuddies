using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.DeconstructionSystem;

internal class DeconstructionNotifier : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	public DeconstructionNotifier(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnEntityDeleted(EntityDeletedEvent entityDeletedEvent)
	{
		Deconstructible enabledComponent = entityDeletedEvent.Entity.GetEnabledComponent<Deconstructible>();
		if (enabledComponent != null)
		{
			NotifyOnBuildingDeconstructed(enabledComponent);
		}
	}

	private void NotifyOnBuildingDeconstructed(Deconstructible deconstructible)
	{
		_eventBus.Post(new BuildingDeconstructedEvent(deconstructible, GetCoordinates(deconstructible)));
	}

	private static ReadOnlyList<Vector3Int> GetCoordinates(Deconstructible deconstructible)
	{
		PositionedBlocks positionedBlocks = deconstructible.GetComponent<BlockObject>().PositionedBlocks;
		List<Vector3Int> list = positionedBlocks.GetFoundationCoordinates().ToList();
		if (list.Count <= 0)
		{
			return positionedBlocks.GetAllCoordinates().ToList().AsReadOnlyList();
		}
		return list.AsReadOnlyList();
	}
}
