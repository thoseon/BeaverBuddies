using Bindito.Core;
using Timberborn.BuildingRange;
using Timberborn.Emptying;
using Timberborn.GoodStackSystem;
using Timberborn.Hauling;
using Timberborn.LaborSystem;
using Timberborn.SimpleOutputBuildings;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;

namespace Timberborn.Gathering;

[Context("Game")]
[Context("MapEditor")]
internal class GatheringConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<GatherWorkplaceBehavior>().AsTransient();
		Bind<Gatherable>().AsTransient();
		Bind<GatherablePrioritizer>().AsTransient();
		Bind<GatherableYieldGrower>().AsTransient();
		Bind<GathererFlag>().AsTransient();
		Bind<GathererFlagYielderRetriever>().AsTransient();
		Bind<GatherableModel>().AsTransient();
		Bind<GoodStackService<GathererFlag>>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<GatherableSpec, Gatherable>();
		builder.AddDecorator<Gatherable, GatherableYieldGrower>();
		builder.AddDecorator<Gatherable, GoodStack>();
		builder.AddDecorator<Gatherable, GatherableModel>();
		builder.AddDecorator<GathererFlagSpec, GathererFlag>();
		builder.AddDecorator<GathererFlag, BuildingWithTerrainRange>();
		builder.AddDecorator<GatherWorkplaceBehavior, GatherablePrioritizer>();
		builder.AddDecorator<GathererFlag, AutoEmptiable>();
		builder.AddDecorator<GathererFlag, Emptiable>();
		builder.AddDecorator<GathererFlag, HaulCandidate>();
		builder.AddDecorator<GathererFlag, SimpleOutputInventoryHaulBehaviorProvider>();
		builder.AddDecorator<GathererFlag, GathererFlagYielderRetriever>();
		AddDecoratingBehaviors(builder);
		return builder.Build();
	}

	private static void AddDecoratingBehaviors(TemplateModule.Builder builder)
	{
		builder.AddDecorator<GathererFlag, RemoveUnwantedStockWorkplaceBehavior>();
		builder.AddDecorator<GathererFlag, GatherWorkplaceBehavior>();
		builder.AddDecorator<GathererFlag, EmptyOutputWorkplaceBehavior>();
		builder.AddDecorator<GathererFlag, EmptyInventoriesWorkplaceBehavior>();
		builder.AddDecorator<GathererFlag, LaborWorkplaceBehavior>();
		builder.AddDecorator<GathererFlag, WaitInsideIdlyWorkplaceBehavior>();
	}
}
