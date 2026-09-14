using Bindito.Core;
using Timberborn.BeaverContaminationSystem;
using Timberborn.Beavers;
using Timberborn.BehaviorSystem;
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
using Timberborn.SleepSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.WalkingSystem;
using Timberborn.Wandering;
using Timberborn.WorkSystem;
using Timberborn.Workshops;
using Timberborn.Yielding;

namespace Timberborn.BeaverBehavior;

[Context("Game")]
internal class BeaverBehaviorConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BeaverBehaviorInitializer>().AsTransient();
		Bind<BeaverNeedBehaviorPicker>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BeaverSpec, BeaverBehaviorInitializer>();
		builder.AddDecorator<BeaverSpec, BeaverNeedBehaviorPicker>();
		builder.AddDecorator<BeaverSpec, BehaviorManager>();
		AddExecutors(builder);
		AddBehaviors(builder);
		return builder.Build();
	}

	private static void AddExecutors(TemplateModule.Builder builder)
	{
		builder.AddDecorator<BeaverSpec, WalkToPositionExecutor>();
		builder.AddDecorator<BeaverSpec, WalkInsideExecutor>();
		builder.AddDecorator<BeaverSpec, ApplyEffectExecutor>();
		builder.AddDecorator<BeaverSpec, WalkToAccessibleExecutor>();
		builder.AddDecorator<BeaverSpec, AnimateExecutor>();
		builder.AddDecorator<BeaverSpec, WaitExecutor>();
		builder.AddDecorator<AdultSpec, WorkExecutor>();
		builder.AddDecorator<AdultSpec, ProduceExecutor>();
		builder.AddDecorator<AdultSpec, PlantExecutor>();
		builder.AddDecorator<AdultSpec, WalkToReservableExecutor>();
		builder.AddDecorator<AdultSpec, RemoveYieldExecutor>();
		builder.AddDecorator<AdultSpec, BuildExecutor>();
		builder.AddDecorator<AdultSpec, DemolishExecutor>();
	}

	private static void AddBehaviors(TemplateModule.Builder builder)
	{
		builder.AddDecorator<BeaverSpec, CharacterControlRootBehavior>();
		builder.AddDecorator<BeaverSpec, DeadRootBehavior>();
		builder.AddDecorator<AdultSpec, CarryRootBehavior>();
		builder.AddDecorator<Child, ChildRootBehavior>();
		builder.AddDecorator<BeaverSpec, DieRootBehavior>();
		builder.AddDecorator<BeaverSpec, ContaminateRootBehavior>();
		builder.AddDecorator<BeaverSpec, CriticalNeederRootBehavior>();
		builder.AddDecorator<BeaverSpec, StrandedRootBehavior>();
		builder.AddDecorator<AdultSpec, WorkerRootBehavior>();
		builder.AddDecorator<BeaverSpec, NeederRootBehavior>();
		builder.AddDecorator<BeaverSpec, SleepNeedBehavior>();
		builder.AddDecorator<BeaverSpec, WanderRootBehavior>();
	}
}
