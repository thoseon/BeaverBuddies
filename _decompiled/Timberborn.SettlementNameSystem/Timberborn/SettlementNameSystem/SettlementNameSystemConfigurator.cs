using Bindito.Core;

namespace Timberborn.SettlementNameSystem;

[Context("Game")]
internal class SettlementNameSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SettlementReferenceService>().AsSingleton();
	}
}
