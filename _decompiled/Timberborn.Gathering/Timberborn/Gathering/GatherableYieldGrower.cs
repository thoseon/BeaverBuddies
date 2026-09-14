using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Growing;
using Timberborn.NaturalResourcesLifecycle;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Gathering;

public class GatherableYieldGrower : BaseComponent, IAwakableComponent, IInitializableEntity, IDeletableEntity, IPersistentEntity
{
	private static readonly ComponentKey GatherableYieldGrowerKey = new ComponentKey("GatherableYieldGrower");

	private static readonly PropertyKey<float> GrowthProgressKey = new PropertyKey<float>("GrowthProgress");

	private readonly ITimeTriggerFactory _timeTriggerFactory;

	private Gatherable _gatherable;

	private LivingNaturalResource _livingNaturalResource;

	private DyingNaturalResource _dyingNaturalResource;

	private Growable _growable;

	private ITimeTrigger _timeTrigger;

	private float _progressWhenDied;

	public float GrowthProgress => _timeTrigger.Progress;

	private bool GrowthIsBlocked
	{
		get
		{
			if (_growable.IsGrown && !_livingNaturalResource.IsDead && !_dyingNaturalResource.IsDying && !_gatherable.Yielder.IsYielding)
			{
				return !_gatherable.UsableWithCurrentFeatureToggles;
			}
			return true;
		}
	}

	public GatherableYieldGrower(ITimeTriggerFactory timeTriggerFactory)
	{
		_timeTriggerFactory = timeTriggerFactory;
	}

	public void Awake()
	{
		_gatherable = GetComponent<Gatherable>();
		_livingNaturalResource = GetComponent<LivingNaturalResource>();
		_dyingNaturalResource = GetComponent<DyingNaturalResource>();
		_growable = GetComponent<Growable>();
		_timeTrigger = _timeTriggerFactory.Create(delegate
		{
			_gatherable.Yielder.ResetYield();
		}, _gatherable.YieldGrowthTimeInDays);
		_gatherable.Gathered += delegate
		{
			RestartGrowth();
		};
		_livingNaturalResource.Died += delegate
		{
			RemoveYield();
		};
		_livingNaturalResource.ReversedDeath += delegate
		{
			ReverseDeath();
		};
		_dyingNaturalResource.StartedDying += delegate
		{
			PauseGrowth();
		};
		_dyingNaturalResource.StoppedDying += delegate
		{
			ResumeGrowth();
		};
		_growable.HasGrown += delegate
		{
			ResumeGrowth();
		};
	}

	public void InitializeEntity()
	{
		ResumeGrowth();
	}

	public void DeleteEntity()
	{
		PauseGrowth();
	}

	public void FastForwardGrowth(float progress)
	{
		_timeTrigger.FastForwardProgress(progress);
		_progressWhenDied = progress;
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_timeTrigger.Progress != 0f)
		{
			entitySaver.GetComponent(GatherableYieldGrowerKey).Set(GrowthProgressKey, _timeTrigger.Progress);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(GatherableYieldGrowerKey, out var objectLoader))
		{
			FastForwardGrowth(objectLoader.Get(GrowthProgressKey));
		}
	}

	private void RestartGrowth()
	{
		_timeTrigger.Reset();
		ResumeGrowth();
	}

	private void ResumeGrowth()
	{
		if (!GrowthIsBlocked)
		{
			_timeTrigger.Resume();
		}
	}

	private void PauseGrowth()
	{
		_timeTrigger.Pause();
	}

	private void RemoveYield()
	{
		if (_timeTrigger.InProgress)
		{
			_progressWhenDied = _timeTrigger.Progress;
		}
		_timeTrigger.Reset();
		_gatherable.Yielder.RemoveRemainingYield();
	}

	private void ReverseDeath()
	{
		if (_growable.IsGrown)
		{
			FastForwardGrowth(_progressWhenDied);
		}
	}
}
