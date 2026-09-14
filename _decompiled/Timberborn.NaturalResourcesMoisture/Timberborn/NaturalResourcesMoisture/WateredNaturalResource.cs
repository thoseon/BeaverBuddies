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

public class WateredNaturalResource : BaseComponent, IAwakableComponent, IPersistentEntity, IDeletableEntity, IDyingProgressProvider
{
	private static readonly ComponentKey WateredNaturalResourceKey = new ComponentKey("WateredNaturalResource");

	private static readonly PropertyKey<float> DyingProgressKey = new PropertyKey<float>("DyingProgress");

	private readonly ITimeTriggerFactory _timeTriggerFactory;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private LivingNaturalResource _livingNaturalResource;

	private WateredNaturalResourceSpec _wateredNaturalResourceSpec;

	private ITimeTrigger _timeTrigger;

	public DyingProgress DyingProgress => DyingProgress.Create(_timeTrigger);

	public event EventHandler StartedDying;

	public event EventHandler StoppedDying;

	public WateredNaturalResource(ITimeTriggerFactory timeTriggerFactory, IRandomNumberGenerator randomNumberGenerator)
	{
		_timeTriggerFactory = timeTriggerFactory;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Awake()
	{
		_livingNaturalResource = GetComponent<LivingNaturalResource>();
		_livingNaturalResource.Died += delegate
		{
			StopDryingOut();
		};
		_wateredNaturalResourceSpec = GetComponent<WateredNaturalResourceSpec>();
		DryObject component = GetComponent<DryObject>();
		component.EnteredDryState += delegate
		{
			StartDryingOut();
		};
		component.ExitedDryState += delegate
		{
			StopDryingOut();
		};
		_timeTrigger = _timeTriggerFactory.Create(_livingNaturalResource.Die, GenerateRandomDaysToDry());
	}

	public void DeleteEntity()
	{
		_timeTrigger.Reset();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_timeTrigger.Progress != 0f)
		{
			entitySaver.GetComponent(WateredNaturalResourceKey).Set(DyingProgressKey, _timeTrigger.Progress);
		}
	}

	[BackwardCompatible(2025, 2, 28, Compatibility.Map)]
	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(WateredNaturalResourceKey, out var objectLoader))
		{
			float progress = (objectLoader.Has(DyingProgressKey) ? objectLoader.Get(DyingProgressKey) : objectLoader.Get(new PropertyKey<float>("DryingProgress")));
			_timeTrigger.FastForwardProgress(progress);
		}
	}

	private float GenerateRandomDaysToDry()
	{
		return _wateredNaturalResourceSpec.DaysToDieDry * _randomNumberGenerator.Range(0.9f, 1.1f);
	}

	private void StartDryingOut()
	{
		if (!_livingNaturalResource.IsDead)
		{
			_timeTrigger.Resume();
			StartedDying?.Invoke(this, EventArgs.Empty);
		}
	}

	private void StopDryingOut()
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
