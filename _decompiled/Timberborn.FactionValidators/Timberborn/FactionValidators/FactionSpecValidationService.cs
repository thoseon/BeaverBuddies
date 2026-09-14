using System;
using System.Collections.Generic;
using Timberborn.FactionSystem;
using Timberborn.Modding;
using Timberborn.SingletonSystem;

namespace Timberborn.FactionValidators;

internal class FactionSpecValidationService : ILoadableSingleton
{
	private readonly FactionSpecService _factionSpecService;

	private readonly IEnumerable<IFactionSpecValidator> _factionSpecValidators;

	public FactionSpecValidationService(FactionSpecService factionSpecService, IEnumerable<IFactionSpecValidator> factionSpecValidators)
	{
		_factionSpecService = factionSpecService;
		_factionSpecValidators = factionSpecValidators;
	}

	public void Load()
	{
		if (!ModdedState.IsModded)
		{
			foreach (FactionSpec faction in _factionSpecService.Factions)
			{
				ValidateFaction(faction);
			}
		}
	}

	private void ValidateFaction(FactionSpec factionSpec)
	{
		foreach (IFactionSpecValidator factionSpecValidator in _factionSpecValidators)
		{
			if (!factionSpecValidator.IsValid(factionSpec, out var errorMessage))
			{
				throw new Exception("Faction " + factionSpec.Id + " load error: " + errorMessage);
			}
		}
	}
}
