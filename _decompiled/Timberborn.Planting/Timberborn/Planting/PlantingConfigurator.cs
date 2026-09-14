using Bindito.Core;
using Timberborn.NaturalResourcesReproduction;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;

namespace Timberborn.Planting;

[Context("Game")]
[Context("MapEditor")]
internal class PlantingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<PlantBehavior>().AsTransient();
		Bind<PlanterWorkplaceBehavior>().AsTransient();
		Bind<PlantExecutor>().AsTransient();
		Bind<Plantable>().AsTransient();
		Bind<InRangePlantingCoordinates>().AsTransient();
		Bind<PlantablePrioritizer>().AsTransient();
		Bind<PlantableReproductionBlocker>().AsTransient();
		Bind<Planter>().AsTransient();
		Bind<PlanterBuilding>().AsTransient();
		Bind<PlanterBuildingStatusUpdater>().AsTransient();
		Bind<PlantingSpotFinder>().AsTransient();
		Bind<PlantingService>().AsSingleton();
		Bind<PlantingAreaValidator>().AsSingleton();
		Bind<PlantingSoilValidator>().AsSingleton();
		Bind<PlantingMapSerializer>().AsSingleton();
		Bind<PlantableReproductionBlockerService>().AsSingleton();
		Bind<PlantingCoordinatesUnsetter>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Worker, Planter>();
		builder.AddDecorator<Worker, PlantBehavior>();
		builder.AddDecorator<Reproducible, PlantableReproductionBlocker>();
		builder.AddDecorator<PlantablePrioritizer, PlantingSpotFinder>();
		builder.AddDecorator<PlanterBuildingSpec, PlanterBuilding>();
		builder.AddDecorator<PlanterBuilding, PlanterBuildingStatusUpdater>();
		builder.AddDecorator<PlanterBuilding, InRangePlantingCoordinates>();
		builder.AddDecorator<PlantableSpec, Plantable>();
		return builder.Build();
	}
}
