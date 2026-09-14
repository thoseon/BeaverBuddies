using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.EntitySystem;
using Timberborn.NeedSystem;
using Timberborn.TimeSystem;

namespace Timberborn.Achievements;

internal class InjuredJustBornBeaverTracker : BaseComponent, IAwakableComponent, IPostInitializableEntity
{
	private static readonly string InjuryNeedId = "Injury";

	private readonly InjuredJustBornBeaverAchievement _injuredJustBornBeaverAchievement;

	private readonly IDayNightCycle _dayNightCycle;

	private NeedManager _needManager;

	private Character _character;

	public InjuredJustBornBeaverTracker(InjuredJustBornBeaverAchievement injuredJustBornBeaverAchievement, IDayNightCycle dayNightCycle)
	{
		_injuredJustBornBeaverAchievement = injuredJustBornBeaverAchievement;
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		_needManager = GetComponent<NeedManager>();
		_character = GetComponent<Character>();
	}

	public void PostInitializeEntity()
	{
		if (_injuredJustBornBeaverAchievement.CanTrackInjury && IsBornToday())
		{
			_needManager.NeedChangedActiveState += OnNeedChangedActiveState;
		}
	}

	private void OnNeedChangedActiveState(object sender, NeedChangedActiveStateEventArgs e)
	{
		if (_injuredJustBornBeaverAchievement.IsEnabled && IsBornToday())
		{
			if (e.IsActive && e.NeedSpec.Id == InjuryNeedId)
			{
				_injuredJustBornBeaverAchievement.Unlock();
				DisableTracking();
			}
		}
		else
		{
			DisableTracking();
		}
	}

	private bool IsBornToday()
	{
		return _character.DayOfBirth == _dayNightCycle.DayNumber;
	}

	private void DisableTracking()
	{
		_needManager.NeedChangedActiveState -= OnNeedChangedActiveState;
	}
}
