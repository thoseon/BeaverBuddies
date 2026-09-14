using Timberborn.BaseComponentSystem;
using Timberborn.BonusSystem;
using Timberborn.EntitySystem;

namespace Timberborn.Wellbeing;

internal class WellbeingTierManager : BaseComponent, IAwakableComponent, IPostLoadableEntity
{
	private readonly IWellbeingTierService _wellbeingTierService;

	private BonusManager _bonusManager;

	private WellbeingTracker _wellbeingTracker;

	public WellbeingTierManager(IWellbeingTierService wellbeingTierService)
	{
		_wellbeingTierService = wellbeingTierService;
	}

	public void Awake()
	{
		_bonusManager = GetComponent<BonusManager>();
		_wellbeingTracker = GetComponent<WellbeingTracker>();
	}

	public void PostLoadEntity()
	{
		_wellbeingTracker.WellbeingChanged += delegate(object _, WellbeingChangedEventArgs e)
		{
			UpdateBonuses(e.OldWellbeing, e.NewWellbeing);
		};
		foreach (string tierableBonuse in _wellbeingTierService.GetTierableBonuses(_wellbeingTracker))
		{
			AddBonus(_wellbeingTracker.Wellbeing, tierableBonuse);
		}
	}

	private void UpdateBonuses(int oldWellbeing, int newWellbeing)
	{
		foreach (string tierableBonuse in _wellbeingTierService.GetTierableBonuses(_wellbeingTracker))
		{
			RemoveBonus(oldWellbeing, tierableBonuse);
			AddBonus(newWellbeing, tierableBonuse);
		}
	}

	private void AddBonus(int wellbeing, string bonusId)
	{
		if (_wellbeingTierService.TryGetTierBonus(_wellbeingTracker, bonusId, wellbeing, out var tierBonus))
		{
			_bonusManager.AddBonus(bonusId, tierBonus.Bonus);
		}
	}

	private void RemoveBonus(int wellbeing, string bonusId)
	{
		if (_wellbeingTierService.TryGetTierBonus(_wellbeingTracker, bonusId, wellbeing, out var tierBonus))
		{
			_bonusManager.RemoveBonus(bonusId, tierBonus.Bonus);
		}
	}
}
