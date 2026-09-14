using Bindito.Core;
using Timberborn.Characters;
using Timberborn.Navigation;
using Timberborn.TemplateInstantiation;

namespace Timberborn.ZiplineMovementSystem;

[Context("Game")]
internal class ZiplineMovementSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ZiplineCharacterAnimator>().AsTransient();
		Bind<ZiplineHarnessModel>().AsTransient();
		Bind<ZiplinePathTracker>().AsTransient();
		Bind<ZiplineSwimmingBlocker>().AsTransient();
		Bind<ZiplineVisitor>().AsTransient();
		Bind<ZiplineVisitorBoundsScaler>().AsTransient();
		Bind<ZiplineWaterPenaltyModifier>().AsTransient();
		MultiBind<IPathTransformer>().To<ZiplinePathTransformer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Character, ZiplinePathTracker>();
		builder.AddDecorator<Character, ZiplineVisitor>();
		builder.AddDecorator<ZiplineVisitor, ZiplineCharacterAnimator>();
		builder.AddDecorator<ZiplineVisitor, ZiplineVisitorBoundsScaler>();
		builder.AddDecorator<ZiplineVisitor, ZiplineSwimmingBlocker>();
		builder.AddDecorator<ZiplineVisitor, ZiplineWaterPenaltyModifier>();
		builder.AddDecorator<ZiplineHarnessModelSpec, ZiplineHarnessModel>();
		return builder.Build();
	}
}
