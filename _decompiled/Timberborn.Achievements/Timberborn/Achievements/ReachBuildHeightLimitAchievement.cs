using System.Linq;
using Timberborn.AchievementSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.EntitySystem;
using Timberborn.MapStateSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Achievements;

internal class ReachBuildHeightLimitAchievement : Achievement
{
	private readonly EventBus _eventBus;

	private readonly MapSize _mapSize;

	private readonly EntityComponentRegistry _entityComponentRegistry;

	public override string Id => "REACH_BUILD_HEIGHT_LIMIT";

	public ReachBuildHeightLimitAchievement(EventBus eventBus, MapSize mapSize, EntityComponentRegistry entityComponentRegistry)
	{
		_eventBus = eventBus;
		_mapSize = mapSize;
		_entityComponentRegistry = entityComponentRegistry;
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		if (IsReachingHeightLimit(enteredFinishedStateEvent.BlockObject))
		{
			Unlock();
		}
	}

	protected override void EnableInternal()
	{
		if (AnyBuildingIsReachingHeightLimit())
		{
			Unlock();
		}
		else
		{
			_eventBus.Register(this);
		}
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
	}

	private bool AnyBuildingIsReachingHeightLimit()
	{
		foreach (BlockObject item in from spec in _entityComponentRegistry.GetEnabled<Building>()
			where spec.GetComponent<BlockObject>().IsFinished
			select spec.GetComponent<BlockObject>())
		{
			if (IsReachingHeightLimit(item))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsReachingHeightLimit(BlockObject blockObject)
	{
		foreach (Vector3Int occupiedCoordinate in blockObject.PositionedBlocks.GetOccupiedCoordinates())
		{
			if (occupiedCoordinate.z == _mapSize.TotalSize.z - 1)
			{
				return true;
			}
		}
		return false;
	}
}
