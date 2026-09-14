using Bindito.Core;

namespace Timberborn.MortalComponents;

[Context("Game")]
internal class MortalComponentsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DeadComponentDisabler>().AsSingleton();
	}
}
