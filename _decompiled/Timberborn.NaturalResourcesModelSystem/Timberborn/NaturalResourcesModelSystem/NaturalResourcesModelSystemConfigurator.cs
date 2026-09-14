using Bindito.Core;
using Timberborn.NaturalResources;
using Timberborn.TemplateInstantiation;

namespace Timberborn.NaturalResourcesModelSystem;

[Context("Game")]
[Context("MapEditor")]
internal class NaturalResourcesModelSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<NaturalResourceModelRandomizer>().AsTransient();
		Bind<NaturalResourceCenterProvider>().AsTransient();
		Bind<NaturalResourceModel>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<NaturalResourceSpec, NaturalResourceModel>();
		builder.AddDecorator<NaturalResourceSpec, NaturalResourceCenterProvider>();
		builder.AddDecorator<NaturalResourceModelRandomizerSpec, NaturalResourceModelRandomizer>();
		return builder.Build();
	}
}
