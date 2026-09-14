using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.MapRepositorySystem;

namespace Timberborn.MapRepositorySystemUI;

public class MapValidator
{
	private readonly ImmutableArray<IMapLoadValidator> _mapLoadValidators;

	public MapValidator(IEnumerable<IMapLoadValidator> mapLoadValidators)
	{
		_mapLoadValidators = mapLoadValidators.OrderBy((IMapLoadValidator validator) => validator.Priority).ToImmutableArray();
	}

	public void ValidateForNewGame(MapFileReference mapFileReference, Action continueCallback)
	{
		CheckNextValidator(mapFileReference, continueCallback, 0, isForNewGame: true);
	}

	public void ValidateForMapEditor(MapFileReference mapFileReference, Action continueCallback)
	{
		CheckNextValidator(mapFileReference, continueCallback, 0, isForNewGame: false);
	}

	private void CheckNextValidator(MapFileReference mapFileReference, Action continueCallback, int index, bool isForNewGame)
	{
		if (index >= _mapLoadValidators.Length)
		{
			continueCallback();
			return;
		}
		IMapLoadValidator mapLoadValidator = _mapLoadValidators[index];
		if (isForNewGame)
		{
			mapLoadValidator.ValidateForNewGame(mapFileReference, delegate
			{
				CheckNextValidator(mapFileReference, continueCallback, index + 1, isForNewGame: true);
			});
		}
		else
		{
			mapLoadValidator.ValidateForMapEditor(mapFileReference, delegate
			{
				CheckNextValidator(mapFileReference, continueCallback, index + 1, isForNewGame: false);
			});
		}
	}
}
