using Timberborn.CharacterModelSystem;
using Timberborn.Characters;
using Timberborn.Debugging;

namespace Timberborn.CharactersUI;

public class CharactersModelToggler : IDevModule
{
	private readonly CharacterPopulation _characterPopulation;

	private bool _charactersHidden;

	public CharactersModelToggler(CharacterPopulation characterPopulation)
	{
		_characterPopulation = characterPopulation;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Toggle models: Characters", ToggleCharactersModels)).Build();
	}

	private void ToggleCharactersModels()
	{
		_charactersHidden = !_charactersHidden;
		foreach (Character character in _characterPopulation.Characters)
		{
			ToggleCharacterModels(character);
		}
	}

	private void ToggleCharacterModels(Character character)
	{
		CharacterModel component = character.GetComponent<CharacterModel>();
		if (_charactersHidden)
		{
			component.Hide();
		}
		else
		{
			component.Show();
		}
	}
}
