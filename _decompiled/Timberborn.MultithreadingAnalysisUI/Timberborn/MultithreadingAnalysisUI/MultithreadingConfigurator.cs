using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.MultithreadingAnalysisUI;

[Context("Game")]
internal class MultithreadingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TaskSnapshotPanel>().AsSingleton();
		Bind<ThreadViewFactory>().AsSingleton();
		Bind<TaskViewFactory>().AsSingleton();
		Bind<MarkerViewFactory>().AsSingleton();
		Bind<TaskColorProvider>().AsSingleton();
		Bind<SnapshotTimeline>().AsSingleton();
		MultiBind<IDevModule>().To<TaskSnapshotDevModule>().AsSingleton();
	}
}
