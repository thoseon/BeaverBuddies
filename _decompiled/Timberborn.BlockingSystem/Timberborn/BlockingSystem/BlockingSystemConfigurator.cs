using Bindito.Core;
using Timberborn.Particles;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BlockingSystem;

[Context("Game")]
[Context("MapEditor")]
internal class BlockingSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BlockableObject>().AsTransient();
		Bind<BlockableObjectAnimationController>().AsTransient();
		Bind<BlockableObjectParticleController>().AsTransient();
		Bind<BlockableObjectVisualizer>().AsTransient();
		Bind<BlockObjectBelowBlocker>().AsTransient();
		Bind<FinishedBlockObjectBelowBlocker>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BlockableObjectVisualizerSpec, BlockableObjectVisualizer>();
		builder.AddDecorator<BlockableObjectParticleControllerSpec, BlockableObjectParticleController>();
		builder.AddDecorator<BlockableObjectParticleController, BlockableObject>();
		builder.AddDecorator<BlockableObjectParticleController, ParticlesCache>();
		builder.AddDecorator<BlockableObjectAnimationControllerSpec, BlockableObjectAnimationController>();
		builder.AddDecorator<FinishedBlockObjectBelowBlockerSpec, FinishedBlockObjectBelowBlocker>();
		builder.AddDecorator<FinishedBlockObjectBelowBlocker, BlockObjectBelowBlocker>();
		return builder.Build();
	}
}
