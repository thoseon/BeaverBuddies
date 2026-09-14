using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.Buildings;
using Timberborn.Persistence;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.ScienceSystem;

public class ScienceNeedingBuilding : TickableComponent, IAwakableComponent, IFinishedStateListener, IPersistentEntity, IFinishedPausable
{
	private static readonly ComponentKey ScienceNeedingBuildingKey = new ComponentKey("ScienceNeedingBuilding");

	private static readonly PropertyKey<float> CurrentScienceKey = new PropertyKey<float>("CurrentScience");

	private readonly IDayNightCycle _dayNightCycle;

	private readonly ScienceService _scienceService;

	private BlockableObject _blockableObject;

	private ScienceNeedingBuildingSpec _scienceNeedingBuildingSpec;

	private float _sciencePerTick;

	private float _currentScience;

	private bool _notEnoughScience;

	public int ScienceUsedPerHour => _scienceNeedingBuildingSpec.ScienceUsedPerHour;

	public float ScienceStoredPercentage => _currentScience / (float)ScienceUsedPerHour;

	public event EventHandler<NotEnoughScienceStateChangedEventArgs> NotEnoughScienceStateChanged;

	public ScienceNeedingBuilding(IDayNightCycle dayNightCycle, ScienceService scienceService)
	{
		_dayNightCycle = dayNightCycle;
		_scienceService = scienceService;
	}

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
		_scienceNeedingBuildingSpec = GetComponent<ScienceNeedingBuildingSpec>();
		_sciencePerTick = _dayNightCycle.FixedDeltaTimeInHours * (float)ScienceUsedPerHour;
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		UpdateNotEnoughScience();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public override void Tick()
	{
		if (_currentScience <= 0f)
		{
			RefillScience();
		}
		if (_currentScience > 0f && _blockableObject.IsUnblocked)
		{
			UseScience();
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(ScienceNeedingBuildingKey).Set(CurrentScienceKey, _currentScience);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ScienceNeedingBuildingKey);
		_currentScience = component.Get(CurrentScienceKey);
	}

	private void RefillScience()
	{
		if (_scienceService.SciencePoints >= ScienceUsedPerHour)
		{
			AddPoints(ScienceUsedPerHour);
		}
		UpdateNotEnoughScience();
	}

	private void AddPoints(int neededScience)
	{
		_scienceService.SubtractPoints(neededScience);
		_currentScience += neededScience;
	}

	private void UseScience()
	{
		_currentScience -= _sciencePerTick;
	}

	private void UpdateNotEnoughScience()
	{
		bool flag = _currentScience <= 0f;
		if (_notEnoughScience != flag)
		{
			_notEnoughScience = flag;
			UpdateBlockableBuilding(flag);
			NotEnoughScienceStateChanged?.Invoke(this, new NotEnoughScienceStateChangedEventArgs(flag));
		}
	}

	private void UpdateBlockableBuilding(bool block)
	{
		if (block)
		{
			_blockableObject.Block(this);
		}
		else
		{
			_blockableObject.Unblock(this);
		}
	}
}
