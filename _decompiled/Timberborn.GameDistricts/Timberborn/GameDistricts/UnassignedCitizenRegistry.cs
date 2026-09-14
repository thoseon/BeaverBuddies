using System.Collections.Generic;

namespace Timberborn.GameDistricts;

public class UnassignedCitizenRegistry
{
	private readonly HashSet<Citizen> _unassignedCitizens = new HashSet<Citizen>();

	public void Add(Citizen citizen)
	{
		_unassignedCitizens.Add(citizen);
	}

	public void Remove(Citizen citizen)
	{
		_unassignedCitizens.Remove(citizen);
	}

	public void GetUnassignedCitizens(List<Citizen> unassignedCitizens)
	{
		foreach (Citizen unassignedCitizen in _unassignedCitizens)
		{
			unassignedCitizens.Add(unassignedCitizen);
		}
	}
}
