using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.NaturalResourcesLifecycle;
using Timberborn.Persistence;
using Timberborn.SoilMoistureSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.NaturalResourcesMoisture;

public class AridNaturalResource : BaseComponent, IAwakableComponent, IPersistentEntity, IDeletableEntity, IDyingProgressProvider
{
	private static readonly ComponentKey ComponentKey = new ComponentKey("AridNaturalResource");

	private static readonly PropertyKey<float> DyingProgressKey = new PropertyKey<float>("DyingProgress");

	private readonly ITimeTriggerFactory _timeTriggerFactory;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private LivingNaturalResource _livingNaturalResource;

	private AridNaturalResourceSpec _aridNaturalResourceSpec;

	private ITimeTrigger _timeTrigger;

	public DyingProgress DyingProgress => DyingProgress.Create(_timeTrigger);

	public event EventHandler StartedDying;

	public event EventHandler StoppedDying;

	public AridNaturalResource(ITimeTriggerFactory timeTriggerFactory, IRandomNumberGenerator randomNumberGenerator)
	{
		_timeTriggerFactory = timeTriggerFactory;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Awake()
	{
		_livingNaturalResource = GetComponent<LivingNaturalResource>();
		_livingNaturalResource.Died += delegate
		{
			StopDying();
		};
		_aridNaturalResourceSpec = GetComponent<AridNaturalResourceSpec>();
		DryObject component = GetComponent<DryObject>();
		component.EnteredDryState += delegate
		{
			StopDying();
		};
		component.ExitedDryState += delegate
		{
			StartDying();
		};
		float delayInDays = _aridNaturalResourceSpec.DaysToDieWet * _randomNumberGenerator.Range(0.9f, 1.1f);
		_timeTrigger = _timeTriggerFactory.Create(_livingNaturalResource.Die, delayInDays);
	}

	public void DeleteEntity()
	{
		_timeTrigger.Reset();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_timeTrigger.Progress != 0f)
		{
			entitySaver.GetComponent(ComponentKey).Set(DyingProgressKey, _timeTrigger.Progress);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(ComponentKey, out var objectLoader))
		{
			float progress = objectLoader.Get(DyingProgressKey);
			_timeTrigger.FastForwardProgress(progress);
		}
	}

	private void StartDying()
	{
		if (!_livingNaturalResource.IsDead)
		{
			_timeTrigger.Resume();
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
