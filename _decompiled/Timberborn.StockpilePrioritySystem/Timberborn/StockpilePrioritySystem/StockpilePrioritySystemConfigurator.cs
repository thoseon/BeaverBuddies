using Bindito.Core;
using Timberborn.Hauling;
using Timberborn.Stockpiles;
using Timberborn.TemplateInstantiation;

namespace Timberborn.StockpilePrioritySystem;

[Context("Game")]
internal class StockpilePrioritySystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<StockpilePriority>().AsTransient();
		Bind<ObtainGoodWorkplaceBehavior>().AsTransient();
		Bind<SupplyGoodWorkplaceBehavior>().AsTransient();
		Bind<GoodObtainer>().AsTransient();
		Bind<GoodObtainerStatusInitializer>().AsTransient();
		Bind<GoodSupplier>().AsTransient();
		Bind<ObtainGoodHaulBehaviorProvider>().AsTransient();
		Bind<StockpilePriorityChangeListener>().AsTransient();
		Bind<SupplyGoodHaulBehaviorProvider>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Stockpile, StockpilePriority>();
		builder.AddDecorator<StockpilePriority, GoodObtainer>();
		builder.AddDecorator<GoodObtainer, ObtainGoodWorkplaceBehavior>();
		builder.AddDecorator<GoodObtainer, GoodObtainerStatusInitializer>();
		builder.AddDecorator<GoodObtainerStatusInitializer, NoHaulingPostStatus>();
		builder.AddDecorator<ObtainGoodWorkplaceBehavior, ObtainGoodHaulBehaviorProvider>();
		builder.AddDecorator<StockpilePriority, GoodSupplier>();
		builder.AddDecorator<GoodSupplier, SupplyGoodWorkplaceBehavior>();
		builder.AddDecorator<SupplyGoodWorkplaceBehavior, SupplyGoodHaulBehaviorProvider>();
		builder.AddDecorator<Stockpile, StockpilePriorityChangeListener>();
		return builder.Build();
	}
}
