namespace Timberborn.Characters;

public class CharacterKilledEvent
{
	public Character Character { get; }

	public CharacterKilledEvent(Character character)
	{
		Character = character;
	}
}
