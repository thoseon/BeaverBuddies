using Bindito.Core;
using Timberborn.BehaviorSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.WalkingSystem;

[Context("Game")]
internal class WalkingSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<WalkInsideExecutor>().AsTransient();
		Bind<WalkToAccessibleExecutor>().AsTransient();
		Bind<WalkToPositionExecutor>().AsTransient();
		Bind<Walker>().AsTransient();
		Bind<NavMeshObserver>().AsTransient();
		Bind<WalkerMover>().AsTransient();
		Bind<WalkerSpeedManager>().AsTransient();
		Bind<NavMeshProximityValidator>().AsTransient();
		Bind<OccupiedAccessiblePathStart>().AsTransient();
		Bind<RunningStateUpdater>().AsTransient();
		Bind<SwimmingAnimator>().AsTransient();
		Bind<WalkerPathStart>().AsTransient();
		Bind<WalkingEnforcer>().AsTransient();
		Bind<PositionDestinationFactory>().AsSingleton();
		Bind<WalkerService>().AsSingleton();
		Bind<RandomDestinationPicker>().AsSingleton();
		Bind<DestinationValueSerializer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<WalkerSpeedManagerSpec, WalkerSpeedManager>();
		builder.AddDecorator<WalkerSpeedManager, Walker>();
		builder.AddDecorator<Walker, NavMeshObserver>();
		builder.AddDecorator<Walker, OccupiedAccessiblePathStart>();
		builder.AddDecorator<Walker, WalkerPathStart>();
		builder.AddDecorator<Walker, NavMeshProximityValidator>();
		builder.AddDecorator<RunningStateUpdaterSpec, RunningStateUpdater>();
		builder.AddDecorator<RunningStateUpdater, WalkingEnforcer>();
		builder.AddDecorator<BehaviorManager, WalkerMover>();
		builder.AddDecorator<SwimmingAnimatorSpec, SwimmingAnimator>();
		return builder.Build();
	}
}
