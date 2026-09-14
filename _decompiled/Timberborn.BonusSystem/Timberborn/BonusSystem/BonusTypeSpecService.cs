using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.BonusSystem;

public class BonusTypeSpecService : ILoadableSingleton
{
	private readonly ISpecService _specService;

	private ImmutableArray<BonusTypeSpec> _specs;

	public IEnumerable<string> BonusIds => _specs.Select((BonusTypeSpec spec) => spec.Id);

	public BonusTypeSpecService(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		_specs = _specService.GetSpecs<BonusTypeSpec>().ToImmutableArray();
	}

	public BonusTypeSpec GetSpec(string bonusId)
	{
		BonusTypeSpec bonusTypeSpec = _specs.SingleOrDefault((BonusTypeSpec spec) => spec.Id == bonusId);
		if (bonusTypeSpec != null)
		{
			return bonusTypeSpec;
		}
		throw new InvalidOperationException("Bonus type spec with id " + bonusId + " not found or multiple specs found");
	}
}
