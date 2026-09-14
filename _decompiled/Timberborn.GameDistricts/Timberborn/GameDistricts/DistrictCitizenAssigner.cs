using System.Collections.Generic;
using Timberborn.Characters;
using Timberborn.Navigation;
using Timberborn.TickSystem;

namespace Timberborn.GameDistricts;

internal class DistrictCitizenAssigner : ITickableSingleton, ISingletonNavMeshListener
{
	private readonly CharacterPopulation _characterPopulation;

	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private readonly UnassignedCitizenRegistry _unassignedCitizenRegistry;

	private readonly List<Citizen> _unassignedCitizens = new List<Citizen>();

	private bool _unassignCutOffCitizens;

	public DistrictCitizenAssigner(CharacterPopulation characterPopulation, DistrictCenterRegistry districtCenterRegistry, UnassignedCitizenRegistry unassignedCitizenRegistry)
	{
		_characterPopulation = characterPopulation;
		_districtCenterRegistry = districtCenterRegistry;
		_unassignedCitizenRegistry = unassignedCitizenRegistry;
	}

	public void Tick()
	{
		if (_unassignCutOffCitizens)
		{
			UnassignCharactersCutOffFromTheirDistricts();
			_unassignCutOffCitizens = false;
		}
		AssignCharactersWithoutDistricts();
	}

	public void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		_unassignCutOffCitizens = true;
	}

	private void AssignCharactersWithoutDistricts()
	{
		_unassignedCitizenRegistry.GetUnassignedCitizens(_unassignedCitizens);
		foreach (Citizen unassignedCitizen in _unassignedCitizens)
		{
			AssignToClosestDistrict(unassignedCitizen);
		}
		_unassignedCitizens.Clear();
	}

	private void AssignToClosestDistrict(Citizen citizen)
	{
		DistrictCenter districtCenter = null;
		float num = float.PositiveInfinity;
		foreach (DistrictCenter finishedDistrictCenter in _districtCenterRegistry.FinishedDistrictCenters)
		{
			if (finishedDistrictCenter.IsGloballyReachableFromCitizen(citizen))
			{
				float num2 = finishedDistrictCenter.DistanceToCitizen(citizen);
				if (num2 < num)
				{
					districtCenter = finishedDistrictCenter;
					num = num2;
				}
			}
		}
		if ((bool)districtCenter)
		{
			citizen.AssignDistrict(districtCenter);
		}
	}

	private void UnassignCharactersCutOffFromTheirDistricts()
	{
		foreach (Character character in _characterPopulation.Characters)
		{
			character.GetComponent<Citizen>().UnassignDistrictIfCutOff();
		}
	}
}
