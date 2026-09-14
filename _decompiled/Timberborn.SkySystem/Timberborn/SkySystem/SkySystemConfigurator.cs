using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.SkySystem;

[Context("Game")]
[Context("MapEditor")]
internal class SkySystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SunStopper>().AsSingleton();
		Bind<Sun>().AsSingleton();
		Bind<SkyboxPositioner>().AsSingleton();
		Bind<DayStageCycle>().AsSingleton();
		MultiBind<IDevModule>().To<SkySystemDevModule>().AsSingleton();
	}
}
