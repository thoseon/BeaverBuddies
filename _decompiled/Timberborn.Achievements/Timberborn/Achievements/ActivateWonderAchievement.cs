using Timberborn.AchievementSystem;
using Timberborn.GameFactionSystem;
using Timberborn.SingletonSystem;
using Timberborn.Wonders;

namespace Timberborn.Achievements;

internal abstract class ActivateWonderAchievement : Achievement
{
	private readonly EventBus _eventBus;

	private readonly FactionService _factionService;

	private readonly string _faction;

	public override string Id => "ACTIVATE_WONDER_" + _faction.ToUpperInvariant();

	protected ActivateWonderAchievement(EventBus eventBus, FactionService factionService, string faction)
	{
		_eventBus = eventBus;
		_factionService = factionService;
		_faction = faction;
	}

	[OnEvent]
	public void OnWonderActivated(WonderActivatedEvent wonderActivatedEvent)
	{
		Unlock();
	}

	protected override void EnableInternal()
	{
		if (_factionService.Current.Id == _faction)
		{
			_eventBus.Register(this);
		}
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
	}
}
