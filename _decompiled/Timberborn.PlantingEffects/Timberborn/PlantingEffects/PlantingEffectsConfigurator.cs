using Bindito.Core;
using Timberborn.Particles;
using Timberborn.Planting;
using Timberborn.TemplateInstantiation;

namespace Timberborn.PlantingEffects;

[Context("Game")]
internal class PlantingEffectsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<PlantingParticleController>().AsTransient();
		Bind<PlantingAnimationController>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Planter, PlantingAnimationController>();
		builder.AddDecorator<PlantingParticleControllerSpec, PlantingParticleController>();
		builder.AddDecorator<PlantingParticleController, ParticlesCache>();
		return builder.Build();
	}
}
