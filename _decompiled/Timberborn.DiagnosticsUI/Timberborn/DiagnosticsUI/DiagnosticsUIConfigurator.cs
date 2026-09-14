using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.DiagnosticsUI;

[Context("Game")]
internal class DiagnosticsUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MeshMetricsDebuggingPanel>().AsSingleton();
		Bind<FramesPerSecondPanel>().AsSingleton();
		MultiBind<IDevModule>().To<GCToggler>().AsSingleton();
		MultiBind<IDevModule>().To<GCTrigger>().AsSingleton();
		MultiBind<IDevModule>().To<EmptySceneLoader>().AsSingleton();
	}
}
