using System;
using System.Collections.Generic;
using Timberborn.AchievementSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.GameFactionSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using Timberborn.Workshops;
using Timberborn.WorldPersistence;

namespace Timberborn.Achievements;

internal class ProducePlanksInDayAchievement : Achievement, ILoadableSingleton, ISaveableSingleton
{
	private static readonly SingletonKey ProducePlanksInDayKey = new SingletonKey("ProducePlanksInDay");

	private static readonly PropertyKey<int> PlanksProducedKey = new PropertyKey<int>("PlanksProduced");

	private static readonly int PlanksToProducePerDay = 500;

	private static readonly string RecipeId = "Plank";

	private readonly EventBus _eventBus;

	private readonly ISingletonLoader _singletonLoader;

	private readonly FactionService _factionService;

	private readonly EntityComponentRegistry _entityComponentRegistry;

	private readonly HashSet<Manufactory> _manufactories = new HashSet<Manufactory>();

	private int _planksProduced;

	public override string Id => "PRODUCE_PLANKS_IN_DAY";

	public ProducePlanksInDayAchievement(EventBus eventBus, ISingletonLoader singletonLoader, FactionService factionService, EntityComponentRegistry entityComponentRegistry)
	{
		_eventBus = eventBus;
		_singletonLoader = singletonLoader;
		_factionService = factionService;
		_entityComponentRegistry = entityComponentRegistry;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (_planksProduced > 0 && _planksProduced < PlanksToProducePerDay)
		{
			singletonSaver.GetSingleton(ProducePlanksInDayKey).Set(PlanksProducedKey, _planksProduced);
		}
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(ProducePlanksInDayKey, out var objectLoader))
		{
			_planksProduced = objectLoader.Get(PlanksProducedKey);
		}
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		Manufactory component = enteredFinishedStateEvent.BlockObject.GetComponent<Manufactory>();
		if (component != null)
		{
			AddManufactory(component);
		}
	}

	[OnEvent]
	public void OnExitedFinishedState(ExitedFinishedStateEvent exitedFinishedStateEvent)
	{
		Manufactory component = exitedFinishedStateEvent.BlockObject.GetComponent<Manufactory>();
		if (component != null)
		{
			RemoveManufactory(component);
		}
	}

	[OnEvent]
	public void OnDaytimeStartEvent(DaytimeStartEvent daytimeStartEvent)
	{
		_planksProduced = 0;
	}

	protected override void EnableInternal()
	{
		if (!(_factionService.Current.Id == AchievementHelper.IronTeeth))
		{
			return;
		}
		_eventBus.Register(this);
		foreach (Manufactory item in _entityComponentRegistry.GetEnabled<Manufactory>())
		{
			AddManufactory(item);
		}
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
		foreach (Manufactory manufactory in _manufactories)
		{
			manufactory.ProductionFinished -= OnProductionFinished;
		}
	}

	private void AddManufactory(Manufactory manufactory)
	{
		if (_manufactories.Add(manufactory))
		{
			manufactory.ProductionFinished += OnProductionFinished;
		}
	}

	private void RemoveManufactory(Manufactory manufactory)
	{
		_manufactories.Remove(manufactory);
		manufactory.ProductionFinished -= OnProductionFinished;
	}

	private void OnProductionFinished(object sender, EventArgs e)
	{
		Manufactory manufactory = (Manufactory)sender;
		if (manufactory.HasCurrentRecipe && manufactory.CurrentRecipe.Id == RecipeId)
		{
			_planksProduced++;
		}
		if (_planksProduced >= PlanksToProducePerDay)
		{
			Unlock();
		}
	}
}
