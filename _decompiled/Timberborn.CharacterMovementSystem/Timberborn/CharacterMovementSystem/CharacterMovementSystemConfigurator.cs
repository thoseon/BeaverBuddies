using Bindito.Core;
using Timberborn.CharacterModelSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.CharacterMovementSystem;

[Context("Game")]
internal class CharacterMovementSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<AnimatedPathFollower>().AsTransient();
		Bind<CharacterRotator>().AsTransient();
		Bind<MovementAnimator>().AsTransient();
		Bind<RunningProhibitor>().AsTransient();
		Bind<PathFollowerFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<CharacterAnimator, RunningProhibitor>();
		builder.AddDecorator<CharacterAnimator, CharacterRotator>();
		builder.AddDecorator<MovementAnimatorSpec, MovementAnimator>();
		return builder.Build();
	}
}
