using Bindito.Core;

namespace Timberborn.InventoryNeedSystem;

[Context("Game")]
internal class InventoryNeedSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<InventoryNeedBehavior>().AsTransient();
		Bind<InventoryGoodConsumptionBlocker>().AsTransient();
		Bind<InventoryNeedBehaviorInitializer>().AsSingleton();
	}
}
