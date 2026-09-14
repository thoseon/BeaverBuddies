using System;
using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.GameDistricts;
using Timberborn.GameDistrictsMigration;
using Timberborn.SelectionSystem;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsMigrationBatchControl;

internal class ManualMigrationDistrictColumnFactory
{
	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly ManualMigrationDistrictSetter _manualMigrationDistrictSetter;

	private readonly ManualMigrationPopulationRowFactory _manualMigrationPopulationRowFactory;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly VisualElementLoader _visualElementLoader;

	public ManualMigrationDistrictColumnFactory(DistrictCenterRegistry districtCenterRegistry, DropdownItemsSetter dropdownItemsSetter, ManualMigrationDistrictSetter manualMigrationDistrictSetter, ManualMigrationPopulationRowFactory manualMigrationPopulationRowFactory, EntitySelectionService entitySelectionService, VisualElementLoader visualElementLoader)
	{
		_districtCenterRegistry = districtCenterRegistry;
		_dropdownItemsSetter = dropdownItemsSetter;
		_manualMigrationDistrictSetter = manualMigrationDistrictSetter;
		_manualMigrationPopulationRowFactory = manualMigrationPopulationRowFactory;
		_entitySelectionService = entitySelectionService;
		_visualElementLoader = visualElementLoader;
	}

	public ManualMigrationDistrictColumn CreateLeftColumn(VisualElement parent)
	{
		return BindUIAndCreateColumn(parent, _manualMigrationPopulationRowFactory.CreateLeftRows(), _manualMigrationDistrictSetter.SetLeftDistrict);
	}

	public ManualMigrationDistrictColumn CreateRightColumn(VisualElement parent)
	{
		return BindUIAndCreateColumn(parent, _manualMigrationPopulationRowFactory.CreateRightRows(), _manualMigrationDistrictSetter.SetRightDistrict);
	}

	private ManualMigrationDistrictColumn BindUIAndCreateColumn(VisualElement parent, IReadOnlyList<ManualMigrationPopulationRow> rows, Action<DistrictCenter> districtChangedAction)
	{
		string elementName = "Game/BatchControl/ManualMigrationHeaderRow";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		parent.Q<VisualElement>("HeaderContent").Add(visualElement);
		VisualElement visualElement2 = parent.Q<VisualElement>("RowsContent");
		foreach (ManualMigrationPopulationRow row in rows)
		{
			visualElement2.Add(row.Root);
		}
		return CreateManualMigrationDistrictColumn(visualElement, parent, rows, districtChangedAction);
	}

	private ManualMigrationDistrictColumn CreateManualMigrationDistrictColumn(VisualElement header, VisualElement parent, IReadOnlyList<ManualMigrationPopulationRow> rows, Action<DistrictCenter> districtChangedAction)
	{
		ManualMigrationDistrictDropdownProvider manualMigrationDistrictDropdownProvider = new ManualMigrationDistrictDropdownProvider(_districtCenterRegistry, districtChangedAction);
		Dropdown dropdown = header.Q<Dropdown>("DistrictDropdown");
		ManualMigrationDistrictDropdown manualMigrationDistrictDropdown = new ManualMigrationDistrictDropdown(_dropdownItemsSetter, manualMigrationDistrictDropdownProvider, dropdown);
		Image icon = header.Q<Image>("DistrictIcon");
		ManualMigrationDistrictColumn manualMigrationDistrictColumn = new ManualMigrationDistrictColumn(manualMigrationDistrictDropdown, rows, icon, parent);
		header.Q<Button>("Select").RegisterCallback<ClickEvent>(delegate
		{
			_entitySelectionService.SelectAndFocusOn(manualMigrationDistrictColumn.DistrictCenter);
		});
		return manualMigrationDistrictColumn;
	}
}
