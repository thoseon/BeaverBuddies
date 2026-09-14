namespace Timberborn.Multithreading;

public interface ITaskRunner
{
	int ExpectedRuns { get; }

	void Run(int runIndex);
}
