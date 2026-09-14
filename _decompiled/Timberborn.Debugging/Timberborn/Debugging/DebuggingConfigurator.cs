using Bindito.Core;
using Timberborn.KeyBindingSystem;

namespace Timberborn.Debugging;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class DebuggingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DebugModeManager>().AsSingleton();
		Bind<DebugModeController>().AsSingleton();
		Bind<DevModeManager>().AsSingleton();
		Bind<DevModeController>().AsSingleton();
		Bind<IKeyBindingBlocker>().To<DevModeKeyBindingBlocker>().AsSingleton();
		MultiBind<IDevModule>().To<TestExceptionDevModule>().AsSingleton();
	}
}
