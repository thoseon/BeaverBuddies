using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.GameSceneLoading;

namespace Timberborn.GameSaveRepositorySystemUI;

public class ValidatingGameLoader
{
	private readonly GameSceneLoader _gameSceneLoader;

	private readonly ImmutableArray<IGameLoadValidator> _gameLoadValidators;

	public ValidatingGameLoader(GameSceneLoader gameSceneLoader, IEnumerable<IGameLoadValidator> gameLoadValidators)
	{
		_gameSceneLoader = gameSceneLoader;
		_gameLoadValidators = gameLoadValidators.OrderBy((IGameLoadValidator validator) => validator.Priority).ToImmutableArray();
	}

	public void LoadGame(SaveReference saveReference)
	{
		CheckNextValidator(saveReference, 0);
	}

	private void CheckNextValidator(SaveReference saveReference, int index)
	{
		if (index >= _gameLoadValidators.Length)
		{
			_gameSceneLoader.StartSaveGame(saveReference);
			return;
		}
		_gameLoadValidators[index].ValidateSave(saveReference, delegate
		{
			CheckNextValidator(saveReference, index + 1);
		});
	}
}
