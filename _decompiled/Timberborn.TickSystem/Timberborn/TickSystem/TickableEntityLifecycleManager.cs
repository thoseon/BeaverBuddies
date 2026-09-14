using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.EntitySystem;
using Timberborn.Metrics;
using Timberborn.SingletonSystem;

namespace Timberborn.TickSystem;

internal class TickableEntityLifecycleManager : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly ITickableBucketService _tickableBucketService;

	private readonly IMetricsService _metricsService;

	private readonly Dictionary<Guid, TickableEntity> _tickableEntities = new Dictionary<Guid, TickableEntity>();

	private readonly List<TickableComponent> _tickableComponentsCache = new List<TickableComponent>();

	public TickableEntityLifecycleManager(EventBus eventBus, ITickableBucketService tickableBucketService, IMetricsService metricsService)
	{
		_eventBus = eventBus;
		_tickableBucketService = tickableBucketService;
		_metricsService = metricsService;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnEntityInitialized(EntityInitializedEvent entityInitializedEvent)
	{
		AddTickableEntity(entityInitializedEvent.Entity);
	}

	[OnEvent]
	public void OnEntityDeleted(EntityDeletedEvent entityDeletedEvent)
	{
		RemoveTickableEntity(entityDeletedEvent.Entity);
	}

	private void AddTickableEntity(EntityComponent entity)
	{
		entity.GetComponents(_tickableComponentsCache);
		if (_tickableComponentsCache.Count > 0)
		{
			IEnumerable<MeteredTickableComponent> tickableComponents = _tickableComponentsCache.OrderBy((TickableComponent tickable) => (tickable is ILateTickable) ? 1 : 0).Select(CreateMeteredComponent);
			TickableEntity tickableEntity = new TickableEntity(entity, tickableComponents, entity.Name);
			_tickableBucketService.AddEntity(tickableEntity);
			_tickableEntities[entity.EntityId] = tickableEntity;
			_tickableComponentsCache.Clear();
		}
	}

	private void RemoveTickableEntity(EntityComponent entity)
	{
		Guid entityId = entity.EntityId;
		if (_tickableEntities.TryGetValue(entityId, out var value))
		{
			_tickableBucketService.RemoveEntity(value);
			_tickableEntities.Remove(entityId);
		}
	}

	private MeteredTickableComponent CreateMeteredComponent(TickableComponent tickableComponent)
	{
		string name = tickableComponent.GetType().Name;
		ITimerMetric timerMetric = _metricsService.GetTimerMetric("Tick", name);
		return new MeteredTickableComponent(tickableComponent, timerMetric, _metricsService.MetricsEnabled);
	}
}
