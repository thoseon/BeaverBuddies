using Bindito.Core;

namespace Timberborn.PrioritySystemUI;

[Context("Game")]
internal class PrioritySystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<PriorityToggleFactory>().AsSingleton();
		Bind<PriorityToggleGroupFactory>().AsSingleton();
		Bind<PriorityColors>().AsSingleton();
	}
}
