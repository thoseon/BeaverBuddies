using System.Collections.Immutable;
using System.Linq;
using Timberborn.AchievementSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.TubeSystem;

namespace Timberborn.Achievements;

internal class LargeTubewayNetworkAchievement : Achievement
{
	private static readonly int StationsRequired = 10;

	private static readonly int TubewaysRequired = 1000;

	private readonly EventBus _eventBus;

	private readonly EntityComponentRegistry _entityComponentRegistry;

	private int _stationCount;

	private int _tubewayCount;

	public override string Id => "LARGE_TUBEWAY_NETWORK";

	public LargeTubewayNetworkAchievement(EventBus eventBus, EntityComponentRegistry entityComponentRegistry)
	{
		_eventBus = eventBus;
		_entityComponentRegistry = entityComponentRegistry;
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		if (enteredFinishedStateEvent.BlockObject.HasComponent<TubeStationSpec>())
		{
			_stationCount++;
		}
		if (enteredFinishedStateEvent.BlockObject.HasComponent<TubeSpec>())
		{
			_tubewayCount++;
		}
		CheckUnlockCondition();
	}

	[OnEvent]
	public void OnExitedFinishedState(ExitedFinishedStateEvent exitedFinishedStateEvent)
	{
		if (exitedFinishedStateEvent.BlockObject.HasComponent<TubeStationSpec>())
		{
			_stationCount--;
		}
		if (exitedFinishedStateEvent.BlockObject.HasComponent<TubeSpec>())
		{
			_tubewayCount--;
		}
	}

	protected override void EnableInternal()
	{
		_eventBus.Register(this);
		ValidateInitialCount();
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
	}

	private void CheckUnlockCondition()
	{
		if (_stationCount >= StationsRequired && _tubewayCount >= TubewaysRequired)
		{
			Unlock();
		}
	}

	private void ValidateInitialCount()
	{
		ImmutableArray<Building> immutableArray = _entityComponentRegistry.GetEnabled<Building>().ToImmutableArray();
		_stationCount = immutableArray.Where((Building spec) => spec.HasComponent<TubeStationSpec>()).Count((Building spec) => spec.GetComponent<BlockObject>().IsFinished);
		_tubewayCount = immutableArray.Where((Building spec) => spec.HasComponent<TubeSpec>()).Count((Building spec) => spec.GetComponent<BlockObject>().IsFinished);
		CheckUnlockCondition();
	}
}
