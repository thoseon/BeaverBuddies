using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.GameDistricts;
using Timberborn.Navigation;
using Timberborn.PathSystem;
using UnityEngine;

namespace Timberborn.BuildingsNavigation;

public class PathDistrictRetriever : BaseComponent, IAwakableComponent
{
	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private PathSpec _pathSpec;

	private BlockObject _blockObject;

	private Vector3 PathCoordinates => CoordinateSystem.GridToWorld(_blockObject.TransformCoordinates(_pathSpec.MainPathCoordinates));

	public PathDistrictRetriever(DistrictCenterRegistry districtCenterRegistry)
	{
		_districtCenterRegistry = districtCenterRegistry;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_pathSpec = GetComponent<PathSpec>();
	}

	public DistrictCenter GetFinishedDistrictCenter()
	{
		foreach (DistrictCenter finishedDistrictCenter in _districtCenterRegistry.FinishedDistrictCenters)
		{
			if (finishedDistrictCenter.IsOnInstantDistrictRoad(PathCoordinates))
			{
				return finishedDistrictCenter;
			}
		}
		return null;
	}

	public DistrictCenter GetAnyDistrictCenter()
	{
		foreach (DistrictCenter allDistrictCenter in _districtCenterRegistry.AllDistrictCenters)
		{
			if (allDistrictCenter.IsOnPreviewDistrictRoad(PathCoordinates) || allDistrictCenter.IsOnInstantDistrictRoad(PathCoordinates))
			{
				return allDistrictCenter;
			}
		}
		return null;
	}

	public bool TryGetDistanceToDistrictCenter(out DistrictCenter districtCenter, out float distance)
	{
		DistrictCenter finishedDistrictCenter = GetFinishedDistrictCenter();
		if (finishedDistrictCenter != null)
		{
			districtCenter = finishedDistrictCenter;
			Accessible enabledComponent = districtCenter.GetEnabledComponent<Accessible>();
			if (!enabledComponent.FindRoadPath(PathCoordinates, out distance))
			{
				return enabledComponent.FindInstantRoadPath(PathCoordinates, out distance);
			}
			return true;
		}
		districtCenter = null;
		distance = 0f;
		return false;
	}
}
