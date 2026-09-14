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

namespace Timberborn.Fields;

[Context("Game")]
internal class FieldsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<FarmHouseGoodStackRetrieverWorkplaceBehavior>().AsTransient();
		Bind<FarmHouseWorkplaceBehavior>().AsTransient();
		Bind<Crop>().AsTransient();
		Bind<FarmHouse>().AsTransient();
		Bind<FarmHouseYielderRetriever>().AsTransient();
		Bind<HarvestStarter>().AsTransient();
		Bind<GoodStackService<FarmHouse>>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<CropSpec, Crop>();
		builder.AddDecorator<FarmHouseSpec, FarmHouse>();
		builder.AddDecorator<FarmHouse, PlantablePrioritizer>();
		builder.AddDecorator<FarmHouse, BuildingWithTerrainRange>();
		builder.AddDecorator<FarmHouse, AutoEmptiable>();
		builder.AddDecorator<FarmHouse, Emptiable>();
		builder.AddDecorator<FarmHouse, HaulCandidate>();
		builder.AddDecorator<FarmHouse, SimpleOutputInventoryHaulBehaviorProvider>();
		builder.AddDecorator<FarmHouse, FarmHouseYielderRetriever>();
		builder.AddDecorator<Worker, HarvestStarter>();
		InitializeBehaviors(builder);
		return builder.Build();
	}

	private static void InitializeBehaviors(TemplateModule.Builder builder)
	{
		builder.AddDecorator<FarmHouse, RemoveUnwantedStockWorkplaceBehavior>();
		builder.AddDecorator<FarmHouse, FarmHouseGoodStackRetrieverWorkplaceBehavior>();
		builder.AddDecorator<FarmHouse, FarmHouseWorkplaceBehavior>();
		builder.AddDecorator<FarmHouse, EmptyOutputWorkplaceBehavior>();
		builder.AddDecorator<FarmHouse, EmptyInventoriesWorkplaceBehavior>();
		builder.AddDecorator<FarmHouse, LaborWorkplaceBehavior>();
		builder.AddDecorator<FarmHouse, WaitInsideIdlyWorkplaceBehavior>();
	}
}
