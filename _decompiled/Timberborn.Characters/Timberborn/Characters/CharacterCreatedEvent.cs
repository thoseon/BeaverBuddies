namespace Timberborn.Characters;

public class CharacterCreatedEvent
{
	public Character Character { get; }

	public CharacterCreatedEvent(Character character)
	{
		Character = character;
	}
}
