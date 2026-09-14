using System;

namespace Timberborn.Navigation;

internal readonly struct DistrictChange
{
	private readonly District _district;

	private readonly int _nodeId;

	private readonly DistrictChangeType _districtChangeType;

	private DistrictChange(District district, int nodeId, DistrictChangeType districtChangeType)
	{
		_district = district;
		_nodeId = nodeId;
		_districtChangeType = districtChangeType;
	}

	public static DistrictChange AddDistrict(District district)
	{
		return new DistrictChange(district, 0, DistrictChangeType.AddDistrict);
	}

	public static DistrictChange RemoveDistrict(District district)
	{
		return new DistrictChange(district, 0, DistrictChangeType.RemoveDistrict);
	}

	public static DistrictChange SetObstacle(int nodeId)
	{
		return new DistrictChange(null, nodeId, DistrictChangeType.SetObstacle);
	}

	public static DistrictChange UnsetObstacle(int nodeId)
	{
		return new DistrictChange(null, nodeId, DistrictChangeType.UnsetObstacle);
	}

	public void ApplyChange(DistrictMap districtMap, DistrictObstacleService districtObstacleService, NavMeshUpdate.Builder navMeshUpdateBuilder = null)
	{
		switch (_districtChangeType)
		{
		case DistrictChangeType.AddDistrict:
			districtMap.AddDistrictCenter(_district);
			navMeshUpdateBuilder?.AddRoadNode(_district.CenterNodeId);
			break;
		case DistrictChangeType.RemoveDistrict:
			districtMap.RemoveDistrictCenter(_district);
			navMeshUpdateBuilder?.AddRoadNode(_district.CenterNodeId);
			break;
		case DistrictChangeType.SetObstacle:
			districtObstacleService.SetObstacle(_nodeId);
			districtMap.OnObstacleChanged();
			navMeshUpdateBuilder?.AddRoadNode(_nodeId);
			break;
		case DistrictChangeType.UnsetObstacle:
			districtObstacleService.UnsetObstacle(_nodeId);
			districtMap.OnObstacleChanged();
			navMeshUpdateBuilder?.AddRoadNode(_nodeId);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}
}
