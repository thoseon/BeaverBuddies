using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.Common;
using Timberborn.Navigation;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.Explosions;

public class CharacterExploder
{
	private readonly CharacterPopulation _characterPopulation;

	private readonly ITerrainService _terrainService;

	public CharacterExploder(CharacterPopulation characterPopulation, ITerrainService terrainService)
	{
		_characterPopulation = characterPopulation;
		_terrainService = terrainService;
	}

	public void ExplodeCharactersAt(Vector3Int position, BaseComponent source)
	{
		for (int i = 0; i < _characterPopulation.NumberOfCharacters; i++)
		{
			Character character = _characterPopulation.Characters[i];
			Vector3Int characterWorldCoordinates = GetCharacterWorldCoordinates(character);
			if (position == characterWorldCoordinates)
			{
				KillCharacter(character, characterWorldCoordinates, source);
			}
		}
	}

	public void ExplodeCharactersAt(ReadOnlyHashSet<Vector3Int> positions, BaseComponent source)
	{
		for (int num = _characterPopulation.NumberOfCharacters - 1; num >= 0; num--)
		{
			Character character = _characterPopulation.Characters[num];
			Vector3Int characterWorldCoordinates = GetCharacterWorldCoordinates(character);
			if (positions.Contains(characterWorldCoordinates))
			{
				KillCharacter(character, characterWorldCoordinates, source);
			}
		}
	}

	private static Vector3Int GetCharacterWorldCoordinates(Character character)
	{
		return NavigationCoordinateSystem.WorldToGridInt(character.Transform.position);
	}

	private void KillCharacter(Character character, Vector3Int coordinates, BaseComponent source)
	{
		character.GetComponent<ExplosionVulnerable>()?.DieFromExplosion(source);
		Vector3 position = character.Transform.position;
		int terrainHeightBelow = _terrainService.GetTerrainHeightBelow(coordinates);
		character.Transform.position = new Vector3(position.x, terrainHeightBelow, position.z);
	}
}
