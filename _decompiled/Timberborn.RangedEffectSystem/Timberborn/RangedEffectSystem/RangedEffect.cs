using System.Collections.Generic;
using System.Linq;
using Timberborn.Effects;

namespace Timberborn.RangedEffectSystem;

internal class RangedEffect
{
	private readonly HashSet<RangedEffectApplier> _appliers;

	public ContinuousEffect BaseEffect { get; }

	public bool IsActive => _appliers.Any((RangedEffectApplier applier) => applier.Active);

	public IEnumerable<RangedEffectApplier> Appliers => _appliers;

	public RangedEffect(ContinuousEffect baseEffect)
	{
		BaseEffect = baseEffect;
		_appliers = new HashSet<RangedEffectApplier>();
	}

	public void Add(RangedEffectApplier rangedEffectApplier)
	{
		_appliers.Add(rangedEffectApplier);
	}

	public void Remove(RangedEffectApplier rangedEffectApplier)
	{
		_appliers.Remove(rangedEffectApplier);
	}

	public ContinuousEffect ToContinuousEffect()
	{
		return new ContinuousEffect(BaseEffect.NeedId, BaseEffect.PointsPerHour * GetEfficiency());
	}

	private float GetEfficiency()
	{
		float num = 0f;
		foreach (RangedEffectApplier applier in _appliers)
		{
			if (applier.Active)
			{
				if (applier.Efficiency >= 1f)
				{
					return 1f;
				}
				if (applier.Efficiency > num)
				{
					num = applier.Efficiency;
				}
			}
		}
		return num;
	}
}
