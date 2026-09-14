using System.Linq;
using Timberborn.AchievementSystem;
using Timberborn.Beavers;
using Timberborn.BlockSystem;
using Timberborn.Characters;
using Timberborn.DwellingSystem;
using Timberborn.EntitySystem;
using Timberborn.GameFactionSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Achievements;

internal class ReachPopulationWithoutDwellingsAchievement : Achievement, ILoadableSingleton, ISaveableSingleton
{
	private static readonly SingletonKey ReachPopulationWithoutDwellingsKey = new SingletonKey("ReachPopulationWithoutDwellings");

	private static readonly PropertyKey<bool> DwellingBuiltKey = new PropertyKey<bool>("DwellingBuilt");

	private static readonly int RequiredPopulation = 200;

	private readonly ISingletonLoader _singletonLoader;

	private readonly EntityComponentRegistry _entityComponentRegistry;

	private readonly FactionService _factionService;

	private readonly BeaverPopulation _beaverPopulation;

	private readonly EventBus _eventBus;

	private bool _dwellingBuilt;

	public override string Id => "REACH_POPULATION_WITHOUT_DWELLINGS";

	public ReachPopulationWithoutDwellingsAchievement(ISingletonLoader singletonLoader, EntityComponentRegistry entityComponentRegistry, FactionService factionService, BeaverPopulation beaverPopulation, EventBus eventBus)
	{
		_singletonLoader = singletonLoader;
		_entityComponentRegistry = entityComponentRegistry;
		_factionService = factionService;
		_beaverPopulation = beaverPopulation;
		_eventBus = eventBus;
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		if (enteredFinishedStateEvent.BlockObject.HasComponent<Dwelling>())
		{
			_dwellingBuilt = true;
			DisableInternal();
		}
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (_dwellingBuilt)
		{
			singletonSaver.GetSingleton(ReachPopulationWithoutDwellingsKey).Set(DwellingBuiltKey, _dwellingBuilt);
		}
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(ReachPopulationWithoutDwellingsKey, out var objectLoader))
		{
			_dwellingBuilt = objectLoader.Get(DwellingBuiltKey);
		}
	}

	[OnEvent]
	public void OnCharacterCreated(CharacterCreatedEvent characterCreatedEvent)
	{
		ValidatePopulation();
	}

	protected override void EnableInternal()
	{
		if (_factionService.Current.Id == AchievementHelper.IronTeeth && !_entityComponentRegistry.GetEnabled<Dwelling>().Any() && !_dwellingBuilt)
		{
			_eventBus.Register(this);
			ValidatePopulation();
		}
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
	}

	private void ValidatePopulation()
	{
		if (_beaverPopulation.NumberOfBeavers >= RequiredPopulation)
		{
			Unlock();
		}
	}
}
