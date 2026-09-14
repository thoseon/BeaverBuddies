using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Carrying;

[Context("Game")]
[Context("MapEditor")]
internal class CarryingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<CarryRootBehavior>().AsTransient();
		Bind<BackpackCarrier>().AsTransient();
		Bind<CarrierInventoryFinder>().AsTransient();
		Bind<GoodCarrier>().AsTransient();
		Bind<GoodCarrierAnimator>().AsTransient();
		Bind<GoodCarrierCapacityReserver>().AsTransient();
		Bind<GoodCarrierModel>().AsTransient();
		Bind<Overburdenable>().AsTransient();
		Bind<CarryAmountCalculator>().AsSingleton();
		Bind<CarriedGoodSerializer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<GoodCarrierSpec, GoodCarrier>();
		builder.AddDecorator<GoodCarrier, BackpackCarrier>();
		builder.AddDecorator<GoodCarrier, GoodCarrierAnimator>();
		builder.AddDecorator<GoodCarrier, CarrierInventoryFinder>();
		builder.AddDecorator<GoodCarrier, GoodCarrierCapacityReserver>();
		builder.AddDecorator<OverburdenableSpec, Overburdenable>();
		builder.AddDecorator<GoodCarrierModelSpec, GoodCarrierModel>();
		return builder.Build();
	}
}
