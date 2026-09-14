using System;

namespace Timberborn.TickSystem;

public interface ITickableSingletonService
{
	TimeSpan LastParallelTickDuration { get; }

	bool ParalleTicklIsFinished { get; }

	bool IsStartingParallelTick { get; }

	event EventHandler ForcedParallelTickFinished;

	void TickAll();

	void ForceFinishParallelTick();
}
