using Bindito.Core;
using Timberborn.NaturalResources;
using Timberborn.SoilMoistureSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.NaturalResourcesMoisture;

[Context("Game")]
[Context("MapEditor")]
internal class NaturalResourcesMoistureConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<LivingWaterNaturalResource>().AsTransient();
		Bind<LivingWaterObject>().AsTransient();
		Bind<WateredNaturalResource>().AsTransient();
		Bind<AridNaturalResource>().AsTransient();
		Bind<FloodableNaturalResourceService>().AsSingleton();
		Bind<AridNaturalResourceService>().AsSingleton();
		MultiBind<ISpawnValidator>().To<WateredNaturalResourceSpawnValidator>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<WateredNaturalResourceSpec, WateredNaturalResource>();
		builder.AddDecorator<WateredNaturalResource, DryObject>();
		builder.AddDecorator<AridNaturalResourceSpec, AridNaturalResource>();
		builder.AddDecorator<AridNaturalResource, DryObject>();
		builder.AddDecorator<FloodableNaturalResourceSpec, LivingWaterObject>();
		builder.AddDecorator<LivingWaterObject, LivingWaterNaturalResource>();
		return builder.Build();
	}
}
