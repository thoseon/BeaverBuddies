using Timberborn.Characters;
using Timberborn.PopulationStatisticsSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.BeaverContaminationSystem;

public class GlobalBeaverContaminationStatisticsProvider : IContaminationStatisticsProvider, ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly BeaverContaminationRegistry _beaverContaminationRegistry = new BeaverContaminationRegistry();

	public GlobalBeaverContaminationStatisticsProvider(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public BeaverContaminationStatistics GetContaminationStatistics()
	{
		return new BeaverContaminationStatistics(_beaverContaminationRegistry.NumberOfContaminatedAdults, _beaverContaminationRegistry.NumberOfContaminatedChildren);
	}

	[OnEvent]
	public void OnCharacterCreated(CharacterCreatedEvent characterCreatedEvent)
	{
		Contaminable component = characterCreatedEvent.Character.GetComponent<Contaminable>();
		if (component != null)
		{
			_beaverContaminationRegistry.AddContaminable(component);
		}
	}

	[OnEvent]
	public void OnCharacterKilled(CharacterKilledEvent characterKilledEvent)
	{
		Contaminable component = characterKilledEvent.Character.GetComponent<Contaminable>();
		if (component != null)
		{
			_beaverContaminationRegistry.RemoveContaminable(component);
		}
	}
}
