namespace Timberborn.WorkSystem;

public class WorkerChangedEventArgs
{
	public Worker Worker { get; }

	public WorkerChangedEventArgs(Worker worker)
	{
		Worker = worker;
	}
}
