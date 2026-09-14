using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.FactionSystem;

public class FactionSpecService : ILoadableSingleton
{
	private readonly ISpecService _specService;

	public ImmutableArray<FactionSpec> Factions { get; private set; }

	public IEnumerable<UnlockableFactionSpec> UnlockableFactions => from faction in Factions
		select faction.GetSpec<UnlockableFactionSpec>() into unlockableFactionSpec
		where unlockableFactionSpec != null
		select unlockableFactionSpec;

	public FactionSpecService(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		Factions = _specService.GetSpecs<FactionSpec>().ToImmutableArray();
	}

	public FactionSpec GetFaction(string id)
	{
		return Factions.SingleOrDefault((FactionSpec faction) => faction.Id == id) ?? throw new ArgumentException("Faction with id " + id + " not found.");
	}
}
