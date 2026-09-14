using System.Collections.Generic;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Yielding;

internal class RemoveYieldStrategySpecService : ILoadableSingleton
{
	private readonly ISpecService _specService;

	private Dictionary<string, RemoveYieldStrategySpec> _removeYieldStrategySpecs;

	public RemoveYieldStrategySpecService(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		_removeYieldStrategySpecs = _specService.GetSpecs<RemoveYieldStrategySpec>().ToDictionary((RemoveYieldStrategySpec spec) => spec.Id, (RemoveYieldStrategySpec spec) => spec);
	}

	public RemoveYieldStrategySpec GetRemoveYieldStrategySpec(string id)
	{
		return _removeYieldStrategySpecs[id];
	}
}
