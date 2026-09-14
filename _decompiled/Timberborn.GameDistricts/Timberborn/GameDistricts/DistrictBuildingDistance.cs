using Timberborn.BaseComponentSystem;
using Timberborn.Buildings;
using Timberborn.EntitySystem;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.GameDistricts;

public class DistrictBuildingDistance : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private readonly NavigationDistance _navigationDistance;

	private readonly DistanceToDistrictDescriber _distanceToDistrictDescriber;

	private DistrictBuilding _districtBuilding;

	private Vector3 _access;

	public DistrictBuildingDistance(NavigationDistance navigationDistance, DistanceToDistrictDescriber distanceToDistrictDescriber)
	{
		_navigationDistance = navigationDistance;
		_distanceToDistrictDescriber = distanceToDistrictDescriber;
	}

	public void Awake()
	{
		_districtBuilding = GetComponent<DistrictBuilding>();
	}

	public void InitializeEntity()
	{
		_access = GetComponent<BuildingAccessible>().CalculateAccess();
	}

	public bool TryGetDistanceToDistrict(out int distance)
	{
		if (!_districtBuilding.IsEnabledDistrictCenter)
		{
			DistrictCenter instantOrConstructionDistrict = _districtBuilding.GetInstantOrConstructionDistrict();
			if (instantOrConstructionDistrict != null)
			{
				Accessible accessible = instantOrConstructionDistrict.GetComponent<BuildingAccessible>().Accessible;
				if (!accessible.FindRoadPath(_access, out var distance2))
				{
					accessible.FindInstantRoadPath(_access, out distance2);
				}
				distance = Mathf.RoundToInt(distance2);
				return true;
			}
		}
		distance = 0;
		return false;
	}

	public string DescribeDistance()
	{
		if (!TryGetDistanceToDistrict(out var distance))
		{
			return null;
		}
		return _distanceToDistrictDescriber.DescribeDistance(distance);
	}

	public bool IsAboveThreshold()
	{
		if (TryGetDistanceToDistrict(out var distance))
		{
			return (float)distance > _navigationDistance.LargeDistrictThreshold;
		}
		return false;
	}
}
