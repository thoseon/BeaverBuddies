using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.GameDistrictsBatchControl;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsMigrationBatchControl;

internal class MigrationBatchControlRowGroupFactory
{
	private static readonly string MarginBottomClass = "migration-batch-control-row-group__margin-bottom";

	private readonly DistrictCenterRowItemFactory _districtCenterRowItemFactory;

	private readonly DistrictMigrationSetterRowItemFactory _districtMigrationSetterRowItemFactory;

	private readonly MigrationBatchControlRowFactory _migrationBatchControlRowFactory;

	private readonly PopulationDataBatchControlRowItemFactory _populationDataBatchControlRowItemFactory;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly BatchControlRowGroupFactory _batchControlRowGroupFactory;

	public MigrationBatchControlRowGroupFactory(DistrictCenterRowItemFactory districtCenterRowItemFactory, DistrictMigrationSetterRowItemFactory districtMigrationSetterRowItemFactory, MigrationBatchControlRowFactory migrationBatchControlRowFactory, PopulationDataBatchControlRowItemFactory populationDataBatchControlRowItemFactory, VisualElementLoader visualElementLoader, BatchControlRowGroupFactory batchControlRowGroupFactory)
	{
		_districtCenterRowItemFactory = districtCenterRowItemFactory;
		_districtMigrationSetterRowItemFactory = districtMigrationSetterRowItemFactory;
		_migrationBatchControlRowFactory = migrationBatchControlRowFactory;
		_populationDataBatchControlRowItemFactory = populationDataBatchControlRowItemFactory;
		_visualElementLoader = visualElementLoader;
		_batchControlRowGroupFactory = batchControlRowGroupFactory;
	}

	public BatchControlRowGroup Create(DistrictCenter districtCenter)
	{
		BatchControlRowGroup batchControlRowGroup = CreateBatchControlRowGroup(districtCenter);
		batchControlRowGroup.AddRow(_migrationBatchControlRowFactory.CreateAdultRow(districtCenter));
		batchControlRowGroup.AddRow(_migrationBatchControlRowFactory.CreateChildRow(districtCenter));
		batchControlRowGroup.AddRow(_migrationBatchControlRowFactory.CreateContaminatedRow(districtCenter));
		batchControlRowGroup.AddRow(_migrationBatchControlRowFactory.CreateBotRow(districtCenter));
		batchControlRowGroup.Root.AddToClassList(MarginBottomClass);
		batchControlRowGroup.UpdateVisibleRows(districtCenter);
		return batchControlRowGroup;
	}

	private BatchControlRowGroup CreateBatchControlRowGroup(DistrictCenter districtCenter)
	{
		string elementName = "Game/BatchControl/BatchControlRow";
		VisualElement root = _visualElementLoader.LoadVisualElement(elementName);
		IBatchControlRowItem batchControlRowItem = _districtCenterRowItemFactory.Create(districtCenter);
		IBatchControlRowItem batchControlRowItem2 = _districtMigrationSetterRowItemFactory.Create(districtCenter);
		IBatchControlRowItem batchControlRowItem3 = _populationDataBatchControlRowItemFactory.CreateHousingDataRowItem(districtCenter);
		EntityComponent component = districtCenter.GetComponent<EntityComponent>();
		return _batchControlRowGroupFactory.CreateUnsorted(new BatchControlRow(root, component, batchControlRowItem, batchControlRowItem2, batchControlRowItem3));
	}
}
