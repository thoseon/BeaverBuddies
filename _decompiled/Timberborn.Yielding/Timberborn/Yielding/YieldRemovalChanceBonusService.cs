using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.BonusSystem;
using Timberborn.Common;
using Timberborn.SingletonSystem;

namespace Timberborn.Yielding;

public class YieldRemovalChanceBonusService : ILoadableSingleton
{
	private readonly ISpecService _specService;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private FrozenDictionary<string, ImmutableList<string>> _yieldRemovalChanceBonusTypes;

	public YieldRemovalChanceBonusService(ISpecService specService, IRandomNumberGenerator randomNumberGenerator)
	{
		_specService = specService;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Load()
	{
		_yieldRemovalChanceBonusTypes = (from spec in _specService.GetSpecs<YieldRemovalChanceBonusSpec>()
			group spec by spec.GoodId).ToFrozenDictionary((IGrouping<string, YieldRemovalChanceBonusSpec> spec) => spec.Key, (IGrouping<string, YieldRemovalChanceBonusSpec> spec) => spec.Select((YieldRemovalChanceBonusSpec s) => s.BonusId).ToImmutableList());
	}

	public bool CheckYieldRemovalSuccess(BonusManager bonusManager, string goodId)
	{
		if (_yieldRemovalChanceBonusTypes.TryGetValue(goodId, out var value))
		{
			if (value.Count != 0)
			{
				return CheckBonusProbability(bonusManager, value);
			}
			return true;
		}
		return true;
	}

	private bool CheckBonusProbability(BonusManager bonusManager, ImmutableList<string> bonusTypes)
	{
		float num = 1f;
		foreach (string bonusType in bonusTypes)
		{
			num *= bonusManager.Multiplier(bonusType);
		}
		return _randomNumberGenerator.CheckProbability(num);
	}
}
