using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Common;
using Timberborn.DropdownSystem;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using UnityEngine;

namespace Timberborn.GameDistrictsMigrationBatchControl;

public class ManualMigrationDistrictDropdownProvider : IExtendedDropdownProvider, IDropdownProvider
{
	private static readonly ImmutableArray<string> DropdownItemClasses = ImmutableArray.Create("dropdown-item--large");

	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private readonly Action<DistrictCenter> _districtChangedAction;

	private readonly List<string> _districtKeys = new List<string>();

	private DistrictCenter _selectedDistrict;

	public IReadOnlyList<string> Items => _districtKeys.AsReadOnlyList();

	public ManualMigrationDistrictDropdownProvider(DistrictCenterRegistry districtCenterRegistry, Action<DistrictCenter> districtChangedAction)
	{
		_districtCenterRegistry = districtCenterRegistry;
		_districtChangedAction = districtChangedAction;
	}

	public void SetDistrict(DistrictCenter selectedDistrict)
	{
		_selectedDistrict = selectedDistrict;
	}

	public void UpdateDistrictsList()
	{
		_districtKeys.Clear();
		for (int i = 0; i < _districtCenterRegistry.FinishedDistrictCenters.Count; i++)
		{
			_districtKeys.Add(i.ToString());
		}
	}

	public string GetValue()
	{
		return _districtCenterRegistry.FinishedDistrictCenters.IndexOf(_selectedDistrict).ToString();
	}

	public void SetValue(string value)
	{
		SelectDistrict(GetDistrict(value));
	}

	public string FormatDisplayText(string value, bool selected)
	{
		return GetDistrict(value).DistrictName;
	}

	public Sprite GetIcon(string value)
	{
		return GetDistrict(value).GetComponent<LabeledEntity>().Image;
	}

	public ImmutableArray<string> GetItemClasses(string value)
	{
		return DropdownItemClasses;
	}

	private DistrictCenter GetDistrict(string value)
	{
		int index = int.Parse(value);
		return _districtCenterRegistry.FinishedDistrictCenters[index];
	}

	private void SelectDistrict(DistrictCenter districtCenter)
	{
		_districtChangedAction(districtCenter);
	}
}
