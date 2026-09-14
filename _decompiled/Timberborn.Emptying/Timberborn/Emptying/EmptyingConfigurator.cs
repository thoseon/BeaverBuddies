using Bindito.Core;
using Timberborn.Carrying;
using Timberborn.GameDistricts;
using Timberborn.Hauling;
using Timberborn.InventorySystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Emptying;

[Context("Game")]
internal class EmptyingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<EmptyInventoriesLaborBehavior>().AsTransient();
		Bind<EmptyInventoriesWorkplaceBehavior>().AsTransient();
		Bind<EmptyOutputWorkplaceBehavior>().AsTransient();
		Bind<RemoveUnwantedStockLaborBehavior>().AsTransient();
		Bind<RemoveUnwantedStockWorkplaceBehavior>().AsTransient();
		Bind<AutoEmptiable>().AsTransient();
		Bind<AutoEmptiableBlocker>().AsTransient();
		Bind<DistrictEmptiableInventoriesRegistry>().AsTransient();
		Bind<DistrictUnwantedStockInventoryRegistry>().AsTransient();
		Bind<Emptiable>().AsTransient();
		Bind<EmptiableHaulBehaviorProvider>().AsTransient();
		Bind<EmptyingStarter>().AsTransient();
		Bind<UnwantedStockHaulBehaviorProvider>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<GoodCarrier, EmptyingStarter>();
		builder.AddDecorator<Emptiable, HaulCandidate>();
		builder.AddDecorator<Emptiable, EmptiableHaulBehaviorProvider>();
		builder.AddDecorator<Emptiable, UnwantedStockHaulBehaviorProvider>();
		builder.AddDecorator<AutoEmptiable, AutoEmptiableBlocker>();
		builder.AddDecorator<DistrictBuildingRegistry, DistrictEmptiableInventoriesRegistry>();
		builder.AddDecorator<DistrictInventoryRegistry, DistrictUnwantedStockInventoryRegistry>();
		return builder.Build();
	}
}
