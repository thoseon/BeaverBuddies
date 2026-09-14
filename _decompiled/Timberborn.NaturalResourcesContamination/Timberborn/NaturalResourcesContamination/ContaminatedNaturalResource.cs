using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.NaturalResourcesLifecycle;
using Timberborn.Persistence;
using Timberborn.SoilContaminationSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.NaturalResourcesContamination;

public class ContaminatedNaturalResource : BaseComponent, IAwakableComponent, IPersistentEntity, IDeletableEntity, IDyingProgressProvider
{
	private static readonly float MinDaysToDie = 0.2f;

	private static readonly float MaxDaysToDie = 0.3f;

	private static readonly ComponentKey ContaminatedNaturalResourceKey = new ComponentKey("ContaminatedNaturalResource");

	private static readonly PropertyKey<float> DyingProgressKey = new PropertyKey<float>("DyingProgress");

	private readonly ITimeTriggerFactory _timeTriggerFactory;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private LivingNaturalResource _livingNaturalResource;

	private ITimeTrigger _timeTrigger;

	public DyingProgress DyingProgress => DyingProgress.Create(_timeTrigger);

	public event EventHandler StartedDying;

	public event EventHandler StoppedDying;

	public ContaminatedNaturalResource(ITimeTriggerFactory timeTriggerFactory, IRandomNumberGenerator randomNumberGenerator)
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
		ContaminatedObject component = GetComponent<ContaminatedObject>();
		component.EnteredContaminatedState += delegate
		{
			StartDying();
		};
		component.ExitedContaminatedState += delegate
		{
			StopDying();
		};
		_timeTrigger = _timeTriggerFactory.Create(_livingNaturalResource.Die, GetDaysToDie());
	}

	public void DeleteEntity()
	{
		_timeTrigger.Reset();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_timeTrigger.Progress != 0f)
		{
			entitySaver.GetComponent(ContaminatedNaturalResourceKey).Set(DyingProgressKey, _timeTrigger.Progress);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(ContaminatedNaturalResourceKey, out var objectLoader))
		{
			_timeTrigger.FastForwardProgress(objectLoader.Get(DyingProgressKey));
		}
	}

	private float GetDaysToDie()
	{
		return _randomNumberGenerator.Range(MinDaysToDie, MaxDaysToDie);
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
