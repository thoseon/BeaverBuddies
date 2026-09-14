using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.EntitySystem;
using Timberborn.NaturalResourcesLifecycle;
using Timberborn.NaturalResourcesLifecycleModelSystem;
using Timberborn.NaturalResourcesReproduction;
using Timberborn.Persistence;
using Timberborn.TerrainSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.Growing;

public class Growable : BaseComponent, IAwakableComponent, IPersistentEntity, IDeletableEntity, IInitializableEntity, IGroundMatterBelowInvalidator
{
	private static readonly ComponentKey GrowableKey = new ComponentKey("Growable");

	private static readonly PropertyKey<float> GrowthProgressKey = new PropertyKey<float>("GrowthProgress");

	private readonly ITimeTriggerFactory _timeTriggerFactory;

	private LivingNaturalResource _livingNaturalResource;

	private DyingNaturalResource _dyingNaturalResource;

	private Reproducible _reproducible;

	private GrowableSpec _growableSpec;

	private ITimeTrigger _timeTrigger;

	private NaturalResourceLifecycleModel _matureModel;

	private NaturalResourceLifecycleModel _seedlingModel;

	public float GrowthTimeInDays => _growableSpec.GrowthTimeInDays;

	public bool IsGrown => _timeTrigger.Finished;

	public bool GrowthInProgress => _timeTrigger.InProgress;

	public float GrowthProgress => _timeTrigger.Progress;

	public event EventHandler HasGrown;

	public Growable(ITimeTriggerFactory timeTriggerFactory)
	{
		_timeTriggerFactory = timeTriggerFactory;
	}

	public void Awake()
	{
		_livingNaturalResource = GetComponent<LivingNaturalResource>();
		_dyingNaturalResource = GetComponent<DyingNaturalResource>();
		_reproducible = GetComponent<Reproducible>();
		_growableSpec = GetComponent<GrowableSpec>();
		_timeTrigger = _timeTriggerFactory.Create(Grow, GrowthTimeInDays);
	}

	public void InitializeEntity()
	{
		GameObject fullModel = GetComponent<BlockObjectModel>().FullModel;
		_matureModel = NaturalResourceLifecycleModel.Create(this, fullModel, "Mature");
		_seedlingModel = NaturalResourceLifecycleModel.Create(this, fullModel, "Seedling");
		if (!IsGrown)
		{
			_reproducible.BlockReproduction(this);
			ResumeGrowing();
		}
		_dyingNaturalResource.StartedDying += delegate
		{
			PauseGrowing();
		};
		_dyingNaturalResource.StoppedDying += delegate
		{
			ResumeGrowing();
		};
		_livingNaturalResource.Died += delegate
		{
			PauseGrowing();
		};
		_livingNaturalResource.ReversedDeath += delegate
		{
			ResumeGrowing();
		};
	}

	public void DeleteEntity()
	{
		PauseGrowing();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_timeTrigger.Progress < 1f)
		{
			entitySaver.GetComponent(GrowableKey).Set(GrowthProgressKey, _timeTrigger.Progress);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(GrowableKey, out var objectLoader))
		{
			_timeTrigger.FastForwardProgress(objectLoader.Get(GrowthProgressKey));
		}
		else
		{
			_timeTrigger.FastForwardProgress(1f);
		}
	}

	public void IncreaseGrowthProgress(float growthProgress)
	{
		_timeTrigger.FastForwardProgress(growthProgress);
	}

	public void ShowSeedlingModel()
	{
		_matureModel.Hide();
		_seedlingModel.Show();
	}

	public void ShowMatureModel()
	{
		_matureModel.Show();
		_seedlingModel.Hide();
	}

	public void HideModel()
	{
		_matureModel.Hide();
		_seedlingModel.Hide();
	}

	private void ResumeGrowing()
	{
		if (!IsGrown && !_livingNaturalResource.IsDead && !_dyingNaturalResource.IsDying)
		{
			_timeTrigger.Resume();
		}
	}

	private void PauseGrowing()
	{
		_timeTrigger.Pause();
	}

	private void Grow()
	{
		_reproducible.UnblockReproduction(this);
		HasGrown?.Invoke(this, EventArgs.Empty);
	}
}
