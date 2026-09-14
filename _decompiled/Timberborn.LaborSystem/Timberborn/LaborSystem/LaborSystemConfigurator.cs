using Bindito.Core;

namespace Timberborn.LaborSystem;

[Context("Game")]
internal class LaborSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<LaborWorkplaceBehavior>().AsTransient();
	}
}
