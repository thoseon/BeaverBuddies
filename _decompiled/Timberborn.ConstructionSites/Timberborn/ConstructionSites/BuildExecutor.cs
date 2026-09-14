using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.BlockSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.WorkSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.ConstructionSites;

public class BuildExecutor : BaseComponent, IAwakableComponent, IExecutor
{
	private static readonly float MaxBuildTimeInHours = 0.5f;

	private static readonly ComponentKey BuildExecutorKey = new ComponentKey("BuildExecutor");

	private static readonly PropertyKey<float> FinishTimestampKey = new PropertyKey<float>("FinishTimestamp");

	private readonly IDayNightCycle _dayNightCycle;

	private CharacterAnimator _characterAnimator;

	private Builder _builder;

	private Worker _worker;

	private CharacterModel _characterModel;

	private ConstructionSite _constructionSite;

	private float _finishTimestamp;

	private bool _constructionSiteHasSlots;

	public BuildExecutor(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		_characterAnimator = GetComponent<CharacterAnimator>();
		_builder = GetComponent<Builder>();
		_worker = GetComponent<Worker>();
		_characterModel = GetComponent<CharacterModel>();
	}

	public ExecutorStatus Tick(float deltaTimeInHours)
	{
		if (!_constructionSite || !_constructionSite.IsOn)
		{
			StopBuilding();
			return ExecutorStatus.Failure;
		}
		if (!_constructionSite.HasMaterialsToResumeBuilding || _dayNightCycle.PartialDayNumber > _finishTimestamp)
		{
			StopBuilding();
			return ExecutorStatus.Success;
		}
		_constructionSite.IncreaseBuildTime(deltaTimeInHours * _worker.WorkingSpeedMultiplier);
		return ExecutorStatus.Running;
	}

	public void Save(IEntitySaver entitySaver)
	{
		if ((bool)_constructionSite)
		{
			entitySaver.GetComponent(BuildExecutorKey).Set(FinishTimestampKey, _finishTimestamp);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(BuildExecutorKey, out var objectLoader))
		{
			_finishTimestamp = objectLoader.Get(FinishTimestampKey);
		}
	}

	public bool Launch(ConstructionSite constructionSite)
	{
		if (constructionSite.HasMaterialsToResumeBuilding)
		{
			Initialize(constructionSite);
			LookTowardConstructionSite();
			_finishTimestamp = _dayNightCycle.DayNumberHoursFromNow(MaxBuildTimeInHours);
			return true;
		}
		return false;
	}

	internal void InitializeAfterLoad(ConstructionSite constructionSite)
	{
		if ((bool)constructionSite && _finishTimestamp > 0f)
		{
			Initialize(constructionSite);
		}
	}

	private void Initialize(ConstructionSite constructionSite)
	{
		_constructionSite = constructionSite;
		_constructionSiteHasSlots = constructionSite.GetComponent<ConstructionSiteSlotManager>();
		ToggleBuildingAnimation(value: true);
	}

	private void ToggleBuildingAnimation(bool value)
	{
		if (!_constructionSiteHasSlots)
		{
			_characterAnimator.SetBool("Building", value);
		}
	}

	private void LookTowardConstructionSite()
	{
		Vector3 worldCenter = _constructionSite.GetComponent<BlockObjectCenter>().WorldCenter;
		_characterModel.LookToward(worldCenter);
	}

	private void StopBuilding()
	{
		_builder.Unreserve();
		ToggleBuildingAnimation(value: false);
	}
}
