using System;
using Timberborn.Forestry;
using Timberborn.NaturalResources;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Achievements;

internal class TreePlantingCounter : ISaveableSingleton, ILoadableSingleton
{
	private static readonly SingletonKey TreePlantingCounterKey = new SingletonKey("TreePlantingCounter");

	private static readonly PropertyKey<int> PlantedCountKey = new PropertyKey<int>("PlantedCount");

	private readonly EventBus _eventBus;

	private readonly ISingletonLoader _singletonLoader;

	private int _plantedCount;

	public event EventHandler<int> CountChanged;

	public TreePlantingCounter(EventBus eventBus, ISingletonLoader singletonLoader)
	{
		_eventBus = eventBus;
		_singletonLoader = singletonLoader;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (_plantedCount > 0)
		{
			singletonSaver.GetSingleton(TreePlantingCounterKey).Set(PlantedCountKey, _plantedCount);
		}
	}

	public void Load()
	{
		_eventBus.Register(this);
		if (_singletonLoader.TryGetSingleton(TreePlantingCounterKey, out var objectLoader))
		{
			_plantedCount = objectLoader.Get(PlantedCountKey);
		}
	}

	[OnEvent]
	public void OnNaturalResourcePlanted(NaturalResourcePlantedEvent naturalResourcePlantedEvent)
	{
		if (naturalResourcePlantedEvent.PlantedResource.HasSpec<TreeComponentSpec>())
		{
			_plantedCount++;
			CountChanged?.Invoke(this, _plantedCount);
		}
	}
}
