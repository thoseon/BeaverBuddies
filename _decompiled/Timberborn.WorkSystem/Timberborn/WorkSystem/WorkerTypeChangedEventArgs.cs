namespace Timberborn.WorkSystem;

public class WorkerTypeChangedEventArgs
{
	public string PreviousWorkerType { get; }

	public string CurrentWorkerType { get; }

	public WorkerTypeChangedEventArgs(string previousWorkerType, string currentWorkerType)
	{
		PreviousWorkerType = previousWorkerType;
		CurrentWorkerType = currentWorkerType;
	}
}
