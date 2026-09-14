using System.Collections.Generic;
using Timberborn.Characters;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Bots;

public class BotPopulation : ILoadableSingleton, ISaveableSingleton
{
	private static readonly SingletonKey BotPopulationKey = new SingletonKey("BotPopulation");

	private static readonly PropertyKey<bool> BotCreatedKey = new PropertyKey<bool>("BotCreated");

	private readonly EventBus _eventBus;

	private readonly ISingletonLoader _singletonLoader;

	private readonly List<BotSpec> _bots = new List<BotSpec>();

	public bool BotCreated { get; private set; }

	public int NumberOfBots => _bots.Count;

	public BotPopulation(EventBus eventBus, ISingletonLoader singletonLoader)
	{
		_eventBus = eventBus;
		_singletonLoader = singletonLoader;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		singletonSaver.GetSingleton(BotPopulationKey).Set(BotCreatedKey, BotCreated);
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(BotPopulationKey, out var objectLoader))
		{
			BotCreated = objectLoader.Get(BotCreatedKey);
		}
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnCharacterCreated(CharacterCreatedEvent characterCreatedEvent)
	{
		BotSpec component = characterCreatedEvent.Character.GetComponent<BotSpec>();
		if ((object)component != null)
		{
			_bots.Add(component);
			BotCreated = true;
		}
	}

	[OnEvent]
	public void OnCharacterKilled(CharacterKilledEvent characterKilledEvent)
	{
		BotSpec component = characterKilledEvent.Character.GetComponent<BotSpec>();
		if ((object)component != null)
		{
			_bots.Remove(component);
		}
	}
}
