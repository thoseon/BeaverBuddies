using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.SingletonSystem;

namespace Timberborn.Characters;

public class CharacterPopulation : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly List<Character> _characters = new List<Character>();

	public ReadOnlyList<Character> Characters => _characters.AsReadOnlyList();

	public int NumberOfCharacters => _characters.Count;

	public CharacterPopulation(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnCharacterCreated(CharacterCreatedEvent characterCreatedEvent)
	{
		_characters.Add(characterCreatedEvent.Character);
	}

	[OnEvent]
	public void OnCharacterKilled(CharacterKilledEvent characterKilledEvent)
	{
		_characters.Remove(characterKilledEvent.Character);
	}
}
