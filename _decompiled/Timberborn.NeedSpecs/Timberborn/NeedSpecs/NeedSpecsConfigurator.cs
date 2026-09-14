using Bindito.Core;

namespace Timberborn.NeedSpecs;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class NeedSpecsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<NeedGroupSpecService>().AsSingleton();
		Bind<NeedSpecFormatter>().AsSingleton();
	}
}
