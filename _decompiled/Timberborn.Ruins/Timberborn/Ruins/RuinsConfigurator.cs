using Bindito.Core;
using Timberborn.BuildingRange;
using Timberborn.Demolishing;
using Timberborn.Emptying;
using Timberborn.Hauling;
using Timberborn.LaborSystem;
using Timberborn.SimpleOutputBuildings;
using Timberborn.SoilContaminationSystem;
using Timberborn.SoilMoistureSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;

namespace Timberborn.Ruins;

[Context("Game")]
[Context("MapEditor")]
internal class RuinsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ScavengerWorkplaceBehavior>().AsTransient();
		Bind<Ruin>().AsTransient();
		Bind<RuinModels>().AsTransient();
		Bind<RuinModelUpdater>().AsTransient();
		Bind<RuinsRemoveYieldStrategy>().AsTransient();
		Bind<ScavengerYielderRetriever>().AsTransient();
		Bind<InstantiatedRuinSelector>().AsTransient();
		Bind<RuinReplacer>().AsSingleton();
		Bind<RuinModelFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<RuinSpec, Ruin>();
		builder.AddDecorator<Ruin, AccessibleDemolishableReacher>();
		builder.AddDecorator<Ruin, RuinModels>();
		builder.AddDecorator<Ruin, RuinModelUpdater>();
		builder.AddDecorator<Ruin, DryObject>();
		builder.AddDecorator<Ruin, ContaminatedObject>();
		builder.AddDecorator<Ruin, RuinsRemoveYieldStrategy>();
		builder.AddDecorator<Ruin, InstantiatedRuinSelector>();
		builder.AddDecorator<ScavengerSpec, BuildingWithTerrainRange>();
		builder.AddDecorator<ScavengerSpec, AutoEmptiable>();
		builder.AddDecorator<ScavengerSpec, Emptiable>();
		builder.AddDecorator<ScavengerSpec, HaulCandidate>();
		builder.AddDecorator<ScavengerSpec, SimpleOutputInventoryHaulBehaviorProvider>();
		builder.AddDecorator<ScavengerSpec, ScavengerYielderRetriever>();
		AddDecoratingBehaviors(builder);
		return builder.Build();
	}

	private static void AddDecoratingBehaviors(TemplateModule.Builder builder)
	{
		builder.AddDecorator<ScavengerSpec, RemoveUnwantedStockWorkplaceBehavior>();
		builder.AddDecorator<ScavengerSpec, ScavengerWorkplaceBehavior>();
		builder.AddDecorator<ScavengerSpec, EmptyOutputWorkplaceBehavior>();
		builder.AddDecorator<ScavengerSpec, EmptyInventoriesWorkplaceBehavior>();
		builder.AddDecorator<ScavengerSpec, LaborWorkplaceBehavior>();
		builder.AddDecorator<ScavengerSpec, WaitInsideIdlyWorkplaceBehavior>();
	}
}
