using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.GoodStackSystem;

[Context("Game")]
[Context("MapEditor")]
internal class GoodStackSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<GoodStackRetrieverBehavior>().AsTransient();
		Bind<GoodStack>().AsTransient();
		Bind<GoodStackAccessible>().AsTransient();
		Bind<GoodStackModel>().AsTransient();
		Bind<GoodStackInventoryInitializer>().AsSingleton();
		Bind<GoodStackModelFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider<GoodStackSystemTemplateModuleProvider>().AsSingleton();
	}
}
