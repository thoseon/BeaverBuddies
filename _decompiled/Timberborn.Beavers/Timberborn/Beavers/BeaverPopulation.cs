using Timberborn.Characters;
using Timberborn.SingletonSystem;

namespace Timberborn.Beavers;

public class BeaverPopulation : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly BeaverCollection _beaverCollection = new BeaverCollection();

	public int NumberOfAdults => _beaverCollection.NumberOfAdults;

	public int NumberOfChildren => _beaverCollection.NumberOfChildren;

	public int NumberOfBeavers => _beaverCollection.NumberOfBeavers;

	public BeaverPopulation(EventBus eventBus)
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
		Beaver component = characterCreatedEvent.Character.GetComponent<Beaver>();
		if (component != null)
		{
			_beaverCollection.AddBeaver(component);
		}
	}

	[OnEvent]
	public void OnCharacterKilled(CharacterKilledEvent characterKilledEvent)
	{
		Beaver component = characterKilledEvent.Character.GetComponent<Beaver>();
		if (component != null)
		{
			_beaverCollection.RemoveBeaver(component);
		}
	}
}
