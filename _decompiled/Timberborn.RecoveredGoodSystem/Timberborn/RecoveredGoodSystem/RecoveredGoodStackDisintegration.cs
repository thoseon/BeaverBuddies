using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.RecoveredGoodSystem;

public class RecoveredGoodStackDisintegration : BaseComponent, IAwakableComponent, IInitializableEntity, IPersistentEntity, IDeletableEntity
{
	private static readonly ComponentKey RecoveredGoodStackDisintegrationKey = new ComponentKey("RecoveredGoodStackDisintegration");

	private static readonly PropertyKey<float> DisintegrationTimeKey = new PropertyKey<float>("DisintegrationTime");

	private readonly EntityService _entityService;

	private readonly ITimeTriggerFactory _timeTriggerFactory;

	private RecoveredGoodStackDisintegrationSpec _spec;

	private ITimeTrigger _timeTrigger;

	public float DaysToDisintegration => _timeTrigger.DaysLeft;

	public float Progress => _timeTrigger.Progress;

	internal RecoveredGoodStackDisintegration(EntityService entityService, ITimeTriggerFactory timeTriggerFactory)
	{
		_entityService = entityService;
		_timeTriggerFactory = timeTriggerFactory;
	}

	public void Awake()
	{
		_spec = GetComponent<RecoveredGoodStackDisintegrationSpec>();
		_timeTrigger = _timeTriggerFactory.Create(Disintegrate, _spec.DaysToDisintegrate);
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(RecoveredGoodStackDisintegrationKey).Set(DisintegrationTimeKey, _timeTrigger.Progress);
	}

	[BackwardCompatible(2025, 11, 20, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(RecoveredGoodStackDisintegrationKey, out var objectLoader))
		{
			_timeTrigger.FastForwardProgress(objectLoader.Get(DisintegrationTimeKey));
		}
	}

	public void DeleteEntity()
	{
		_timeTrigger.Reset();
	}

	public void InitializeEntity()
	{
		_timeTrigger.Resume();
	}

	private void Disintegrate()
	{
		_entityService.Delete(this);
	}
}
