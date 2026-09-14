using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.CharacterModelSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.GameDistricts;

internal class CitizenUnstucker
{
	private readonly IBlockService _blockService;

	private readonly IDistrictService _districtService;

	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private readonly List<DistrictCenter> _districts = new List<DistrictCenter>();

	public CitizenUnstucker(IBlockService blockService, IDistrictService districtService, DistrictCenterRegistry districtCenterRegistry)
	{
		_blockService = blockService;
		_districtService = districtService;
		_districtCenterRegistry = districtCenterRegistry;
	}

	public bool TryUnstuckAndKeepDistrict(Citizen citizen, DistrictCenter preferredDistrict)
	{
		Asserts.IsTrue(this, preferredDistrict, "preferredDistrict");
		if (IsStuckInsideFinishedBuilding(citizen))
		{
			CollectDistricts(preferredDistrict);
			if (_districts.Any())
			{
				bool num = TryFindUnstuckPosition(citizen, out var unstuckPosition);
				if (num)
				{
					MoveCitizen(citizen, unstuckPosition);
				}
				_districts.Clear();
				return num;
			}
		}
		return false;
	}

	private bool IsStuckInsideFinishedBuilding(Citizen citizen)
	{
		Vector3Int coordinates = NavigationCoordinateSystem.WorldToGridInt(citizen.Transform.position);
		return _blockService.GetMiddleObjectComponentAt<Building>(coordinates)?.GetComponent<BlockObject>().IsFinished ?? false;
	}

	private void CollectDistricts(DistrictCenter preferredDistrict)
	{
		_districts.Add(preferredDistrict);
		foreach (DistrictCenter finishedDistrictCenter in _districtCenterRegistry.FinishedDistrictCenters)
		{
			if (finishedDistrictCenter != preferredDistrict)
			{
				_districts.Add(finishedDistrictCenter);
			}
		}
	}

	private bool TryFindUnstuckPosition(Citizen citizen, out Vector3 unstuckPosition)
	{
		if (TryFindReachablePosition(citizen, out unstuckPosition))
		{
			return true;
		}
		unstuckPosition = Vector3.zero;
		return false;
	}

	private bool TryFindReachablePosition(Citizen citizen, out Vector3 reachablePosition)
	{
		Vector3 position = citizen.Transform.position;
		foreach (DistrictCenter district in _districts)
		{
			Vector3Int[] neighbors26Vector3Int = Deltas.Neighbors26Vector3Int;
			foreach (Vector3Int vector3Int in neighbors26Vector3Int)
			{
				Vector3 vector = position + vector3Int;
				if (_districtService.DistrictIsGloballyReachable(district.District, vector))
				{
					reachablePosition = vector;
					return true;
				}
			}
		}
		reachablePosition = Vector3.zero;
		return false;
	}

	private static void MoveCitizen(Citizen citizen, Vector3 position)
	{
		citizen.Transform.position = position;
		citizen.GetComponent<CharacterModel>().Position = position;
	}
}
