using System;
using Timberborn.DropdownSystem;
using Timberborn.GameDistricts;

namespace Timberborn.GameDistrictsMigrationBatchControl;

internal class ManualMigrationDistrictDropdown
{
	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly ManualMigrationDistrictDropdownProvider _manualMigrationDistrictDropdownProvider;

	private readonly Dropdown _dropdown;

	private readonly Action<DistrictCenter> _districtChangedAction;

	public ManualMigrationDistrictDropdown(DropdownItemsSetter dropdownItemsSetter, ManualMigrationDistrictDropdownProvider manualMigrationDistrictDropdownProvider, Dropdown dropdown)
	{
		_dropdownItemsSetter = dropdownItemsSetter;
		_manualMigrationDistrictDropdownProvider = manualMigrationDistrictDropdownProvider;
		_dropdown = dropdown;
	}

	public void SetDistrict(DistrictCenter selectedDistrict)
	{
		_manualMigrationDistrictDropdownProvider.SetDistrict(selectedDistrict);
		UpdateDistricts();
	}

	private void UpdateDistricts()
	{
		_manualMigrationDistrictDropdownProvider.UpdateDistrictsList();
		_dropdownItemsSetter.SetItems(_dropdown, _manualMigrationDistrictDropdownProvider);
	}
}
