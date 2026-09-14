using Bindito.Core;
using Timberborn.Emptying;
using Timberborn.Hauling;
using Timberborn.TemplateInstantiation;
using Timberborn.Workshops;

namespace Timberborn.GoodConsumingBuildingSystem;

[Context("Game")]
internal class GoodConsumingBuildingSystemConfigurator : Configurator
{
	private class TemplateModuleProvider : IProvider<TemplateModule>
	{
		private readonly GoodConsumingBuildingInventoryInitializer _goodConsumingBuildingInventoryInitializer;

		public TemplateModuleProvider(GoodConsumingBuildingInventoryInitializer goodConsumingBuildingInventoryInitializer)
		{
			_goodConsumingBuildingInventoryInitializer = goodConsumingBuildingInventoryInitializer;
		}

		public TemplateModule Get()
		{
			TemplateModule.Builder builder = new TemplateModule.Builder();
			builder.AddDecorator<GoodConsumingBuildingSpec, GoodConsumingBuilding>();
			builder.AddDecorator<GoodConsumingBuilding, AutoEmptiable>();
			builder.AddDecorator<GoodConsumingBuilding, Emptiable>();
			builder.AddDecorator<GoodConsumingBuilding, FillInputHaulBehaviorProvider>();
			builder.AddDecorator<GoodConsumingBuilding, GoodConsumingBuildingStatusInitializer>();
			builder.AddDecorator<GoodConsumingBuildingStatusInitializer, LackOfResourcesStatus>();
			builder.AddDecorator<GoodConsumingBuildingStatusInitializer, NoHaulingPostStatus>();
			builder.AddDecorator<PoweredGoodConsumingBuildingSpec, PoweredGoodConsumingBuilding>();
			builder.AddDedicatedDecorator(_goodConsumingBuildingInventoryInitializer);
			InitializeBehaviors(builder);
			return builder.Build();
		}

		private static void InitializeBehaviors(TemplateModule.Builder builder)
		{
			builder.AddDecorator<GoodConsumingBuilding, EmptyInventoriesWorkplaceBehavior>();
			builder.AddDecorator<GoodConsumingBuilding, FillInputWorkplaceBehavior>();
			builder.AddDecorator<GoodConsumingBuilding, RemoveUnwantedStockWorkplaceBehavior>();
		}
	}

	protected override void Configure()
	{
		Bind<GoodConsumingBuilding>().AsTransient();
		Bind<PoweredGoodConsumingBuilding>().AsTransient();
		Bind<GoodConsumingBuildingStatusInitializer>().AsTransient();
		Bind<GoodConsumingBuildingInventoryInitializer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider<TemplateModuleProvider>().AsSingleton();
	}
}
