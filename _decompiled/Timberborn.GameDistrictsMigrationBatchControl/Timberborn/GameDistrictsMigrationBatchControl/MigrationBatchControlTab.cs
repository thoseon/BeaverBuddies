using System.Collections.Generic;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.Localization;
using Timberborn.Population;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsMigrationBatchControl;

internal class MigrationBatchControlTab : BatchControlTab
{
	private static readonly string AutomaticMigrationLocKey = "Migration.AutomaticMigration";

	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private readonly ILoc _loc;

	private readonly ManualMigrationPanelFactory _manualMigrationPanelFactory;

	private readonly MigrationBatchControlRowGroupFactory _migrationBatchControlRowGroupFactory;

	private ManualMigrationPanel _manualMigrationPanel;

	private bool _isTabVisible;

	public override string TabNameLocKey => "BatchControl.Migration";

	public override string TabImage => "Migration";

	public override string BindingKey => "MigrationTab";

	public override bool IgnoreDistrictSelection => true;

	public override bool MiddleRowVisible => false;

	protected override bool RemoveEmptyRowGroups => true;

	public MigrationBatchControlTab(VisualElementLoader visualElementLoader, BatchControlDistrict batchControlDistrict, DistrictCenterRegistry districtCenterRegistry, ILoc loc, ManualMigrationPanelFactory manualMigrationPanelFactory, MigrationBatchControlRowGroupFactory migrationBatchControlRowGroupFactory, EventBus eventBus)
		: base(visualElementLoader, batchControlDistrict, eventBus)
	{
		_districtCenterRegistry = districtCenterRegistry;
		_loc = loc;
		_manualMigrationPanelFactory = manualMigrationPanelFactory;
		_migrationBatchControlRowGroupFactory = migrationBatchControlRowGroupFactory;
	}

	[OnEvent]
	public void OnPopulationChangedEvent(PopulationChangedEvent populationChangedEvent)
	{
		if (_isTabVisible)
		{
			UpdateRowsVisibility();
		}
	}

	protected override VisualElement GetHeader()
	{
		if (_districtCenterRegistry.FinishedDistrictCenters.Count > 0)
		{
			_manualMigrationPanel = _manualMigrationPanelFactory.Create();
			return _manualMigrationPanel.Root;
		}
		return null;
	}

	protected override string GetRowsLabel()
	{
		return _loc.T(AutomaticMigrationLocKey);
	}

	protected override IEnumerable<BatchControlRowGroup> GetRowGroups(IEnumerable<EntityComponent> entities)
	{
		foreach (DistrictCenter finishedDistrictCenter in _districtCenterRegistry.FinishedDistrictCenters)
		{
			yield return _migrationBatchControlRowGroupFactory.Create(finishedDistrictCenter);
		}
	}

	protected override void Show()
	{
		_isTabVisible = true;
		_manualMigrationPanel?.Show();
	}

	protected override void Update()
	{
		_manualMigrationPanel?.Update();
	}

	protected override void Hide()
	{
		_manualMigrationPanel?.Hide();
		_isTabVisible = false;
	}
}
