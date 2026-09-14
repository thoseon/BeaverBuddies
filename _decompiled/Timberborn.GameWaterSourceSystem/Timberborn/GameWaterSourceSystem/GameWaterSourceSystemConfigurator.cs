using Bindito.Core;
using Timberborn.TemplateInstantiation;
using Timberborn.WaterSourceSystem;

namespace Timberborn.GameWaterSourceSystem;

[Context("Game")]
internal class GameWaterSourceSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<UndergroundWaterSource>().AsTransient();
		Bind<UndergroundWaterSourceDrill>().AsTransient();
		Bind<WaterSourceActivator>().AsTransient();
		Bind<WaterSourceActivatorOverrider>().AsTransient();
		Bind<HazardousWeatherWaterSource>().AsTransient();
		Bind<UndergroundWaterSourceDrillSounds>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<UndergroundWaterSourceDrillSpec, UndergroundWaterSourceDrill>();
		builder.AddDecorator<UndergroundWaterSourceDrill, UnderlyingWaterSource>();
		builder.AddDecorator<UndergroundWaterSourceDrill, HazardousWeatherObserver>();
		builder.AddDecorator<UndergroundWaterSourceDrill, UndergroundWaterSourceDrillSounds>();
		builder.AddDecorator<UndergroundWaterSourceSpec, UndergroundWaterSource>();
		builder.AddDecorator<UndergroundWaterSource, HazardousWeatherObserver>();
		builder.AddDecorator<WaterSource, WaterSourceActivator>();
		builder.AddDecorator<WaterSourceDischargerSpec, WaterSourceActivatorOverrider>();
		builder.AddDecorator<HazardousWeatherWaterSourceSpec, HazardousWeatherWaterSource>();
		return builder.Build();
	}
}
