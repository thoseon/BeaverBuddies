using Bindito.Core;

namespace Timberborn.LifeSystem;

[Context("Game")]
internal class LifeSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<LifeProgressor>().AsTransient();
		Bind<LifeService>().AsSingleton();
	}
}
