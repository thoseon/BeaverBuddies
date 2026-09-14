using Bindito.Core;
using Timberborn.Particles;
using Timberborn.TemplateInstantiation;

namespace Timberborn.WindSystem;

[Context("Game")]
[Context("MapEditor")]
internal class WindSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BlockableBuildingWindAnimator>().AsTransient();
		Bind<WindParticleController>().AsTransient();
		Bind<WindRotationAnimator>().AsTransient();
		Bind<WindService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<WindParticleControllerSpec, WindParticleController>();
		builder.AddDecorator<WindParticleControllerSpec, ParticlesCache>();
		builder.AddDecorator<WindRotationAnimatorSpec, WindRotationAnimator>();
		builder.AddDecorator<BlockableBuildingWindAnimatorSpec, BlockableBuildingWindAnimator>();
		return builder.Build();
	}
}
