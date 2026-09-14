using Bindito.Core;
using Timberborn.GameDistricts;
using Timberborn.TemplateInstantiation;

namespace Timberborn.GameDistrictsMigration;

[Context("Game")]
internal class GameDistrictsMigrationConfigurator : Configurator
{
	private class GameDistrictsMigrationTemplateModuleProvider : IProvider<TemplateModule>
	{
		private readonly DistributorTemplateInitializer _distributorTemplateInitializer;

		public GameDistrictsMigrationTemplateModuleProvider(DistributorTemplateInitializer distributorTemplateInitializer)
		{
			_distributorTemplateInitializer = distributorTemplateInitializer;
		}

		public TemplateModule Get()
		{
			TemplateModule.Builder builder = new TemplateModule.Builder();
			builder.AddDecorator<DistrictCenter, AdultsDistributorTemplate>();
			builder.AddDecorator<DistrictCenter, BotsDistributorTemplate>();
			builder.AddDecorator<DistrictCenter, ChildrenDistributorTemplate>();
			builder.AddDecorator<DistrictCenter, ContaminatedDistributorTemplate>();
			builder.AddDecorator<DistrictCenter, MigrationTrigger>();
			builder.AddDedicatedDecorator(_distributorTemplateInitializer);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<AdultsDistributorTemplate>().AsTransient();
		Bind<BotsDistributorTemplate>().AsTransient();
		Bind<ChildrenDistributorTemplate>().AsTransient();
		Bind<ContaminatedDistributorTemplate>().AsTransient();
		Bind<MigrationTrigger>().AsTransient();
		Bind<PopulationDistributor>().AsTransient();
		Bind<DistributorTemplateInitializer>().AsSingleton();
		Bind<MigrationCoordinator>().AsSingleton();
		Bind<MigrationNeighbours>().AsSingleton();
		Bind<MigrationService>().AsSingleton();
		Bind<PopulationDistributorRetriever>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider<GameDistrictsMigrationTemplateModuleProvider>().AsSingleton();
	}
}
