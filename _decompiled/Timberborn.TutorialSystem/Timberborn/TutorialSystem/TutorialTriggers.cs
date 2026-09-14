using System.Collections.Generic;
using Timberborn.FactionSystem;
using Timberborn.GameFactionSystem;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;

namespace Timberborn.TutorialSystem;

internal class TutorialTriggers : ITutorialTriggers, ILoadableSingleton, ITickableSingleton
{
	private readonly TutorialService _tutorialService;

	private readonly FactionService _factionService;

	private readonly Queue<string> _pendingTriggers = new Queue<string>();

	private bool _canTrigger;

	public TutorialTriggers(TutorialService tutorialService, FactionService factionService)
	{
		_tutorialService = tutorialService;
		_factionService = factionService;
	}

	public void Load()
	{
		_canTrigger = _factionService.Current.HasSpec<StartingFactionSpec>();
	}

	public bool TriggerPending(string triggerId)
	{
		if (_canTrigger)
		{
			return !_tutorialService.TutorialWasFinished(triggerId);
		}
		return false;
	}

	public void AddTrigger(string triggerId)
	{
		_pendingTriggers.Enqueue(triggerId);
	}

	public void Tick()
	{
		string result;
		while (_pendingTriggers.TryDequeue(out result))
		{
			_tutorialService.AddTutorialTrigger(result);
		}
	}
}
