using Timberborn.SingletonSystem;

namespace Timberborn.GameWonderCompletion;

internal class GameWonderCompletionRestorer : IPostLoadableSingleton
{
	private readonly GameWonderCompletionService _gameWonderCompletionService;

	private readonly WonderCompletionCountdownStarter _wonderCompletionCountdownStarter;

	public GameWonderCompletionRestorer(GameWonderCompletionService gameWonderCompletionService, WonderCompletionCountdownStarter wonderCompletionCountdownStarter)
	{
		_gameWonderCompletionService = gameWonderCompletionService;
		_wonderCompletionCountdownStarter = wonderCompletionCountdownStarter;
	}

	public void PostLoad()
	{
		CompleteWonderIfDataIsLost();
	}

	private void CompleteWonderIfDataIsLost()
	{
		if (_wonderCompletionCountdownStarter.CountdownFinished && !_gameWonderCompletionService.IsWonderCompletedWithAnyFaction())
		{
			_gameWonderCompletionService.CompleteWonder();
		}
	}
}
