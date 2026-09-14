using Bindito.Core;
using Timberborn.Particles;
using Timberborn.StatusSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.ActivatorSystem;

[Context("Game")]
[Context("MapEditor")]
internal class ActivatorSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TimedComponentActivator>().AsTransient();
		Bind<ActivationWarningStatus>().AsTransient();
		Bind<ActivationProgressParticles>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<TimedComponentActivatorSpec, TimedComponentActivator>();
		builder.AddDecorator<ActivationWarningStatusSpec, ActivationWarningStatus>();
		builder.AddDecorator<ActivationWarningStatus, StatusSubject>();
		builder.AddDecorator<ActivationProgressParticlesSpec, ActivationProgressParticles>();
		builder.AddDecorator<ActivationProgressParticles, ParticlesCache>();
		return builder.Build();
	}
}
