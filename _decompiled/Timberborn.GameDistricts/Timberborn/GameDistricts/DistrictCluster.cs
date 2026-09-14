using System.Collections.Generic;
using System.Linq;

namespace Timberborn.GameDistricts;

internal class DistrictCluster
{
	private readonly HashSet<DistrictCenter> _districtCenters;

	public DistrictCluster(DistrictCenter districtCenter)
	{
		_districtCenters = new HashSet<DistrictCenter> { districtCenter };
	}

	public bool TryAddDistrict(DistrictCenter districtCenter)
	{
		if (_districtCenters.First().IsGloballyReachableFromAnotherDistrict(districtCenter))
		{
			_districtCenters.Add(districtCenter);
			return true;
		}
		return false;
	}

	public bool Contains(DistrictCenter districtCenter)
	{
		return _districtCenters.Contains(districtCenter);
	}

	public IEnumerable<DistrictCenter> GetDistrictsOtherThan(DistrictCenter districtCenter)
	{
		foreach (DistrictCenter districtCenter2 in _districtCenters)
		{
			if ((bool)districtCenter2 && districtCenter != districtCenter2)
			{
				yield return districtCenter2;
			}
		}
	}
}
