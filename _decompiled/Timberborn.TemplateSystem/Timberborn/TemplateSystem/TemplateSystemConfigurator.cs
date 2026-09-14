using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.TemplateSystem;

[Context("Game")]
[Context("MapEditor")]
internal class TemplateSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<InstantiatedTemplate>().AsTransient();
		Bind<TemplateNameRetriever>().AsSingleton();
		Bind<TemplateNameMapper>().AsSingleton();
		Bind<TemplateInstantiationOrderService>().AsSingleton();
		Bind<TemplateService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<TemplateSpec, InstantiatedTemplate>();
		return builder.Build();
	}
}
