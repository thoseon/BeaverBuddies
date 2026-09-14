using Bindito.Core;
using Timberborn.BuildingRange;
using Timberborn.Emptying;
using Timberborn.GoodStackSystem;
using Timberborn.Hauling;
using Timberborn.LaborSystem;
using Timberborn.Planting;
using Timberborn.SimpleOutputBuildings;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;

namespace Timberborn.Forestry;

[Context("Game")]
internal class ForestryConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<LumberjackFlagWorkplaceBehavior>().AsTransient();
		Bind<Forester>().AsTransient();
		Bind<LumberjackFlagStatusDeactivator>().AsTransient();
		Bind<LumberjackGoodStackAdder>().AsTransient();
		Bind<TreeComponent>().AsTransient();
		Bind<TreeCutter>().AsTransient();
		Bind<TreeRemoveYieldStrategy>().AsTransient();
		Bind<TreeReacher>().AsTransient();
		Bind<GoodStackService<LumberjackFlagSpec>>().AsSingleton();
		Bind<TreeCuttingArea>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		InitializeBehaviors(builder);
		builder.AddDecorator<TreeComponentSpec, TreeComponent>();
		builder.AddDecorator<TreeComponent, LumberjackGoodStackAdder>();
		builder.AddDecorator<TreeComponent, TreeRemoveYieldStrategy>();
		builder.AddDecorator<TreeComponent, TreeReacher>();
		builder.AddDecorator<BushSpec, LumberjackGoodStackAdder>();
		builder.AddDecorator<ForesterSpec, Forester>();
		builder.AddDecorator<Forester, PlantablePrioritizer>();
		builder.AddDecorator<Forester, BuildingWithTerrainRange>();
		builder.AddDecorator<LumberjackFlagSpec, BuildingWithTerrainRange>();
		builder.AddDecorator<LumberjackFlagSpec, AutoEmptiable>();
		builder.AddDecorator<LumberjackFlagSpec, Emptiable>();
		builder.AddDecorator<LumberjackFlagSpec, HaulCandidate>();
		builder.AddDecorator<LumberjackFlagSpec, SimpleOutputInventoryHaulBehaviorProvider>();
		builder.AddDecorator<LumberjackFlagSpec, LumberjackFlagStatusDeactivator>();
		builder.AddDecorator<Worker, TreeCutter>();
		return builder.Build();
	}

	private static void InitializeBehaviors(TemplateModule.Builder builder)
	{
		builder.AddDecorator<Forester, PlanterWorkplaceBehavior>();
		builder.AddDecorator<Forester, LaborWorkplaceBehavior>();
		builder.AddDecorator<Forester, WaitInsideIdlyWorkplaceBehavior>();
		builder.AddDecorator<LumberjackFlagSpec, LumberjackFlagWorkplaceBehavior>();
		builder.AddDecorator<LumberjackFlagSpec, EmptyOutputWorkplaceBehavior>();
		builder.AddDecorator<LumberjackFlagSpec, EmptyInventoriesWorkplaceBehavior>();
		builder.AddDecorator<LumberjackFlagSpec, RemoveUnwantedStockWorkplaceBehavior>();
		builder.AddDecorator<LumberjackFlagSpec, LaborWorkplaceBehavior>();
		builder.AddDecorator<LumberjackFlagSpec, WaitInsideIdlyWorkplaceBehavior>();
	}
}
