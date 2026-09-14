using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Workshops;

[Context("Game")]
internal class WorkshopsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<RecipeGoodDisallower>().AsTransient();
		Bind<FillInputWorkplaceBehavior>().AsTransient();
		Bind<ProduceWorkplaceBehavior>().AsTransient();
		Bind<WorkWorkplaceBehavior>().AsTransient();
		Bind<ProduceExecutor>().AsTransient();
		Bind<WorkExecutor>().AsTransient();
		Bind<LackOfResourcesStatus>().AsTransient();
		Bind<ProductionIncreaser>().AsTransient();
		Bind<ProductionResetter>().AsTransient();
		Bind<WorkplacePowerConsumptionSwitch>().AsTransient();
		Bind<WorkshopProductivityCounter>().AsTransient();
		Bind<WorkplaceWorkStarter>().AsTransient();
		Bind<AutomaticRecipeManufactory>().AsTransient();
		Bind<FillInputHaulBehaviorProvider>().AsTransient();
		Bind<Manufactory>().AsTransient();
		Bind<ManufactoryHaulBehaviorProvider>().AsTransient();
		Bind<ManufactoryInputChecker>().AsTransient();
		Bind<ManufactoryTogglableRecipes>().AsTransient();
		Bind<NoRecipeStatus>().AsTransient();
		Bind<Workshop>().AsTransient();
		Bind<RecipeSpecService>().AsSingleton();
		Bind<ManufactoryInventoryInitializer>().AsSingleton();
		Bind<DailyProductivitySerializer>().AsSingleton();
		Bind<HourlyProductivitySerializer>().AsSingleton();
		Bind<RecipeGoodsProcessor>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider<WorkshopsTemplateModuleProvider>().AsSingleton();
	}
}
