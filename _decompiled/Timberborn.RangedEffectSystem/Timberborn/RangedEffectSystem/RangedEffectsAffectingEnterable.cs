using Timberborn.BaseComponentSystem;
using Timberborn.Common;

namespace Timberborn.RangedEffectSystem;

internal class RangedEffectsAffectingEnterable : BaseComponent
{
	private readonly RangedEffects _rangedEffects = new RangedEffects();

	public ReadOnlyList<RangedEffect> ActiveEffects => _rangedEffects.ActiveEffects;

	public void Add(RangedEffectApplier rangedEffectApplier)
	{
		_rangedEffects.Add(rangedEffectApplier);
	}

	public void Remove(RangedEffectApplier rangedEffectApplier)
	{
		_rangedEffects.Remove(rangedEffectApplier);
	}
}
