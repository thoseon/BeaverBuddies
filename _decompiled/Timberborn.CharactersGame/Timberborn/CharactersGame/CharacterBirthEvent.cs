namespace Timberborn.CharactersGame;

public class CharacterBirthEvent
{
	public CharacterBirth CharacterBirth { get; }

	public CharacterBirthEvent(CharacterBirth characterBirth)
	{
		CharacterBirth = characterBirth;
	}
}
