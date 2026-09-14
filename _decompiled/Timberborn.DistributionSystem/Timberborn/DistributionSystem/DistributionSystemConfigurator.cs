using Bindito.Core;
using Timberborn.GameDistricts;
using Timberborn.Hauling;
using Timberborn.InventoryNeedSystem;
using Timberborn.LaborSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;

namespace Timberborn.DistributionSystem;

[Context("Game")]
internal class DistributionSystemConfigurator : Configurator
{
	private class TemplateModuleProvider : IProvider<TemplateModule>
	{
		private readonly DistrictCrossingInventoryInitializer _districtCrossingInventoryInitializer;

		public TemplateModuleProvider(DistrictCrossingInventoryInitializer districtCrossingInventoryInitializer)
		{
			_districtCrossingInventoryInitializer = districtCrossingInventoryInitializer;
		}

		public TemplateModule Get()
		{
			TemplateModule.Builder builder = new TemplateModule.Builder();
			builder.AddDedicatedDecorator(_districtCrossingInventoryInitializer);
			builder.AddDecorator<DistrictCrossingSpec, DistrictCrossing>();
			builder.AddDecorator<DistrictCrossing, DistrictCrossingInventory>();
			builder.AddDecorator<DistrictCrossing, DistrictCrossingValidator>();
			builder.AddDecorator<DistrictCrossing, DistrictCrossingWorkplaceBehavior>();
			builder.AddDecorator<DistrictCrossing, DistrictCrossingAutoExporter>();
			builder.AddDecorator<DistrictCrossing, LaborWorkplaceBehavior>();
			builder.AddDecorator<DistrictCrossing, WaitInsideIdlyWorkplaceBehavior>();
			builder.AddDecorator<DistrictCrossing, WorkplaceWithBackpacks>();
			builder.AddDecorator<DistrictCrossing, InventoryNeedBehavior>();
			builder.AddDecorator<DistrictCenter, DistrictDistributableGoodProvider>();
			builder.AddDecorator<DistrictCenter, DistrictDistributionSetting>();
			builder.AddDecorator<DistrictDistributableGoodProvider, DistributionInventoryRegistry>();
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<DistrictCrossingWorkplaceBehavior>().AsTransient();
		Bind<DistrictCrossingAutoExporter>().AsTransient();
		Bind<DistributionInventoryRegistry>().AsTransient();
		Bind<DistrictCrossing>().AsTransient();
		Bind<DistrictCrossingInventory>().AsTransient();
		Bind<DistrictCrossingValidator>().AsTransient();
		Bind<DistrictDistributableGoodProvider>().AsTransient();
		Bind<DistrictDistributionSetting>().AsTransient();
		Bind<DistrictCrossingInventoryInitializer>().AsSingleton();
		Bind<GoodDistributionSettingSerializer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider<TemplateModuleProvider>().AsSingleton();
	}
}
