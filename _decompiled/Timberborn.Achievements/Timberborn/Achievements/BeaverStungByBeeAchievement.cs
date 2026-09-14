using System.Collections.Generic;
using Timberborn.AchievementSystem;
using Timberborn.BlockSystem;
using Timberborn.GameFactionSystem;
using Timberborn.NeedApplication;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class BeaverStungByBeeAchievement : Achievement
{
	private static readonly string BeeStingNeedId = "BeeSting";

	private readonly EventBus _eventBus;

	private readonly FactionService _factionService;

	private readonly HashSet<AreaNeedApplier> _needAppliers = new HashSet<AreaNeedApplier>();

	public override string Id => "BEAVER_STUNG_BY_BEE";

	public BeaverStungByBeeAchievement(EventBus eventBus, FactionService factionService)
	{
		_eventBus = eventBus;
		_factionService = factionService;
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		AreaNeedApplier component = enteredFinishedStateEvent.BlockObject.GetComponent<AreaNeedApplier>();
		if (component != null && _needAppliers.Add(component))
		{
			component.NeedApplied += OnNeedApplied;
		}
	}

	[OnEvent]
	public void OnExitedFinishedState(ExitedFinishedStateEvent exitedFinishedStateEvent)
	{
		AreaNeedApplier component = exitedFinishedStateEvent.BlockObject.GetComponent<AreaNeedApplier>();
		if (component != null)
		{
			_needAppliers.Remove(component);
			component.NeedApplied -= OnNeedApplied;
		}
	}

	protected override void EnableInternal()
	{
		if (_factionService.Current.Id == AchievementHelper.Folktails)
		{
			_eventBus.Register(this);
		}
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
		foreach (AreaNeedApplier needApplier in _needAppliers)
		{
			needApplier.NeedApplied -= OnNeedApplied;
		}
		_needAppliers.Clear();
	}

	private void OnNeedApplied(object sender, NeedAppliedEventArgs needAppliedEventArgs)
	{
		if (needAppliedEventArgs.NeedEffect.NeedId == BeeStingNeedId)
		{
			Unlock();
		}
	}
}
