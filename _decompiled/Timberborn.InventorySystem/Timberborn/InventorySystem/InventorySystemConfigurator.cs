using Bindito.Core;
using Timberborn.Characters;
using Timberborn.GameDistricts;
using Timberborn.TemplateInstantiation;

namespace Timberborn.InventorySystem;

[Context("Game")]
[Context("MapEditor")]
internal class InventorySystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SingleGoodAllower>().AsTransient();
		Bind<Inventories>().AsTransient();
		Bind<DistrictInventoryAssigner>().AsTransient();
		Bind<DistrictInventoryPicker>().AsTransient();
		Bind<DistrictInventoryRegistry>().AsTransient();
		Bind<GoodReserver>().AsTransient();
		Bind<Inventory>().AsTransient();
		Bind<InventoryFillCalculator>().AsSingleton();
		Bind<GoodReservationValueSerializer>().AsSingleton();
		Bind<InventoryInitializerFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Character, GoodReserver>();
		builder.AddDecorator<DistrictCenter, DistrictInventoryRegistry>();
		builder.AddDecorator<DistrictCenter, DistrictInventoryPicker>();
		builder.AddDecorator<DistrictBuilding, DistrictInventoryAssigner>();
		builder.AddDecorator<Inventory, Inventories>();
		return builder.Build();
	}
}
