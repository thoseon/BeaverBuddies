using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.WaterSystemUI;

[Context("Game")]
[Context("MapEditor")]
internal class WaterSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<WaterColumnDebuggingPanel>().AsSingleton();
		Bind<WaterOpacityTogglePanel>().AsSingleton();
		MultiBind<IDevModule>().To<WaterSystemDevModule>().AsSingleton();
	}
}
