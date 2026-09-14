using System.Collections.Generic;
using System.Linq;
using Timberborn.Beavers;
using Timberborn.GameDistricts;
using Timberborn.TickSystem;

namespace Timberborn.DwellingSystem;

internal class DwellerHomeAssigner : ITickableSingleton
{
	private readonly StaleAssignableDwellingService _staleAssignableDwellingService;

	public DwellerHomeAssigner(StaleAssignableDwellingService staleAssignableDwellingService)
	{
		_staleAssignableDwellingService = staleAssignableDwellingService;
	}

	public void Tick()
	{
		AssignToDwelling();
	}

	private void AssignToDwelling()
	{
		AutoAssignableDwelling stalest = _staleAssignableDwellingService.GetStalest();
		if ((bool)stalest && AddDweller(stalest) && stalest.HasAssignableSlot)
		{
			_staleAssignableDwellingService.SetAsStalest(stalest);
		}
	}

	private static bool AddDweller(AutoAssignableDwelling dwelling)
	{
		DistrictCenter district = dwelling.GetComponent<DistrictBuilding>().District;
		if ((bool)district)
		{
			DistrictPopulation districtPopulation = district.DistrictPopulation;
			if (!dwelling.ShouldAssignAdult)
			{
				return AssignDweller(dwelling, districtPopulation.Children, districtPopulation.Adults);
			}
			return AssignDweller(dwelling, districtPopulation.Adults, districtPopulation.Children);
		}
		return false;
	}

	private static bool AssignDweller(AutoAssignableDwelling dwelling, IEnumerable<Beaver> primaryBeavers, IEnumerable<Beaver> secondaryBeavers)
	{
		foreach (Beaver item in primaryBeavers.Concat(secondaryBeavers))
		{
			Dweller component = item.GetComponent<Dweller>();
			if (component.IsLookingForBetterHome() && dwelling.CanAssignDweller(component))
			{
				dwelling.AssignDweller(component);
				return true;
			}
		}
		return false;
	}
}
