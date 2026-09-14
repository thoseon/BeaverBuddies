using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.Diagnostics;

[Context("Game")]
[Context("MapEditor")]
internal class DiagnosticsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MeshMetricsRetriever>().AsSingleton();
		Bind<SelectedMeshMetrics>().AsSingleton();
		Bind<FramesPerSecondCounter>().AsSingleton();
		MultiBind<IDevModule>().To<MeshMetricsDumper>().AsSingleton();
	}
}
