using Bindito.Core;

namespace Timberborn.TemplateCollectionSystem;

[Context("Game")]
[Context("MapEditor")]
internal class TemplateCollectionSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TemplateCollectionService>().AsSingleton();
		MultiBind<ITemplateCollectionIdProvider>().To<CommonTemplateCollectionIdProvider>().AsSingleton();
	}
}
