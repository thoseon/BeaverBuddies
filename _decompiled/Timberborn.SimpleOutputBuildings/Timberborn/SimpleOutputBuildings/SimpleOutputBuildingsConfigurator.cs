using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.SimpleOutputBuildings;

[Context("Game")]
internal class SimpleOutputBuildingsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SimpleOutputInventory>().AsTransient();
		Bind<SimpleOutputInventoryHaulBehaviorProvider>().AsTransient();
		Bind<SimpleOutputInventoryInitializer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider<SimpleOutputBuildingsTemplateModuleProvider>().AsSingleton();
	}
}
