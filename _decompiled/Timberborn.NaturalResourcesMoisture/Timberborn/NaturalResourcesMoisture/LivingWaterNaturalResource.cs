using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.NaturalResourcesLifecycle;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.NaturalResourcesMoisture;

public class LivingWaterNaturalResource : BaseComponent, IAwakableComponent, IPersistentEntity, IDeletableEntity, IDyingProgressProvider
{
	private static readonly ComponentKey LivingWaterNaturalResourceKey = new ComponentKey("LivingWaterNaturalResource");

	private static readonly PropertyKey<float> DyingProgressKey = new PropertyKey<float>("DyingProgress");

	private static readonly PropertyKey<bool> DeathByFloodingKey = new PropertyKey<bool>("DeathByFlooding");

	private readonly ITimeTriggerFactory _timeTriggerFactory;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private FloodableNaturalResourceSpec _floodableNaturalResourceSpec;

	private LivingNaturalResource _livingNaturalResource;

	private ITimeTrigger _timeTrigger;

	public bool DeathByFlooding { get; private set; }

	public DyingProgress DyingProgress => DyingProgress.Create(_timeTrigger);

	public event EventHandler StartedDying;

	public event EventHandler StoppedDying;

	public LivingWaterNaturalResource(ITimeTriggerFactory timeTriggerFactory, IRandomNumberGenerator randomNumberGenerator)
	{
		_timeTriggerFactory = timeTriggerFactory;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Awake()
	{
		_floodableNaturalResourceSpec = GetComponent<FloodableNaturalResourceSpec>();
		_livingNaturalResource = GetComponent<LivingNaturalResource>();
		_livingNaturalResource.Died += delegate
		{
			StopDying();
		};
		_timeTrigger = _timeTriggerFactory.Create(_livingNaturalResource.Die, GenerateRandomDaysToDie());
		LivingWaterObject component = GetComponent<LivingWaterObject>();
		component.WaterNeedsUnmet += delegate(object _, WaterNeedsUnmetEventArgs e)
		{
			StartDying(e.Flooded);
		};
		component.WaterNeedsMet += delegate
		{
			StopDying();
		};
	}

	public void DeleteEntity()
	{
		_timeTrigger.Reset();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_timeTrigger.Progress != 0f || DeathByFlooding)
		{
			IObjectSaver component = entitySaver.GetComponent(LivingWaterNaturalResourceKey);
			component.Set(DyingProgressKey, _timeTrigger.Progress);
			component.Set(DeathByFloodingKey, DeathByFlooding);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(LivingWaterNaturalResourceKey, out var objectLoader))
		{
			DeathByFlooding = objectLoader.Has(DeathByFloodingKey) && objectLoader.Get(DeathByFloodingKey);
			_timeTrigger.FastForwardProgress(objectLoader.Get(DyingProgressKey));
		}
	}

	private float GenerateRandomDaysToDie()
	{
		return _floodableNaturalResourceSpec.DaysToDie * _randomNumberGenerator.Range(0.9f, 1.1f);
	}

	private void StartDying(bool deathByFlooding)
	{
		if (!_livingNaturalResource.IsDead)
		{
			_timeTrigger.Resume();
			DeathByFlooding = deathByFlooding;
			StartedDying?.Invoke(this, EventArgs.Empty);
		}
	}

	private void StopDying()
	{
		if (_livingNaturalResource.IsDead)
		{
			_timeTrigger.Pause();
		}
		else
		{
			_timeTrigger.Reset();
		}
		StoppedDying?.Invoke(this, EventArgs.Empty);
	}
}
