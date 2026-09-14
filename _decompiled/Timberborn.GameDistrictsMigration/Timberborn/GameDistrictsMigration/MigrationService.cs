using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BeaverContaminationSystem;
using Timberborn.Beavers;
using Timberborn.Characters;
using Timberborn.DwellingSystem;
using Timberborn.GameDistricts;
using Timberborn.WorkSystem;

namespace Timberborn.GameDistrictsMigration;

public class MigrationService
{
	private readonly List<BaseComponent> _charactersToMove = new List<BaseComponent>();

	public bool RefusesWork(BaseComponent component)
	{
		return component.GetComponent<WorkRefuser>().RefusesWork;
	}

	public bool IsEmployed(BaseComponent component)
	{
		return component.GetComponent<Worker>().Employed;
	}

	public bool IsNotContaminated(BaseComponent component)
	{
		return !IsContaminated(component);
	}

	public bool IsContaminated(BaseComponent component)
	{
		return component.GetComponent<Contaminable>().IsContaminated;
	}

	public bool HasHome(Beaver beaver)
	{
		return beaver.GetComponent<Dweller>().HasHome;
	}

	public int GetDayOfBirth(BaseComponent component)
	{
		return component.GetComponent<Character>().DayOfBirth;
	}

	public void Migrate(IEnumerable<BaseComponent> charactersToMove, DistrictCenter targetDistrict, int numberOfCharactersToMove)
	{
		_charactersToMove.AddRange(charactersToMove.Take(numberOfCharactersToMove));
		if (_charactersToMove.Count < numberOfCharactersToMove)
		{
			_charactersToMove.Clear();
			throw new InvalidOperationException("Couldn't move enough beavers to the target district.");
		}
		foreach (BaseComponent item in _charactersToMove)
		{
			item.GetComponent<Citizen>().AssignDistrict(targetDistrict);
		}
		_charactersToMove.Clear();
	}
}
