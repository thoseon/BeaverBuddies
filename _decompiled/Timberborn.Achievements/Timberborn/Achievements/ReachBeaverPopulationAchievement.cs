using Timberborn.AchievementSystem;
using Timberborn.Beavers;
using Timberborn.Characters;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal abstract class ReachBeaverPopulationAchievement : Achievement
{
	private readonly BeaverPopulation _beaverPopulation;

	private readonly EventBus _eventBus;

	private readonly int _threshold;

	public override string Id => $"REACH_POPULATION_OF_{_threshold}_BEAVERS";

	protected ReachBeaverPopulationAchievement(BeaverPopulation beaverPopulation, EventBus eventBus, int threshold)
	{
		_beaverPopulation = beaverPopulation;
		_eventBus = eventBus;
		_threshold = threshold;
	}

	[OnEvent]
	public void OnCharacterCreated(CharacterCreatedEvent characterCreatedEvent)
	{
		ValidatePopulation();
	}

	protected override void EnableInternal()
	{
		_eventBus.Register(this);
		ValidatePopulation();
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
	}

	private void ValidatePopulation()
	{
		if (_beaverPopulation.NumberOfBeavers >= _threshold)
		{
			Unlock();
		}
	}
}
