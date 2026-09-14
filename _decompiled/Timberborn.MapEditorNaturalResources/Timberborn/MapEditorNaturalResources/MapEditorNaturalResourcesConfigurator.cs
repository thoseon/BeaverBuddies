using Bindito.Core;
using Timberborn.NaturalResources;
using Timberborn.NaturalResourcesMoisture;
using Timberborn.SoilMoistureSystem;
using Timberborn.StatusSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.MapEditorNaturalResources;

[Context("MapEditor")]
internal class MapEditorNaturalResourcesConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<InstantNaturalResource>().AsTransient();
		Bind<NaturalResourceLayerService>().AsSingleton();
		Bind<NaturalResourceSpawner>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<NaturalResourceSpec, StatusSubject>();
		builder.AddDecorator<WateredNaturalResourceSpec, InstantNaturalResource>();
		builder.AddDecorator<WateredNaturalResourceSpec, DryObject>();
		builder.AddDecorator<WateredNaturalResourceSpec, WateredNaturalResource>();
		builder.AddDecorator<FloodableNaturalResourceSpec, InstantNaturalResource>();
		builder.AddDecorator<FloodableNaturalResourceSpec, LivingWaterObject>();
		builder.AddDecorator<LivingWaterObject, LivingWaterNaturalResource>();
		return builder.Build();
	}
}
