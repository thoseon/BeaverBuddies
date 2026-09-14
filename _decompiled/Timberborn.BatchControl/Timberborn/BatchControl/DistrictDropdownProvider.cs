using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.AssetSystem;
using Timberborn.Common;
using Timberborn.DropdownSystem;
using Timberborn.GameDistricts;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.BatchControl;

internal class DistrictDropdownProvider : IExtendedDropdownProvider, IDropdownProvider, ILoadableSingleton
{
	private static readonly string GlobalViewLocKey = "Districts.GlobalView";

	private static readonly string GlobalViewKey = "Global";

	private static readonly ImmutableArray<string> DropdownItemClasses = ImmutableArray.Create("dropdown-item--medium");

	private readonly BatchControlDistrict _batchControlDistrict;

	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private readonly ILoc _loc;

	private readonly IAssetLoader _assetLoader;

	private readonly List<string> _districtKeys = new List<string>();

	private Sprite _districtIcon;

	private Sprite _globalIcon;

	public IReadOnlyList<string> Items => _districtKeys.AsReadOnlyList();

	public DistrictDropdownProvider(BatchControlDistrict batchControlDistrict, DistrictCenterRegistry districtCenterRegistry, ILoc loc, IAssetLoader assetLoader)
	{
		_batchControlDistrict = batchControlDistrict;
		_districtCenterRegistry = districtCenterRegistry;
		_loc = loc;
		_assetLoader = assetLoader;
	}

	public void Load()
	{
		_districtIcon = _assetLoader.Load<Sprite>("UI/Images/Game/ico-district");
		_globalIcon = _assetLoader.Load<Sprite>("UI/Images/Game/ico-global");
	}

	public void UpdateDistrictsList()
	{
		_districtKeys.Clear();
		_districtKeys.Add(GlobalViewKey);
		for (int i = 0; i < _districtCenterRegistry.FinishedDistrictCenters.Count; i++)
		{
			_districtKeys.Add(i.ToString());
		}
	}

	public string GetValue()
	{
		DistrictCenter selectedDistrict = _batchControlDistrict.SelectedDistrict;
		if ((bool)selectedDistrict)
		{
			return _districtCenterRegistry.FinishedDistrictCenters.IndexOf(selectedDistrict).ToString();
		}
		return GlobalViewKey;
	}

	public void SetValue(string value)
	{
		if (value == GlobalViewKey)
		{
			SelectDistrict(null);
		}
		else
		{
			SelectDistrict(GetDistrict(value));
		}
	}

	public string FormatDisplayText(string value, bool selected)
	{
		if (value == GlobalViewKey)
		{
			return _loc.T(GlobalViewLocKey);
		}
		return GetDistrict(value).DistrictName;
	}

	public Sprite GetIcon(string value)
	{
		if (!(value == GlobalViewKey))
		{
			return _districtIcon;
		}
		return _globalIcon;
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
		_batchControlDistrict.SetDistrict(districtCenter);
	}
}
