using Timberborn.AchievementSystem;
using Timberborn.Beavers;
using Timberborn.GameFactionSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class BornBeaverAfterBeaverExtinctionAchievement : Achievement
{
	private readonly EventBus _eventBus;

	private readonly BeaverPopulation _beaverPopulation;

	private readonly FactionService _factionService;

	public override string Id => "BORN_BEAVER_AFTER_BEAVER_EXTINCTION";

	public BornBeaverAfterBeaverExtinctionAchievement(EventBus eventBus, BeaverPopulation beaverPopulation, FactionService factionService)
	{
		_eventBus = eventBus;
		_beaverPopulation = beaverPopulation;
		_factionService = factionService;
	}

	[OnEvent]
	public void OnBeaverBorn(BeaverBornEvent beaverBornEvent)
	{
		if (_beaverPopulation.NumberOfBeavers == 0)
		{
			Unlock();
		}
	}

	protected override void EnableInternal()
	{
		if (_factionService.Current.Id == AchievementHelper.IronTeeth)
		{
			_eventBus.Register(this);
		}
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
	}
}
