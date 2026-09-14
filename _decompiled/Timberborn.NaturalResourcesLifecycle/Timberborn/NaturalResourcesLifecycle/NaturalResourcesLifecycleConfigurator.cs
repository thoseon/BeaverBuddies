using Bindito.Core;

namespace Timberborn.NaturalResourcesLifecycle;

[Context("Game")]
[Context("MapEditor")]
internal class NaturalResourcesLifecycleConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DyingNaturalResource>().AsTransient();
		Bind<LivingNaturalResource>().AsTransient();
	}
}
