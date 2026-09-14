using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Effects;
using Timberborn.GameFactionSystem;
using Timberborn.NeedSpecs;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;

namespace Timberborn.SoakedEffects;

internal class SoakedEffectService : ILoadableSingleton
{
	private readonly FactionNeedService _factionNeedService;

	private readonly IDayNightCycle _dayNightCycle;

	public ImmutableArray<InstantEffect> Effects { get; private set; }

	public SoakedEffectService(FactionNeedService factionNeedService, IDayNightCycle dayNightCycle)
	{
		_factionNeedService = factionNeedService;
		_dayNightCycle = dayNightCycle;
	}

	public void Load()
	{
		Effects = CreateEffects(_factionNeedService.Needs).ToImmutableArray();
	}

	private IEnumerable<InstantEffect> CreateEffects(IEnumerable<NeedSpec> needSpecs)
	{
		foreach (NeedSpec needSpec in needSpecs)
		{
			NeedAffectedBySoakednessSpec spec = needSpec.GetSpec<NeedAffectedBySoakednessSpec>();
			if ((object)spec != null)
			{
				float points = spec.PointsPerHour * _dayNightCycle.FixedDeltaTimeInHours;
				yield return new InstantEffect(needSpec.Id, points, 1);
			}
		}
	}
}
