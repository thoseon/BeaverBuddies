using Bindito.Core;
using Timberborn.LaborSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;
using Timberborn.Workshops;

namespace Timberborn.PowerGeneration;

[Context("Game")]
internal class PowerGenerationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<GoodPoweredGenerator>().AsTransient();
		Bind<PowerGeneratorSounds>().AsTransient();
		Bind<WalkerPoweredGenerator>().AsTransient();
		Bind<WaterPoweredGenerator>().AsTransient();
		Bind<AdjustableStrengthPowerGenerator>().AsTransient();
		Bind<RotationMechanicalNodeUpdater>().AsTransient();
		Bind<WindPoweredGenerator>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		InitializeBehaviors(builder);
		builder.AddDecorator<WaterPoweredGeneratorSpec, WaterPoweredGenerator>();
		builder.AddDecorator<WaterPoweredGenerator, RotationMechanicalNodeUpdater>();
		builder.AddDecorator<WindPoweredGeneratorSpec, WindPoweredGenerator>();
		builder.AddDecorator<GoodPoweredGeneratorSpec, GoodPoweredGenerator>();
		builder.AddDecorator<PowerGeneratorSoundsSpec, PowerGeneratorSounds>();
		builder.AddDecorator<WalkerPoweredGeneratorSpec, WalkerPoweredGenerator>();
		builder.AddDecorator<AdjustableStrengthPowerGeneratorSpec, AdjustableStrengthPowerGenerator>();
		return builder.Build();
	}

	private static void InitializeBehaviors(TemplateModule.Builder builder)
	{
		builder.AddDecorator<WalkerPoweredGenerator, WorkWorkplaceBehavior>();
		builder.AddDecorator<WalkerPoweredGenerator, LaborWorkplaceBehavior>();
		builder.AddDecorator<WalkerPoweredGenerator, WaitInsideIdlyWorkplaceBehavior>();
	}
}
