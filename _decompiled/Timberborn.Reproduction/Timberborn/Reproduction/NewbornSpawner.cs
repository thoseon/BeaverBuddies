using Timberborn.BaseComponentSystem;
using Timberborn.Beavers;
using Timberborn.Buildings;
using Timberborn.CharactersGame;
using Timberborn.DwellingSystem;
using Timberborn.GameDistricts;
using UnityEngine;

namespace Timberborn.Reproduction;

public class NewbornSpawner
{
	private readonly BeaverFactory _beaverFactory;

	public NewbornSpawner(BeaverFactory beaverFactory)
	{
		_beaverFactory = beaverFactory;
	}

	public void SpawnAdult(BaseComponent spawner)
	{
		Vector3? unblockedSingleAccess = spawner.GetComponent<BuildingAccessible>().Accessible.UnblockedSingleAccess;
		if (unblockedSingleAccess.HasValue)
		{
			Vector3 valueOrDefault = unblockedSingleAccess.GetValueOrDefault();
			_beaverFactory.CreateNewbornAdult(valueOrDefault, CreateInitComponent(spawner));
		}
	}

	public void SpawnChild(BaseComponent spawner)
	{
		Vector3? unblockedSingleAccess = spawner.GetComponent<BuildingAccessible>().Accessible.UnblockedSingleAccess;
		if (unblockedSingleAccess.HasValue)
		{
			Vector3 valueOrDefault = unblockedSingleAccess.GetValueOrDefault();
			_beaverFactory.CreateNewbornChild(valueOrDefault, CreateInitComponent(spawner));
		}
	}

	private static CharacterBirthInit CreateInitComponent(BaseComponent spawner)
	{
		return new CharacterBirthInit(spawner.GetComponent<DistrictBuilding>().District, spawner.GetComponent<Dwelling>(), new BeaverBornEvent());
	}
}
