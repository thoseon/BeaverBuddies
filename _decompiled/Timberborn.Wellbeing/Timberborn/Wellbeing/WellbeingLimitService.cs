using System.Collections.Generic;
using System.Linq;
using Timberborn.Bots;
using Timberborn.GameFactionSystem;
using Timberborn.NeedSpecs;
using Timberborn.SingletonSystem;

namespace Timberborn.Wellbeing;

public class WellbeingLimitService : ILoadableSingleton
{
	private readonly FactionNeedService _factionNeedService;

	private int _maxBotWellbeing;

	public int MaxBeaverWellbeing { get; private set; }

	public WellbeingLimitService(FactionNeedService factionNeedService)
	{
		_factionNeedService = factionNeedService;
	}

	public void Load()
	{
		MaxBeaverWellbeing = GetMaxWellbeing(_factionNeedService.GetBeaverNeeds());
		_maxBotWellbeing = GetMaxWellbeing(_factionNeedService.GetBotNeeds());
	}

	public int GetMaxWellbeing(WellbeingTracker wellbeingTracker)
	{
		if (!wellbeingTracker.HasComponent<BotSpec>())
		{
			return MaxBeaverWellbeing;
		}
		return _maxBotWellbeing;
	}

	private static int GetMaxWellbeing(IEnumerable<NeedSpec> needSpecs)
	{
		return needSpecs.Sum((NeedSpec spec) => spec.GetFavorableWellbeing());
	}
}
