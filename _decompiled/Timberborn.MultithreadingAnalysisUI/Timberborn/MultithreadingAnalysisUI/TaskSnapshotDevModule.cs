using Timberborn.Debugging;
using Timberborn.MultithreadingAnalysis;
using Timberborn.QuickNotificationSystem;

namespace Timberborn.MultithreadingAnalysisUI;

internal class TaskSnapshotDevModule : IDevModule
{
	private readonly SnapshotCollector _snapshotCollector;

	private readonly QuickNotificationService _quickNotificationService;

	public TaskSnapshotDevModule(SnapshotCollector snapshotCollector, QuickNotificationService quickNotificationService)
	{
		_snapshotCollector = snapshotCollector;
		_quickNotificationService = quickNotificationService;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Multithreading: Snapshot (1 tick)", delegate
		{
			ScheduleSnapshot(1);
		})).AddMethod(DevMethod.Create("Multithreading: Snapshot (2 ticks)", delegate
		{
			ScheduleSnapshot(2);
		})).AddMethod(DevMethod.Create("Multithreading: Snapshot (3 ticks)", delegate
		{
			ScheduleSnapshot(3);
		}))
			.Build();
	}

	private void ScheduleSnapshot(int ticks)
	{
		_quickNotificationService.SendNotification("Collecting snapshot...");
		_snapshotCollector.ScheduleCollection(ticks);
	}
}
