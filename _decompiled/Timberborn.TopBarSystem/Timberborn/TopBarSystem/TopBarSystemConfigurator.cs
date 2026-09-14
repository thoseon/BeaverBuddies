using Bindito.Core;

namespace Timberborn.TopBarSystem;

[Context("Game")]
internal class TopBarSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TopBarCounterRowFactory>().AsSingleton();
		Bind<TopBarCounterFactory>().AsSingleton();
		Bind<TopBarPanel>().AsSingleton();
	}
}
