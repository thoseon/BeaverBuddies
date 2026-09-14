using Bindito.Core;
using Timberborn.Emptying;
using Timberborn.Hauling;
using Timberborn.InventoryNeedSystem;
using Timberborn.LaborSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;

namespace Timberborn.Workshops;

public class WorkshopsTemplateModuleProvider : IProvider<TemplateModule>
{
	private readonly ManufactoryInventoryInitializer _manufactoryInventoryInitializer;

	public WorkshopsTemplateModuleProvider(ManufactoryInventoryInitializer manufactoryInventoryInitializer)
	{
		_manufactoryInventoryInitializer = manufactoryInventoryInitializer;
	}

	public TemplateModule Get()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDedicatedDecorator(_manufactoryInventoryInitializer);
		builder.AddDecorator<ManufactorySpec, Manufactory>();
		builder.AddDecorator<Manufactory, AutoEmptiable>();
		builder.AddDecorator<Manufactory, Emptiable>();
		builder.AddDecorator<Manufactory, HaulCandidate>();
		builder.AddDecorator<Manufactory, ManufactoryHaulBehaviorProvider>();
		builder.AddDecorator<Manufactory, ManufactoryInputChecker>();
		builder.AddDecorator<ManufactoryInputChecker, LackOfResourcesStatus>();
		builder.AddDecorator<Manufactory, NoRecipeStatus>();
		builder.AddDecorator<Manufactory, RecipeGoodDisallower>();
		builder.AddDecorator<Manufactory, InventoryNeedBehavior>();
		builder.AddDecorator<WorkshopSpec, Workshop>();
		builder.AddDecorator<Workshop, WorkshopProductivityCounter>();
		builder.AddDecorator<Workshop, WorkplacePowerConsumptionSwitch>();
		builder.AddDecorator<Worker, WorkplaceWorkStarter>();
		builder.AddDecorator<ManufactoryTogglableRecipesSpec, ManufactoryTogglableRecipes>();
		builder.AddDecorator<ProductionResetterSpec, ProductionResetter>();
		builder.AddDecorator<ProductionIncreaserSpec, ProductionIncreaser>();
		InitializeBehaviors(builder);
		return builder.Build();
	}

	private static void InitializeBehaviors(TemplateModule.Builder builder)
	{
		builder.AddDecorator<SimpleManufactoryBehaviorsSpec, ProduceWorkplaceBehavior>();
		builder.AddDecorator<SimpleManufactoryBehaviorsSpec, EmptyOutputWorkplaceBehavior>();
		builder.AddDecorator<SimpleManufactoryBehaviorsSpec, FillInputWorkplaceBehavior>();
		builder.AddDecorator<SimpleManufactoryBehaviorsSpec, EmptyInventoriesWorkplaceBehavior>();
		builder.AddDecorator<SimpleManufactoryBehaviorsSpec, RemoveUnwantedStockWorkplaceBehavior>();
		builder.AddDecorator<SimpleManufactoryBehaviorsSpec, LaborWorkplaceBehavior>();
		builder.AddDecorator<SimpleManufactoryBehaviorsSpec, WaitInsideIdlyWorkplaceBehavior>();
	}
}
