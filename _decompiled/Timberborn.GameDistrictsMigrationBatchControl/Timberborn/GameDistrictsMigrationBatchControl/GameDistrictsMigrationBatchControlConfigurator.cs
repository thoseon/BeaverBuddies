using Bindito.Core;
using Timberborn.BatchControl;
using Timberborn.GameDistrictsMigration;

namespace Timberborn.GameDistrictsMigrationBatchControl;

[Context("Game")]
internal class GameDistrictsMigrationBatchControlConfigurator : Configurator
{
	private class BatchControlModuleProvider : IProvider<BatchControlModule>
	{
		private readonly MigrationBatchControlTab _migrationBatchControlTab;

		public BatchControlModuleProvider(MigrationBatchControlTab migrationBatchControlTab)
		{
			_migrationBatchControlTab = migrationBatchControlTab;
		}

		public BatchControlModule Get()
		{
			BatchControlModule.Builder builder = new BatchControlModule.Builder();
			builder.AddTab(_migrationBatchControlTab, 7);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<CurrentPopulationBatchControlRowItemFactory>().AsSingleton();
		Bind<DistrictMigrationSetterRowItemFactory>().AsSingleton();
		Bind<ManualMigrationBlocker>().AsSingleton();
		Bind<ManualMigrationDistrictColumnFactory>().AsSingleton();
		Bind<ManualMigrationDistrictSetter>().AsSingleton();
		Bind<ManualMigrationPanelFactory>().AsSingleton();
		Bind<ManualMigrationPopulationRowFactory>().AsSingleton();
		Bind<MigrationBatchControlRowFactory>().AsSingleton();
		Bind<MigrationBatchControlRowGroupFactory>().AsSingleton();
		Bind<MigrationBatchControlTab>().AsSingleton();
		Bind<PopulationDataBatchControlRowItemFactory>().AsSingleton();
		Bind<PopulationDistributorBatchControlRowItemFactory>().AsSingleton();
		MultiBind<BatchControlModule>().ToProvider<BatchControlModuleProvider>().AsSingleton();
	}
}
