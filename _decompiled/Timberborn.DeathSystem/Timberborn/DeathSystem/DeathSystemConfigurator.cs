using Bindito.Core;

namespace Timberborn.DeathSystem;

[Context("Game")]
internal class DeathSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DieRootBehavior>().AsTransient();
	}
}
