using Bindito.Core;
using Timberborn.TemplateInstantiation;
using Timberborn.WaterSourceSystem;

namespace Timberborn.MapEditorHazardousWeatherUI;

[Context("MapEditor")]
internal class MapEditorHazardousWeatherUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MapEditorWaterContaminationController>().AsTransient();
		Bind<MapEditorWaterStrengthModifier>().AsTransient();
		Bind<MapEditorHazardousWeatherWaterSource>().AsTransient();
		Bind<MapEditorHazardousWeatherSetter>().AsSingleton();
		Bind<HazardousWeatherToggleFactory>().AsSingleton();
		Bind<MapEditorHazardousWeatherPanel>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<WaterSource, MapEditorWaterStrengthModifier>();
		builder.AddDecorator<WaterSourceContamination, MapEditorWaterContaminationController>();
		builder.AddDecorator<HazardousWeatherWaterSourceSpec, MapEditorHazardousWeatherWaterSource>();
		return builder.Build();
	}
}
