using Timberborn.Benchmarking;
using Timberborn.GameWonderCompletion;
using Timberborn.SingletonSystem;

namespace Timberborn.GameOver;

internal class GameOverDisabler : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private int _blockers;

	public bool Disabled => _blockers > 0;

	public GameOverDisabler(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnBenchmarkStarted(BenchmarkStartedEvent benchmarkStartedEvent)
	{
		_blockers++;
	}

	[OnEvent]
	public void OnWonderCompletionCountdownStarted(WonderCompletionCountdownStartedEvent wonderCompletionCountdownStartedEvent)
	{
		_blockers++;
	}

	[OnEvent]
	public void OnWonderCompleted(WonderCompletedEvent wonderCompletedEvent)
	{
		_blockers--;
	}
}
