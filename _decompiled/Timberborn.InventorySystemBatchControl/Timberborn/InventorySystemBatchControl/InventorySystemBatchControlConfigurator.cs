using Bindito.Core;

namespace Timberborn.InventorySystemBatchControl;

[Context("Game")]
internal class InventorySystemBatchControlConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<InventoryCapacityBatchControlRowItemFactory>().AsSingleton();
	}
}
