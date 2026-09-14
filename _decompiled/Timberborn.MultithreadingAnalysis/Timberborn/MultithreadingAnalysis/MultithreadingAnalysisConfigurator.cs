using Bindito.Core;
using Timberborn.Multithreading;

namespace Timberborn.MultithreadingAnalysis;

[Context("Game")]
[Context("MapEditor")]
internal class MultithreadingAnalysisConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SnapshotCollector>().AsSingleton();
		Bind<ISnapshotCollector>().ToExisting<SnapshotCollector>();
	}
}
