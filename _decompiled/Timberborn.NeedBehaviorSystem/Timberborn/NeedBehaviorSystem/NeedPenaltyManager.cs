using Timberborn.BaseComponentSystem;
using Timberborn.BonusSystem;
using Timberborn.EntitySystem;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;

namespace Timberborn.NeedBehaviorSystem;

public class NeedPenaltyManager : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private BonusManager _bonusManager;

	private NeedManager _needManager;

	public void Awake()
	{
		_bonusManager = GetComponent<BonusManager>();
		_needManager = GetComponent<NeedManager>();
	}

	public void InitializeEntity()
	{
		_needManager.NeedChangedIsFavorable += delegate(object _, NeedChangedIsFavorableEventArgs e)
		{
			UpdatePenalties(e.NeedSpec);
		};
		foreach (NeedSpec needSpec in _needManager.NeedSpecs)
		{
			PunitiveNeedSpec spec = needSpec.GetSpec<PunitiveNeedSpec>();
			if ((object)spec != null && !_needManager.NeedIsFavorable(needSpec.Id))
			{
				AddPenalties(spec);
			}
		}
	}

	private void UpdatePenalties(NeedSpec needSpec)
	{
		PunitiveNeedSpec spec = needSpec.GetSpec<PunitiveNeedSpec>();
		if ((object)spec != null)
		{
			if (!_needManager.NeedIsFavorable(needSpec.Id))
			{
				AddPenalties(spec);
			}
			else
			{
				RemovePenalties(spec);
			}
		}
	}

	private void AddPenalties(PunitiveNeedSpec punitiveNeedSpec)
	{
		foreach (BonusSpec penalty in punitiveNeedSpec.Penalties)
		{
			_bonusManager.AddBonus(penalty.Id, penalty.MultiplierDelta);
		}
	}

	private void RemovePenalties(PunitiveNeedSpec punitiveNeedSpec)
	{
		foreach (BonusSpec penalty in punitiveNeedSpec.Penalties)
		{
			_bonusManager.RemoveBonus(penalty.Id, penalty.MultiplierDelta);
		}
	}
}
