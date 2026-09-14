using Bindito.Core;
using Timberborn.BuilderHubSystem;
using Timberborn.BuilderPrioritySystem;
using Timberborn.ModelHiding;
using Timberborn.StatusSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;

namespace Timberborn.RecoveredGoodSystem;

[Context("Game")]
internal class RecoveredGoodSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<RecoveredGoodStackCarryingBehavior>().AsTransient();
		Bind<NoStorageStatus>().AsTransient();
		Bind<PrioritizedRecoveredGoodStackRegistrar>().AsTransient();
		Bind<RecoveredGoodStack>().AsTransient();
		Bind<RecoveredGoodStackAccessible>().AsTransient();
		Bind<RecoveredGoodStackDisintegration>().AsTransient();
		Bind<RecoveredGoodStackModel>().AsTransient();
		Bind<RecoveredGoodStackMover>().AsTransient();
		Bind<BuildingGoodsRecoveryService>().AsSingleton();
		Bind<RecoveredGoodStackCoordinatesFinder>().AsSingleton();
		Bind<RecoveredGoodStackFactory>().AsSingleton();
		Bind<PrioritizedRecoveredGoodStackRegistry>().AsSingleton();
		Bind<RecoveredGoodStackSpawner>().AsSingleton();
		MultiBind<IBuilderJobProvider>().To<RecoverGoodStackJobProvider>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<RecoveredGoodStackSpec, RecoveredGoodStack>();
		builder.AddDecorator<RecoveredGoodStack, BuilderPrioritizable>();
		builder.AddDecorator<RecoveredGoodStack, NoStorageStatus>();
		builder.AddDecorator<RecoveredGoodStack, RecoveredGoodStackAccessible>();
		builder.AddDecorator<RecoveredGoodStack, RecoveredGoodStackMover>();
		builder.AddDecorator<RecoveredGoodStack, PrioritizedRecoveredGoodStackRegistrar>();
		builder.AddDecorator<RecoveredGoodStack, StatusSubject>();
		builder.AddDecorator<RecoveredGoodStack, HidabilityPositionUpdater>();
		builder.AddDecorator<RecoveredGoodStackDisintegrationSpec, RecoveredGoodStackDisintegration>();
		builder.AddDecorator<Worker, RecoveredGoodStackCarryingBehavior>();
		builder.AddDecorator<RecoveredGoodStackModelSpec, RecoveredGoodStackModel>();
		return builder.Build();
	}
}
