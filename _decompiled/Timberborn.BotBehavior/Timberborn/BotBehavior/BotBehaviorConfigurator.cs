using Bindito.Core;
using Timberborn.BehaviorSystem;
using Timberborn.Bots;
using Timberborn.Carrying;
using Timberborn.CharacterControlSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.ConstructionSites;
using Timberborn.DeathSystem;
using Timberborn.Demolishing;
using Timberborn.MortalSystem;
using Timberborn.NeedBehaviorSystem;
using Timberborn.Planting;
using Timberborn.ReservableSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.WalkingSystem;
using Timberborn.Wandering;
using Timberborn.WorkSystem;
using Timberborn.Workshops;
using Timberborn.Yielding;

namespace Timberborn.BotBehavior;

[Context("Game")]
internal class BotBehaviorConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BotBehaviorInitializer>().AsTransient();
		Bind<BotNeedBehaviorPicker>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BotSpec, BotBehaviorInitializer>();
		builder.AddDecorator<BotSpec, BotNeedBehaviorPicker>();
		builder.AddDecorator<BotSpec, BehaviorManager>();
		AddExecutors(builder);
		AddBehaviors(builder);
		return builder.Build();
	}

	private static void AddExecutors(TemplateModule.Builder builder)
	{
		builder.AddDecorator<BotSpec, WalkToPositionExecutor>();
		builder.AddDecorator<BotSpec, WalkInsideExecutor>();
		builder.AddDecorator<BotSpec, ApplyEffectExecutor>();
		builder.AddDecorator<BotSpec, WalkToAccessibleExecutor>();
		builder.AddDecorator<BotSpec, AnimateExecutor>();
		builder.AddDecorator<BotSpec, WaitExecutor>();
		builder.AddDecorator<BotSpec, WorkExecutor>();
		builder.AddDecorator<BotSpec, ProduceExecutor>();
		builder.AddDecorator<BotSpec, PlantExecutor>();
		builder.AddDecorator<BotSpec, WalkToReservableExecutor>();
		builder.AddDecorator<BotSpec, RemoveYieldExecutor>();
		builder.AddDecorator<BotSpec, BuildExecutor>();
		builder.AddDecorator<BotSpec, DemolishExecutor>();
	}

	private static void AddBehaviors(TemplateModule.Builder builder)
	{
		builder.AddDecorator<BotSpec, CharacterControlRootBehavior>();
		builder.AddDecorator<BotSpec, DeadRootBehavior>();
		builder.AddDecorator<BotSpec, CarryRootBehavior>();
		builder.AddDecorator<BotSpec, DieRootBehavior>();
		builder.AddDecorator<BotSpec, CriticalNeederRootBehavior>();
		builder.AddDecorator<BotSpec, StrandedRootBehavior>();
		builder.AddDecorator<BotSpec, NeederRootBehavior>();
		builder.AddDecorator<BotSpec, WorkerRootBehavior>();
		builder.AddDecorator<BotSpec, WanderRootBehavior>();
	}
}
