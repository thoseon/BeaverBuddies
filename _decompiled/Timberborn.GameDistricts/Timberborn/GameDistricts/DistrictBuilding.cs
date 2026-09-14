using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Buildings;

namespace Timberborn.GameDistricts;

public class DistrictBuilding : BaseComponent, IAwakableComponent
{
	private BuildingAccessible _buildingAccessible;

	private DistrictCenter _districtCenter;

	public DistrictCenter District { get; private set; }

	public DistrictCenter InstantDistrict { get; private set; }

	public DistrictCenter ConstructionDistrict { get; private set; }

	public bool IsEnabledDistrictCenter
	{
		get
		{
			if ((bool)_districtCenter)
			{
				return _districtCenter.Enabled;
			}
			return false;
		}
	}

	public event EventHandler ReassignedDistrict;

	public event EventHandler ReassignedInstantDistrict;

	public event EventHandler ReassignedConstructionDistrict;

	public void Awake()
	{
		_buildingAccessible = GetComponent<BuildingAccessible>();
		_districtCenter = GetComponent<DistrictCenter>();
	}

	public void AssignDistrict(DistrictCenter district)
	{
		if (district != District)
		{
			District = district;
			ReassignedDistrict?.Invoke(this, EventArgs.Empty);
		}
	}

	public void UnassignDistrict()
	{
		AssignDistrict(null);
	}

	public void AssignInstantDistrict(DistrictCenter district)
	{
		if (district != InstantDistrict)
		{
			InstantDistrict = district;
			ReassignedInstantDistrict?.Invoke(this, EventArgs.Empty);
		}
	}

	public void UnassignInstantDistrict()
	{
		AssignInstantDistrict(null);
	}

	public void AssignConstructionDistrict(DistrictCenter district)
	{
		if (district != ConstructionDistrict)
		{
			ConstructionDistrict = district;
			ReassignedConstructionDistrict?.Invoke(this, EventArgs.Empty);
		}
	}

	public void UnassignConstructionDistrict()
	{
		AssignConstructionDistrict(null);
	}

	public bool ShouldBeAssignedToDistrict(DistrictCenter district)
	{
		if (!IsEnabledDistrictCenter || _districtCenter != district)
		{
			return district.AccessibleIsOnDistrictRoad(_buildingAccessible.Accessible);
		}
		return true;
	}

	public bool ShouldBeAssignedToInstantDistrict(DistrictCenter district)
	{
		if (!IsEnabledDistrictCenter || _districtCenter != district)
		{
			return district.AccessibleIsOnInstantDistrictRoad(_buildingAccessible.Accessible);
		}
		return true;
	}

	public bool ShouldBeAssignedToConstructionDistrict(DistrictCenter district)
	{
		return district.IsOnInstantDistrictRoad(_buildingAccessible.CalculateAccess());
	}

	public DistrictCenter GetInstantOrConstructionDistrict()
	{
		if (!InstantDistrict)
		{
			return ConstructionDistrict;
		}
		return InstantDistrict;
	}

	public DistrictCenter GetDistrictOrConstructionDistrict()
	{
		if (!District)
		{
			return ConstructionDistrict;
		}
		return District;
	}
}
