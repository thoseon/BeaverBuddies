using Bindito.Core;
using Timberborn.TemplateInstantiation;
using Timberborn.Timbermesh;

namespace Timberborn.TimbermeshAnimations;

[Context("Game")]
[Context("MapEditor")]
internal class TimbermeshAnimationsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TimbermeshAnimatorController>().AsTransient();
		Bind<VertexAnimationTextureGenerator>().AsSingleton();
		Bind<NodeAnimationCache>().AsSingleton();
		Bind<AnimatorRegistry>().AsSingleton();
		Bind<VertexAnimationInitializer>().AsSingleton();
		Bind<NodeAnimationInitializer>().AsSingleton();
		MultiBind<IModelPostprocessor>().To<AnimationInitializer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<TimbermeshAnimatorControllerSpec, TimbermeshAnimatorController>();
		return builder.Build();
	}
}
