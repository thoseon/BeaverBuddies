using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.DwellingSystem;
using Timberborn.EnterableSystem;
using Timberborn.GameDistricts;
using Timberborn.Navigation;
using Timberborn.TerrainSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.WalkingSystem;

public class RandomDestinationPicker
{
	private static readonly float TerrainDestinationOffset = 0.4f;

	private static readonly float NonTerrainDestinationOffset = 0.1f;

	private readonly IDistrictService _districtService;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private readonly ITerrainService _terrainService;

	public RandomDestinationPicker(IDistrictService districtService, IRandomNumberGenerator randomNumberGenerator, IThreadSafeWaterMap threadSafeWaterMap, ITerrainService terrainService)
	{
		_districtService = districtService;
		_randomNumberGenerator = randomNumberGenerator;
		_threadSafeWaterMap = threadSafeWaterMap;
		_terrainService = terrainService;
	}

	public Vector3 RandomDestination(Citizen citizen)
	{
		DistrictCenter assignedDistrict = citizen.AssignedDistrict;
		Vector3 randomDestination = (((bool)assignedDistrict && assignedDistrict.District != null) ? RandomDestination(citizen, assignedDistrict.District) : citizen.Transform.position);
		return OffsetDestination(randomDestination);
	}

	public bool TryGetSafeRandomDestination(Citizen citizen, out Vector3 destination)
	{
		destination = RandomDestination(citizen);
		Vector3Int coordinates = NavigationCoordinateSystem.WorldToGridInt(destination);
		if (_threadSafeWaterMap.ColumnContamination(coordinates) == 0f)
		{
			return true;
		}
		Enterer component = citizen.GetComponent<Enterer>();
		if (component.IsInside)
		{
			Vector3Int coordinates2 = component.CurrentBuilding.GetComponent<BlockObject>().PositionedEntrance.Coordinates;
			destination = OffsetDestination(CoordinateSystem.GridToWorldCentered(coordinates2));
			return true;
		}
		return false;
	}

	private Vector3 RandomDestination(Citizen citizen, District district)
	{
		return _districtService.GetRandomDestinationInDistrict(district, GetCoordinates(citizen));
	}

	private static Vector3 GetCoordinates(Citizen citizen)
	{
		Dweller component = citizen.GetComponent<Dweller>();
		if ((bool)component && component.HasHome)
		{
			Vector3? homeAccess = component.HomeAccess;
			if (homeAccess.HasValue)
			{
				Vector3 valueOrDefault = homeAccess.GetValueOrDefault();
				if (component.Home.GetComponent<DistrictBuilding>().District != null)
				{
					return valueOrDefault;
				}
			}
		}
		return CoordinateSystem.GridToWorldCentered(citizen.AssignedDistrict.CenterCoordinates);
	}

	private Vector3 OffsetDestination(Vector3 randomDestination)
	{
		Vector3Int coords = NavigationCoordinateSystem.WorldToGridInt(randomDestination);
		float num = (_terrainService.OnGround(coords) ? TerrainDestinationOffset : NonTerrainDestinationOffset);
		Vector3 vector = CoordinateSystem.GridToWorld(_randomNumberGenerator.InsideUnitCircle() * num);
		return randomDestination + vector;
	}
}
