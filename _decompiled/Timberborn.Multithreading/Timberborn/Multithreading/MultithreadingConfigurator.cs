using Bindito.Core;

namespace Timberborn.Multithreading;

[Context("Game")]
[Context("MapEditor")]
internal class MultithreadingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Parallelizer>().AsSingleton();
		Bind<IParallelizer>().ToExisting<Parallelizer>();
		Bind<MainThreadPrioritySetter>().AsSingleton();
		Bind<ScheduledTaskPool>().AsSingleton();
	}
}
