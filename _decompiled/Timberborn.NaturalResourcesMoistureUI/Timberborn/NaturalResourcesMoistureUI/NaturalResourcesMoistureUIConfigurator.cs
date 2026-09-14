using Bindito.Core;
using Timberborn.NaturalResourcesMoisture;
using Timberborn.TemplateInstantiation;

namespace Timberborn.NaturalResourcesMoistureUI;

[Context("Game")]
[Context("MapEditor")]
internal class NaturalResourcesMoistureUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<FloodableNaturalResourceDescriber>().AsTransient();
		Bind<LivingWaterNaturalResourceStatus>().AsTransient();
		Bind<WateredNaturalResourceStatus>().AsTransient();
		Bind<WateredNaturalResourceDescriber>().AsTransient();
		Bind<AridNaturalResourceDescriber>().AsTransient();
		Bind<AridNaturalResourceStatus>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<FloodableNaturalResourceSpec, FloodableNaturalResourceDescriber>();
		builder.AddDecorator<LivingWaterNaturalResource, LivingWaterNaturalResourceStatus>();
		builder.AddDecorator<WateredNaturalResource, WateredNaturalResourceStatus>();
		builder.AddDecorator<WateredNaturalResourceSpec, WateredNaturalResourceDescriber>();
		builder.AddDecorator<AridNaturalResourceSpec, AridNaturalResourceDescriber>();
		builder.AddDecorator<AridNaturalResourceSpec, AridNaturalResourceStatus>();
		return builder.Build();
	}
}
