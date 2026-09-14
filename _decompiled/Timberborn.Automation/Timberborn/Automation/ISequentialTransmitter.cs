namespace Timberborn.Automation;

public interface ISequentialTransmitter : ITransmitter
{
	bool IsProcessingNewInput { get; }

	void EvaluateNext();

	void CommitTick();

	void Reset();
}
