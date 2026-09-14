using Bindito.Core;
using Timberborn.BlockingSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.WaterSourceSystem;

[Context("Game")]
[Context("MapEditor")]
internal class WaterSourceSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BadtideWaterSourceContaminationController>().AsTransient();
		Bind<WaterSource>().AsTransient();
		Bind<DroughtWaterStrengthModifier>().AsTransient();
		Bind<WaterDepthStrengthModifier>().AsTransient();
		Bind<WaterSourceContamination>().AsTransient();
		Bind<WaterSourceDisabler>().AsTransient();
		Bind<WaterSourceDischarger>().AsTransient();
		Bind<WaterSourceRegulator>().AsTransient();
		Bind<WaterSourceRegulatorAnimationController>().AsTransient();
		Bind<UnderlyingWaterSource>().AsTransient();
		Bind<HazardousWeatherObserver>().AsTransient();
		Bind<RegulatedWaterSourceBlocker>().AsTransient();
		Bind<DirectionalWaterSource>().AsTransient();
		Bind<WaterStrengthService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<WaterSourceSpec, WaterSource>();
		builder.AddDecorator<WaterSource, DroughtWaterStrengthModifier>();
		builder.AddDecorator<WaterSourceRegulator, UnderlyingWaterSource>();
		builder.AddDecorator<WaterSourceDisablerSpec, WaterSourceDisabler>();
		builder.AddDecorator<WaterSourceDisabler, UnderlyingWaterSource>();
		builder.AddDecorator<WaterSourceDischargerSpec, WaterSourceDischarger>();
		builder.AddDecorator<WaterSourceDischarger, UnderlyingWaterSource>();
		builder.AddDecorator<WaterSourceContaminationSpec, WaterSourceContamination>();
		builder.AddDecorator<WaterSourceRegulatorSpec, WaterSourceRegulator>();
		builder.AddDecorator<WaterSourceRegulatorAnimationControllerSpec, WaterSourceRegulatorAnimationController>();
		builder.AddDecorator<BadtideWaterSourceContaminationControllerSpec, BadtideWaterSourceContaminationController>();
		builder.AddDecorator<BadtideWaterSourceContaminationController, HazardousWeatherObserver>();
		builder.AddDecorator<WaterDepthStrengthModifierSpec, WaterDepthStrengthModifier>();
		builder.AddDecorator<RegulatedWaterSourceBlockerSpec, RegulatedWaterSourceBlocker>();
		builder.AddDecorator<RegulatedWaterSourceBlocker, BlockObjectBelowBlocker>();
		builder.AddDecorator<DirectionalWaterSourceSpec, DirectionalWaterSource>();
		return builder.Build();
	}
}
