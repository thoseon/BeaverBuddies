using Bindito.Core;
using Timberborn.Debugging;
using Timberborn.EntityPanelSystem;
using Timberborn.Particles;
using Timberborn.PowerGeneration;
using Timberborn.TemplateInstantiation;

namespace Timberborn.PowerGenerationUI;

[Context("Game")]
internal class PowerGenerationUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly AdjustableStrengthPowerGeneratorFragment _sliderFragment;

		public EntityPanelModuleProvider(AdjustableStrengthPowerGeneratorFragment sliderFragment)
		{
			_sliderFragment = sliderFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddBottomFragment(_sliderFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<GoodPoweredGeneratorAnimator>().AsTransient();
		Bind<PowerGeneratorParticleController>().AsTransient();
		Bind<WindPoweredGeneratorAnimator>().AsTransient();
		Bind<WaterPoweredGeneratorAnimator>().AsTransient();
		Bind<WaterPoweredGeneratorPreview>().AsTransient();
		Bind<AdjustableStrengthPowerGeneratorFragment>().AsSingleton();
		Bind<WaterPoweredGeneratorSpeedCalculator>().AsSingleton();
		Bind<WaterPoweredGeneratorPreviewPanel>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<IDevModule>().To<WaterPoweredGeneratorSpeedChanger>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<WaterPoweredGenerator, WaterPoweredGeneratorAnimator>();
		builder.AddDecorator<PowerGeneratorParticleControllerSpec, PowerGeneratorParticleController>();
		builder.AddDecorator<PowerGeneratorParticleController, ParticlesCache>();
		builder.AddDecorator<GoodPoweredGeneratorAnimatorSpec, GoodPoweredGeneratorAnimator>();
		builder.AddDecorator<WindPoweredGeneratorAnimatorSpec, WindPoweredGeneratorAnimator>();
		builder.AddDecorator<WaterPoweredGenerator, WaterPoweredGeneratorPreview>();
		return builder.Build();
	}
}
