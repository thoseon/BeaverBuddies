using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.DebuggingUI;

[Context("Game")]
[Context("MapEditor")]
internal class DebuggingUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DebugPanelMover>().AsTransient();
		Bind<DevPanel>().AsSingleton();
		Bind<DebuggingPanel>().AsSingleton();
		Bind<ObjectDebuggingPanel>().AsSingleton();
		Bind<ObjectSelector>().AsSingleton();
		Bind<ObjectViewer>().AsSingleton();
		Bind<ObjectViewerNodeFactory>().AsSingleton();
		MultiBind<IDevModule>().To<DebuggingPanelResetter>().AsSingleton();
	}
}
