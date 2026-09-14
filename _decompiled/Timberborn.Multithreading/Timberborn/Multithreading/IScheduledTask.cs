namespace Timberborn.Multithreading;

internal interface IScheduledTask
{
	bool AddDependent(int expectedVersion, IScheduledTask dependent);

	void Run(Parallelizer parallelizer);

	void AdvancePrerequisites(Parallelizer parallelizer);
}
