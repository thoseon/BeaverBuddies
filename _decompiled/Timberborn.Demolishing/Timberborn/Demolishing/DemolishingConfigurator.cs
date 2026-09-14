using Bindito.Core;
using Timberborn.BlockObjectAccesses;
using Timberborn.BuilderHubSystem;
using Timberborn.BuilderPrioritySystem;
using Timberborn.Rendering;
using Timberborn.ReservableSystem;
using Timberborn.StatusSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;

namespace Timberborn.Demolishing;

[Context("Game")]
internal class DemolishingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DemolishBehavior>().AsTransient();
		Bind<DemolishExecutor>().AsTransient();
		Bind<Demolishable>().AsTransient();
		Bind<DemolishableParticleController>().AsTransient();
		Bind<DemolishablePrioritizableEnabler>().AsTransient();
		Bind<DemolishableStatusIconOffsetter>().AsTransient();
		Bind<Demolisher>().AsTransient();
		Bind<DemolishJob>().AsTransient();
		Bind<ReachableDemolishable>().AsTransient();
		Bind<AccessibleDemolishableReacher>().AsTransient();
		Bind<DemolishableScienceReward>().AsTransient();
		Bind<DemolishJobs>().AsSingleton();
		Bind<ReservedDemolishableSerializer>().AsSingleton();
		MultiBind<IBuilderJobProvider>().To<DemolishJobProvider>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<DemolishableSpec, Demolishable>();
		builder.AddDecorator<Demolishable, DemolishablePrioritizableEnabler>();
		builder.AddDecorator<Demolishable, DemolishJob>();
		builder.AddDecorator<Demolishable, ReachableDemolishable>();
		builder.AddDecorator<Demolishable, Reservable>();
		builder.AddDecorator<Demolishable, StatusSubject>();
		builder.AddDecorator<DemolishablePrioritizableEnabler, BuilderPrioritizable>();
		builder.AddDecorator<Demolisher, DemolishBehavior>();
		builder.AddDecorator<DemolishableFromTopSpec, AccessibleDemolishableReacher>();
		builder.AddDecorator<DemolishableFromTopSpec, BlockObjectAccessible>();
		builder.AddDecorator<DemolishableFromTopSpec, HighBlockObjectAccessesAdder>();
		builder.AddDecorator<Worker, Demolisher>();
		builder.AddDecorator<DemolishableParticleControllerSpec, DemolishableParticleController>();
		builder.AddDecorator<DemolishableStatusIconOffsetter, MarkerPosition>();
		builder.AddDecorator<DemolishableStatusIconOffsetterSpec, DemolishableStatusIconOffsetter>();
		builder.AddDecorator<DemolishableScienceRewardSpec, DemolishableScienceReward>();
		return builder.Build();
	}
}
