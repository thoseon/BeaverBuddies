namespace Timberborn.WorkerOutfitSystem;

internal class WorkerOutfitChangedEventArgs
{
	public WorkerOutfitSpec WorkerOutfitSpec { get; }

	public static WorkerOutfitChangedEventArgs None => new WorkerOutfitChangedEventArgs(null);

	public WorkerOutfitChangedEventArgs(WorkerOutfitSpec workerOutfitSpec)
	{
		WorkerOutfitSpec = workerOutfitSpec;
	}
}
