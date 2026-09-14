using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Particles;

[Context("Game")]
[Context("MapEditor")]
internal class ParticlesConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<FinishedStateParticlesSpeedMultiplier>().AsTransient();
		Bind<NonLinearParticlesSpeedMultiplier>().AsTransient();
		Bind<AnimationParticlesTrigger>().AsTransient();
		Bind<ParticlesCache>().AsTransient();
		Bind<ParticlesRunnerCreator>().AsTransient();
		Bind<ParticlesFastForwarder>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<AnimationParticlesTriggerSpec, AnimationParticlesTrigger>();
		builder.AddDecorator<ParticlesCache, ParticlesRunnerCreator>();
		builder.AddDecorator<ParticlesRunnerCreator, NonLinearParticlesSpeedMultiplier>();
		builder.AddDecorator<ParticlesRunnerCreator, FinishedStateParticlesSpeedMultiplier>();
		builder.AddDecorator<AnimationParticlesTrigger, ParticlesRunnerCreator>();
		return builder.Build();
	}
}
